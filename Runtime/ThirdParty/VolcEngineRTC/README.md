# VolcEngine RTC Unity SDK

This directory vendors the Android runtime subset of VolcEngine RTC Unity SDK 3.58.1.

Included runtime artifacts:

- Managed Unity wrapper sources from `Assets/RTCVideo`.
- `VolcEngineRTC-3.58.1.14400.aar`.
- `ByteRTCCWrapper-3.58.1.14400.aar`.

The wrapper AAR manifest namespace is changed to `com.ss.bytertc.unitywrapper`
to avoid an Android Gradle Plugin namespace collision with the main RTC AAR.
The wrapper contains no Android components, and Java package names are unchanged.

Optional beauty, enhancement, Windows, and iOS artifacts from the original Unity package are intentionally excluded from this package.
