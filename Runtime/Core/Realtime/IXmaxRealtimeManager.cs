using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 实时 Camera 流的公开入口，负责本地输入、连接、生成和资源清理；在创建管理器的 Unity 主线程调用。
    /// </summary>
    /// <remarks>
    /// 应在 Unity 主线程创建管理器并调用其 API。异步操作使用 Unity 同步上下文恢复，清理期间不接纳新操作。
    /// </remarks>
    public interface IXmaxRealtimeManager
    {
        /// <summary>
        /// 当前管理器使用的模型配置。
        /// </summary>
        RealtimeConfiguration Options { get; }

        /// <summary>
        /// 最近提交的连接和生成状态快照。
        /// </summary>
        RealtimeState CurrentState { get; }

        /// <summary>
        /// 当前管理器有效的本地流；尚未创建或已关闭时为 null。
        /// </summary>
        RealtimeMediaStream LocalStream { get; }

        /// <summary>
        /// 连接或生成状态变化时触发；订阅者异常不会中断 SDK 流程。
        /// </summary>
        event Action<RealtimeState> StateChanged;

        /// <summary>
        /// 当前生成收到有效远端帧时触发；跨回调保留帧需先 Clone。
        /// </summary>
        event Action<XmaxVideoFrame> RemoteFrameReceived;

        /// <summary>
        /// 异步致命错误完成清理后触发；被 await 的操作失败直接通过异常返回。
        /// </summary>
        event Action<XmaxException> ErrorOccurred;

        /// <summary>
        /// 服务端会话关闭失败时触发的可恢复警告，本地连接资源仍已清理。
        /// </summary>
        event Action<XmaxException> CleanupWarning;

        /// <summary>
        /// 有效连接中的本地上下行网络质量更新。
        /// </summary>
        event Action<RealtimeNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 使用当前模型的默认 Camera 编码规格创建本地输入流，可在未连接时预览。
        /// </summary>
        /// <returns>采用模型默认宽高和帧率的本地流；宿主仍需持续推送 Camera 帧。</returns>
        /// <exception cref="XmaxException">自定义模型没有默认规格，或当前状态不允许替换本地流。</exception>
        RealtimeMediaStream CreateLocalExternalStream();

        /// <summary>
        /// 创建或替换本地 Camera 输入流，可在未连接时预览；连接或异步操作期间不可替换。
        /// </summary>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零；Pro 须精确匹配固定分辨率。</param>
        /// <returns>属于此管理器的新本地流，旧本地流随即失效。</returns>
        /// <exception cref="XmaxException">编码格式无效，或当前线程、连接、操作状态不允许替换本地流。</exception>
        RealtimeMediaStream CreateLocalExternalStream(RealtimeVideoFormat format);

        /// <summary>
        /// 创建服务端会话并加入 RTC 房间；失败或取消时回滚连接资源。
        /// </summary>
        /// <param name="localStream">由当前管理器创建且仍有效的本地外部视频流。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>可接收生成视频的远端流，此时尚未启动生成。</returns>
        /// <exception cref="XmaxException">配置、调用状态或平台无效，或会话与 RTC 连接失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次连接。</exception>
        Task<RealtimeMediaStream> ConnectAsync(
            RealtimeMediaStream localStream,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建服务端会话并加入 RTC 房间；失败或取消时回滚连接资源。
        /// </summary>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>可接收生成视频的远端流，此时尚未启动生成。</returns>
        /// <remarks>
        /// 此重载会创建或替换管理器的本地流；已有连接时不能重复连接。
        /// </remarks>
        /// <exception cref="XmaxException">配置、调用状态或平台无效，或会话与 RTC 连接失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次连接。</exception>
        Task<RealtimeMediaStream> ConnectAsync(
            RealtimeVideoFormat format,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 在已连接状态下直接向 RTC 推送一帧 Camera 数据。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        /// <remarks>
        /// 此入口直接发布至 RTC；需要本地预览时，通过 LocalStream.PushVideoFrame 输入。
        /// </remarks>
        /// <exception cref="XmaxException">未连接、正在清理、线程不符合要求，或帧数据无效。</exception>
        /// <exception cref="ArgumentNullException">允许推帧时传入的 frame 为 null。</exception>
        void PushVideoFrame(XmaxVideoFrame frame);

        /// <summary>
        /// 向当前生成任务发送编码画面内的交互轨迹，非生成状态不能发送有效轨迹。
        /// </summary>
        /// <param name="tracks">以编码画面左上角为原点的交互点序列，坐标须位于画面内。</param>
        void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks);

        /// <summary>
        /// 启动或更新生成；首次启动等待任务确认和远端首帧，已有生成任务时更新条件。
        /// </summary>
        /// <param name="context">生成条件；首次生成必须提供，后续可传 null 复用最近一次成功条件。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>首次生成的首帧已就绪，或当前任务条件已更新的任务。</returns>
        /// <exception cref="XmaxException">没有可复用的首次生成条件、调用状态无效，或生成确认、首帧等待失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次启动。</exception>
        Task StartGenerationAsync(RealtimeContext context = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用本管理器的本地流按需连接，再启动或更新生成；整个调用占用同一个操作租约。
        /// </summary>
        /// <param name="localStream">由当前管理器创建且仍有效的本地外部视频流。</param>
        /// <param name="context">生成条件；首次生成必须提供，后续可传 null 复用最近一次成功条件。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>当前连接的远端流；首次启动在任务确认及首帧就绪后返回。</returns>
        /// <remarks>
        /// 连接阶段失败会回滚连接；连接成功后的生成阶段失败保留连接，便于重新生成。
        /// </remarks>
        /// <exception cref="XmaxException">没有可复用的首次生成条件、调用状态无效，或生成确认、首帧等待失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次启动。</exception>
        Task<RealtimeMediaStream> StartGenerationAsync(
            RealtimeMediaStream localStream,
            RealtimeContext context,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 停止生成并保留已有连接和成功条件；独立连接操作进行中时不取消该连接。
        /// </summary>
        /// <returns>生成清理已完成的任务；一步式启动仍在连接阶段时同时清理该连接。</returns>
        Task StopGenerationAsync();

        /// <summary>
        /// 停止生成并清理 RTC 和服务端会话，保留本地流用于预览或重新连接。
        /// </summary>
        /// <returns>连接清理结束的任务，重复请求合并处理。</returns>
        Task DisconnectAsync();

        /// <summary>
        /// 断开连接并使本地流失效；管理器可重新创建本地流后继续使用。
        /// </summary>
        /// <returns>全部清理结束的任务，重复请求合并处理。</returns>
        Task CloseAsync();
    }
}
