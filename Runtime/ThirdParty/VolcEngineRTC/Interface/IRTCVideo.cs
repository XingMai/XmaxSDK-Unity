using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    #region EventHandler
    /** {zh}
     * @type callback
     * @brief 发生警告回调。
     * @notes SDK 运行时出现了（网络或媒体相关的）警告。SDK 通常会自动恢复，警告信息可以忽略。
     * @param warn 警告代码，具体警告参看 WarningCode{@link #WarningCode}。
     */
    /** {en}
     * @type callback
     * @brief Warning callback occurred.
     * @notes SDK A (network or media-related) warning occurred at the runtime. The SDK usually recovers automatically and warnings can be ignored.
     * @param warn See WarningCode{@link #WarningCode}.
     */
    public delegate void OnWarningEventHandler(int warn);

    /** {zh}
     * @type callback
     * @brief 发生错误回调。
     * @notes SDK 运行时出现了（网络或媒体相关的）错误。SDK 通常无法自动恢复，你可能需要干预。
     * @param err 错误代码，具体错误参看 ErrorCode{@link #ErrorCode}。
     */
    /** {en}
     * @type callback
     * @brief Error callback occurred.
     * @notes SDK An error (network or media related) occurred at runtime. The SDK usually does not recover automatically, and you may need to intervene.
     * @param err See ErrorCode{@link #ErrorCode}.
     */
    public delegate void OnErrorEventHandler(int err);

    /** {zh}
     * @type callback
     * @brief 上报日志时回调该事件。
     * @param logType 日志类型。
     * @param logContent 日志内容。
     */
    /** {en}
     * @type callback
     * @brief Callback this event when logging is reported.
     * @param logType Log type.
     * @param logContent  Log content.
     */
    public delegate void OnLogReportEventHandler(string logType, string logContent);

    /** {zh}
     * @type callback
     * @brief 回调 SDK 与信令服务器连接状态相关事件。当 SDK 与信令服务器的网络连接状态改变时回调该事件。
     * @param state  当前 SDK 与信令服务器连接状态。 参看 ConnectionState{@link #ConnectionState}。
     * @notes 在信令服务器连接状态发生改变时触发，并告知用户当前的连接状态，和引起改变的原因。
     */
    /** {en}
     * @type callback
     * @brief Callback SDK and signaling server connection status related events. Callbacks the event when the network connection state of the SDK to the signaling server changes.
     * @param state The current SDK and signaling server connection status. See ConnectionState{@link #ConnectionState}.
     * @notes Triggers when the signaling server connection state changes, and informs the user of the current connection state and the reason for the change.
     */
    public delegate void OnConnectionStateChangedEventHandler(ConnectionState state);

    /** {zh}
     * @type callback
     * @brief SDK 当前网络连接类型改变回调。当 SDK 的当前网络连接类型发生改变时回调该事件。
     * @param type  网络类型。参看 NetworkType{@link #NetworkType}。
     */
    /** {en}
     * @type callback
     * @brief SDK Current network connection type change callback. Callbacks the event when the current network connection type of the SDK changes.
     * @param type Network type. See NetworkType{@link #NetworkType}.
     */
    public delegate void OnNetworkTypeChangedEventHandler(NetworkType type);

    /** {zh}
     * @type callback
     * @brief 每 2 秒发生回调，通知当前 CPU 及内存使用的信息。
     * @param stats CPU 和内存信息。参看 SysStats{@link #SysStats}。
     */
    /** {en}
     * @type callback
     * @brief Callbacks occur every 2 seconds to inform the current CPU and memory usage information.
     * @param stats CPU and memory information. See SysStats{@link #SysStats}.
     */
    public delegate void OnSysStatsEventHandler(SysStats stats);

    // audio
    /** {zh}
     * @type callback
     * @brief 音频播放路由变化时，收到该回调。
     * @param device 新的音频播放路由，参看 AudioRouteDevice{@link #AudioRouteDevice}。
     * @notes 关于音频路由设置，参看 SetAudioRoute{@link #IRTCVideo#SetAudioRoute}
     */
    /** {en}
     * @type callback
     * @brief Receives this callback when the audio playback route changes.
     * @param device New audio playback route. See AudioRouteDevice{@link #AudioRouteDevice}.
     * @notes To set the audio playback route, see SetAudioRoute{@link #IRTCVideo#SetAudioRoute}.
     */
    public delegate void OnAudioRouteChangedEventHandler(AudioRouteDevice device);

    /** {zh}
     * @type callback
     * @brief 音频设备状态回调。提示音频采集、音频播放等媒体设备的状态。
     * @param deviceID 设备 ID
     * @param deviceType 设备类型，详见 RTCAudioDeviceType{@link #RTCAudioDeviceType}。
     * @param deviceState 设备状态，详见 MediaDeviceState{@link #MediaDeviceState}。
     * @param deviceError 设备错误类型，详见 MediaDeviceError{@link #MediaDeviceError}。
     */
    /** {en}
     * @type callback
     * @brief Audio device state callback, returns the state of audio capture and audio playback devices.
     * @param deviceID Device ID
     * @param deviceType Device type. See RTCAudioDeviceType{@link #RTCAudioDeviceType}.
     * @param deviceState Device state. See MediaDeviceState{@link #MediaDeviceState}.
     * @param deviceError Device error. See MediaDeviceError{@link #MediaDeviceError}.
     */
    public delegate void OnAudioDeviceStateChangedEventHandler(string deviceID, RTCAudioDeviceType deviceType, MediaDeviceState deviceState, MediaDeviceError deviceError);

    /** {zh}
     * @type callback
     * @brief 媒体设备警告回调。媒体设备包括：音频采集设备、音频渲染设备、和视频采集设备。
     * @param deviceID 设备 ID
     * @param deviceType 参看 RTCAudioDeviceType{@link #RTCAudioDeviceType}
     * @param deviceWarning 参看 MediaDeviceWarning{@link #MediaDeviceWarning}
     */
    /** {en}
     * @type callback
     * @brief Media device warning callback. The media devices includes: audio capture devices, audio rendering devices, and video capture devices.
     * @param deviceID Device ID
     * @param deviceType see RTCAudioDeviceType{@link #RTCAudioDeviceType}
     * @param deviceWarning see MediaDeviceWarning{@link #MediaDeviceWarning}
     */
    public delegate void OnAudioDeviceWarningEventHandler(string deviceID, RTCAudioDeviceType deviceType, MediaDeviceWarning deviceWarning);

    /** {zh}
     * @type callback
     * @brief 调用 EnableAudioPropertiesReport{@link #IRTCVideo#EnableAudioPropertiesReport} 后，根据设置的 Interval 值，你会周期性地收到此回调，了解本地音频的相关信息。
     *        本地音频包括使用 RTC SDK 内部机制采集的麦克风音频和屏幕音频。
     * @param audioPropertiesInfos 本地音频信息，详见 LocalAudioPropertiesInfo{@link #LocalAudioPropertiesInfo}。
     * @param audioPropertiesInfoNumber 本地音频信息编号。
     */
    /** {en}
     * @type callback
     * @brief After calling EnableAudioPropertiesReport{@link #IRTCVideo#EnableAudioPropertiesReport}, you will periodically receive this callback for the information about local audio. <br>
     *        Local audio includes the microphone audio and the screen audio captured using RTC SDK internal mechanisms.
     * @param audioPropertiesInfos Local audio properties information. See LocalAudioPropertiesInfo{@link #LocalAudioPropertiesInfo}.
     * @param audioPropertiesInfoNumber Audio properties information number.
     */
    public delegate void OnLocalAudioPropertiesReportEventHandler(List<LocalAudioPropertiesInfo> audioPropertiesInfos, int audioPropertiesInfoNumber);

    /** {zh}
     * @type callback
     * @brief 远端用户进房后，本地调用 EnableAudioPropertiesReport{@link #IRTCVideo#EnableAudioPropertiesReport} ，根据设置的 interval 值，本地会周期性地收到此回调，了解订阅的远端用户的音频信息。<br>
     *        远端用户的音频包括使用 RTC SDK 内部机制/自定义机制采集的麦克风音频和屏幕音频。
     * @param audioPropertiesInfos 远端音频信息，其中包含音频流属性、房间 ID、用户 ID ，详见 RemoteAudioPropertiesInfo{@link #RemoteAudioPropertiesInfo}。  <br>
     * @param audioPropertiesInfoNumber 本地音频信息编号。
     * @param totalRemoteVolume 订阅的所有远端流的总音量。
     */
    /** {en}
     * @type callback
     * @brief After calling EnableAudioPropertiesReport{@link #IRTCVideo#EnableAudioPropertiesReport}, you will periodically receive this callback for the information about the subscribed remote audio streams. <br>
     *        The remote audio streams includes the microphone audio and screen audio collected using the RTC SDK internal mechanism/custom mechanism.
     * @param audioPropertiesInfos See RemoteAudioPropertiesInfo{@link #RemoteAudioPropertiesInfo}.
     * @param audioPropertiesInfoNumber Audio properties information number.
     * @param totalRemoteVolume The total volume of all the subscribed remote streams
     */
    public delegate void OnRemoteAudioPropertiesReportEventHandler(List<RemoteAudioPropertiesInfo> audioPropertiesInfos, int audioPropertiesInfoNumber, int totalRemoteVolume);

    /** {zh}
    * @type callback
    * @region 引擎管理
    * @author shenpengliang
    * @brief 创建房间失败回调。
    * @param room_id 房间 ID。
    * @param error_code 创建房间错误码，具体原因参看 ErrorCode{@link #ErrorCode}。
    */
    /** {en}
     * @type callback
     * @region Engine Management
     * @author shenpengliang
     * @brief Callback on create room failure.
     * @param room_id Room ID.
     * @param error_code Create room error code. See ErrorCode{@link #ErrorCode} for specific indications.
     */
    public delegate void OnCreateRoomStateChangedEventHandler(string room_id, int error_code);

    /** {zh}
     *  @type callback
     *  @author huangshouqin
     *  @brief 调用 StartAudioRecording{@link #IRTCVideo#StartAudioRecording} 或 StopAudioRecording{@link #IRTCVideo#StopAudioRecording} 改变音频文件录制状态时，收到此回调。
     *  @param [in] state 录制状态，参看 AudioRecordingState{@link #AudioRecordingState}
     *  @param [in] error_code 录制错误码，参看 AudioRecordingErrorCode{@link #AudioRecordingErrorCode}
     */
    /** {en}
     *  @type callback
     *  @author huangshouqin
     *  @brief When calling StartAudioRecording{@link #IRTCVideo#StartAudioRecording} or StopAudioRecording{@link #IRTCVideo#StopAudioRecording} to change the recording status, receive the callback.
     *  @param [in] state See AudioRecordingState{@link #AudioRecordingState}.
     *  @param [in] error_code See AudioRecordingErrorCode{@link #AudioRecordingErrorCode}.
     */
    public delegate void OnAudioRecordingStateUpdateEventHandler(AudioRecordingState state, AudioRecordingErrorCode error_code);

    /**
    * @hidden for internal use only
    * @valid since 3.50
    * @author liujingchao
    */
    public delegate void OnRecordAudioFrameOriginalEventHandler(AudioFrame audioFrame);

    /** {zh}
     * @type callback
     * @region 音频数据回调
     * @author wangjunzheng
     * @brief 返回麦克风录制的音频数据
     * @param audioFrame 音频数据, 参看 AudioFrame{@link #AudioFrame}。
     */
    /** {en}
     * @type callback
     * @region Audio Data Callback
     * @author wangjunzheng
     * @brief Returns the audio data recorded by local microphone.
     * @param audioFrame Audio data. See AudioFrame{@link #AudioFrame}.
     */
    public delegate void OnRecordAudioFrameEventHandler(AudioFrame audioFrame);

    /** {zh}
    * @type callback
    * @region 音频数据回调
    * @author wangjunzheng
    * @brief 返回订阅的所有远端用户混音后的音频数据。
    * @param audioFrame 音频数据, 参看 AudioFrame{@link #AudioFrame}。
    */
    /** {en}
     * @type callback
     * @region Audio Data Callback
     * @author wangjunzheng
     * @brief Returns the mixed audio data of all remote users subscribed by the local user.
     * @param audioFrame Audio data. See AudioFrame{@link #AudioFrame}.
     */
    public delegate void OnPlaybackAudioFrameEventHandler(AudioFrame audioFrame);

    /** {zh}
     * @type callback
     * @region 音频数据回调
     * @author wangjunzheng
     * @brief 返回远端单个用户的音频数据
     * @param streamInfo 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}。
     * @param audioFrame 音频数据，参看 AudioFrame{@link #AudioFrame}。
     * @notes 此回调在播放线程调用。不要在此回调中做任何耗时的事情，否则可能会影响整个音频播放链路。
     */
    /** {en}
     * @type callback
     * @region Audio Data Callback
     * @author wangjunzheng
     * @brief Returns the audio data of one remote user.
     * @param streamInfo Remote stream information. See RemoteStreamKey{@link #RemoteStreamKey}.
     * @param audioFrame Audio data. See AudioFrame{@link #AudioFrame}.
     * @notes This callback works on the playback thread. Don't do anything time-consuming in this callback, or it may affect the entire audio playback chain.
     */
    public delegate void OnRemoteUserAudioFrameEventHandler(RemoteStreamKey streamInfo, AudioFrame audioFrame);

    /** {zh}
     * @type callback
     * @region 音频数据回调
     * @author wangjunzheng
     * @brief 返回本地麦克风录制和订阅的所有远端用户混音后的音频数据
     * @param audioFrame 音频数据, 参看 AudioFrame{@link #AudioFrame}。
     */
    /** {en}
     * @type callback
     * @region Audio Data Callback
     * @author wangjunzheng
     * @brief Returns the mixed audio data including the data recorded by local microphone and that of all remote users subscribed by the local user.
     * @param audioFrame Audio data. See AudioFrame{@link #AudioFrame}.
     */
    public delegate void OnMixedAudioFrameEventHandler(AudioFrame audioFrame);

    /** {zh}
* @type callback
* @region 屏幕音频数据回调
* @brief 返回本地屏幕录制的音频数据
* @param [in] audio_frame 音频数据, AudioFrame{@link #AudioFrame}
*/
    /** {en}
     * @type callback
     * @region Screen audio data callback
     * @brief Returns the audio data played locally
      * @param [in] audio_frame 音频数据，参看 AudioFrame{@link #AudioFrame}
     */
    public delegate void OnRecordScreenAudioFrameEventHandler(AudioFrame audioFrame);
    // video
    /** {zh}
     * @type callback
     * @brief 视频设备状态回调。提示摄像头视频采集、屏幕视频采集等媒体设备的状态。
     * @param deviceID 设备 ID。
     * @param deviceType 设备类型，详见 RTCVideoDeviceType{@link #RTCVideoDeviceType}。
     * @param deviceState 设备状态，详见 MediaDeviceState{@link #MediaDeviceState}。
     * @param deviceError 设备错误类型，详见 MediaDeviceError{@link #MediaDeviceError}。
     */
    /** {en}
     * @type callback
     * @brief Video device state callback, returns the state of video capture and screen capture devices.
     * @param deviceID Device ID.
     * @param deviceType Device type. See RTCVideoDeviceType{@link #RTCVideoDeviceType}.
     * @param deviceState Device state. See MediaDeviceState{@link #MediaDeviceState}.
     * @param deviceError Device error. See MediaDeviceError{@link #MediaDeviceError}.
     */
    public delegate void OnVideoDeviceStateChangedEventHandler(string deviceID, RTCVideoDeviceType deviceType, MediaDeviceState deviceState, MediaDeviceError deviceError);

    /** {zh}
     * @type callback
     * @brief 视频设备警告回调，包括视频采集等设备。
     * @param deviceID 设备 ID
     * @param deviceType 参看 RTCVideoDeviceType{@link #RTCVideoDeviceType}
     * @param deviceWarning 参看 MediaDeviceWarning{@link #MediaDeviceWarning}
     */
    /** {en}
     * @type callback
     * @brief Video device warning callback, including video capture devices.
     * @param deviceID Device ID
     * @param deviceType See RTCVideoDeviceType{@link #RTCVideoDeviceType}
     * @param deviceWarning See MediaDeviceWarning{@link #MediaDeviceWarning}
     */
    public delegate void OnVideoDeviceWarningEventHandler(string deviceID, RTCVideoDeviceType deviceType, MediaDeviceWarning deviceWarning);


    // local video sink
    /** {zh}
     * @type callback
     * @brief 自定义渲染器本地视频帧回调。
     * @param index 视频流属性，参看 StreamIndex{@link #StreamIndex}。
     * @param videoFrame 视频帧结构类，参看 VideoFrame{@link #VideoFrame}。
     */
    /** {en}
     * @type callback
     * @brief Video frame callback from the cunstom video sink.
     * @param index Stream index. See StreamIndex{@link #StreamIndex}.
     * @param videoFrame Video frame structure. See VideoFrame{@link #VideoFrame}.
     */
    public delegate bool OnLocalVideoSinkOnFrameEventHandler(StreamIndex index, VideoFrame videoFrame);

    /** {zh}
     * @type callback
     * @brief 自定义渲染本地视频帧耗时。
     * @param index 视频流属性，参看 StreamIndex{@link #StreamIndex}。
     * @return 外部渲染平均耗时，单位：毫秒。
     */
    /** {en}
     * @type callback
     * @brief Getting external rendering takes time.
     * @param index Stream index. See StreamIndex{@link #StreamIndex}.
     * @return Average external rendering time in milliseconds.
     */
    public delegate int OnLocalVideoSinkGetRenderElapseEventHandler(StreamIndex index);

    /** {zh}
     * @type callback
     * @brief 释放本地自定义渲染器。
     * @notes 通知开发者渲染器即将被废弃。收到该返回通知后即可释放资源。
     */
    /** {en}
     * @type callback
     * @brief Releases the renderer.
     * @notes Used to notify the user that the renderer is about to be deprecated. Resources can be released upon receipt of this notification.
     */
    public delegate void OnLocalVideoSinkReleaseEventHandler(StreamIndex index);

    // remote video sink
    /** {zh}
     * @type callback
     * @brief 自定义渲染器远端视频帧回调。
     * @param key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}。
     * @param videoFrame 视频帧结构类，参看 VideoFrame{@link #VideoFrame}。
     */
    /** {en}
     * @type callback
     * @brief Video frame callback from the cunstom video sink.
     * @param key Remote stream information. See RemoteStreamKey{@link #RemoteStreamKey}.
     * @param videoFrame Video frame structure. See VideoFrame{@link #VideoFrame}.
     */
    public delegate bool OnRemoteVideoSinkOnFrameEventHandler(RemoteStreamKey key, VideoFrame videoFrame);

    /** {zh}
     * @type callback
     * @brief 自定义渲染远端视频帧耗时。
     * @param key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}。
     * @return 外部渲染平均耗时，单位：毫秒。
     */
    /** {en}
     * @type callback
     * @brief Getting external rendering takes time.
     * @param key Remote stream information. See RemoteStreamKey{@link #RemoteStreamKey}.
     * @return Average external rendering time in milliseconds.
     */
    public delegate int OnRemoteVideoSinkGetRenderElapseEventHandler(RemoteStreamKey key);

    /** {zh}
     * @type callback
     * @brief 释放远端自定义渲染器。
     * @notes 通知开发者渲染器即将被废弃。收到该返回通知后即可释放资源。
     */
    /** {en}
     * @type callback
     * @brief Releases the renderer.
     * @notes Used to notify the user that the renderer is about to be deprecated. Resources can be released upon receipt of this notification.
     */
    public delegate void OnRemoteVideoSinkReleaseEventHandler(RemoteStreamKey key);

    // room audio
    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StartAudioCapture{@link #IRTCVideo#StartAudioCapture} 开启音频采集时，房间内其他用户会收到此回调。
     * @param roomId 开启音频采集的远端用户所在的房间 ID。
     * @param userID 开启音频采集的远端用户 ID。
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via this callback after the visible user starts audio capture by calling StartAudioCapture{@link #IRTCVideo#StartAudioCapture}.
     * @param roomId ID of the room where the remote user enables audio capture.
     * @param userID The user who starts the internal audio capture.
     */
    public delegate void OnUserStartAudioCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StopAudioCapture{@link #IRTCVideo#StopAudioCapture} 关闭音频采集时，房间内其他用户会收到此回调。
     * @param roomId 关闭音频采集的远端用户所在的房间 ID。
     * @param userID 关闭音频采集的远端用户 ID。
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via this callback after the visible user stops audio capture by calling StopAudioCapture{@link #IRTCVideo#StopAudioCapture}.
     * @param roomId ID of the room where the remote user disables audio capture.
     * @param userID The user who stops the internal audio capture
     */
    public delegate void OnUserStopAudioCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief 本地采集到第一帧音频帧时，收到该回调。
     * @param index 音频流属性, 参看 StreamIndex{@link #StreamIndex}
     */
    /** {en}
     * @type callback
     * @brief Receive the callback when the first audio frame is locally collected.
     * @param index  Audio stream properties. See StreamIndex{@link #StreamIndex}
     */
    public delegate void OnFirstLocalAudioFrameEventHandler(StreamIndex index);

    /** {zh}
     * @type callback
     * @brief 本地音频流的状态发生改变时，该回调通知当前的本地音频流状态。
     * @param state 本地音频设备的状态，详见 LocalAudioStreamState{@link #LocalAudioStreamState}
     * @param error 本地音频流状态改变时的错误码，详见 LocalAudioStreamError{@link #LocalAudioStreamError}
     */
    /** {en}
     * @type callback
     * @brief When the state of the local audio stream changes, the callback notifies the current local audio stream state.
     * @param state The status of the local audio device. See LocalAudioStreamState{@link #LocalAudioStreamState}
     * @param error The error code when the state of the local audio stream changes. See LocalAudioStreamError{@link #LocalAudioStreamError}
     */
    public delegate void OnLocalAudioStateChangedEventHandler(LocalAudioStreamState state, LocalAudioStreamError error);

    /** {zh}
     * @type callback
     * @brief 本地音频首帧发送状态发生改变时，收到此回调。
     * @param roomId 音频发布用户所在的房间 ID。
     * @param user 本地用户信息，详见 RtcUser{@link #RtcUser}。
     * @param state 首帧发送状态，详见 FirstFrameSendState{@link #FirstFrameSendState}。
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the sending status of the first frame of the local audio changes.
     * @param roomId ID of the room from which the audio is published.
     * @param user Local user information. See RtcUser{@link #RtcUser}.
     * @param state First frame sending status. See FirstFrameSendState{@link #FirstFrameSendState}.
     */
    public delegate void OnAudioFrameSendStateChangedEventHandler(string roomID, RtcUser user, FirstFrameSendState state);

    /** {zh}
     * @type callback
     * @brief 音频首帧播放状态发生改变时，收到此回调
     * @param roomId 音频发布用户所在的房间 ID。
     * @param user 远端用户信息，详见 RtcUser{@link #RtcUser}
     * @param state 首帧播放状态，详见 FirstFramePlayState{@link #FirstFramePlayState}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the audio first frame playback state changes.
     * @param roomId ID of the room from which the audio is published.
     * @param user Remote user information. See RtcUser{@link #RtcUser}
     * @param state First frame playback status. See FirstFramePlayState{@link #FirstFramePlayState}
     */
    public delegate void OnAudioFramePlayStateChangedEventHandler(string roomID, RtcUser user, FirstFramePlayState state);

    /** {zh}
     * @type callback
     * @author majun.lvhiei
     * @brief 回调本地采集的音频帧地址，供自定义音频处理。
     * @param audioFrameCallback 音频帧地址，参看 AudioFrameCallback{@link #AudioFrameCallback}
     * @notes <br>
     *        + 完成自定义音频处理后，SDK 会对处理后的音频帧进行编码，并传输到远端。如果开启了耳返功能，那么对耳返音频也会生效。<br>
     *        + 调用 `enableAudioProcessor`，并在参数中选择本地采集的音频时，收到此回调。
     */
    /** {en}
     * @type callback
     * @author majun.lvhiei
     * @brief Returns the address of the locally captured audio frame for custom processing.
     * @param audioFrameCallback The address of the audio frame. See AudioFrameCallback{@link #AudioFrameCallback}
     * @notes <br>
     *        + After custom processing, SDK will encode the processed audio frames and transmit to the remote user. If you enabled ear monitoring, the ear-monitoring audio will be processed.<br>
     *        + After calling `enableAudioProcessor` with locally captured audio frame specified, you will receive this callback.
     */
    public delegate int OnProcessRecordAudioFrameEventHandler(AudioFrameCallback audioFrameCallback);

    /** {zh}
     * @type callback
     * @author majun.lvhiei
     * @brief 回调远端音频混音的音频帧地址，供自定义音频处理。
     * @param audioFrameCallback 音频帧地址，参看 AudioFrameCallback{@link #AudioFrameCallback}
     * @notes 调用 `enableAudioProcessor`，并在参数中选择远端音频流的的混音音频时，收到此回调。
     */
    /** {en}
     * @type callback
     * @author majun.lvhiei
     * @brief Returns the address of the locally captured audio frame for custom processing.
     * @param audioFrameCallback The address of the audio frame. See AudioFrameCallback{@link #AudioFrameCallback}
     * @notes After calling `enableAudioProcessor` with mixed remote audio, you will receive this callback.
     */
    public delegate int OnProcessPlaybackAudioFrameEventHandler(AudioFrameCallback audioFrameCallback);

    /** {zh}
     * @type callback
     * @author majun.lvhiei
     * @brief 回调单个远端用户的音频帧地址，供自定义音频处理。
     * @param streamInfo 音频流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}
     * @param audioFrameCallback 音频帧地址，参看 AudioFrameCallback{@link #AudioFrameCallback}
     * @notes 调用 `enableAudioProcessor`，并在参数中选择各个远端音频流时，收到此回调。
     */
    /** {en}
     * @type callback
     * @author majun.lvhiei
     * @brief Returns the address of the locally captured audio frame for custom processing.
     * @param streamInfo Information of the audio stream. See RemoteStreamKey{@link #RemoteStreamKey}
     * @param audioFrameCallback The address of the audio frame. See AudioFrameCallback{@link #AudioFrameCallback}
     * @notes After calling `enableAudioProcessor` with audio streams of the remote users, you will receive this callback.
     */
    public delegate int OnProcessRemoteUserAudioFrameEventHandler(RemoteStreamKey streamInfo, AudioFrameCallback audioFrameCallback);

    /** {zh}
     * @type callback
     * @author songxiaomeng.19
     * @brief 回调耳返数据的音频帧地址，供自定义音频处理。
     * @param audioFrameCallback 音频帧地址，参看 AudioFrameCallback{@link #AudioFrameCallback}
     * @notes <br>
     *        + 此数据处理只影响耳返音频数据。<br>
     *        + 调用 `enableAudioProcessor`，并在参数中选择耳返音频时，收到此回调。
     */
    /** {en}
     * @type callback
     * @author songxiaomeng.19
     * @brief Returns the address of the ear monitor audio frame for custom processing.
     * @param audioFrameCallback The address of the audio frame. See audioFrameCallback{@link #audioFrameCallback}
     * @notes <br>
     *        + This data process only affect ear monitor audio data.<br>
     *        + After calling `enableAudioProcessor` with ear monitor audio frame specified, you will receive this callback.
     */
    public delegate int OnProcessEarMonitorAudioFrameEventHandler(AudioFrameCallback audioFrameCallback);

    /** {zh}
     * @type callback
     * @author zhangcaining
     * @brief 屏幕共享的音频帧地址回调。你可根据此回调自定义处理音频。
     * @param audioFrameCallback 音频帧地址，参看 AudioFrameCallback{@link #AudioFrameCallback}
     * @notes 调用 `enableAudioProcessor`，把返回给音频处理器的音频类型设置为屏幕共享音频后，你将收到此回调。
     */
    /** {en}
     * @type callback
     * @author zhangcaining
     * @brief Returns the address of the shared-screen audio frames for custom processing.
     * @param audioFrameCallback The address of the audio frame. See AudioFrameCallback{@link #AudioFrameCallback}
     * @notes After calling `enableAudioProcessor` to set the audio input to the shared-screen audio, you will receive this callback.
     */
    public delegate int OnProcessScreenAudioFrameEventHandler(AudioFrameCallback audioFrameCallback);

    /** {zh}
 * @type callback
 * @region 音频管理
 * @author wangjunzheng
 * @brief 音频流同步信息回调。可以通过此回调，在远端用户调用 SendStreamSyncInfo{@link #IRTCVideo#SendStreamSyncInfo} 发送音频流同步消息后，收到远端发送的音频流同步信息。  <br>
 * @param streamKey 远端流信息，详见 RemoteStreamKey{@link #RemoteStreamKey} 。
 * @param streamType 媒体流类型，详见 SyncInfoStreamType{@link #SyncInfoStreamType} 。
 * @param buffer 消息内容。
 */
    /** {en}
     * @type callback
     * @region audio management
     * @author wangjunzheng
     * @brief audio stream synchronization information callback. You can use this callback to receive audio stream synchronization information sent remotely after the remote user calls SendStreamSyncInfo{@link #IRTCVideo#SendStreamSyncInfo} to send an audio stream synchronization message. <br>
     * @param streamKey  For remote stream information. See RemoteStreamKey{@link #RemoteStreamKey}.
     * @param StreamType Media stream type. See SyncInfoStreamType{@link #SyncInfoStreamType}.
     * @param buffer Message content.
     */
    public delegate void OnStreamSyncInfoReceivedEventHandler(RemoteStreamKey stream_key, SyncInfoStreamType stream_type, byte[] buffer);

    /** {zh}
     * @type callback
     * @region 视频管理
     * @author wangzhanqiang
     * @brief 收到通过调用 SendSEIMessage{@link #IRTCVideo#SendSEIMessage} 发送带有 SEI 消息的视频帧时，收到此回调。
     * @param stream_key 包含 SEI 发送者的用户名，所在的房间名和媒体流，详见 RemoteStreamKey{@link #RemoteStreamKey}
     * @param buffer 收到的 SEI 消息内容
     */
    /** {en}
     * @type callback
     * @region Video management
     * @author wangzhanqiang
     * @brief Receive this callback when you receive a video frame with a SEI message sent by calling SendSEIMessage{@link #IRTCVideo#SendSEIMessage}
     * @param stream_key Contains the user name, room name and media stream of the SEI sender, as detailed in RemoteStreamKey{@link #RemoteStreamKey}
     * @param buffer Received SEI message content
    */
    public delegate void OnSEIMessageReceivedEventHandler(RemoteStreamKey stream_key, byte[] buffer);

        /** {zh}
     * @type callback
     * @region 消息
     * @author wangzhanqiang
     * @brief 黑帧视频流发布状态回调。  <br>
     *        在语音通话场景下，本地用户调用 SendSEIMessage{@link #IRTCVideo#SendSEIMessage} 通过黑帧视频流发送 SEI 数据时，流的发送状态会通过该回调通知远端用户。  <br>
     *        你可以通过此回调判断携带 SEI 数据的视频帧为黑帧，从而不对该视频帧进行渲染。
     * @param stream_key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}。
     * @param type 黑帧视频流状态，参看 SEIStreamEventType{@link #SEIStreamEventType}
     */
    /** {en}
     * @type callback
     * @region Message
     * @author wangzhanqiang
     * @brief Callback about publishing status of the black frame video stream .  <br>
     *        In a voice call scenario, when the local user calls SendSEIMessage{@link #IRTCVideo#SendSEIMessage} to send SEI data with a black frame, the sending status of the stream is notified to the remote user through this callback.  <br>
     *        You can tell from this callback that the video frame carrying SEI data is a black frame and thus not render that video frame.
     * @param stream_key Information about stream from the remote user, see RemoteStreamKey{@link #RemoteStreamKey}.
     * @param type State of the black frame video stream, see SEIStreamEventType{@link #SEIStreamEventType}.
     */
    public delegate void OnSEIMessageUpdateEventHandler(RemoteStreamKey stream_key, SEIStreamEventType type);

    // room video
    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StartVideoCapture{@link #IRTCVideo#StartVideoCapture} 开启内部视频采集时，房间内其他用户会收到此回调。
     * @param roomId 开启视频采集的远端用户所在的房间 ID。
     * @param userID 开启视频采集的远端用户 ID。
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via this callback after the visible user starts video capture by calling StartVideoCapture{@link #IRTCVideo#StartVideoCapture}.
     * @param roomId ID of the room where the remote user enables video capture.
     * @param userID The user who starts the internal video capture.
     */
    public delegate void OnUserStartVideoCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StopVideoCapture{@link #IRTCVideo#StopVideoCapture} 关闭内部视频采集时，房间内其他用户会收到此回调。
     * @param uid 关闭视频采集的远端用户 ID
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via  this callback after the visible user stops video capture by calling StopVideoCapture{@link #IRTCVideo#StopVideoCapture}.
     * @param uid The user who stops the internal video capture.
     */
    public delegate void OnUserStopVideoCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief RTC SDK 在本地完成第一帧视频帧或屏幕视频帧采集时，收到此回调。
     * @param index 流属性，参看 StreamIndex{@link #StreamIndex}
     * @param info 视频信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
     * @notes 对于采集到的本地视频帧，你可以调用 SetLocalVideoSink{@link #IRTCVideo#SetLocalVideoSink} 在本地渲染。
     */
    /** {en}
     * @type callback
     * @brief RTC SDK receives this callback when the first video frame or screen video frame capture is completed locally.
     * @param index Stream properties. See StreamIndex{@link #StreamIndex}
     * @param info Video information. See VideoFrameInfo{@link #VideoFrameInfo}
     * @notes For captured local video frames, you can call SetLocalVideoSink{@link #IRTCVideo#SetLocalVideoSink} to render locally.
     */
    public delegate void OnFirstLocalVideoFrameCapturedEventHandler(StreamIndex index, VideoFrameInfo info);

    /** {zh}
     * @type callback
     * @brief 本地视频大小或旋转信息发生改变时，收到此回调。
     * @param index 流属性，参看 StreamIndex{@link #StreamIndex}。
     * @param info 视频帧信息，参看 VideoFrameInfo{@link #VideoFrameInfo}。
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when local video size or rotation information changes.
     * @param index Stream properties. See StreamIndex{@link #StreamIndex}.
     * @param info Video frame information. See VideoFrameInfo{@link #VideoFrameInfo}.
     */
    public delegate void OnLocalVideoSizeChangedEventHandler(StreamIndex index, VideoFrameInfo info);

    /** {zh}
     * @type callback
     * @brief 本地视频流的状态发生改变时，收到该事件。
     * @param index 流属性，参看 StreamIndex{@link #StreamIndex}。
     * @param state 本地视频流状态，参看 LocalVideoStreamState{@link #LocalVideoStreamState}。
     * @param error 本地视频状态改变时的错误码，参看 LocalVideoStreamError{@link #LocalVideoStreamError}。
     */
    /** {en}
     * @type callback
     * @brief Receive this event when the state of the local video stream changes.
     * @param index Stream property. See StreamIndex{@link #StreamIndex}.
     * @param state Local video stream state. See LocalVideoStreamState{@link #LocalVideoStreamState}.
     * @param error Error code when local video state changes. See LocalVideoStreamError{@link #LocalVideoStreamError}.
     */
    public delegate void OnLocalVideoStateChangedEventHandler(StreamIndex index, LocalVideoStreamState state, LocalVideoStreamError error);

    /** {zh}
     * @type callback
     * @brief 本地视频首帧发送状态发生改变时，收到此回调。
     * @param roomId 视频发布用户所在的房间 ID。
     * @param user 本地用户信息，详见 RtcUser{@link #RtcUser}。
     * @param state 首帧发送状态，详见 FirstFrameSendState{@link #FirstFrameSendState}。
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the sending status of the first frame of the local video changes.
     * @param roomId ID of the room from which the video is published.
     * @param user Local user information. See RtcUser{@link #RtcUser}.
     * @param state First frame sending status. See FirstFrameSendState{@link #FirstFrameSendState}.
     */
    public delegate void OnVideoFrameSendStateChangedEventHandler(string roomID, RtcUser user, FirstFrameSendState state);

    /** {zh}
     * @type callback
     * @brief 屏幕共享流的视频首帧发送状态发生改变时，收到此回调。
     * @param roomId 屏幕视频发布用户所在的房间 ID。
     * @param user 本地用户信息，详见 RtcUser{@link #RtcUser}。
     * @param state 首帧发送状态，详见 FirstFrameSendState{@link #FirstFrameSendState}。
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the video first frame sending state of the screen sharing stream changes.
     * @param roomId ID of the room from which the screen video frame is published.
     * @param user Local user information. See RtcUser{@link #RtcUser}.
     * @param state First frame sending status. See FirstFrameSendState{@link #FirstFrameSendState}.
     */
    public delegate void OnScreenVideoFrameSendStateChangedEventHandler(string roomID, RtcUser user, FirstFrameSendState state);

    /** {zh}
     * @type callback
     * @brief 视频首帧播放状态发生改变时，收到此回调。
     * @param roomId 视频发布用户所在的房间 ID。
     * @param user 远端用户信息，详见 RtcUser{@link #RtcUser}。
     * @param state 首帧播放状态，详见 FirstFramePlayState{@link #FirstFramePlayState}。
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the playback state of the first frame of the video changes.
     * @param roomId ID of the room from which the video is published.
     * @param user Remote user information. See RtcUser{@link #RtcUser}.
     * @param state First frame playback status. See FirstFramePlayState{@link #FirstFramePlayState}.
     */
    public delegate void OnVideoFramePlayStateChangedEventHandler(string roomID, RtcUser user, FirstFramePlayState state);

    /** {zh}
    * @type callback
    * @brief 远端视频大小或旋转信息发生改变时，房间内订阅此视频流的用户会收到此回调。
    * @param roomID 房间 ID。
    * @param key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}
    * @param info 视频帧信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
    */
    /** {en}
     * @type callback
     * @brief Users in the room who subscribe to this video stream receive this callback when the remote video size or rotation information changes.
     * @param roomID  Room ID.
     * @param key Remote Stream Information. See RemoteStreamKey{@link #RemoteStreamKey}
     * @param info Video Frame Information. See VideoFrameInfo{@link #VideoFrameInfo}
     */
    public delegate void OnRemoteVideoSizeChangedEventHandler(RemoteStreamKey key, VideoFrameInfo info);
    /** {zh}
     * @type callback
     * @brief 远端视频流的状态发生改变时，房间内订阅此流的用户会收到该事件。
     * @param roomID 房间 ID。
     * @param key 远端视频流信息，房间、用户 ID、流属性等，参看 RemoteStreamKey{@link #RemoteStreamKey}
     * @param state 远端视频流状态，参看 RemoteVideoState{@link #RemoteVideoState}
     * @param reason 远端视频流状态改变原因，参看 RemoteVideoStateChangeReason{@link #RemoteVideoStateChangeReason}
     */
    /** {en}
     * @type callback
     * @brief When the state of a remote video stream changes, users in the room who subscribe to this stream receive the event.
     * @param roomID  Room ID.
     * @param key Remote video stream information, room, user ID, stream attributes, etc.. See RemoteStreamKey{@link #RemoteStreamKey}
     * @param state Remote video stream status. See RemoteVideoState{@link #RemoteVideoState}
     * @param reason Remote video stream status change reason. See RemoteVideoStateChangeReason{@link #RemoteVideoStateChangeReason}
     */
    public delegate void OnRemoteVideoStateChangedEventHandler(RemoteStreamKey key, RemoteVideoState state, RemoteVideoStateChangeReason reason);

    /** {zh}
     * @type callback
     * @brief 用户订阅来自远端的音频流状态发生改变时，会收到此回调，了解当前的远端音频流状态。
     * @param key 远端流信息, 详见 RemoteStreamKey{@link #RemoteStreamKey}
     * @param state 远端音频流状态，详见 RemoteAudioState{@link #RemoteAudioState}
     * @param reason 远端音频流状态改变的原因，详见 RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}
     */
    /** {en}
     * @type callback
     * @brief When the state of the audio stream from the remote user subscribes to changes, this callback will be received to understand the current state of the remote audio stream.
     * @param key Remote Stream Information. See RemoteStreamKey{@link #RemoteStreamKey}
     * @param state Remote Audio Stream Status. See RemoteAudioState{@link #RemoteAudioState}
     * @param reason Remote Audio Stream Status Change Reason. See RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}
     */
    public delegate void OnRemoteAudioStateChangedEventHandler(RemoteStreamKey key, RemoteAudioState state, RemoteAudioStateChangeReason error);//349 add
    /** {zh}
     * @type callback
     * @brief 加入房间后， 以 2 秒 1 次的频率，报告用户的网络质量信息
     * @param localQuality 本端网络质量，详见 NetworkQualityStats{@link #NetworkQualityStats}。
     * @param remoteQualities 已订阅用户的网络质量，详见 NetworkQualityStats{@link #NetworkQualityStats}。
     * @param remoteQualityNum `remoteQualities` 数组长度
     * @note 更多通话中的监测接口，详见[通话中质量监测](106866)
     */
    /** {en}
     * @type callback
     * @brief Report the network quality of the users every 2s after the local user joins the room.
     * @param localQuality Local network quality. Refer to NetworkQualityStats{@link #NetworkQualityStats} for details.
     * @param remoteQualities Network quality of the subscribed users. Refer to NetworkQualityStats{@link #NetworkQualityStats} for details.
     * @param remoteQualityNum Array length of `remoteQualities`
     */
    public delegate void OnNetworkQualityEventHandler(string roomID, NetworkQualityStats localQuality, List<NetworkQualityStats> remoteQualities, int remoteQualityNum);
   
    /** {zh}
     * @type callback
     * @brief 本地流数据统计以及网络质量回调。  <br>
     *        本地用户发布流成功后，SDK 会周期性（2s）的通过此回调事件通知用户发布的流在此次统计周期内的质量统计信息。  <br>
     *        统计信息通过 LocalStreamStats{@link #LocalStreamStats} 类型的回调参数传递给用户，其中包括发送音视频比特率、发送帧率、编码帧率，网络质量等。
     * @param stat 音视频流以及网络状况统计信息。参见 LocalStreamStats{@link #LocalStreamStats}。
     */
    /** {en}
     * @type callback
     * @brief Local stream data statistics and network quality callback.   <br>
     *        After the local user publishes the flow successfully, the SDK will periodically (2s) notify the user through this callback event of the quality statistics of the published flow during this reference period. <br>
     *        Statistics are passed to the user through the callback parameters of the LocalStreamStats{@link #LocalStreamStats} type, including the sent audio & video bit rate, sent frame rate, encoded frame rate, network quality, etc. <br>
     * @param stat  Audio & video stream and network status statistics. See LocalStreamStats{@link #LocalStreamStats}.
     */
    public delegate void OnLocalStreamStatsEventHandler(LocalStreamStats stat);

    /** {zh}
     * @type callback
     * @brief 本地订阅的远端音/视频流数据统计以及网络质量回调。  <br>
     *        本地用户订阅流成功后，SDK 会周期性（2s）的通过此回调事件通知用户订阅的流在此次统计周期内的质量统计信息，包括：发送音视频比特率、发送帧率、编码帧率，网络质量等。
     * @param stat 音视频流以及网络状况统计信息。参见 RemoteStreamStats{@link #RemoteStreamStats}。
     */
    /** {en}
     * @type callback
     * @brief Remote audio/video stream statistics and network quality callbacks for local subscriptions.   <br>
     *        After the local user subscribes to the stream successfully, the SDK will periodically (2s) notify the user through this callback event of the quality statistics of the subscribed stream during this reference period, including: sending audio & video bit rate, sending frame rate, encoded frame rate, network quality, etc.
     * @param stat  Audio & video stream and network status statistics. See RemoteStreamStats{@link #RemoteStreamStats}.
     */
    public delegate void OnRemoteStreamStatsEventHandler(RemoteStreamStats stat);


    /** {zh}
     * @type callback
     * @brief SDK 接收并解码远端视频流首帧后，收到此回调。
     * @param key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}
     * @param info 视频帧信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback after the first frame of remote video stream is received and decoded by SDK.
     * @param key Remote Stream Information, see RemoteStreamKey{@link #RemoteStreamKey}
     * @param info Video Frame Information, see VideoFrameInfo{@link #VideoFrameInfo}
     */
    public delegate void OnFirstRemoteVideoFrameDecodedEventHandler(RemoteStreamKey key, VideoFrameInfo info);

    /** {zh}
 * @type callback
 * @region 视频管理
 * @author zhushufan.ref
 * @brief SDK 内部渲染成功远端视频流首帧后，收到此回调。
 * @param [in] key 远端流信息。参看 RemoteStreamKey{@link #RemoteStreamKey}。
 * @param [in] info 视频帧信息。参看 VideoFrameInfo{@link #VideoFrameInfo}。
 */
    /** {en}
     * @type callback
     * @region Video Management
     * @author zhushufan.ref
     * @brief  Receive this callback after the first frame of remote video stream is locally rendered by SDK.
     * @param  [in] key Remote stream information. See RemoteStreamKey{@link #RemoteStreamKey}.
     * @param  [in] info Video frame information. See VideoFrameInfo{@link #VideoFrameInfo}.
     */
    public delegate void OnFirstRemoteVideoFrameRenderedEventHandler(RemoteStreamKey key, VideoFrameInfo info);

    public delegate void OnPlayPublicStreamResultEventHandler(string public_stream_id, PublicStreamErrorCode result);
    public delegate void OnFaceDetectResultEventHandler(FaceDetectResult result);
    public delegate void OnExpressionDetectResultEventHandler(ExpressionDetectResult result);

    /** {zh}
     * @type callback
     * @brief 接收到来自远端某音频流的第一帧时，收到该回调。
     * @param key 远端音频流信息, 详见 RemoteStreamKey{@link #RemoteStreamKey}
     * @notes 用户刚收到房间内订阅的每一路音频流时，都会收到该回调。
    */
    /** {en}
     * @type callback
     * @brief Receives the callback when the first frame from a remote audio stream is received.
     * @param key Remote audio stream information. See RemoteStreamKey{@link #RemoteStreamKey}
     * @notes The callback will be received when the user has just received each audio stream subscribed to in the room.
     */
    public delegate void OnFirstRemoteAudioFrameEventHandler(RemoteStreamKey key);


    #endregion

    /** {zh}
     * @type api
     * @brief 引擎接口
     */
    /** {en}
     * @type api
     * @brief Engine interface.
     */
    public interface IRTCVideo
    {
        #region Events
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnWarningEventHandler OnWarningEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnErrorEventHandler OnErrorEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLogReportEventHandler OnLogReportEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnConnectionStateChangedEventHandler OnConnectionStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnNetworkTypeChangedEventHandler OnNetworkTypeChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnSysStatsEventHandler OnSysStatsEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnAudioRouteChangedEventHandler OnAudioRouteChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnAudioDeviceStateChangedEventHandler OnAudioDeviceStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnAudioDeviceWarningEventHandler OnAudioDeviceWarningEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalAudioPropertiesReportEventHandler OnLocalAudioPropertiesReportEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteAudioPropertiesReportEventHandler OnRemoteAudioPropertiesReportEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnVideoDeviceStateChangedEventHandler OnVideoDeviceStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnVideoDeviceWarningEventHandler OnVideoDeviceWarningEvent;

        // local video sink
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalVideoSinkOnFrameEventHandler OnLocalVideoSinkOnFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalVideoSinkGetRenderElapseEventHandler OnLocalVideoSinkGetRenderElapseEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalVideoSinkReleaseEventHandler OnLocalVideoSinkReleaseEvent;

        // remote video sink
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteVideoSinkOnFrameEventHandler OnRemoteVideoSinkOnFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteVideoSinkGetRenderElapseEventHandler OnRemoteVideoSinkGetRenderElapseEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteVideoSinkReleaseEventHandler OnRemoteVideoSinkReleaseEvent;

        // room audio
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnUserStartAudioCaptureEventHandler OnUserStartAudioCaptureEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnUserStopAudioCaptureEventHandler OnUserStopAudioCaptureEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnFirstLocalAudioFrameEventHandler OnFirstLocalAudioFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalAudioStateChangedEventHandler OnLocalAudioStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteAudioStateChangedEventHandler OnRemoteAudioStateChangedEvent; //349 add
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnAudioFrameSendStateChangedEventHandler OnAudioFrameSendStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnAudioFramePlayStateChangedEventHandler OnAudioFramePlayStateChangedEvent;

        /** {zh}
     * @hidden
     */
        /** {en}
        * @hidden
        */
        event OnCreateRoomStateChangedEventHandler OnCreateRoomStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
        * @hidden
        */
        event OnAudioRecordingStateUpdateEventHandler OnAudioRecordingStateUpdateEvent;

        /** {zh}
       * @hidden
       */
        /** {en}
        * @hidden
        */
        event OnRecordAudioFrameOriginalEventHandler OnRecordAudioFrameOriginalEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
        * @hidden
        */
        event OnRecordAudioFrameEventHandler OnRecordAudioFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
        * @hidden
        */
        event OnPlaybackAudioFrameEventHandler OnPlaybackAudioFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
        * @hidden
        */
        event OnRemoteUserAudioFrameEventHandler OnRemoteUserAudioFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
        * @hidden
        */
        event OnMixedAudioFrameEventHandler OnMixedAudioFrameEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
        * @hidden
        */
        event OnRecordScreenAudioFrameEventHandler OnRecordScreenAudioFrameEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnProcessRecordAudioFrameEventHandler OnProcessRecordAudioFrameEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnProcessPlaybackAudioFrameEventHandler OnProcessPlaybackAudioFrameEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnProcessRemoteUserAudioFrameEventHandler OnProcessRemoteUserAudioFrameEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnProcessEarMonitorAudioFrameEventHandler OnProcessEarMonitorAudioFrameEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnProcessScreenAudioFrameEventHandler OnProcessScreenAudioFrameEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnStreamSyncInfoReceivedEventHandler OnStreamSyncInfoReceivedEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnSEIMessageReceivedEventHandler OnSEIMessageReceivedEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnSEIMessageUpdateEventHandler OnSEIMessageUpdateEvent;

        // room video
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnUserStartVideoCaptureEventHandler OnUserStartVideoCaptureEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnUserStopVideoCaptureEventHandler OnUserStopVideoCaptureEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnFirstLocalVideoFrameCapturedEventHandler OnFirstLocalVideoFrameCapturedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalVideoSizeChangedEventHandler OnLocalVideoSizeChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnLocalVideoStateChangedEventHandler OnLocalVideoStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnVideoFrameSendStateChangedEventHandler OnVideoFrameSendStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnScreenVideoFrameSendStateChangedEventHandler OnScreenVideoFrameSendStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnVideoFramePlayStateChangedEventHandler OnVideoFramePlayStateChangedEvent;
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteVideoSizeChangedEventHandler OnRemoteVideoSizeChangedEvent;  //349 add
        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnRemoteVideoStateChangedEventHandler OnRemoteVideoStateChangedEvent;  //349 add

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnFirstRemoteVideoFrameDecodedEventHandler OnFirstRemoteVideoFrameDecodedEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnFirstRemoteVideoFrameRenderedEventHandler OnFirstRemoteVideoFrameRenderedEvent;


        event OnPlayPublicStreamResultEventHandler OnPlayPublicStreamResultEvent;
        event OnFaceDetectResultEventHandler OnFaceDetectResultEvent;
        event OnExpressionDetectResultEventHandler OnExpressionDetectResultEvent;

        /** {zh}
         * @hidden
         */
        /** {en}
         * @hidden
         */
        event OnFirstRemoteAudioFrameEventHandler OnFirstRemoteAudioFrameEvent;

        #endregion

        /** {zh}
         * @type api
         * @brief 创建 RTCVideoEngine 实例。
         *        RTCVideoEngine 实例成功创建后，你才可以使用 SDK 提供的其他能力。
         * @param initParams 引擎初始化参数，参看 RTCVideoEngineParams{@link #RTCVideoEngineParams}。
         */
        /** {en}
         * @type api
         * @brief Create GameRTCEngine instance. <br>
         *        After the GameRTCEngine instance is successfully created, you can use other capabilities provided by the SDK.
         * @param initParams  Engine initialization parameters. See RTCVideoEngineParams{@link #RTCVideoEngineParams}.
         */
        int CreateRTCVideo(RTCVideoEngineParams initParams);
        //DestroyRTCVideoEngine
        /** {zh}
         * @type api
         * @brief 销毁由 CreateRTCVideo{@link #IRTCVideo#CreateRTCVideo} 所创建引擎实例，并释放所有相关资源。
         * @notes  <br>
         *        + 你必须在所有业务场景的最后阶段调用该方法。该方法在调用之后，会销毁所有 SDK 相关的内存，并且停止与媒体服务器的任何交互。  <br>
         *        + 本方法为阻塞调用，会阻塞当前线程直到 SDK 彻底完成退出逻辑。因此，不可在回调线程中直接调用本方法；也不可在回调方法中等待主线程的执行而同时在主线程调用本方法，否则会造成死锁。
         */
        /** {en}
         * @type api
         * @brief Destroys the engine instance created by CreateRTCVideo{@link #IRTCVideo#CreateRTCVideo} and releases all related resources.
         * @notes   <br>
         *         + You must call this method in the final stage of all business scenarios. This method, when called, destroys all SDK-related memory and stops any interaction with the media server. <br>
         *         + This method is a blocking call and will block the current thread until the SDK completes the exit logic completely. Therefore, this method cannot be called directly in the callback thread; nor can this method be called in the main thread while waiting for the execution of the main thread in the callback method. Otherwise it will cause deadlock.
         */
        void Release();

        /** {zh}
         * @type api
         * @brief 获取 SDK 内各种错误码、警告码的描述文字
         * @param code 警告码、错误码。
         * @return string 描述文字
         */
        /** {en}
         * @type api
         * @brief Get the description text of various error codes and warning codes in the SDK
         * @param code Error code or warning code.
         * @return string  Description.
         */
        string GetErrorDescription(int code);

        /** {zh}
         * @type api
         * @brief 获取 SDK 当前的版本号。
         * @return SDK 当前的版本号。
         */
        /** {en}
         * @type api
         * @brief gets the current version number of the SDK.
         * @return The current version number of the SDK.
         */
        string GetSDKVersion();

        /** {zh}
         * @type api
         * @brief 设置业务标识参数。
         *        可通过 businessId 区分不同的业务场景。businessId 由客户自定义，相当于一个“标签”，可以分担和细化现在 AppId 的逻辑划分的功能，但不需要鉴权。
         * @param businessID  <br>
         *        用户设置的自己的 businessId 值
         *        businessId 只是一个标签，颗粒度需要用户自定义。
         * @return  <br>
         *        + 0： 成功  <br>
         *        + < 0： 失败  <br>
         *        + -6001： 用户已经在房间中。  <br>
         *        + -6002： 输入非法，合法字符包括所有小写字母、大写字母和数字，除此外还包括四个独立字符，分别是：英文句号，短横线，下划线和 @ 。
         * @notes  需要在调用 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 之前调用，之后调用该方法无效。
         */
        /** {en}
         * @type api
         * @brief Sets the business ID.
         *        You can use businessId to distinguish different business scenarios. You can customize your businessId to serve as a sub AppId, which can share and refine the function of the AppId, but it does not need authentication.
         * @param businessID   <br>
         *        Your customized businessId
         *        BusinessId is a tag, and you can customize its granularity.
         * @return   <br>
         *         + 0: Success <br>
         *         + < 0: Failure <br>
         *         + -6001: The user is already in the room. <br>
         *         + -6002: The input is invalid. Legal characters include all lowercase letters, uppercase letters, numbers, and four other symbols, including '.', '-','_', and '@'.
         * @notes  You must call this API before the JoinRoom{@link #IRTCVideoRoom#JoinRoom} API, otherwise it will be invalid.
         */
        int SetBusinessId(string businessID);

        /** {zh}
         * @type api
         * @brief 通话结束，将用户反馈的问题上报到 RTC。
         * @param roomID 房间 ID。
         * @param userID 用户 ID。
         * @param typeArray 预设问题列表，参看 ProblemFeedbackOption{@link #ProblemFeedbackOption}。
         * @param problemDescription 预设问题以外的其他问题的具体描述。
         * @return <br>
         *         + 0: 上报成功  <br>
         *         + -1: 上报失败，还没加入过房间 <br>
         *         + -2: 上报失败，数据解析错误  <br>
         *         + -3: 上报失败，字段缺失  <br>
         * @notes   <br>
         *        + 如果用户上报时在房间内，那么问题会定位到用户当前所在的一个或多个房间；
         *        + 如果用户上报时不在房间内，那么问题会定位到引擎此前退出的房间。
         */
        /** {en}
         * @type api
         * @brief End of call, report the user feedback to RTC.
         * @param roomID Room ID.
         * @param userID User ID.
         * @param typeArray List of preset problems. See ProblemFeedbackOption{@link #ProblemFeedbackOption}.
         * @param problemDescription Specific description of other problems other than the preset problem.
         * @return  <br>
         *          + 0: Report successfully <br>
         *          + -1: Failed to report, not yet joined the room <br>
         *          + -2: Failed to report, data analysis error <br>
         *          + -3: Failed to report, missing fields <br>
         * @notes   <br>
         *        + If the user is in the room when reporting, the problem will be located in one or more rooms where the user is currently located;
         *        + If the user is not in the room when reporting, the problem will be located in the engine that previously exited Room.
         */
        int Feedback(String roomID, String userID, List<int> typeArray, string problemDescription);

        /** {zh}
         * @type api
         * @brief 设置运行时的参数。
         * @param jsonString  json 序列化之后的字符串。
         */
        /** {en}
         * @type api
         * @brief  Setting runtime parameters.
         * @param jsonString String after json serialization.
         */
        void SetRuntimeParameters(string jsonString);

        /** {zh}
         * @type api
         * @brief 创建房间
         * @param roomID 标识通话房间的房间 ID，最大长度为 128 字节的非空字符串。支持的字符集范围为:  <br>
         *       + 26 个大写字母 A ~ Z  <br>
         *       + 26 个小写字母 a ~ z <br>
         *       + 10 个数字 0 ~ 9  <br>
         *       + 下划线 "_", at 符 "@", 减号 "-"  <br>
         *        多房间模式下，调用创建房间接口后，请勿调用同样的 roomID 创建房间，否则会导致创建房间失败。  <br>
         * @return IRTCVideoRoom 创建的房间 IRTCVideoRoom  <br>
         * @notes  <br>
         *       + 用户可以多次调用此方法创建多个 IRTCVideoRoom{@link #IRTCVideoRoom} 对象，再分别调用各 IRTCVideoRoom 对象的 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 方法，实现同时加入多个房间；  <br>
         *       + 加入多个房间后，用户可以同时订阅各房间的音频流，同一时间仅能在一个房间内发布音频流。<br>
         */
        /** {en}
         * @type api
         * @brief Create room
         * @param roomID Identify the room ID of the calling room, and the maximum length is 128 bytes of non-empty string. Supported character set ranges are: <br>
         *        + 26 uppercase letters A~ Z <br>
         *        + 26 lowercase letters a~ z <br>
         *        + 10 digits 0~ 9 <br>
         *        + underscore "_", at character "@" , Minus "-" <br>
         *        In multi-room mode, after calling the create room interface, do not call the same roomID to create a room, otherwise it will cause the creation of a room to fail. <br>
         * @return IRTCVideoRoom Created room IRTCVideoRoom <br>
         * @notes   <br>
         *        + Users can call this method multiple times to create multiple IRTCVideoRoom{@link #IRTCVideoRoom} objects, and then call the JoinRoom{@link #IRTCVideoRoom#JoinRoom} method of each IRTCAudioRoom object separately to achieve simultaneous joining of multiple rooms; <br>
         *        + After joining multiple rooms, users can subscribe to the Audio streaming, but currently only supports publishing all audio streams in one room at the same time. <br>
         */
        IRTCVideoRoom CreateRTCRoom(string roomID);

        /** {zh}
        * @valid since 3.58.1
        * @type api
        * @region 音量管理
        * @author shiyayun
        * @brief 设置是否将录音信号静音（不改变本端硬件）。
        * @param index 流索引，指定调节主流/屏幕流音量，参看 [StreamIndex](191862#streamindex)。
        * @param mute 是否静音音频采集。 <br>
        *        + True：静音（关闭麦克风）
        *        + False：（默认）开启麦克风
        * @return
        *        + 0: 调用成功。
        *        + < 0 : 调用失败。
        * @notes
        *        + 该方法支持选择静音或取消静音麦克风采集，而不影响 SDK 音频流发布状态。
        *        + 静音后通过 SetCaptureVolume{@link #IRTCVideo#SetCaptureVolume} 调整音量不会取消静音状态，音量状态会保存至取消静音。
        *        + 调用 StartAudioCapture{@link #IRTCVideo#StartAudioCapture} 开启音频采集前后，都可以使用此接口设置采集音量。
        */
        /** {en}
        * @valid since 3.58.1
        * @type api
        * @region Volume management
        * @author shiyayun
        * @brief Set whether to mute the recording signal (without changing the local hardware).
        * @param index Stream index, specifying the main stream or screen stream volume adjustment. See [StreamIndex](70083#streamindex-2).
        * @param mute Whether to mute audio capture. <br>
        *        + True: Mute (disable microphone)
        *        + False: (Default) Enable microphone
        * @return
        *        + 0: Success.
        *        + < 0 : Failure. 
        * @notes
        *        + Calling this API does not affect the status of SDK audio stream publishing.
        *        + Adjusting the volume by calling SetCaptureVolume{@link #IRTCVideo#SetCaptureVolume} after muting will not cancel the mute state. The volume state will be retained until unmuted.
        *        + You can use this interface to set the capture volume before or after calling startAudioCapture{@link #IRTCVideo#startAudioCapture} to enable audio capture.
        */
        int MuteAudioCapture(StreamIndex index, bool mute);

        /** {zh}
         * @type api
         * @brief 调节音频采集音量
         * @param index 流索引，指定调节主流还是调节屏幕流的音量，参看 StreamIndex{@link #StreamIndex}
         * @param volume 采集的音量值和原始音量的比值，范围是 [0, 400]，单位为 %，自带溢出保护。
         *        为保证更好的通话质量，建议将 volume 值设为 [0,100]。
         *       + 0：静音  <br>
         *       + 100：原始音量  <br>
         *       + 400: 最大可为原始音量的 4 倍(自带溢出保护)  <br>
         * @notes 在开启音频采集前后，你都可以使用此接口设定采集音量。
         */
        /** {en}
         * @type api
         * @brief Adjust the volume of the audio capture
         * @param index Index of the stream, whose volume needs to be adjusted. Refer to StreamIndex{@link #StreamIndex} for more details.
         * @param volume Ratio of capture volume to original volume. Ranging: [0,400]. Unit: %
         *        + 0: Mute <br>
         *        + 100: Original volume. To ensure the audio quality, we recommend [0, 100].
         *        + 400: Four times the original volume with signal-clipping protection.
         * @notes  Call this API to set the volume of the audio capture before or during the audio capture.
         */
        void SetCaptureVolume(StreamIndex index, int volume); //

        /** {zh}
         * @type api
         * @brief 调节本地播放的所有远端用户音频混音后的音量，混音内容包括远端人声、音乐、音效等。
         * @param volume 播放音量，调节范围：[0,400]  <br>
         *        为保证更好的通话质量，建议将 volume 值设为 [0,100]。
         *       + 0：静音  <br>
         *       + 100：原始音量  <br>
         *       + 400: 最大可为原始音量的 4 倍(自带溢出保护)  <br>
         */
        /** {en}
         * @type api
         * @brief Adjust the playback volume of the mixed remote audios.
         * @param volume Ratio of playback volume to original volume. Ranging: [0,400]. Unit: %
         *        To ensure the audio quality, we recommend setting the volume within `100`.
         *        + 0: mute <br>
         *        + 100: original volume <br>
         *        + 400: Four times the original volume with signal-clipping protection.
         */
        void SetPlaybackVolume(int volume);

        /** {zh}
         * @type api
         * @brief 开启内部音频采集。默认为关闭状态。
         *        内部采集是指：使用 RTC SDK 内置的视频采集机制进行视频采集。
         *        调用该方法开启后，本地用户会收到 OnAudioDeviceStateChangedEventHandler{@link #EventHandler#OnAudioDeviceStateChangedEventHandler} 的回调。
         *        可见用户进房后调用该方法，房间中的其他用户会收到 OnUserStartAudioCaptureEventHandler{@link #EventHandler#OnUserStartAudioCaptureEventHandler} 的回调。
         * @notes  <br>
         *       + 若未取得当前设备的麦克风权限，调用该方法后会触发 OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler} 回调。  <br>
         *       + 调用 StopAudioCapture{@link #IRTCVideo#StopAudioCapture} 可以关闭音频采集设备，否则，SDK 只会在销毁引擎的时候自动关闭设备。  <br>
         *       + 创建引擎后，无论是否发布音频数据，你都可以调用该方法开启音频采集，并且调用后方可发布音频。  <br>
         */
        /** {en}
         * @type api
         * @brief Start internal audio capture. The default is off.
         *         Internal audio capture refers to: capturing audio using the built-in module.
         *         The local client will be informed via OnAudioDeviceStateChangedEventHandler{@link #EventHandler#OnAudioDeviceStateChangedEventHandler} after starting audio capture by calling this API.
         *         The remote clients in the room will be informed of the state change via OnUserStartAudioCaptureEventHandler{@link #EventHandler#OnUserStartAudioCaptureEventHandler} after the visible user starts audio capture by calling this API.
         * @notes   <br>
         *        + To enable a microphone without the user's permission will trigger OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler}.  <br>
         *        + Call StopAudioCapture{@link #IRTCVideo#StopAudioCapture} to stop the internal audio capture. Otherwise, the internal audio capture will sustain until you destroy the engine instance. <br>
         *        + Once you create the engine instance, you can start internal audio capture regardless of the audio publishing state. The audio stream will start publishing only after the audio capture starts. <br>
         *        + To switch from custom to internal audio capture, stop publishing before disabling the custom audio capture module and then call this API to enable the internal audio capture.<br>
         */
        void StartAudioCapture();

        /** {zh}
         * @type api
         * @brief 立即关闭内部音频采集。默认为关闭状态。  <br>
         *        内部采集是指：使用 RTC SDK 内置的视频采集机制进行视频采集。
         *        调用该方法，本地用户会收到 OnAudioDeviceStateChangedEventHandler{@link #EventHandler#OnAudioDeviceStateChangedEventHandler} 的回调。  <br>
         *        可见用户进房后调用该方法，房间中的其他用户会收到 OnUserStopAudioCaptureEventHandler{@link #EventHandler#OnUserStopAudioCaptureEventHandler} 的回调。
         * @notes  <br>
         *       + 调用 StartAudioCapture{@link #IRTCVideo#StartAudioCapture} 可以开启内部音频采集设备。  <br>
         *       + 如果不调用本方法停止内部视频采集，则只有当销毁引擎实例时，内部音频采集才会停止。
         */
        /** {en}
        * @type api
        * @brief Stop internal audio capture. The default is off.   <br>
        *        Internal audio capture refers to: capturing audio using the built-in module.
        *         The local client will be informed via OnAudioDeviceStateChangedEventHandler{@link #EventHandler#OnAudioDeviceStateChangedEventHandler} after stopping audio capture by calling this API.<br>
        *         The remote clients in the room will be informed of the state change via OnUserStopAudioCaptureEventHandler{@link #EventHandler#OnUserStopAudioCaptureEventHandler} after the visible client stops audio capture by calling this API.<br>
        * @notes   <br>
        *        + Call StartAudioCapture{@link #IRTCVideo#StartAudioCapture} to enable the internal audio capture. <br>
        *        + Without calling this API the internal audio capture will sustain until you destroy the engine instance.<br>
        */
        void StopAudioCapture();

        /** {zh}
        * @type api
        * @brief 设置音质档位。你应根据业务场景需要选择适合的音质档位。
        * @param audioProfile 音质档位，参看 AudioProfileType{@link #AudioProfileType}
        * @notes  <br>
        *        + 该方法在进房前后均可调用；  <br>
        *        + 支持通话过程中动态切换音质档位。
        */
        /** {en}
        * @type api
        * @region audio management
        * @author dixing
        * @brief Sets the sound quality gear. You should choose the appropriate sound quality gear according to the needs of the business scenario.   <br>
        * @param audioProfile  Sound quality gear. See AudioProfileType{@link #AudioProfileType}
        * @notes   <br>
        *         + This method can be called before and after entering the room; <br>
        *         + Support dynamic switching of sound quality gear during a call.
        */
        void SetAudioProfile(AudioProfileType audioProfile);

        /** {zh}
         * @type api
         * @brief 设置音频场景类型
         * @param scenario 音频场景类型，参看 AudioScenarioType{@link #AudioScenarioType} 。
         * @notes  该方法在进房前后均可调用。
         */
        /** {en}
         * @type api
         * @brief Sets the audio scenarios.
         * @param scenario Audio scenarios. See AudioScenarioType {@link #AudioScenarioType}.
         * @notes  You can call this API before or after joining room.
         */
        void SetAudioScenario(AudioScenarioType scenario);

        /** {zh}
         * @type api
         * @brief 启用音频信息提示。  <br>
         * @param config 详见 AudioPropertiesConfig{@link #AudioPropertiesConfig}
         * @notes 开启提示后，你可以：  <br>
         *       + 通过 OnLocalAudioPropertiesReportEventHandler{@link #EventHandler#OnLocalAudioPropertiesReportEventHandler} 回调获取本地麦克风和屏幕音频流采集的音频信息；  <br>
         *       + 通过 OnRemoteAudioPropertiesReportEventHandler{@link #EventHandler#OnRemoteAudioPropertiesReportEventHandler} 回调获取订阅的远端用户的音频信息。  <br>
         */
        /** {en}
         * @type api
         * @brief Enable audio information prompts.   <br>
         * @param config See AudioPropertiesConfig{@link #AudioPropertiesConfig}
         * @notes After enable the prompt, you can:   <br>
         *        + Get the information of the audio stream collected by the local microphone and screen audio stream through OnLocalAudioPropertiesReportEventHandler{@link #EventHandler#OnLocalAudioPropertiesReportEventHandler}; <br>
         *        + Get the information of the subscribed remote audio streams through OnRemoteAudioPropertiesReportEventHandler{@link #EventHandler#OnRemoteAudioPropertiesReportEventHandler}.
         */
        int EnableAudioPropertiesReport(AudioPropertiesConfig config);

        /** {zh}
         * @type api
         * @region 美声特效管理
         * @author luomingkang
         * @brief 设置混响特效类型
         * @param type 混响特效类型，参看 VoiceReverbType{@link #VoiceReverbType}
         * @notes  <br>
         *        + 在进房前后都可设置。  <br>
         *        + 对 RTC SDK 内部采集的音频和自定义采集的音频都生效。 <br>
         *        + 只对单声道音频生效。<br>
         *        + 只在包含美声特效能力的 SDK 中有效。<br>
         */
        /** {en}
         * @type api
         * @region Bel Sound Effect Management
         * @author luomingkang
         * @brief Set the reverb effect type
         * @param type Reverb effect type. See VoiceReverbType{@link #VoiceReverbType}
         * @notes   <br>
         *         + You can call it before and after entering the room. <br>
         *         + Effective for both internal and external audio source. <br>
         *         + Only valid for mono-channel audio. <br>
         *         + Only valid in SDKs that include the ability of sound effects. <br>
         */
        int SetVoiceReverbType(VoiceReverbType type);


        int SetVoiceChangerType(VoiceChangerType voice_changer);

    /** {zh}
     * @type api
     * @region 混音
     * @author wangjunzheng
     * @brief 开启本地语音变调功能，多用于 K 歌场景。  
     *        使用该方法，你可以对本地语音的音调进行升调或降调等调整。
     * @param pitch 相对于语音原始音调的升高/降低值，取值范围[-12，12]，默认值为 0，即不做调整。  
     *        取值范围内每相邻两个值的音高距离相差半音，正值表示升调，负值表示降调，设置的绝对值越大表示音调升高或降低越多。  
     *        超出取值范围则设置失败，并且会触发 OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler} 回调，提示 WarningCode{@link #WarningCode} 错误码为 `WARNING_CODE_SET_SCREEN_STREAM_INVALID_VOICE_PITCH` 设置语音音调不合法
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     */
    /** {en}
     * @type api
     * @region Audio Mixing
     * @author wangjunzheng
     * @brief Change local voice to a different key, mostly used in Karaoke scenarios.  
     *        You can adjust the pitch of local voice such as ascending or descending with this method.
     * @param pitch The value that is higher or lower than the original local voice within a range from -12 to 12. The default value is 0, i.e. No adjustment is made.  
     *        The difference in pitch between two adjacent values within the value range is a semitone, with positive values indicating an ascending tone and negative values indicating a descending tone, and the larger the absolute value set, the more the pitch is raised or lowered.  
     *        Out of the value range, the setting fails and triggers OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler} callback, indicating `WARNING_CODE_SET_SCREEN_STREAM_INVALID_VOICE_PITCH` for invalid value setting with WarningCode{@link #WarningCode}.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     */
        int SetLocalVoicePitch(int pitch);
        int SetLocalVoiceEqualization(VoiceEqualizationConfig config);
        int SetLocalVoiceReverbParam(VoiceReverbConfig param);
        int EnableLocalVoiceReverb(bool enable);

        int SetDummyCaptureImagePath(string file_path);
        int RequestRemoteVideoKeyFrame(ref RemoteStreamKey stream_info);
        int StartPlayPublicStream(string public_stream_id);
        int StopPlayPublicStream(string public_stream_id);

        int StartPushPublicStream(string public_stream_id, IntPtr public_stream);
        int StopPushPublicStream(string public_stream_id);
        int SetPublicStreamAudioPlaybackVolume(string public_stream_id, int volume);

    /** {zh}
     * @type api
     * @author zhaomingliang
     * @region 视频管理
     * @brief 设置本端采集的视频帧的旋转角度。
     *        当摄像头倒置或者倾斜安装时，可调用本接口进行调整。
     * @param rotation 相机朝向角度，默认为 `kVideoRotation0`，无旋转角度。详见 VideoRotation{@link #VideoRotation}。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     * @notes 
     *        + 调用本接口也将对自定义采集视频画面生效，在原有的旋转角度基础上叠加本次设置。
     *        + 视频贴纸特效或通过 EnableVirtualBackground{@link #IRTCVideo#EnableVirtualBackground} 增加的虚拟背景，也会跟随本接口的设置进行旋转。
     */
    /** {en}
     * @type api
     * @brief Set the rotation of the video images captured from the local device.
     *        Call this API to rotate the videos when the camera is fixed upside down or tilted.
     * @param rotation It defaults to `VIDEO_ROTATION_0(0)`, which means not to rotate. Refer to VideoRotation{@link #VideoRotation}.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     * @notes 
     *        + For the videos captured by the internal module, the rotation will be combined with that set by calling setVideoRotationMode{@link #IRTCVideo#setVideoRotationMode}.
     *        + This API affects the external-sourced videos. The final rotation would be the original rotation angles adding up with the rotation set by calling this API.
     *        + For the Windows SDK, the elements added during the video pre-processing stage, such as video sticker and background applied using EnableVirtualBackground{@link #IRTCVideo#EnableVirtualBackground} will also be rotated by this API.
     */
        int SetVideoCaptureRotation(VideoRotation rotation);

    /** {zh}
     * @type api
     * @region 音视频处理
     * @author zhushufan.ref
     * @brief 在指定视频流上添加水印。
     * @param stream_index 需要添加水印的视频流属性，参看 StreamIndex{@link #StreamIndex}。
     * @param image_path 水印图片路径，仅支持本地文件绝对路径，长度限制为 512 字节。  
     *          水印图片为 PNG 或 JPG 格式。
     * @param config 水印参数，参看 RTCWatermarkConfig{@link #RTCWatermarkConfig}。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     * @notes  
     *        + 调用 ClearVideoWatermark{@link #IRTCVideo#ClearVideoWatermark} 移除指定视频流的水印。  
     *        + 同一路流只能设置一个水印，新设置的水印会代替上一次的设置。你可以多次调用本方法来设置不同流的水印。  
     *        + 若开启本地预览镜像，或开启本地预览和编码传输镜像，则远端水印均不镜像；在开启本地预览水印时，本端水印会镜像。  
     *        + 开启大小流后，水印对大小流均生效，且针对小流进行等比例缩小。
     */
    /** {en}
     * @type api
     * @region Audio & Video Processing
     * @author zhushufan.ref
     * @brief Adds watermark to designated video stream.
     * @param stream_index The index of the target stream. See StreamIndex{@link #StreamIndex}.
     * @param image_path The absolute path of the watermark image. The path should be less than 512 bytes.
     *        The watermark image should be in PNG or JPG format.
     * @param config Watermark configurations. See RTCWatermarkConfig{@link #RTCWatermarkConfig}.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     * @notes 
     *        + Call ClearVideoWatermark{@link #IRTCVideo#ClearVideoWatermark} to remove the watermark from the designated video stream. 
     *        + You can only add one watermark to one video stream. The newly added watermark replaces the previous one. You can call this API multiple times to add watermarks to different streams. 
     *        + If you mirror the preview, or the preview and the published stream, the watermark will also be mirrored locally, but the published watermark will not be mirrored. 
     *        + When you enable simulcast mode, the watermark will be added to all video streams, and it will be scaled down to smaller encoding configurations accordingly. 
     */
        int SetVideoWatermark(StreamIndex stream_index, string image_path, RTCWatermarkConfig config);

    /** {zh}
     * @type api
     * @region 音视频处理
     * @author zhushufan.ref
     * @brief 移除指定视频流的水印。
     * @param stream_index 需要移除水印的视频流属性，参看 StreamIndex{@link #StreamIndex}。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     */
    /** {en}
     * @type api
     * @region Audio & Video Processing
     * @author zhushufan.ref
     * @brief  Removes video watermark from designated video stream.
     * @param stream_index The index of the target stream. See StreamIndex{@link #StreamIndex}.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     */
        int ClearVideoWatermark(StreamIndex stream_index);
        int EnableEffectBeauty(bool enable);
        int SetBeautyIntensity(EffectBeautyMode beauty_mode, float intensity);
        int GetAuthMessage(ref IntPtr ppmsg, ref int len);
        int FreeAuthMessage(string pmsg);
        int InitCVResource(string license_file_path, string algo_model_dir);
        int EnableVideoEffect();
        int DisableVideoEffect();
        int SetEffectNodes(string[] effect_node_paths, int node_num);
        int UpdateEffectNode(string effect_node_path, string node_key, float node_value);
        int SetColorFilter(string res_path);
        int SetColorFilterIntensity(float intensity);
        int EnableVirtualBackground(string background_sticker_path, VirtualBackgroundSource source);
        int DisableVirtualBackground();

        //开启人脸识别功能，并设置人脸检测结果回调观察者
        int VideoSDKEnableFaceDetection(uint interval_ms, string face_model_path);
        //关闭人脸识别功能
        int VideoSDKDisableFaceDetection();
        int SetVideoDigitalZoomConfig(ZoomConfigType type, float size);
        int SetVideoDigitalZoomControl(ZoomDirectionType direction);
        int StartVideoDigitalZoomControl(ZoomDirectionType direction);
        int StopVideoDigitalZoomControl();
      
        /** {zh}
  * @type api
  * @region 音频管理
  * @author luoyi.007
  * @brief 打开/关闭耳返功能。
  * @param mode 耳返功能是否开启，详见 EarMonitorMode{@link #EarMonitorMode}。
  * @notes <br>
  *        + 你必须在使用 RTC SDK 提供的内部音频采集功能时，使用耳返功能。<br>
  *        + 耳返功能仅在连接了有线耳机时有效。<br>
  *        + RTC SDK 支持硬件耳返和软件耳返。一般来说，硬件耳返的时延和音质等效果明显更佳。
  *        + 使用华为手机硬件耳返功能时，请添加[华为硬件耳返的依赖配置](https://www.volcengine.com/docs/6348/1155036#%E5%A6%82%E4%BD%95%E5%9C%A8%E5%8D%8E%E4%B8%BA%E6%89%8B%E6%9C%BA%E4%BD%BF%E7%94%A8%E7%A1%AC%E4%BB%B6%E8%80%B3%E8%BF%94%E5%8A%9F%E8%83%BD%EF%BC%9F)
  *        + 关于你的应用能否使用硬件耳返功能，你需要联系终端厂商进行确认和开启白名单。此后，你必须联系技术支持同学，在 RTC 侧开启硬件耳返白名单。
  */
        /** {en}
         * @type api
         * @region Audio management
         * @author luoyi.007
         * @brief Enable/Disable in-ear monitoring.
         * @param mode Whether or not in-ear monitoring is enabled. See EarMonitorMode{@link #EarMonitorMode}.
         * @notes  <br>
         *         + In-ear monitoring is effective for audios captured by the RTC SDK.  <br>
         *         + We recommend that you use wired earbuds/headphones for a low-latency, high-resolution audio experience.  <br>
         *         + The RTC SDK supports both the hardware-level and SDK-level in-ear monitoring. Most hardware-level in-ear monitors enjoy lower latency and better audio quality.
         *           The use of hardware-level in-ear monitoring in your app is determined by the type of your smartphone. Hardware-level monitoring is only supported by wired earbuds/headphones.
         */
        void SetEarMonitorMode(EarMonitorMode mode);
        /** {zh}
         * @type api
         * @region 音频管理
         * @author luoyi.007
         * @brief 设置耳返音量。
         * @param volume 耳返音量，调节范围：[0,100]，单位：%
         * @notes 设置耳返音量前，你必须先调用 SetEarMonitorMode{@link #IRTCVideo#SetEarMonitorMode} 打开耳返功能。  <br>
         */
        /** {en}
         * @type api
         * @region Audio management
         * @author luoyi.007
         * @brief Set the monitoring volume.
         * @param volume  The monitoring volume with the adjustment range between 0% and 100%.
         * @notes Call SetEarMonitorMode{@link #IRTCVideo#SetEarMonitorMode} before setting the volume.
         */
        void SetEarMonitorVolume(int volume);

        /** {zh}
         * @type api
         * @brief 开启/关闭音量均衡功能。  <br>
         *        开启音量均衡功能后，人声的响度会调整为 -16lufs。
         * @param enable 是否开启音量均衡功能：  <br>
         *       + True: 是  <br>
         *       + False: 否
         */
        /** {en}
         * @type api
         * @brief Enables/disables the loudness equalization function.  <br>
         *        If you call this API with the parameter set to True, the loudness of user's voice will be adjusted to -16lufs.
         * @param enable Whether to enable loudness equalization funcion:  <br>
         *        + True: Yes <br>
         *        + False: No
         */

        void EnableVocalInstrumentBalance(bool enable); //

        /** {zh}
     * @type api
     * @brief 调节来自指定远端用户的音频播放音量，默认为 100。
     * @param userID 音频来源的远端用户 ID
     * @param volume 播放音量，调节范围：[0,400]  <br>
     *              + 0: 静音  <br>
     *              + 100: 原始音量  <br>
     *              + 400: 最大可为原始音量的 4 倍(自带溢出保护)
     */
        /** {en}
         * @type api
         * @brief  Adjusts the audio playback volume from the specified remote user, the default is 100.
         * @param userID Remote user ID of the audio source
         * @param volume Playback volume, adjustment range: [0,400] <br>
         *               + 0: mute <br>
         *               + 100: original volume <br>
         *               + 400: Maximum can be 4 times the original volume (with overflow protection)
         */
        int SetRemoteAudioPlaybackVolume(string roomID, string userID, int volume);

        /** {zh}
     * @type api
     * @region 音频设备管理
     * @author dixing
     * @brief 将默认的音频播放设备设置为听筒或扬声器。
     * @param route 音频播放设备。参看 AudioRouteDevice{@link #AudioRouteDevice}。仅支持听筒或扬声器。
     * @return <br>
     *        + 0: 方法调用成功。<br>
     *        + < 0: 方法调用失败。
     * @notes 对于音频路由切换逻辑，参见[移动端设置音频路由](https://www.volcengine.com/docs/6348/117836)。
     */
        /** {en}
         * @type api
         * @author dixing
         * @brief Set the speaker or earpiece as the default audio playback device.
         * @param route Audio playback device. Refer to AudioRouteDevice{@link #AudioRouteDevice}. You can only use earpiece and speakerphone.
         * @return <br>
         *         + 0: Success. <br>
         *         + < 0: failure. It fails when the device designated is neither a speaker nor an earpiece.
         * @notes For the default audio route switching strategy of the RTC SDK, see [Set the Audio Route](https://docs.byteplus.com/byteplus-rtc/docs/117836).
         */
        int SetDefaultAudioRoute(AudioRouteDevice route);

        /** {zh}
       * @type api
       * @brief 设置当前音频播放路由。
       *        音频播放路由发生变化时，会收到 OnAudioRouteChangedEventHandler{@link #EventHandler#OnAudioRouteChangedEventHandler} 回调。
       * @param device 音频播放路由，参见 AudioRouteDevice{@link #AudioRouteDevice}。<br>
       *        不支持 `AudioRouteUnknown`。<br>
       *        当音量类型为媒体音量时，此参数不可设置为 `AudioRouteEarpiece`；当音量模式为通话音量时，此参数不可设置为 `AudioRouteHeadsetBluetooth` 或 `AudioRouteHeadsetUSB`。
       * @return  <br>
       *        + 0: 方法调用成功  <br>
       *        + < 0: 方法调用失败。失败原因参看 MediaDeviceWarning{@link #MediaDeviceWarning} 回调。指定为 `kAudioRouteUnknown` 时将会失败。
       * @notes <br>
       *          + 连接有线或者蓝牙音频播放设备后，音频路由将自动切换至此设备。<br>
       *          + 移除后，音频设备会自动切换回原设备。<br>
       *          + 不同音频场景中，音频路由和发布订阅状态到音量类型的映射关系详见 AudioScenarioType{@link #AudioScenarioType} 。
       */
        /** {en}
         * @type api
         * @brief Designate the device as the current audio playback device.
         *         When the audio playback route changes, you will get notified via OnAudioRouteChangedEventHandler{@link #EventHandler#OnAudioRouteChangedEventHandler}.
         * @param device Audio route. Refer to AudioRouteDevice{@link #AudioRouteDevice} for more details. You cannot choose `AudioRouteUnknown`. <br>
         *        You cannot choose `AudioRouteEarpiece` except in the communication audio mode.<br>
         *        You can choose `AudioRouteHeadsetBluetooth` or `AudioRouteHeadsetUSB` except in the communication audio mode.
         * @return <br>
         *         + 0: Success <br>
         *         + < 0: Failure <br>
         * @notes <br>
         *         + The audio route automatically switches to the wired or Bluetooth audio playback device once it is connected. <br>
         *         + Disconnecting the current audio device will have the audio route switched to the previous device. If all the peripheral devices are removed, the audio route will switch to the default route audio. <br>
         *         + For the volume type in different audio scenarios, refer to AudioScenarioType{@link#AudioScenarioType}.
         */
        int SetAudioRoute(AudioRouteDevice device);

        /** {zh}
         * @type api
         * @region 音频管理
         * @author dixing
         * @brief 获取当前使用的音频播放路由。  <br>
         * @return 详见 AudioRouteDevice{@link #AudioRouteDevice}
         * @notes 要设置音频路由，详见 SetAudioRoute{@link #IRTCVideo#SetAudioRoute}。
         */
        /** {en}
         * @type api
         * @region Audio management
         * @author dixing
         * @brief Get the information of currently-using playback route.
         * @return See AudioRouteDevice{@link #AudioRouteDevice}.
         * @notes To set the audio playback route, see SetAudioRoute{@link #IRTCVideo#SetAudioRoute}.
         */
        AudioRouteDevice GetAudioRoute();

        /** {zh}
      * @type api
      * @region 音频数据回调
      * @author gongzhengduo
      * @brief 设置并开启指定的音频数据帧回调
      * @param method 音频回调方法，方法对应的回调名称参看 AudioFrameCallbackMethod{@link #AudioFrameCallbackMethod}。每次调用本接口设置一种回调。  <br>
      *               当音频回调方法设置为 `kAudioFrameCallbackRecord`、`kAudioFrameCallbackPlayback`、`kAudioFrameCallbackMixed`、`kAudioFrameCallbackRecordScreen`时，你需要在参数 `format` 中指定准确的采样率和声道，暂不支持设置为自动。  <br>
      *               当音频回调方法设置为 `kAudioFrameCallbackRemoteUser`时，暂不支持音频参数格式中设置准确的采样率和声道，你需要设置为自动。
      * @param format 音频参数格式，参看 AudioFormat{@link #AudioFormat}。
      * @return  <br>
      *        + 0: 调用成功。
      *        + < 0 : 调用失败。
      */
        /** {en}
         * @type api
         * @region Audio Data Callback
         * @author gongzhengduo
         * @brief Enable audio frames callback and set the format for the specified type of audio frames.
         * @param method Audio data callback method. See AudioFrameCallbackMethod{@link #AudioFrameCallbackMethod} for the callback enabled by each `method`. Multiple API calls would be required if you want to enable or configure different types of callbacks.<br>
         *               If `method` is set as  `kAudioFrameCallbackRecord`, `kAudioFrameCallbackPlayback`, or `kAudioFrameCallbackMixed`、`kAudioFrameCallbackRecordScreen`, set `format` to the accurate value listed in the audio parameters format.
         *               If `method` is set as `kAudioFrameCallbackRemoteUser`, set `format` to `auto`.
         * @param format Audio parameters format. See AudioFormat{@link #AudioFormat}.
         * @return  <br>
         *        + 0: Success.
         *        + < 0 : Failure
         */
        int EnableAudioFrameCallback(AudioFrameCallbackMethod method, AudioFormat format);

        /** {zh}
         * @type api
         * @region 音频数据回调
         * @author gongzhengduo
         * @brief 关闭音频回调
         * @param method 音频回调方法，参看 AudioFrameCallbackMethod{@link #AudioFrameCallbackMethod}。
         * @return  <br>
         *        + 0: 调用成功。
         *        + < 0 : 调用失败。
         * @notes 该方法需要在调用 EnableAudioFrameCallback{@link #IRTCVideo#EnableAudioFrameCallback} 之后调用。
         */
        /** {en}
         * @type api
         * @region Audio Data Callback
         * @author gongzhengduo
         * @brief Disables audio data callback.
         * @param method Audio data callback method. See AudioFrameCallbackMethod{@link #AudioFrameCallbackMethod}.
         * @return  <br>
         *        + 0: Success.
         *        + < 0 : Failure
         * @notes Call this API after calling EnableAudioFrameCallback{@link #IRTCVideo#EnableAudioFrameCallback}.
         */
        int DisableAudioFrameCallback(AudioFrameCallbackMethod method);

        /** {zh}
         * @type api
         * @author huangshouqin
         * @brief 开启录制语音通话，生成本地文件。
         * @param config 参看 AudioRecordingConfig{@link #AudioRecordingConfig}
         * @return  <br>
         *        + 0: 正常 <br>
         *        + -2: 参数设置异常 <br>
         *        + -3: 当前版本 SDK 不支持该特性，请联系技术支持人员
         * @notes <br>
         *        + 录制包含各种音频效果。但不包含混音的背景音乐。<br>
         *        + 调用 StopAudioRecording{@link #IRTCVideo#StopAudioRecording} 关闭录制。<br>
         *        + 加入房间后才可调用。如果加入了多个房间，录制的文件中会包含各个房间的音频。离开最后一个房间后，录制任务自动停止。 <br>
         *        + 调用该方法后，你会收到 OnAudioRecordingStateUpdateEventHandler{@link #EventHandler#OnAudioRecordingStateUpdateEventHandler} 回调。  <br>
         */
        /** {en}
         * @type api
         * @author huangshouqin
         * @brief Start recording audio communication, and generate the local file. <br>
         *        If you call this API before or after joining the room without internal audio capture, then the recording task can still begin but the data will not be recorded in the local files. Only when you call startAudioCapture{@link #IRTCVideo#startAudioCapture} to enable internal audio capture, the data will be recorded in the local files.
         * @param config See AudioRecordingConfig{@link #AudioRecordingConfig}.
         * @return  <br>
         *        + 0: Success <br>
         *        + -2: Invalid parameters <br>
         *        + -3: Not valid in this SDK. Please contact the technical support.
         * @notes <br>
         *        + All audio effects are valid in the file. Mixed audio file is not included in the file. <br>
         *        + Call StopAudioRecording{@link #IRTCVideo#StopAudioRecording} to stop recording. <br>
         *        + Call this API after joining the room. If you join multiple rooms, audio from all rooms are recorded in one file. After you leave the last room, the recording task ends automatically. <br>
         *        + After calling the API, you'll receive OnAudioRecordingStateUpdateEventHandler{@link #EventHandler#OnAudioRecordingStateUpdateEventHandler}. <br>
         */
        int StartAudioRecording(AudioRecordingConfig config);

        /** {zh}
         * @type api
         * @author huangshouqin
         * @brief 停止音频文件录制
         * @return <br>
         *         + 0: 正常 <br>
         *         + -3: 当前版本 SDK 不支持该特性，请联系技术支持人员
         * @notes 调用 StartAudioRecording{@link #IRTCVideo#StartAudioRecording} 开启本地录制后，你必须调用该方法停止录制。
         */
        /** {en}
         * @type api
         * @author huangshouqin
         * @brief Stop audio recording.
         * @return <br>
         *         + 0: Success <br>
         *         + <0: Failure
         * @notes Call StartAudioRecording{@link #IRTCVideo#StartAudioRecording} to start the recording task.
         */
        int StopAudioRecording();

        /** {zh}
        * @type api
        * @brief 发送音频流同步信息。将消息通过音频流发送到远端，并实现与音频流同步，该接口调用成功后，远端用户会收到 OnStreamSyncInfoReceivedEventHandler{@link #EventHandler#OnStreamSyncInfoReceivedEventHandler} 回调。
        * @param data 消息内容。
        * @param length 消息长度。取值范围是 [1,255] 字节，建议小于 16 字节，否则可能占用较大带宽。
        * @param stream_index 指定携带 SEI 数据的媒体流类型，参看 StreamIndex{@link #StreamIndex}。
        *        语音通话场景下，该值需设为 `kStreamIndexMain`，否则 SEI 数据会被丢弃从而无法送达远端。
        * @param repeat_count 消息发送重复次数。取值范围是 [0, max{29, %{视频帧率}-1}]。推荐范围 [2,4]。
        *                    调用此接口后，SEI 数据会添加到从当前视频帧开始的连续 `repeat_count+1` 个视频帧中。
        * @return  <br>
        *        + >=0: 消息发送成功。返回成功发送的次数。  <br>
        *        + -1: 消息发送失败。消息长度大于 255 字节。  <br>
        *        + -2: 消息发送失败。传入的消息内容为空。  <br>
        *        + -3: 消息发送失败。通过屏幕流进行消息同步时，此屏幕流还未发布。  <br>
        *        + -4: 消息发送失败。通过用麦克风或自定义设备采集到的音频流进行消息同步时，此音频流还未发布，详见错误码 ErrorCode{@link #ErrorCode}。  <br>
        * @notes
        * + 调用本接口的频率建议不超过 50 次每秒。
        * + 在 `kRoomProfileTypeInteractivePodcast` 房间模式下，此消息一定会送达。在其他房间模式下，如果本地用户未说话，此消息不一定会送达。
        */
        /** {en}
        * @type api
        * @brief Send audio stream synchronization information. The message is sent to the remote end through the audio stream and synchronized with the audio stream. After the interface is successfully called, the remote user will receive a OnStreamSyncInfoReceivedEventHandler{@link #EventHandler#OnStreamSyncInfoReceivedEventHandler} callback.
        * @param data Message content.
        * @param length Message length. Message length must be [1,16] bytes.
        * @param stream_index Specifies the type of media stream that carries SEI data. See StreamIndex{@link #StreamIndex}.  <br>
        *        In a voice call, you should set this parameter to `kStreamIndexMain`, otherwise the SEI data is discarded and cannot be sent to the remote user.
        * @param repeat_count Number of times a message is sent repeatedly. The value range is [0, max{29, %{video frame rate}-1}]. Recommended range: [2,4].<br>
        *                    After calling this API, the SEI data will be added to a consecutive `repeat_count+1` number of video frames starting from the current frame.
        * @return   <br>
        *         + > = 0: Message sent successfully. Returns the number of successful sends. <br>
        *         + -1: Message sending failed. Message length greater than 16 bytes. <br>
        *         + -2: Message sending failed. The content of the incoming message is empty. <br>
        *         + -3: Message sending failed. This screen stream was not published when the message was synchronized through the screen stream. <br>
        *         + -4: Message sending failed. This audio stream is not yet published when you synchronize messages with an audio stream captured by a microphone or custom device, as described in ErrorCode{@link #ErrorCode}. <br>
        * @notes
        * + Regarding the frequency, we recommend no more than 50 calls per second.
        * + When using `kRoomProfileTypeInteractivePodcast` as room profile, the data will be delivered. For other coom profiles, the data may be lost when the local user is muted.
        */
        int SendStreamSyncInfo(byte[] data, int length, int stream_index, int repeat_count);

        /** {zh}
        * @type api
        * @brief 通过视频帧发送 SEI 数据。
        *        在视频通话场景下，SEI 数据会随视频帧发送；在语音通话场景下，SDK 会自动生成一路 16px × 16px 的黑帧视频流用来发送 SEI 数据。
        * @param data SEI 消息。
        * @param length SEI 消息长度，建议每帧 SEI 数据总长度长度不超过 4 KB。
        * @param stream_index 指定携带 SEI 数据的媒体流类型，参看 StreamIndex{@link #StreamIndex}。
        *        语音通话场景下，该值需设为 `kStreamIndexMain`，否则 SEI 数据会被丢弃从而无法送达远端。
        * @param repeat_count 消息发送重复次数。取值范围是 [0, max{29, %{视频帧率}-1}]。推荐范围 [2,4]。
        *                    调用此接口后，SEI 数据会添加到从当前视频帧开始的连续 `repeat_count+1` 个视频帧中。
        * @param sei_mode SEI 发送模式。<br>
        *        + 0: 单发模式。即在 1 帧间隔内多次发送 SEI 数据时，多个 SEI 按队列逐帧发送。
        *        + 1: 多发模式。即在 1 帧间隔内多次发送 SEI 数据时，多个 SEI 随下个视频帧同时发送。
        * @return <br>
        *        + >= 0: 将被添加到视频帧中的 SEI 的数量。
        *        + < 0: 发送失败。
        * @notes <br>
        *        + 每秒发送的 SEI 消息数量建议不超过当前的视频帧率。在语音通话场景下，黑帧帧率为 15 fps。
        *        + 语音通话场景中，仅支持在内部采集模式下调用该接口发送 SEI 数据。
        *        + 视频通话场景中，使用自定义采集并通过 PushExternalVideoFrame{@link #IRTCVideo#PushExternalVideoFrame} 推送至 SDK 的视频帧，若本身未携带 SEI 数据，也可通过本接口发送 SEI 数据；若原视频帧中已添加了 SEI 数据，则调用此方法不生效。
        *        + 视频帧仅携带前后 2s 内收到的 SEI 数据；语音通话场景下，若调用此接口后 1min 内未有 SEI 数据发送，则 SDK 会自动取消发布视频黑帧。
        *        + 消息发送成功后，远端会收到 OnSEIMessageReceivedEventHandler{@link #EventHandler#OnSEIMessageReceivedEventHandler} 回调。
        *        + 语音通话切换至视频通话时，会停止使用黑帧发送 SEI 数据，自动转为用采集到的正常视频帧发送 SEI 数据。
        */
        /** {en}
        * @type api
        * @brief Sends SEI data.
        *        In a video call scenario, SEI is sent with the video frame, while in a voice call scenario, SDK will automatically publish a black frame with a resolution of 16 × 16 pixels to carry SEI data.
        * @param data SEI data.
        * @param length SEI data length. No more than 4 KB SEI data per frame is recommended.
        * @param stream_index Specifies the type of media stream that carries SEI data. See StreamIndex{@link #StreamIndex}.  <br>
        *        In a voice call, you should set this parameter to `kStreamIndexMain`, otherwise the SEI data is discarded and cannot be sent to the remote user.
        * @param repeat_count Number of times a message is sent repeatedly. The value range is [0, max{29, %{video frame rate}-1}]. Recommended range: [2,4].<br>
        *                    After calling this API, the SEI data will be added to a consecutive `repeat_count+1` number of video frames starting from the current frame.
        * @param sei_mode SEI sending mode. <br>
        *        + 0: Single-SEI mode. When you send multiple SEI messages in 1 frame, they will be sent frame by frame in a queue.
        *        + 1: Multi-SEI mode. When you send multiple SEI messages in 1 frame, they will be sent together in the next frame.
        * @return  <br>
        *         + >= 0: The number of SEIs to be added to the video frame <br>
        *         + < 0: Failure
        * @notes  <br>
        *         + We recommend the number of SEI messages per second should not exceed the current video frame rate. In a voice call, the blank-frame rate is 15 fps.
        *         + In a voice call, this API can be called to send SEI data only in internal capture mode.
        *         + In a video call, the custom captured video frame can also be used for sending SEI data if the original video frame contains no SEI data, otherwise calling this method will not take effect.
        *         + Each video frame carries only the SEI data received within 2s before and after. In a voice call scenario, if no SEI data is sent within 1min after calling this API, SDK will automatically cancel publishing black frames.
        *         + After the message is sent successfully, the remote user who subscribed your video stream will receive OnSEIMessageReceivedEventHandler{@link #EventHandler#OnSEIMessageReceivedEventHandler}.
        *         + When you switch from a voice call to a video call, SEI data will automatically start to be sent with normally captured video frames instead of black frames.
        */
        int SendSEIMessage(byte[] data, int length, int stream_index, int repeat_count,int sei_mode);

        /** {zh}
        * @type api
        * @brief 设置并开启指定的音频帧回调，进行自定义处理。
        * @param method 音频帧类型，参看 AudioProcessorMethod{@link #AudioProcessorMethod}。可多次调用此接口，处理不同类型的音频帧。  <br>
        *        选择不同类型的音频帧将收到对应的回调：<br>
        *        + 选择本地采集的音频时，会收到 OnProcessRecordAudioFrameEventHandler{@link #EventHandler#OnProcessRecordAudioFrameEventHandler}。  <br>
        *        + 选择远端音频流的混音音频时，会收到 OnProcessPlaybackAudioFrameEventHandler{@link #EventHandler#OnProcessPlaybackAudioFrameEventHandler}。  <br>
        *        + 选择远端音频流时，会收到 OnProcessRemoteUserAudioFrameEventHandler{@link #EventHandler#OnProcessRemoteUserAudioFrameEventHandler}。  <br>
        *        + 选择软件耳返音频时，会收到 OnProcessEarMonitorAudioFrameEventHandler{@link #EventHandler#OnProcessEarMonitorAudioFrameEventHandler}。  <br>
        *        + 选择屏幕共享音频流时，会收到 OnProcessScreenAudioFrameEventHandler{@link #EventHandler#OnProcessScreenAudioFrameEventHandler}。
        * @param format 设定自定义处理时获取的音频帧格式，参看 AudioFormat{@link #AudioFormat}。
        * @return  <br>
        *        + 0: 调用成功。
        *        + < 0 : 调用失败。
        * @notes 要关闭音频自定义处理，调用 DisableAudioProcessor{@link #IRTCVideo#DisableAudioProcessor}。
        */
        /** {en}
        * @type api
        * @brief Enable audio frames callback for custom processing and set the format for the specified type of audio frames.
        * @param  method The types of audio frames. See AudioProcessorMethod{@link #AudioProcessorMethod}. Set this parameter to process multiple types of audio. <br>
        *        With different values, you will receive the corresponding callback: <br>
        *        + For locally captured audio, you will receive OnProcessRecordAudioFrameEventHandler{@link #EventHandler#OnProcessRecordAudioFrameEventHandler}.  <br>
        *        + For mixed remote audio, you will receive OnProcessPlaybackAudioFrameEventHandler{@link #EventHandler#OnProcessPlaybackAudioFrameEventHandler}.  <br>
        *        + For audio from remote users, you will receive OnProcessRemoteUserAudioFrameEventHandler{@link #EventHandler#OnProcessRemoteUserAudioFrameEventHandler}.  <br>
        *        + For SDK-level in-ear monitoring audio, you will receive OnProcessEarMonitorAudioFrameEventHandler{@link #EventHandler#OnProcessEarMonitorAudioFrameEventHandler}.  <br>
        *        + For shared-screen audio, you will receive OnProcessScreenAudioFrameEventHandler{@link #EventHandler#OnProcessScreenAudioFrameEventHandler}.
        * @param  format The format of audio frames. See AudioFormat{@link #AudioFormat}.
        * @return  <br>
        *        + 0: Success.
        *        + < 0 : Fail.
        * @notes To disable custom audio processing, call DisableAudioProcessor{@link #IRTCVideo#DisableAudioProcessor}.
        */
        int EnableAudioProcessor(AudioProcessorMethod method, AudioFormat format);

        /** {zh}
        * @type api
        * @brief 关闭自定义音频处理。
        * @param method 音频帧类型，参看 AudioProcessorMethod{@link #AudioProcessorMethod}。
        * @return  <br>
        *        + 0: 调用成功。
        *        + < 0 : 调用失败。
        */
        /** {en}
        * @type api
        * @brief Disable custom audio processing.
        * @param method Audio Frame type. See AudioProcessorMethod{@link #AudioProcessorMethod}.
        * @return  <br>
        *        + 0: Success.
        *        + < 0 : Fail.
        */
        int DisableAudioProcessor(AudioProcessorMethod method);
        // audio device related functions
        /** {zh}
         * @type api
         * @brief 获取音频设备管理接口，该接口依赖引擎
         * @return 音频设备管理接口
         */
        /** {en}
         * @type api
         * @brief get audio device test interface
         * @return audio device test interface
         */
        IAudioDeviceManager GetAudioDeviceManager();

		// video manager
        /** {zh}
         * @type api
         * @brief 获取视频设备管理接口
         * @return 视频设备管理接口
         */
        /** {en}
         * @type api
         * @brief Get IVideoDeviceManager{@link #IVideoDeviceManager}
         * @return IVideoDeviceManager{@link #IVideoDeviceManager}
         */
		IVideoDeviceManager GetVideoDeviceManager();

        /** {zh}
         * @type api
         * @brief 立即开启内部视频采集。默认为关闭状态。
         *        内部视频采集指：使用 RTC SDK 内置视频采集模块，进行采集。
         *        调用该方法后，本地用户会收到 OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} 的回调。
         *        本地用户在非隐身状态下调用该方法后，房间中的其他用户会收到 OnUserStartVideoCaptureEventHandler{@link #EventHandler#OnUserStartVideoCaptureEventHandler} 的回调。
         * @notes  <br>
         *       + 调用 StopVideoCapture{@link #IRTCVideo#StopVideoCapture} 可以停止内部视频采集。否则，只有当销毁引擎实例时，内部视频采集才会停止。  <br>
         *       + 创建引擎后，无论是否发布视频数据，你都可以调用该方法开启内部视频采集。只有当（内部或外部）视频采集开始以后视频流才会发布。  <br>
         *       + 如果需要从自定义视频采集切换为内部视频采集，你必须先停止发布流，关闭自定义采集，再调用此方法手动开启内部采集。
         *       + 内部视频采集使用的摄像头由 SwitchCamera{@link #IRTCVideo#SwitchCamera} 接口指定。
         *       + 你还可以联系技术支持同学，帮助你在服务端配置采集格式并下发到 Android 端。
         */
        /** {en}
         * @type api
         * @brief Enable internal video capture immediately. The default setting is off.
         *        Internal video capture refers to: capturing video using the built-in module.
         *        The local client will be informed via OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} after starting video capture by calling this API.
         *        The remote clients in the room will be informed of the state change via OnUserStartVideoCaptureEventHandler{@link #EventHandler#OnUserStartVideoCaptureEventHandler} after the visible client starts video capture by calling this API.
         * @notes   <br>
         *        + Call StopVideoCapture{@link #IRTCVideo#StopVideoCapture} to stop the internal video capture. Otherwise, the internal video capture will sustain until you destroy the engine instance.<br>
         *        + Once you create the engine instance, you can start internal video capture regardless of the video publishing state. The video stream will start publishing only after the video capture starts. <br>
         *        + To switch from custom to internal video capture, stop publishing before disabling the custom video capture module and then call this API to enable the internal video capture.<br>
         *        + Call SwitchCamera{@link #IRTCVideo#SwitchCamera} to switch the camera used by the internal video capture module.<br>
         *        + If the default video format can not meet your requirement, contact our technical specialist to help you with Cloud Config. After that, you can push and apply these configurations to Android clients at any time.
         */
        void StartVideoCapture();

        /** {zh}
        * @type api
        * @brief 立即关闭内部视频采集。默认为关闭状态。
        *        内部视频采集指：使用 RTC SDK 内置视频采集模块，进行采集。
        *        调用该方法，本地用户会收到 OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} 的回调。
        *        可见用户进房后调用该方法，房间中的其他用户会收到 OnUserStopVideoCaptureEventHandler{@link #EventHandler#OnUserStopVideoCaptureEventHandler} 的回调。
        * @notes  <br>
        *       + 调用 StartVideoCapture{@link #IRTCVideo#StartVideoCapture} 可以开启内部视频采集。  <br>
        *       + 如果不调用本方法停止内部视频采集，则只有当销毁引擎实例时，内部视频采集才会停止。   <br>
        */
        /** {en}
        * @type api
        * @brief Disable internal video capture immediately. The default is off.
        *        Internal video capture refers to: capturing video using the built-in module.
        *        The local client will be informed via OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} after stopping video capture by calling this API.
        *        The remote clients in the room will be informed of the state change via OnUserStopVideoCaptureEventHandler{@link #EventHandler#OnUserStopVideoCaptureEventHandler} after the visible client stops video capture by calling this API.
        * @notes  <br>
        *        + Call StartVideoCapture{@link #IRTCVideo#StartVideoCapture} to enable the internal video capture. <br>
        *        + Without calling this API the internal video capture will sustain until you destroy the engine instance.
        */
        void StopVideoCapture();

        /** {zh}
         * @type api
         * @brief 设置 RTC SDK 内部采集时的视频采集参数。<br>
         *        如果你的项目使用了 SDK 内部采集模块，可以通过本接口指定视频采集参数包括模式、分辨率、帧率。
         * @param videoCaptureConfig 视频采集参数。参看 VideoCaptureConfig{@link #VideoCaptureConfig}。
         * @return  <br>
         *        + 0: 成功  <br>
         *        + < 0: 失败  <br>
         * @notes  <br>
         * + 本接口在引擎创建后即可调用，建议在调用 StartVideoCapture{@link #IRTCVideo#StartVideoCapture} 前调用本接口。
         * + 建议同一设备上的不同 Engine 使用相同的视频采集参数。
         * + 如果调用本接口前使用内部模块开始视频采集，采集参数默认为 Auto 模式。
         */
        /** {en}
         * @type api
         * @brief Set the video capture parameters for internal capture of the RTC SDK. <br>
         *         If your project uses the SDK internal capture module, you can specify the video capture parameters including preference, resolution and frame rate through this interface.
         * @param videoCaptureConfig Video capture parameters. See: VideoCaptureConfig{@link #VideoCaptureConfig}.
         * @return   <br>
         *         + 0: Success; <br>
         *         + < 0: Failure; <br>
         * @notes   <br>
         *  + This interface can be called after the engine is created. It is recommended to call this interface before calling StartVideoCapture{@link #IRTCVideo#StartVideoCapture}.
         *  + It is recommended that different Engines on the same device use the same video capture parameters.
         *  + If you used the internal module to start video capture before calling this interface, the capture parameters default to Auto.
         */
        int SetVideoCaptureConfig(VideoCaptureConfig videoCaptureConfig);

        /** {zh}
         * @type api
         * @brief 视频发布端设置期望发布的最大分辨率视频流参数，包括分辨率、帧率、码率、缩放模式、网络不佳时的回退策略等。
         * @param maxSolution 期望发布的最大分辨率视频流参数。参看 VideoEncoderConfig{@link #VideoEncoderConfig}。
         * @return  <br>
         *        + 0：成功  <br>
         *        + !0：失败  <br>
         * @notes  <br>
         *        + 调用该方法设置多路视频流参数前，SDK 默认仅发布一条分辨率为 640px × 360px，帧率为 15fps 的视频流。  <br>
         *        + 该方法适用于摄像头采集的视频流，设置屏幕共享视频流参数参看 SetScreenVideoEncoderConfig{@link #IRTCVideo#SetScreenVideoEncoderConfig}。
         */
        /**
         * {en}
         * @type api
         * @brief Video publisher call this API to set the parameters of the maximum resolution video stream that is expected to be published, including resolution, frame rate, bitrate, scale mode, and fallback strategy in poor network conditions.
         * @param maxSolution The maximum video encoding parameter. See VideoEncoderConfig{@link #VideoEncoderConfig}.
         * @return   <br>
         *        + 0: Success <br>
         *        + ! 0: Failure <br>
         * @notes  <br>
         *        + Without calling this API, SDK will only publish one stream for you with a resolution of 640px × 360px and a frame rate of 15fps.  <br>
         *        + This API is applicable to the video stream captured by the camera, see SetScreenVideoEncoderConfig{@link #IRTCVideo#SetScreenVideoEncoderConfig} for setting parameters for screen sharing video stream.
         */
        int SetVideoEncoderConfig1(VideoEncoderConfig maxSolution);

        /** {zh}
         * @hidden
         * @type api
         * @brief 视频发布端设置推送多路流时各路流的参数，包括分辨率、帧率、码率、缩放模式、网络不佳时的回退策略等。
         * @param channelSolutions 要推送的多路视频流的参数，最多支持设置 3 路参数，超过 3 路时默认取前 3 路的值。  <br>
         *        当设置了多路参数时，分辨率必须是从大到小排列。需注意，所设置的分辨率是各路流的最大分辨率。参看 VideoEncoderConfig{@link #VideoEncoderConfig}。
         * @param solutionNum 发布的分辨率的个数
         * @return  <br>
         *        + 0：成功  <br>
         *        + !0：失败  <br>
         * @notes  <br>
         *        + 调用该方法设置多路视频流参数前，SDK 默认仅发布一条分辨率为 640px × 360px，帧率为 15fps 的视频流。  <br>
         *        + 调用该方法设置分辨率不同的多条流后，SDK 会根据订阅端设置的期望订阅参数自动匹配发送的流，具体规则参看[推送多路流](70139)文档。  <br>
         *        + 该方法适用于摄像头采集的视频流，设置屏幕共享视频流参数参看 SetScreenVideoEncoderConfig{@link #IRTCVideo#SetScreenVideoEncoderConfig}。
         */
        /** {en}
         * @hidden
         * @type api
         * @brief Video publisher call this API to set the configurations of each stream published, including resolution, frame rate, bitrate, scale mode, and fallback strategy in poor network conditions.
         * @param channelSolutions List of configurations for multiple streams. You can set parameters for up to 3 streams, SDK will always take the value of the first 3 streams when parameters for more streams are set.
         *        You should note that the resolution you set is the maximum resolution of each stream, and must be arranged from largest to smallest. See VideoEncoderConfig{@link #VideoEncoderConfig}.
         * @param solutionNum The number of the solutions that you set.
         * @return  <br>
         *        + 0: Success <br>
         *        + ! 0: Failure <br>
         * @notes  <br>
         *        + Without calling this API to set parameters for multiple video streams, the SDK only publishes one video stream with a resolution of 640px × 360px and a frame rate of 15fps.  <br>
         *        + After setting parameters for multiple video streams, SDK will automatically match the streams to be published based on the desired subscription parameters set by subscribers, see [Publish Multiple Streams](70139) for details.  <br>
         *        + This API is applicable to the video stream captured by the camera, see SetScreenVideoEncoderConfig{@link #IRTCVideo#SetScreenVideoEncoderConfig} for setting parameters for screen sharing video stream.
         */
        int SetVideoEncoderConfig2(VideoEncoderConfig[] channelSolutions, int solutionNum);

        /** {zh}
         * @type api
         * @brief 为发布的屏幕共享视频流设置期望的编码参数，包括分辨率、帧率、码率、缩放模式、网络不佳时的回退策略等。
         * @param screenSolution 屏幕共享视频流参数。参看 ScreenVideoEncoderConfig{@link #ScreenVideoEncoderConfig}。
         * @return 方法调用结果： <br>
         *         0：成功  <br>
         *         !0：失败  <br>
         * @notes 调用该方法之前，屏幕共享视频流默认的编码参数为：分辨率 1920px × 1080px，帧率 15fps。
         */
        /** {en}
         * @type api
         * @brief Video publisher call this API to set the expected configurations for the screen sharing video stream, including resolution, frame rate, bitrate, scale mode, and fallback strategy in poor network conditions.
         * @param screenSolution You expected configurations for screen sharing video stream. See ScreenVideoEncoderConfig{@link #ScreenVideoEncoderConfig}.
         * @return  API call result: <br>
         *        + 0: Success <br>
         *        + ! 0: Failure <br>
         * @notes Without calling this API, the default encoding parameters for screen sharing video streams are: resolution 1920px × 1080px, frame rate 15fps.
         */
        int SetScreenVideoEncoderConfig(ScreenVideoEncoderConfig screenSolution);

        /** {zh}
         * @type api
         * @brief 设置向 SDK 输入的视频源，包括屏幕流。
         *        默认使用内部采集。内部采集指：使用 RTC SDK 内置的视频采集机制进行视频采集。
         * @param streamIndex 视频流的属性，参看 StreamIndex{@link #StreamIndex}。
         * @param type 视频输入源类型，参看 VideoSourceType{@link #VideoSourceType}。
         * @notes  <br>
         *        + 该方法进房前后均可调用。
         *        + 当你已调用 StartVideoCapture{@link #IRTCVideo#StartVideoCapture} 开启内部采集后，再调用此方法切换至自定义采集时，SDK 会自动关闭内部采集。
         *        + 当你调用此方法开启自定义采集后，想要切换至内部采集，你必须先调用此方法关闭自定义采集，然后调用 StartVideoCapture{@link #IRTCVideo#StartVideoCapture} 手动开启内部采集。
         *        + 当你需要向 SDK 推送自定义编码后的视频帧，你需调用该方法将视频源切换至自定义编码视频源。
         */
        /** {en}
         * @type api
         * @brief Set the video source, including the screen recordings.
         *        The internal video capture is the default, which refers to capturing video using the built-in module.
         * @param streamIndex Stream index. Refer to StreamIndex{@link #StreamIndex} for more details.
         * @param type Video source type. Refer to VideoSourceType{@link #VideoSourceType} for more details.
         * @notes   <br>
         *         + You can call this API whether the user is in a room or not.
         *         + Calling this API to switch to the custom video source will stop the enabled internal video capture.
         *         + To switch to internal video capture, call this API to stop custom capture and then call StartVideoCapture{@link #IRTCVideo#StartVideoCapture} to enable internal video capture.
         *         + To push custom encoded video frames to the SDK, call this API to switch video source type to custom encoded video stream.
         */
        void SetVideoSourceType(StreamIndex streamIndex, VideoSourceType type);

        /** {zh}
         * @type api
         * @brief 为采集到的视频流开启镜像
         * @param mirrorType 镜像类型，参看 MirrorType{@link #MirrorType}
         * @notes <br>
         *        + 切换视频源不影响镜像设置。<br>
         *        + 屏幕视频流始终不受镜像设置影响。<br>
         *        + 该接口调用前，各视频源的初始状态如下：
         *        <table>
         *           <tr><th></th><th>前置摄像头</th><th>后置摄像头</th><th>自定义采集视频源</th> <th>桌面端摄像头</th> </tr>
         *           <tr><td>移动端</td><td>本地预览镜像，编码传输不镜像</td><td> 本地预览不镜像，编码传输不镜像 </td><td> 本地预览不镜像，编码传输不镜像 </td><td>/</td></tr>
         *           <tr><td>桌面端</td><td>/</td><td>/</td><td> 本地预览不镜像，编码传输不镜像 </td><td> 本地预览镜像，编码传输不镜像 </td></tr> <br>
         *        </table>
         */
        /** {en}
         * @type api
         * @brief Sets the mirror mode for the captured video stream.
         * @param mirrorType Mirror type. See MirrorType{@link #MirrorType}.
         * @notes  <br>
         *         - Switching video streams does not affect the settings of the mirror type.  <br>
         *         - This API is not applicable to screen-sharing streams.   <br>
         *         - Before you call this API, the initial states of each video stream are as follows:
         *         <table>
         *            <tr><th></th><th>Front-facing camera</th><th>Back-facing camera</th><th>Custom capturing</th><th>Built-in camera</th></tr>
         *            <tr><td>Mobile device</td><td>The preview is mirrored. The published video stream is not mirrored.</td><td>The preview and the published video stream are not mirrored.</td><td>The preview and the published video stream are not mirrored.</td><td>/</td></tr>
         *            <tr><td>PC</td><td>/</td><td>/</td><td>The preview and the published video stream are not mirrored.</td><td>The preview is mirrored. The published video stream is not mirrored.</td></tr> <br>
         *         </table>
         */
        void SetLocalVideoMirrorType(MirrorType mirrorType);

        /** {zh}
         * @type api
         * @brief 设置采集视频的旋转模式。默认以 App 方向为旋转参考系。<br>
         *        接收端渲染视频时，将按照和发送端相同的方式进行旋转。<br>
         * @param rotationMode 视频旋转参考系为 App 方向或重力方向，参看 VideoRotationMode{@link #VideoRotationMode}<br>
         * @return <br>
         *        + 0:设置成功
         *        + <0:设置失败
         * @notes <br>
         *        + 旋转仅对内部视频采集生效，不适用于外部视频源和屏幕源。
         *        + 调用该接口时已开启视频采集，将立即生效；调用该接口时未开启视频采集，则将在采集开启后生效。
         *        + 更多信息请参考[视频采集方向](106458)。
         */
        /** {en}
         * @type api
         * @brief Set the orientation of the video capture. By default, the App direction is used as the orientation reference.<br>
         *        During rendering, the receiving client rotates the video in the same way as the sending client did.<br>
         * @param rotationMode Rotation reference can be the orientation of the App or gravity. Refer to VideoRotationMode{@link #VideoRotationMode} for details.
         * @return <br>
         *        + 0: Success
         *        + <0: Failure
         * @notes <br>
         *        + The orientation setting is effective for internal video capture only. That is, the orientation setting is not effective to the custom video source or the screen-sharing stream.
         *        + If the video capture is on, the setting will be effective once you call this API. If the video capture is off, the setting will be effective on when capture starts.
         */
        int SetVideoRotationMode(VideoRotationMode rotationMode);

        /** {zh}
         * @type api
         * @brief 将本地视频流与自定义渲染器绑定。
         * @param index 视频流属性。采集的视频流/屏幕视频流，参看 StreamIndex{@link #StreamIndex}
         * @param requiredFormat videoSink 适用的视频帧编码格式，参看 VideoSinkPixelFormat{@link #VideoSinkPixelFormat}
         * @notes  <br>
         *        + RTC SDK 默认使用自带的渲染器（内部渲染器）进行视频渲染。
         *        + 进退房操作不会影响绑定状态。
         *        + 一般在收到 OnFirstLocalVideoFrameCapturedEventHandler{@link #EventHandler#OnFirstLocalVideoFrameCapturedEventHandler} 回调通知完成本地视频首帧采集后，调用此方法为视频流绑定自定义渲染器；然后加入房间。
         */
        /** {en}
         * @type api
         * @brief Binds the local video stream to a custom renderer.
         * @param index Video stream type. See StreamIndex{@link #StreamIndex}.
         * @param requiredFormat Video frame encoding format that applys to custom rendering. See VideoSinkPixelFormat{@link #VideoSinkPixelFormat}.
         * @notes  <br>
         *        + RTC SDK uses its own renderer (internal renderer) for video rendering by default.
         *        + Joining or leaving the room will not affect the binding state.
         *        + You should call this API before joining the room, and after receiving OnFirstLocalVideoFrameCapturedEventHandler{@link #EventHandler#OnFirstLocalVideoFrameCapturedEventHandler} which reports that the first local video frame has been successfully captured.
         */
        void SetLocalVideoSink(StreamIndex index, VideoSinkPixelFormat requiredFormat);

        /** {zh}
         * @type api
         * @brief 将远端视频流与自定义渲染器绑定。
         * @param streamKey 远端流信息，用于指定需要渲染的视频流来源及属性，参看 RemoteStreamKey{@link #RemoteStreamKey}
         * @param requiredFormat videoSink 适用的视频帧编码格式，参看 VideoSinkPixelFormat{@link #VideoSinkPixelFormat}
         * @notes  <br>
         *        + RTC SDK 默认使用 RTC SDK 自带的渲染器（内部渲染器）进行视频渲染。
         *        + 该方法进房前后均可以调用。若想在进房前调用，你需要在加入房间前获取远端流信息；若无法预先获取远端流信息，你可以在加入房间并通过 OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler} 回调获取到远端流信息之后，再调用该方法。
         */
        /** {en}
         * @type api
         * @brief Binds the remote video stream to a custom renderer.
         * @param streamKey Remote stream information which specifies the source and type of the video stream to be rendered. See RemoteStreamKey{@link #RemoteStreamKey}
         * @param requiredFormat Encoding format that applies to the custom renderer. See VideoSinkPixelFormat{@link #VideoSinkPixelFormat}
         * @notes  <br>
         *        + RTC SDK uses its own renderer (internal renderer) for video rendering by default.  <br>
         *        + Joining or leaving the room will not affect the binding state. <br>
         *        + This API can be called before and after entering the room. To call before entering the room, you need to get the remote stream information before joining the room; if you cannot get the remote stream information in advance, you can call the API after joining the room and getting the remote stream information via OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler}.
         */
        void SetRemoteVideoSink(RemoteStreamKey streamKey, VideoSinkPixelFormat requiredFormat);

        // screen

        /** {zh}
         * @brief 获取共享对象(应用窗口和桌面)列表, 使用完之后需要调用对应的 release 接口释放
         * @return 屏幕共享对象列表，参看 IScreenCaptureSourceList{@link #IScreenCaptureSourceList}
         * @notes  <br>
         *       + 枚举的窗体可作为开启屏幕共享时的输入参数，详见：StartScreenVideoCapture{@link #IRTCVideo#StartScreenVideoCapture} <br>
         *       + 本函数仅供PC端使用
         */
        /** {en}
         * @brief  Get the list of shared objects (application windows and desktops),  after using it, you need to call the corresponding release interface to release
         * @return Screen capture sources. See IScreenCaptureSourceList{@link #IScreenCaptureSourceList}.
         * @notes   <br>
         *        + The enumerated form can be used as an input parameter when opening screen sharing, see: StartScreenVideoCapture{@link #IRTCVideo#StartScreenVideoCapture} <br>
         *        + This function For PC side use only
         */
        IScreenCaptureSourceList GetScreenCaptureSourceList();

        /** {zh}
         * @type api
         * @brief 使用 RTC SDK 提供的采集模块，采集当前屏幕视频流，用于共享。
         * @param sourceInfo 屏幕共享对象信息。参看 ScreenCaptureSourceInfo{@link #ScreenCaptureSourceInfo}。
         * @param captureParams 屏幕采集参数。参看 ScreenCaptureParameters{@link #ScreenCaptureParameters}。
         * @return <br>
         *       + 0 :开启成功
         *       + -1 :开启失败
         * @notes <br>
         *       + 调用此方法仅开启屏幕流视频采集，不会发布采集到的视频。发布屏幕流视频需要调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen}。<br>
         *       + 要关闭屏幕视频源采集，调用 StopScreenVideoCapture{@link #IRTCVideo#StopScreenVideoCapture}。  <br>
         *       + 调用成功后，本端会收到 OnFirstLocalVideoFrameCapturedEventHandler{@link #EventHandler#OnFirstLocalVideoFrameCapturedEventHandler} 回调。<br>
         *       + 调用此接口前，你可以调用 SetVideoEncoderConfig1{@link #IRTCVideo#SetVideoEncoderConfig1} 设置屏幕视频流的采集帧率和编码分辨率。<br>
         */
        /** {en}
         * @type api
         * @brief Use the capture module provided by the RTC SDK to capture the current screen video stream for sharing.
         * @param sourceInfo See ScreenCaptureSourceInfo{@link #ScreenCaptureSourceInfo}.
         * @param captureParams See ScreenCaptureParameters{@link #ScreenCaptureParameters}.
         * @return  <br>
         *        + 0: Success.
         *        + -1: Failure.
         * @notes  <br>
         *        + Call this method to only open Screen stream video capture will not release the captured video. Publishing a screencast video requires calling PublishScreen{@link #IRTCVideoRoom#PublishScreen}. <br>
         *        + To turn off screen video source capture, call StopScreenVideoCapture{@link #IRTCVideo#StopScreenVideoCapture}. <br>
         *        + After the call succeeds, the local side receives the OnFirstLocalVideoFrameCapturedEventHandler{@link #EventHandler#OnFirstLocalVideoFrameCapturedEventHandler} callback. <br>
         *        + Before calling this interface, you can call SetVideoEncoderConfig1{@link #IRTCVideo#SetVideoEncoderConfig1} to set the capture frame rate and encoding resolution of the screen video stream. <br>
         */
        int StartScreenVideoCapture(ScreenCaptureSourceInfo sourceInfo, ScreenCaptureParameters captureParams);

        /** {zh}
         * @type api
         * @brief 停止屏幕视频流采集。
         * @return <br>
         *       + 0 :调用成功 <br>
         *       + -1 :调用失败
         * @notes 调用此接口不影响屏幕视频流发布状态。
         */
        /** {en}
         * @type api
         * @brief Stop screen video stream capture.
         * @return  <br>
         *        + 0: Call succeeded <br>
         *        + -1: Failure
         * @notes  Calling this interface does not affect on-screen video stream publishing.
         */
        void StopScreenVideoCapture();

        /** {zh}
         * @type api
         * @brief 通过 RTC SDK 提供的采集模块采集屏幕视频流时，更新采集区域。
         * @param  regionRect 采集区域。参见 Rectangle{@link #Rectangle}  <br>
         *                          此参数描述了调用此接口后的采集区域，和 StartScreenVideoCapture{@link #StartScreenVideoCapture} 中 `source_info` 设定区域的相对关系。
         * @notes 调用此接口前，必须已通过调用 StartScreenVideoCapture{@link #StartScreenVideoCapture} 开启了内部屏幕流采集。
         */
        /** {en}
         * @type api
         * @brief Update the capture area when capturing screen video streams through the capture module provided by the RTC SDK.
         * @param  regionRect Acquisition area. See Rectangle{@link #Rectangle} <br>
         *                           This parameter describes the acquisition area after calling this interface, and the relative relationship between the 'source_info' setting area in StartScreenVideoCapture{@link #StartScreenVideoCapture}.
         * @notes Before calling this interface, internal screen stream capture must have been turned on by calling StartScreenVideoCapture{@link #StartScreenVideoCapture}.
         */
        void UpdateScreenCaptureRegion(Rectangle regionRect);

        /** {zh}
         * @type api
         * @brief 通过 RTC SDK 提供的采集模块采集屏幕视频流时，更新边框高亮设置。默认展示表框。
         * @param highlightConfig 边框高亮设置。参见 HighlightConfig{@link #HighlightConfig}
         * @notes 调用此接口前，必须已通过调用 StartScreenVideoCapture{@link #StartScreenVideoCapture} 开启了内部屏幕流采集。
         */
        /** {en}
         * @type api
         * @brief Update border highlighting settings when capturing screen video streams through the capture module provided by the RTC SDK. The default display table box.
         * @param  highlightConfig Border highlighting settings. See HighlightConfig{@link #HighlightConfig}
         * @notes  Before calling this interface, you must have turned on internal screen flow collection by calling StartScreenVideoCapture{@link #StartScreenVideoCapture}.
         */
        void UpdateScreenCaptureHighlightConfig(HighlightConfig highlightConfig);

        /** {zh}
         * @type api
         * @brief 通过 RTC SDK 提供的采集模块采集屏幕视频流时，更新对鼠标的处理设置。默认采集鼠标。
         * @param captureMouseCursor 参看 MouseCursorCaptureState{@link #MouseCursorCaptureState}
         * @notes 调用此接口前，必须已通过调用 StartScreenVideoCapture{@link #StartScreenVideoCapture} 开启了内部屏幕流采集。
         */
        /** {en}
         * @type api
         * @brief Update the processing settings for the mouse when capturing screen video streams through the capture module provided by the RTC SDK. Default acquisition mouse.
         * @param captureMouseCursor See MouseCursorCaptureState{@link #MouseCursorCaptureState}
         * @notes  Before calling this interface, internal screen stream capture must have been turned on by calling StartScreenVideoCapture{@link #StartScreenVideoCapture}.
         */
        void UpdateScreenCaptureMouseCursor(MouseCursorCaptureState captureMouseCursor);

        /** {zh}
         * @type api
         * @brief 通过 RTC SDK 提供的采集模块采集屏幕视频流时，设置需要过滤的窗口。
         * @param filterConfig 窗口过滤设置，参看 ScreenFilterConfig{@link #ScreenFilterConfig}  <br>
         * @notes <br>
         *       + 调用此接口前，必须已通过调用 StartScreenVideoCapture{@link #StartScreenVideoCapture} 开启了内部屏幕流采集。<br>
         *       + 本函数在屏幕源类别是屏幕而非应用窗体时才起作用。详见：ScreenCaptureSourceType{@link #ScreenCaptureSourceType}
         */
        /** {en}
         * @type api
         * @brief When capturing screen video streams through the capture module provided by the RTC SDK, set the window that needs to be filtered.
         * @param  filterConfig Window filtering settings. See ScreenFilterConfig{@link #ScreenFilterConfig} <br>
         * @notes  <br>
         *        + Before calling this interface, internal screen stream capture must have been turned on by calling StartScreenVideoCapture{@link #StartScreenVideoCapture}. <br>
         *        + This function only works when the screen source category is a screen rather than an application form. See: ScreenCaptureSourceType{@link #ScreenCaptureSourceType}
         */
        void UpdateScreenCaptureFilterConfig(ScreenFilterConfig filterConfig);

        /** {zh}
         * @brief 获取共享对象缩略图
         * @param type 屏幕采集对象的类型。详见 ScreenCaptureSourceType{@link #ScreenCaptureSourceType}。
         * @param sourceID 屏幕分享时，共享对象的 ID。
         * @param maxWidth 最大宽度
         * @param maxHeight 最大高度
         */
        /** {en}
         * @brief  Get the shared object thumbnail
         * @param type Type of the screen capture object. Refer to ScreenCaptureSourceType{@link #ScreenCaptureSourceType} for more details.
         * @param sourceID ID of the screen-shared object.
         * @param maxWidth Maximum width
         * @param maxHeight Maximum height
         */
        VideoFrame GetThumbnail(ScreenCaptureSourceType type, IntPtr sourceID, int maxWidth, int maxHeight);

        /** {zh}
         * @type api
         * @brief 在屏幕共享时，设置屏幕音频的采集方式（内部采集/自定义采集）
         * @param sourceType 屏幕音频输入源类型, 参看 AudioSourceType{@link #AudioSourceType}。
         * @notes  <br>
         *      + 默认采集方式是 RTC SDK 内部采集。<br>
         *      + 你应该在 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 前，调用此方法。否则，你将收到 OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler} 的报错：`WARNING_CODE_SET_SCREEN_AUDIO_SOURCE_TYPE_FAILED`。 <br>
         *      + 如果设定为内部采集，你必须再调用 StartScreenVideoCapture{@link #IRTCVideo#StartScreenVideoCapture} 开始采集。
         *      + 如果设定为自定义采集，你必须再调用 PushScreenAudioFrame{@link #IRTCVideo#PushScreenAudioFrame} 将自定义采集到的屏幕音频帧推送到 RTC SDK。<br>
         *      + 无论是内部采集还是自定义采集，你都必须调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 发布采集到的屏幕音频流。
         */
        /** {en}
         * @type api
         * @brief Sets the screen audio source type. (internal capture/custom capture)
         * @param sourceType Screen audio source type. See AudioSourceType{@link #AudioSourceType}.
         * @notes   <br>
         *       + The default screen audio source type is RTC SDK internal capture. <br>
         *       + You should call this API before calling PublishScreen{@link #IRTCVideoRoom#PublishScreen}. Otherwise, you will receive OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler} with 'WARNING_CODE_SET_SCREEN_AUDIO_SOURCE_TYPE_FAILED'. <br>
         *       + When using internal capture, you need to call StartScreenVideoCapture{@link #IRTCVideo#StartScreenVideoCapture} to start capturing.
         *       + When using custom capture, you need to call PushScreenAudioFrame{@link #IRTCVideo#PushScreenAudioFrame} to push the audio stream to the RTC SDK. <br>
         *       + Whether you use internal capture or custom capture, you must call PublishScreen{@link #IRTCVideoRoom#PublishScreen} to publish the captured screen audio stream.
         */
        void SetScreenAudioSourceType(AudioSourceType sourceType);

        /** {zh}
         * @type api
         * @brief 在屏幕共享时，设置屏幕音频流和麦克风采集到的音频流的混流方式
         * @param index 混流方式，参看 StreamIndex{@link #StreamIndex} <br>
         *        + `kStreamIndexMain`: 将屏幕音频流和麦克风采集到的音频流混流 <br>
         *        + `kStreamIndexScreen`: 将屏幕音频流和麦克风采集到的音频流分为两路音频流
         * @notes 你应该在 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 之前，调用此方法。否则，你将收到 OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler} 的报错：`WARNING_CODE_SET_SCREEN_STREAM_INDEX_FAILED`
         */
        /** {en}
         * @type api
         * @brief Set the mixing mode of the screen audio stream and the audio stream collected by the microphone during screen sharing
         * @param index Mixing mode. See StreamIndex{@link #StreamIndex} <br>
         *         + 'kStreamIndexMain': Mixing the screen audio stream and the audio stream collected by the microphone <br>
         *         + 'kStreamIndexScreen': Divide the screen audio stream and the audio stream collected by the microphone into two audio streams
         * @notes You should call this method before PublishScreen{@link #IRTCVideoRoom#PublishScreen}. Otherwise, you will receive an error from OnWarningEventHandler{@link #EventHandler#OnWarningEventHandler}: 'WARNING_CODE_SET_SCREEN_STREAM_INDEX_FAILED
         */
        void SetScreenAudioStreamIndex(StreamIndex index);

        /** {zh}
         * @type api
         * @brief 在屏幕共享时，开始使用 RTC SDK 内部采集方式，采集屏幕音频
         * @notes 
         *        + 本接口仅对内部采集生效，RTC SDK 默认使用内部采集模块采集屏幕音频。若已调用 SetScreenAudioSourceType{@link #IRTCVideo#SetScreenAudioSourceType} 将音频输入源设置为 `kAudioSourceTypeExternal` 自定义采集，需先切换为 `kAudioSourceTypeInternal` 内部采集，否则该接口调用无效，并将触发 onAudioDeviceWarning{@link #onAudioDeviceWarning} 回调。
         *        + 采集后，你还需要调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 将采集到的屏幕音频推送到远端。<br>
         *        + 要关闭屏幕音频内部采集，调用 StopScreenAudioCapture{@link #IRTCVideo#StopScreenAudioCapture}。
         */
        /** {en}
         * @type api
         * @brief When sharing the screen, start using the RTC SDK internal acquisition method to capture the screen audio
         * @notes  <br>
         *         + After the acquisition, you also need to call PublishScreen{@link #IRTCVideoRoom#PublishScreen} to push the collected screen audio to the remote end. <br>
         *         + To turn off screen audio internal capture, call StopScreenAudioCapture{@link #IRTCVideo#StopScreenAudioCapture}. <br>
         */
        void StartScreenAudioCapture();

        /** {zh}
         * @type api
         * @brief 在屏幕共享时，停止使用 RTC SDK 内部采集方式，采集屏幕音频。
         * @notes <br>
         *      + 要开始屏幕音频内部采集，调用 StartScreenAudioCapture{@link #IRTCVideo#StartScreenAudioCapture}。<br>
         *      + 此接口暂不支持。
         */
        /** {en}
         * @type api
         * @brief Stop using RTC SDK internal capture method to capture screen audio during screen sharing.
         * @notes  <br>
         *       + To start the screen audio internal acquisition, call StartScreenAudioCapture{@link #IRTCVideo#StartScreenAudioCapture}. <br>
         *       + This interface is not supported yet.
         */
        void StopScreenAudioCapture();

        /** {zh}
         * @type api
         * @brief 使用 RTC SDK 内部采集模块开始采集屏幕音频流和（或）视频流。
         * @param type 媒体类型，参看 ScreenMediaType{@link #ScreenMediaType}。
         * @param bundleId 绑定 Extension 的 Bundle ID，绑定后在应用中，共享屏幕的选择列表中只展示你的 Extension 可供选择。
         * @notes <br>
         *      + 调用本接口时，采集模式应为内部模式。在外部采集模式下调用无效，并将触发 OnVideoDeviceWarningEventHandler{@link #EventHandler#OnVideoDeviceWarningEventHandler} 回调。<br>
         *      + 当从 iOS 控制中心发起屏幕采集时无需调用本方法。 <br>
         *      + 采集后，你还需要调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 发布采集到的屏幕音视频。<br>
         *      + 开启屏幕音频/视频采集成功后，本地用户会收到 OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} 回调。
         */
        /** {en}
         * @type api
         * @brief Starts capturing the screen audio and/or video stream with the RTC SDK internal module.
         * @param type Media type. See ByteRTCScreenMediaType{@link #ByteRTCScreenMediaType}.
         * @param bundleId The bundle ID of the Extension, which is used to only display your Extension in your app.
         * @notes <br>
         *      + The call of this API takes effects only when you are using RTC SDK to record screen. You will get a warning by OnVideoDeviceWarningEventHandler{@link #EventHandler#OnVideoDeviceWarningEventHandler} after calling this API when the source is set to an external recorder.<br>
         *      + If you start the Extension from the iOS control center, this API do not need to be called. <br>
         *      + After the streams are captured, you need to call PublishScreen{@link #IRTCVideoRoom#PublishScreen} to push the streams to the remote end. <br>
         *      + You will receive OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} when the capturing is started. <br>
         */
        void StartScreenCapture(ScreenMediaType type, string bundleId);

        /** {zh}
         * @type api
         * @brief 使用自定义采集方式，采集屏幕共享时的屏幕音频时，将音频帧推送至 RTC SDK 处进行编码等处理。
         * @param frame 音频数据帧，参见 AudioFrame{@link #AudioFrame}
         * @return  <br>
         *        + 0: 成功。
         *        + < 0: 失败。
         * @notes  <br>
         *        + 调用此接口推送屏幕共享时的自定义采集的音频数据前，必须调用 SetScreenAudioSourceType{@link #IRTCVideo#SetScreenAudioSourceType} 开启屏幕音频自定义采集。  <br>
         *        + 你应每隔 10 毫秒，调用一次此方法推送一次自定义采集的音频帧。一次推送的音频帧中应包含 frame.sample_rate / 100 个音频采样点。比如，假如采样率为 48000Hz，则每次应该推送 480 个采样点。  <br>
         *        + 音频采样格式为 S16。音频缓冲区内的数据格式必须为 PCM 数据，其容量大小应该为 samples × frame.channel × 2。  <br>
         *        + 调用此接口将自定义采集的音频帧推送到 RTC SDK 后，你必须调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 将采集到的屏幕音频推送到远端。在调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 前，推送到 RTC SDK 的音频帧信息会丢失。
         */
        /** {en}
         * @type api
         * @brief Using a custom capture method, when capturing screen audio during screen sharing, push the audio frame to the RTC SDK for encoding and other processing.
         * @param frame  Audio data frame. See AudioFrame{@link #AudioFrame}
         * @return  <br>
         *         + 0: Success.
         *         + < 0: Failure.
         * @notes   <br>
         *         + Before calling this API to push custom collected audio data, you must call SetScreenAudioSourceType{@link #IRTCVideo#SetScreenAudioSourceType} to start custom capture of the screen audio. <br>
         *         + You should call this method every 10 milliseconds to push a custom captured audio frame. A push audio frame should contain frame.sample _rate/100 audio sample points. For example, if the sampling rate is 48000Hz, 480 sampling points should be pushed each time. <br>
         *         + The audio sampling format is S16. The data format in the audio buffer must be PCM data, and its capacity size should be samples × frame.channel × 2. <br>
         *         + After calling this interface to push the custom captured audio frame to the RTC SDK, you must call PublishScreen{@link #IRTCVideoRoom#PublishScreen} to push the captured screen audio to the remote end. Audio frame information pushed to the RTC SDK is lost before calling PublishScreen{@link #IRTCVideoRoom#PublishScreen}.
         */
        int PushScreenAudioFrame(AudioFrame frame);

        /** {zh}
         * @type api
         * @brief 切换视频内部采集时使用的前置/后置摄像头 <br>
         *        调用此接口后，在本地会触发 OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} 回调。
         * @param  cameraId Camera ID，移动端摄像头。参看 CameraID{@link #CameraID}。
         * @return  <br>
         *        + 0：成功  <br>
         *        + !0：失败  <br>
         * @notes  <br>
         *       + 默认使用前置摄像头。
         *       + 如果你正在使用相机进行视频采集，切换操作当即生效；如果相机未启动，后续开启内部采集时，会打开设定的摄像头。
         *       + 如果本地有多个摄像头且想选择特定工作摄像头可通过 IVideoDeviceManager{@link #IVideoDeviceManager} 来控制。  <br>
         */
        /** {en}
         * @type api
         * @brief Toggle the front/postcondition camera used for internal video capture  <br>
         *        After calling this interface, OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} callback will be triggered locally.
         * @param   cameraId Camera ID, mobile end camera. See CameraID{@link #CameraID}.
         * @return   <br>
         *         + 0: Success.
         *         + !0: Failure.
         * @notes   <br>
         *        + Default uses front-facing camera.
         *        + If you are using the camera for video capture, the toggle operation will take effect immediately; if the camera is not activated, the set camera will be turned on when the internal capture is turned on later.
         *        + If you have multiple cameras locally and want to select a specific working camera, you can control it through IVideoDeviceManager{@link #IVideoDeviceManager}. <br>
         */
        int SwitchCamera(CameraID cameraId);

        /** {zh}
         * @type api
         * @brief 推送自定义采集的音频数据到 RTC SDK。
         * @param frame 音频数据帧，参看 AudioFrame{@link #AudioFrame}。
         * @return <br>
         *        + 0: 成功  <br>
         *        + < 0: 失败  <br>
         * @notes  <br>
         *       + 推送自定义采集的音频数据前，必须先调用 SetAudioSourceType{@link #IRTCVideo#SetAudioSourceType} 开启自定义采集。<br>
         *       + 你必须每隔 10 毫秒推送一次外部采集的音频数据。单次推送的 samples (音频采样点个数）应该为 audioFrame.sampleRate / 100。比如设置采样率为 48000 时， 每次应该推送 480 个采样点。  <br>
         *       + 音频采样格式必须为 S16。音频缓冲区内的数据格式必须为 PCM，其容量大小应该为 audioFrame.samples × audioFrame.channel × 2。
         */
        /** {en}
         * @type api
         * @brief Push custom captured audio data to the RTC SDK.
         * @param frame Audio data frame. See AudioFrame{@link #AudioFrame}.
         * @return <br>
         *         + 0: Success.
         *         + < 0: Failure.
         * @notes   <br>
         *        + Before pushing external audio data, you must call SetAudioSourceType{@link #IRTCVideo#SetAudioSourceType} to enable custom audio capture. <br>
         *        + You must push custom captured audio data every 10 milliseconds. The samples (number of audio sampling points) of a single push should be `audioFrame.sample Rate/100`. For example, when the sampling rate is set to 48000, data of 480 sampling points should be pushed each time. <br>
         *        + Audio sampling format must be S16. The data format in the audio buffer must be PCM, and its capacity size should be `audioFrame.samples × audioFrame.channel × 2`.
         */
        int PushExternalAudioFrame(AudioFrame frame);

        /** {zh}
         * @type api
         * @brief 切换音频采集方式
         * @param sourceType 音频数据源，参看 AudioSourceType{@link #AudioSourceType}。
         *             默认使用内部音频采集。音频采集和渲染方式无需对应。
         * @return   <br>
         *        + >0: 成功。
         *        + -1：失败。
         * @notes  <br>
         *      + 进房前后调用此方法均有效。<br>
         *      + 如果你调用此方法由内部采集切换至自定义采集，SDK 会自动关闭内部采集。然后，调用 PushExternalAudioFrame{@link #IRTCVideo#PushExternalAudioFrame} 推送自定义采集的音频数据到 RTC SDK 用于传输。 <br>
         *      + 如果你调用此方法由自定义采集切换至内部采集，你必须再调用 StartAudioCapture{@link #IRTCVideo#StartAudioCapture} 手动开启内部采集。 <br>
         */
        /** {en}
         * @type api
         * @brief Switch the audio capture type.
         * @param sourceType Audio input source type. See AudioSourceType{@link #AudioSourceType} <br>
         *            Use internal audio capture by default. The audio capture type and the audio render type may be different from each other.
         * @return   <br>
         *         + >0: Success.
         *         + -1: Failure.
         * @notes   <br>
         *       + You can call this API before or after joining the room.<br>
         *       + If you call this API to switch from internal audio capture to custom capture, the internal audio capture is automatically disabled. You must call PushExternalAudioFrame{@link #IRTCVideo#PushExternalAudioFrame} to push custom captured audio data to RTC SDK for transmission. <br>
         *       + If you call this API to switch from custom capture to internal capture, you must then call StartAudioCapture{@link #IRTCVideo#StartAudioCapture} to enable internal capture. <br>
         */
        void SetAudioSourceType(AudioSourceType sourceType);

        /** {zh}
         * @type api
         * @brief 推送外部视频帧。
         * @param frame 视频帧的数据信息，参看 VideoFrame{@link #VideoFrame}。
         * @return  <br>
         *        + 0: 成功  <br>
         *        + -1: 失败  <br>
         * @notes  <br>
         *       + 该方法主动将视频帧数据用 VideoFrame{@link #VideoFrame} 类封装后传递给 SDK。  <br>
         *       + 请确保在你调用本方法前已调用 SetVideoSourceType{@link #IRTCVideo#SetVideoSourceType} 设置为自定义视频采集。 <br>
         *       + 当使用纹理数据时， 确保 createEngine中的 `eglContext`与 `frame` 中的 `eglContext` 为 `sharedContext` 或者相同，否则会无法编码
         */
        /** {en}
         * @type api
         * @brief Push external video frames.
         * @param frame  The data information of the video frame. See VideoFrame{@link #VideoFrame}.
         * @return   <br>
         *         + 0: success <br>
         *         + -1: failure <br>
         * @notes   <br>
         *        + This method actively encapsulates the video frame data with the VideoFrame{@link #VideoFrame} class and passes it to the SDK. <br>
         *        + Make sure that SetVideoSourceType{@link #IRTCVideo#SetVideoSourceType} is set to custom video capture before you call this method. <br>
         *        + When using texture data, make sure eglContext in createEngine is sharedContext or the same as eglContext in frame, otherwise it will not be able to encode
         */
        int PushExternalVideoFrame(VideoFrame frame);

        /** {zh}
         * @type api
         * @brief 推送屏幕视频帧
         * @param frame 视频帧 VideoFrame{@link #VideoFrame}
         * @return <br>
         *       + 0 :调用成功 <br>
         *       + -1 :调用失败
         */
        /** {en}
         * @type api
         * @brief  Push screen video frame.
         * @param frame  Video frame VideoFrame{@link #VideoFrame}
         * @return  <br>
         *        + 0: call succeeded <br>
         *        + -1: Failure
         */
        int PushScreenVideoFrame(VideoFrame frame);
    }

    /** {zh}
     * @type keytype
     * @brief 引擎初始化参数
     */
    /** {en}
     * @type keytype
     * @brief Engine initialization parameters.
     */
    public struct RTCVideoEngineParams {
        /** {zh}
         * @brief 应用 ID。
         */
        /** {en}
         * @brief App ID.
         */
        public string AppID;
        /** {zh}
         * @brief 初始化参数。
         */
        /** {en}
         * @brief Initialization parameters.
         */
        public Dictionary<string, object> Params;
    };
    /** {zh}
     * @type keytype
     * @brief 音质档位
     */
    /** {en}
     * @type keytype
     * @brief  Sound quality gear
     */
    public enum AudioProfileType {
        /** {zh}
         * @brief 默认音质。
         *        服务器下发或客户端已设置的 RoomProfileType{@link #RoomProfileType} 的音质配置。
         */
        /** {en}
         * @brief Default sound quality
         *        The sound quality configuration of RoomProfileType{@link #RoomProfileType} set by the server or client side
         */
        kAudioProfileTypeDefault = 0,
        /** {zh}
         * @brief 流畅音质。
         *        单声道，采样率为 16kHz，编码码率为 24kbps。
         *        流畅优先、低延迟、低功耗、低流量消耗，适用于大部分游戏场景，如 MMORPG、MOBA、FPS 等游戏中的小队语音、组队语音、国战语音等。
         */
        /** {en}
         * @brief Smooth sound quality.   <br>
         *         Mono, sampling rate of 16kHz, coding rate of 24 Kbps. <br>
         *         Fluent priority, low latency, low power consumption, low traffic consumption, suitable for most game scenarios, such as MMORPG, MOBA, FPS and other game team voice, team voice, national war voice, etc.
         */
        kAudioProfileTypeFluent = 1,
        /** {zh}
         * @brief 标准音质。  <br>
         *        单声道，采样率为 48kHz，编码码率为 48kbps。 <br>
         *        适用于对音质有一定要求的场景，同时延时、功耗和流量消耗相对适中，适合教育场景和 Sirius 等狼人杀类游戏。
         */
        /** {en}
         * @brief Standard sound quality.   <br>
         *         Mono, the sampling rate is 48kHz, the coding rate is 48 Kbps. <br>
         *         Suitable for scenes with certain requirements for sound quality, while the delay, power consumption and traffic consumption are relatively moderate, suitable for educational scenes and Werewolf games such as Sirius.
         */
        kAudioProfileTypeStandard = 2,
        /** {zh}
         * @brief 高清音质  <br>
         *        双声道，采样率为 48kHz，编码码率为 128 Kbps。 <br>
         *        超高音质，同时延时、功耗和流量消耗相对较大，适用于连麦 PK 等音乐场景。 <br>
         *        游戏场景不建议使用。
         */
        /** {en}
         * @brief High-definition sound quality   <br>
         *         Dual channel, sampling rate is 48kHz, coding rate is 128 Kbps. <br>
         *         Ultra-high sound quality, while the delay, power consumption and flow consumption are relatively large, suitable for music scenes such as Lianmai PK. <br>
         *         Game scenes are not recommended.
         */
        kAudioProfileTypeHD = 3,
        /** {zh}
         * @brief 双声道标准音质。
         *        采样率为 48 KHz，编码码率最大值为 80 Kbps。
         */
        /** {en}
         * @brief Dual-channel standard.
         *        Sample rate: 48 KHz.
         *        Encoding bitrate: 80 Kbps.
         */
        kAudioProfileTypeStandardStereo = 4,
        /** {zh}
         * @brief 单声道音乐音质。采样率为 48 kHz，编码码率最大值为 64 Kbps。
         */
        /** {en}
         * @brief Mono-channel music.
         *        Sample rate: 48 KHz.
         *        Encoding bitrate: 64 Kbps.
         */
        kAudioProfileTypeHDMono = 5
    };

    /** {zh}
     * @type keytype
     * @brief 音频场景类型  <br>
     *        选择音频场景后，SDK 会自动根据客户端音频采集播放设备和状态，适用通话音量/媒体音量。  <br>
     *        你可以调用 SetAudioScenario{@link #IRTCVideo#SetAudioScenario} 设置音频场景。  <br>
     *        如果以下音频场景类型无法满足你的业务需要，请联系技术支持同学进行定制。
     */
    /** {en}
     * @type keytype
     * @brief  Audio scenarios   <br>
     *        After selecting the audio scenario, SDK will automatically select the call/media volume, according to the client-side audio device and status. <br>
     *        You can set the audio scenario by calling SetAudioScenario{@link #IRTCVideo#SetAudioScenario}. <br>
     *        If the following audio scenarios cannot meet your business needs, please contact our technical support team for customization.
     */
    public enum AudioScenarioType {
        /** {zh}
        * @brief 音乐场景。默认为此场景。<br>
        *        此场景适用于对音乐表现力有要求的场景。如音乐直播等。<br>
        *        音频采集播放设备和采集播放状态，到音量类型的映射如下：<br>
        *        <table>
        *           <tr><th></th><th>仅采集音频，不播放音频</th><th>仅播放音频，不采集音频</th><th>采集并播放音频</th><th>备注</th></tr>
        *           <tr><td>设备自带麦克风和扬声器/听筒</td><td>媒体音量</td><td>媒体音量</td><td>通话音量</td><td>/</td></tr>
        *           <tr><td>有线耳机</td><td>媒体音量</td><td>媒体音量</td><td>媒体音量</td><td>/</td></tr>
        *           <tr><td>蓝牙耳机</td><td>媒体音量</td><td>媒体音量</td><td>媒体音量</td><td>即使蓝牙耳机有麦克风，也只能使用设备自带麦克风进行本地音频采集。</td></tr>
        *        </table>
        */
        /** {en}
        * @brief Music scene. Default to this scenario. <br>
        *        This scene is suitable for scenes that require musical expression. Such as live music, etc. <br>
        *         Audio capture playback device and capture playback status, the mapping to the volume type is as follows: <br>
        *         < Table >
        *            <tr><th></th><th>Only Capture audio, do not play audio</th><th>Only play audio, do not collect audio</th><th>Collect and play audio</th><th>Remarks</th><tr>
        *            <tr><td>The Device comes with a microphone and speaker/earpiece</td><td>Media volume</td><td>Media volume</td><td>Call volume</td><td>/</td></tr>
        *            <tr><td>Wired Headset</td><td>Media volume</td><td>Media volume</td><td>Media volume</td><td> Media Volume</td><td>/</td></tr>
        *            <tr><td>Bluetooth Headset</td><td>Media Volume</td><td>Media Volume</td><td>Media Volume</td><td>Even if the Bluetooth headset has a microphone, you can only use the device's own microphone for local audio acquisition.</td></tr>
        *         </table >
        */
        kAudioScenarioTypeMusic = 0,
        /** {zh}
        * @brief 高质量通话场景。<br>
        *        此场景适用于对音乐表现力有要求的场景。但又希望能够使用蓝牙耳机上自带的麦克风进行音频采集的场景。
        *        此场景可以兼顾外放/使用蓝牙耳机时的音频体验；并尽可能避免使用蓝牙耳机时音量类型切换导致的听感突变。<br>
        *        音频采集播放设备和采集播放状态，到音量类型的映射如下：<br>
        *        <table>
        *           <tr><th></th><th>仅采集音频，不播放音频</th><th>仅播放音频，不采集音频</th><th>采集并播放音频</th> <th>备注</th> </tr>
        *           <tr><td>设备自带麦克风和扬声器/听筒</td><td>媒体音量</td><td>媒体音量</td><td>通话音量</td><td>/</td></tr>
        *           <tr><td>有线耳机</td><td>媒体音量</td><td>媒体音量</td><td>媒体音量</td><td>/</td></tr>
        *           <tr><td>蓝牙耳机</td><td>通话音量</td><td>通话音量</td><td>通话音量</td><td>能够使用蓝牙耳机上自带的麦克风进行音频采集。</td></tr>
        *        </table>
        */
        /** {en}
        * @brief High-quality call scenarios. <br>
        *        This scene is suitable for scenes that require musical expression. However, I hope to be able to use the microphone on the Bluetooth headset for audio collection.
        *        This scene can take into account the audio experience when playing/using the Bluetooth headset; and avoid sudden changes in hearing caused by volume type switching when using the Bluetooth headset as much as possible. <br>
        *         Audio capture playback device and capture playback status, the mapping to the volume type is as follows: <br>
        *         < Table>
        *            <tr><th></th><th>Only Capture audio, do not play audio</th><th>Only play audio, do not collect audio</th><th> Capture and play audio </th><th> Remarks </th></tr>
        *            <tr><td>The Device comes with a microphone and speaker/earpiece </td><td> media volume </td><td> media volume </td><td> call volume </td><td>/</td></tr>
        *            <tr><td> Wired headset </td><td> media volume </td><td> media volume </td> <td> media volume </td><td> Media Volume </td><td>/</td></tr>
        *            <tr><td> Bluetooth Headset </td><td> Call Volume </td><td> Call Volume </td><td> Call Volume </td><td> Ability to use the microphone included in the Bluetooth headset for audio capture. </td></tr>
        *         </table>
        */
        kAudioScenarioTypeHighQualityCommunication = 1,
        /** {zh}
        * @brief 纯通话音量场景。<br>
        *        此场景下，无论客户端音频采集播放设备和采集播放状态，全程使用通话音量。
        *        适用于需要频繁上下麦的通话或会议场景。<br>
        *        此场景可以保持统一的音频模式，不会有音量突变的听感；
        *        最大程度上的消除回声，使通话清晰度达到最优；
        *        使用蓝牙耳机时，能够使用蓝牙耳机上自带的麦克风进行音频采集。<br>
        *        但是，使用媒体音量进行播放的其他音频的音量会被压低，且音质会变差。
        */
        /** {en}
        * @brief Pure call volume scene. <br>
        *         In this scenario, regardless of the client side audio acquisition and playback device and the acquisition and playback status, the call volume is used throughout the process.
        *         Suitable for calls or meeting scenarios that require frequent access. <br>
        *        This scene can maintain a unified audio mode without sudden volume changes;
        *        Eliminate echoes to the greatest extent, so that the call definition is optimal;
        *        When using Bluetooth headsets, you can use the microphone on the Bluetooth headset for audio collection. <br>
        *         However, the volume of other audio played using media volume will be lowered and the sound quality will deteriorate.
        */
        kAudioScenarioTypeCommunication = 2,
        /** {zh}
        * @brief 纯媒体场景。一般不建议使用。<br>
        *        此场景下，无论客户端音频采集播放设备和采集播放状态，全程使用媒体音量。
        */
        /** {en}
        * @brief Pure media scene. It is generally not recommended. <br>
        *         In this scenario, regardless of the client side audio acquisition and playback device and the acquisition and playback status, the media volume is used throughout the process.
        */
        kAudioScenarioTypeMedia = 3,
        /** {zh}
        * @brief 游戏媒体场景。仅适合游戏场景。  <br>
        *        此场景下，蓝牙耳机时使用通话音量，其它设备使用媒体音量。
        *        外放通话且无游戏音效消除优化时，极易出现回声和啸叫。
        */
        /** {en}
        * @brief Game media scene. Only suitable for game scenes.   <br>
        *         In this scenario, the Bluetooth headset uses the call volume, and other devices use the media volume.
        *        Echo and howling are easy to occur when talking outside and there is no game sound cancellation optimization.
        */
        kAudioScenarioTypeGameStreaming = 4,
    };

    /** {zh}
     * @type keytype
     * @brief 音频播放路由。
     */
    /** {en}
     * @type keytype
     * @brief Audio playback route
     */
    public enum AudioRouteDevice {
        /** {zh}
         * @brief 未知设备
         */
        /** {en}
         * @brief unknown devices
         */
        kAudioRouteDefault = -1,
        /** {zh}
         * @brief 有线耳机
         */
        /** {en}
         * @brief Wired earphones
         */
        kAudioRouteDeviceHeadset = 1,
        /** {zh}
         * @brief 听筒。设备自带的，一般用于通话的播放硬件。
         */
        /** {en}
         * @brief Earpiece. Built-in device for calling.
         */
        kAudioRouteDeviceEarpiece = 2,
        /** {zh}
         * @brief 扬声器。设备自带的，一般用于免提播放的硬件。
         */
        /** {en}
         * @brief Speaker. Built-in device for hands-free audio playing.
         */
        kAudioRouteDeviceSpeakerphone = 3,
        /** {zh}
         * @brief 蓝牙耳机
         */
        /** {en}
         * @brief Bluetooth earphones
         */
        kAudioRouteDeviceHeadsetBluetooth = 4,
        /** {zh}
         * @brief USB 设备
         */
        /** {en}
         * @brief USB device
         */
        kAudioRouteDeviceHeadsetUSB = 5
    };


    /** {zh}
    * @type keytype
    * @brief 音频属性信息提示的相关配置。
    */
    /** {en}
    * @type keytype
    * @brief  Configuration related to audio attribute information prompt.
    */
    public struct AudioPropertiesConfig {
        /** {zh}
         * @brief 信息提示间隔，单位：ms
         * @notes  <br>
         *       + <= 0: 关闭信息提示  <br>
         *       + >0 && <=100: 开启信息提示，不合法的 interval 值，SDK 自动设置为 100ms  <br>
         *       + > 100: 开启信息提示，并将信息提示间隔设置为此值  <br>
         */
        /** {en}
         * @brief Prompt interval in ms
         * @notes   <br>
         *        + <= 0: Turn off prompt <br>
         *        + > 0 && <= 100: Invalid interval value, automatically reset to 100ms <br>
         *        + > 100: the actual value of interval
         */
        public int Interval;
        /** {zh}
         * @brief 是否开启音频频谱检测
         */
        /** {en}
         * @brief Whether to enable audio spectrum detection
         */
        public bool EnableSpectrum;
        /** {zh}
         * @brief 是否开启人声检测 (VAD)
         */
        /** {en}
         * @brief Whether to enable Voice Activity Detection
         */
        public bool EnableVad;
    };

    /** {zh} 
    * @type keytype
    * @brief 音频回调方法
    */
    /** {en}
    * @type keytype
    * @brief Audio data callback method
    */
    public enum AudioFrameCallbackMethod {
        /** {zh}
         * @brief 本地麦克风录制的音频数据回调 OnRecordAudioFrameEventHandler{@link #EventHandler#OnRecordAudioFrameEventHandler}
         */
        /** {en}
         * @brief OnRecordAudioFrameEventHandler{@link #EventHandler#OnRecordAudioFrameEventHandler}, the callback of the audio data recorded by local microphone.
         */
        kAudioFrameCallbackRecord,
        /** {zh}
         * @brief 订阅的远端所有用户混音后的音频数据回调 OnPlaybackAudioFrameEventHandler{@link #EventHandler#OnPlaybackAudioFrameEventHandler}
         */
        /** {en}
         * @brief OnPlaybackAudioFrameEventHandler{@link #EventHandler#OnPlaybackAudioFrameEventHandler}, the callback of the mixed audio data of all remote users subscribed by the local user.
         */
        kAudioFrameCallbackPlayback,
        /** {zh}
         * @brief 本地麦克风录制和订阅的远端所有用户混音后的音频数据回调 OnMixedAudioFrameEventHandler{@link #EventHandler#OnMixedAudioFrameEventHandler}
         */
        /** {en}
         * @brief OnMixedAudioFrameEventHandler{@link #EventHandler#OnMixedAudioFrameEventHandler}, the callback of the mixed audio data including the data recorded by local microphone and that of all remote users subscribed by the local user.
         */
        kAudioFrameCallbackMixed,
        /** {zh}
         * @brief 订阅的远端每个用户混音前的音频数据回调 OnRemoteUserAudioFrameEventHandler{@link #EventHandler#OnRemoteUserAudioFrameEventHandler}
         */
        /** {en}
         * @brief OnRemoteUserAudioFrameEventHandler{@link #EventHandler#OnRemoteUserAudioFrameEventHandler}, the callback of the audio data before mixing of each remote user subscribed by the local user.
         */
        kAudioFrameCallbackRemoteUser,
        /** {zh}
         * @brief 本地屏幕录制的音频数据回调 OnRecordScreenAudioFrameEventHandler{@link #EventHandler#OnRecordScreenAudioFrameEventHandler}
         */
        /** {en}
         * @brief OnRecordScreenAudioFrameEventHandler{@link #EventHandler#OnRecordScreenAudioFrameEventHandler}, the callback of screen audio data captured locally.
         */
        kAudioFrameCallbackRecordScreen,
    };
    
    /** {zh} 
     * @type keytype
     * @brief 媒体流信息同步的流类型
     */
    /** {en} 
     * @type keytype
     * @brief  Stream type for media stream information synchronization
     */
    public enum SyncInfoStreamType {
        /** {zh}
            * @brief 音频流
            */
        /** {en}
         * @brief Audio stream
         */
        kSyncInfoStreamTypeAudio = 0
    };

    /** {zh} 
    * @type keytype
    * @brief 音频参数格式
    */
    /** {en} 
    * @type keytype
    * @brief Audio parameters format
    */
    public struct AudioFormat {
        /** {zh}
            * @brief 音频采样率，参看 AudioSampleRate{@link #AudioSampleRate}。
            */
        /** {en}
         * @brief Audio sample rate. See AudioSampleRate{@link #AudioSampleRate}.
         */
        public AudioSampleRate SampleRate;
        /** {zh}
            * @brief 音频声道，参看 AudioChannel{@link #AudioChannel}。
            */
        /** {en}
         * @brief Audio channel. See AudioChannel{@link #AudioChannel}.
         */
        public AudioChannel Channel;
    };

    /** {zh}
 * @type keytype
 * @brief 返回给音频处理器的音频类型
 */
    /** {en}
     * @type keytype
     * @brief The type of the audio for the audio processor
     */
    public enum AudioProcessorMethod {
        /** {zh}
         * @brief 本地采集的音频 
         */
        /** {en}
         * @brief Locally captured audio frame
         */
        kAudioFrameProcessorRecord = 0,
        /** {zh}
         * @brief 远端音频流的混音音频
         */
        /** {en}
         * @brief The mixed remote audio
         */
        kAudioFrameProcessorPlayback = 1,
        /** {zh}
         * @brief 各个远端音频流
         */
        /** {en}
         * @brief The audio streams from remote users
         */
        kAudioFrameProcessorRemoteUser = 2,
        /** {zh}
         * @hidden(Windows,Linux,macOS)
         * @brief 软件耳返音频。
         */
        /** {en}
         * @hidden(Windows,Linux,macOS)
         * @brief The SDK-level in-ear monitoring.
         */
        kAudioFrameProcessorEarMonitor = 3,
        /** {zh}
         * @hidden(Linux)
         * @brief 屏幕共享音频。
         */
        /** {en}
         * @hidden(Linux)
         * @brief The shared-screen audio.
         */
        kAudioFrameProcessorScreen = 4,
    };

    /** {zh}
     * @type keytype
     * @brief 视频采集模式
     */
    /** {en}
     * @type keytype
     * @brief Video capture preference
     */
    public enum CapturePreference {
       /** {zh}
        * @brief （默认）自动设置采集参数。
        *        SDK在开启采集时根据服务端下发的采集配置结合编码参数设置最佳采集参数。
        */
       /** {en}
        * @brief (Default) Video capture preference: auto <br>
        *        SDK determines the best video capture parameters referring to the camera output parameters and the encoder configuration.
        */
        KAuto = 0,
       /** {zh}
        * @brief 手动设置采集参数，包括采集分辨率、帧率。
        */
       /** {en}
        * @brief Video capture preference: manual <br>
        *        Set the resolution and the frame rate manually.
        */
        KManual = 1,
       /** {zh}
        * @brief 采集参数与编码参数一致，即在 SetVideoEncoderConfig1{@link #IRTCVideo#SetVideoEncoderConfig1} 中设置的参数。
        */
       /** {en}
        * @brief Video capture preference: encoder configuration <br>
        *        The capture parameters are the same with the parameters set in SetVideoEncoderConfig1{@link #IRTCVideo#SetVideoEncoderConfig1}.
        */
        KAutoPerformance = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 视频采集配置
     */
    /** {en}
     * @type keytype
     * @brief  Video Capture Configuration
     */
    public struct VideoCaptureConfig {
        /** {zh}
         * @type keytype
         * @brief 视频采集模式，参看 CapturePreference{@link #CapturePreference}。
         */
        /** {en}
         * @type keytype
         * @brief Video capture preference. See CapturePreference{@link #CapturePreference}.
         */
        public CapturePreference CapturePreference;
        /** {zh}
         * @brief 视频采集分辨率的宽度，单位：px。
         */
        /** {en}
         * @brief The width of video capture resolution in px.
         */
        public int Width;
        /** {zh}
         * @brief 视频采集分辨率的高度，单位：px。
         */
        /** {en}
         * @brief The height of video capture resolution in px.
         */
        public int Height;
        /** {zh}
         * @brief 视频采集帧率，单位：fps。
         */
        /** {en}
         * @brief Video capture frame rate in fps.
         */
        public int FrameRate;
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧缩放模式
     */
    /** {en}
     * @type keytype
     * @brief  Video frame scale mode
     */
    public enum VideoStreamScaleMode {
        /** {zh}
         * @brief 自动模式，默认值为 kVideoStreamScaleModeFitWithCropping
         */
        /** {en}
         * @brief Auto mode, default to FitWithCropping.
         */
        kVideoStreamScaleModeAuto = 0,
        /** {zh}
         * @brief 对视频帧进行缩放，直至充满和视窗分辨率一致为止。这一过程不保证等比缩放。这一过程不保证等比缩放。
         */
        /** {en}
         * @brief Stretch the video frame until the video frame and the window have the same resolution. The video frame's aspect ratio can be changed as it is automatically stretched to fill the window, but the whole image is visible.
         */
        kVideoStreamScaleModeStretch = 1,
        /** {zh}
         * @brief 视窗填满优先。<br>
         *        视频帧等比缩放，直至视窗被视频填满。如果视频帧长宽比例与视窗不同，视频帧的多出部分将无法显示。<br>
         *        缩放完成后，视频帧的一边长和视窗的对应边长一致，另一边长大于等于视窗对应边长。
         */
        /** {en}
         * @brief  Fit the window with cropping <br>
         *         Scale the video frame uniformly until the window is filled. If the video frame's aspect ratio is different from that of the window, the extra part of the video frame will be cropped. <br>
         *         After the scaling process is completed, the width or height of the video frame will be consistent with that of the window, and the other dimension will be greater than or equal to that of the window.
         */
        kVideoStreamScaleModeFitWithCropping = 2,
        /** {zh}
         * @brief 视频帧内容全部显示优先。<br>
         *        视频帧等比缩放，直至视频帧能够在视窗上全部显示。如果视频帧长宽比例与视窗不同，视窗上未被视频帧填满区域将被涂黑。<br>
         *        缩放完成后，视频帧的一边长和视窗的对应边长一致，另一边长小于等于视窗对应边长。
         */
        /** {en}
         * @brief  Fit the window with filling <br>
         *         Scale the video frame uniformly until its width or height reaches the boundary of the window. If the video frame's aspect ratio is different from that of the window, the area that is not filled will be black. <br>
         *         After the scaling process is completed, the width or height of the video frame will be consistent with that of the window, and the other dimension will be less than or equal to that of the window.
         */
        kVideoStreamScaleModeFitWithFilling = 3,
    };

    /** {zh}
     * @type keytype
     * @brief 编码策略偏好。
     */
    /** {en}
     * @type keytype
     * @brief  Encoder preference.
     */
    public enum VideoEncodePreference {
        /** {zh}
         * @brief 无偏好。不降低帧率和分辨率。
         */
        /** {en}
         * @brief No preference. The frame rate and the resolution will not be adjusted.
         */
        kVideoEncodePreferenceDisabled = 0,
        /** {zh}
         * @brief （默认值）帧率优先。
         */
        /** {en}
         * @brief (Default) Frame rate first.
         */
        kVideoEncodePreferenceFramerate,
        /** {zh}
         * @brief 分辨率优先。
         */
        /** {en}
         * @brief Resolution first.
         */
        kVideoEncodePreferenceQuality,
        /** {zh}
         * @brief 平衡帧率与分辨率。
         */
        /** {en}
         * @brief Balancing resolution and frame rate.
         */
        kVideoEncodePreferenceBalance,
    };

    /** {zh}
     * @type keytype
     * @brief 屏幕编码配置。参考 [设置视频发布参数](https://www.volcengine.com/docs/6348/70122)。
     */
    /** {en}
     * @type keytype
     * @brief  The encoding configuration for shared-screen streams. See [Setting Video Encoder Configuration](https://docs.byteplus.com/byteplus-rtc/docs/70122).
     */
    public struct ScreenVideoEncoderConfig {
        /** {zh}
         * @brief 视频宽度，单位：像素。
         */
        /** {en}
         * @brief Width(in px).
         */
        public int Width;
        /** {zh}
         * @brief 视频高度，单位：像素。
         */
        /** {en}
         * @brief Height(in px).
         */
        public int Height;
        /** {zh}
         * @brief 视频帧率，单位：fps。
         */
        /** {en}
         * @brief The frame rate(in fps).
         */
        public int FrameRate;
        /** {zh}
         * @brief 最大编码码率，使用 SDK 内部采集时可选设置，自定义采集时必须设置，单位：kbps。默认值为 –1。
         *        设为 -1 即适配码率模式，系统将根据输入的分辨率和帧率自动计算适用的码率。
         *        设为 0 则不对视频流进行编码发送。
         */
        /** {en}
         * @brief The maximum bitrate(in kbps). Optional for internal capture while mandatory for custom capture. The default value is –1.
         *        If you set this value to -1, RTC will automatically recommend the bitrate based on the input resolution and frame rate.
         *        If you set this value to 0, streams will not be encoded and published.
         */
        public int MaxBitrate;
        /** {zh}
         * @brief 最小编码码率，使用 SDK 内部采集时可选设置，自定义采集时必须设置，单位：kbps。
         *        最小编码码率必须小于或等于最大编码，否则不对视频流进行编码发送。
         */
        /** {en}
         * @brief The minimum bitrate(in kbps).Optional for internal capture while mandatory for custom capture.
         *        The minimum bitrate must be set lower than the maximum bitrate. Otherwise, streams will not be encoded and published.
         */
        public int MinBitrate;
        /** {zh}
         * @brief 屏幕流编码模式。参见 ScreenVideoEncodePreference{@link #ScreenVideoEncodePreference}。
         */
        /** {en}
         * @brief The encoding modes for shared-screen streams.See ScreenVideoEncoderPreference{@link #ScreenVideoEncodePreference}.
         */
        public ScreenVideoEncodePreference EncoderPreference;
    };

    /** {zh}
     * @type keytype
     * @brief 屏幕流编码模式。默认采用清晰模式。若在采集时设置 ScreenFilterConfig{@link #ScreenFilterConfig} 排除指定窗口，共享视频时帧率无法达到 30fps。
     */
    /** {en}
     * @type keytype
     * @brief The encoding modes for shared-screen streams. The default mode is the high-resolution mode. If you exclude specific windows by setting ScreenFilterConfig{@link #ScreenFilterConfig}, the frame rate of the shared-screen stream will be slower than 30fps。
     */
    public enum ScreenVideoEncodePreference {
        /** {zh}
         * @brief 智能模式。根据屏幕内容智能决策选择流畅模式或清晰模式。
         */
        /** {en}
         * @brief The automatic mode. The encoding mode is dynamically determined by RTC based on the content.
         */
        kScreenVideoEncodePreferenceAuto = 0,
        /** {zh}
         * @brief 流畅模式，优先保障帧率。适用于共享游戏、视频等动态画面。
         */
        /** {en}
         * @brief The high frame rate mode. Ensure the highest framerate possible under challenging network conditions. This mode is designed to share audiovisual content, including games and videos.
         */
        kScreenVideoEncodePreferenceFramerate,
        /** {zh}
         * @brief 清晰模式，优先保障分辨率。适用于共享PPT、文档、图片等静态画面。
         */
        /** {en}
         * @brief The high-resolution mode. Ensure the highest resolution possible under challenging network conditions. This mode is designed to share micro-detailed content, including slides, documents, images, illustrations, or graphics.
         */
        kScreenVideoEncodePreferenceQuality,

    };

    /** {zh}
     * @type keytype
     * @brief 视频流参数描述。
     */
    /** {en}
     * @type keytype
     * @brief Video stream configuration
     */
    public struct VideoEncoderConfig {
        /** {zh}
         * @brief 视频宽度，单位：px
         */
        /** {en}
         * @brief Width of the video frame in px
         */
        public int Width;
        /** {zh}
         * @brief 视频高度，单位：px
         */
        /** {en}
         * @brief Height of the video frame in px
         */
        public int Height;
        /** {zh}
         * @brief 视频帧率，单位：fps
         */
        /** {en}
         * @brief Video frame rate in fps
         */
        public int FrameRate;
        /** {zh}
         * @brief 最大编码码率，使用 SDK 内部采集时可选设置，自定义采集时必须设置，单位：kbps。  <br>
         *        内部采集模式下默认值为 -1，即适配码率模式，系统将根据输入的分辨率和帧率自动计算适用的码率。 <br>
         *        设为 0 则不对视频流进行编码发送。
         */
        /** {en}
         * @brief Maximum bit rate in kbps. Optional for internal capturing while mandatory for custom capturing.  <br>
         *        The default value is -1 in internal capturing mode, SDK will automatically calculate the applicable bit rate based on the input resolution and frame rate.  <br>
         *        No stream will be encoded and published if you set this parameter to 0.
         */
        public int MaxBitrate;
        /** {zh}
         * @brief 视频最小编码码率, 单位 kbps。编码码率不会低于 `MinBitrate`。<br>
         *        默认值为 `0`。<br>
         *        范围：[0, MaxBitrate)，当 `MaxBitrate` < `MinBitrate` 时，为适配码率模式。<br>
         *        以下情况，设置本参数无效：<br>
         *        + 当 `MaxBitrate` 为 `0` 时，不对视频流进行编码发送。<br>
         *        + 当 `MaxBitrate` < `0` 时，适配码率模式。
         */
        /** {en}
         * @brief Minimum video encoding bitrate in kbps. The encoding bitrate will not be lower than the `MinBitrate`.<br>
         *        It defaults to `0`. <br>
         *        It ranges within [0, MaxBitrate). When `MaxBitrate` < `MinBitrate`, the bitrate is self-adpapted.<br>
         *        In the following circumstance, the assignment to this variable has no effect:<br>
         *        + When `MaxBitrate` = `0`, the video encoding is disabled.<br>
         *        + When `MaxBitrate` < `0`, the bitrate is self-adapted.
         */
        public int MinBitrate;
        /** {zh}
         * @brief 编码策略偏好。参看 VideoEncodePreference{@link #VideoEncodePreference}。
         */
        /** {en}
         * @brief Encoding policy preference. See VideoEncodePreference{@link #VideoEncodePreference}.
         */
        public VideoEncodePreference EncoderPreference;
    };

    /** {zh}
     * @type keytype
     * @brief 视频输入源类型
     */
    /** {en}
     * @type keytype
     * @brief Video source type
     */
    public enum VideoSourceType {
        /** {zh}
        * @brief 自定义采集视频源
        */
        /** {en}
        * @brief Custom video source
        */
        VideoSourceTypeExternal = 0,
        /** {zh}
        * @brief 内部采集视频源
        */
        /** {en}
        * @brief Internal video capture
        */
        VideoSourceTypeInternal = 1,
        /** {zh}
        * @brief 自定义编码视频源。
        *        你仅需推送分辨率最大的一路编码后视频流，SDK 将自动转码生成多路小流
        */
        /** {en}
        * @brief Custom encoded video source.   <br>
        *        Push the encoded video stream with the largest resolution, and the SDK will automatically transcode to generate multiple lower-quality streams for Simulcast.
        */
        VideoSourceTypeEncodedWithAutoSimulcast = 2,
        /** {zh}
        * @brief 自定义编码视频源。  <br>
        *        SDK 不会自动生成多路流，你需要自行生成并推送多路流
        */
        /** {en}
        * @brief Custom encoded video source.   <br>
        *         The SDK does not automatically generate multiple streams for Simulcast, you need to generate and push streams of different qualities.
        */
        VideoSourceTypeEncodedWithoutAutoSimulcast = 3,
    };

    /** {zh}
     * @type keytype
     * @brief 镜像类型
     */
    /** {en}
     * @type keytype
     * @brief  Mirror type
     */
    public enum MirrorType {
        /** {zh}
        * @brief 本地预览和编码传输时均无镜像效果
        */
        /** {en}
        * @brief The preview and the published video stream are not mirrored.
        */
        kMirrorTypeNone = 0,
        /** {zh}
        * @brief 本地预览时有镜像效果，编码传输时无镜像效果
        */
        /** {en}
        * @brief The preview is mirrored. The published video stream is not mirrored.
        */
        kMirrorTypeRender = 1,
        /** {zh}
        * @brief 本地预览和编码传输时均有镜像效果
        */
        /** {en}
        * @brief The preview and the published video stream are mirrored.
        */
        kMirrorTypeRenderAndEncoder = 3,
    };

    /** {zh}
     * @type keytype
     * @brief 视频旋转模式
     */
    /** {en}
     * @type keytype
     * @brief Video rotation mode.
     */
    public enum VideoRotationMode {
        /** {zh}
        * @brief 跟随 App 界面方向
        */
        /** {en}
        * @brief Follow the rotation of app.
        */
        kVideoRotationModeFollowApp = 0,
        /** {zh}
        * @brief 跟随设备重力方向
        */
        /** {en}
        * @brief Follow the rotation of the G sensor.
        */
        kVideoRotationModeFollowGSensor = 1,
    };

    /** {zh}
 * @type keytype
 * @brief 音频文件录制内容来源。
 */
    /** {en}
     * @type keytype
     * @brief Audio file recording source type.
     */
    public enum AudioFrameSource {
        /** {zh}
         * @brief 本地麦克风采集的音频数据。
         */
        /** {en}
         * @brief The audio captured by the local microphone.
         */
        kAudioFrameSourceMic = 0,
        /** {zh}
         * @brief 远端所有用户混音后的数据
         */
        /** {en}
         * @brief The audio got by mixing all remote user's audio.
         */
        kAudioFrameSourcePlayback = 1,
        /** {zh}
         * @brief 本地麦克风和所有远端用户音频流的混音后的数据
         */
        /** {en}
         * @brief The audio got by mixing the local captured audio and all remote user's audio.
         */
        kAudioFrameSourceMixed = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 音频质量。
     */
    /** {en}
     * @type keytype
     * @brief Audio quality.
     */
    public enum AudioQuality {
        /** {zh}
         * @brief 低音质
         */
        /** {en}
         * @brief low quality
         */
        kAudioQualityLow = 0,
        /** {zh}
         * @brief 中音质
         */
        /** {en}
         * @brief medium quality
         */
        kAudioQualityMedium = 1,
        /** {zh}
         * @brief 高音质
         */
        /** {en}
         * @brief high quality
         */
        kAudioQualityHigh = 2,
        /** {zh}
         * @brief 超高音质
         */
        /** {en}
         * @brief ultra high quality
         */
        kAudioQualityUltraHigh = 3,
    };

    /** {zh}
     * @type keytype
     * @brief 录音配置
     */
    /** {en}
     * @type keytype
     * @brief Audio recording config
     */
    public struct AudioRecordingConfig {
        /** {zh}
         * @brief 录制文件路径。一个有读写权限的绝对路径，包含文件名和文件后缀。
         * @notes 录制文件的格式仅支持 .aac 和 .wav。
         */
        /** {en}
         * @brief Absolute path of the recorded file, file name included. The App must have the write and read permission of the path.
         * @notes The files format is restricted to .aac and .wav.
         */
        public string absolute_file_name;
        /** {zh}
         * @brief 录音内容来源，参看 AudioFrameSource{@link #AudioFrameSource}。
         */
        /** {en}
         * @brief The source of the recording. See AudioFrameSource{@link #AudioFrameSource}.
         */
        public AudioFrameSource frame_source;
        /** {zh}
         * @brief 录音采样率。参看 AudioSampleRate{@link #AudioSampleRate}。
         */
        /** {en}
         * @brief See AudioSampleRate{@link #AudioSampleRate}.
         */
        public AudioSampleRate sample_rate;
        /** {zh}
         * @brief 录音音频声道。参看 AudioChannel{@link #AudioChannel}。
         * @notes 如果录制时设置的的音频声道数与采集时的音频声道数不同：<br>
         *        + 如果采集的声道数为 1，录制的声道数为 2，那么，录制的音频为经过单声道数据拷贝后的双声道数据，而不是立体声。<br>
         *        + 如果采集的声道数为 2，录制的声道数为 1，那么，录制的音频为经过双声道数据混合后的单声道数据。
         */
        /** {en}
         * @brief Number of audio channels. See AudioChannel{@link #AudioChannel}.
         * @notes If number of audio channels of recording is different than that of audio capture, the behavior is: <br>
         *        + If the number of capture is 1, and the number of recording is 2, the recorded audio is two-channel data after copying mono-channel data. <br>
         *        + If the number of capture is 2, and the number of recording is 1, the recorded audio is recorded by mixing the audio of the two channels.
         */
        public AudioChannel channel;
        /** {zh}
         * @brief 录音音质。仅在录制文件格式为 .aac 时可以设置。参看 AudioQuality{@link #AudioQuality}。
         * @notes 采样率为 32kHz 时，不同音质录制文件（时长为 10min）的大小分别是： <br>
         *        + 低音质：1.2MB；<br>
         *        + 中音质：2MB；<br>
         *        + 高音质：3.75MB；<br>
         *        + 超高音质：7.5MB。
         */
        /** {en}
         * @brief Recording quality. Only valid for .aac file. See AudioQuality{@link #AudioQuality}.
         * @notes When the sample rate is 32kHz, the file (10min) size for different qualities are: <br>
         *        + low: 1.2MB; <br>
         *        + medium: 2MB; <br>
         *        + high: 3.75MB; <br>
         *        + ultra high: 7.5MB.
         */
        public AudioQuality quality;
    };

    /** {zh}
     * @type keytype
     * @brief SDK 与信令服务器连接状态。
     */
    /** {en}
     * @type keytype
     * @brief SDK  Connection status with the signaling server.
     */
    public enum ConnectionState {
        /** {zh}
         * @brief 连接断开。
         */
        /** {en}
         * @brief Connection disconnected.
         */
        kConnectionStateDisconnected = 1,
        /** {zh}
         * @brief 首次连接，正在连接中。
         */
        /** {en}
         * @brief First connection, connecting now.
         */
        kConnectionStateConnecting = 2,
        /** {zh}
         * @brief 首次连接成功。
         */
        /** {en}
         * @brief The first connection was successful.
         */
        kConnectionStateConnected = 3,
        /** {zh}
         * @brief 连接断开后重新连接中。
         */
        /** {en}
         * @brief Reconnect after disconnection.
         */
        kConnectionStateReconnecting = 4,
        /** {zh}
         * @brief 连接断开后重连成功。
         */
        /** {en}
         * @brief Successful reconnection after disconnection.
         */
        kConnectionStateReconnected = 5,
        /** {zh}
         * @brief 网络连接断开超过 10 秒，仍然会继续重连。
         */
        /** {en}
         * @brief If the network connection is disconnected for more than 10 seconds, it will continue to reconnect.
         */
        kConnectionStateLost = 6,
    };

    /** {zh}
     * @type keytype
     * @brief SDK 网络连接类型。
     */
    /** {en}
     * @type keytype
     * @brief SDK  Network connection type.
     */
    public enum NetworkType {
        /** {zh}
         * @brief 网络连接类型未知。
         */
        /** {en}
         * @brief Network connection type unknown.
         */
        kNetworkTypeUnknown = -1,
        /** {zh}
         * @brief 网络连接已断开。
         */
        /** {en}
         * @brief The network connection has been disconnected.
         */
        kNetworkTypeDisconnected = 0,
        /** {zh}
         * @brief 网络连接类型为 LAN 。
         */
        /** {en}
         * @brief The network connection type is LAN.
         */
        kNetworkTypeLAN = 1,
        /** {zh}
         * @brief 网络连接类型为 Wi-Fi（包含热点）。
         */
        /** {en}
         * @brief The network connection type is Wi-Fi (including hotspots).
         */
        kNetworkTypeWIFI = 2,
        /** {zh}
         * @brief 网络连接类型为 2G 移动网络。
         */
        /** {en}
         * @brief The network connection type is 2G mobile network.
         */
        kNetworkTypeMobile2G = 3,
        /** {zh}
         * @brief 网络连接类型为 3G 移动网络。
         */
        /** {en}
         * @brief The network connection type is 3G mobile network.
         */
        kNetworkTypeMobile3G = 4,
        /** {zh}
         * @brief 网络连接类型为 4G 移动网络。
         */
        /** {en}
         * @brief The network connection type is 4G mobile network.
         */
        kNetworkTypeMobile4G = 5,
        /** {zh}
         * @brief 网络连接类型为 5G 移动网络。
         */
        /** {en}
         * @brief The network connection type is 5G mobile network.
         */
        kNetworkTypeMobile5G = 6,
    };

    /** {zh}
     * @type keytype
     * @brief 音频设备类型
     */
    /** {en}
     * @type keytype
     * @brief Audio device type
     */
    public enum RTCAudioDeviceType {
        /** {zh}
        * @brief 未知设备类型
        */
        /** {en}
        * @brief Unknown device type
        */
        kRTCAudioDeviceTypeUnknown = -1,
        /** {zh}
        * @brief 音频渲染设备类型
        */
        /** {en}
        * @brief Audio Rendering Device Type
        */
        kRTCAudioDeviceTypeRenderDevice = 0,
        /** {zh}
        * @brief 音频采集设备类型
        */
        /** {en}
        * @brief Audio Acquisition Device Type
        */
        kRTCAudioDeviceTypeCaptureDevice = 1,
        /** {zh}
        * @brief 屏幕流音频设备
        */
        /** {en}
        * @brief Screen Streaming Audio Devices
        */
        kRTCAudioDeviceTypeScreenCaptureDevice = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 视频设备类型
     */
    /** {en}
     * @type keytype
     * @brief  Video device type.
     */
    public enum RTCVideoDeviceType {
        /** {zh}
        * @brief 未知设备类型
        */
        /** {en}
        * @brief Unknown device type
        */
        kRTCVideoDeviceTypeUnknown = -1,
        /** {zh}
        * @brief 视频渲染设备类型
        */
        /** {en}
        * @brief Audio Rendering Device Type
        */
        kRTCVideoDeviceTypeRenderDevice = 0,
        /** {zh}
        * @brief 视频采集设备类型
        */
        /** {en}
        * @brief Audio Acquisition Device Type
        */
        kRTCVideoDeviceTypeCaptureDevice = 1,
        /** {zh}
        * @brief 屏幕流视频设备
        */
        /** {en}
        * @brief Screen Streaming Audio Devices
        */
        kRTCVideoDeviceTypeScreenCaptureDevice = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 媒体设备状态。通过 OnAudioDeviceStateChangedEventHandler{@link #EventHandler#OnAudioDeviceStateChangedEventHandler} 或 OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler} 回调设备状态。
     */
    /** {en}
     * @type keytype
     * @brief  Media device state. You will be informed of this state byOnAudioDeviceStateChangedEventHandler{@link #EventHandler#OnAudioDeviceStateChangedEventHandler} or OnVideoDeviceStateChangedEventHandler{@link #EventHandler#OnVideoDeviceStateChangedEventHandler}.
     */
    public enum MediaDeviceState {
        /** {zh}
         * @brief 设备已开启
         */
        /** {en}
         * @brief On
         */
        kMediaDeviceStateStarted = 1,
        /** {zh}
         * @brief 设备已停止
         */
        /** {en}
         * @brief Off
         */
        kMediaDeviceStateStopped = 2,
        /** {zh}
         * @brief 设备运行时错误<br>
         *       例如，当媒体设备的预期行为是正常采集，但没有收到采集数据时，将回调该状态。
         */
        /** {en}
         * @brief Runtime error<br>
         *        For example, when the media device is expected to be working but no data is received.
         */
        kMediaDeviceStateRuntimeError = 3,
        /** {zh}
         * @brief 设备已暂停。包括：
         *        + 采集过程中，目标应用窗体最小化到任务栏。
         *        + 开启采集或采集过程中，目标应用窗体被隐藏。
         *        + 采集过程中，目标应用窗体正在被拉伸。
         *        + 采集过程中，目标应用窗体正在被拖动。
         */
        /** {en}
         * @brief Device paused. Including: <br>
         *         + During screen capturing, the target application window is minimized in the taskbar. <br>
         *         + Before or during screen capturing, the target application window is hidden. <br>
         *         + During screen capturing, the target application window is being stretched. <br>
         *         + During screen capturing, the target application window is being dragged. <br>
         */
        kMediaDeviceStatePaused = 4,
        /** {zh}
         * @brief 设备已恢复
         */
        /** {en}
         * @brief Device resumed
         */
        kMediaDeviceStateResumed = 5,
        /** {zh}
         * @brief 设备已插入
         */
        /** {en}
         * @brief Added
         */
        kMediaDeviceStateAdded = 10,
        /** {zh}
         * @brief 设备被移除
         */
        /** {en}
         * @brief Removed
         */
        kMediaDeviceStateRemoved = 11,
        /** {zh}
         * @brief 用户合盖打断了视频通话。如果系统未休眠或关机，将在开盖后自动恢复视频通话。
         */
        /** {en}
         * @brief Closing the laptop interrupted the RTC call. RTC call will resume once the laptop is opened.
         */
        kMediaDeviceInterruptionBegan = 12,
        /** {zh}
         * @brief 视频通话已从合盖打断中恢复
         */
        /** {en}
         * @brief RTC call resumed from the interruption caused by Closing the laptop.
         */
        kMediaDeviceInterruptionEnded = 13
    };

    /** {zh}
     * @type keytype
     * @brief 媒体设备错误类型
     */
    /** {en}
     * @type keytype
     * @brief  Media device error type
     */
    public enum MediaDeviceError {
        /** {zh}
         * @brief 媒体设备正常
         */
        /** {en}
         * @brief Media equipment is normal
         */
        kMediaDeviceErrorOK = 0,
        /** {zh}
         * @brief 没有权限启动媒体设备
         */
        /** {en}
         * @brief No permission to start media device
         */
        kMediaDeviceErrorDeviceNoPermission = 1,
        /** {zh}
         * @brief 媒体设备已经在使用中
         */
        /** {en}
         * @brief Media devices are already in use
         */
        kMediaDeviceErrorDeviceBusy = 2,
        /** {zh}
         * @brief 媒体设备错误
         */
        /** {en}
         * @brief Media device error
         */
        kMediaDeviceErrorDeviceFailure = 3,
        /** {zh}
         * @brief 未找到指定的媒体设备
         */
        /** {en}
         * @brief The specified media device was not found
         */
        kMediaDeviceErrorDeviceNotFound = 4,
        /** {zh}
         * @brief 媒体设备被移除
         *       对象为采集屏幕流时，表明窗体被关闭或显示器被移除。
         */
        /** {en}
         * @brief Media device, window or monitor removed
         */
        kMediaDeviceErrorDeviceDisconnected = 5,
        /** {zh}
         * @brief 设备没有数据回调
         */
        /** {en}
         * @brief Device has no data callback
         */
        kMediaDeviceErrorDeviceNoCallback = 6,
        /** {zh}
         * @brief 设备采样率不支持
         */
        /** {en}
         * @brief Device sample rate not supported
         */
        kMediaDeviceErrorDeviceUNSupportFormat = 7,
        /** {zh}
         * @brief iOS 屏幕采集没有 group Id 参数
         */
        /** {en}
         * @brief iOS screen capture not find group Id parameter
         */
        kMediaDeviceErrorDeviceNotFindGroupId = 8,
    };

    /** {zh}
     * @type keytype
     * @brief 媒体设备警告
     */
    /** {en}
     * @type keytype
     * @brief Media device warning
     */
    public enum MediaDeviceWarning {
        /** {zh}
         * @brief 无警告
         */
        /** {en}
         * @brief No warning
         */
        kMediaDeviceWarningOK = 0,
        /** {zh}
         * @brief 非法设备操作。在使用外部设备时，调用了 SDK 内部设备 API。
         */
        /** {en}
         * @brief Illegal device operation. Calls the API for internal device when using the external device.
         */
        kMediaDeviceWarningOperationDenied = 1,
        /** {zh}
         * @brief 采集静音。
         */
        /** {en}
         * @brief No audio is captured.
         */
        kMediaDeviceWarningCaptureSilence = 2,
        /** {zh}
         * @brief Android 特有的静音，系统层面的静音上报
         */
        /** {en}
         * @brief Silence warning by Android system.
         */
        kMediaDeviceWarningAndroidSysSilence = 3,
        /** {zh}
         * @brief Android 特有的静音消失
         */
        /** {en}
         * @brief Silence disappearing warning by Android system.
         */
        kMediaDeviceWarningAndroidSysSilenceDisappear = 4,
        // The following warning codes are only valid for meeting scenarios.
        /** {zh}
         * @hidden
         * @brief 音量过大，超过设备采集范围。建议降低麦克风音量或者降低声源音量。
         */
        /** {en}
         * @hidden
         * @brief The volume is too loud and exceeds the acquisition range of the device. Lower the microphone volume or lower the volume of the audio source.
         */
        kMediaDeviceWarningDetectClipping = 10,
        /** {zh}
         * @hidden
         * @brief 回声泄露
         */
        /** {en}
         * @hidden
         * @brief Leaking echo detected.
         */
        kMediaDeviceWarningDetectLeakEcho = 11,
        /** {zh}
         * @hidden
         * @brief 低信噪比
         */
        /** {en}
         * @hidden
         * @brief Low SNR.
         */
        kMediaDeviceWarningDetectLowSNR = 12,
        /** {zh}
         * @hidden
         * @brief 采集插零现象
         */
        /** {en}
         * @hidden
         * @brief Silence inserted during capture.
         */
        kMediaDeviceWarningDetectInsertSilence = 13,
        /** {zh}
         * @hidden
         * @brief 设备采集静音（算法层）
         */
        /** {en}
         * @hidden
         * @brief Silence during capture.
         */
        kMediaDeviceWarningCaptureDetectSilence = 14,
        /** {zh}
         * @hidden
         * @brief 设备采集静音消失
         */
        /** {en}
         * @hidden
         * @brief Silence disappears during capture.
         */
        kMediaDeviceWarningCaptureDetectSilenceDisappear = 15,
        /** {zh}
         * @hidden
         * @brief 啸叫
         */
        /** {en}
         * @hidden
         * @brief Howling
         */
        kMediaDeviceWarningCaptureDetectHowling = 16,
    };

    /** {zh}
     * @type keytype
     * @brief 录音配置
     */
    /**
     * {en}
     * @type keytype
     * @brief Audio recording config
     */
    public enum AudioRecordingState {
        /** {zh}
         * @brief 录制异常
         */
        /** {en}
         * @brief Recording exception
         */
        kAudioRecordingStateError = 0,
        /** {zh}
         * @brief 录制进行中
         */
        /** {en}
         * @brief Recording in progress
         */
        kAudioRecordingStateProcessing = 1,
        /** {zh}
         * @brief 已结束录制，并且录制文件保存成功。
         */
        /** {en}
         * @brief The recording task ends, and the file is saved.
         */
        kAudioRecordingStateSuccess = 2,
    };

    /** {zh}
     * @type errorcode
     * @brief 音频文件录制的错误码
     */
    /** {en}
     * @type errorcode
     * @brief Error code for audio recording.
     */
    public enum AudioRecordingErrorCode {
        /** {zh}
         * @brief 录制正常
         */
        /** {en}
         * @brief OK
         */
        kAudioRecordingErrorCodeOk = 0,
        /** {zh}
         * @brief 没有文件写权限
         */
        /** {en}
         * @brief No file write permissions.
         */
        kAudioRecordingErrorCodeNoPermission = -1,
        /** {zh}
         * @brief 没有进入房间
         */
        /** {en}
         * @brief Not in the room.
         */
        kAudioRecordingErrorNotInRoom = -2,
        /** {zh}
         * @brief 录制已经开始
         */
        /** {en}
         * @brief Started.
         */
        kAudioRecordingAlreadyStarted = -3,
        /** {zh}
         * @brief 录制还未开始
         */
        /** {en}
         * @brief Not started.
         */
        kAudioRecordingNotStarted = -4,
        /** {zh}
         * @brief 录制失败。文件格式不支持。
         */
        /** {en}
         * @brief Failure. Invalid file format.
         */
        kAudioRecordingErrorCodeNotSupport = -5,
        /** {zh}
         * @brief 其他异常
         */
        /** {en}
         * @brief Other error.
         */
        kAudioRecordingErrorCodeOther = -6,
    };

    /** {zh}
     * @type keytype
     * @brief CPU 和内存信息。
     */
    /** {en}
     * @type keytype
     * @brief System statistics.
     */
    public struct SysStats {
        /** {zh}
         * @brief CPU 核数。
         */
        /** {en}
         * @brief  Number of CPU cores.
         */
        public uint CpuCores;
        /** {zh}
         * @brief 当前应用的 CPU 使用率 (%)
         */
        /** {en}
         * @brief Current application CPU usage (%)
         */
        public double CpuAppUsage;
        /** {zh}
         * @brief 当前系统的 CPU 使用率 (%)
         */
        /** {en}
         * @brief Current system CPU usage (%)
         */
        public double CpuTotalUsage;
        /** {zh}
         * @brief 当前 App 的内存使用（单位 MB）
         */
        /** {en}
         * @brief Memory usage of the current app (in MB)
         */
        public double MemoryUsage;
        /** {zh}
         * @brief 全量内存（单位 MB）
         */
        /** {en}
         * @brief Full memory (in MB)
         */
        public ulong FullMemory;
        /** {zh}
         * @brief 系统已使用内存（单位 MB）
         */
        /** {en}
         * @brief System used memory (in MB)
         */
        public ulong TotalMemoryUsage;
        /** {zh}
         * @brief 空闲可分配内存（单位 MB）
         */
        /** {en}
         * @brief Free allocable memory (in MB)
         */
        public ulong FreeMemory;
        /** {zh}
         * @brief 当前应用的内存使用率（单位 %）
         */
        /** {en}
         * @brief Memory usage of the current application (in %)
         */
        public double MemoryRatio;
        /** {zh}
         * @brief 系统内存使用率（单位 %）
         */
        /** {en}
         * @brief System memory usage (in %)
         */
        public double TotalMemoryRatio;
    };

    /** {zh}
     * @type keytype
     * @brief 音频属性信息。
     */
    /** {en}
     * @type keytype
     * @brief Audio properties.
     */
    public struct AudioPropertiesInfo {
        /** {zh}
         * @brief 线性音量，与原始音量呈线性关系，数值越大，音量越大。取值范围是：[0,255]。<br>
         *        - [0, 25]: 无声 <br>
         *        - [26, 75]: 低音量 <br>
         *        - [76, 204]: 中音量 <br>
         *        - [205, 255]: 高音量 <br>
         */
        /** {en}
         * @brief linear volume. The value is in linear relation to the original volume. The higher the value, the higher the volume. The range is [0,255]. <br>
         *        - [0, 25]: Silence <br>
         *        - [26, 75]: Low volume <br>
         *        - [76, 204]: Medium volume <br>
         *        - [205, 255]: High volume <br>
         */
        public int LinearVolume;
        /** {zh}
         * @brief 非线性音量。由原始音量的对数值转化而来，因此在中低音量时更灵敏，可以用作 Active Speaker（房间内最活跃用户）的识别。取值范围是：[-127，0]，单位 dB。 <br>
         *        - [-127, -60]: 无声 <br>
         *        - [-59, -40]: 低音量 <br>
         *        - [-39, -20]: 中音量 <br>
         *        - [-19, 0]: 高音量 <br>
         */
        /** {en}
         * @brief non-linear volume in dB. The value is in proportion to the log value of the original volume. You can use the value to recognize the Active Speaker in the room. The range is [-127, 0]. <br>
         *        - [-127, -60]: Silence <br>
         *        - [-59, -40]: Low volume <br>
         *        - [-39, -20]: Medium volume <br>
         *        - [-19, 0]: High volume <br>
         */
        public int NonlinearVolume;
        /** {zh}
         * @brief 频谱数组
         */
        /** {en}
         * @brief Spectrum array
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 257)]
        public float[] Spectrum;
        /** {zh}
         * @brief 人声检测（VAD）结果
         *        - 1: 检测到人声。<br>
         *        - 0: 未检测到人声。<br>
         *        - -1: 未开启 VAD。
         */
        /** {en}
         * @brief Voice Activity Detection (VAD) result
         *        + 1: Voice activity detected.<br>
         *        + 0: No voice activity detected.<br>
         *        + -1: VAD not activated.<br>
         */
        public int Vad;
    };

    /** {zh}
     * @type keytype
     * @brief 本地音频属性信息
     */
    /** {en}
     * @type keytype
     * @brief Local audio properties
     */
    public struct LocalAudioPropertiesInfo {
        /** {zh}
         * @brief 流属性，主流或屏幕流。参看 StreamIndex{@link #StreamIndex}。
         */
        /** {en}
         * @brief See StreamIndex{@link #StreamIndex}.
         */
        public StreamIndex StreamIndex;
        /** {zh}
         * @brief 音频属性信息，参看 AudioPropertiesInfo{@link #AudioPropertiesInfo}。
         */
        /** {en}
         * @brief See AudioPropertiesInfo{@link #AudioPropertiesInfo}.
         */
        public AudioPropertiesInfo AudioPropertiesInfo;
    };

    /** {zh}
     * @type keytype
     * @brief 远端音频属性信息
     */
    /** {en}
     * @type keytype
     * @brief Remote audio properties
     */
    public struct RemoteAudioPropertiesInfo {
        /** {zh}
         * @brief 远端流信息，详见 RemoteStreamKey{@link #RemoteStreamKey}
         */
        /** {en}
         * @brief Remote stream information. See RemoteStreamKey{@link #RemoteStreamKey}.
         */
        public RemoteStreamKey StreamKey;
        /** {zh}
         * @brief 音频属性信息，详见 AudioPropertiesInfo{@link #AudioPropertiesInfo}
         */
        /** {en}
         * @brief Information of audio properties. See AudioPropertiesInfo{@link #AudioPropertiesInfo}.
         */
        public AudioPropertiesInfo AudioPropertiesInfo;
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧颜色编码格式
     */
    /** {en}
     * @type keytype
     * @brief Encoding format for video frame color
     */
    public enum VideoPixelFormat {
        /** {zh}
        * @brief 未知的颜色编码格式
        */
        /** {en}
        * @brief Unknown format
        */
        kVideoPixelFormatUnknown = 0,
        /** {zh}
        * @brief YUV I420 格式
        */
        /** {en}
        * @brief YUV I420
        */
        kVideoPixelFormatI420,
        /** {zh}
        * @brief YUV NV12 格式
        */
        /** {en}
        * @brief YUV NV12
        */
        kVideoPixelFormatNV12,
        /** {zh}
        * @brief YUV NV21 格式
        */
        /** {en}
        * @brief YUV NV21
        */
        kVideoPixelFormatNV21,
        /** {zh}
        * @brief RGB 24bit格式，
        */
        /** {en}
        * @brief RGB 24bit
        */
        kVideoPixelFormatRGB24,
        /** {zh}
        * @brief RGBA 编码格式
        */
        /** {en}
        * @brief RGBA
        */
        kVideoPixelFormatRGBA,
        /** {zh}
        * @brief ARGB 编码格式
        */
        /** {en}
        * @brief ARGB
        */
        kVideoPixelFormatARGB,
        /** {zh}
        * @brief BGRA 编码格式
        */
        /** {en}
        * @brief BGRA
        */
        kVideoPixelFormatBGRA,
        /** {zh}
        * @brief Texture2D格式
        */
        /** {en}
        * @brief Texture2D
        */
        kVideoPixelFormatTexture2D = 0x0DE1,
        /** {zh}
        * @brief TextureOES格式
        */
        /** {en}
        * @brief TextureOES
        */
        kVideoPixelFormatTextureOES = 0x8D65,
    };

    /** {zh}
     * @type keytype
     * @brief 视频内容类型
     */
    /** {en}
     * @type keytype
     * @brief Video content type
     */
    public enum VideoContentType {
        /** {zh}
         * @brief 普通视频
         */
        /** {en}
         * @brief Normal video
         */
        kVideoContentTypeNormalFrame = 0,
        /** {zh}
         * @brief 黑帧视频
         */
        /** {en}
         * @brief Black frame video
         */
        kVideoContentTypeBlackFrame,
    };

    /** {zh}
     * @hidden for internal use only on Windows and Android
     * @type keytype
     * @brief 视野范围（Fov）内的视频帧信息<br>
     *        Tile 是 全景视频的基本单位。<br>
     *        视野范围内的视频又分为高清视野和低清背景，均包含了多个 Tile。<br>
     *        视频帧信息为发送端使用 `setVideoEncoderConfig(const VideoEncoderConfig& encoderConfig, const char* parameters)` 接口进行设置。
     */
    /** {en}
     * @hidden for internal use only on Windows and Android
     * @type keytype
     * @brief Information of video frames within the FoV (Field of View). <br>
     *        Tile is the unit of a video within Fov.<br>
     *        A video within Fov includes HD view and LD background each of which consists of multiple Tiles.<br>
     *        The information of the video frames within the Fov is set by calling `setVideoEncoderConfig(const VideoEncoderConfig& encoderConfig, const char* parameters)` on the sender side.
     */
    public class FovVideoTileInfo
    {
        /** {zh}
         * @brief 高清视野宽度
         */
        /** {en}
         * @brief Width of the HD view.
         */
        public int hd_width = 0;
        /** {zh}
         * @brief 高清视野高度
         */
        /** {en}
         * @brief Height of the HD view
         */
        public int hd_height = 0;
        /** {zh}
         * @brief 低清背景宽度
         */
        /** {en}
         * @brief Width of the LD background
         */
        public int ld_width = 0;
        /** {zh}
         * @brief 低清背景高度
         */
        /** {en}
         * @brief Height of the LD background
         */
        public int ld_height = 0;
        /** {zh}
         * @brief Tile 宽度
         */
        /** {en}
         * @brief Width of a Tile
         */
        public int tile_width = 0;
        /** {zh}
         * @brief Tile 高度
         */
        /** {en}
         * @brief Height of a Tile
         */
        public int tile_height = 0;
        /** {zh}
         * @brief 高清视野中的 Tile 行数
         */
        /** {en}
         * @brief Number of Tile rows in the HD view
         */
        public int hd_row = 0;
        /** {zh}
         * @brief 高清视野中的 Tile 列数
         */
        /** {en}
         * @brief Number of Tile columns in the HD view
         */
        public int hd_column = 0;
        /** {zh}
         * @brief 低清背景中的 Tile 行数
         */
        /** {en}
         * @brief Number of Tile rows in the LD background
         */
        public int ld_row = 0;
        /** {zh}
         * @brief 低清背景中的 Tile 列数
         */
        /** {en}
         * @brief Number of Tile columns in the LD background
         */
        public int ld_column = 0;
        /** {zh}
         * @brief 视野范围中的 Tile 行数
         */
        /** {en}
         * @brief Number of tile rows within the FoV
         */
        public int dest_row = 0;
        /** {zh}
         * @brief 视野范围中的 Tile 列数
         */
        /** {en}
         * @brief Number of tile columns within the FoV
         */
        public int dest_column = 0;
        /** {zh}
         * @brief Tile 位置映射表
         */
        /** {en}
         * @brief Position map of the Tiles
         */
        public IntPtr tile_map;
        /** {zh}
         * @brief Tile 数量
         */
        /** {en}
         * @brief Number of the Tiles
         */
        public int tile_size = 0;
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧编码格式
     */
    /** {en}
     * @type keytype
     * @brief Video frame encoding format
     */
    public enum VideoSinkPixelFormat {
        /** {zh}
         * @brief YUV I420 格式
        */
        /** {en}
         * @brief YUV I420
        */
        kI420 = VideoPixelFormat.kVideoPixelFormatI420,
        /** {zh}
         * @brief 原始视频帧格式
         */
        /** {en}
         * @brief Original format
         */
        kOriginal = VideoPixelFormat.kVideoPixelFormatUnknown,
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧格式
     */
    /** {en}
     * @type keytype
     * @brief Video frame format
     */
    public enum VideoFrameType {
        /** {zh}
        * @brief 原始数据格式，按照内存存储
        */
        /** {en}
        * @brief Original format, stored as memory
        */
        kVideoFrameTypeRawMemory,
        /** {zh}
        * @brief CVPixelBufferRef类型，支持 iOS and macOS 平台
        */
        /** {en}
        * @brief CVPixelBufferRef which applys to iOS and macOS
        */
        kVideoFrameTypeCVPixelBuffer,
        /** {zh}
        * @brief open gl 纹理数据类型
        */
        /** {en}
        * @brief Open gl texture
        */
        kVideoFrameTypeGLTexture,
        /** {zh}
        * @brief cuda 数据类型
        */
        /** {en}
        * @brief Cuda
        */
        kVideoFrameTypeCuda,
        /** {zh}
        * @brief direct3d 11 数据格式
        */
        /** {en}
        * @brief Direct3d 11
        */
        kVideoFrameTypeD3D11,
        /** {zh}
        * @brief direct3d 9 数据格式
        */
        /** {en}
        * @brief Direct3d 9
        */
        kVideoFrameTypeD3D9,
        /** {zh}
        * @brief Java的VideoFrame 数据格式
        */
        /** {en}
        * @brief VideoFrame used in Java
        */
        kVideoFrameTypeJavaFrame,
        /** {zh}
        * @brief vaapi 数据格式
        */
        /** {en}
        * @brief Vaapi
        */
        kVideoFrameTypeVAAPI,
    };

    /** {zh}
     * @type keytype
     * @brief 视频 YUV 格式颜色空间
     */
    /** {en}
     * @type keytype
     * @brief  Video YUV format color space
     */
    public enum ColorSpace {
        /** {zh}
        * 未知的颜色空间，默认使用 kColorSpaceYCbCrBT601LimitedRange 颜色空间
        */
        /** {en}
        * Unknown color space, default kColorSpaceYCbCrBT601LimitedRange color space
        */
        kColorSpaceUnknown = 0,
        /** {zh}
        * @brief BT.601数字编码标准，颜色空间[16-235]
        */
        /** {en}
        * @brief BT.601 Digital Coding Standard, Color Space [16-235]
        */
        kColorSpaceYCbCrBT601LimitedRange,
        /** {zh}
        *  @brief BT.601数字编码标准，颜色空间[0-255]
        */
        /** {en}
        *  @brief BT.601 Digital Coding Standard, Color Space [0-255]
        */
        kColorSpaceYCbCrBT601FullRange,
        /** {zh}
        * @brief BT.7091数字编码标准，颜色空间[16-235]
        */
        /** {en}
        * @brief BT.7091 Digital Coding Standard, Color Space [16-235]
        */
        kColorSpaceYCbCrBT709LimitedRange,
        /** {zh}
        * @brief BT.7091数字编码标准，颜色空间[0-255]
        */
        /** {en}
        * @brief BT.7091 Digital Coding Standard, Color Space [0-255]
        */
        kColorSpaceYCbCrBT709FullRange,
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧
     */
    /** {en}
     * @type keytype
     * @brief Video frame structure
     */
    public struct VideoFrame {
        /** {zh}
         * @brief 视频帧类型，参看 VideoFrameType{@link #VideoFrameType}。
         */
        /** {en}
         * @brief  Video frame type. See VideoFrameType{@link #VideoFrameType}.
         */
        public VideoFrameType FrameType;
        /** {zh}
         * @brief 视频帧格式，参看 VideoPixelFormat{@link #VideoPixelFormat}。
         */
        /** {en}
         * @brief Video frame format. See VideoPixelFormat{@link #VideoPixelFormat}.
         */
        public VideoPixelFormat PixelFormat;
        /** {zh}
         * @brief 视频帧时间戳，单位：微秒
         */
        /** {en}
         * @brief Video frame timestamp in microseconds
         */
        public long TimestampUs;
        /** {zh}
         * @brief 视频帧宽度，单位：px
         */
        /** {en}
         * @type api
         * @brief Video frame width in px
         */
        public int Width;
        /** {zh}
         * @brief 视频帧高度，单位：px
         */
        /** {en}
         * @type api
         * @brief Video frame height in px
         */
        public int Height;
        /** {zh}
         * @brief 视频帧旋转角度
         */
        /** {en}
         * @brief Video frame rotation angle
         */
        public VideoRotation Rotation;
        /** {zh}
         * @brief 视频帧色彩空间，参看 ColorSpace{@link #ColorSpace}
         */
        /** {en}
         * @brief The color space of video frame, see ColorSpace{@link #ColorSpace}
         */
        public ColorSpace ColorSpace;
        /** {zh}
         * @brief 获取视频帧像素平面数量
         *        yuv 数据存储格式分为打包（planar）存储格式和平面（packed）存储格式，planar 格式中 Y、U、V 分平面存储，packed 格式中 Y、U、V 交叉存储
         */
        /** {en}
         * @brief Gets Video frame color plane number
         *        YUV formats are categorized into planar format and packed format.  <br>
         *        In a planar format, the Y, U, and V components are stored separately as three planes, while in a packed format, the Y, U, and V components are stored in a single array.
         */
        public int NumberOfPlanes;
        /** {zh}
         * @brief plane 数据
         */
        /** {en}
         * @brief Plane data pointer
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[][] PlaneData;
        /** {zh}
         * @brief plane 中数据行的长度
         */
        /** {en}
         * @brief The length of the data line in the specified plane
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] PlaneLineSize;
        /** {zh}
         * @brief 扩展数据指针
         */
        /** {en}
         * @brief Extended data pointer
         */
        public byte[] ExtraDataInfo;
        /** {zh}
         * @brief 扩展数据字节数
         */
        /** {en}
         * @brief Size of extended data in bytes
         */
        public int ExtraDataInfoSize;
        /** {zh}
         * @brief 补充数据指针
         */
        /** {en}
         * @brief Supplementary data
         */
        public byte[] SupplementaryInfo;
        /** {zh}
         * @brief 补充数据字节数
         */
        /** {en}
         * @brief Size of supplementary data in bytes
         */
        public int SupplementaryInfoSize;
        /** {zh}
         * @brief 本地缓冲区指针
         */
        /** {en}
         * @brief Local buffer pointer
         */
        public IntPtr HwaccelBuffer ;
        /** {zh}
         * @brief 硬件加速 Context 对象(AKA Opengl Context, Vulkan Context)
         */
        /** {en}
         * @brief Hardware accelerate context(AKA Opengl Context, Vulkan Context)
         */
        public IntPtr HwaccelContext;
#if UNITY_ANDROID
        /** {zh}
         * @brief 硬件加速 Context 的 Java 对象(Only for Android, AKA Opengl Context)
         */
        /** {en}
         * @brief Hardware accelerate context's java object(Only for Android, AKA Opengl Context)
         */
        public IntPtr AndroidHwaccelContext;
#endif
        /** {zh}
         * @brief 纹理矩阵(仅针对纹理类型的 frame 生效)
         */
        /** {en}
         * @brief Get Texture matrix (only for texture type frame)
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] Matrix;
        /** {zh}
         * @brief 获取纹理 ID(仅针对纹理类型的 frame 生效)
         */
        /** {en}
         * @brief Get Texture ID (only for texture type frame)
         */
        public int   TextureId;

    };

    /** {zh}
     * @type keytype
     * @brief 屏幕采集对象的类型
     */
    /** {en}
     * @type keytype
     * @brief Type of the screen capture object
     */
    public enum ScreenCaptureSourceType {
        /** {zh}
         * @brief 类型未知
         */
        /** {en}
         * @brief Type unknown
         */
        kScreenCaptureSourceTypeUnknown,
        /** {zh}
         * @brief 应用程序的窗口
         */
        /** {en}
         * @brief Application window
         */
        kScreenCaptureSourceTypeWindow,
        /** {zh}
         * @brief 桌面
         */
        /** {en}
         * @brief Desktop
         */
        kScreenCaptureSourceTypeScreen
    };

    /** {zh}
     * @type keytype
     * @brief 屏幕采集对象的具体信息
     */
    /** {en}
     * @type keytype
     * @brief Screen capture object specific information
     */
    public struct ScreenCaptureSourceInfo {
        /** {zh}
         * @brief 屏幕分享时，共享对象的类型，参看 ScreenCaptureSourceType{@link #ScreenCaptureSourceType}
         */
        /** {en}
         * @brief When sharing a screen. See ScreenCaptureSourceType{@link #ScreenCaptureSourceType} for the type of shared object
         */
        public ScreenCaptureSourceType type;
        /** {zh}
         * @brief 屏幕分享时，共享对象的 ID。
         */
        /** {en}
         * @brief ID of the screen-shared object
         */
        public IntPtr SourceID;
        /** {zh}
         * @brief 屏幕分享时共享对象的名称。
         *        内存在调用 Release{@link #IScreenCaptureSourceList#Release} 时释放，请及时转为 string 对象保存。
         */
        /** {en}
         * @brief Name of the screen-shared object<br>
         *        Save it as a `string` object before calling Release{@link #IScreenCaptureSourceList#Release} to release the dynamic memory.
         */
        public string SourceName;
        /** {zh}
         * @brief 共享对象的字符串长度。
         */
        /** {en}
         * @brief The length of `SourceName`.
         */
        public int length;
        /** {zh}
         * @brief 共享的应用窗口所属应用的名称。
         *        当共享对象为应用窗口时有效。
         *        调用 release{@link #IScreenCaptureSourceList#Release} 时将被释放，请及时转为 string 对象保存。
         */
        /** {en}
         * @brief The title of the application to be shared.
         *        Only available if the sharing object is an application windows.
         *        Save it as a `string` object before calling Release{@link #IScreenCaptureSourceList#Release} to release the dynamic memory.
         */
        public string application;
        /** {zh}
         * @brief 应用的字符串长度。
         */
        /** {en}
         * @brief The length of `application`.
         */
        public int AppLength;
        /** {zh}
         * @brief 共享的应用窗体所属应用进程的 pid。
         *        当共享对象为应用窗体时有效。
         */
        /** {en}
         * @brief Process pid of the application to be shared.
         *        Only available if the sharing object is an application windows.
         */
        public int pid;
        /** {zh}
         * @brief 共享的屏幕是否为主屏。
         *        当共享对象为屏幕时有效。
         */
        /** {en}
         * @brief Tag for the screen to be shared identifying whether it is the primary screen.
         *        Only available when the screen-sharing object is a screen.
         */
        public bool PrimaryMonitor;
        /** {zh}
         * @brief 屏幕共享对象的坐标。参看 Rectangle{@link #Rectangle}。
         *        + 对于多屏幕的场景，不同平台的坐标系原点不同：
         *          - 对于 Windows 平台，屏幕坐标以主屏左上角为原点 (0, 0)，向右向下扩展。
         *          — 对于 Linux 平台，屏幕坐标以 **恰好包住所有显示器的矩形区域的左上角** 为原点 (0, 0)，向右向下扩展。
         *        + 对于不同平台，窗口区域不同：
         *          - 对于 Windows 和 macOS 平台，窗口区域包含系统标题栏。
         *          - 对于 Linux 平台，窗口区域不包含系统标题栏。
         */
        /** {en}
         * @brief Coordinates of the screen-sharing object. See Rectangle{@link #Rectangle}.
         *       + When there are several screens, the origin (0, 0) is different for different platforms:
         *         - For Windows, the origin (0, 0) is the top-left corner of the main screen.
         *         - For Linux, the origin (0, 0) is the top-left corner of the rectangle that merely covers all screens.
         *       + The region of the window is different for different platforms:
         *         - For Windows and macOS, the region includes the system title bar of the window.
         *         - For Linux, the region does not includes the system title bar of the window.
         */
        public Rectangle RegionRect;
    };

    /** {zh}
     * @type keytype
     * @brief 内部采集屏幕视频流的内容类型。
     */
    /** {en}
     * @type keytype
     * @brief Content hints of the internally captured screen video stream.
     */
    public enum ContentHint {
        /** {zh}
        * @brief 细节内容。当共享文档、图片时，建议使用该内容类型。
        */
        /** {en}
        * @brief Detailed content. Recommended when you capture documents, images, etc.
        */
        kContentHintDetails = 0,
        /** {zh}
        * @brief 动画内容。当共享视频、游戏时，建议使用该内容类型。
        */
        /** {en}
        * @brief  Animation content. Recommended when you capture videos, games, etc.
        */
        kContentHintMotion,
    };

    /** {zh}
     * @type keytype
     * @brief 矩形区域
     */
    /** {en}
     * @type keytype
     * @brief Rectangle Area
     */
    public struct Rectangle {
        /** {zh}
         * @brief 矩形区域左上角的 x 坐标
         */
        /** {en}
         * @brief The x coordinate of the upper left corner of the rectangular area
         */
        public int X;
        /** {zh}
         * @brief 矩形区域左上角的 x 坐标
         */
        /** {en}
         * @brief The x coordinate of the upper left corner of the rectangular area
         */
        public int Y;
        /** {zh}
         * @brief 矩形宽度(px)
         */
        /** {en}
         * @brief Rectangle width in px
         */
        public int Width;
        /** {zh}
         * @brief 矩形高度(px)
         */
        /** {en}
         * @brief Rectangular height in px
         */
        public int Height;
    };

    /** {zh}
     * @type keytype
     * @brief 内部采集屏幕视频流时，是否采集鼠标信息
     */
    /** {en}
     * @type keytype
     * @brief Whether to collect mouse information when collecting screen video stream internally
     */
    public enum MouseCursorCaptureState {
        /** {zh}
        * @brief 采集鼠标信息
        */
        /** {en}
        * @brief Collect mouse information
        */
        kMouseCursorCaptureStateOn,
        /** {zh}
        * @brief 不采集鼠标信息
        */
        /** {en}
        * @brief Do not collect mouse information
        */
        kMouseCursorCaptureStateOff,
    };

    /** {zh}
     * @type keytype
     * @brief 抓取屏幕时排除指定窗口，默认不排除任何窗体
     */
    /** {en}
     * @type keytype
     * @brief  Excludes the specified window when grabbing the screen, no form is excluded by default
     */
    public struct ScreenFilterConfig {
        /** {zh}
         * @brief 抓取屏幕时排除窗口列表。这个参数仅在抓取屏幕时生效。
         */
        /** {en}
         * @brief Exclude the list of windows when grabbing the screen. This parameter only takes effect when the screen is captured.
         */
        public IntPtr[] ExcludedWindowList;
        /** {zh}
         * @brief 排除窗口的数量。
         */
        /** {en}
         * @brief Exclude the number of windows.
         */
        public int ExcludedWindowNum;
    };

    /** {zh}
     * @type keytype
     * @brief 屏幕共享时的边框高亮设置
     */
    /** {en}
     * @type keytype
     * @brief  Border highlighting settings for screen sharing
     */
    public struct HighlightConfig {
        /** {zh}
         * @brief 是否显示高亮边框，默认显示。
         */
        /** {en}
         * @brief Whether to display a highlighted border, the default display.
         */
        public bool EnableHighlight;
        /** {zh}
         * @brief 边框的颜色, 颜色格式为十六进制 ARGB:  0xAARRGGBB
         */
        /** {en}
         * @brief The color of the border,  the color format is hexadecimal ARGB: 0xAARRGGBB
         */
        public uint BorderColor;
        /** {zh}
         * @brief 边框的宽度，单位：像素
         */
        /** {en}
         * @brief The width of the border, in pixels
         */
        public int BorderWidth;
    };

    /** {zh}
     * @type keytype
     * @brief 屏幕共享内部采集参数
     */
    /** {en}
     * @type keytype
     * @brief  Screen internal capture parameters
     */
    public struct ScreenCaptureParameters {
        /** {zh}
         * @brief 内容类型，参看 ContentHint{@link #ContentHint}。
         */
        /** {en}
         * @brief Content hint. See ContentHint{@link #ContentHint}.
         */
        public ContentHint ContentHint;
        /** {zh}
         * @brief 采集区域，参看 Rectangle{@link #Rectangle}。
         */
        /** {en}
         * @brief Collection area. See Rectangle{@link #Rectangle}.
         */
        public Rectangle RegionRect;
        /** {zh}
         * @brief 是否采集鼠标状态，参看 MouseCursorCaptureState{@link #MouseCursorCaptureState}。
         */
        /** {en}
         * @brief To collect mouse status. See MouseCursorCaptureState{@link #MouseCursorCaptureState}.
         */
        public MouseCursorCaptureState CaptureMouseCursor;
        /** {zh}
         * @brief 屏幕过滤设置，参看 ScreenFilterConfig{@link #ScreenFilterConfig}。
         */
        /** {en}
         * @brief Screen filtering settings. See ScreenFilterConfig{@link #ScreenFilterConfig}.
         */
        public ScreenFilterConfig FilterConfig;
        /** {zh}
         * @brief 采集区域的边框高亮设置，参看 HighlightConfig{@link #HighlightConfig}。
         */
        /** {en}
         * @brief For the border highlighting settings of the acquisition area. See HighlightConfig{@link #HighlightConfig}.
         */
        public HighlightConfig HighlightConfig;
    };

    /** {zh}
     * @type keytype
     * @brief 音频输入源类型
     */
    /** {en}
     * @type keytype
     * @brief Audio input source type
     */
    public enum AudioSourceType {
        /** {zh}
        * @brief 自定义采集音频源
        */
        /** {en}
        * @brief Custom Capture Audio Source
        */
        kAudioSourceTypeExternal = 0,
        /** {zh}
        * @brief RTC SDK 内部采集音频源
        */
        /** {en}
        * @brief RTC SDK internal acquisition audio source
        */
        kAudioSourceTypeInternal,
    };

    /** {zh}
     * @type keytype
     * @brief 音频采样率，单位为 Hz。
     */
    /** {en}
     * @type keytype
     * @brief Audio sample rate in Hz.
     */
    public enum AudioSampleRate {
        /** {zh}
        * @brief 默认设置。
        */
        /** {en}
        * @brief Default value.
        */
        kAudioSampleRateAuto = -1,
        /** {zh}
        * @brief 8000 Hz
        */
        /** {en}
        * @brief 8000
        */
        kAudioSampleRate8000 = 8000,
        /** {zh}
        * @brief 16000 Hz
        */
        /** {en}
        * @brief 16000
        */
        kAudioSampleRate16000 = 16000,
        /** {zh}
        * @brief 32000 Hz
        */
        /** {en}
        * @brief 32000
        */
        kAudioSampleRate32000 = 32000,
        /** {zh}
        * @brief 44100 Hz
        */
        /** {en}
        * @brief 44100
        */
        kAudioSampleRate44100 = 44100,
        /** {zh}
        * @brief 48000 Hz
        */
        /** {en}
        * @brief 48000
        */
        kAudioSampleRate48000 = 48000
    };

    /** {zh}
     * @type keytype
     * @brief 音频声道。
     */
    /** {en}
     * @type keytype
     * @brief Audio channel
     */
    public enum AudioChannel {
        /** {zh}
        * @brief 默认设置。
        */
        /** {en}
        * @brief Default value.
        */
        kAudioChannelAuto = -1,
        /** {zh}
        * @brief 单声道
        */
        /** {en}
        * @brief Mono channel
        */
        kAudioChannelMono = 1,
        /** {zh}
        * @brief 双声道
        */
        /** {en}
        * @brief Dual channels
        */
        kAudioChannelStereo = 2
    };

    /** {zh}
     * @type keytype
     * @brief 音频帧类型
     */
    /** {en}
     * @type keytype
     * @brief Audio frame type
     */
    public enum AudioFrameType {
        /** {zh}
         * @brief PCM 16bit
         */
        /** {en}
         * @brief PCM 16bit
         */
        kFrameTypePCM16 = 0
    };

    /** {zh}
     * @type keytype
     * @brief 音频帧
     */
    /** {en}
     * @type keytype
     * @brief Audio frame
     */
    public struct AudioFrame {
        /** {zh}
         * @brief 音频帧时间戳。单位：微秒。
         */
        /** {en}
         * @brief Audio frame timestamp in microseconds.
         */
        public long TimestampUs;
        /** {zh}
         * @brief 音频采样率。参看 AudioSampleRate{@link #AudioSampleRate}
         */
        /** {en}
         * @brief Audio sample rate. See AudioSampleRate{@link #AudioSampleRate}
         */
        public AudioSampleRate SampleRate;
        /** {zh}
         * @brief 音频通道数。参看 AudioChannel{@link #AudioChannel}
         */
        /** {en}
         * @brief The number of audio channels. See AudioChannel{@link #AudioChannel}
         */
        public AudioChannel Channel;
        /** {zh}
         * @brief 音频帧内存块地址
         */
        /** {en}
         * @brief Audio frame memory address
         */
        public byte[] Data;
        /** {zh}
         * @brief 音频帧数据大小，单位：字节。
         */
        /** {en}
         * @brief Audio frame data size in bytes.
         */
        public int DataSize;
        /** {zh}
         * @brief 音频帧类型，目前只支持 PCM，详见 AudioFrameType{@link #AudioFrameType}
         */
        /** {en}
         * @brief Audio frame type, support PCM only. See AudioFrameType{@link #AudioFrameType}.
         */
        public AudioFrameType FrameType;
        /** {zh}
         * @brief 音频静音标志
         */
        /** {en}
         * @brief Gets audio mute state identifier
         */
        public bool IsMutedData;
    };

    /** {zh}
    * @type keytype
    * @brief 是否开启耳返功能。
    */
    /** {en}
     * @type keytype
     * @brief Whether or not in-ear monitoring is enabled.
     */
    public enum EarMonitorMode {
        /** {zh}
         * @brief 关闭（默认设置）
         */
        /** {en}
         * @brief Disabled (default)
         */
        kEarMonitorModeOff = 0,
        /** {zh}
         * @brief 开启
         */
        /** {en}
         * @brief Enabled
         */
        kEarMonitorModeOn = 1,
    };

    /** {zh}
 * @type keytype
 * @brief 混响特效类型。
 */
    /** {en}
     * @type keytype
     * @brief Reverb effect type.
     */
    public enum VoiceReverbType {
        /** {zh}
     * @brief 原声，不含特效
     */
        /** {en}
         * @brief Acoustic, no special effects
         */
        kVoiceReverbTypeOriginal = 0,
        /** {zh}
         * @brief 回声
         */
        /** {en}
         * @brief Echo
         */
        kVoiceReverbTypeEcho = 1,
        /** {zh}
         * @brief 演唱会
         */
        /** {en}
         * @brief Concert
         */
        kVoiceReverbTypeConcert = 2,
        /** {zh}
         * @brief 空灵
         */
        /** {en}
         * @brief Ethereal
         */
        kVoiceReverbTypeEthereal = 3,
        /** {zh}
         * @brief KTV
         */
        /** {en}
         * @brief Karaoke
         */
        kVoiceReverbTypeKTV = 4,
        /** {zh}
         * @brief 录音棚
         */
        /** {en}
         * @brief Recording studio
         */
        kVoiceReverbTypeStudio = 5,
        /** {zh}
         * @brief 虚拟立体声
         */
        /** {en}
         * @brief Virtual Stereo
         */
        kVoiceReverbTypeVirtualStereo = 6,
        /** {zh}
         * @brief 空旷
         */
        /** {en}
         * @brief Spacious
         */
        kVoiceReverbTypeSpacious = 7,
        /** {zh}
         * @brief 3D人声
         */
        /** {en}
         * @brief 3D vocal
         */
        kVoiceReverbType3D = 8,
        /** {zh}
         * @hidden for internal use only
         * @brief 流行
         */
        /** {en}
         * @hidden for internal use only
         * @brief Pop
         */
        kVoiceReverbTypePop = 9,
        /** {zh}
         * @hidden for internal use only
         * @brief 蹦迪
         */
        /** {en}
         * @hidden for internal use only
         * @brief Disco
         */
        kVoiceReverbTypeDisco = 10,
        /** {zh}
         * @hidden for internal use only
         * @brief 老唱片
         */
        /** {en}
         * @hidden for internal use only
         * @brief Old Record
         */
        kVoiceReverbTypeOldRecord = 11,
        /** {zh}
         * @hidden for internal use only
         * @brief 和声
         */
        /** {en}
         * @hidden for internal use only
         * @brief Harmony
         */
        kVoiceReverbTypeHarmony = 12,
        /** {zh}
         * @hidden for internal use only
         * @brief 摇滚
         */
        /** {en}
         * @hidden for internal use only
         * @brief Rock
         */
        kVoiceReverbTypeRock = 13,
        /** {zh}
         * @hidden for internal use only
         * @brief 蓝调
         */
        /** {en}
         * @hidden for internal use only
         * @brief Blues
         */
        kVoiceReverbTypeBlues = 14,
        /** {zh}
         * @hidden for internal use only
         * @brief 爵士
         */
        /** {en}
         * @hidden for internal use only
         * @brief Jazz
         */
        kVoiceReverbTypeJazz = 15,
        /** {zh}
         * @hidden for internal use only
         * @brief 电子
         */
        /** {en}
         * @hidden for internal use only
         * @brief Electronic
         */
        kVoiceReverbTypeElectronic = 16,
        /** {zh}
         * @hidden for internal use only
         * @brief 黑胶
         */
        /** {en}
         * @hidden for internal use only
         * @brief Vinyl
         */
        kVoiceReverbTypeVinyl = 17,
        /** {zh}
         * @hidden for internal use only
         * @brief 密室
         */
        /** {en}
         * @hidden for internal use only
         * @brief Chamber
         */
        kVoiceReverbTypeChamber = 18,
    };

    /** {zh}
     * @type keytype
     * @brief 屏幕采集媒体类型
     */
    /** {en}
     * @type keytype
     * @brief Screen media type
     */
    public enum ScreenMediaType {
        /** {zh}
         * @brief 仅采集视频
         */
        /** {en}
         * @brief Capture video only
         */
        kScreenMediaTypeVideoOnly = 0,
        /** {zh}
         * @brief 仅采集音频
         */
        /** {en}
         * @brief Capture audio only
         */
        kScreenMediaTypeAudioOnly = 1,
        /** {zh}
         * @brief 同时采集音频和视频
         */
        /** {en}
         * @brief Capture Audio and video simultaneously.
         */
        kScreenMediaTypeVideoAndAudio = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 音视频质量反馈问题类型
     */
    /** {en}
     * @type keytype
     * @brief  Audio & video quality feedback problem type
     */
    public enum ProblemFeedbackOption {
        /** {zh}
         * @brief 没有问题
         */
        /** {en}
         * @brief No problem
         */
        kProblemFeedbackOptionNone = 0,
        /** {zh}
         * @brief 其他问题
         */
        /** {en}
         * @brief Other issues
         */
        kProblemFeedbackOptionOtherMessage = (1 << 0),
        /** {zh}
         * @brief 声音不清晰
         */
        /** {en}
         * @brief Unclear voice
         */
        kProblemFeedbackOptionAudioNotClear = (1 << 1),
        /** {zh}
         * @brief 视频不清晰
         */
        /** {en}
         * @brief Video is not clear
         */
        kProblemFeedbackOptionVideoNotClear = (1 << 2),
        /** {zh}
         * @brief 音视频不同步
         */
        /** {en}
         * @brief Audio & video out of sync
         */
        kProblemFeedbackOptionNotSync = (1 << 3),
        /** {zh}
         * @brief 音频卡顿
         */
        /** {en}
         * @brief Audio card
         */
        kProblemFeedbackOptionAudioLagging = (1 << 4),
        /** {zh}
         * @brief 视频卡顿
         */
        /** {en}
         * @brief Video card
         */
        kProblemFeedbackOptionVideoLagging = (1 << 5),
        /** {zh}
         * @brief 连接失败
         */
        /** {en}
         * @brief Connection failed
         */
        kProblemFeedbackOptionDisconnected = (1 << 6),
        /** {zh}
         * @brief 无声音
         */
        /** {en}
         * @brief No sound
         */
        kProblemFeedbackOptionNoAudio = (1 << 7),
        /** {zh}
         * @brief 无画面
         */
        /** {en}
         * @brief No picture
         */
        kProblemFeedbackOptionNoVideo = (1 << 8),
        /** {zh}
         * @brief 声音过小
         */
        /** {en}
         * @brief Too little sound
         */
        kProblemFeedbackOptionAudioStrength = (1 << 9),
        /** {zh}
         * @brief 回声噪音
         */
        /** {en}
         * @brief Echo noise
         */
        kProblemFeedbackOptionEcho = (1 << 10),
        /** {zh}
         * @brief 耳返延迟大
         */
        /** {en}
         * @brief Large delay in earphone monitoring
         */
        KFeedbackOptionEarBackDelay = (1 << 11),
    };

    /** {zh}
    * @type keytype
    * @brief 音频数据
    */
    /** {en}
    * @type keytype
    * @brief  Audio data
    */
    public struct AudioFrameCallback {
        /** {zh} 
         * @brief 音频帧时间戳。单位：微秒。
         */
        /** {en} 
         * @brief Audio frame timestamp in microseconds.
         */
        public long TimestampUs;
        /** {zh}
         * @brief 音频采样率。参看 AudioSampleRate{@link #AudioSampleRate}
         */
        /** {en}
         * @brief Audio sample rate. See AudioSampleRate{@link #AudioSampleRate}
         */
        public AudioSampleRate SampleRate;
        /** {zh} 
         * @brief 音频通道数。参看 AudioChannel{@link #AudioChannel}
         */
        /** {en} 
         * @brief The number of audio channels. See AudioChannel{@link #AudioChannel}
         */
        public AudioChannel Channel;
        /** {zh} 
         * @brief 音频帧内存块地址
         */
        /** {en} 
         * @brief Audio frame memory address
         */
        public IntPtr Data;
        /** {zh} 
         * @brief 音频帧数据大小，单位：字节。
         */
        /** {en} 
         * @brief Audio frame data size in bytes.
         */
        public int DataSize;
        /** {zh} 
         * @brief 音频帧类型，目前只支持 PCM，详见 AudioFrameType{@link #AudioFrameType}
         */
        /** {en} 
         * @type api
         * @region Audio Management
         * @brief Audio frame type, support PCM only. See AudioFrameType{@link #AudioFrameType}.
         * @return Audio frame type, support PCM only. See AudioFrameType{@link #AudioFrameType}
         */
        public AudioFrameType FrameType;
        /** {zh} 
         * @brief 音频静音标志
         */
        /** {en} 
         * @brief Gets audio mute state identifier
         */
        public bool IsMutedData;
    };

    /** {zh}
     * @type keytype
     * @brief 摄像头。
     */
    /** {en}
     * @type keytype
     * @brief  camera.
     */
    public enum CameraID {
        /** {zh}
         * @brief 移动端前置摄像头，PC端内置摄像头
         */
        /** {en}
         * @brief Front-facing camera for mobile, build-in camera for PC
         */
        kCameraIDFront = 0,
        /** {zh}
         * @brief 移动端后置摄像头，PC端无定义
         */
        /** {en}
         * @brief Postconditioning camera for mobile, PC is undefined for camera 1
         */
        kCameraIDBack = 1,
        /** {zh}
         * @brief 外接摄像头
         */
        /** {en}
         * @brief External camera
         */
        kCameraIDExternal = 2,
        /** {zh}
         * @brief 无效值
         */
        /** {en}
         * @brief Invalid value
         */
        kCameraIDInvalid = 3
    };
    /** {zh}
 * @type keytype
 * @brief 音视频质量反馈的房间信息
 */
    /** {en}
     * @type keytype
     * @brief Room information for audio & video quality feedback
     */
   public struct ProblemFeedbackRoomInfo {
        /** {zh}
         * @brief 房间 ID。
         */
        /** {en}
         * @brief Room ID.
         */
        public string  room_id;

        /** {zh}
         * @brief 本地用户 ID。
         */
        /** {en}
         * @brief The ID of the  local user.
         */
        public string user_id;
    };

    /** {zh}
 * @type keytype
 * @brief 音视频质量反馈的信息
 */
    /** {en}
     * @type keytype
     * @brief Information for audio & video quality feedback
     */
    public struct ProblemFeedbackInfo {
        /** {zh}
         * @brief 预设问题以外的其他问题的具体描述。
         */
        /** {en}
         * @brief Specific description of problems other than the preset problem.
         */
        public string problem_desc;

        /** {zh}
         * @type keytype
         * @brief 音视频质量反馈的房间信息。查看 ProblemFeedbackRoomInfo{@link #ProblemFeedbackRoomInfo}。
         */
        /** {en}
         * @type keytype
         * @brief Room information for audio & video quality feedback. See ProblemFeedbackRoomInfo{@link #ProblemFeedbackRoomInfo}.
         */
        public IntPtr room_info;
        /** {zh}
         * @type keytype
         * @brief `FeedbackRoomInfo` 的数组长度。
         */
        /** {en}
         * @type keytype
         * @brief The length of `FeedbackRoomInfo`.
         */
        public  int room_info_count;
    };
    public enum PublicStreamErrorCode
    {
        kPublicStreamErrorCodeOK = 0,
        kPublicStreamErrorCodePushInvalidParam = 1191,
        kPublicStreamErrorCodePushInvalidStatus = 1192,
        kPublicStreamErrorCodePushInternalError = 1193,
        kPublicStreamErrorCodePushFailed = 1195,
        kPublicStreamErrorCodePushTimeout = 1196,
        kPublicStreamErrorCodePullNoPushStream = 1300,
    };
    public struct FaceDetectResult
    {
        /** {zh}
         * @brief 人脸信息存储上限，最多可存储 10 个人脸信息
         */
        /** {en}
         * @brief Face information storage limit, up to 10 faces can be stored.
         */
        public const  int max_face_num = 10;
        /** {zh}
         * @brief 人脸检测结果 <br>
         *        + 0：检测成功 <br>
         *        + !0：检测失败。详见[错误码](https://www.volcengine.com/docs/6705/102042)。
         */
        /** {en}
         * @brief Face Detection Result <br>
         *        + 0: Success <br>
         *        + !0: Failure. See [Error Code Table](https://docs.byteplus.com/en/effects/docs/error-code-table).
         */
        public int detect_result;
        /** {zh}
         * @brief 检测到的人脸的数量
         */
        /** {en}
         * @brief Number of the detected face
         */
        public int face_count;
        /** {zh}
         * @brief 识别到人脸的矩形框。数组的长度和检测到的人脸数量一致。参看 Rectangle{@link #Rectangle}。
         */
        /** {en}
         * @brief The face recognition rectangles. The length of the array is the same as the number of detected faces. See Rectangle{@link #Rectangle}.
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public Rectangle[] rect;
        /** {zh}
         * @brief 原始图片宽度(px)
         */
        /** {en}
         * @brief Width of the original image (px)
         */
        public int image_width;
        /** {zh}
         * @brief 原始图片高度(px)
         */
        /** {en}
         * @brief Height of the original image (px)
         */
       public int image_height ;
        /** {zh}
         * @brief 进行人脸识别的视频帧的时间戳。
         */
        /** {en}
         * @brief The time stamp of the video frame using face detection.
         */
        public ulong frame_timestamp_us;
    };
    public class ExpressionDetectInfo
    {
        /** {zh}
         * @brief 预测年龄，取值范围 (0, 100)。
         */
        /** {en}
         * @brief The estimated age in range of (0, 100).
         */
        public float age = 0;
        /** {zh}
        * @brief 预测为男性的概率，取值范围 (0.0, 1.0)。
        */
        /** {en}
         * @brief The estimated probability of being a male in range of (0.0, 1.0).
         */
        public float boy_prob = 0;
        /** {zh}
        * @brief 预测的吸引力分数，取值范围 (0, 100)。
        */
        /** {en}
         * @brief The estimated attractiveness in range of (0, 100).
         */
        public float attractive = 0;
        /** {zh}
        * @brief 预测的微笑程度，取值范围 (0, 100)。
        */
        /** {en}
         * @brief The estimated happy score in range of (0, 100).
         */
        public float happy_score = 0;
        /** {zh}
        * @brief 预测的伤心程度，取值范围 (0, 100)。
        */
        /** {en}
         * @brief The estimated sad score in range of (0, 100).
         */
        public float sad_score = 0;
        /** {zh}
        * @brief 预测的生气程度，取值范围 (0, 100)。
        */
        /** {en}
         * @brief The estimated angry score in range of (0, 100).
         */
        public float angry_score = 0;
        /** {zh}
        * @brief 预测的吃惊程度，取值范围 (0, 100)。
        */
        /** {en}
         * @brief The estimated surprise score in range of (0, 100).
         */
        public float surprise_score = 0;
        /** {zh}
        * @brief 预测的情绪激动程度，取值范围 (0, 100)。
        */
        /** {en}
         * @brief The estimated emotional arousal in range of (0, 100).
         */
        public float arousal = 0;
        /** {zh}
        * @brief 预测的情绪正负程度，取值范围 (-100, 100)。
        */
        /** {en}
         * @brief The estimated emotional valence in range of (-100, 100).
         */
        public float valence = 0;
    };
   public struct ExpressionDetectResult
    {
        /** {zh}
         * @brief 人脸信息存储上限，最多可存储 10 个人脸信息
         */
        /** {en}
         * @brief Face information storage limit, up to 10 faces can be stored.
         */
        public  const  int max_face_num = 10;
        /** {zh}
         * @brief 特征识别结果 <br>
         *        + 0：识别成功 <br>
         *        + !0：识别失败 <br>
         */
        /** {en}
         * @brief Expression detection result <br>
         *        + 0: Success <br>
         *        + !0: Failure <br>
         */
        public int detect_result ;
        /** {zh}
         * @brief 识别到的人脸数量。
         */
        /** {en}
         * @brief The number of detected faces.
         */
        public int face_count;
        /** {zh}
         * @brief 特征识别信息。数组的长度和检测到的人脸数量一致。参看 ExpressionDetectInfo{@link #ExpressionDetectInfo}。
         */
        /** {en}
         * @brief Expression detection information. The length of the array is the same as the number of detected faces. See ExpressionDetectInfo{@link #ExpressionDetectInfo}.
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ExpressionDetectInfo[] detect_info;
    };
}
