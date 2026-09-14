using System.Collections;
using System.Threading;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Xmax.SDK.Tests
{
    public sealed class EncodingControllerTests
    {
        [TestCase(120, 120, 15, 50, 100)]
        [TestCase(160, 120, 15, 65, 130)]
        [TestCase(180, 180, 15, 100, 200)]
        [TestCase(240, 180, 15, 120, 240)]
        [TestCase(320, 180, 15, 140, 280)]
        [TestCase(320, 240, 15, 200, 400)]
        [TestCase(424, 240, 15, 220, 440)]
        [TestCase(360, 360, 15, 260, 520)]
        [TestCase(480, 360, 15, 320, 640)]
        [TestCase(640, 360, 15, 400, 800)]
        [TestCase(640, 480, 15, 500, 1000)]
        [TestCase(848, 480, 15, 610, 1220)]
        [TestCase(960, 720, 15, 910, 1820)]
        [TestCase(1280, 720, 15, 1130, 2260)]
        [TestCase(1920, 1080, 15, 2080, 4160)]
        [TestCase(360, 360, 30, 400, 800)]
        [TestCase(480, 360, 30, 490, 980)]
        [TestCase(640, 360, 30, 600, 1200)]
        [TestCase(640, 480, 30, 750, 1500)]
        [TestCase(848, 480, 30, 930, 1860)]
        [TestCase(960, 720, 30, 1380, 2760)]
        [TestCase(1280, 720, 30, 1710, 3420)]
        [TestCase(1920, 1080, 30, 3150, 6300)]
        [TestCase(640, 480, 10, 400, 800)]
        [TestCase(1920, 1080, 60, 4780, 6500)]
        public void DefaultsMatchReferencePoints(int width, int height, int fps, int minimum, int maximum)
        {
            var configuration = EncodingController.Resolve(new RealtimeVideoFormat(width, height, fps));

            Assert.AreEqual(minimum, configuration.MinimumBitrate);
            Assert.AreEqual(maximum, configuration.MaximumBitrate);
        }

        [TestCase(1920, 1080, 24, 2722, 5444)]
        [TestCase(832, 1472, 24, 1805, 3611)]
        [TestCase(1472, 832, 24, 1805, 3611)]
        [TestCase(1024, 1920, 30, 3016, 6031)]
        [TestCase(1920, 1024, 30, 3016, 6031)]
        [TestCase(1024, 1920, 24, 2606, 5212)]
        [TestCase(1024, 768, 30, 1516, 3033)]
        [TestCase(1024, 768, 24, 1310, 2620)]
        [TestCase(1920, 1080, 120, 9560, 13000)]
        [TestCase(3840, 2160, 30, 12600, 25200)]
        [TestCase(120, 120, 30, 77, 154)]
        [TestCase(1920, 1080, 1, 166, 333)]
        [TestCase(2, 2, 1, 1, 2)]
        public void DefaultsInterpolateAndExtrapolate(int width, int height, int fps, int minimum, int maximum)
        {
            var configuration = EncodingController.Resolve(new RealtimeVideoFormat(width, height, fps));

            Assert.AreEqual(minimum, configuration.MinimumBitrate);
            Assert.AreEqual(maximum, configuration.MaximumBitrate);
        }

        [TestCase(1500, 3000, 1500, 3000)]
        [TestCase(0, 500, 0, 500)]
        [TestCase(null, 4000, 1805, 4000)]
        [TestCase(1500, null, 1500, 3611)]
        [TestCase(2000, 2000, 2000, 2000)]
        [TestCase(null, null, 1805, 3611)]
        public void ExplicitBitratesOverrideDefaults(int? minimum, int? maximum, int expectedMinimum, int expectedMaximum)
        {
            var format = new RealtimeVideoFormat(832, 1472, 24, minimum, maximum);
            var configuration = EncodingController.Resolve(format);

            Assert.AreEqual(expectedMinimum, configuration.MinimumBitrate);
            Assert.AreEqual(expectedMaximum, configuration.MaximumBitrate);
        }

        [TestCase(-1, null)]
        [TestCase(null, -1)]
        [TestCase(null, 0)]
        [TestCase(3000, 1500)]
        [TestCase(4000, null)]
        [TestCase(null, 1000)]
        public void InvalidExplicitAndMergedRangesAreRejected(int? minimum, int? maximum)
        {
            var format = new RealtimeVideoFormat(832, 1472, 24, minimum, maximum);
            var exception = Assert.Throws<XmaxException>(() => EncodingController.Resolve(format));

            Assert.AreEqual(XmaxErrorCode.InvalidConfiguration, exception.Code);
        }

        [Test]
        public void InvalidFormatsAndDefaultBitrateOverflowAreRejected()
        {
            var formats = new[]
            {
                default(RealtimeVideoFormat),
                new RealtimeVideoFormat(1023, 768, 30),
                new RealtimeVideoFormat(1024, 767, 30),
                new RealtimeVideoFormat(1024, 768, 0),
                new RealtimeVideoFormat(int.MaxValue - 1, int.MaxValue - 1, int.MaxValue),
                new RealtimeVideoFormat(1024, 768, 30, encoderPreference: (RealtimeVideoEncoderPreference)99)
            };

            foreach (var format in formats)
            {
                var exception = Assert.Throws<XmaxException>(() => EncodingController.Resolve(format));
                Assert.AreEqual(XmaxErrorCode.InvalidConfiguration, exception.Code);
            }
        }

        [Test]
        public void ExplicitRangeDoesNotRequireDefaultBitrateCalculation()
        {
            var format = new RealtimeVideoFormat(int.MaxValue - 1, int.MaxValue - 1, int.MaxValue, 0, int.MaxValue);
            var configuration = EncodingController.Resolve(format);

            Assert.AreEqual(0, configuration.MinimumBitrate);
            Assert.AreEqual(int.MaxValue, configuration.MaximumBitrate);
        }

        [Test]
        public void DefaultBitratesRemainOrderedAndMonotonicAcrossFrameRateBoundaries()
        {
            foreach (var size in new[] { (120, 120), (320, 240), (832, 1472), (1920, 1080), (3840, 2160) })
            {
                var previousMinimum = 0;
                var previousMaximum = 0;
                foreach (var fps in new[] { 1, 9, 10, 11, 14, 15, 16, 24, 29, 30, 31, 59, 60, 61, 120 })
                {
                    var configuration = EncodingController.Resolve(new RealtimeVideoFormat(size.Item1, size.Item2, fps));
                    Assert.Greater(configuration.MinimumBitrate, 0);
                    Assert.Greater(configuration.MaximumBitrate, configuration.MinimumBitrate);
                    Assert.GreaterOrEqual(configuration.MinimumBitrate, previousMinimum);
                    Assert.GreaterOrEqual(configuration.MaximumBitrate, previousMaximum);

                    previousMinimum = configuration.MinimumBitrate;
                    previousMaximum = configuration.MaximumBitrate;
                }
            }
        }

        [TestCase(RealtimeVideoEncoderPreference.Auto, bytertc.VideoEncodePreference.kVideoEncodePreferenceBalance)]
        [TestCase(RealtimeVideoEncoderPreference.MaintainFramerate, bytertc.VideoEncodePreference.kVideoEncodePreferenceFramerate)]
        [TestCase(RealtimeVideoEncoderPreference.MaintainQuality, bytertc.VideoEncodePreference.kVideoEncodePreferenceQuality)]
        public void RtcAdapterPreservesValuesAndMapsPreference(
            RealtimeVideoEncoderPreference preference,
            bytertc.VideoEncodePreference expected)
        {
            var format = new RealtimeVideoFormat(1920, 1024, 30, 0, 4500, preference);
            var native = RtcManager.ToRtcVideoEncoderConfig(EncodingController.Resolve(format));

            Assert.AreEqual(1920, native.Width);
            Assert.AreEqual(1024, native.Height);
            Assert.AreEqual(30, native.FrameRate);
            Assert.AreEqual(0, native.MinBitrate);
            Assert.AreEqual(4500, native.MaxBitrate);
            Assert.AreEqual(expected, native.EncoderPreference);
        }

        [UnityTest]
        public IEnumerator StreamPassesResolvedEncodingToRtc() => AsyncTest.Run(async () =>
        {
            var rtc = new FakeRtc();
            var stream = new StreamController(rtc);
            var format = new RealtimeVideoFormat(1920, 1024, 30, encoderPreference: RealtimeVideoEncoderPreference.MaintainQuality);
            try
            {
                await stream.ConnectAsync(FakeSessions.Session().JoinInfo, format, CancellationToken.None);
                Assert.IsTrue(rtc.EncodingConfiguration.HasValue);
                var configuration = rtc.EncodingConfiguration.Value;
                Assert.AreEqual(3016, configuration.MinimumBitrate);
                Assert.AreEqual(6031, configuration.MaximumBitrate);
                Assert.AreEqual(format.EncoderPreference, configuration.EncoderPreference);
            }
            finally
            {
                await stream.DisconnectAsync();
            }
        });

        [UnityTest]
        public IEnumerator InvalidMergedRangeDoesNotReachRtc() => AsyncTest.Run(async () =>
        {
            var rtc = new FakeRtc();
            var stream = new StreamController(rtc);
            try
            {
                await AsyncTest.Throws<XmaxException>(stream.ConnectAsync(
                    FakeSessions.Session().JoinInfo,
                    new RealtimeVideoFormat(1920, 1024, 30, maximumBitrate: 1000),
                    CancellationToken.None));

                Assert.IsFalse(rtc.EncodingConfiguration.HasValue);
            }
            finally
            {
                await stream.DisconnectAsync();
            }
        });

        [UnityTest]
        public IEnumerator InvalidEncodingDoesNotReplacePreviewOrCreateSession() => AsyncTest.Run(async () =>
        {
            var sessions = new FakeSessions();
            var stream = new FakeStream();
            var manager = new XmaxRealtimeManager(new XmaxConfiguration("test"),
                new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0_Pro)), sessions, stream);
            try
            {
                var local = manager.CreateLocalExternalStream();
                foreach (var format in new[]
                {
                    new RealtimeVideoFormat(1920, 1024, 30, maximumBitrate: 1000),
                    new RealtimeVideoFormat(1920, 1024, 30, minimumBitrate: 7000),
                    new RealtimeVideoFormat(1920, 1024, int.MaxValue),
                    new RealtimeVideoFormat(1920, 1024, 30, encoderPreference: (RealtimeVideoEncoderPreference)99)
                })
                {
                    Assert.Throws<XmaxException>(() => manager.CreateLocalExternalStream(format));
                    await AsyncTest.Throws<XmaxException>(manager.ConnectAsync(format));
                }

                Assert.AreSame(local, manager.LocalStream);
                Assert.AreEqual(0, sessions.Created);
                Assert.AreEqual(0, stream.Joins);

                var explicitFormat = new RealtimeVideoFormat(1920, 1024, 30, 0, 4500, RealtimeVideoEncoderPreference.MaintainFramerate);
                var explicitLocal = manager.CreateLocalExternalStream(explicitFormat);
                await manager.ConnectAsync(explicitLocal);
                Assert.AreEqual(explicitFormat, stream.ConnectedFormat);
                await manager.DisconnectAsync();
                await manager.ConnectAsync(explicitLocal);
                Assert.AreEqual(explicitFormat, stream.ConnectedFormat);
            }
            finally
            {
                await manager.CloseAsync();
            }
        });
    }
}
