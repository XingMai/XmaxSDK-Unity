using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理服务端会话、RTC 连接及会话心跳，并负责连接失败回滚。
    /// </summary>
    internal sealed class XmaxRealtimeConnectionManager
    {
        private readonly IRealtimeSessionService _sessions;
        private readonly IStreamController _stream;
        private readonly RenderController _render;
        private readonly SessionHeartbeat _heartbeat;
        private readonly RealtimeTiming _timing;

        /// <summary>
        /// 已成功连接的会话；未连接或已开始断开时为 null。
        /// </summary>
        internal XmaxSession ActiveSession { get; private set; }

        /// <summary>
        /// 当前连接的远端媒体流；未连接时为 null。
        /// </summary>
        internal RealtimeMediaStream CurrentRemoteStream { get; private set; }

        /// <summary>
        /// 会话心跳出现需要终止连接的失败时触发。
        /// </summary>
        internal event Action<XmaxException> FatalError;

        /// <summary>
        /// 服务端会话关闭失败时报告可恢复的清理警告。
        /// </summary>
        internal event Action<XmaxException> CleanupError;

        /// <summary>
        /// 注入会话、传输、渲染及可选计时依赖。
        /// </summary>
        /// <param name="sessions">实时会话创建、续期和关闭服务。</param>
        /// <param name="stream">当前管理器的房间信令和视频传输入口。</param>
        /// <param name="render">当前管理器的远端轨道及首帧就绪控制器。</param>
        /// <param name="timing">共享启动计时器；为 null 时不记录连接阶段耗时。</param>
        internal XmaxRealtimeConnectionManager(
            IRealtimeSessionService sessions,
            IStreamController stream,
            RenderController render,
            RealtimeTiming timing = null)
        {
            _sessions = sessions;
            _stream = stream;
            _render = render;
            _timing = timing;
            _heartbeat = new SessionHeartbeat(sessions);
            _heartbeat.Failed += exception => EventDispatch.Raise(FatalError, exception);
        }

        /// <summary>
        /// 创建会话并加入房间，成功后启动心跳；失败或取消时回滚已创建资源。
        /// </summary>
        /// <param name="model">服务端实时模型名称，例如 x2.0。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="token">当前操作或循环的取消令牌。</param>
        /// <returns>连接成功后的远端媒体流。</returns>
        internal async Task<RealtimeMediaStream> ConnectAsync(
            string model,
            RealtimeVideoFormat format,
            CancellationToken token)
        {
            _stream.ValidatePlatform(); // 先检查平台，避免创建无法使用的在线会话。
            XmaxSession session = null;

            try
            {
                _timing?.Mark("session-start");
                session = await _sessions.CreateSessionAsync(model, token);
                _timing?.Mark("session-end");
                token.ThrowIfCancellationRequested();

                var info = RealtimeSessionService.RequireJoinInfo(session);
                var track = new RealtimeVideoTrack(info.BotName);
                _render.Bind(track);

                _timing?.Mark("room-start");
                await _stream.ConnectAsync(info, format, token);
                _timing?.Mark("room-end");
                token.ThrowIfCancellationRequested();

                ActiveSession = session;
                _heartbeat.Start(session.SessionUid);

                return CurrentRemoteStream = new RealtimeMediaStream(RealtimeStreamId.Remote, track);
            }
            catch
            {
                _render.Reset();

                try
                {
                    await _stream.DisconnectAsync();
                }
                finally
                {
                    await CloseSessionAsync(session);
                }

                throw;
            }
        }

        /// <summary>
        /// 先使当前会话失效，再停止心跳、断开 RTC 并尝试关闭服务端会话。
        /// </summary>
        /// <returns>本地连接资源已清理且服务端关闭请求已处理的任务。</returns>
        internal async Task DisconnectAsync()
        {
            var session = ActiveSession;
            ActiveSession = null;
            CurrentRemoteStream = null;
            _render.Reset();

            try
            {
                await _heartbeat.StopAsync();
                await _stream.DisconnectAsync();
            }
            finally
            {
                await CloseSessionAsync(session);
            }
        }

        /// <summary>
        /// 尽力关闭指定服务端会话，失败通过清理警告报告。
        /// </summary>
        /// <param name="session">服务端实时会话及其 RTC 入房信息。</param>
        /// <returns>关闭请求已结束的任务；空会话直接完成。</returns>
        private async Task CloseSessionAsync(XmaxSession session)
        {
            if (session == null || string.IsNullOrEmpty(session.SessionUid))
                return;

            try
            {
                await _sessions.CloseSessionAsync(session.SessionUid, CancellationToken.None);
            }
            catch (Exception exception)
            {
                XmaxLogger.Realtime.Failure(exception);
                EventDispatch.Raise(CleanupError, RealtimeErrorHandler.Wrap(exception));
            }
        }
    }
}
