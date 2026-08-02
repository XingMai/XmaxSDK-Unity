# XmaxSDK

`ai.xmax.sdk` 是供 Xmax Unity 应用使用的实时视频 SDK。

当前版本集成火山引擎 RTC Unity SDK 3.58.1，支持 Android（含 PICO 的 ARM64 Android 运行环境）。

## 职责边界

- SDK 不依赖 PICO SDK，也不访问摄像头。
- 宿主负责采集、处理视频画面，再以 RGBA、BGRA 或 I420 帧传给 SDK。
- SDK 负责创建 Xmax Session、加入 RTC 房间、发布宿主视频帧、订阅远端视频、生成控制、心跳和退出清理。
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
    private RealtimeMediaStream _remoteStream;

    public async Task ConnectAsync(string apiKey)
    {
        var client = new XmaxClient(new XmaxConfiguration(apiKey));
        _realtime = client.CreateRealtimeManager(
            new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)));

        // 尺寸必须与之后传入的宿主帧一致。
        _remoteStream = await _realtime.ConnectAsync(
            new RealtimeVideoFormat(1280, 720, 30));

        _remoteStream.VideoTrack.FrameReceived += remoteI420Frame =>
        {
            // 宿主在这里将远端 I420 帧上传到纹理或交给自己的渲染管线。
        };
    }

    // 由宿主自己的相机/图像处理管线逐帧调用。
    public void PushProcessedRgba(byte[] rgba, long timestampMicroseconds)
    {
        var frame = XmaxVideoFrame.CreateRgba(
            rgba,
            1280,
            720,
            1280 * 4,
            timestampMicroseconds);
        _realtime.PushVideoFrame(frame);
    }

    public Task StartGenerationAsync(string prompt)
    {
        return _realtime.StartGenerationAsync(new RealtimeContext(prompt));
    }

    public Task StopGenerationAsync() => _realtime.StopGenerationAsync();

    public Task DisconnectAsync() => _realtime.DisconnectAsync();
}
```

`ConnectAsync` 和 RTC 运行只支持 Android Player；在 Unity Editor 中调用会返回 `NotSupported`。公共模型和项目脚本仍可在 Editor 中正常编译。
