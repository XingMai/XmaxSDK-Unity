# XmaxSDK

`ai.xmax.sdk` 是供 Xmax Unity 应用使用的实时视频 SDK。

当前版本集成火山引擎 RTC Unity SDK 3.58.1，支持 Android（含 PICO 的 ARM64 Android 运行环境）。

## 职责边界

- SDK 不依赖 PICO SDK，也不访问摄像头。
- 当前只围绕 Camera 场景开发：宿主负责采集、处理相机画面，再以 RGBA、BGRA 或 I420 帧传给 SDK。暂不提供 Video / Image 媒体源 API。
- SDK 负责创建 Xmax Session、加入 RTC 房间、发布宿主视频帧、订阅远端视频、生成控制、心跳和退出清理。
- 内部按 iOS 的 Core / Service / Media / Stream / Render / Foundation 分层；详见 [架构说明](ARCHITECTURE.md)。
- SDK 不包含文件上传或图片上传业务。

## PicoDemo 依赖

在 `PicoDemo/Packages/manifest.json` 中使用本地 UPM 依赖：

```json
"ai.xmax.sdk": "file:../../XmaxSDK"
```

## 最小接入示例

```csharp
using System.Threading.Tasks;
using Xmax.SDK;

public sealed class XmaxHost
{
    private XmaxRealtimeManager _realtime;

    public async Task ConnectAsync(string apiKey)
    {
        _realtime = new XmaxRealtimeManager(apiKey);

        // 设置 RTC 编码输出尺寸；外部输入帧可以使用不同尺寸，由 RTC 内部裁切。
        await _realtime.ConnectAsync(1280, 720, 30);

        _realtime.RemoteFrameReceived += remoteI420Frame =>
        {
            // 宿主在这里将远端 I420 帧上传到纹理或交给自己的渲染管线。
        };
    }

    // 由宿主自己的相机/图像处理管线逐帧调用。
    public void PushProcessedRgba(byte[] rgba1280x1280, long timestampMicroseconds)
    {
        _realtime.PushRgbaFrame(
            rgba1280x1280,
            1280,
            1280,
            timestampMicroseconds,
            1280 * 4);
    }

    public Task StartGenerationAsync(string prompt)
    {
        return _realtime.StartGenerationAsync(prompt);
    }

    public Task StopGenerationAsync() => _realtime.StopGenerationAsync();

    public Task DisconnectAsync() => _realtime.DisconnectAsync();
}
```

`ConnectAsync` 和 RTC 运行只支持 Android Player；在 Unity Editor 中调用会返回 `NotSupported`。公共模型和项目脚本仍可在 Editor 中正常编译。

## 生命周期与 iOS 对齐

创建 Manager、调用 API 和处理事件均在 Unity 主线程执行。请 `await` 异步 API，避免使用 `.Wait()` 或 `.Result` 阻塞 Unity 主线程。

- `ConnectAsync`：建立 Session 与 RTC 连接；同时只允许一个连接或生成操作，清理期间拒绝新操作。
- `StartGenerationAsync(context)`：首次生成需要 context；等待任务 SEI 匹配和首个有效远端帧后进入 `Generating`。
- `StartGenerationAsync(localStream, context)`：使用本 Manager 的本地流，按需连接并开始生成，返回远端流；已经连接时复用连接，正在生成时复用或更新任务。显式传入 `null` context 可复用之前成功的条件。
- 生成中再次传入 context：更新当前任务条件；不传 context：复用当前任务，不再重启。
- `StopGenerationAsync`：取消待完成的生成或停止当前任务，保留连接与最近成功的 context。
- `DisconnectAsync`：取消进行中的操作，停止心跳、释放 RTC、关闭 Session；保留本地流，可继续本地预览和重新连接。
- `CloseAsync`：完成断开并关闭本地流。旧流失效，Manager 可重新创建本地流使用。

创建 Manager 和本地相机预览流不校验 API Key，也不会创建在线 Session。连接阶段才校验 Key 和服务地址。本地预览可在 Key 尚未配置时工作。

一键生成的连接和生成阶段共享一次操作，期间拒绝其他连接／生成操作。连接阶段失败会清理 Session；连接成功后生成启动失败则保留连接和本地预览，便于重试。断开和关闭仍可取消整个操作。

连接清理时会发出新增状态 `Disconnecting`。重复断开共享同一清理任务；旧操作完成后不会覆盖新连接。`StateChanged` 监听器抛出的异常不会中断 SDK 清理，开启业务日志后会记录其错误类型。

异步致命错误通过 `ErrorOccurred` 在清理后通知，`Severity` 为 `Fatal`；直接调用失败由返回的 Task 抛出。关闭服务端 Session 失败通过 `CleanupWarning` 报告，客户端资源仍然释放。`NetworkQualityChanged` 提供与 iOS 一致的 `Uplink` / `Downlink` 质量等级，并保留丢包率、RTT 和带宽统计。

## 显式本地流与通用渲染

```csharp
var client = new XmaxClient(new XmaxConfiguration(apiKey, XmaxEnvironment.China));
IXmaxRealtimeManager manager = client.CreateRealtimeManager(
    new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)));
var format = client.CreateMediaService().RecommendVideoFormat(
    Models.Realtime(RealtimeModel.X2_0), 1920, 1080);
var local = manager.CreateLocalExternalStream(format);
local.VideoTrack.FrameReceived += frame => { /* 可选本地预览 */ };

// 先启动宿主相机，每帧在 Unity 主线程调用：
// local.PushVideoFrame(XmaxVideoFrame.CreateRgba(rgba, width, height));
// 相机必须在等待生成期间持续送帧。
var remote = await manager.StartGenerationAsync(local,
    new RealtimeContext("A watercolor landscape"));

// i420Material 的 shader 需采样 _YTex、_UTex、_VTex。
// 可复用宿主的 PICO 门户材质；SDK 负责步长处理、纹理上传和尺寸变化。
var texture = new XmaxVideoTexture();
texture.Bind(remote.VideoTrack, i420Material);

// 后续显式 context 更新当前任务，null 复用当前任务。
await manager.StartGenerationAsync(local, null);
await manager.CloseAsync();
texture.Dispose();
```

推荐编码尺寸沿用 iOS x2.0 的 600,000–1,280,000 像素、32 对齐规则，默认 24 fps。显式 `ConnectAsync(1280, 720, 30)` 仍保留，外部输入帧尺寸可以与编码尺寸不同。自定义模型需提供明确尺寸，不套用 x2.0 的建议。

内置模型包括 `RealtimeModel.X2_0`（`"x2.0"`）和 `RealtimeModel.X2_0_Pro`（`"x2.0-pro"`），默认仍为 `X2_0`。选择 Pro 时使用显式编码格式；参考 iOS 的 Pro 模型约定，宽高使用 `1024 × 1920` 或 `1920 × 1024`，默认帧率为 30 fps。当前 Unity 的 `RecommendVideoFormat` 和 `GetCapabilities` 仅支持 `x2.0`。

```csharp
var proManager = client.CreateRealtimeManager(
    new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0_Pro)));
var proLocal = proManager.CreateLocalExternalStream(
    new RealtimeVideoFormat(1024, 1920, 30));
// 与上例相同，在等待生成期间持续向 proLocal 推送 Camera 帧。
```

交互使用 `SendTracks` 的编码像素坐标；`InteractionCoordinateMapper.TryMap` 可将 Fit/Fill 视口坐标映射为编码像素，统一采用左上角原点。Unity 屏幕坐标通常以左下角为原点，宿主应先转换 Y；相机旋转或镜像也应先反向映射。纹理上传器暴露 `Rotation`，材质的 UV 旋转、YUV 色彩转换和视觉效果由宿主处理。

视频帧包装现有数组，不复制像素。调用期间不要修改输入数据；若需跨回调保留并修改帧，使用 `frame.Clone()`。长期不使用的 Manager 应调用 `CloseAsync`，纹理上传器应调用 `Dispose`。

## 质量与日志

```csharp
var configuration = new XmaxConfiguration(apiKey, XmaxEnvironment.China,
    XmaxLoggerOption.Business | XmaxLoggerOption.Performance);
var client = new XmaxClient(configuration);
var manager = client.CreateRealtimeManager(
    new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)));
manager.NetworkQualityChanged += quality =>
    UnityEngine.Debug.Log($"Up: {quality.Uplink}, Down: {quality.Downlink}");
```

日志选项与 iOS 一致：`None`（默认关闭）、`Business`（API、连接、生成和错误）、`Performance`（网络质量和启动耗时）、`All`。配置为 SDK 全局选项，仅在 Client 初始化时更新；从已有 Client 创建 Manager 不会覆盖较新 Client 的配置。直接使用 `new XmaxRealtimeManager(...)` 的兼容入口会创建一个 Client 来应用配置。第三方 RTC 和 Unity 自身的日志不受此开关控制，日志开关不影响错误事件或 Task 异常。

内部调用与 iOS 的分类实例对应，例如 `XmaxLogger.Realtime.Info(() => "Connected")`、`XmaxLogger.Rtc.Warn(() => "Limited", XmaxLoggerOption.Performance)`。`Debug / Info / Warn / Error` 均支持相同的分类选项和延迟消息求值。Unity Console 的 Debug 和 Info 均输出为普通 Log，Warn 和 Error 分别输出为 Warning 和 Error。

启动耗时使用单调时钟，覆盖 Session 创建、RTC 进房、连接、开始信令、SEI 匹配和首帧就绪；失败记录停留阶段，更新已运行任务不会重复计算启动耗时。直接连接后再生成只统计生成阶段，一键生成包含连接阶段。

API 日志只输出请求方法、路由模板、HTTP 状态、耗时和响应字节数。不会输出 API Key、Token、Session ID、prompt、响应正文或任意异常正文。

质量等级包含 `Unknown / Excellent / Good / Poor / Bad / VeryBad / Down`。当前 Unity RTC 包定义到 `VeryBad`，未知厂商值映射为 `Unknown`，不推测 `Down`；该包也未暴露 iOS 的性能限制／恢复告警回调，因此当前未提供性能告警事件。

## 测试

`Tests/Editor` 包含无需真实 API key 或 RTC 设备的回归测试。消费工程安装 `com.unity.test-framework` 1.6.0，并在 `Packages/manifest.json` 添加 `"testables": ["ai.xmax.sdk"]` 后，可以通过 Unity Test Runner 的 EditMode 执行。

本地 `.cicd/validate.sh` 自动运行这些测试、最小消费端 Editor 检查和 Android IL2CPP/ARM64 构建。真实 RTC 收发、PICO 相机输入和网络重连仍需真机联调。
