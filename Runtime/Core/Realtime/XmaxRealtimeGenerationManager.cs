using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal sealed class XmaxRealtimeGenerationManager
    {
        private readonly IStreamController _stream;
        private readonly RenderController _render;
        private RealtimeContext _context;
        internal string TaskId { get; private set; }
        internal XmaxRealtimeGenerationManager(IStreamController stream, RenderController render)
        { _stream = stream; _render = render; }
        internal async Task<string> StartAsync(RealtimeContext context, RealtimeVideoFormat format, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (TaskId != null)
            {
                if (context != null)
                {
                    _stream.ChangeGenerationCondition(context, format, TaskId);
                    _context = context;
                }
                return TaskId; // Match iOS: nil context while generating reuses the current task.
            }
            var resolved = context ?? _context ?? throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "A realtime context is required for the first generation.");
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
            catch { await StopAsync(); throw; }
        }
        internal async Task StopAsync()
        {
            TaskId = null;
            _render.ResetGeneration();
            await _stream.StopGenerationAsync();
        }
        internal void ResetContext() => _context = null;
    }
}
