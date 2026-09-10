using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Xmax.SDK
{
    // Shared by one manager's serialized startup operation. Stopwatch is monotonic.
    internal sealed class RealtimeTiming
    {
        private readonly Func<double> _now;
        private readonly Dictionary<string, double> _stages = new Dictionary<string, double>();
        private bool _active;
        private string _taskId;
        internal RealtimeTiming(Func<double> now = null)
        { _now = now ?? (() => Stopwatch.GetTimestamp() * 1000.0 / Stopwatch.Frequency); }
        internal void Begin()
        {
            _stages.Clear(); _taskId = null; _active = true; Mark("start");
        }
        internal void Mark(string stage)
        {
            if (_active && !_stages.ContainsKey(stage)) _stages[stage] = _now();
        }
        internal void BeginSignal(string taskId) { _taskId = taskId; Mark("signal-start"); }
        internal void SignalSent(string taskId) { if (_taskId == taskId) Mark("signal-sent"); }
        internal void MatchSei(string taskId) { if (_taskId == taskId) Mark("sei"); }
        internal void Finish()
        {
            if (!_active) return;
            Mark("ready");
            XmaxLogger.Timing.Info(() => Format("ready"), XmaxLoggerOption.Performance);
            _active = false;
        }
        internal void Fail(Exception exception)
        {
            if (!_active) return;
            Mark("failed");
            if (!(exception is OperationCanceledException))
                XmaxLogger.Timing.Info(() => Format("failed") + " error=" + XmaxLogger.ErrorCode(exception), XmaxLoggerOption.Performance);
            _active = false;
        }
        internal string Format(string end)
        {
            var details = new List<string> { "startup=" + end, "total=" + Duration("start", end) };
            Add(details, "session", "session-start", "session-end", end);
            Add(details, "room", "room-start", "room-end", end);
            Add(details, "connection", "connection-start", "connection-end", end);
            Add(details, "signal", "signal-start", "signal-sent", end);
            Add(details, "confirmation", "signal-start", "sei", end);
            Add(details, "first-frame", "sei", "ready", end);
            if (end == "failed") details.Add("pending=" + PendingStage());
            return string.Join(" ", details);
        }
        private void Add(List<string> details, string name, string start, string finish, string end)
        {
            if (_stages.ContainsKey(start)) details.Add(name + "=" + Duration(start, _stages.ContainsKey(finish) ? finish : end));
        }
        private string Duration(string start, string end) => _stages.TryGetValue(start, out var a) && _stages.TryGetValue(end, out var b)
            ? Math.Max(0, b - a).ToString("F1", CultureInfo.InvariantCulture) + "ms" : "unavailable";
        private string PendingStage() => _stages.ContainsKey("sei") ? "first-frame" : _stages.ContainsKey("signal-start") ? "confirmation" :
            _stages.ContainsKey("connection-end") ? "generation" : _stages.ContainsKey("room-start") ? "room" :
            _stages.ContainsKey("session-start") ? "session" : "preparation";
    }
}
