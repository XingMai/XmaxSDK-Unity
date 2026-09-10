using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 隔离厂商 RTC 类型的传输接口，只负责入房、主流视频和消息传输。
    /// </summary>
    internal interface IRtcManager
    {
        /// <summary>
        /// 入房等待之外的引擎或房间致命错误。
        /// </summary>
        event Action<XmaxException> FatalError;

        /// <summary>
        /// 当前房间的本地网络质量统计更新。
        /// </summary>
        event Action<RtcNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 当前房间的远端用户发布包含视频的流时触发。
        /// </summary>
        event Action<RemoteStream> VideoPublished;

        /// <summary>
        /// 当前房间的远端用户取消发布视频时触发。
        /// </summary>
        event Action<RemoteStream> VideoUnpublished;

        /// <summary>
        /// 当前房间主流收到 SEI 数据时触发。
        /// </summary>
        event Action<RemoteStream, byte[]> SeiReceived;

        /// <summary>
        /// 当前房间远端主流收到可转换的 I420 帧时触发。
        /// </summary>
        event Action<RemoteStream, XmaxVideoFrame> FrameReceived;

        /// <summary>
        /// 校验当前运行平台支持原生 RTC；目前仅支持 Android Player。
        /// </summary>
        void ValidatePlatform();

        /// <summary>
        /// 获取独占引擎租约并加入 RTC 房间，成功后发布外部视频；失败或取消时释放资源。
        /// </summary>
        /// <param name="info">经过校验的 RTC 应用、房间、用户及入房令牌。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>房间加入完成并已请求发布视频的任务。</returns>
        Task JoinAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken cancellationToken);

        /// <summary>
        /// 取消入房等待，解除回调并尽力销毁房间、引擎和独占租约。
        /// </summary>
        /// <returns>本地 RTC 资源清理流程已结束的任务。</returns>
        Task LeaveAsync();

        /// <summary>
        /// 为当前房间的指定远端用户设置 I420 视频接收并订阅主流。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        void SubscribeVideo(RemoteStream key);

        /// <summary>
        /// 校验并向已加入房间的原生引擎推送外部视频帧。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        void PushFrame(XmaxVideoFrame frame);

        /// <summary>
        /// 向当前 RTC 房间发送业务消息，负返回值转换为 SDK 错误。
        /// </summary>
        /// <param name="message">按房间协议编码的业务消息文本。</param>
        void SendRoomMessage(string message);

        /// <summary>
        /// 向当前主流发送 SEI 数据，原生调用失败时抛出 SDK 错误。
        /// </summary>
        /// <param name="data">用于传输或匹配的 SEI 字节数据。</param>
        void SendSei(byte[] data);
    }
}
