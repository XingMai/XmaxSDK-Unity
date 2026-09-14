using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Xmax.SDK.Tests
{
    public sealed class RealtimeModelTests
    {
        [Test]
        public void EveryModelHasUniqueRegistrationAndConsistentLookups()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            var service = new MediaService();
            foreach (RealtimeModel model in Enum.GetValues(typeof(RealtimeModel)))
            {
                var definition = Models.Realtime(model);
                var capabilities = model.GetCapabilities();
                var camera = capabilities.DefaultCameraVideoFormat;

                Assert.IsTrue(names.Add(definition.Name), "Model names must be unique.");
                Assert.AreSame(capabilities, definition.Capabilities);
                Assert.AreSame(capabilities,
                    service.GetCapabilities(new ModelDefinition("  " + definition.Name + "  ")));
                Assert.Greater(capabilities.MinimumPixels, 0);
                Assert.GreaterOrEqual(capabilities.MaximumPixels, capabilities.MinimumPixels);
                Assert.Greater(capabilities.DimensionAlignment, 0);
                Assert.DoesNotThrow(() => camera.Validate());
                Assert.AreEqual(camera, service.RecommendVideoFormat(definition, camera.Width, camera.Height));
            }
        }

        [Test]
        public void ModelRulesUseSuppliedParametersAndCopyResolutionInput()
        {
            var service = new MediaService();
            var camera = new RealtimeVideoFormat(160, 240, 48);
            var capabilities = new RealtimeModelCapabilities(10000, 50000, 16, camera);
            var definition = new ModelDefinition("test-flexible-model", capabilities);
            var recommended = service.RecommendVideoFormat(definition, 100, 100);

            Assert.AreEqual(10000, capabilities.MinimumPixels);
            Assert.AreEqual(50000, capabilities.MaximumPixels);
            Assert.AreEqual(16, capabilities.DimensionAlignment);
            Assert.AreEqual(48, recommended.Fps);
            Assert.That(recommended.Width * recommended.Height, Is.InRange(10000, 50000));
            Assert.AreEqual(0, recommended.Width % 16);
            Assert.AreEqual(0, recommended.Height % 16);

            var sizes = new[] { new RealtimeVideoSize(160, 240) };
            var fixedCapabilities = new RealtimeModelCapabilities(10000, 50000, 16, camera, sizes);
            var fixedModel = new ModelDefinition("test-fixed-model", fixedCapabilities);
            sizes[0] = new RealtimeVideoSize(128, 128);

            Assert.AreEqual(camera, service.RecommendVideoFormat(fixedModel, 160, 240));
            Assert.AreEqual(XmaxErrorCode.InvalidConfiguration,
                Assert.Throws<XmaxException>(() => service.RecommendVideoFormat(fixedModel, 128, 128)).Code);
        }

        [TestCase(RealtimeModel.X2_0, 832, 1472, 1280000)]
        [TestCase(RealtimeModel.X2_0_Pro, 1024, 1920, 2100000)]
        public void ModelDefaultsMatchPublishedSpecificationsAndRemainValidThroughRecommendation(
            RealtimeModel model, int width, int height, int maximumPixels)
        {
            var definition = Models.Realtime(model);
            var service = new MediaService();
            var capabilities = model.GetCapabilities();
            var camera = capabilities.DefaultCameraVideoFormat;

            Assert.AreSame(capabilities, definition.Capabilities);
            Assert.AreSame(capabilities, service.GetCapabilities(new ModelDefinition(definition.Name)));
            Assert.AreEqual(600000, capabilities.MinimumPixels);
            Assert.AreEqual(maximumPixels, capabilities.MaximumPixels);
            Assert.AreEqual(32, capabilities.DimensionAlignment);
            Assert.AreEqual(30, capabilities.DefaultFps);
            Assert.AreEqual(new RealtimeVideoFormat(width, height, 30), camera);
            Assert.AreEqual(camera, service.RecommendVideoFormat(definition, camera.Width, camera.Height));
        }

        [Test]
        public void ResolutionBucketsAreReadOnlyAndDoNotRestrictFrameRate()
        {
            Assert.IsEmpty(RealtimeModel.X2_0.GetCapabilities().ResolutionBuckets);
            var buckets = RealtimeModel.X2_0_Pro.GetCapabilities().ResolutionBuckets;

            CollectionAssert.AreEqual(new[]
            {
                new RealtimeVideoSize(1024, 1920),
                new RealtimeVideoSize(1920, 1024)
            }, buckets);
            Assert.Throws<NotSupportedException>(() =>
                ((IList<RealtimeVideoSize>)buckets)[0] = new RealtimeVideoSize(1, 1));
        }

        [TestCase(1024, 1920, 0, 30)]
        [TestCase(1920, 1024, 0, 30)]
        [TestCase(1024, 1920, 24, 24)]
        [TestCase(1920, 1024, 60, 60)]
        public void ProRecommendationPreservesExactSizeAndExplicitFrameRate(
            int width, int height, int fps, int expectedFps)
        {
            var format = new MediaService().RecommendVideoFormat(
                Models.Realtime(RealtimeModel.X2_0_Pro), width, height, fps);

            Assert.AreEqual(new RealtimeVideoFormat(width, height, expectedFps), format);
        }

        [TestCase(1920, 1080)]
        [TestCase(832, 1472)]
        [TestCase(512, 960)]
        [TestCase(2048, 3840)]
        [TestCase(1120, 1120)]
        [TestCase(1024, 1919)]
        [TestCase(0, 1920)]
        [TestCase(-1, 1920)]
        public void ProRejectsOtherSizesWithoutScalingOrRounding(int width, int height)
        {
            var exception = Assert.Throws<XmaxException>(() => new MediaService().RecommendVideoFormat(
                Models.Realtime(RealtimeModel.X2_0_Pro), width, height));

            Assert.AreEqual(XmaxErrorCode.InvalidConfiguration, exception.Code);
        }

        [Test]
        public void ExplicitFrameRatesArePreservedAndNegativeRecommendationsAreRejected()
        {
            var service = new MediaService();
            foreach (var model in new[] { RealtimeModel.X2_0, RealtimeModel.X2_0_Pro })
            {
                var definition = Models.Realtime(model);
                var camera = definition.Capabilities.DefaultCameraVideoFormat;
                var format = service.RecommendVideoFormat(definition, camera.Width, camera.Height, 24);

                Assert.AreEqual(24, format.Fps);
                var exception = Assert.Throws<XmaxException>(() =>
                    service.RecommendVideoFormat(definition, camera.Width, camera.Height, -1));
                Assert.AreEqual(XmaxErrorCode.InvalidConfiguration, exception.Code);
            }
        }

        [UnityTest]
        public IEnumerator DefaultCameraPreviewWorksWithoutCredentialsForBothModels() => AsyncTest.Run(async () =>
        {
            foreach (var model in new[] { RealtimeModel.X2_0, RealtimeModel.X2_0_Pro })
            {
                IXmaxRealtimeManager manager = new XmaxClient(new XmaxConfiguration("")).CreateRealtimeManager(
                    new RealtimeConfiguration(Models.Realtime(model)));
                try
                {
                    var frames = 0;
                    var local = manager.CreateLocalExternalStream();
                    local.VideoTrack.FrameReceived += frame => frames++;
                    local.PushVideoFrame(AsyncTest.Frame());

                    Assert.AreEqual(model.GetCapabilities().DefaultCameraVideoFormat, local.VideoFormat.Value);
                    Assert.AreEqual(1, frames);
                    Assert.AreEqual(RealtimeConnectionState.Idle, manager.CurrentState.ConnectionState);
                }
                finally
                {
                    await manager.CloseAsync();
                }
            }
        });

        [UnityTest]
        public IEnumerator InvalidProFormatsDoNotReplacePreviewOrCreateSessions() => AsyncTest.Run(async () =>
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
                    new RealtimeVideoFormat(1920, 1080, 30),
                    new RealtimeVideoFormat(1024, 1920, 0)
                })
                {
                    var exception = Assert.Throws<XmaxException>(() => manager.CreateLocalExternalStream(format));
                    Assert.AreEqual(XmaxErrorCode.InvalidConfiguration, exception.Code);
                    await AsyncTest.Throws<XmaxException>(manager.ConnectAsync(format));
                    await AsyncTest.Throws<XmaxException>(manager.ConnectAsync(format.Width, format.Height, format.Fps));
                }

                Assert.AreSame(local, manager.LocalStream);
                Assert.AreEqual(0, sessions.Created);
                Assert.AreEqual(0, stream.Joins);

                await manager.StartGenerationAsync(local, new RealtimeContext("test"));
                Assert.AreEqual("x2.0-pro", sessions.LastModel);
                Assert.AreEqual(local.VideoFormat.Value, stream.ConnectedFormat);
                Assert.AreEqual(RealtimeConnectionState.Generating, manager.CurrentState.ConnectionState);

                await manager.DisconnectAsync();
                var landscape = new RealtimeVideoFormat(1920, 1024, 24);
                await manager.ConnectAsync(landscape);
                Assert.AreEqual(landscape, stream.ConnectedFormat);
                Assert.AreEqual(landscape, manager.LocalStream.VideoFormat.Value);
            }
            finally
            {
                await manager.CloseAsync();
            }
        });

        [UnityTest]
        public IEnumerator CustomModelsKeepExplicitFormatsAndHaveNoImplicitDefaults() => AsyncTest.Run(async () =>
        {
            var model = new ModelDefinition("custom-model");
            var sessions = new FakeSessions();
            var stream = new FakeStream();
            var manager = new XmaxRealtimeManager(new XmaxConfiguration("test"),
                new RealtimeConfiguration(model), sessions, stream);
            try
            {
                Assert.IsNull(model.Capabilities);
                Assert.AreEqual(XmaxErrorCode.NotSupported,
                    Assert.Throws<XmaxException>(() => new MediaService().GetCapabilities(model)).Code);
                Assert.AreEqual(XmaxErrorCode.NotSupported,
                    Assert.Throws<XmaxException>(() => manager.CreateLocalExternalStream()).Code);
                Assert.Throws<ArgumentOutOfRangeException>(() => ((RealtimeModel)99).GetCapabilities());

                var format = new RealtimeVideoFormat(1280, 720, 24);
                var local = manager.CreateLocalExternalStream(format);
                await manager.ConnectAsync(local);

                Assert.AreEqual(format, stream.ConnectedFormat);
                Assert.AreEqual("custom-model", sessions.LastModel);
            }
            finally
            {
                await manager.CloseAsync();
            }
        });
    }
}
