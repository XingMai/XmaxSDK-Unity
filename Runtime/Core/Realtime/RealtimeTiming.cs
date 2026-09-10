using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Xmax.SDK
{
    /// <summary>
    /// 记录同一管理器一次生成启动过程的阶段耗时，使用单调时钟。
    /// </summary>
    internal sealed class RealtimeTiming
    {
        // 单调时钟。
        private readonly Func<double> _now;

        // 当前启动过程的阶段记录与任务关联。
        private readonly Dictionary<string, double> _stages = new Dictionary<string, double>();
        private bool _active;
        private string _taskId;

        /// <summary>
        /// 创建启动计时器，可注入测试时钟。
        /// </summary>
        /// <param name="now">返回单调递增毫秒时间的函数；为空时使用 Stopwatch。</param>
        internal RealtimeTiming(Func<double> now = null)
        {
            _now = now ?? (() => Stopwatch.GetTimestamp() * 1000.0 / Stopwatch.Frequency);
        }

        /// <summary>
        /// 清除上一轮数据并开始本轮启动计时。
        /// </summary>
        internal void Begin()
        {
            _stages.Clear();
            _taskId = null;
            _active = true;

            Mark("start");
        }

        /// <summary>
        /// 仅在计时启用时记录指定阶段的首次时间戳。
        /// </summary>
        /// <param name="stage">需要记录的阶段名称，同一轮只记录首次发生时间。</param>
        internal void Mark(string stage)
        {
            if (_active && !_stages.ContainsKey(stage))
                _stages[stage] = _now();
        }

        /// <summary>
        /// 关联当前生成任务并记录信令发送起点。
        /// </summary>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        internal void BeginSignal(string taskId)
        {
            _taskId = taskId;
            Mark("signal-start");
        }

        /// <summary>
        /// 仅为当前生成任务记录信令已发送时间。
        /// </summary>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        internal void SignalSent(string taskId)
        {
            if (_taskId == taskId)
                Mark("signal-sent");
        }

        /// <summary>
        /// 仅为当前生成任务记录远端 SEI 确认时间。
        /// </summary>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        internal void MatchSei(string taskId)
        {
            if (_taskId == taskId)
                Mark("sei");
        }

        /// <summary>
        /// 记录首帧就绪时间，按性能日志配置输出耗时并结束计时。
        /// </summary>
        internal void Finish()
        {
            if (!_active)
                return;

            Mark("ready");
            XmaxLogger.Timing.Info(() => Format("ready"), XmaxLoggerOption.Performance);
            _active = false;
        }

        /// <summary>
        /// 结束失败或取消的计时；主动取消不输出失败耗时日志。
        /// </summary>
        /// <param name="exception">需要处理的原始异常。</param>
        internal void Fail(Exception exception)
        {
            if (!_active)
                return;

            Mark("failed");
            if (!(exception is OperationCanceledException))
                XmaxLogger.Timing.Info(
                    () => Format("failed") + " error=" + XmaxLogger.ErrorCode(exception),
                    XmaxLoggerOption.Performance);

            _active = false;
        }

        /// <summary>
        /// 汇总已记录阶段的耗时，失败时附加尚未完成的阶段。
        /// </summary>
        /// <param name="end">用于结束计时或截断未完成阶段的时间戳名称。</param>
        /// <returns>不含提示词、令牌或任务标识的性能日志文本。</returns>
        internal string Format(string end)
        {
            var details = new List<string> { "startup=" + end, "total=" + Duration("start", end) };
            Add(details, "session", "session-start", "session-end", end);
            Add(details, "room", "room-start", "room-end", end);
            Add(details, "connection", "connection-start", "connection-end", end);
            Add(details, "signal", "signal-start", "signal-sent", end);
            Add(details, "confirmation", "signal-start", "sei", end);
            Add(details, "first-frame", "sei", "ready", end);
            if (end == "failed")
                details.Add("pending=" + PendingStage());

            return string.Join(" ", details);
        }

        /// <summary>
        /// 将已开始阶段的完整或截至结束点的耗时追加至日志列表。
        /// </summary>
        /// <param name="details">用于依次收集各阶段耗时的文本列表。</param>
        /// <param name="name">输出到日志的阶段显示名称。</param>
        /// <param name="start">阶段起点的时间戳名称。</param>
        /// <param name="finish">阶段完成点的时间戳名称。</param>
        /// <param name="end">用于结束计时或截断未完成阶段的时间戳名称。</param>
        private void Add(List<string> details, string name, string start, string finish, string end)
        {
            if (_stages.ContainsKey(start))
                details.Add(name + "=" + Duration(start, _stages.ContainsKey(finish) ? finish : end));
        }

        /// <summary>
        /// 计算两个阶段之间的非负毫秒耗时。
        /// </summary>
        /// <param name="start">阶段起点的时间戳名称。</param>
        /// <param name="end">用于结束计时或截断未完成阶段的时间戳名称。</param>
        /// <returns>保留一位小数并带 ms 单位的文本；缺少时间戳时为 unavailable。</returns>
        private string Duration(string start, string end) =>
            _stages.TryGetValue(start, out var a) && _stages.TryGetValue(end, out var b)
                ? Math.Max(0, b - a).ToString("F1", CultureInfo.InvariantCulture) + "ms"
                : "unavailable";

        /// <summary>
        /// 根据已完成的阶段判断启动流程当前等待的位置。
        /// </summary>
        /// <returns>首帧、确认、生成、房间、会话或准备阶段的日志名称。</returns>
        private string PendingStage() =>
            _stages.ContainsKey("sei") ? "first-frame" :
            _stages.ContainsKey("signal-start") ? "confirmation" :
            _stages.ContainsKey("connection-end") ? "generation" :
            _stages.ContainsKey("room-start") ? "room" :
            _stages.ContainsKey("session-start") ? "session" : "preparation";
    }
}
