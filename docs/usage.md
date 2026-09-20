# XmaxSDK Usage Guide

This guide complements the [README](../README.md) and covers lifecycle
semantics, explicit local streams and general rendering, encoding
configuration, interaction coordinates, and logging and diagnostics in
detail.

## Lifecycle and State

Create managers, call APIs, and handle events on the Unity main thread.
Always `await` asynchronous APIs — never block the Unity main thread with
`.Wait()` or `.Result`.

- `ConnectAsync`: establishes the session and RTC connection. Only one
  connect or generation operation is allowed at a time, and new operations
  are rejected while cleanup is in progress.
- `StartGenerationAsync(context)`: the first generation requires a context;
  the state becomes `Generating` after the task SEI is matched and the first
  valid remote frame arrives.
- `StartGenerationAsync(localStream, context)`: connects on demand with this
  manager's local stream and starts generation, returning the remote stream.
  An existing connection is reused; an active task is reused or updated.
  Passing an explicit `null` context reuses the last successful conditions.
- Passing a context while generating updates the current task's conditions;
  omitting it reuses the current task without restarting.
- `StopGenerationAsync`: cancels a pending generation or stops the current
  task, keeping the connection and the last successful context.
- `DisconnectAsync`: cancels in-flight operations, stops heartbeats,
  releases RTC, and closes the session; the local stream is kept for local
  preview and reconnection.
- `CloseAsync`: disconnects and closes the local stream. Old streams become
  invalid, and the manager can create new local streams for further use.

Creating a manager and a local camera preview stream neither validates the
API key nor creates an online session. The key and service URL are validated
at connect time, so local preview works before a key is configured.

One-call generation shares a single operation across its connect and
generation phases, rejecting other connect/generation operations in the
meantime. A connect-phase failure rolls back the session; a generation
failure after a successful connect keeps the connection and local preview
for retry. Disconnect and close can still cancel the whole operation.

Connection cleanup emits the additional `Disconnecting` state. Repeated
disconnects share the same cleanup task, and a stale operation never
overwrites a newer connection. Exceptions thrown by `StateChanged`
listeners do not interrupt SDK cleanup; with business logging enabled, the
error type is recorded.

Asynchronous fatal errors are delivered through `ErrorOccurred` after
cleanup, with `Severity` set to `Fatal`; direct call failures are thrown by
the returned Task. Failures to close the server session are reported via
`CleanupWarning` while client resources are still released.
`NetworkQualityChanged` provides the same `Uplink` / `Downlink` levels as
iOS and keeps packet loss, RTT, and bandwidth statistics.

`RealtimeState.Reason` is aligned with iOS: the `Disconnected` / `Error`
terminal states carry the reason they were entered —
`RealtimeReason.Normal` for explicit stops and cancellations, and
`RealtimeReason.Failure(error)` carrying the error that ended the workflow
(the same instance delivered through `ErrorOccurred` or the thrown Task).
In-progress states have a null reason, and starting a new operation clears
it. Glasses have no display orientation changes, so the iOS orientation
reason is not provided.

## Explicit Local Streams and General Rendering

```csharp
var client = new XmaxClient(new XmaxConfiguration(apiKey, XmaxEnvironment.China));
IXmaxRealtimeManager manager = client.CreateRealtimeManager(
    new RealtimeConfiguration(Models.Realtime(RealtimeModel.X2_0)));
var format = client.CreateMediaService().RecommendVideoFormat(
    Models.Realtime(RealtimeModel.X2_0), 1920, 1080);
var local = manager.CreateLocalExternalStream(format);
local.VideoTrack.FrameReceived += frame => { /* optional local preview */ };

// Start the host camera first, and call this every frame on the Unity main
// thread:
// local.PushVideoFrame(XmaxVideoFrame.CreateRgba(rgba, width, height));
// The camera must keep feeding frames while waiting for generation.
var remote = await manager.StartGenerationAsync(local,
    new RealtimeContext("A watercolor landscape"));

// The i420Material shader must sample _YTex, _UTex, and _VTex.
// You can reuse your PICO portal material; the SDK handles strides, texture
// upload, and size changes.
var texture = new XmaxVideoTexture();
texture.Bind(remote.VideoTrack, i420Material);

// An explicit context updates the current task; null reuses it.
await manager.StartGenerationAsync(local, null);
await manager.CloseAsync();
texture.Dispose();
```

When the fixed-resolution list is non-empty, width and height must match
exactly, and the pixel bounds and alignment parameters do not participate in
automatic scaling. The Pro model's size recommendation, local stream
creation, and connect entry points all reject other sizes, validated before
creating an online session. A `RecommendVideoFormat` `fps` of 0 uses the
model default frame rate, while an explicit positive rate is preserved; when
passing a `RealtimeVideoFormat` directly, the frame rate must be greater
than zero.

```csharp
var model = RealtimeModel.X2_0_Pro;
var capabilities = model.GetCapabilities();
IXmaxRealtimeManager proManager = client.CreateRealtimeManager(
    new RealtimeConfiguration(Models.Realtime(model)));

// Use the model default format: 1024×1920@30.
var proLocal = proManager.CreateLocalExternalStream();
// The host still captures the camera and keeps pushing frames to proLocal.
// For landscape, create while disconnected:
// proManager.CreateLocalExternalStream(new RealtimeVideoFormat(1920, 1024, 30));
```

`X2_0` remains the default model. The standard model's size recommendation
was adjusted from the previous 24 fps default to 30 fps; integrations that
need 24 fps should pass it explicitly. An explicit
`ConnectAsync(1280, 720, 30)` on the standard model keeps the given size —
size adjustments are made explicitly through `RecommendVideoFormat`.
External input frames may differ in size from the encoding size.

### Upload Encoding Configuration

`RealtimeVideoFormat` supports optional `MinimumBitrate`, `MaximumBitrate`
(in kbps), and `EncoderPreference`. Unspecified bitrates are interpolated
from the final encoding pixel area and frame rate, and out-of-table formats
are extrapolated proportionally. This computation happens at connection
configuration time — it is not a realtime network feedback algorithm.

```csharp
var format = new RealtimeVideoFormat(
    1920, 1024, 30,
    minimumBitrate: 0,
    maximumBitrate: 4500,
    encoderPreference: RealtimeVideoEncoderPreference.MaintainQuality);
var local = proManager.CreateLocalExternalStream(format);
```

`minimumBitrate: 0` means no minimum, while `null` uses the SDK default; an
explicit maximum must be greater than zero. You may override only one side,
but after merging with defaults the minimum must not exceed the maximum —
otherwise an error is raised before creating an online session. The default
encoder preference `Auto` balances frame rate and resolution;
`MaintainFramerate` and `MaintainQuality` are also available.

For example, `1920 × 1024 @ 30 fps` defaults to `3016–6031 kbps`. The
original three-argument constructor and `ConnectAsync(width, height, fps)`
still work, but default bitrates changed from a simple pixel estimate with a
zero minimum to a range computed from the reference table; the actual send
bitrate depends on the encoder and network conditions.

## Interaction Coordinates

Interactions use the encoding pixel coordinates of `SendTracks`;
`InteractionCoordinateMapper.TryMap` maps Fit/Fill viewport coordinates to
encoding pixels, always with a top-left origin. Unity screen coordinates
usually have a bottom-left origin, so the host should flip Y first; camera
rotation or mirroring should also be unmapped beforehand. The texture
uploader exposes `Rotation`; UV rotation, YUV color conversion, and visual
effects in the material remain the host's responsibility.

## Frame Ownership and Resource Release

Video frames wrap existing arrays without copying pixels. Do not modify the
input data while a call is in flight; use `frame.Clone()` when you need to
keep and mutate a frame across callbacks. Call `CloseAsync` on managers that
are no longer used, and `Dispose` on texture uploaders.

## Logging and Diagnostics

Logger options match iOS: `None` (default off), `Business` (API, connection,
generation, and errors), `Performance` (network quality and startup
timing), and `All`. Configuration is an SDK-global option applied only when
a client initializes; creating a manager from an existing client does not
override a newer client's configuration. The compatible
`new XmaxRealtimeManager(...)` entry point creates a client to apply the
configuration. Third-party RTC and Unity's own logging are not affected by
these switches, and logging switches do not affect error events or Task
exceptions.

Internal calls correspond to the iOS categorized instances, for example
`XmaxLogger.Realtime.Info(() => "Connected")` and
`XmaxLogger.Rtc.Warn(() => "Limited", XmaxLoggerOption.Performance)`.
`Debug / Info / Warn / Error` all support the same category options and lazy
message evaluation. In the Unity Console, Debug and Info are emitted as
plain Log, while Warn and Error are emitted as Warning and Error.

Startup timing uses a monotonic clock and covers session creation, RTC room
join, connection, start signaling, SEI matching, and first-frame readiness;
failures record the stage they stopped at, and updating a running task does
not restart the timing. Connecting first and generating later measures only
the generation phase, while one-call generation includes the connect phase.

API logs contain only the request method, route template, HTTP status,
elapsed time, and response byte count. They never include API keys, tokens,
session IDs, prompts, response bodies, or arbitrary exception bodies.

Quality levels include `Unknown / Excellent / Good / Poor / Bad / VeryBad /
Down`. The current Unity RTC package only defines levels up to `VeryBad`;
unknown vendor values map to `Unknown` and `Down` is never guessed. The
package also does not expose the iOS performance limitation/recovery alarm
callbacks, so no performance alarm event is provided at this time.
