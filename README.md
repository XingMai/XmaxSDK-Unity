# XmaxSDK

`ai.xmax.sdk` 是供 Xmax Unity 应用使用的实时视频 SDK。

当前版本集成火山引擎 RTC Unity SDK 3.58.1，支持 Android（含 PICO 的 ARM64 Android 运行环境）。

## 职责边界

- SDK 不依赖 PICO SDK，也不访问摄像头。
- 宿主负责采集、处理视频画面，再以 RGBA、BGRA 或 I420 帧传给 SDK。
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
- 生成中再次传入 context：更新当前任务条件；不传 context：复用当前任务，不再重启。
- `StopGenerationAsync`：取消待完成的生成或停止当前任务，保留连接与最近成功的 context。
- `DisconnectAsync`：取消进行中的操作，停止心跳、释放 RTC、关闭 Session；保留本地流，可继续本地预览和重新连接。
- `CloseAsync`：完成断开并关闭本地流。旧流失效，Manager 可重新创建本地流使用。

连接清理时会发出新增状态 `Disconnecting`。重复断开共享同一清理任务；旧操作完成后不会覆盖新连接。`StateChanged` 监听器抛出的异常会记录到 Unity 日志，不会中断 SDK 清理。

异步致命错误通过 `ErrorOccurred` 在清理后通知，`Severity` 为 `Fatal`；直接调用失败由返回的 Task 抛出。关闭服务端 Session 失败通过 `CleanupWarning` 报告，客户端资源仍然释放。`NetworkQualityChanged` 提供丢包率、RTT 和带宽统计。

## 显式本地流与通用渲染

```csharp
var client = new XmaxClient(new XmaxConfiguration(apiKey, XmaxEnvironment.China));
IXmaxRealtimeManager manager = client.CreateRealtimeManager(
    new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)));
var format = client.CreateMediaService().RecommendVideoFormat(
    Models.Realtime(RealtimeModel.X2_0), 1920, 1080);
var local = manager.CreateLocalExternalStream(format);
local.VideoTrack.FrameReceived += frame => { /* 可选本地预览 */ };
var remote = await manager.ConnectAsync(local);

// i420Material 的 shader 需采样 _YTex、_UTex、_VTex。
// 可复用宿主的 PICO 门户材质；SDK 负责步长处理、纹理上传和尺寸变化。
var texture = new XmaxVideoTexture();
texture.Bind(remote.VideoTrack, i420Material);

// 相机每帧调用；本地流断开后仍可接收输入用于预览。
local.PushVideoFrame(XmaxVideoFrame.CreateRgba(rgba, width, height));
await manager.StartGenerationAsync(new RealtimeContext("A watercolor landscape"));
await manager.CloseAsync();
texture.Dispose();
```

推荐编码尺寸沿用 iOS x2.0 的 600,000–1,280,000 像素、32 对齐规则，默认 24 fps。显式 `ConnectAsync(1280, 720, 30)` 仍保留，外部输入帧尺寸可以与编码尺寸不同。自定义模型需提供明确尺寸，不套用 x2.0 的建议。

交互使用 `SendTracks` 的编码像素坐标；`InteractionCoordinateMapper.TryMap` 可将 Fit/Fill 视口坐标映射为编码像素，统一采用左上角原点。Unity 屏幕坐标通常以左下角为原点，宿主应先转换 Y；相机旋转或镜像也应先反向映射。纹理上传器暴露 `Rotation`，材质的 UV 旋转、YUV 色彩转换和视觉效果由宿主处理。

视频帧包装现有数组，不复制像素。调用期间不要修改输入数据；若需跨回调保留并修改帧，使用 `frame.Clone()`。长期不使用的 Manager 应调用 `CloseAsync`，纹理上传器应调用 `Dispose`。

## 测试

`Tests/Editor` 包含无需真实 API key 或 RTC 设备的回归测试。消费工程安装 `com.unity.test-framework` 1.6.0，并在 `Packages/manifest.json` 添加 `"testables": ["ai.xmax.sdk"]` 后，可以通过 Unity Test Runner 的 EditMode 执行。

本地 `.cicd/validate.sh` 自动运行这些测试、最小消费端 Editor 检查和 Android IL2CPP/ARM64 构建。真实 RTC 收发、PICO 相机输入和网络重连仍需真机联调。
