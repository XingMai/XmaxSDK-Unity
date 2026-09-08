using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Xmax.SDK.Tests
{
    public sealed class RealtimeLifecycleTests
    {
        private FakeSessions _sessions;
        private FakeStream _stream;
        private XmaxRealtimeManager _manager;
        private static RealtimeVideoFormat Format => new RealtimeVideoFormat(1280, 720, 30);
        [SetUp] public void SetUp()
        {
            _sessions = new FakeSessions(); _stream = new FakeStream();
            _manager = new XmaxRealtimeManager(new XmaxConfiguration("test"), new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)), _sessions, _stream);
        }
        [UnityTearDown] public IEnumerator TearDown() => AsyncTest.Run(() => _manager.CloseAsync());

        [UnityTest] public IEnumerator LateSessionIsClosedWithoutJoining() => AsyncTest.Run(async () =>
        {
            var pending = AsyncTest.Pending<XmaxSession>();
            _sessions.Create = token => pending.Task; // Deliberately ignores cancellation.
            var connect = _manager.ConnectAsync(Format);
            var disconnect = _manager.DisconnectAsync();
            Assert.False(disconnect.IsCompleted);
            pending.SetResult(FakeSessions.Session("late"));
            await AsyncTest.Throws<OperationCanceledException>(connect);
            await disconnect;
            Assert.AreEqual(0, _stream.Joins);
            CollectionAssert.AreEqual(new[] { "late" }, _sessions.Closed);
            Assert.AreEqual(RealtimeConnectionState.Disconnected, _manager.CurrentState.ConnectionState);
        });
        [UnityTest] public IEnumerator CleanupBlocksReconnectAndCoalescesDisconnects() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            var pending = AsyncTest.Pending<bool>();
            _sessions.Close = () => pending.Task;
            var first = _manager.DisconnectAsync();
            var second = _manager.DisconnectAsync();
            Assert.AreSame(first, second);
            Assert.AreEqual(RealtimeConnectionState.Disconnecting, _manager.CurrentState.ConnectionState);
            await AsyncTest.Throws<XmaxException>(_manager.ConnectAsync(Format));
            pending.SetResult(true);
            await first;
            _sessions.Close = null;
            await _manager.ConnectAsync(Format);
            Assert.AreEqual(RealtimeConnectionState.Connected, _manager.CurrentState.ConnectionState);
            Assert.AreEqual(1, _sessions.Closed.Count);
        });
        [UnityTest] public IEnumerator StopCancelsPendingGenerationWithoutDisconnecting() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            _stream.Generate = async token => { await Task.Delay(Timeout.Infinite, token); return "never"; };
            var start = _manager.StartGenerationAsync("first");
            var stop = _manager.StopGenerationAsync();
            await AsyncTest.Throws<OperationCanceledException>(start);
            await stop;
            Assert.AreEqual(RealtimeConnectionState.Connected, _manager.CurrentState.ConnectionState);
            Assert.IsNull(_manager.CurrentState.TaskId);
            _stream.Generate = null;
            await _manager.StartGenerationAsync("second");
            Assert.AreEqual(RealtimeConnectionState.Generating, _manager.CurrentState.ConnectionState);
        });
        [UnityTest] public IEnumerator GenerationNeedsAnActualRemoteFrame() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            _stream.Generate = token => Task.FromResult("confirmed");
            var start = _manager.StartGenerationAsync("first");
            await Task.Yield();
            Assert.False(start.IsCompleted);
            Assert.AreEqual(RealtimeConnectionState.Connected, _manager.CurrentState.ConnectionState);
            _stream.Frame();
            await start;
            Assert.AreEqual("confirmed", _manager.CurrentState.TaskId);
        });
        [UnityTest] public IEnumerator NilContextReusesTaskAndExplicitContextUpdatesIt() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            await _manager.StartGenerationAsync("first");
            var taskId = _manager.CurrentState.TaskId;
            await _manager.StartGenerationAsync();
            await _manager.StartGenerationAsync("second");
            Assert.AreEqual(taskId, _manager.CurrentState.TaskId);
            Assert.AreEqual(1, _stream.Starts);
            Assert.AreEqual(1, _stream.Updates);
            Assert.AreEqual(0, _stream.Stops);
        });
        [UnityTest] public IEnumerator FailedContextUpdateKeepsGenerationRunning() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            await _manager.StartGenerationAsync("first");
            var id = _manager.CurrentState.TaskId;
            _stream.RejectUpdate = true;
            await AsyncTest.Throws<XmaxException>(_manager.StartGenerationAsync("second"));
            Assert.AreEqual(id, _manager.CurrentState.TaskId);
            Assert.AreEqual(RealtimeConnectionState.Generating, _manager.CurrentState.ConnectionState);
        });
        [UnityTest] public IEnumerator ReentrantDisconnectFromConnectingDoesNotCreateSession() => AsyncTest.Run(async () =>
        {
            Task disconnect = null;
            _manager.StateChanged += state => { if (state.ConnectionState == RealtimeConnectionState.Connecting) disconnect = _manager.DisconnectAsync(); };
            await AsyncTest.Throws<OperationCanceledException>(_manager.ConnectAsync(Format));
            await disconnect;
            Assert.AreEqual(0, _sessions.Created);
        });
        [UnityTest] public IEnumerator ReentrantDisconnectFromConnectedCannotReturnLiveStream() => AsyncTest.Run(async () =>
        {
            Task disconnect = null;
            _manager.StateChanged += state => { if (state.ConnectionState == RealtimeConnectionState.Connected) disconnect = _manager.DisconnectAsync(); };
            await AsyncTest.Throws<OperationCanceledException>(_manager.ConnectAsync(Format));
            await disconnect;
            Assert.AreEqual(1, _sessions.Closed.Count);
        });
        [UnityTest] public IEnumerator ListenerExceptionsDoNotBreakConnection() => AsyncTest.Run(async () =>
        {
            Action<RealtimeState> listener = state => { throw new InvalidOperationException("listener failure"); };
            _manager.StateChanged += listener;
            LogAssert.Expect(UnityEngine.LogType.Exception, "InvalidOperationException: listener failure");
            LogAssert.Expect(UnityEngine.LogType.Exception, "InvalidOperationException: listener failure");
            await _manager.ConnectAsync(Format);
            _manager.StateChanged -= listener;
            Assert.AreEqual(RealtimeConnectionState.Connected, _manager.CurrentState.ConnectionState);
        });
        [UnityTest] public IEnumerator TerminalListenerCanUpgradeDisconnectToClose() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            var source = _manager.LocalStream;
            Task close = null;
            _manager.StateChanged += state =>
            {
                if (state.ConnectionState == RealtimeConnectionState.Disconnected) close = _manager.CloseAsync();
            };
            await _manager.DisconnectAsync();
            await close;
            Assert.IsNull(_manager.LocalStream);
            Assert.AreEqual(RealtimeConnectionState.Disconnected, _manager.CurrentState.ConnectionState);
            Assert.Throws<XmaxException>(() => source.PushVideoFrame(AsyncTest.Frame()));
        });
        [UnityTest] public IEnumerator ReentrantStateChangeDoesNotDeliverObsoleteStateToNextListener() => AsyncTest.Run(async () =>
        {
            Task disconnect = null;
            var observed = new List<RealtimeConnectionState>();
            _manager.StateChanged += state =>
            {
                if (state.ConnectionState == RealtimeConnectionState.Connecting) disconnect = _manager.DisconnectAsync();
            };
            _manager.StateChanged += state => observed.Add(state.ConnectionState);
            await AsyncTest.Throws<OperationCanceledException>(_manager.ConnectAsync(Format));
            await disconnect;
            CollectionAssert.DoesNotContain(observed, RealtimeConnectionState.Connecting);
            Assert.AreEqual(RealtimeConnectionState.Disconnected, observed[observed.Count - 1]);
        });
        [UnityTest] public IEnumerator CancellationFromGeneratingListenerStopsCommittedTask() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            using (var cancellation = new CancellationTokenSource())
            {
                _manager.StateChanged += state => { if (state.ConnectionState == RealtimeConnectionState.Generating) cancellation.Cancel(); };
                await AsyncTest.Throws<OperationCanceledException>(_manager.StartGenerationAsync(new RealtimeContext("test"), cancellation.Token));
                Assert.AreEqual(RealtimeConnectionState.Connected, _manager.CurrentState.ConnectionState);
                Assert.IsNull(_manager.CurrentState.TaskId);
                Assert.Greater(_stream.Stops, 0);
            }
        });
        [UnityTest] public IEnumerator FatalErrorIsDeliveredAfterCleanup() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            var pending = AsyncTest.Pending<bool>();
            _sessions.Close = () => pending.Task;
            XmaxException failure = null;
            _manager.ErrorOccurred += error => failure = error;
            _stream.Fail();
            Assert.IsNull(failure);
            var cleanup = _manager.DisconnectAsync();
            pending.SetResult(true);
            await cleanup;
            Assert.AreEqual(XmaxErrorSeverity.Fatal, failure.Severity);
            Assert.AreEqual(RealtimeConnectionState.Error, _manager.CurrentState.ConnectionState);
        });
        [UnityTest] public IEnumerator FatalErrorDuringJoinCancelsAndClosesSession() => AsyncTest.Run(async () =>
        {
            _stream.Join = token => Task.Delay(Timeout.Infinite, token);
            var connect = _manager.ConnectAsync(Format);
            _stream.Fail();
            await AsyncTest.Throws<OperationCanceledException>(connect);
            await _manager.DisconnectAsync();
            Assert.AreEqual(1, _sessions.Closed.Count);
        });
        [UnityTest] public IEnumerator CloseUpgradesPendingDisconnectAndInvalidatesLocalSource() => AsyncTest.Run(async () =>
        {
            var source = _manager.CreateLocalExternalStream(Format);
            await _manager.ConnectAsync(source);
            var pending = AsyncTest.Pending<bool>();
            _sessions.Close = () => pending.Task;
            var disconnect = _manager.DisconnectAsync();
            var close = _manager.CloseAsync();
            Assert.AreSame(disconnect, close);
            pending.SetResult(true);
            await close;
            Assert.IsNull(_manager.LocalStream);
            Assert.Throws<XmaxException>(() => source.PushVideoFrame(AsyncTest.Frame()));
            Assert.AreEqual(1, _sessions.Closed.Count);
        });
        [UnityTest] public IEnumerator DisconnectKeepsLocalPreviewAndCloseReleasesIt() => AsyncTest.Run(async () =>
        {
            var source = _manager.CreateLocalExternalStream(Format);
            var previewFrames = 0;
            source.VideoTrack.FrameReceived += frame => previewFrames++;
            source.PushVideoFrame(AsyncTest.Frame());
            Assert.AreEqual(0, _stream.Pushes);
            await _manager.ConnectAsync(source);
            source.PushVideoFrame(AsyncTest.Frame());
            await _manager.DisconnectAsync();
            source.PushVideoFrame(AsyncTest.Frame());
            Assert.AreEqual(3, previewFrames);
            Assert.AreEqual(1, _stream.Pushes);
            Assert.AreSame(source, _manager.LocalStream);
        });
        [UnityTest] public IEnumerator UnsupportedPlatformDoesNotCreateSession() => AsyncTest.Run(async () =>
        {
            _stream.RejectPlatform = true;
            await AsyncTest.Throws<XmaxException>(_manager.ConnectAsync(Format));
            Assert.AreEqual(0, _sessions.Created);
        });
        [UnityTest] public IEnumerator CleanupFailureIsRecoverableAndDoesNotBlockReconnect() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            _sessions.Close = () => Task.FromException(new XmaxException(XmaxErrorCode.NetworkError, "offline"));
            XmaxException warning = null;
            _manager.CleanupWarning += error => warning = error;
            await _manager.DisconnectAsync();
            Assert.AreEqual(XmaxErrorSeverity.Recoverable, warning.Severity);
            Assert.AreEqual(RealtimeConnectionState.Disconnected, _manager.CurrentState.ConnectionState);
            await _manager.ConnectAsync(Format);
        });
        [UnityTest] public IEnumerator TracksRequireGenerationAndEncoderBounds() => AsyncTest.Run(async () =>
        {
            await _manager.ConnectAsync(Format);
            Assert.Throws<XmaxException>(() => _manager.SendTracks(new[] { new XmaxTrackPoint(1, 1) }));
            await _manager.StartGenerationAsync("first");
            Assert.Throws<XmaxException>(() => _manager.SendTracks(new[] { new XmaxTrackPoint(1280, 0) }));
            _manager.SendTracks(new[] { new XmaxTrackPoint(1279, 719) });
            Assert.AreEqual(1, _stream.Tracks);
        });
        [UnityTest] public IEnumerator FrameCallbackCanStopWithoutForwardingStaleManagerFrame() => AsyncTest.Run(async () =>
        {
            var remote = await _manager.ConnectAsync(Format);
            await _manager.StartGenerationAsync("first");
            var count = 0;
            Task stop = null;
            _manager.RemoteFrameReceived += frame => count++;
            remote.VideoTrack.FrameReceived += frame => stop = _manager.StopGenerationAsync();
            _stream.Frame();
            await stop;
            Assert.AreEqual(0, count);
        });
    }
}
