using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xmax.SDK.Tests
{
    internal static class AsyncTest
    {
        internal static IEnumerator Run(Func<Task> body)
        {
            var task = body();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (!task.IsCompleted)
            {
                if (watch.Elapsed.TotalSeconds > 15) Assert.Fail("Async SDK test timed out.");
                yield return null;
            }
            task.GetAwaiter().GetResult();
        }
        internal static async Task Throws<T>(Task task) where T : Exception
        {
            try { await task; }
            catch (T) { return; }
            Assert.Fail("Expected " + typeof(T).Name);
        }
        internal static XmaxVideoFrame Frame() => XmaxVideoFrame.CreateI420(new byte[4], new byte[1], new byte[1], 2, 2);
        internal static TaskCompletionSource<T> Pending<T>() => new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
    }
    internal sealed class FakeSessions : IRealtimeSessionService
    {
        internal Func<CancellationToken, Task<XmaxSession>> Create;
        internal Func<CancellationToken, Task<XmaxSession>> Heartbeat;
        internal Func<Task> Close;
        internal int Created;
        internal string LastModel;
        internal readonly List<string> Closed = new List<string>();
        internal static XmaxSession Session(string id = "session") => new XmaxSession
        {
            SessionUid = id, Status = "ACTIVE", JoinInfo = new RtcJoinInfo { AppId = "app", RoomId = "room", UserId = "user", Token = "token", BotName = "bot" }
        };
        public Task<XmaxSession> CreateSessionAsync(string model, CancellationToken token)
        {
            Created++;
            LastModel = model;

            return Create?.Invoke(token) ?? Task.FromResult(Session());
        }
        public Task<XmaxSession> HeartbeatSessionAsync(string id, CancellationToken token)
            => Heartbeat?.Invoke(token) ?? Task.FromResult(Session(id));
        public Task CloseSessionAsync(string id, CancellationToken token)
        { Closed.Add(id); return Close?.Invoke() ?? Task.CompletedTask; }
    }
    internal sealed class FakeStream : IStreamController
    {
        public event Action<XmaxException> FatalError;
        public event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        public event Action<XmaxVideoFrame> FrameReceived;
        internal Func<CancellationToken, Task> Join;
        internal Func<CancellationToken, Task<string>> Generate;
        internal bool RejectPlatform;
        internal int Joins, Leaves, Starts, Stops, Updates, Pushes, Tracks;
        internal bool RejectUpdate;
        internal RealtimeVideoFormat ConnectedFormat;
        public void ValidatePlatform() { if (RejectPlatform) throw new XmaxException(XmaxErrorCode.NotSupported, "Test unsupported platform."); }
        public Task ConnectAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken token)
        {
            Joins++;
            ConnectedFormat = format;

            return Join?.Invoke(token) ?? Task.CompletedTask;
        }
        public Task DisconnectAsync() { Leaves++; return Task.CompletedTask; }
        public Task<string> StartGenerationAsync(RealtimeContext context, RealtimeVideoFormat format, CancellationToken token)
        {
            Starts++;
            if (Generate != null) return Generate(token);
            FrameReceived?.Invoke(AsyncTest.Frame());
            return Task.FromResult("task-" + Starts);
        }
        public void ChangeGenerationCondition(RealtimeContext context, RealtimeVideoFormat format, string taskId)
        { if (RejectUpdate) throw new XmaxException(XmaxErrorCode.RtcError, "Update failed."); Updates++; }
        public Task StopGenerationAsync() { Stops++; return Task.CompletedTask; }
        public void PushFrame(XmaxVideoFrame frame) { Pushes++; }
        public void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks, string taskId) { Tracks++; }
        internal void Fail() => FatalError?.Invoke(new XmaxException(XmaxErrorCode.RtcError, "Test RTC failure."));
        internal void Frame() => FrameReceived?.Invoke(AsyncTest.Frame());
        internal void Quality() => NetworkQualityChanged?.Invoke(new RealtimeNetworkQuality(0.01, 20, 100000));
    }
    internal sealed class FakeRtc : IRtcManager
    {
        public event Action<XmaxException> FatalError;
        public event Action<RtcNetworkQuality> NetworkQualityChanged;
        public event Action<RemoteStream> VideoPublished;
        public event Action<RemoteStream> VideoUnpublished;
        public event Action<RemoteStream, byte[]> SeiReceived;
        public event Action<RemoteStream, XmaxVideoFrame> FrameReceived;
        internal readonly List<string> Messages = new List<string>();
        internal bool RejectSei;
        internal int Subscriptions;
        internal static RemoteStream Bot => new RemoteStream("room", "bot");
        public void ValidatePlatform() { }
        public Task JoinAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken token) => Task.CompletedTask;
        public Task LeaveAsync() => Task.CompletedTask;
        public void SubscribeVideo(RemoteStream key) { Subscriptions++; }
        public void PushFrame(XmaxVideoFrame frame) { }
        public void SendRoomMessage(string message) { Messages.Add(message); }
        public void SendSei(byte[] data) { if (RejectSei) throw new XmaxException(XmaxErrorCode.RtcError, "SEI failed."); }
        internal void Sei(string id, RemoteStream? key = null) => SeiReceived?.Invoke(key ?? Bot, System.Text.Encoding.UTF8.GetBytes(id));
        internal void Frame(RemoteStream? key = null) => FrameReceived?.Invoke(key ?? Bot, AsyncTest.Frame());
        internal void Publish() => VideoPublished?.Invoke(Bot);
        internal void Unpublish() => VideoUnpublished?.Invoke(Bot);
        internal void Fail() => FatalError?.Invoke(new XmaxException(XmaxErrorCode.RtcError, "RTC failed."));
        internal void Quality(RtcNetworkQuality? quality = null) => NetworkQualityChanged?.Invoke(quality ??
            new RtcNetworkQuality(RtcQualityLevel.Good, RtcQualityLevel.Poor, 0.02, 30, 100000));
    }
}
