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
