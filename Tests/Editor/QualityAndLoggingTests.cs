using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Xmax.SDK.Tests
{
    public sealed class QualityAndLoggingTests
    {
        private readonly List<string> _logs = new List<string>();
        [SetUp] public void SetUp()
        {
            XmaxLogger.Configure(XmaxLoggerOption.None);
            _logs.Clear();
            Application.logMessageReceived += Capture;
        }
        [TearDown] public void TearDown()
        {
            Application.logMessageReceived -= Capture;
            XmaxLogger.Configure(XmaxLoggerOption.None);
        }
        private void Capture(string text, string stack, LogType type)
        { if (text.StartsWith("[Xmax]", StringComparison.Ordinal)) _logs.Add(text); }

        [Test] public void LoggingDefaultsOffAndDoesNotEvaluateDisabledMessages()
        {
            var evaluated = false;
            XmaxLogger.Info("Test", () => { evaluated = true; return "test"; });
            XmaxLogger.Failure("Test", new Exception("private message"));
            Assert.False(evaluated);
            Assert.IsEmpty(_logs);
            Assert.AreEqual(XmaxLoggerOption.None, new XmaxConfiguration("").LoggerOptions);
        }
        [Test] public void BusinessAndPerformanceCanBeEnabledIndependently()
        {
            XmaxLogger.Configure(XmaxLoggerOption.Business);
            XmaxLogger.Info("Business", () => "one\ntwo");
            XmaxLogger.Info("Performance", () => "hidden", XmaxLoggerOption.Performance);
            Assert.AreEqual(1, _logs.Count);
            Assert.AreEqual("[Xmax][Business] one\n[Xmax][Business] two", _logs[0]);
            XmaxLogger.Configure(XmaxLoggerOption.Performance);
            XmaxLogger.Info("Business", () => "hidden");
            XmaxLogger.Info("Performance", () => "visible", XmaxLoggerOption.Performance);
            Assert.AreEqual(2, _logs.Count);
        }
        [Test] public void ApiAndErrorLogsExcludeIdentifiersCredentialsAndExceptionBodies()
        {
            XmaxLogger.Configure(XmaxLoggerOption.All);
            ApiLogger.Response("PUT", "/session/private-session/heartbeat?token=secret-token", 200, 123, 42);
            LogAssert.Expect(LogType.Error, "[Xmax][API] POST <route> failed=ApiError duration=50ms");
            ApiLogger.Failure("POST", "/private-path?apiKey=secret-key", new XmaxException(XmaxErrorCode.ApiError, "secret-body"), 50);
            LogAssert.Expect(LogType.Error, "[Xmax][Callback] Operation failed: Exception");
            XmaxLogger.Failure("Callback", new Exception("secret-prompt"));
            var output = string.Join("\n", _logs);
            Assert.That(output, Does.Contain("/session/{id}/heartbeat"));
            Assert.That(output, Does.Contain("status=200 duration=42ms bytes=123"));
            Assert.That(output, Does.Contain("failed=ApiError"));
            foreach (var secret in new[] { "private-session", "private-path", "secret-token", "secret-key", "secret-body", "secret-prompt" })
                Assert.That(output, Does.Not.Contain(secret));
        }
        [Test] public void ClientConfigurationAppliesLoggerOptionsWithoutValidatingApiKey()
        {
            var client = new XmaxClient(new XmaxConfiguration("", XmaxEnvironment.Global, XmaxLoggerOption.Performance));
            Assert.True(XmaxLogger.IsEnabled(XmaxLoggerOption.Performance));
            Assert.False(XmaxLogger.IsEnabled(XmaxLoggerOption.Business));
            Assert.AreEqual(XmaxConfiguration.GlobalBaseUrl, client.Configuration.BaseUrl);
        }
        [Test] public void QualityControllerMapsBothDirectionsAndRetainsMetrics()
        {
            var rtc = new FakeRtc();
            var controller = new QualityController(rtc);
            RealtimeNetworkQuality result = null;
            controller.NetworkQualityChanged += quality => result = quality;
            foreach (RtcQualityLevel level in Enum.GetValues(typeof(RtcQualityLevel)))
            {
                rtc.Quality(new RtcNetworkQuality(level, RtcQualityLevel.Poor, 0.02, 30, 100000));
                Assert.AreEqual(level.ToString(), result.Uplink.ToString());
                Assert.AreEqual(RealtimeNetworkQualityLevel.Poor, result.Downlink);
                Assert.AreEqual(0.02, result.PacketLossRatio);
                Assert.AreEqual(30, result.RoundTripMilliseconds);
                Assert.AreEqual(100000, result.BandwidthBitsPerSecond);
            }
            rtc.Quality(new RtcNetworkQuality((RtcQualityLevel)999, (RtcQualityLevel)(-1), 0, 0, 0));
            Assert.AreEqual(RealtimeNetworkQualityLevel.Unknown, result.Uplink);
            Assert.AreEqual(RealtimeNetworkQualityLevel.Unknown, result.Downlink);
        }
        [UnityTest] public IEnumerator QualityEventsStopAfterDisconnect() => AsyncTest.Run(async () =>
        {
            var rtc = new FakeRtc(); var stream = new StreamController(rtc); var count = 0;
            stream.NetworkQualityChanged += quality => count++;
            rtc.Quality(); Assert.AreEqual(0, count);
            await stream.ConnectAsync(FakeSessions.Session().JoinInfo, new RealtimeVideoFormat(1280, 720, 30), CancellationToken.None);
            rtc.Quality(); Assert.AreEqual(1, count);
            await stream.DisconnectAsync();
            rtc.Quality(); Assert.AreEqual(1, count);
        });
        [Test] public void StartupTimingRejectsStaleTaskConfirmationAndResetsBetweenAttempts()
        {
            var now = 0.0; var timing = new RealtimeTiming(() => now);
            XmaxLogger.Configure(XmaxLoggerOption.Performance);
            timing.Begin(); now = 10; timing.Mark("session-start"); now = 30; timing.Mark("session-end");
            now = 40; timing.BeginSignal("current"); now = 41; timing.SignalSent("current");
            timing.MatchSei("old");
            Assert.That(timing.Format("failed"), Does.Contain("pending=confirmation"));
            now = 80; timing.MatchSei("current"); now = 100; timing.Finish();
            Assert.That(_logs[0], Does.Contain("total=100.0ms"));
            Assert.That(_logs[0], Does.Contain("session=20.0ms"));
            Assert.That(_logs[0], Does.Contain("confirmation=40.0ms"));
            Assert.That(_logs[0], Does.Contain("first-frame=20.0ms"));
            timing.Begin(); now = 150;
            timing.Fail(new XmaxException(XmaxErrorCode.Timeout, "secret failure"));
            Assert.That(_logs[1], Does.Contain("total=50.0ms"));
            Assert.That(_logs[1], Does.Contain("pending=preparation"));
            Assert.That(_logs[1], Does.Not.Contain("secret failure"));
            Assert.That(_logs[1], Does.Not.Contain("session="));
            timing.Begin(); timing.Fail(new OperationCanceledException());
            Assert.AreEqual(2, _logs.Count);
        }
        [UnityTest] public IEnumerator CombinedStartupProducesRealStageTiming() => AsyncTest.Run(async () =>
        {
            var timing = new RealtimeTiming(); var rtc = new FakeRtc();
            var stream = new StreamController(rtc, taskIdFactory: () => "startup", timing: timing);
            var manager = new XmaxRealtimeManager(new XmaxConfiguration("test", loggerOptions: XmaxLoggerOption.Performance),
                new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)), new FakeSessions(), stream, timing);
            try
            {
                var local = manager.CreateLocalExternalStream(new RealtimeVideoFormat(1280, 720, 30));
                var start = manager.StartGenerationAsync(local, new RealtimeContext("secret prompt"));
                rtc.Sei("startup"); rtc.Frame(); await start;
                var output = string.Join("\n", _logs);
                foreach (var stage in new[] { "session=", "room=", "connection=", "signal=", "confirmation=", "first-frame=" })
                    Assert.That(output, Does.Contain(stage));
                Assert.That(output, Does.Not.Contain("secret prompt"));
                Assert.That(output, Does.Not.Contain("[Realtime]"));
            }
            finally { await manager.CloseAsync(); }
        });
    }
}
