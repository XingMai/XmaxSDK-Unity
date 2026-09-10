using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理生成条件缓存、任务标识以及 SEI 确认后的首帧就绪流程。
    /// </summary>
    internal sealed class XmaxRealtimeGenerationManager
    {
        // 信令与渲染依赖。
        private readonly IStreamController _stream;
        private readonly RenderController _render;

        // 最近一次成功的生成条件。
        private RealtimeContext _context;

        /// <summary>
        /// 已就绪的生成任务标识；未生成或已停止时为 null。
        /// </summary>
        internal string TaskId { get; private set; }

        /// <summary>
        /// 注入生成信令和远端渲染依赖。
        /// </summary>
        /// <param name="stream">当前管理器的房间信令和视频传输入口。</param>
        /// <param name="render">当前管理器的远端轨道及首帧就绪控制器。</param>
        internal XmaxRealtimeGenerationManager(IStreamController stream, RenderController render)
        {
            _stream = stream;
            _render = render;
        }

        /// <summary>
        /// 启动生成并等待首帧；已有任务时仅更新非空条件，失败时停止本次生成。
        /// </summary>
        /// <param name="context">生成条件；首次生成必须提供，后续可传 null 复用最近一次成功条件。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="token">当前操作或循环的取消令牌。</param>
        /// <returns>已就绪或继续复用的生成任务标识。</returns>
        internal async Task<string> StartAsync(
            RealtimeContext context,
            RealtimeVideoFormat format,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (TaskId != null)
            {
                if (context != null)
                {
                    _stream.ChangeGenerationCondition(context, format, TaskId);
                    _context = context;
                }

                return TaskId; // 生成中传入空条件时复用当前任务，与 iOS 语义一致。
            }

            var resolved = context ?? _context ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "A realtime context is required for the first generation.");
            _render.BeginGeneration();

            try
            {
                var taskId = await _stream.StartGenerationAsync(resolved, format, token);
                await _render.WaitUntilRemoteFrameReadyAsync(token);
                token.ThrowIfCancellationRequested();
                _context = resolved;
                TaskId = taskId;

                return taskId;
            }
            catch
            {
                await StopAsync();

                throw;
            }
        }

        /// <summary>
        /// 使任务和远端帧失效并停止生成，保留上一次成功的生成条件。
        /// </summary>
        /// <returns>生成信令及其后台循环已停止的任务。</returns>
        internal async Task StopAsync()
        {
            TaskId = null;
            _render.ResetGeneration();
            await _stream.StopGenerationAsync();
        }

        /// <summary>
        /// 清除成功条件缓存，下一次生成必须显式提供条件。
        /// </summary>
        internal void ResetContext() => _context = null;
    }
}
