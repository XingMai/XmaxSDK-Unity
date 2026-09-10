using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理实时房间协议、任务确认和视频传输的内部接口。
    /// </summary>
    internal interface IStreamController
    {
        /// <summary>
        /// 有效连接中的传输、任务确认或后台循环发生致命失败时触发。
        /// </summary>
        event Action<XmaxException> FatalError;

        /// <summary>
        /// 当前有效连接的公开网络质量统计更新。
        /// </summary>
        event Action<RealtimeNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 收到已通过当前任务 SEI 确认的远端视频帧时触发。
        /// </summary>
        event Action<XmaxVideoFrame> FrameReceived;

        /// <summary>
        /// 在创建在线会话前校验底层 RTC 的运行平台。
        /// </summary>
        void ValidatePlatform();

        /// <summary>
        /// 加入 RTC 房间并启动房间心跳，失败或取消时清理本次连接。
        /// </summary>
        /// <param name="info">经过校验的 RTC 应用、房间、用户及入房令牌。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>入房成功且心跳已启动的任务。</returns>
        Task ConnectAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken cancellationToken);

        /// <summary>
        /// 停止房间心跳和生成后台循环，再退出 RTC 房间。
        /// </summary>
        /// <returns>全部房间循环和 RTC 清理已结束的任务。</returns>
        Task DisconnectAsync();

        /// <summary>
        /// 发送启动信令并周期发送任务 SEI，等待远端匹配确认，失败时停止本次生成。
        /// </summary>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>远端 SEI 已确认的任务标识；首帧就绪由渲染层另行等待。</returns>
        Task<string> StartGenerationAsync(
            RealtimeContext context,
            RealtimeVideoFormat format,
            CancellationToken cancellationToken);

        /// <summary>
        /// 校验任务归属后发送生成条件更新信令。
        /// </summary>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        void ChangeGenerationCondition(RealtimeContext context, RealtimeVideoFormat format, string taskId);

        /// <summary>
        /// 使任务和匹配流失效，停止 SEI 循环并尽力发送停止信令。
        /// </summary>
        /// <returns>生成后台循环和停止信令处理已结束的任务。</returns>
        Task StopGenerationAsync();

        /// <summary>
        /// 将本地外部帧交给 RTC 传输层。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        void PushFrame(XmaxVideoFrame frame);

        /// <summary>
        /// 校验当前任务标识后发送交互轨迹消息。
        /// </summary>
        /// <param name="tracks">以编码画面左上角为原点的交互点序列，坐标须位于画面内。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks, string taskId);
    }
}
