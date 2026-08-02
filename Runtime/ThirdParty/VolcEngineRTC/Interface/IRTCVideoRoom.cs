using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    #region EventHandler
    /** {zh}
     * @type callback
     * @brief 首次加入房间/重连加入房间的回调。此回调表示调用 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 的结果，根据错误码判断成功/失败以及区别是否为重连。
     * @param roomID 房间 ID。
     * @param userID 用户 ID。
     * @param errorCode 用户加入房间回调的状态码。  <br>
     *        + 0: 加入房间成功； <br>
     *        + 非 0: 加入房间失败。  <br>
     * @param joinType 用户加入房间的类型，参看 JoinRoomType{@link #JoinRoomType}。
     * @param elapsed 保留字段，无意义。
     */
    /** {en}
     * @type callback
     * @brief Callback for the first time to join a room/reconnect to join a room.<br>
     * @param roomID  Room ID.
     * @param userID  User ID.
     * @param errorCode The status code of joining the room: <br>
     *         + 0: Joining room successfully; <br>
     *         + != 0: Joining room failed.  <br>
     * @param joinType The type of joining the room. See JoinRoomType{@link #JoinRoomType}.
     * @param elapsed  Reserved parameter.
     */
    // public delegate void OnJoinRoomResultEventHandler(string roomID, string userID, int errorCode, JoinRoomType joinType, int elapsed);

    /** {zh}
     * @type callback
     * @brief 房间状态改变回调，加入房间、异常退出房间、发生房间相关的警告或错误时会收到此回调。
     * @param roomId 房间 ID。
     * @param userID 用户 ID。
     * @param state 房间状态码。
     *              + 0: 成功。
     *              + !0: 失败或异常退房。具体原因参看 ErrorCode{@link #ErrorCode} 及 WarningCode{@link #WarningCode}。异常退出房间的具体原因包括
     *                –1004：相同 ID 用户在其他端进房；
     *                –1006：用户被踢出当前房间。
     * @param ExtraInfo 额外信息。
     *                  `join_type`表示加入房间的类型，`0`为首次进房，`1`为重连进房。
     *                  `elapsed`表示加入房间耗时，即本地用户从调用 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 到加入房间成功所经历的时间间隔，单位为 ms。
     */
    /** {en}
     * @type callback
     * @brief Callback on room state changes. Via this callback you get notified of room relating warnings,  errors and events. For example, the user joins the room, the user is removed from the room, and so on.
     * @param roomId  Room ID.
     * @param userID  User ID.
     * @param state Room state code. <br>
     *              + 0: Success. <br>
     *              + !0: Failure. See ErrorCode{@link #ErrorCode} and WarningCode{@link #WarningCode} for specific indications. The user has been removed from the room due to one of the following reasons.  <br>
     *                -1004: The same user joined the room in another client<br>
     *                -1006: The administrator kicked the user out of the room by calling openAPI.
     * @param ExtraInfo Extra information.
     *                 `join_type` indicates the type of room the user joins. `0` means the user joins the room for the first time, and `1` means the user rehoins the room. <br>
     *                 `elapsed` indicates the time interval from calling JoinRoom{@link #IRTCVideoRoom#JoinRoom}  to successfully joining room, in ms.
     */
    public delegate void OnRoomStateChangedEventHandler(string roomID, string userID, int state, string ExtraInfo);

    /** {zh}
     * @type callback
     * @brief 流状态改变回调，发生流相关的警告或错误时会收到此回调。
     * @param roomID 房间 ID。
     * @param userID 用户 ID。
     * @param state 流状态码，参看 ErrorCode{@link #ErrorCode} 及 WarningCode{@link #WarningCode}。
     * @param ExtraInfo 附加信息，目前为空。
     */
    /** {en}
     * @type callback
     * @brief Callback on stream state changes. You will receive this callback when you receive stream relating warnings and errors.
     * @param roomID  Room ID.
     * @param userID  User ID.
     * @param state Room state code. See ErrorCode{@link #ErrorCode} and WarningCode{@link #WarningCode} for specific indications.
     * @param ExtraInfo Extra information. Currently unavailable.
     */
    public delegate void OnStreamStateChangedEventHandler(string roomID, string userID, int state, string ExtraInfo);

    /** {zh}
     * @type callback
     * @brief 离开房间成功回调。  <br>
     *        用户调用 LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} 方法后，SDK 会停止所有的发布订阅流，并在释放所有通话相关的音视频资源后，通过此回调通知用户离开房间成功。  <br>
     * @param roomID 房间 ID。
     * @param stats 本次通话的统计数据。详见 RtcRoomStats{@link #RtcRoomStats}。  <br>
     * @notes  <br>
     *       + 用户调用 LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} 方法离开房间后，如果立即调用 Destroy{@link #IRTCVideoRoom#Destroy} 销毁房间实例或 Release{@link #IRTCVideo#Release} 方法销毁 RTC 引擎，则将无法收到此回调事件。  <br>
     *       + 离开房间后，如果 App 需要使用系统音视频设备，则建议在收到此回调后再初始化音视频设备，否则可能由于 SDK 占用音视频设备导致初始化失败。  <br>
     */
    /** {en}
     * @type callback
     * @brief After leaving the room successfully, receives the callback.   <br>
     *        When the user calls the LeaveRoom{@link #IRTCVideoRoom#LeaveRoom}, the SDK will stop all publishing subscription streams and release all call-related media resources. After that, the user receives this callback . <br>
     * @param roomID  Room ID.
     * @param stats  Statistics for this call. See RtcRoomStats{@link #RtcRoomStats}. <br>
     * @notes   <br>
     *        + After the user calls the LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} method to leave the room, if Destroy{@link #IRTCVideoRoom#Destroy} is called to destroy the room instance or Release{@link #IRTCVideo#Release} method is called to destroy the RTC engine immediately, this callback event will not be received. <br>
     *        + If the app needs to use the system audio & video device after leaving the room, it is recommended to initialize the audio & video device after receiving this callback, otherwise the initialization may fail due to the SDK occupying the audio & video device. <br>
     */
    public delegate void OnLeaveRoomEventHandler(string roomID, RtcRoomStats stats);

    /** {zh}
     * @hidden
     * @deprecated since 3.41, use OnRoomStateChanged and OnStreamStateChanged instead.
     * @type callback
     * @brief 房间警告回调。  <br>
     *        SDK 通常会自动恢复，警告信息可以忽略。
     * @param roomID 房间 ID。
     * @param warn 警告码，参看 WarningCode{@link #WarningCode}。
     */
    /** {en}
     * @hidden
     * @deprecated since 3.41, use OnRoomStateChanged and OnStreamStateChanged instead.
     * @type callback
     * @brief Room warning.   <br>
     *        The SDK usually recovers automatically and warnings can be ignored.
     * @param roomID  Room ID.
     * @param warn See WarningCode{@link #WarningCode}.
     */
    public delegate void OnRoomWarningEventHandler(string roomID, int warn);

    /** {zh}
     * @hidden
     * @deprecated since 3.41, use OnRoomStateChanged and OnStreamStateChanged instead.
     * @type callback
     * @brief 房间错误回调。  <br>
     *        SDK 内部遇到不可恢复错误，需要根据错误码进行操作或提示用户。
     * @param roomID 房间 ID。
     * @param err 错误码，参看 ErrorCode{@link #ErrorCode}。
     */
    /** {en}
     * @hidden
     * @deprecated since 3.41, use OnRoomStateChanged and OnStreamStateChanged instead.
     * @type callback
     * @brief Room error. <br>
     *        SDK internal encountered an unrecoverable error, will notify the App through this callback, requires the App to operate according to the error code or prompt the user. <br>
     * @param roomID  Room ID.
     * @param err See ErrorCode{@link #ErrorCode}.
     */
    public delegate void OnRoomErrorEventHandler(string roomID, int err);

    /** {zh}
     * @type callback
     * @brief 远端可见用户加入房间，或房内不可见用户切换为可见的回调。  <br>
     *        + 远端可见用户调用 SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} 方法将自身设为可见后加入房间时，房间内其他用户将收到该事件。  <br>
     *        + 远端可见用户断网后重新连入房间时，房间内其他用户将收到该事件。  <br>
     *        + 房间内不可见远端用户调用 SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} 方法切换至可见时，房间内其他用户将收到该事件。  <br>
     *        + 新进房用户也会收到进房前已在房内的可见用户的进房回调通知。  <br>
     * @param roomID 房间 ID。
     * @param userInfo 用户信息，参看 UserInfo{@link #UserInfo}。  <br>
     * @param elapsed 保留字段，无意义。
     */
    /** {en}
     * @type callback
     * @brief You will receive this callback in following cases: <br>
     *        + The remote user calls SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} turns visible and joins your room. <br>
     *        + The remote visible user is disconnected and then reconnected to your room. <br>
     *        + The invisible remote user in your room calls SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} and turns visible. <br>
     *        + You join the room when there are visible users in the room.
     * @param roomID  Room ID.
     * @param userInfo See UserInfo{@link #UserInfo}. <br>
     * @param elapsed Reserved parameter.
     */
    public delegate void OnUserJoinedEventHandler(string roomID, UserInfo userInfo, int elapsed);

    /** {zh}
     * @type callback
     * @brief 远端用户离开房间，或切至不可见时，本地用户会收到此事件
     * @param roomID 房间 ID。
     * @param userID 离开房间，或切至不可见的的远端用户 ID。  <br>
     * @param reason 用户离开房间的原因，参看 UserOfflineReason{@link #UserOfflineReason}。
     */
    /** {en}
     * @type callback
     * @brief This callback is triggered when a remote user is disconnected or turns invisible.
     * @param roomID  Room ID.
     * @param userID ID of the user who leaves the room, or switches to invisible. <br>
     * @param reason Reason to leave the room. See UserOfflineReason{@link #UserOfflineReason}.
     */
    public delegate void OnUserLeaveEventHandler(string roomID, string userID, UserOfflineReason reason);


    /** {zh}
     * @type callback
     * @region 房间
     * @author hanchenchen.c
     * @brief 接收到房间内广播消息的回调。<br>
     *        房间内其他用户调用 sendRoomMessage{@link #IRTCAudioRoomsendRoomMessage} 发送广播消息时，收到此回调。
     * @param roomID 房间 ID。
     * @param userID 消息发送者 ID
     * @param message 收到的消息内容
     */
    /** {en}
     * @type callback
     * @region Room
     * @author hanchenchen.c
     * @brief Receive a callback for broadcast messages in the room. <br>
     *        This callback is received when other users in the room call sendRoomMessage{@link #IRTCAudioRoomsendRoomMessage} to send a broadcast message.
     * @param roomID Room ID.
     * @param userID User ID of the message sender
     * @param message Message content received
     */
    public delegate void OnRoomMessageReceivedEventHandler(string roomID, string userID, string message);

    /** {zh}
     * @type callback
     * @region 房间
     * @author hanchenchen.c
     * @brief 收到来自房间中其他用户通过 SendUserMessage{@link #IRTCVideoRoom#SendUserMessage} 发来的点对点文本消息时，会收到此回调。
     * @param roomID 房间 ID。
     * @param userID 消息发送者的用户 ID 。
     * @param message 收到的文本消息内容。
     */
    /** {en}
     * @type callback
     * @region Room
     * @author hanchenchen.c
     * @brief Receive this callback when you receive a peer-to-peer text message from another user in the room via SendUserMessage{@link #IRTCVideoRoom#SendUserMessage}.
     * @param roomID Room ID.
     * @param userID The user ID of the sender of the message.
     * @param Message The content of the received text message.
     */
    public delegate void OnUserMessageReceivedEventHandler(string roomID, string userID, string message);

    /** {zh}
     * @type callback
     * @region 房间
     * @author hanchenchen.c
     * @brief 向房间内单个用户发送点对点文本或点对点二进制消息后，消息发送方会收到该消息发送结果回调。
     * @param roomID 房间 ID。
     * @param msgID 本条消息的 ID。
     * @param error 文本或二进制消息发送结果。
     * @notes 调用 sendUserMessage{@link #IRTCAudioRoomsendUserMessage} 或 sendUserBinaryMessage{@link #IRTCAudioRoomsendUserBinaryMessage} 接口，才能收到此回调。
     */
    /** {en}
     * @type callback
     * @region Room
     * @author hanchenchen.c
     * @brief After sending a point-to-point text or point-to-point binary message to a single user in the room, the message sender will receive a callback with the result of the message.
     * @param roomID Room ID.
     * @param msgID The ID of this message.
     * @param error Text or binary message sending results.
     * @notes Call sendUserMessage{@link #IRTCAudioRoomsendUserMessage} or sendUserBinaryMessage{@link #IRTCAudioRoomsendUserBinaryMessage} interface to receive this callback.
     */
    public delegate void OnUserMessageSendResultEventHandler(string roomID, long msgID, int error);

    /** {zh}
     * @type callback
     * @region 房间
     * @author hanchenchen.c
     * @brief 调用 SendRoomMessage{@link #IRTCVideoRoom#SendRoomMessage} 向房间内群发文本或二进制消息后，消息发送方会收到该消息发送结果回调。
     * @param roomID 房间 ID。
     * @param msgid 本条消息的 ID。
     * @param error 消息发送结果。
     */
    /** {en}
     * @type callback
     * @region Room
     * @author hanchenchen.c
     * @brief Receives this callback after sending a text message or a binary message to a room with SendRoomMessage{@link #IRTCVideoRoom#SendRoomMessage}.
     * @param roomID Room ID.
     * @param msgid The ID of this message.
     * @param error Message sending result.
     */
    public delegate void OnRoomMessageSendResultEventHandler(string roomID, long msgID, int error);
    // stream
    /** {zh}
     * @type callback
     * @brief 房间内新增远端摄像头/麦克风采集的媒体流的回调。
     * @param roomID 房间 ID。
     * @param userID 远端流发布用户的用户 ID。
     * @param type 远端媒体流的类型，参看 MediaStreamType{@link #MediaStreamType}。
     * @notes 当房间内的远端用户调用 PublishStream{@link #IRTCVideoRoom#PublishStream} 成功发布由摄像头/麦克风采集的媒体流时，本地用户会收到该回调，此时本地用户可以自行选择是否调用 SubscribeStream{@link #IRTCVideoRoom#SubscribeStream} 订阅此流。
     */
    /** {en}
     * @type callback
     * @brief Callback on new media streams captured by camera/microphone in the room.
     * @param roomID  Room ID.
     * @param userID The ID of the remote user who published the stream.
     * @param type Media stream type. See MediaStreamType{@link #MediaStreamType}.
     * @notes You will receive this callback after a remote user successfully published media streams captured by camera/microphone in the room with PublishStream{@link #IRTCVideoRoom#PublishStream}. Then you can choose whether to call SubscribeStream{@link #IRTCVideoRoom#SubscribeStream} to subscribe to the streams or not.
     */
    public delegate void OnUserPublishStreamEventHandler(string roomID, string userID, MediaStreamType type);

    /** {zh}
     * @type callback
     * @brief 房间内远端摄像头/麦克风采集的媒体流移除的回调。
     * @param roomID 房间 ID。
     * @param userID 移除的远端流发布用户的用户 ID。  <br>
     * @param type 移除的远端流类型，参看 MediaStreamType{@link #MediaStreamType}。  <br>
     * @param reason 远端流移除的原因，参看 StreamRemoveReason{@link #StreamRemoveReason}。
     * @notes 收到该回调通知后，你可以自行选择是否调用 UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream} 取消订阅此流。
     */
    /** {en}
     * @type callback
     * @brief Callback on removal of remote media stream captured by camera/microphone.
     * @param roomID  Room ID.
     * @param userID The ID of the remote user who removed the stream.
     * @param type Media stream type. See MediaStreamType{@link #MediaStreamType}.
     * @param reason The reason for the removal, see StreamRemoveReason{@link #StreamRemoveReason}.
     * @notes After receiving this callback, you can choose whether to call UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream} to unsubscribe from the streams or not.
     */
    public delegate void OnUserUnPublishStreamEventHandler(string roomID, string userID, MediaStreamType type, StreamRemoveReason reason);

    /** {zh}
     * @type callback
     * @brief 房间内新增远端屏幕共享音视频流的回调。
     * @param roomID 房间 ID。
     * @param userID 远端流发布用户的用户 ID。
     * @param type 远端媒体流的类型，参看 MediaStreamType{@link #MediaStreamType}。
     * @notes 当房间内的远端用户调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 成功发布来自屏幕共享的音视频流时，本地用户会收到该回调，此时本地用户可以自行选择是否调用 SubscribeScreen{@link #IRTCVideoRoom#SubscribeScreen} 订阅此流。
     */
    /** {en}
     * @type callback
     * @brief Callback on new screen sharing media streams from remote users in the room.
     * @param roomID  Room ID.
     * @param userID The ID of the remote user who published the stream.
     * @param type Media stream type. See MediaStreamType{@link #MediaStreamType}.
     * @notes You will receive this callback after a remote user successfully published screen sharing streams in the room with PublishScreen{@link #IRTCVideoRoom#PublishScreen}. Then you can choose whether to call SubscribeScreen{@link #IRTCVideoRoom#SubscribeScreen} to subscribe to the streams or not.
     */
    public delegate void OnUserPublishScreenEventHandler(string roomID, string userID, MediaStreamType type);

    /** {zh}
     * @type callback
     * @brief 房间内远端屏幕共享音视频流移除的回调。
     * @param roomID 房间 ID。
     * @param userID 移除的远端流发布用户的用户 ID。  <br>
     * @param type 移除的远端流类型，参看 MediaStreamType{@link #MediaStreamType}。  <br>
     * @param reason 远端流移除的原因，参看 StreamRemoveReason{@link #StreamRemoveReason}。
     * @notes 收到该回调通知后，你可以自行选择是否调用 UnsubscribeScreen{@link #IRTCVideoRoom#UnsubscribeScreen} 取消订阅此流。
     */
    /** {en}
     * @type callback
     * @brief Callback on removal of screen sharing media streams from remote users in the room.
     * @param roomID  Room ID.
     * @param userID The ID of the remote user who removed the stream.
     * @param type Media stream type. See MediaStreamType{@link #MediaStreamType}.
     * @param reason The reason for the removal, see StreamRemoveReason{@link #StreamRemoveReason}.
     * @notes After receiving this callback, you can choose whether to call UnsubscribeScreen{@link #IRTCVideoRoom#UnsubscribeScreen} to unsubscribe from the streams or not.
     */
    public delegate void OnUserUnPublishScreenEventHandler(string roomID, string userID, MediaStreamType type, StreamRemoveReason reason);

    /** {zh}
     * @type callback
     * @brief 关于订阅媒体流状态改变的回调
     * @param roomID 房间 ID。
     * @param stateCode 订阅媒体流状态，参看 SubscribeState{@link #SubscribeState}
     * @param userId 流发布用户的用户 ID
     * @param info 流的属性，参看 SubscribeConfig{@link #SubscribeConfig}
     * @notes 本地用户收到该回调的时机包括：  <br>
     *        + 调用 SubscribeStream{@link #IRTCVideoRoom#SubscribeStream} 或 UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream} 订阅/取消订阅指定远端摄像头音视频流后；  <br>
     *        + 调用 SubscribeScreen{@link #IRTCVideoRoom#SubscribeScreen} 或 UnsubscribeScreen{@link #IRTCVideoRoom#UnsubscribeScreen} 订阅/取消订阅指定远端屏幕共享流后。
     */
    /** {en}
     * @type callback
     * @brief Callback on subscription status of media streams
     * @param roomID  Room ID.
     * @param stateCode Subscription status of media streams, see SubscribeState{@link #SubscribeState}.
     * @param userId The ID of the user who published the stream.
     * @param info Configurations of stream subscription, see SubscribeConfig{@link #SubscribeConfig}.
     * @notes Local users will receive this callback:  <br>
     *        + After calling SubscribeStream{@link #IRTCVideoRoom#SubscribeStream}/UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream} to change the subscription status of remote media streams captured by camera/microphone.  <br>
     *        + After calling SubscribeScreen{@link #IRTCVideoRoom#SubscribeScreen}/UnsubscribeScreen{@link #IRTCVideoRoom#UnsubscribeScreen} to change the subscription status of remote screen sharing streams.
     */
    public delegate void OnStreamSubscribedEventHandler(string roomID, SubscribeState stateCode, string userID, SubscribeConfig info);

    // audio
    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StartAudioCapture{@link #IRTCVideo#StartAudioCapture} 开启音频采集时，房间内其他用户会收到此回调。
     * @param roomID 房间 ID。
     * @param userID 开启音频采集的远端用户 ID
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via this callback after the visible user starts audio capture by calling StartAudioCapture{@link #IRTCVideo#StartAudioCapture}.
     * @param roomID  Room ID.
     * @param userID The user who starts the internal audio capture
     */
    // public delegate void OnUserStartAudioCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StopAudioCapture{@link #IRTCVideo#StopAudioCapture} 关闭音频采集时，房间内其他用户会收到此回调。
     * @param roomID 房间 ID。
     * @param userID 关闭音频采集的远端用户 ID
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via this callback after the visible user stops audio capture by calling StopAudioCapture{@link #IRTCVideo#StopAudioCapture}.
     * @param roomID  Room ID.
     * @param userID The user who stops the internal audio capture
     */
    // public delegate void OnUserStopAudioCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief 本地采集到第一帧音频帧时，收到该回调。
     * @param roomID 房间 ID。
     * @param index 音频流属性, 详见 StreamIndex{@link #StreamIndex}
     */
    /** {en}
     * @type callback
     * @brief Receive the callback when the first audio frame is locally collected.
     * @param roomID  Room ID.
     * @param index  Audio stream properties. See StreamIndex{@link #StreamIndex}
     */
    // public delegate void OnFirstLocalAudioFrameEventHandler(string roomID, StreamIndex index);

    /** {zh}
     * @type callback
     * @brief 接收到来自远端某音频流的第一帧时，收到该回调。
     * @param roomID 房间 ID。
     * @param key 远端音频流信息, 详见 RemoteStreamKey{@link #RemoteStreamKey}
     * @notes 用户刚收到房间内订阅的每一路音频流时，都会收到该回调。
     */
    /** {en}
     * @type callback
     * @brief Receives the callback when the first frame from a remote audio stream is received.
     * @param roomID  Room ID.
     * @param key Remote audio stream information. See RemoteStreamKey{@link #RemoteStreamKey}
     * @notes The callback will be received when the user has just received each audio stream subscribed to in the room.
     */
    //public delegate void OnFirstRemoteAudioFrameEventHandler(string roomID, RemoteStreamKey key);

    /** {zh}
     * @type callback
     * @brief 本地音频流的状态发生改变时，该回调通知当前的本地音频流状态。
     * @param roomID 房间 ID。
     * @param state 本地音频设备的状态，详见 LocalAudioStreamState{@link #LocalAudioStreamState}
     * @param error 本地音频流状态改变时的错误码，详见 LocalAudioStreamError{@link #LocalAudioStreamError}
     */
    /** {en}
     * @type callback
     * @brief When the state of the local audio stream changes, the callback notifies the current local audio stream state.
     * @param roomID  Room ID.
     * @param state The status of the local audio device. See LocalAudioStreamState{@link #LocalAudioStreamState}
     * @param error The error code when the state of the local audio stream changes. See LocalAudioStreamError{@link #LocalAudioStreamError}
     */
    //public delegate void OnLocalAudioStateChangedEventHandler(string roomID, LocalAudioStreamState state, LocalAudioStreamError error);

    /** {zh}
     * @type callback
     * @brief 用户订阅来自远端的音频流状态发生改变时，会收到此回调，了解当前的远端音频流状态。
     * @param roomID 房间 ID。
     * @param key 远端流信息, 详见 RemoteStreamKey{@link #RemoteStreamKey}
     * @param state 远端音频流状态，详见 RemoteAudioState{@link #RemoteAudioState}
     * @param reason 远端音频流状态改变的原因，详见 RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}
     */
    /** {en}
     * @type callback
     * @brief When the state of the audio stream from the remote user subscribes to changes, this callback will be received to understand the current state of the remote audio stream.
     * @param roomID  Room ID.
     * @param key Remote Stream Information. See RemoteStreamKey{@link #RemoteStreamKey}
     * @param state Remote Audio Stream Status. See RemoteAudioState{@link #RemoteAudioState}
     * @param reason Remote Audio Stream Status Change Reason. See RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}
     */
    //   public delegate void OnRemoteAudioStateChangedEventHandler(string roomID, RemoteStreamKey key, RemoteAudioState state, RemoteAudioStateChangeReason reason);

    /** {zh}
     * @type callback
     * @brief 本地音频首帧发送状态发生改变时，收到此回调
     * @param roomID 房间 ID。
     * @param user 本地用户信息，详见 RtcUser{@link #RtcUser}
     * @param state 首帧发送状态，详见 FirstFrameSendState{@link #FirstFrameSendState}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the sending status of the first frame of the local audio changes
     * @param roomID  Room ID.
     * @param user Local user information. See RtcUser{@link #RtcUser}
     * @param state First frame sending status. See FirstFrameSendState{@link #FirstFrameSendState}
     */
    // public delegate void OnAudioFrameSendStateChangedEventHandler(string roomID, RtcUser user, FirstFrameSendState state);

    /** {zh}
     * @type callback
     * @brief 音频首帧播放状态发生改变时，收到此回调
     * @param roomID 房间 ID。
     * @param user 远端用户信息，详见 RtcUser{@link #RtcUser}
     * @param state 首帧播放状态，详见 FirstFramePlayState{@link #FirstFramePlayState}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the audio first frame playback state changes
     * @param roomID  Room ID.
     * @param user Remote user information. See RtcUser{@link #RtcUser}
     * @param state First frame playback status. See FirstFramePlayState{@link #FirstFramePlayState}
     */
    // public delegate void OnAudioFramePlayStateChangedEventHandler(string roomID, RtcUser user, FirstFramePlayState state);

    // video
    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StartVideoCapture{@link #IRTCVideo#StartVideoCapture} 开启内部视频采集时，房间内其他用户会收到此回调。
     * @param roomID 房间 ID。
     * @param userID 开启视频采集的远端用户 ID
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via this callback after the visible user starts video capture by calling StartVideoCapture{@link #IRTCVideo#StartVideoCapture}.
     * @param roomID  Room ID.
     * @param userID The user who starts the internal video capture
     */
    //public delegate void OnUserStartVideoCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief 房间内的可见用户调用 StopVideoCapture{@link #IRTCVideo#StopVideoCapture} 关闭内部视频采集时，房间内其他用户会收到此回调。
     * @param roomID 房间 ID。
     * @param userID 关闭视频采集的远端用户 ID
     */
    /** {en}
     * @type callback
     * @brief The remote clients in the room will be informed of the state change via  this callback after the visible user stops video capture by calling StopVideoCapture{@link #IRTCVideo#StopVideoCapture}.
     * @param roomID  Room ID.
     * @param userID The user who stops the internal video capture
     */
    //public delegate void OnUserStopVideoCaptureEventHandler(string roomID, string userID);

    /** {zh}
     * @type callback
     * @brief RTC SDK 在本地完成第一帧视频帧或屏幕视频帧采集时，收到此回调。
     * @param roomID 房间 ID。
     * @param index 流属性，参看 StreamIndex{@link #StreamIndex}
     * @param info 视频信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
     * @notes 对于采集到的本地视频帧，你可以调用 SetLocalVideoSink{@link #IRTCVideo#SetLocalVideoSink} 在本地渲染。
     */
    /** {en}
     * @type callback
     * @brief RTC SDK receives this callback when the first video frame or screen video frame capture is completed locally.
     * @param roomID  Room ID.
     * @param index Stream properties. See StreamIndex{@link #StreamIndex}
     * @param info Video information. See VideoFrameInfo{@link #VideoFrameInfo}
     * @notes For captured local video frames, you can call SetLocalVideoSink{@link #IRTCVideo#SetLocalVideoSink} to render locally.
     */
    //public delegate void OnFirstLocalVideoFrameCapturedEventHandler(string roomID, StreamIndex index, VideoFrameInfo info);

    /** {zh}
     * @type callback
     * @brief 本地视频大小或旋转信息发生改变时，收到此回调。
     * @param roomID 房间 ID。
     * @param index 流属性，参看 StreamIndex{@link #StreamIndex}
     * @param info 视频帧信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when local video size or rotation information changes.
     * @param roomID  Room ID.
     * @param index Stream properties. See StreamIndex{@link #StreamIndex}
     * @param info Video frame information. See VideoFrameInfo{@link #VideoFrameInfo}
     */
    //  public delegate void OnLocalVideoSizeChangedEventHandler(string roomID, StreamIndex index, VideoFrameInfo info);

    /** {zh}
     * @type callback
     * @brief SDK 内部渲染成功远端视频流首帧后，收到此回调。
     * @param roomID 房间 ID。
     * @param key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}
     * @param info 视频帧信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback after the first frame of remote video stream is locally rendered by SDK.
     * @param roomID  Room ID.
     * @param key Remote Stream Information. See RemoteStreamKey{@link #RemoteStreamKey}
     * @param info Video Frame Information. See VideoFrameInfo{@link #VideoFrameInfo}
     */
    // public delegate void OnFirstRemoteVideoFrameRenderedEventHandler(string roomID, RemoteStreamKey key, VideoFrameInfo info);

    /** {zh}
     * @type callback
     * @brief SDK 接收并解码远端视频流首帧后，收到此回调。
     * @param roomID 房间 ID。
     * @param key 远端流信息，参看 RemoteStreamKey{@link #RemoteStreamKey}
     * @param info 视频帧信息，参看 VideoFrameInfo{@link #VideoFrameInfo}
     */
    /** {en}
     * @brief Receive this callback after the first frame of remote video stream is received and decoded by SDK.
     * @param roomID  Room ID.
     * @param key Remote Stream Information, see RemoteStreamKey {@link #RemoteStreamKey}
     * @param info Video Frame Information, see VideoFrameInfo {@link #VideoFrameInfo}
     */
    //public delegate void OnFirstRemoteVideoFrameDecodedEventHandler(string roomID, RemoteStreamKey key, VideoFrameInfo info);

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
    //  public delegate void OnRemoteVideoSizeChangedEventHandler(string roomID, RemoteStreamKey key, VideoFrameInfo info);

    /** {zh}
     * @type callback
     * @brief 本地视频流的状态发生改变时，收到该事件。
     * @param roomID 房间 ID。
     * @param index 流属性，参看 StreamIndex{@link #StreamIndex}
     * @param state 本地视频流状态，参看 LocalVideoStreamState{@link #LocalVideoStreamState}
     * @param error 本地视频状态改变时的错误码，参看 LocalVideoStreamError{@link #LocalVideoStreamError}
     */
    /** {en}
     * @type callback
     * @brief Receive this event when the state of the local video stream changes.
     * @param roomID  Room ID.
     * @param index Stream property. See StreamIndex{@link #StreamIndex}
     * @param state Local video stream state. See LocalVideoStreamState{@link #LocalVideoStreamState}
     * @param error Error code when local video state changes. See LocalVideoStreamError{@link #LocalVideoStreamError}
     */
    //  public delegate void OnLocalVideoStateChangedEventHandler(string roomID, StreamIndex index, LocalVideoStreamState state, LocalVideoStreamError error);

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
    //  public delegate void OnRemoteVideoStateChangedEventHandler(string roomID, RemoteStreamKey key, RemoteVideoState state, RemoteVideoStateChangeReason reason);

    /** {zh}
     * @type callback
     * @brief 本地视频首帧发送状态发生改变时，收到此回调
     * @param roomID 房间 ID。
     * @param user 本地用户信息，详见 RtcUser{@link #RtcUser}
     * @param state 首帧发送状态，详见 FirstFrameSendState{@link #FirstFrameSendState}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the sending status of the first frame of the local video changes
     * @param roomID  Room ID.
     * @param user Local user information. See RtcUser{@link #RtcUser}
     * @param state First frame sending status. See FirstFrameSendState{@link #FirstFrameSendState}
     */
    // public delegate void OnVideoFrameSendStateChangedEventHandler(string roomID, RtcUser user, FirstFrameSendState state);

    /** {zh}
     * @type callback
     * @brief 屏幕共享流的视频首帧发送状态发生改变时，收到此回调
     * @param roomID 房间 ID。
     * @param user 本地用户信息，详见 RtcUser{@link #RtcUser}
     * @param state 首帧发送状态，详见 FirstFrameSendState{@link #FirstFrameSendState}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the video first frame sending state of the screen sharing stream changes
     * @param roomID  Room ID.
     * @param user Local user information. See RtcUser{@link #RtcUser}
     * @param state First frame sending status. See FirstFrameSendState{@link #FirstFrameSendState}
     */
    // public delegate void OnScreenVideoFrameSendStateChangedEventHandler(string roomID, RtcUser user, FirstFrameSendState state);

    /** {zh}
     * @type callback
     * @brief 视频首帧播放状态发生改变时，收到此回调
     * @param roomID 房间 ID。
     * @param user 远端用户信息，详见 RtcUser{@link #RtcUser}
     * @param state 首帧播放状态，详见 FirstFramePlayState{@link #FirstFramePlayState}
     */
    /** {en}
     * @type callback
     * @brief Receive this callback when the playback state of the first frame of the video changes
     * @param roomID  Room ID.
     * @param user Remote user information. See RtcUser{@link #RtcUser}
     * @param state First frame playback status. See FirstFramePlayState{@link #FirstFramePlayState}
     */
    //  public delegate void OnVideoFramePlayStateChangedEventHandler(string roomID, RtcUser user, FirstFramePlayState state);
    /** {zh}
     * @type callback
     * @brief Token 过期前 30 秒将触发该回调。<br>
     *        调用 UpdateToken{@link #IRTCVideoRoom#UpdateToken} 更新 Token。否则 Token 过期后，用户将被移出房间无法继续进行音视频通话。
     */
    /** {en}
    * @type callback
    * @brief You will be informed 30 sec before the expiration via this callback.
    *        Call UpdateToken{@link #IRTCVideoRoom#UpdateToken} to renew the Token. If the Token expired, the user would be removed from the room and not be able to continue the call.
    */
    public delegate void OnTokenWillExpireEventHandler(string roomID);
    /** {zh}
    * @type callback
    * @region 音频事件回调
    * @brief 通过调用服务端 MuteUser/UnmuteUser 方法禁用/解禁指定房间内指定用户视音频流的发送时，触发此回调。
    * @param uid 被禁用/解禁的音频流用户 ID
    * @param banned 音频流发送状态 <br>
    *        + true: 音频流发送被禁用 <br>
    *        + false: 音频流发送被解禁
    * @notes  <br>
    *        + 房间内指定用户被禁止/解禁音频流发送时，房间内所有用户都会收到该回调。  <br>
    *        + 若被封禁用户退房后再进房，则依然是封禁状态，且房间内所有人会再次收到该回调。  <br>
    *        + 若被封禁用户断网后重连进房，则依然是封禁状态，且只有本人会再次收到该回调。  <br>
    *        + 指定用户被封禁后，房间内其他用户退房后再进房，会再次收到该回调。  <br>
    *        + 通话人数超过 5 人时，只有被封禁/解禁用户会收到该回调。   <br>
    *        + 同一房间解散后再次创建，房间内状态清空。
    */
    /** {en}
     * @type callback
     * @region Audio event callback
     * @brief This callback is triggered when the server level MuteUser/UnmuteUser method is called to disable/unban the sending of the specified user's audio and video stream in a specified room.
     * @param  uid Disabled/unbanned audio stream user ID
     * @param  banned Audio stream sending status <br>
     *         + True: audio stream sending is disabled <br>
     *         + False: audio stream sending is unbanned
     * @notes   <br>
     *         + Specified users in the room are prohibited/all users in the room when audio stream sending is unbanned Will receive the callback. <br>
     *         + If the banned user checks out and then enters the room, it will still be banned status, and everyone in the room will receive the callback again. <br>
     *         + If the banned user is disconnected and reconnected to the room, it will still be banned status, and only the person will receive the callback again. <br>
     *         + After the specified user is banned, other users in the room will check out and enter the room again, and will receive the callback again. <br>
     *         + When the number of calls exceeds 5, only blocked/unblocked users will receive the callback. <br>
     *         + The same room is created again after dissolution, and the state in the room is empty.
     */
    public delegate void OnAudioStreamBannedEventHandler(string roomID, string userID,bool banned);
    /** {zh}
    * @type callback
    * @region 视频管理
    * @brief 通过调用服务端 MuteUser/UnmuteUser 方法禁用/解禁指定房间内指定用户视频流的发送时，触发此回调。
    * @param roomID 被禁用/解禁的视频流房间 ID
    * @param userID 被禁用/解禁的视频流用户 ID
    * @param banned 视频流发送状态 <br>
    *        + true: 视频流发送被禁用 <br>
    *        + false: 视频流发送被解禁
    * @notes  <br>
    *        + 房间内指定用户被禁止/解禁视频流发送时，房间内所有用户都会收到该回调。  <br>
    *        + 若被封禁用户退房后再进房，则依然是封禁状态，且房间内所有人会再次收到该回调。  <br>
    *        + 若被封禁用户断网后重连进房，则依然是封禁状态，且只有本人会再次收到该回调。  <br>
    *        + 指定用户被封禁后，房间内其他用户退房后再进房，会再次收到该回调。  <br>
    *        + 通话人数超过 5 人时，只有被封禁/解禁用户会收到该回调。  <br>
    *        + 同一房间解散后再次创建，房间内状态清空。
    */
    /** {en}
     * @type callback
     * @region Video management
     * @brief This callback is triggered when the server level MuteUser/UnmuteUser method is invoked to disable/unban the sending of a specified user's video stream in a specified room.
     * @param  roomID Room ID of the video stream that was disabled/unbanned
     * @param  userID User ID of the video stream that was disabled/unbanned
     * @param  banned Video stream sending status <br>
     *         + True: Video stream sending was disabled <br>
     *         + False: Video stream sending was unbanned
     * @notes   <br>
     *         + When the specified user in the room is disabled/unbanned Video stream sending, all users in the room will receive the callback .. <br>
     *         + If the banned user checks out and then enters the room, it will still be banned status, and everyone in the room will receive the callback again. <br>
     *         + If the banned user is disconnected and reconnected to the room, it will still be banned status, and only the person will receive the callback again. <br>
     *         + After the specified user is banned, other users in the room will check out and enter the room again, and will receive the callback again. <br>
     *         + When the number of calls exceeds 5, only blocked/unblocked users will receive the callback. <br>
     *         + The same room is created again after dissolution, and the state in the room is empty.
     */
    public delegate void OnVideoStreamBannedEventHandler(string roomID, string userID, bool banned);

    public delegate void OnForwardStreamStateChangedEventHandler(List<ForwardStreamStateInfo> infos, int info_count);
    public delegate void OnForwardStreamStateChangedCallback(IntPtr infos, int info_count);

    public delegate void OnForwardStreamEventEventHandler(List<ForwardStreamEventInfo> infos, int info_count);
    public delegate void OnForwardStreamEventCallback(IntPtr infos, int info_coun);
    public delegate void OnAVSyncStateChangeCallback(AVSyncState state);
    #endregion


    /** {zh} 
     * @type api
     * @brief 房间接口
     */
    /** {en} 
     * @type api
     * @brief Room interface.
     */
    public interface IRTCVideoRoom
    {
        #region Events
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //     event OnJoinRoomResultEventHandler OnJoinRoomResultEvent;//
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnRoomStateChangedEventHandler OnRoomStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnStreamStateChangedEventHandler OnStreamStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnLeaveRoomEventHandler OnLeaveRoomEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnRoomWarningEventHandler OnRoomWarningEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnRoomErrorEventHandler OnRoomErrorEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnUserJoinedEventHandler OnUserJoinedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnUserLeaveEventHandler OnUserLeaveEvent;

        /** {zh} 
    * @hidden
    */
        /** {en} 
        * @hidden
        */
        event OnRoomMessageReceivedEventHandler OnRoomMessageReceivedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
        * @hidden
        */
        event OnUserMessageReceivedEventHandler OnUserMessageReceivedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
        * @hidden
        */
        event OnUserMessageSendResultEventHandler OnUserMessageSendResultEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
        * @hidden
        */
        event OnRoomMessageSendResultEventHandler OnRoomMessageSendResultEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnUserPublishStreamEventHandler OnUserPublishStreamEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnUserUnPublishStreamEventHandler OnUserUnPublishStreamEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnUserPublishScreenEventHandler OnUserPublishScreenEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnUserUnPublishScreenEventHandler OnUserUnPublishScreenEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnStreamSubscribedEventHandler OnStreamSubscribedEvent;

        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnNetworkQualityEventHandler OnNetworkQualityEvent;

        event OnForwardStreamStateChangedEventHandler OnForwardStreamStateChangedEvent;
        event OnForwardStreamEventEventHandler OnForwardStreamEventEvent;
        event OnAVSyncStateChangeCallback OnAVSyncStateChangeEvent;

        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnLocalStreamStatsEventHandler OnLocalStreamStatsEvent;

        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        event OnRemoteStreamStatsEventHandler OnRemoteStreamStatsEvent;

        // audio
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //  event OnUserStartAudioCaptureEventHandler OnUserStartAudioCaptureEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //   event OnUserStopAudioCaptureEventHandler OnUserStopAudioCaptureEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //  event OnFirstLocalAudioFrameEventHandler OnFirstLocalAudioFrameEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //  event OnFirstRemoteAudioFrameEventHandler OnFirstRemoteAudioFrameEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        // event OnLocalAudioStateChangedEventHandler OnLocalAudioStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //   event OnRemoteAudioStateChangedEventHandler OnRemoteAudioStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        // event OnAudioFrameSendStateChangedEventHandler OnAudioFrameSendStateChangedEvent;

        // video
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //  event OnUserStartVideoCaptureEventHandler OnUserStartVideoCaptureEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //      event OnUserStopVideoCaptureEventHandler OnUserStopVideoCaptureEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //      event OnFirstLocalVideoFrameCapturedEventHandler OnFirstLocalVideoFrameCapturedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //     event OnLocalVideoSizeChangedEventHandler OnLocalVideoSizeChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //     event OnFirstRemoteVideoFrameRenderedEventHandler OnFirstRemoteVideoFrameRenderedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //       event OnFirstRemoteVideoFrameDecodedEventHandler OnFirstRemoteVideoFrameDecodedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //      event OnRemoteVideoSizeChangedEventHandler OnRemoteVideoSizeChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //      event OnLocalVideoStateChangedEventHandler OnLocalVideoStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //       event OnRemoteVideoStateChangedEventHandler OnRemoteVideoStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //        event OnVideoFrameSendStateChangedEventHandler OnVideoFrameSendStateChangedEvent;
        /** {zh} 
         * @hidden
         */
        /** {en} 
         * @hidden
         */
        //        event OnScreenVideoFrameSendStateChangedEventHandler OnScreenVideoFrameSendStateChangedEvent;
        /** {zh} 
        * @hidden
        */
        /** {en} 
         * @hidden
         */
        event OnTokenWillExpireEventHandler OnTokenWillExpireEvent;
        /** {zh} 
        * @hidden
        */
        /** {en} 
         * @hidden
         */
                event OnAudioStreamBannedEventHandler OnAudioStreamBannedEvent;
        /** {zh} 
        * @hidden
        */
        /** {en} 
         * @hidden
         */
               event OnVideoStreamBannedEventHandler OnVideoStreamBannedEvent;
        #endregion

        /** {zh}
         * @type api
         * @brief 退出并销毁调用 CreateRTCRoom{@link #IRTCVideo#CreateRTCRoom} 所创建的房间。
         */
        /** {en}
         * @type api
         * @brief Leave and Destroy the room created by calling CreateRTCRoom{@link #IRTCVideo#CreateRTCRoom}.
         */
        void Destroy();

        /** {zh}
         * @type api
         * @brief 加入房间。
         *        调用 CreateRTCRoom{@link #IRTCVideo#CreateRTCRoom} 创建房间后，调用此方法加入房间，同房间内其他用户进行音视频通话。
         * @param token 动态密钥，用于对进房用户进行鉴权验证。
         *        进入房间需要携带 Token。测试时可使用控制台生成临时 Token，正式上线需要使用密钥 SDK 在你的服务端生成并下发 Token。
         *        使用不同 AppID 的 App 是不能互通的。
         *        请务必保证生成 Token 使用的 AppID 和创建引擎时使用的 AppID 相同，否则会导致加入房间失败。
         * @param info 用户信息。参看 UserInfo{@link #UserInfo}。
         * @param roomConfig 房间参数配置，设置房间模式以及是否自动发布或订阅流。具体配置模式参看 MultiRoomConfig{@link #MultiRoomConfig}。
         * @return  <br>
         *        +  0: 成功。
         *        + -1: 参数无效。
         *        + -2: 已经在房间内。接口调用成功后，只要收到返回值为 0，且未调用 LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} 成功，则再次调用进房接口时，无论填写的房间 ID 和用户 ID 是否重复，均触发此返回值。
         *        + -3: room 为空。
         * @notes  <br>
         *       + 同一个 App ID 的同一个房间内，每个用户的用户 ID 必须是唯一的。如果两个用户的用户 ID 相同，则后进房的用户会将先进房的用户踢出房间，并且先进房的用户会收到 OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} 回调通知。
         *       + 本地用户调用此方法加入房间成功后，会收到 OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} 回调通知。
         *       + 本地用户调用 SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} 将自身设为可见后加入房间，远端用户会收到 OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} 回调通知。
         *       + 用户加入房间成功后，在本地网络状况不佳的情况下，SDK 可能会与服务器失去连接，此时 SDK 将会自动重连。重连成功后，本地会收到 OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} 回调通知；如果加入房间的用户是可见用户，远端用户会收到 OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} 回调通知。
        */
        /** {en}
         * @type api
         * @brief Join the room.
         *         Call CreateRTCRoom{@link #IRTCVideo#CreateRTCRoom} After creating a room, call this method to join the room and make audio & video calls with other users in the room. <br>
         * @param token  Dynamic key for authentication and verification of users entering the room. <br>
         *         You need to bring Token to enter the room. When testing, you can use the console to generate temporary tokens. The official launch requires the use of the key SDK to generate and issue tokens at your server level. <br>
         *         Apps with different AppIDs are not interoperable. <br>
         *         Make sure that the AppID used to generate the Token is the same as the AppID used to create the engine, otherwise it will cause the join room to fail. <br>
         * @param info  User information. See UserInfo{@link #UserInfo}.
         * @param roomConfig  Room parameter configuration, set the room mode and whether to automatically publish or subscribe to the flow. See MultiRoomConfig{@link #MultiRoomConfig} for the specific configuration mode. <br>
         * @return  <br>
         *         + 0: Success.
         *         + -1: Invalid parameter.
         *         + -2: Already in the room. After the interface call is successful, as long as the return value of 0 is received and LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} is not called successfully, this return value will be triggered when the room entry interface is called again, regardless of whether the filled room ID and user ID are duplicated.
         *         + -3: Room is empty.
         * @notes   <br>
         *         + In the same room with the same AppID, the user ID of each user must be unique. If two users have the same user ID, the user who joined the room later will kick the user who joined the room first out of the room, and the user who joined the room first will receive OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} callback notification.
         *         + Local users will receive OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} callback notification after calling this method to join the room successfully.
         *         + Local users call SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} to add the room after making itself visible, and remote users will receive OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} callback notification.
         *         + After the user successfully joins the room, the SDK may lose connection to the server in case of poor local network conditions, and the SDK will automatically reconnect at this time. After successful reconnection, you will receive a callback notification from OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} locally.  If the user is set to visiable, the remote user will receive OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} callback notification. 
         */
        int JoinRoom(string token, UserInfo info, MultiRoomConfig roomConfig);

        /** {zh}
         * @type api
         * @brief 离开房间。  <br>
         *        用户调用此方法离开房间，结束通话过程，释放所有通话相关的资源。  <br>  
         * @notes <br>
         *      + 调用 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 方法加入房间后，必须调用此方法结束通话，否则无法开始下一次通话。无论当前是否在房间内，都可以调用此方法。重复调用此方法没有负面影响。  
         *      + 此方法是异步操作，调用返回时并没有真正退出房间。真正退出房间后，本地会收到 OnLeaveRoomEventHandler{@link #EventHandler#OnLeaveRoomEventHandler} 回调通知。  
         *      + 调用 SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} 将自身设为可见的用户离开房间后，房间内其他用户会收到 OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler} 回调通知。  
         *      + 如果调用此方法后立即销毁引擎，SDK 将无法触发 OnLeaveRoomEventHandler{@link #EventHandler#OnLeaveRoomEventHandler} 回调。  
         */
        /** {en}
         * @type api
         * @brief Leave the room.   <br>
         *        The user calls this method to leave the room, end the call process, and release all call-related resources. <br>
         *         After calling the JoinRoom{@link #IRTCVideoRoom#JoinRoom} method to join the room, you must call this method to end the call, otherwise you cannot start the next call. This method can be called regardless of whether it is currently in the room. Repeated calls to this method have no negative impact. <br>
         *        This method is an asynchronous operation, and the call returns without actually exiting the room. After you actually exit the room, you will receive a callback notification from OnLeaveRoomEventHandler{@link #EventHandler#OnLeaveRoomEventHandler} locally. <br>
         * @notes   <br>
         *        + After a user who calls SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} to make himself visible leaves the room, other users in the room will receive a callback notification from OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler}. <br>
         *        + If the engine is Destroyed immediately after this method is called, the SDK will not be able to trigger the OnLeaveRoomEventHandler{@link #EventHandler#OnLeaveRoomEventHandler} callback. <br>
         */
        void LeaveRoom();

        /** {zh}
         * @type api
         * @brief 更新 Token。
         *        收到 OnTokenWillExpireEventHandler{@link #EventHandler#OnTokenWillExpireEventHandler} 时，你必须重新获取 Token，并调用此方法更新 Token，以保证通话的正常进行。
         * @param token 重新获取的有效 Token。
         *        如果 Token 无效，你会收到 OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler}，错误码是 `-1010`。
         */
        /** {en}
         * @type api
         * @brief Update Token.
         *        You must call this API to update token to ensure the RTC call to continue when you receive OnTokenWillExpireEventHandler{@link #EventHandler#OnTokenWillExpireEventHandler}.
         * @param token  Valid token.
         *        If the Token is invalid, you will receive OnRoomStateChangedEventHandler{@link #EventHandler#OnRoomStateChangedEventHandler} with the error code of `-1010`.
         */
        void UpdateToken(string token);

        /** {zh}
         * @type api
         * @brief 设置用户可见性。默认可见。  <br>
         * @param enable 设置用户是否对房间内其他用户可见：  <br>
         *        + true: 可以在房间内发布音视频流，房间中的其他用户将收到用户的行为通知，例如进房、开启视频采集和退房。  <br>
         *        + false: 不可以在房间内发布音视频流，房间中的其他用户不会收到用户的行为通知，例如进房、开启视频采集和退房。
         * @notes  <br>
         *       + 该方法在加入房间前后均可调用。 <br>
         *       + 在房间内调用此方法，房间内其他用户会收到相应的回调通知：<br>
         *            - 从 false 切换至 true 时，房间内其他用户会收到 OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} 回调通知；  <br>
         *            - 从 true 切换至 false 时，房间内其他用户会收到 OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler} 回调通知。  <br>
         *       + 若调用该方法将可见性设为 false，此时尝试发布流会收到 `WARNING_CODE_PUBLISH_STREAM_FORBIDEN` 警告。
         */
        /** {en}
         * @type api
         * @brief Set the visibility of the user in the room. Visible by default.   <br>
         * @param enable  Visibility of the user in the room: <br>
         *         + True: The user can publish media streams. And the other users in the room get informed of the behaviors of the user, such as Joining room, starting video capture, and Leaving room.<br>
         *         + False: The user cannot publish media streams. And the other users in the room do not get informed of the behaviors of the user, such as joining, starting video capture, or leaving.<br>
         * @notes   <br>
         *        + You can call this API whether the user is in a room or not. <br>
         *        + When you call this API, the other users in the room will be informed via the related callback: <br>
         *            - Switch from `false` to `true`: OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler}<br>
         *            - Switch from `true` to `false`: OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler} <br>
         *        + The invisible user will receive the warning code, `WARNING_CODE_PUBLISH_STREAM_FORBIDEN`, when trying to publish media streams.
         */
        void SetUserVisibility(bool enable);
        
        /** {zh}
         * @type api
         * @brief 在当前所在房间内发布本地通过摄像头/麦克风采集的媒体流
         * @param type 媒体流类型，用于指定发布音频/视频，参看 MediaStreamType{@link #MediaStreamType}
         * @notes <br>
         *        + 调用 SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} 方法将自身设置为不可见后无法调用该方法，需将自身切换至可见后方可调用该方法发布摄像头音视频流。 <br>
         *        + 如果你需要发布屏幕共享流，调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen}。<br>
         *        + 调用此方法后，房间中的所有远端用户会收到 OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler} 回调通知，其中成功收到了音频流的远端用户会收到 OnFirstRemoteAudioFrameEventHandler{@link #EventHandler#OnFirstRemoteAudioFrameEventHandler} 回调，订阅了视频流的远端用户会收到 OnFirstRemoteVideoFrameDecodedEventHandler{@link #EventHandler#OnFirstRemoteVideoFrameDecodedEventHandler} 回调。<br>
         *        + 调用 UnpublishStream{@link #IRTCVideoRoom#UnpublishStream} 取消发布。
         */
        /** {en}
         * @type api
         * @brief Publishes media streams captured by camera/microphone in the current room.
         * @param type Media stream type, used for specifying whether to publish audio stream or video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *        + An invisible user cannot publish media streams. Call SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} to change your visibility in the room.  <br>
         *        + Call PublishScreen{@link #IRTCVideoRoom#PublishScreen} to start screen sharing.  <br>
         *        + After you call this API, the other users in the room will receive OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler}. Those who successfully received your streams will receive OnFirstRemoteAudioFrameEventHandler{@link #EventHandler#OnFirstRemoteAudioFrameEventHandler}/OnFirstRemoteVideoFrameDecodedEventHandler{@link #EventHandler#OnFirstRemoteVideoFrameDecodedEventHandler} at the same time.  <br>
         *        + Call UnpublishStream{@link #IRTCVideoRoom#UnpublishStream} to stop publishing streams.
         */
        void PublishStream(MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 停止将本地摄像头/麦克风采集的媒体流发布到当前所在房间中
         * @param type 媒体流类型，用于指定停止发布音频/视频，参看 MediaStreamType{@link #MediaStreamType}
         * @notes  <br>
         *        + 调用 PublishStream{@link #IRTCVideoRoom#PublishStream} 手动发布摄像头音视频流后，你需调用此接口停止发布。<br>
         *        + 调用此方法停止发布音视频流后，房间中的其他用户将会收到 OnUserUnPublishStreamEventHandler{@link #EventHandler#OnUserUnPublishStreamEventHandler} 回调通知。
         */
        /** {en}
         * @type api
         * @brief Stops publishing media streams captured by camera/microphone in the current room.
         * @param type Media stream type, used for specifying whether to stop publishing audio stream or video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *         + After calling PublishStream{@link #IRTCVideoRoom#PublishStream}, call this API to stop publishing streams. <br>
         *         + After calling this API, the other users in the room will receive OnUserUnPublishStreamEventHandler{@link #EventHandler#OnUserUnPublishStreamEventHandler}
         */
        void UnpublishStream(MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 在当前所在房间内发布本地屏幕共享音视频流
         * @param type 媒体流类型，用于指定发布屏幕音频/视频，参看 MediaStreamType{@link #MediaStreamType}。
         * @notes <br>
         *        + 调用 SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} 方法将自身设置为不可见后无法调用该方法，需将自身切换至可见后方可调用该方法发布屏幕流。 <br>
         *        + 调用该方法后，房间中的所有远端用户会收到 OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler} 回调，其中成功收到音频流的远端用户会收到 OnFirstRemoteAudioFrameEventHandler{@link #EventHandler#OnFirstRemoteAudioFrameEventHandler} 回调，订阅了视频流的远端用户会收到 OnFirstRemoteVideoFrameDecodedEventHandler{@link #EventHandler#OnFirstRemoteVideoFrameDecodedEventHandler} 回调。<br>
         *        + 调用 UnpublishScreen{@link #IRTCVideoRoom#UnpublishScreen} 取消发布。
         */
        /** {en}
         * @type api
         * @brief Publishes local screen sharing streams in the current room.
         * @param type Media stream type, used for specifying whether to publish audio stream or video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *         + An invisible user cannot publish media streams. Call SetUserVisibility{@link #IRTCVideoRoom#SetUserVisibility} to change your visibility in the room. <br>
         *         + After you called this API, the other users in the room will receive OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler}. Those who successfully received your streams will receive OnFirstRemoteAudioFrameEventHandler{@link #EventHandler#OnFirstRemoteAudioFrameEventHandler}/OnFirstRemoteVideoFrameDecodedEventHandler{@link #EventHandler#OnFirstRemoteVideoFrameDecodedEventHandler} at the same time.  <br>
         *         + Call UnpublishScreen{@link #IRTCVideoRoom#UnpublishScreen} to stop publishing screen sharing streams.
         */
        void PublishScreen(MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 停止将本地屏幕共享音视频流发布到当前所在房间中
         * @param type 媒体流类型，用于指定停止发布屏幕音频/视频，参看 MediaStreamType{@link #MediaStreamType}
         * @notes <br>
         *        + 调用 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 发布屏幕流后，你需调用此接口停止发布。 <br>
         *        + 调用此方法停止发布屏幕音视频流后，房间中的其他用户将会收到 OnUserUnPublishScreenEventHandler{@link #EventHandler#OnUserUnPublishScreenEventHandler} 回调。
         */
        /** {en}
         * @type api
         * @brief Stops publishing local screen sharing streams in the current room.
         * @param type Media stream type, used for specifying whether to publish audio stream or video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *         + After calling PublishScreen{@link #IRTCVideoRoom#PublishScreen}, call this API to stop publishing streams. <br>
         *         + After calling this API, the other users in the room will receive OnUserUnPublishScreenEventHandler{@link #EventHandler#OnUserUnPublishScreenEventHandler}.
         */
        void UnpublishScreen(MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 订阅房间内指定的通过摄像头/麦克风采集的媒体流，或更新对指定远端用户的订阅选项。
         * @param userID 指定订阅的远端发布音视频流的用户 ID。
         * @param type 媒体流类型，用于指定订阅音频/视频。参看 MediaStreamType{@link #MediaStreamType}。
         * @notes <br>
         *        + 当调用本接口时，当前用户已经订阅该远端用户，不论是通过手动订阅还是自动订阅，都将根据本次传入的参数，更新订阅配置。<br>
         *        + 你必须先通过 OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler} 回调获取当前房间里的远端摄像头音视频流信息，然后调用本方法按需订阅。  <br>
         *        + 调用该方法后，你会收到 OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler} 通知方法调用结果。  <br>
         *        + 成功订阅远端用户的媒体流后，订阅关系将持续到调用 UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream} 取消订阅或本端用户退房。 <br>
         *        + 关于其他调用异常，你会收到 OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler} 回调通知，具体异常原因参看 ErrorCode{@link #ErrorCode}。
         */
        /** {en}
         * @type api
         * @brief Subscribes to specific remote media streams captured by camera/microphone.  Or update the options of the subscribed user.
         * @param userID ID of the remote user who published the target audio/video stream.
         * @param type Media stream type, used for specifying whether to subscribe to the audio stream or the video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *        + Calling this API to update the subscribe configuration when the user has subscribed the remote user either by calling this API or by auto-subscribe.  <br>
         *        + You must first get the remote stream information through OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler} before calling this API to subscribe to streams accordingly.  <br>
         *        + After calling this API, you will be informed of the calling result with OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler}.  <br>
         *        + Once the local user subscribes to the stream of a remote user, the subscription to the remote user will sustain until the local user leaves the room or unsubscribe from it by calling UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream}.<br>
         *        + Any other exceptions will be included in OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler}, see ErrorCode{@link #ErrorCode} for the reasons.
         */
        void SubscribeStream(string userID, MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 取消订阅房间内指定的通过摄像头/麦克风采集的媒体流。  <br>
         *        该方法对自动订阅和手动订阅模式均适用。
         * @param userID 指定取消订阅的远端发布音视频流的用户 ID。
         * @param type 媒体流类型，用于指定取消订阅音频/视频。参看 MediaStreamType{@link #MediaStreamType}。
         * @notes  <br>
         *        + 调用该方法后，你会收到 OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler} 通知方法调用结果。  <br>
         *        + 关于其他调用异常，你会收到 OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler} 回调通知，具体失败原因参看 ErrorCode{@link #ErrorCode}。
         */
        /** {en}
         * @type api
         * @brief Unsubscribes from specific remote media streams captured by camera/microphone.  <br>
         *        You can call this API in both automatic subscription mode and manual subscription mode.
         * @param userID The ID of the remote user who published the target audio/video stream.
         * @param type Media stream type, used for specifying whether to unsubscribe from the audio stream or the video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *        + After calling this API, you will be informed of the calling result with OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler}.  <br>
         *        + Any other exceptions will be included in OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler}, see ErrorCode{@link #ErrorCode} for the reasons.
         */
        void UnsubscribeStream(string userID, MediaStreamType type);

        int StartForwardStreamToRooms(ForwardStreamConfiguration configuration);

    /** {zh}
     * @type api
     * @region 多房间
     * @author shenpengliang
     * @brief 停止跨房间媒体流转发。
     *        通过 StartForwardStreamToRooms{@link #IRTCVideoRoom#StartForwardStreamToRooms} 发起媒体流转发后，可调用本方法停止向所有目标房间转发媒体流。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     * @notes 
     *        + 调用本方法后，将在本端触发 OnForwardStreamStateChangedEventHandler{@link #EventHandler#OnForwardStreamStateChangedEventHandler} 回调。
     *        + 调用本方法后，原目标房间中的用户将接收到本地用户停止发布 OnUserUnpublishStreamEventHandler{@link #EventHandler#OnUserUnPublishStreamEventHandler}/OnUserUnPublishScreenEventHandler{@link #EventHandler#OnUserUnPublishScreenEventHandler} 和退房 OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler} 的回调。
     *        + 如果需要停止向指定的房间转发媒体流，请调用 UpdateForwardStreamToRooms{@link #IRTCVideoRoom#UpdateForwardStreamToRooms} 更新房间信息。
     *        + 如果需要暂停转发，请调用 PauseForwardStreamToAllRooms{@link #IRTCVideoRoom#PauseForwardStreamToAllRooms}，并在之后随时调用 ResumeForwardStreamToAllRooms{@link #IRTCVideoRoom#ResumeForwardStreamToAllRooms} 快速恢复转发。
     */
    /** {en}
     * @type api
     * @region Multi-room
     * @author shenpengliang
     * @brief Call to this method to stop relaying media stream to all rooms after calling StartForwardStreamToRooms{@link #IRTCVideoRoom#StartForwardStreamToRooms}. 
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     * @notes 
     *        + Call this method will trigger OnForwardStreamStateChangedEventHandler{@link #EventHandler#OnForwardStreamStateChangedEventHandler}.
     *        + The other users in the room will receive callback of OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} and OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler}/OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler} when you stop relaying.
     *        + To stop relaying media stream to specific rooms, call UpdateForwardStreamToRooms{@link #IRTCVideoRoom#UpdateForwardStreamToRooms} instead.
     *        + To resume the relaying in a short time, call PauseForwardStreamToAllRooms{@link #IRTCVideoRoom#PauseForwardStreamToAllRooms} instead and then call ResumeForwardStreamToAllRooms{@link #IRTCVideoRoom#ResumeForwardStreamToAllRooms} to recsume after that.
     */
        int StopForwardStreamToRooms();
        int UpdateForwardStreamToRooms(ForwardStreamConfiguration configuration);

    /** {zh}
     * @type api
     * @region 多房间
     * @author shenpengliang
     * @brief 暂停跨房间媒体流转发。
     *        通过 StartForwardStreamToRooms{@link #IRTCVideoRoom#StartForwardStreamToRooms} 发起媒体流转发后，可调用本方法暂停向所有目标房间转发媒体流。
     *        调用本方法暂停向所有目标房间转发后，你可以随时调用 ResumeForwardStreamToAllRooms{@link #IRTCVideoRoom#ResumeForwardStreamToAllRooms} 快速恢复转发。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     * @notes 调用本方法后，目标房间中的用户将接收到本地用户停止发布 OnUserUnpublishStreamEventHandler{@link #EventHandler#OnUserUnPublishStreamEventHandler}/OnUserUnPublishScreenEventHandler{@link #EventHandler#OnUserUnPublishScreenEventHandler} 和退房 OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler} 的回调。
     */
     /** {en}
     * @type api
     * @region Multi-room
     * @author shenpengliang
     * @brief Call this method to pause relaying media stream to all rooms after calling StartForwardStreamToRooms{@link #IRTCVideoRoom#StartForwardStreamToRooms}. 
     *        After that, call ResumeForwardStreamToAllRooms{@link #IRTCVideoRoom#ResumeForwardStreamToAllRooms} to resume.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     * @notes The other users in the room will receive callback of OnUserUnpublishStreamEventHandler{@link #EventHandler#OnUserUnPublishStreamEventHandler}/OnUserUnPublishScreenEventHandler{@link #EventHandler#OnUserUnPublishScreenEventHandler} and OnUserLeaveEventHandler{@link #EventHandler#OnUserLeaveEventHandler} when you pause relaying.
     */
        int PauseForwardStreamToAllRooms();

    /** {zh}
     * @type api
     * @region 多房间
     * @author shenpengliang
     * @brief 恢复跨房间媒体流转发。
     *        调用 PauseForwardStreamToAllRooms{@link #IRTCVideoRoom#PauseForwardStreamToAllRooms} 暂停转发之后，调用本方法恢复向所有目标房间转发媒体流。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     * @notes 
     *        目标房间中的用户将接收到本地用户进房 OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} 和发布 OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler}/OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler} 的回调。
     */
    /** {en}
     * @type api
     * @region Multi-room
     * @author shenpengliang
     * @brief Call this method to resume relaying to all rooms from the pause by calling PauseForwardStreamToAllRooms{@link #IRTCVideoRoom#PauseForwardStreamToAllRooms}.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     * @notes The other users in the room will receive callback of OnUserJoinedEventHandler{@link #EventHandler#OnUserJoinedEventHandler} and OnUserPublishStreamEventHandler{@link #EventHandler#OnUserPublishStreamEventHandler}/OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler} when you resume relaying.
     */
        int ResumeForwardStreamToAllRooms();

    /** {zh}
     * @type api
     * @region 多房间
     * @author wangzhanqiang
     * @brief 设置发流端音画同步。  
     *        当同一用户同时使用两个通话设备分别采集发送音频和视频时，有可能会因两个设备所处的网络环境不一致而导致发布的流不同步，此时你可以在视频发送端调用该接口，SDK 会根据音频流的时间戳自动校准视频流，以保证接收端听到音频和看到视频在时间上的同步性。
     * @param audio_user_id 音频发送端的用户 ID，将该参数设为空则可解除当前音视频的同步关系。
     * @return  
     *        + 0: 调用成功。
     *        + < 0 : 调用失败。
     * @notes 
     *        + 该方法在进房前后均可调用。  
     *        + 进行音画同步的音频发布用户 ID 和视频发布用户 ID 须在同一个 RTC 房间内。  
     *        + 调用该接口后音画同步状态发生改变时，你会收到 OnAVSyncStateChangeCallback{@link #EventHandler#OnAVSyncStateChangeCallback} 回调。  
     *        + 同一 RTC 房间内允许存在多个音视频同步关系，但需注意单个音频源不支持与多个视频源同时同步。  
     *        + 如需更换同步音频源，再次调用该接口传入新的 `audio_user_id` 即可；如需更换同步视频源，需先解除当前的同步关系，后在新视频源端开启同步。
     */
    /** {en}
     * @type api
     * @region Multi-room
     * @author wangzhanqiang
     * @brief Synchronizes published audio and video.  
     *        When the same user simultaneously uses separate devices to capture and publish audio and video, there is a possibility that the streams are out of sync due to the network disparity. In this case, you can call this API on the video publisher side and the SDK will automatically line the video stream up according to the timestamp of the audio stream, ensuring that the audio the receiver hears corresponds to the video the receiver watches.
     * @param audio_user_id The ID of audio publisher. You can stop the current A/V synchronization by setting this parameter to null.
     * @return  
     *        + 0: Success.
     *        + < 0 : Fail. 
     * @notes  
     *        + You can call this API anytime before or after entering the room.  
     *        + The source user IDs of the audio and video stream to be synchronized must be in the same RTC room.  
     *        + When the A/V synchronization state changes, you will receive OnAVSyncStateChangeCallback{@link #EventHandler#OnAVSyncStateChangeCallback}.  
     *        + More than one pair of audio and video can be synchronized simultaneously in the same RTC room, but you should note that one single audio source cannot be synchronized with multiple video sources at the same time.  
     *        + If you want to change the audio source, call this API again with a new `audio_user_id`. If you want to change the video source, you need to stop the current synchronization first, then call this API on the new video publisher side.
     */
        int SetMultiDeviceAVSync(string audio_user_id);
     
        /** {zh}
         * @type api
         * @brief 订阅房间内指定的远端屏幕共享音视频流，或更新对指定远端用户的订阅选项
         * @param userID 指定订阅的远端发布屏幕流的用户 ID。
         * @param type 媒体流类型，用于指定订阅音频/视频。参看 MediaStreamType{@link #MediaStreamType}。
         * @notes  <br>
         *        + 当调用本接口时，当前用户已经订阅该远端用户，不论是通过手动订阅还是自动订阅，都将根据本次传入的参数，更新订阅配置。<br>
         *        + 你必须先通过 OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler} 回调获取当前房间里的远端屏幕流信息，然后调用本方法按需订阅。  <br>
         *        + 调用该方法后，你会收到 OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler} 通知方法调用结果。  <br>
         *        + 成功订阅远端用户的媒体流后，订阅关系将持续到调用 UnsubscribeScreen{@link #IRTCVideoRoom#UnsubscribeScreen} 取消订阅或本端用户退房。 <br>
         *        + 关于其他调用异常，你会收到 OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler} 回调通知，具体异常原因参看 ErrorCode{@link #ErrorCode}。
         */
        /** {en}
         * @type api
         * @brief Subscribes to specific screen sharing media stream.  Or update the subscribe options of the subscribed user.
         * @param userID The ID of the remote user who published the target screen audio/video stream.
         * @param type Media stream type, used for specifying whether to subscribe to the audio stream or the video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *        + Calling this API to update the subscribe configuration when the user has subscribed the remote user either by calling this API or by auto-subscribe.  <br>
         *        + You must first get the remote stream information through OnUserPublishScreenEventHandler{@link #EventHandler#OnUserPublishScreenEventHandler} before calling this API to subscribe to streams accordingly.  <br>
         *        + After calling this API, you will be informed of the calling result with OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler}.  <br>
         *        + Once the local user subscribes to the stream of a remote user, the subscription to the remote user will sustain until the local user leaves the room or unsubscribe from it by calling UnsubscribeScreen{@link #IRTCVideoRoom#UnsubscribeScreen}.<br>
         *        + Any other exceptions will be included in OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler}, see ErrorCode{@link #ErrorCode} for the reasons.
         */
        void SubscribeScreen(string userID, MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 取消订阅房间内指定的远端屏幕共享音视频流。
         *        该方法对自动订阅和手动订阅模式均适用。
         * @param userID 指定取消订阅的远端发布屏幕流的用户 ID。
         * @param type 媒体流类型，用于指定取消订阅音频/视频。参看 MediaStreamType{@link #MediaStreamType}。
         * @notes  <br>
         *        + 调用该方法后，你会收到 OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler} 通知流的退订结果。  <br>
         *        + 关于其他调用异常，你会收到 OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler} 回调通知，具体失败原因参看 ErrorCode{@link #ErrorCode}。
         */
        /** {en}
         * @type api
         * @brief Unsubscribes from specific screen sharing media stream.
         *        You can call this API in both automatic subscription mode and manual subscription mode.
         * @param userID The ID of the remote user who published the target screen audio/video stream.
         * @param type Media stream type, used for specifying whether to unsubscribe from the audio stream or the video stream. See MediaStreamType{@link #MediaStreamType}.
         * @notes  <br>
         *        + After calling this API, you will be informed of the calling result with OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler}.  <br>
         *        + Any other exceptions will be included in OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler}, see ErrorCode{@link #ErrorCode} for the reasons.
         */
        void UnsubscribeScreen(string userID, MediaStreamType type);

        /** {zh}
         * @type ap
         * @brief 暂停接收来自远端的媒体流。
         * @param mediaType 媒体流类型，指定需要暂停接收音频还是视频流，参看 PauseResumeControlMediaType{@link #PauseResumeControlMediaType}。
         * @notes  <br>
         *        + 该方法仅暂停远端流的接收，并不影响远端流的采集和发送；  <br>
         *        + 该方法不改变用户的订阅状态以及订阅流的属性。  <br>
         *        + 若想恢复接收远端流，需调用 ResumeAllSubscribedStream{@link #IRTCVideoRoom#ResumeAllSubscribedStream}。
         */
        /** {en}
         * @type api
         * @brief Pause receiving remote media streams.
         * @param mediaType Media stream type subscribed to. Refer to PauseResumeControlMediaType{@link #PauseResumeControlMediaType}.
         * @notes   <br>
         *         + Calling this API does not change the capture state and the transmission state of the remote clients. <br>
         *         + Calling this API does not cancel the subscription or change any subscription configuration. <br>
         *         + To resume, call ResumeAllSubscribedStream{@link #IRTCVideoRoom#ResumeAllSubscribedStream}.
         */
        void PauseAllSubscribedStream(PauseResumeControlMediaType mediaType);

        /** {zh}
         * @type api
         * @brief 恢复接收来自远端的媒体流
         * @param mediaType 媒体流类型，指定需要暂停接收音频还是视频流，参看 PauseResumeControlMediaType{@link #PauseResumeControlMediaType}
         * @notes <br>
         *        + 该方法仅恢复远端流的接收，并不影响远端流的采集和发送；  <br>
         *        + 该方法不改变用户的订阅状态以及订阅流的属性。
         */
        /** {en}
         * @type api
         * @brief Resume receiving remote media streams
         * @param mediaType Media stream type subscribed to. Refer to PauseResumeControlMediaType{@link #PauseResumeControlMediaType}
         * @notes  <br>
         *         + Calling this API does not change the capture state and the transmission state of the remote clients.
         *         + Calling this API does not change any subscription configuration.
         */
        void ResumeAllSubscribedStream(PauseResumeControlMediaType mediaType);

        /** {zh}
    * @type api
    * @region 房间管理
    * @author yejing.luna
    * @brief 订阅房间内所有通过摄像头/麦克风采集的媒体流，或更新订阅选项。
    * @param [in] type 媒体流类型，用于指定订阅音频/视频。参看 MediaStreamType{@link #MediaStreamType}。
    * @return  <br>
    *        0: 方法调用成功  <br>
    *        !0: 方法调用失败  <br>
    * @notes  <br>
    *        + 多次调用订阅接口时，将根据末次调用接口和传入的参数，更新订阅配置。<br>
    *        + 大会模式下，如果房间内的媒体流超过上限，建议通过调用 SubscribeStream{@link #IRTCVideoRoom#SubscribeStream} 逐一指定需要订阅的媒体流。<br>
    *        + 调用该方法后，你会收到 OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler} 通知方法调用结果。  <br>
    *        + 成功订阅远端用户的媒体流后，订阅关系将持续到调用 UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream} 取消订阅或本端用户退房。 <br>
    *        + 关于其他调用异常，你会收到 OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler} 回调通知，具体异常原因参看 ErrorCode{@link #ErrorCode}。
    */
        /** {en}
        * @type api
        * @region Room Management
        * @author yejing.luna
        * @brief Subscribes to all remote media streams captured by camera/microphone. Or update the subscribe options of all subscribed user.
        * @param [in] type Media stream type, used for specifying whether to subscribe to the audio stream or the video stream. See MediaStreamType{@link #MediaStreamType}.
        * @return API call result:  <br>
        *        + 0: Success  <br>
        *        + !0: Failure
        * @notes  <br>
        *        + If the subscription options conflict with the previous ones, they are subject to the configurations in the last call.<br>
        *        + In the Conference Mode, if the number of media streams exceeds the limit, we recommend you call SubscribeStream{@link #IRTCVideoRoom#SubscribeStream} to subscribe each target media stream other than calling this API.<br>
        *        + After calling this API, you will be informed of the calling result with OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler}.  <br>
        *        + Once the local user subscribes to the stream of a remote user, the subscription to the remote user will sustain until the local user leaves the room or unsubscribe from it by calling UnsubscribeStream{@link #IRTCVideoRoom#UnsubscribeStream}.<br>
        *        + Any other exceptions will be included in OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler}, see ErrorCode{@link #ErrorCode} for the reasons.
        */
         int SubscribeAllStreams(MediaStreamType type);

        /** {zh}
    * @type api
    * @region 房间管理
    * @author yejing.luna
    * @brief 取消订阅房间内所有的通过摄像头/麦克风采集的媒体流。  <br>
    *        自动订阅和手动订阅的流都可以通过本方法取消订阅。
    * @param [in] type 媒体流类型，用于指定取消订阅音频/视频。参看 MediaStreamType{@link #MediaStreamType}。
    * @return 方法调用结果：  <br>
    *        + 0：成功  <br>
    *        + !0：失败
    * @notes  <br>
    *        + 调用该方法后，你会收到 OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler} 通知方法调用结果。  <br>
    *        + 关于其他调用异常，你会收到 OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler} 回调通知，具体失败原因参看 ErrorCode{@link #ErrorCode}。
    */
        /** {en}
         * @type api
         * @region Room Management
         * @author yejing.luna
         * @brief Unsubscribes from all remote media streams captured by camera/microphone.  <br>
         *        You can call this API to unsubscribe from streams that are subscribed to either automatically or manually.
         * @param [in] type Media stream type, used for specifying whether to unsubscribe from the audio stream or the video stream. See MediaStreamType{@link #MediaStreamType}.
         * @return API call result:  <br>
         *        + 0: Success  <br>
         *        + !0: Failure
         * @notes  <br>
         *        + After calling this API, you will be informed of the calling result with OnStreamSubscribedEventHandler{@link #EventHandler#OnStreamSubscribedEventHandler}.  <br>
         *        + Any other exceptions will be included in OnStreamStateChangedEventHandler{@link #EventHandler#OnStreamStateChangedEventHandler}, see ErrorCode{@link #ErrorCode} for the reasons.
         */
         int UnsubscribeAllStreams(MediaStreamType type);

        /** {zh}
         * @type api
         * @brief 屏蔽指定远端用户的音频
         */
        /** {en}
         * @type api
         * @brief Block the audio of the specified remote user.
         */
      //  void MuteRemoteAudio(string userID, bool muted);

        /** {zh}
         * @type api
         * @brief 屏蔽指定远端用户的视频
         */
        /** {en}
         * @type api
         * @brief Block the video of the specified remote user.
         */
       // void MuteRemoteVideo(string userID, bool muted);

        /** {zh}
         * @type api
         * @brief 获取范围语音接口。
         * @return 范围语音接口，参看 IRangeAudio{@link #IRangeAudio}。
         */
        /** {en}
         * @type api
         * @brief Gets range audio interfaces.
         * @return Range audio interfaces. See IRangeAudio{@link #IRangeAudio}.
         */
		IRangeAudio GetRangeAudio();

        /** {zh}
         * @type api
         * @brief 获取空间音频接口。
         * @return 空间音频接口，参看 IRangeAudio{@link #IRangeAudio}。
         */
        /** {en}
         * @type api
         * @brief Gets spatial audio interfaces.
         * @return Spatial audio interfaces. See ISpatialAudio{@link #ISpatialAudio}.
         */
		ISpatialAudio GetSpatialAudio();

        /** {zh}
         * @type api
         * @brief 给房间内指定的用户发送点对点文本消息（P2P）。
         * @param userID 消息接收用户的 ID
         * @param message 发送的文本消息内容。消息不超过 64 KB。
         * @return <br>
         *        + >0：发送成功，返回这次发送消息的编号，从 1 开始递增  <br>
         *        + -1：发送失败，RTCRoom 实例未创建  <br>
         *        + -2：发送失败，userID 为空
         * @notes 在发送房间内文本消息前，必须先调用 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 加入房间。
         */
        /** {en}
         * @type api
         * @brief Send a point-to-point text message to the specified user in the room
         * @param userID User ID of the message receiver
         * @param message Text message content sent. Message does not exceed 64 KB.
         * @return  <br>
         *         + > 0: Sent successfully, return the number of the sent message, increment from 1 <br>
         *         + -1: Sent failed, RTCRoom instance not created <br>
         *         + -2: Sent failed, userID is empty
         * @notes Before sending an in-room text message, you must call JoinRoom{@link #IRTCVideoRoom#JoinRoom} to join the room.
         */
        long SendUserMessage(string userID, string message);

        /** {zh}
         * @type api
         * @brief 给房间内的所有其他用户群发文本消息。
         * @param message 发送的文本消息内容，消息不超过 64 KB。
         * @notes 在房间内广播文本消息前，必须先调用 JoinRoom{@link #IRTCVideoRoom#JoinRoom} 加入房间。
         */
        /** {en}
         * @type api
         * @brief Mass text messages to all other users in the room.
         * @param message The content of the text message sent. The message does not exceed 64 KB.
         * @notes Before broadcasting a text message in the room, you must call JoinRoom{@link #IRTCVideoRoom#JoinRoom} to join the room.
         */
        long SendRoomMessage(string message);

    }
    /** {zh}
     * @type keytype
     * @brief 用户加入房间的类型。
     */
    /** {en}
     * @type keytype
     * @brief The type of joining the room.
     */
    public enum JoinRoomType {
        /** {zh}
        * @brief 首次加入房间。用户手动调用 `JoinRoom` 加入房间。
        */
        /** {en}
        * @brief Join the room for the first time. The user calls `JoinRoom` to join the room. <br>
        */
        kJoinRoomTypeFirst = 0,
        /** {zh}
         * @brief 重新加入房间。用户网络较差，失去与服务器的连接，进行重连时收到加入成功。  <br>
         */
        /** {en}
         * @brief Rejoin the room. The user loses connection to the server due to poor network, and rejoins the room successfully.   <br>
         */
        kJoinRoomTypeReconnected = 1,
    }

    /** {zh}
     * @type keytype
     * @brief 通话相关的统计信息
     */
    /** {en}
     * @type keytype
     * @brief  Call related statistics
     */
    public struct RtcRoomStats {
        /** {zh}
         * @brief 当前应用的上行丢包率，取值范围为 [0, 1]
         */
        /** {en}
         * @brief The uplink packet loss rate currently applied, the value range is  [0,1]
         */
        public float txLostrate;
        /** {zh}
         * @brief 当前应用的下行丢包率，取值范围为 [0, 1]
         */
        /** {en}
         * @brief The downlink packet loss rate of the current application, the value range is  [0,1]
         */
        public float rxLostrate;
        /** {zh}
         * @brief 客户端到服务端数据传输的往返时延（单位 ms）
         */
        /** {en}
         * @brief Round-trip time (in ms) for client side to server level data transfers
         */
        public int rtt;
        /** {zh}
         * @brief 本地用户在本次通话中的参与时长，单位为 s
         */
        /** {en}
         * @brief The duration, in seconds, that the local user has spent in the room.
         */
        public uint duration;
        /** {zh}
         * @brief 本地用户的总发送字节数 (bytes)，累计值
         */
        /** {en}
         * @brief The total number of bytes sent by the local user  (bytes), the cumulative value
         */
        public uint tx_bytes;
        /** {zh}
         * @brief 本地用户的总接收字节数 (bytes)，累计值
         */
        /** {en}
         * @brief The total number of bytes received by the local user  (bytes), the cumulative value
         */
        public uint rx_bytes;
        /** {zh}
         * @brief 发送码率（kbps），获取该数据时的瞬时值
         */
        /** {en}
         * @brief Send bit rate (kbps), get the instantaneous value of the data
         */
        public ushort tx_kbitrate;
        /** {zh}
         * @brief 接收码率（kbps），获取该数据时的瞬时值
        */
        /** {en}
         * @brief Receive bit rate (kbps), get the instantaneous value of the data
         */
        public ushort rx_kbitrate;
        /** {zh}
         * @brief 音频包的发送码率（kbps），获取该数据时的瞬时值
         */
        /** {en}
         * @brief The transmission rate (kbps) of the audio packet, the instantaneous value when obtaining the data
         */
        public ushort rx_audio_kbitrate;
        /** {zh}
         * @brief 音频接收码率（kbps），获取该数据时的瞬时值
         */
        /** {en}
         * @brief Audio receiving bit rate (kbps), obtaining the instantaneous value of the data
         */
        public ushort tx_audio_kbitrate;
        /** {zh}
         * @brief 视频发送码率（kbps），获取该数据时的瞬时值
         */
        /** {en}
         * @brief Video transmission rate (kbps), the instantaneous value when obtaining the data
         */
        public ushort rx_video_kbitrate;
        /** {zh}
         * @brief 视频接收码率（kbps），获取该数据时的瞬时值
         */
        /** {en}
         * @brief Video receiving bit rate (kbps), the instantaneous value when obtaining the data
         */
        public ushort tx_video_kbitrate;
        /** {zh}
         * @brief 当前房间内的可见用户人数。
         */
        /** {en}
         * @brief Number of visible users in the current room
         */
        public uint user_count;
        /** {zh}
         * @brief 当前应用的 CPU 使用率 (%)
         */
        /** {en}
         * @brief Current application CPU usage (%)
         */
        public double cpu_app_usage;
        /** {zh}
         * @brief 当前系统的 CPU 使用率 (%)
         */
        /** {en}
         * @brief Current system CPU usage (%)
         */
        public double cpu_total_usage;
        /** {zh}
         * @brief 系统上行网络抖动（ms）
         */
        /** {en}
         * @brief Tx jitter(ms)
         */
        public int tx_jitter;
        /** {zh}
         * @brief 系统下行网络抖动（ms）
         */
        /** {en}
         * @brief Rx jitter(ms)
         */
        public int rx_jitter;
    }

    /** {zh}
     * @type keytype
     * @brief 手动订阅流的配置信息。  <br>
     */
    /** {en}
     * @type keytype
     * @brief  Configuration information for manual subscription flows.   <br>
     */
    public struct SubscribeConfig {
        /** {zh}
         * @brief 是否是屏幕流（默认为否）。  <br>
         */
        /** {en}
         * @brief Whether it is a screen stream (default is no).   <br>
         */
        public bool is_screen;
        /** {zh}
         * @brief 是否订阅视频。  <br>
         */
        /** {en}
         * @brief Whether to subscribe to videos.   <br>
         */
        public bool sub_video;
        /** {zh}
         * @brief 是否订阅音频。  <br>
         */
        /** {en}
         * @brief Whether to subscribe to audio.   <br>
         */
        public bool sub_audio;
        /** {zh}
         * @brief 订阅的视频流分辨率下标，默认值为 0。  <br>
         */
        /** {en}
         * @brief Subscribed video stream resolution subscript. The default value is 0. <br>
         */
        public int video_index;
        /** {zh}
         * @brief 远端用户的需求优先级.  <br>
         *        + 0: 用户优先级为低，默认值。
         *        + 100: 用户优先级为正常。
         *        + 200: 用户优先级为高。
         */
        /** {en}
         * @brief For the remote user's requirement priority.
         *        + 0: Low, the default.
         *        + 100: Medium.
         *        + 200: High.
         */
        public int priority;
        /** {zh}
         * @brief 远端用户的时域分层。  <br>
         *        仅码流支持SVC特性时可以生效。  <br>
         *        + 0: 不指定分层(默认值）
         *        + 1: T0 层
         *        + 2: T0+T1 层
         *        + 3: T0+T1+T2 层
         */
        /** {en}
         * @brief The time domain hierarchy of the remote user. <br>
         *        This only works if the stream supports the SVC feature. <br>
         *        + 0: No hierarchy specified (default)
         *        + 1: T0 layer
         *        + 2: T0 + T1 layer
         *        + 3: T0 + T1 + T2 layer
         */
        public int svc_layer;
        /** {zh}
         * @brief 期望订阅的最高帧率，单位：fps，默认值为 0，设为大于 0 的值时开始生效。  <br>
         *        当发布端帧率低于设定帧率，或订阅端开启性能回退后下行弱网，则帧率会相应下降。  <br>
         *        仅码流支持 SVC 分级编码特性时方可生效。
         */
        /** {en}
         * @brief Expected maximum frame rate of the subscribed stream in px. The default value is 0, values greater than 10 are valid.  <br>
         *        If the frame rate of the stream published is lower than the value you set, or if your subscribed stream falls back under limited network conditions, the frame rate you set will drop accordingly.  <br>
         *        Only valid if the stream is coded with SVC technique.
         */
        public int framerate;
        /** {zh}
         * @brief 用户通过指定UI对应的最合适的流的宽度，单位：px
         */
        /** {en}
         * @brief The user specifies the width(px) of the most appropriate stream corresponding to the UI by specifying
         */
        public int sub_width;
        /** {zh}
         * @brief 用户通过指定UI对应的最合适的流的高度，单位：px
         */
        /** {en}
         * @brief The user specifies the height(px) of the most appropriate stream corresponding to the UI by specifying
         */
        public int sub_height;
    }

    /** {zh}
     * @type keytype
     * @brief 用户信息
     */
    /** {en}
     * @type keytype
     * @brief  User information
     */
    public struct UserInfo {
        /** {zh}
         * @brief 用户 ID。该字符串符合正则表达式：`[a-zA-Z0-9_@\-]{1,128}`。
         */
        /** {en}
         * @brief User ID. The string matches the regular expression: `[a-zA-Z0-9_@\-]{1,128}`.
         */
        public string UserID;
        /** {zh}
         * @brief 用户的额外信息，最大长度为 200 字节。会在 `OnUserJoined` 中回调给远端用户。
         */
        /** {en}
         * @brief Additional information of the user. The maximum length is 200 bytes. The remote user will receive the info in `OnUserJoined`.
         */
        public string ExtraInfo;
    }

    /** {zh}
     * @type keytype
     * @brief 用户离开房间的原因。
     */
    /** {en}
     * @type keytype
     * @brief  The reason why the user left the room. 
     */
    public enum UserOfflineReason {
        /** {zh}
         * @brief 远端用户调用 LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} 方法主动退出房间。  <br>
         */
        /** {en}
         * @brief The remote user invokes the LeaveRoom{@link #IRTCVideoRoom#LeaveRoom} method to actively exit the room. <br>
         */
        kUserOfflineReasonQuit = 0,
        /** {zh}
         * @brief 远端用户因网络等原因掉线。  <br>
         */
        /** {en}
         * @brief The remote user is offline.   <br>
         */
        kUserOfflineReasonDropped = 1,
        /** {zh}
         * @brief 远端用户切换至隐身状态。  <br>
         */
        /** {en}
         * @brief The remote user switches to invisible.   <br>
         */
        kUserOfflineReasonSwitchToInvisible = 2,
    }

    /** {zh}
     * @type keytype
     * @brief 房间内远端流被移除的原因。  <br>
     */
    /** {en}
     * @type keytype
     * @brief The reason why the remote flow in the room was removed.   <br>
     */
    public enum StreamRemoveReason {
        /** {zh}
         * @brief 远端用户停止发布流。  <br>
         */
        /** {en}
         * @brief The remote user stops publishing the flow.   <br>
         */
        kStreamRemoveReasonUnpublish = 0,
        /** {zh}
         * @brief 远端用户发布流失败。  <br>
         */
        /** {en}
         * @brief The remote user failed to publish the flow.   <br>
         */
        kStreamRemoveReasonPublishFailed = 1,
        /** {zh}
         * @brief 保活失败。  <br>
         */
        /** {en}
         * @brief Keep alive failed.   <br>
         */
        kStreamRemoveReasonKeepLiveFailed = 2,
        /** {zh}
         * @brief 远端用户断网。  <br>
         */
        /** {en}
         * @brief The remote user is disconnected.   <br>
         */
        kStreamRemoveReasonClientDisconnected = 3,
        /** {zh}
         * @brief 远端用户重新发布流。  <br>
         */
        /** {en}
         * @brief The remote user republishes the flow.   <br>
         */
        kStreamRemoveReasonRepublish = 4,
        /** {zh}
         * @brief 其他原因。  <br>
         */
        /** {en}
         * @brief Other reasons.   <br>
         */
        kStreamRemoveReasonOther = 5
    }

    /** {zh}
     * @type keytype
     * @brief 订阅媒体流状态
     */
    /** {en}
     * @type keytype
     * @brief Subscription status of media streams
     */
    public enum SubscribeState {
        /** {zh}
         * @brief 订阅/取消订阅流成功
         */
        /** {en}
         * @brief Successfully changed the subscription status
         */
        kSubscribeStateSuccess,
        /** {zh}
         * @brief 订阅/取消订阅流失败，本地用户未在房间中
         */
        /** {en}
         * @brief Failed to change the subscription status, because you were not in the room.
         */
        kSubscribeStateFailedNotInRoom,
        /** {zh}
         * @brief 订阅/取消订阅流失败，房间内未找到指定的音视频流
         */
        /** {en}
         * @brief Failed to change the subscription status, because the target audio/video stream was not found.
         */
        kSubscribeStateFailedStreamNotFound,
        /** {zh}
         * @brief 超过订阅流数上限
         */
        /** {en}
         * @brief Failed to change the subscription status, because the number of streams you have subscribed to has exceeded the limit.
         */
        kSubscribeStateFailedOverLimit
    }

    /** {zh}
     * @type keytype
     * @brief 黑帧视频流状态
     */
    /** {en}
     * @type keytype
     * @brief State of the black frame video stream
     */
    public enum SEIStreamEventType {
        /** {zh}
         * @brief 远端用户发布黑帧视频流。  <br>
         *        纯语音通话场景下，远端用户调用 SendSEIMessage{@link #IRTCVideo#SendSEIMessage} 发送 SEI 数据时，SDK 会自动发布一路黑帧视频流，并触发该回调。
         */
        /** {en}
         * @brief A black frame video stream is published from the remote user.  <br>
         *        In a voice call, when the remote user calls SendSEIMessage{@link #IRTCVideo#SendSEIMessage} to send SEI data, SDK will automatically publish a black frame video stream, and trigger this callback.
         */
        kSEIStreamEventTypeStreamAdd = 0,
        /** {zh}
         * @brief 远端黑帧视频流移除。该回调的触发时机包括：  <br>
         *        + 远端用户开启摄像头采集，由语音通话切换至视频通话，黑帧视频流停止发布；  <br>
         *        + 远端用户调用 SendSEIMessage{@link #IRTCVideo#SendSEIMessage} 后 1min 内未有 SEI 数据发送，黑帧视频流停止发布；  <br>
         *        + 远端用户调用 SetVideoSourceType{@link #IRTCVideo#SetVideoSourceType} 切换至自定义视频采集时，黑帧视频流停止发布。
         */
        /** {en}
         * @brief The black frame video stream is removed. The timing this callback will be triggered is as following:  <br>
         *        + The remote user turns on their camera, switching from a voice call to a video call.  <br>
         *        + No SEI data is sent within 1min after the remote user calls SendSEIMessage{@link #IRTCVideo#SendSEIMessage}.  <br>
         *        + The black frame video stream stops when the remote user calls SetVideoSourceType{@link #IRTCVideo#SetVideoSourceType} to switch to custom video capture.
         */
        kSEIStreamEventTypeStreamRemove,
    };

    /** {zh}
     * @type keytype
     * @brief 远端流信息
     */
    /** {en}
     * @type keytype
     * @brief Information about the remote stream
     */
    public struct RemoteStreamKey {
        /** {zh}
         * @brief 媒体流所在房间的房间 ID。
         */
        /** {en}
         * @brief The room ID of the media stream.
         */
        public string RoomID;
        /** {zh}
         * @brief 用户 ID
         */
        /** {en}
         * @brief The ID of the user who published the stream.
         */
        public string UserID;
        /** {zh}
         * @brief 流属性，主流或屏幕流。参看 StreamIndex{@link #StreamIndex}
         */
        /** {en}
         * @brief Stream type. See StreamIndex{@link #StreamIndex}
         */
        public StreamIndex streamIndex;

        public static bool operator ==(RemoteStreamKey l, RemoteStreamKey r)
        {
            if (l.UserID == r.UserID && l.RoomID == r.RoomID && l.streamIndex == r.streamIndex)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(RemoteStreamKey l, RemoteStreamKey r)
        {
            return !(l == r);
        }
    };
    /** 
 * @type keytype
 * @brief 音视频同步状态
 */
    public enum AVSyncState
    {
        /** 
         * @brief 音视频开始同步
         */
        kAVSyncStateAVStreamSyncBegin = 0,
        /** 
         * @brief 音视频同步过程中音频移除，但不影响当前的同步关系
         */
        kAVSyncStateAudioStreamRemove,
        /** 
         * @brief 音视频同步过程中视频移除，但不影响当前的同步关系
         */
        kAVSyncStateVdieoStreamRemove,
        /** 
         * @hidden for internal use only
         * @brief 订阅端设置同步  <br>
         */
        kAVSyncStateSetAVSyncStreamId,
    };
    /** 
 * @type keytype
 * @brief 媒体流跨房间转发状态
 */
    public enum ForwardStreamState
    {
        /** 
         * @brief 空闲状态
         *        + 成功调用 `stopForwardStreamToRooms` 后，所有目标房间为空闲状态。
         *        + 成功调用 `updateForwardStreamToRooms` 减少目标房间后，本次减少的目标房间为空闲状态。
         */
        kForwardStreamStateIdle = 0,
        /** 
         * @brief 开始转发
         *        + 调用 `startForwardStreamToRooms` 成功向所有房间开始转发媒体流后，返回此状态。
         *        + 调用 `updateForwardStreamToRooms` 后，成功向新增目标房间开始转发媒体流后，返回此状态。
         */
        kForwardStreamStateSuccess = 1,
        /** 
         * @brief 转发失败，失败详情参考 ForwardStreamError{@link #ForwardStreamError}
         *        调用 `startForwardStreamToRooms` 或 `updateForwardStreamToRooms` 后，如遇转发失败，返回此状态。
         */
        kForwardStreamStateFailure = 2,
    };

    /** 
     * @type keytype
     * @brief 媒体流跨房间转发过程中抛出的错误码
     */
    public enum ForwardStreamError
    {
        /** 
         * @brief 正常
         */
        kForwardStreamErrorOK = 0,
        /** 
         * @brief 参数异常
         */
        kForwardStreamErrorInvalidArgument = 1201,
        /** 
         * @brief token 错误
         */
        kForwardStreamErrorInvalidToken = 1202,
        /** 
         * @brief 服务端异常
         */
        kForwardStreamErrorResponse = 1203,
        /** 
         * @brief 目标房间有相同 user id 的用户加入，转发中断
         */
        kForwardStreamErrorRemoteKicked = 1204,
        /** 
         * @brief 服务端不支持转发功能
         */
        kForwardStreamErrorNotSupport = 1205,
    };
    /** 
     * @type keytype
     * @brief 媒体流跨房间转发事件
     */
    public enum ForwardStreamEvent
    {
        /** 
         * @brief 本端与服务器网络连接断开，暂停转发。
         */
        kForwardStreamEventDisconnected = 0,
        /** 
         * @brief 本端与服务器网络连接恢复，转发服务连接成功。
         */
        kForwardStreamEventConnected = 1,
        /** 
         * @brief 转发中断。转发过程中，如果相同 user_id 的用户进入目标房间，转发中断。
         */
        kForwardStreamEventInterrupt = 2,
        /** 
         * @brief 目标房间已更新，由 `updateForwardStreamToRooms` 触发。
         */
        kForwardStreamEventDstRoomUpdated = 3,
        /** 
         * @brief API 调用时序错误。例如，在调用 `startForwardStreamToRooms` 之前调用 `updateForwardStreamToRooms` 。
         */
        kForwardStreamEventUnExpectAPICall = 4,
    };

    /** 
     * @type keytype
     * @brief 跨房间转发媒体流过程中的不同目标房间的状态和错误信息
     */
    public struct ForwardStreamStateInfo
    {
        /** 
         * @brief 跨间转发媒体流过程中目标房间 ID<br>
         */
        public string room_id;
        /** 
         * @brief 跨房间转发媒体流过程中该目标房间的状态，参看 ForwardStreamState{@link #ForwardStreamState}
         */
        public ForwardStreamState state;
        /** 
         * @brief 跨房间转发媒体流过程中该目标房间抛出的错误码，参看 ForwardStreamError{@link #ForwardStreamError}
         */
        public ForwardStreamError error;
    };

    /** 
     * @type keytype
     * @brief 跨房间转发媒体流过程中的不同目标房间发生的事件
     */
    public struct ForwardStreamEventInfo
    {
        /** 
         * @brief 跨房间转发媒体流过程中的发生该事件的目标房间 ID<br>
         *        空字符串代表所有目标房间
         */
          public string room_id;
        /** 
         * @brief 跨房间转发媒体流过程中该目标房间发生的事件，参看 ForwardStreamEvent{@link #ForwardStreamEvent}
         */
          public ForwardStreamEvent stream_event;
     };
   public enum VoiceChangerType
    {
        /** {zh}
         * @brief 原声，不含特效
         */
        /** {en}
         * @brief Acoustic, no special effects
         */
        kVoiceChangerTypeOriginal = 0,
        /** {zh}
         * @brief 巨人
         */
        /** {en}
         * @brief Giant
         */
        kVoiceChangerTypeGiant = 1,
        /** {zh}
         * @brief 花栗鼠
         */
        /** {en}
         * @brief Chipmunk
         */
        kVoiceChangerTypeChipmunk = 2,
        /** {zh}
         * @brief 小黄人
         */
        /** {en}
         * @brief Little yellow man
         */
        kVoiceChangerTypeMinionst = 3,
        /** {zh}
         * @brief 颤音
         */
        /** {en}
         * @brief Trill
         */
        kVoiceChangerTypeVibrato = 4,
        /** {zh}
         * @brief 机器人
         */
        /** {en}
         * @brief Robot
         */
        kVoiceChangerTypeRobot = 5,
    };

    /** {zh}
 * @type keytype
 * @brief 音频均衡效果。
 */
    /** {en}
     * @type keytype
     * @brief Audio Equalization effect.
     */
    public enum VoiceEqualizationBandFrequency
    {
        /** {zh}
         * @brief 中心频率为 31Hz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 31Hz.
         */
        kVoiceEqualizationBandFrequency31 = 0,
        /** {zh}
         * @brief 中心频率为 62Hz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 62Hz.
         */
        kVoiceEqualizationBandFrequency62 = 1,
        /** {zh}
         * @brief 中心频率为 125Hz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 125Hz.
         */
        kVoiceEqualizationBandFrequency125 = 2,
        /** {zh}
     * @brief 中心频率为 250Hz 的频带。
     */
        /** {en}
         * @brief The frequency band with a center frequency of 250Hz.
         */
        kVoiceEqualizationBandFrequency250 = 3,
        /** {zh}
     * @brief 中心频率为 500Hz 的频带。
     */
        /** {en}
         * @brief The frequency band with a center frequency of 500Hz.
         */
        kVoiceEqualizationBandFrequency500 = 4,
        /** {zh}
     * @brief 中心频率为 1kHz 的频带。
     */
        /** {en}
         * @brief The frequency band with a center frequency of 1kHz.
         */
        kVoiceEqualizationBandFrequency1k = 5,
        /** {zh}
         * @brief 中心频率为 2kHz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 2kHz.
         */
        kVoiceEqualizationBandFrequency2k = 6,
        /** {zh}
         * @brief 中心频率为 4kHz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 4kHz.
         */
        kVoiceEqualizationBandFrequency4k = 7,
        /** {zh}
         * @brief 中心频率为 8kHz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 8kHz.
         */
        kVoiceEqualizationBandFrequency8k = 8,
        /** {zh}
         * @brief 中心频率为 16kHz 的频带。
         */
        /** {en}
         * @brief The frequency band with a center frequency of 16kHz.
         */
        kVoiceEqualizationBandFrequency16k = 9,
    };
    /** {zh}
 * @type keytype
 * @brief 语音均衡效果。
 */
    /** {en}
     * @type keytype
     * @brief Voice equalization effect.
     */
    public struct VoiceEqualizationConfig
    {
        /** {zh}
         * @brief 频带。参看 VoiceEqualizationBandFrequency{@link #VoiceEqualizationBandFrequency}。
         */
        /** {en}
         * @brief Frequency band. See VoiceEqualizationBandFrequency{@link #VoiceEqualizationBandFrequency}.
         */
        public VoiceEqualizationBandFrequency frequency;
        /** {zh}
         * @brief 频带增益（dB）。取值范围是 `[-15, 15]`。
         */
        /** {en}
         * @brief Gain of the frequency band in dB. The range is `[-15, 15]`.
         */
        public int gain;
    };
    /** {zh}
     * @type keytype
     * @brief 音频混响效果。
     */
    /** {en}
     * @type keytype
     * @brief Voice reverb effect.
     */
    public struct VoiceReverbConfig
    {
        /** {zh}
         * @brief 混响模拟的房间大小，取值范围 `[0.0, 100.0]`。默认值为 `50.0f`。房间越大，混响越强。
         */
        /** {en}
         * @brief The room size for reverb simulation. The range is `[0.0, 100.0]`. The default value is `50.0f`. The larger the room, the stronger the reverberation.
         */
        public float room_size;
        /** {zh}
         * @brief 混响的拖尾长度，取值范围 `[0.0, 100.0]`。默认值为 `50.0f`。
         */
        /** {en}
         * @brief The decay time of the reverb effect. The range is `[0.0, 100.0]`. The default value is `50.0f`. 
         */
        public float decay_time ;
        /** {zh}
         * @brief 混响的衰减阻尼大小，取值范围 `[0.0, 100.0]`。默认值为 `50.0f`。
         */
        /** {en}
         * @brief The damping index of the reverb effect. The range is `[0.0, 100.0]`. The default value is `50.0f`. 
         */
        public float damping ;
        /** {zh}
         * @brief 早期反射信号强度。取值范围 `[-20.0, 10.0]`，单位为 dB。默认值为 `0.0f`。
         */
        /** {en}
         * @brief The Intensity of the wet signal in dB. The range is `[-20.0, 10.0]`. The default value is `0.0f`. 
         */
        public float wet_gain ;
        /** {zh}
         * @brief 原始信号强度。取值范围 `[-20.0, 10.0]`，单位为 dB。默认值为 `0.0f`。
         */
        /** {en}
         * @brief The Intensity of the dry signal in dB. The range is `[-20.0, 10.0]`. The default value is `0.0f`. 
         */
        public float dry_gain;
        /** {zh}
         * @brief 早期反射信号的延迟。取值范围 `[0.0, 200.0]`，单位为 ms。默认值为 `0.0f`。
         */
        /** {en}
         * @brief The delay of the wet signal in ms. The range is `[0.0, 200.0]`. The default value is `0.0f`. 
         */
        public float pre_delay;
    };

    /** {zh}
 * @type keytype
 * @brief 水印图片相对视频流的位置和大小。
 */
    /** {en}
     * @type keytype
     * @brief Watermark's scaled coordinates and size, relative to the video stream.
     */
    public struct ByteWatermark
    {
        /**
         * @hidden currently not available
         */
        public string url;
        /** {zh}
         * @brief 水印图片相对视频流左上角的横向偏移与视频流宽度的比值，取值范围为 [0,1)。
         */
        /** {en}
         * @brief The watermark's horizontal offset from the upper left corner of the video stream to the video stream's width in range of [0,1).
         */
        public float x;
        /** {zh}
         * @brief 水印图片相对视频流左上角的纵向偏移与视频流高度的比值，取值范围为 [0,1)。
         */
        /** {en}
         * @brief The watermark's vertical offset from the upper left corner of the video stream to the video stream's height in range of [0,1).
         */
        public float y;
        /** {zh}
         * @brief 水印图片宽度与视频流宽度的比值，取值范围 [0,1)。
         */
        /** {en}
         * @brief The watermark's width to the video stream's width in range of [0,1).
         */
        public float width;
        /** {zh}
         * @brief 水印图片高度与视频流高度的比值，取值范围为 [0,1)。
         */
        /** {en}
         * @brief The watermark height to the video stream's height in range of [0,1).
         */
        public float height;
    };
    /** {zh}
 * @type keytype
 * @brief 水印参数
 */
    /** {en}
     * @type keytype
     * @brief Watermark configurations
     */
   public struct RTCWatermarkConfig
    {
        /** {zh}
         * @type keytype
         * @brief 水印是否在视频预览中可见，默认可见。
         */
        /** {en}
         * @type keytype
         * @brief Whether the watermark is visible in preview. Its default value is `true`.
         */
        public bool visible_in_preview;
        /** {zh}
         * @type keytype
         * @brief 横屏时的水印位置和大小，参看 ByteWatermark{@link #ByteWatermark}。
         */
        /** {en}
         * @type keytype
         * @brief Watermark's coordinates and size in landscape mode. See ByteWatermark{@link #ByteWatermark}.
         */
          public ByteWatermark position_in_landscape_mode;
        /** {zh}
         * @type keytype
         * @brief 视频编码的方向模式为竖屏时的水印位置和大小，参看 ByteWatermark{@link #ByteWatermark}。
         */
        /** {en}
         * @type keytype
         * @brief Watermark's coordinates and size in portrait mode. See ByteWatermark{@link #ByteWatermark}.
         */
         public ByteWatermark position_in_portrait_mode;
    };

    /** {zh}
 * @type keytype
 * @brief 基础美颜模式。
 */
    /** {en}
     * @type keytype
     * @brief Basic beauty effect.
     */
    public enum EffectBeautyMode
    {
        /** {zh}
         * @brief 美白。
         */
        /** {en}
         * @brief Brightening.
         */
        kEffectBeautyModeWhite = 0,
        /** {zh}
         * @brief 磨皮。
         */
        /** {en}
         * @brief Smoothing.
         */
        kEffectBeautyModeSmooth = 1,
        /** {zh}
         * @brief 锐化。
         */
        /** {en}
         * @brief Sharpening.
         */
        kEffectBeautyModeSharpen = 2,
        /** {zh}
         * @valid since 3.55
         * @brief 清晰，需集成 v4.4.2+ 版本的特效 SDK。
         */
        /** {en}
         * @valid since 3.55
         * @brief Clarity. Integrating Effects SDK v4.4.2+ is required for this sub-item.
         */
        kEffectBeautyModeClear = 3,
    };
    /** {zh}
 * @type keytype
 * @brief 虚拟背景类型。
 */
    /** {en}
     * @type keytype
     * @brief Virtual background type.
     */
   public enum VirtualBackgroundSourceType
    {
        /** {zh}
         * @brief 使用纯色背景替换视频原有背景。
         */
        /** {en}
         * @brief Replace the original background with a solid color.
         */
        kVirtualBackgroundSourceTypeColor = 0,
        /** {zh}
         * @brief 使用自定义图片替换视频原有背景。
         */
        /** {en}
         * @brief Replace the original background with the specified image.
         */
        kVirtualBackgroundSourceTypeImage = 1,
    };
    /** {zh}
 * @type keytype
 * @brief 虚拟背景对象。
 */
    /** {en}
     * @type keytype
     * @brief Virtual background object.
     */
    public struct VirtualBackgroundSource
    {
        /** {zh}
         * @brief 虚拟背景类型，详见 VirtualBackgroundSourceType{@link #VirtualBackgroundSourceType} 。
         */
        /** {en}
         * @brief See VirtualBackgroundSourceType{@link #VirtualBackgroundSourceType}.
         */
        public VirtualBackgroundSourceType source_type ;
        /** {zh}
         * @brief 纯色背景使用的颜色。<br>
         *        格式为 0xAARRGGBB 。
         */
        /** {en}
         * @brief The solid color of the background. <br>
         *        The format is 0xAARRGGBB. <br>
         */
        public uint source_color;
        /** {zh}
         * @brief 自定义背景图片的绝对路径。
         *       + 支持的格式为 jpg、jpeg、png。  <br>
         *       + 图片分辨率超过 1080P 时，图片会被等比缩放至和视频一致。  <br>
         *       + 图片和视频宽高比一致时，图片会被直接缩放至和视频一致。  <br>
         *       + 图片和视频长宽比不一致时，为保证图片内容不变形，图片按短边缩放至与视频帧一致，使图片填满视频帧，对多出的高或宽进行剪裁。  <br>
         *       + 自定义图片带有局部透明效果时，透明部分由黑色代替。
         */
        /** {en}
         * @brief The absolute path of the specified image.
         *       + You can upload a .JPG, .PNG, or .JPEG file.  <br>
         *       + The image with a resolution higher than 1080p(Full HD) will be rescaled proportionally to fit in the video.  <br>
         *       + If the image's aspect ratio matches the video's, the image will be rescaled proportionally to fit in the video.  <br>
         *       + If the image’s aspect ratio doesn't match the video's, the shortest side (either height or width) of the image will be stretched proportionally to match the video. Then the image will be cropped to fill in the video.  <br>
         *       + The transparent area in the image will be filled with black.
         */
        public string source_path;
    };

    /** {zh}
 * @type keytype
 * @brief 数码变焦参数类型
 */
    /** {en}
     * @type keytype
     * @brief Digital Zoom type
     */
    public enum ZoomConfigType
    {
        /** {zh}
         * @brief 设置缩放系数
         */
        /** {en}
         * @brief To set the offset for zooming in and zooming out.
         */
        kZoomConfigTypeFocusOffset = 0,
        /** {zh}
         * @brief 设置移动步长
         */
        /** {en}
         * @brief To set the offset for panning and tiling.
         */
        kZoomConfigTypeMoveOffset,
    };

    /** {zh}
     * @type keytype
     * @brief 数码变焦操作类型
     */
    /** {en}
     * @type keytype
     * @brief Action of the digital zoom control
     */
    public enum ZoomDirectionType
    {
        /** {zh}
         * @brief 相机向左移动
         */
        /** {en}
         * @brief Move to the left.
         */
        kZoomDirectionTypeMoveLeft = 0,
        /** {zh}
         * @brief 相机向右移动
         */
        /** {en}
         * @brief Move to the right.
         */
        kZoomDirectionTypeMoveRight,
        /** {zh}
         * @brief 相机向上移动
         */
        /** {en}
         * @brief Move upwards.
         */
        kZoomDirectionTypeMoveUp,
        /** {zh}
         * @brief 相机向下移动
         */
        /** {en}
         * @brief Move downwards.
         */
        kZoomDirectionTypeMoveDown,
        /** {zh}
         * @brief 相机缩小焦距
         */
        /** {en}
         * @brief Zoom out.
         */
        kZoomDirectionTypeZoomOut,
        /** {zh}
         * @brief 相机放大焦距
         */
        /** {en}
         * @brief Zoom in.
         */
        kZoomDirectionTypeZoomIn,
        /** {zh}
         * @brief 恢复到原始画面
         */
        /** {en}
         * @brief Reset digital zoom.
         */
        kZoomDirectionTypeReset,
    };
    /** {zh}
     * @type keytype
     * @brief 房间模式
     */
    /** {en}
     * @type keytype
     * @brief  Room profile
     */
    public enum RoomProfileType {
        /** {zh}
         * @brief 普通音视频通话模式。<br>
         *        单声道，采样率为 48kHz。 <br>
         *        你应在 1V1 音视频通话时，使用此设置。 <br>
         *        此设置下，弱网抗性较好。
         */
        /** {en}
         * @brief Normal call mode. <br>
         *        Mono audio channel. The sampling rate is 48kHz. <br>
         *        You should use this mode for 1V1 calls. This mode works fine even if the network quality is poor.
         */
        kRoomProfileTypeCommunication = 0,
        /** {zh}
         * @brief 直播模式。<br>
         *        单声道，采样率为 48kHz。 <br>
         *        当你对音视频通话的音质和画质要求较高时，应使用此设置。<br>
         *        此设置下，当用户使用蓝牙耳机收听时，蓝牙耳机使用媒体模式。
         */
        /** {en}
         * @brief Live broadcasting mode. <br>
         *        Mono audio channel. The sampling rate is 48kHz. <br>
         *        Use this mode for high quality of the media. <br>
         *        In this mode, audio plays in media mode for Bluetooth earphones.
         */
        kRoomProfileTypeLiveBroadcasting = 1,
        /** {zh}
         * @brief 游戏语音模式。<br>
         *        单声道，采样率为 16kHz。 <br>
         *        低端机在此模式下运行时，进行了额外的性能优化：<br>
         *            + 部分低端机型配置编码帧长 40/60 <br>
         *            + 部分低端机型关闭软件 3A 音频处理 <br>
         *        增强对 iOS 其他屏幕录制进行的兼容性，避免音频录制被 RTC 打断。
         */
        /** {en}
         * @brief Game voice mode. <br>
         *        Mono audio channel. The sampling rate is 16kHz. <br>
         *        Additional performance optimizations have been made for low-end devices: < br >
         *             + Encodes frame length 40/60 for some low-end models. < br >
         *             + Disables software 3A audio processing for some low-end models.< br >
         *        Enhance iOS Compatibility with other screen recordings to avoid audio recordings being interrupted by RTC.
         */
        kRoomProfileTypeGame = 2,
        /** {zh}
         * @brief 云游戏模式。<br>
         *        单声道，采样率为 48kHz。 <br>
         *        如果你需要低延迟、高码率的设置时，你可以使用此设置。<br>
         *        此设置下，弱网抗性较差。
         */
        /** {en}
         * @brief Cloud game mode. <br>
         *        Mono audio channel. The sampling rate is 48kHz. <br>
         *        Use this mode for scenerios of low latency and high bitrate. <br>
         *        This mode works poor when the network quality is poor.
         */
        kRoomProfileTypeCloudGame = 3,
        /** {zh}
         * @brief 低时延模式。SDK 会使用低延时设置。  <br>
         *        当你的场景非游戏或云游戏场景，又需要极低延时的体验时，可以使用该模式。 <br>
         *        该模式下，音视频通话延时会明显降低，但同时弱网抗性、通话音质等均会受到一定影响。  <br>
         *        在使用此模式前，强烈建议咨询技术支持同学。
         */
        /** {en}
         * @brief Low latency mode. <br>
         *        Use this mode when the scenario is neither a game or cloud game but requires very low latency. <br>
         *        In this mode, call delay will be significantly reduced. But the audio quality and the redundancy for weak network are poor. <br>
         *        You must consult the technical support specialist before using this mode.
         */
        kRoomProfileTypeLowLatency = 4,
    }

    /** {zh}
     * @type keytype
     * @brief 本地音频流状态。
     *        SDK 通过 `OnLocalAudioStateChangedEventHandler` 回调本地音频流状态
     */
    /** {en}
     * @type keytype
     * @brief  Local audio stream status.
     *        SDK callbacks local audio stream status via `OnLocalAudioStateChangedEventHandler`
     */
    public enum LocalAudioStreamState {
        /** {zh}
         * @brief 本地音频默认初始状态。
         *        麦克风停止工作时回调该状态，对应错误码 kLocalAudioStreamErrorOk
         */
        /** {en}
         * @brief The default initial state of the local audio.
         *         Callback to this state when the microphone stops working, corresponding to the error code kLocalAudioStreamErrorOk
         */
        kLocalAudioStreamStateStopped = 0,
        /** {zh}
         * @brief 本地音频录制设备启动成功。
         *        采集到音频首帧时回调该状态，对应错误码 kLocalAudioStreamErrorOk
         */
        /** {en}
         * @brief The local audio recording device started successfully.
         *         Callback to the state when the first frame of audio is collected, corresponding to the error code kLocalAudioStreamErrorOk
         */
        kLocalAudioStreamStateRecording,
        /** {zh}
         * @brief 本地音频首帧编码成功。
         *        音频首帧编码成功时回调该状态，对应错误码 kLocalAudioStreamErrorOk
         */
        /** {en}
         * @brief The first frame of the local audio was successfully encoded.
         *         Callback to the state when the audio first frame encoding is successful, corresponding to the error code kLocalAudioStreamErrorOk
         */
        kLocalAudioStreamStateEncoding,
        /** {zh}
         * @brief  本地音频启动失败，在以下时机回调该状态：  <br>
         *       + 本地录音设备启动失败，对应错误码 kLocalAudioStreamErrorRecordFailure <br>
         *       + 检测到没有录音设备权限，对应错误码 kLocalAudioStreamErrorDeviceNoPermission <br>
         *       + 音频编码失败，对应错误码 kLocalAudioStreamErrorEncodeFailure
        */
        /** {en}
         * @brief  The local audio startup failed, and the status is called back at the following times:   <br>
         *        + The local recording device failed to start, corresponding to the error code kLocalAudioStreamErrorRecordFailure <br>
         *        + No recording device permission was detected, corresponding to the error code kLocalAudioStreamErrorDeviceNoPermission <br>
         *        + The audio encoding failed, corresponding to the error code kLocalAudioStreamErrorEncodeFailure
         */
        kLocalAudioStreamStateFailed,
        /** {zh}
         * @brief 本地音频静音成功后回调该状态。
         *        调用 SetAudioCaptureDeviceMute{@link #IAudioDeviceManager-setaudiocapturedevicemute} 成功后回调，对应错误码 LocalAudioStreamError{@link #LocalAudioStreamError} 中的 kLocalAudioStreamErrorOk 。  <br>
         */
        /** {en}
         * @brief Callback the state after the local audio is successfully muted.
         *         Callback after successful SetAudioCaptureDeviceMute{@link #IAudioDeviceManager-setaudiocapturedevicemute} call, corresponding to kLocalAudioStreamErrorOk in the error code LocalAudioStreamError{@link #LocalAudioStreamError}. <br>
         */
        kLocalAudioStreamMute,
        /** {zh}
         * @brief 本地音频解除静音成功后回调该状态。
         *        调用 SetAudioCaptureDeviceMute{@link #IAudioDeviceManager-setaudiocapturedevicemute} 成功后回调，对应错误码 LocalAudioStreamError{@link #LocalAudioStreamError} 中的 kLocalAudioStreamErrorOk 。  <br>
         */
        /** {en}
         * @brief Callback the state after the local audio is successfully unmuted.
         *         Callback after successful SetAudioCaptureDeviceMute{@link #IAudioDeviceManager-setaudiocapturedevicemute} call, corresponding to kLocalAudioStreamErrorOk in the error code LocalAudioStreamError{@link #LocalAudioStreamError}. <br>
         */
        kLocalAudioStreamUnmute
    }

    /** {zh}
     * @type errorcode
     * @brief 本地音频流状态改变时的错误码。
     *        SDK 通过 `OnLocalAudioStateChangedEventHandler` 回调该错误码。
     */
    /** {en}
     * @type errorcode
     * @brief Error code when the state of the local audio stream changes. The
     *        SDK calls back the error code via `OnLocalAudioStateChangedEventHandler`.
     */
    public enum LocalAudioStreamError {
        /** {zh}
        * @brief 本地音频状态正常
        */
        /** {en}
        * @brief Local audio status is normal
        */
        kLocalAudioStreamErrorOk = 0,
        /** {zh}
        * @brief 本地音频出错原因未知
        */
        /** {en}
        * @brief Local audio error cause unknown
        */
        kLocalAudioStreamErrorFailure,
        /** {zh}
        * @brief 没有权限启动本地音频录制设备
        */
        /** {en}
        * @brief No permission to start local audio recording device
        */
        kLocalAudioStreamErrorDeviceNoPermission,
        /** {zh}
        * @brief 本地音频录制设备已经在使用中
        * @notes 该错误码暂未使用
        */
        /** {en}
        * @brief The local audio recording device is already in use
        * @notes The error code is not yet in use
        */
        kLocalAudioStreamErrorDeviceBusy,
        /** {zh}
        * @brief 本地音频录制失败，建议你检查录制设备是否正常工作
        */
        /** {en}
        * @brief Local audio recording failed, it is recommended that you check whether the recording device is working properly
        */
        kLocalAudioStreamErrorRecordFailure,
        /** {zh}
        * @brief 本地音频编码失败
        */
        /** {en}
        * @brief Local audio encoding failed
        */
        kLocalAudioStreamErrorEncodeFailure,
        /** {zh}
         * @brief 没有可用的音频录制设备
         */
        /** {en}
         * @brief No audio recording equipment available
         */
        kLocalAudioStreamErrorNoRecordingDevice
    }

    /** {zh}
     * @type keytype
     * @brief 远端音频流状态。
     *        SDK 通过 `OnLocalAudioStateChangedEventHandler` 回调本地音频流状态
     */
    /** {en}
     * @type keytype
     * @brief  Local audio stream status.
     *        SDK callbacks local audio stream status via `OnLocalAudioStateChangedEventHandler`
     */
    public enum RemoteAudioStreamState {
        /** {zh}
         * @brief  不接收远端音频流。 <br>
         *         以下情况下会触发回调 `onRemoteAudioStateChanged`：  <br>
         *       + 本地用户停止接收远端音频流，对应原因是：kRemoteAudioStateChangeReasonLocalMuted{@link #RemoteAudioStateChangeReason}  <br>
         *       + 远端用户停止发送音频流，对应原因是：kRemoteAudioStateChangeReasonRemoteMuted{@link #RemoteAudioStateChangeReason}  <br>
         *       + 远端用户离开房间，对应原因是：kRemoteAudioStateChangeReasonRemoteOffline{@link #RemoteAudioStateChangeReason}  <br>
         */
        /** {en}
         * @brief  The remote audio stream is not received.  <br>
         *          The callback `onRemoteAudioStateChanged` triggers: <br>
         *        + Local users stop receiving remote audio streams due to kRemoteAudioStateChangeReasonLocalMuted {@link #RemoteAudioStateChangeReason} <br>
         *        + The remote user stops sending audio streams due to kRemoteAudioStateChangeReasonRemoteMuted {@link #RemoteAudioStateChangeReason} <br>
         *        + The remote user left the room, the corresponding reason is: kRemoteAudioStateChangeReasonRemoteOffline{@link #RemoteAudioStateChangeReason} <br>
         */
        kRemoteAudioStateStopped = 0,
        /** {zh}
         * @brief 开始接收远端音频流首包。<br>
         *        刚收到远端音频流首包会触发回调 `onRemoteAudioStateChanged`，
         *        对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonLocalUnmuted`
         */
        /** {en}
         * @brief Start receiving the remote audio stream header. <br>
         *        Just received the remote audio stream header will trigger a callback `onRemoteAudioStateChanged`,
         *         The corresponding reason is: RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} in "kRemoteAudioStateChangeReasonLocalUnmuted"
         */
        kRemoteAudioStateStarting,
        /** {zh}
         * @brief  远端音频流正在解码，正常播放。 <br>
         *         以下情况下会触发回调 `onRemoteAudioStateChanged`：  <br>
         *       + 成功解码远端音频首帧，对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonLocalUnmuted` <br>
         *       + 网络由阻塞恢复正常，对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonNetworkRecovery` <br>
         *       + 本地用户恢复接收远端音频流，对应原因是：RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonLocalUnmuted` <br>
         *       + 远端用户恢复发送音频流，对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonRemoteUnmuted` <br>
         */
        /** {en}
         * @brief  The remote audio stream is decoding and playing normally.  <br>
         *          A callback is triggered in the following cases: `onRemoteAudioStateChanged`: <br>
         *        + The remote audio first frame was successfully decoded, and the corresponding reason is: 'kRemoteAudioStateChangeReasonLocalUnmuted' in RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}  <br>
         *        + The network returned to normal from blocking, and the corresponding reason is: "kRemoteAudioStateChangeReasonNetworkRecovery ' in RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}  <br>
         *        + Local users resume receiving remote audio streams, and the corresponding reason is 'kRemoteAudioStateChangeReasonLocalUnmuted ' RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} <br>
         *        + Remote users resume sending audio streams, and the corresponding reason is 'kRemoteAudioStateChangeReasonRemoteUnmuted' in RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}
         */
        kRemoteAudioStateDecoding,
        /** {zh}
         * @brief 远端音频流卡顿。<br>
         *        网络阻塞、丢包率大于 40% 时，会触发回调 `onRemoteAudioStateChanged`，
         *        对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonNetworkCongestion`
         */
        /** {en}
         * @brief Remote audio streaming card. <br>
         *         A callback is triggered when the network is blocked and the packet loss rate is greater than 40%. `onRemoteAudioStateChanged`,
         *         The corresponding reason is: "kRemoteAudioStateChangeReasonNetworkCongestion" in RemoteAudioStateChangeReasonNetworkCongestion}{@link #RemoteAudioStateChangeReasonNetworkCongestion}
         */
        kRemoteAudioStateFrozen,
        /** {zh}
         * @hidden currently not available
         * @brief 远端音频流播放失败
         * @notes 该错误码暂未使用
         */
        /** {en}
         * @hidden currently not available
         * @brief  The remote audio stream failed to play
         * @notes  The error code is not yet used
         */
        kRemoteAudioStateFailed,
    }



    /** {zh}
     * @type keytype
     * @brief 远端音频流状态。<br>
     *        用户可以通过 `OnRemoteAudioStateChangedEventHandler` 了解该状态。
     */
    /** {en}
     * @type keytype
     * @brief  Remote audio stream state. <br>
     *         Users can learn about this status through `OnRemoteAudioStateChangedEventHandler`.
     */
    public enum RemoteAudioState {
        /** {zh}
         * @brief  不接收远端音频流。 <br>
         *         以下情况下会触发回调 `OnRemoteAudioStateChangedEventHandler`：  <br>
         *       + 本地用户停止接收远端音频流，对应原因是：kRemoteAudioStateChangeReasonLocalMuted{@link #RemoteAudioStateChangeReason}  <br>
         *       + 远端用户停止发送音频流，对应原因是：kRemoteAudioStateChangeReasonRemoteMuted{@link #RemoteAudioStateChangeReason}  <br>
         *       + 远端用户离开房间，对应原因是：kRemoteAudioStateChangeReasonRemoteOffline{@link #RemoteAudioStateChangeReason}  <br>
         */
        /** {en}
         * @brief  The remote audio stream is not received.  <br>
         *          The callback `OnRemoteAudioStateChangedEventHandler` triggers: <br>
         *        + Local users stop receiving remote audio streams due to kRemoteAudioStateChangeReasonLocalMuted {@link #RemoteAudioStateChangeReason} <br>
         *        + The remote user stops sending audio streams due to kRemoteAudioStateChangeReasonRemoteMuted {@link #RemoteAudioStateChangeReason} <br>
         *        + The remote user left the room, the corresponding reason is: kRemoteAudioStateChangeReasonRemoteOffline {@link #RemoteAudioStateChangeReason} <br>
         */
        kRemoteAudioStateStopped = 0,
        /** {zh}
         * @brief 开始接收远端音频流首包。<br>
         *        刚收到远端音频流首包会触发回调 `OnRemoteAudioStateChangedEventHandler`，
         *        对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonLocalUnmuted`
         */
        /** {en}
         * @brief Start receiving the remote audio stream header. <br>
         *        Just received the remote audio stream header will trigger a callback `OnRemoteAudioStateChangedEventHandler`,
         *         The corresponding reason is: RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} in "kRemoteAudioStateChangeReasonLocalUnmuted"
         */
        kRemoteAudioStateStarting,
        /** {zh}
         * @brief  远端音频流正在解码，正常播放。 <br>
         *         以下情况下会触发回调 `OnRemoteAudioStateChangedEventHandler`：  <br>
         *       + 成功解码远端音频首帧，对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonLocalUnmuted` <br>
         *       + 网络由阻塞恢复正常，对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonNetworkRecovery` <br>
         *       + 本地用户恢复接收远端音频流，对应原因是：RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonLocalUnmuted` <br>
         *       + 远端用户恢复发送音频流，对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonRemoteUnmuted` <br>
         */
        /** {en}
         * @brief  The remote audio stream is decoding and playing normally.  <br>
         *          A callback is triggered in the following cases: `OnRemoteAudioStateChangedEventHandler`: <br>
         *        + The remote audio first frame was successfully decoded, and the corresponding reason is: 'kRemoteAudioStateChangeReasonLocalUnmuted' in RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}  <br>
         *        + The network returned to normal from blocking, and the corresponding reason is: "kRemoteAudioStateChangeReasonNetworkRecovery ' in RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}  <br>
         *        + Local users resume receiving remote audio streams, and the corresponding reason is 'kRemoteAudioStateChangeReasonLocalUnmuted ' RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} <br>
         *        + Remote users resume sending audio streams, and the corresponding reason is 'kRemoteAudioStateChangeReasonRemoteUnmuted' in RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason}
         */
        kRemoteAudioStateDecoding,
        /** {zh}
         * @brief 远端音频流卡顿。<br>
         *        网络阻塞、丢包率大于 40% 时，会触发回调 `OnRemoteAudioStateChangedEventHandler`，
         *        对应原因是： RemoteAudioStateChangeReason{@link #RemoteAudioStateChangeReason} 中的 `kRemoteAudioStateChangeReasonNetworkCongestion`
         */
        /** {en}
         * @brief Remote audio streaming card. <br>
         *         A callback is triggered when the network is blocked and the packet loss rate is greater than 40%. `OnRemoteAudioStateChangedEventHandler`,
         *         The corresponding reason is: "kRemoteAudioStateChangeReasonNetworkCongestion" in RemoteAudioStateChangeReasonNetworkCongestion} {@link #RemoteAudioStateChangeReasonNetworkCongestion}
         */
        kRemoteAudioStateFrozen,
        /** {zh}
         * @hidden
         * @brief 远端音频流播放失败
         * @notes 该错误码暂未使用
         */
        /** {en}
         * @hidden
         * @brief  The remote audio stream failed to play
         * @notes  The error code is not yet used
         */
        kRemoteAudioStateFailed,
    };

    /** {zh}
     * @type keytype
     * @brief 接收远端音频流状态改变的原因。  <br>
     *        用户可以通过 `OnRemoteAudioStateChangedEventHandler` 了解该原因。
     */
    /** {en}
     * @type keytype
     * @brief  Receives the cause of the remote audio stream state change.   <br>
     *        Users can learn about this reason through `OnRemoteAudioStateChangedEventHandler`.
     */
    public enum RemoteAudioStateChangeReason {
        /** {zh}
         * @brief 内部原因
         */
        /** {en}
         * @brief Internal reasons
         */
        kRemoteAudioStateChangeReasonInternal = 0,
        /** {zh}
         * @brief 网络阻塞
         */
        /** {en}
         * @brief Network blocking
         */
        kRemoteAudioStateChangeReasonNetworkCongestion,
        /** {zh}
         * @brief 网络恢复正常
         */
        /** {en}
         * @brief Network back to normal
         */
        kRemoteAudioStateChangeReasonNetworkRecovery,
        /** {zh}
         * @brief 本地用户停止接收远端音频流
         */
        /** {en}
         * @brief Local user stops receiving remote audio stream
         */
        kRemoteAudioStateChangeReasonLocalMuted,
        /** {zh}
         * @brief 本地用户恢复接收远端音频流
         */
        /** {en}
         * @brief Local users resume receiving remote audio streams
         */
        kRemoteAudioStateChangeReasonLocalUnmuted,
        /** {zh}
         * @brief 远端用户停止发送音频流
         */
        /** {en}
         * @brief Remote user stops sending audio stream
         */
        kRemoteAudioStateChangeReasonRemoteMuted,
        /** {zh}
         * @brief 远端用户恢复发送音频流
         */
        /** {en}
         * @brief Remote user resumes sending audio stream
         */
        kRemoteAudioStateChangeReasonRemoteUnmuted,
        /** {zh}
         * @brief 远端用户离开房间
         */
        /** {en}
         * @brief Remote user leaves room
         */
        kRemoteAudioStateChangeReasonRemoteOffline,
    };

    /** {zh}
     * @type keytype
     * @brief 首帧发送状态
    */
    /** {en}
     * @type keytype
     * @brief  First frame sending status
     */
    public enum FirstFrameSendState {
        /** {zh}
         * @brief 发送中
         */
        /** {en}
         * @brief Sending
         */
        kFirstFrameSendStateSending = 0,
        /** {zh}
         * @brief 发送成功
         */
        /** {en}
         * @brief Sent successfully
         */
        kFirstFrameSendStateSent = 1,
        /** {zh}
         * @brief 发送失败
         */
        /** {en}
         * @brief Send failed
         */
        kFirstFrameSendStateEnd = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 首帧播放状态
     */
    /** {en}
     * @type keytype
     * @brief  First frame playback status
     */
    public enum FirstFramePlayState {
        /** {zh}
        * @brief 播放中
        */
        /** {en}
        * @brief Playing
        */
        kFirstFramePlayStatePlaying = 0,
        /** {zh}
        * @brief 播放成功
        */
        /** {en}
        * @brief Play success
        */
        kFirstFramePlayStatePlayed = 1,
        /** {zh}
        * @brief 播放失败
        */
        /** {en}
        * @brief Play failed
        */
        kFirstFramePlayStateEnd = 2,
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧信息
     */
    /** {en}
     * @type keytype
     * @brief  Video frame information
     */
    public struct VideoFrameInfo {
        /** {zh}
         * @brief 宽（像素）
         */
        /** {en}
         * @brief Width (pixels)
         */
        public int width;
        /** {zh}
         * @brief 高（像素）
         */
        /** {en}
         * @brief High (pixels)
         */
        public int height;
        /** {zh}
         * @brief 视频帧顺时针旋转角度。参看 VideoRotation{@link #VideoRotation}。
         */
        /** {en}
         * @brief Video frame clockwise rotation angle. See VideoRotation{@link #VideoRotation}.
         */
        public VideoRotation rotation;
    };

    /** {zh}
     * @type keytype
     * @brief 视频帧旋转角度
     */
    /** {en}
     * @type keytype
     * @brief Video frame rotation angle
     */
    public enum VideoRotation {
        /** {zh}
         * @brief 顺时针旋转 0 度
         */
        /** {en}
         * @brief Video does not rotate
         */
        kVideoRotation0 = 0,
        /** {zh}
         * @brief 顺时针旋转 90 度
         */
        /** {en}
         * @brief Video rotate 90 degrees clockwise
         */
        kVideoRotation90 = 90,
        /** {zh}
         * @brief 顺时针旋转 180 度
         */
        /** {en}
         * @brief Video rotate 180 degrees clockwise
         */
        kVideoRotation180 = 180,
        /** {zh}
         * @brief 顺时针旋转 270 度
         */
        /** {en}
         * @brief Video rotate 270 degrees clockwise
         */
        kVideoRotation270 = 270
    };

    /** {zh}
     * @type keytype
     * @brief 本地视频流状态
     */
    /** {en}
     * @type keytype
     * @brief  Local video stream status
     */
    public enum LocalVideoStreamState {
        /** {zh}
         * @brief 本地视频采集停止状态
         */
        /** {en}
         * @brief Local video capture stop state
         */
        kLocalVideoStreamStateStopped = 0,
        /** {zh}
         * @brief 本地视频采集设备启动成功
         */
        /** {en}
         * @brief Local video capture device activated
         */
        kLocalVideoStreamStateRecording,
        /** {zh}
         * @brief 本地视频采集后，首帧编码成功
         */
        /** {en}
         * @brief After local video capture, the first frame is encoded successfully
         */
        kLocalVideoStreamStateEncoding,
        /** {zh}
         * @brief 本地视频采集设备启动失败
         */
        /** {en}
         * @brief Local video capture device failed to start
         */
        kLocalVideoStreamStateFailed,
    };

     /** {zh}
     * @type keytype
     * @brief 多房间参数配置
     */
    /** {en}
     * @type keytype
     * @brief  Multi-room parameter configuration
     */
    public class MultiRoomConfig {
        /** {zh}
         * @brief 房间模式，参看 RoomProfileType{@link #RoomProfileType}，默认为普通音视频通话模式，进房后不可更改。
         */
        /** {en}
         * @brief Room mode. See RoomProfileType{@link #RoomProfileType}, the default is normal audio & video call mode, which cannot be changed after entering the room.
         */
        public RoomProfileType roomProfileType;
        /** {zh}
         * @brief 是否自动发布音视频流，默认为自动发布。 <br>
         *        创建和加入多房间时，只能将其中一个房间设置为自动发布。若每个房间均不做设置，则默认在第一个加入的房间内自动发布流。<br>
         *        若调用 setUserVisibility{@link #IRTCRoom#setUserVisibility} 将自身可见性设为 false，无论是默认的自动发布流还是手动设置的自动发布流都不会进行发布，你需要将自身可见性设为 true 后方可发布。
         */
        /** {en}
         * @brief Whether to publish media streams automatically. The default is automatic publishing.  <br>
          *       Only one of the rooms the user joined can be set to auto-publish. If no settings are made in each room, the stream is automatically published in the first room joined by default.<br>
         *        If you call setUserVisibility{@link #IRTCRoom#setUserVisibility} to set your own visibility to false, you will not publish media streams regardless of the value of `is_auto_publish`.
         */
        public bool is_auto_publish;
        /** {zh}
         * @brief 是否自动订阅音频流，默认为自动订阅
         */
        /** {en}
         * @brief Whether to automatically subscribe to the audio stream, the default is automatic subscription
         */
        public bool isAutoSubscribeAudio;
        /** {zh}
         * @brief 是否自动订阅视频流，默认为自动订阅
         */
        /** {en}
         * @brief Whether to automatically subscribe to the video stream, the default is automatic subscription
         */
        public bool isAutoSubscribeVideo;
        /** {zh}
         * @brief 远端视频流参数，参看 RemoteVideoConfig{@link #RemoteVideoConfig}
         */
        /** {en}
         * @brief Expected configuration of remote video stream, see RemoteVideoConfig{@link #RemoteVideoConfig}.
         */
        public RemoteVideoConfig remoteVideoConfig;

        public MultiRoomConfig(RoomProfileType roomProfile = RoomProfileType.kRoomProfileTypeCommunication, bool Is_auto_publish = true, bool IsAutoSubscribeAudio = true, bool IsAutoSubscribeVideo = true)
        {
            roomProfileType = roomProfile;
            is_auto_publish = Is_auto_publish;
            isAutoSubscribeAudio = IsAutoSubscribeAudio;
            isAutoSubscribeVideo = IsAutoSubscribeVideo;
        }
    }

    /** {zh} 
     * @type keytype
     * @brief 远端视频帧信息
     */
    /** {en} 
     * @type keytype
     * @brief Information on remote video frame
     */
    public struct RemoteVideoConfig {
        /** {zh}
         * @brief 期望订阅的最高帧率，单位：fps，默认值为 0，设为大于 0 的值时开始生效。  <br>
         *        当发布端帧率低于设定帧率，或订阅端开启性能回退后下行弱网，则帧率会相应下降。  <br>
         *        仅码流支持 SVC 分级编码特性时方可生效。
         */
        /** {en}
         * @brief Expected maximum frame rate of the subscribed stream in px. The default value is 0, values greater than 10 are valid.  <br>
         *        If the frame rate of the stream published is lower than the value you set, or if your subscribed stream falls back under limited network conditions, the frame rate you set will drop accordingly.  <br>
         *        Only valid if the stream is coded with SVC technique.
         */
        public int framerate;
        /** {zh}
         * @brief 视频宽度，单位：px
         */
        /** {en}
         * @brief Width of the video frame in px
         */
        public int resolutionWidth;
        /** {zh}
         * @brief 视频高度，单位：px
         */
        /** {en}
         * @brief Height of the video frame in px
         */
        public int resolutionHeight;
    };

    /** {zh}
     * @type keytype
     * @brief 媒体流类型
     */
    /** {en} 
     * @type keytype
     * @brief Media stream type
     */
    public enum MediaStreamType {
        /** {zh}
         * @brief 只控制音频
         */
        /** {en}
         * @brief Controls audio only
         */
        kMediaStreamTypeAudio,
        /** {zh}
         * @brief 只控制视频
         */
        /** {en}
         * @brief Controls video only
         */
        kMediaStreamTypeVideo,
        /** {zh}
         * @brief 同时控制音频和视频
         */
        /** {en}
         * @brief Controls both audio and video
         */
        kMediaStreamTypeBoth
    }

    /** {zh}
     * @type keytype
     * @brief 流属性
     */
    /** {en}
     * @type keytype
     * @brief Stream type
     */
    public enum StreamIndex {
        /** {zh}
         * @brief 主流。包括：<br>
         *        + 由摄像头/麦克风通过内部采集机制，采集到的视频/音频; <br>
         *        + 通过自定义采集，采集到的视频/音频。
         */
        /** {en}
         * @brief Mainstream, including: <br>
         *       + Video/audio captured by the the camera/microphone using internal capturing; <br>
         *       + Video/audio captured by custom method.
         */
        kStreamIndexMain = 0,
        /** {zh}
         * @brief 屏幕流。屏幕共享时共享的视频流，或来自声卡的本地播放音频流。
         */
        /** {en}
         * @brief Screen-sharing stream. Video/Audio streams for screen sharing.
         */
        kStreamIndexScreen = 1,
    }

    /** {zh}
     * @type keytype
     * @brief 用户信息
     */
    /** {en}
     * @type keytype
     * @brief  User information
     */
    public struct RtcUser {
        /** {zh}
        * @brief 用户 ID
        */
        /** {en}
        * @brief User ID.
        */
        public string UserID;
        /** {zh}
        * @brief 元数据
        */
        /** {en}
        * @brief Metadata
        */
        public string MetaData;
    }

    /** {zh}
     * @type errorcode
     * @brief 本地视频状态改变时的错误码
     */
    /** {en}
     * @type errorcode
     * @brief Error code when the local video state changes
     */
    public enum LocalVideoStreamError {
        /** {zh}
         * @brief 状态正常
         */
        /** {en}
         * @brief Normal condition
         */
        kLocalVideoStreamErrorOk = 0,
        /** {zh}
         * @brief 本地视频流发布失败
         */
        /** {en}
         * @brief Local video stream publishing failed
         */
        kLocalVideoStreamErrorFailure,
        /** {zh}
         * @brief 没有权限启动本地视频采集设备
         */
        /** {en}
         * @brief No access to the local video capture device
         */
        kLocalVideoStreamErrorDeviceNoPermission,
        /** {zh}
         * @brief 本地视频采集设备被占用
         */
        /** {en}
         * @brief Local video capture equipment is occupied
         */
        kLocalVideoStreamErrorDeviceBusy,
        /** {zh}
         * @brief 本地视频采集设备不存在
         */
        /** {en}
         * @brief Local video capture device does not exist
         */
        kLocalVideoStreamErrorDeviceNotFound,
        /** {zh}
         * @brief 本地视频采集失败，建议检查采集设备是否正常工作
         */
        /** {en}
         * @brief Local video capture failed, it is recommended to check whether the acquisition device is working properly
         */
        kLocalVideoStreamErrorCaptureFailure,
        /** {zh}
         * @brief 本地视频编码失败
         */
        /** {en}
         * @brief Local video encoding failed
         */
        kLocalVideoStreamErrorEncodeFailure,
        /** {zh}
         * @brief 本地视频采集设备被移除
         */
        /** {en}
         * @brief Local video capture device removed
         */
        kLocalVideoStreamErrorDeviceDisconnected
    };


    /** {zh}
     * @type keytype
     * @brief 远端视频流状态。状态改变时，会收到 `OnRemoteVideoStateChangedEventHandler` 回调
     */
    /** {en}
     * @type keytype
     * @brief  Remote video stream status. You will receive `OnRemoteVideoStateChangedEventHandler` callback when the state changes.
     */
    public enum RemoteVideoState {
        /** {zh}
         * @brief 远端视频流默认初始状态，视频尚未开始播放。
         */
        /** {en}
         * @brief The remote video stream defaults to the initial state, and the video has not yet started playing.
         */
        kRemoteVideoStateStopped = 0,
        /** {zh}
         * @brief 本地用户已接收远端视频流首包。
         */
        /** {en}
         * @brief Local user has received remote video stream header packet.
         */
        kRemoteVideoStateStarting,
        /** {zh}
         * @brief 远端视频流正在解码，正常播放。
         */
        /** {en}
         * @brief The remote video stream is decoding and playing normally.
         */
        kRemoteVideoStateDecoding,
        /** {zh}
         * @brief 远端视频流卡顿，可能有网络等原因。
         */
        /** {en}
         * @brief Remote video streaming card, there may be network and other reasons.
         */
        kRemoteVideoStateFrozen,
        /** {zh}
         * @brief 远端视频流播放失败。
         */
        /** {en}
         * @brief The remote video stream failed to play.
         */
        kRemoteVideoStateFailed,
    }

    /** {zh}
     * @type keytype
     * @brief 远端视频流状态改变的原因
     */
    /** {en}
     * @type keytype
     * @brief  Cause of remote video stream state change
     */
    public enum RemoteVideoStateChangeReason {
        /** {zh}
         * @brief 内部原因
         */
        /** {en}
         * @brief Internal reasons
         */
        kRemoteVideoStateChangeReasonInternal = 0,
        /** {zh}
         * @brief 网络阻塞
         */
        /** {en}
         * @brief Network blocking
         */
        kRemoteVideoStateChangeReasonNetworkCongestion,
        /** {zh}
         * @brief 网络恢复正常
         */
        /** {en}
         * @brief Network back to normal
         */
        kRemoteVideoStateChangeReasonNetworkRecovery,
        /** {zh}
         * @brief 本地用户停止接收远端视频流或本地用户禁用视频模块
         */
        /** {en}
         * @brief Local user stops receiving remote video stream or local user disables video module
         */
        kRemoteVideoStateChangeReasonLocalMuted,
        /** {zh}
         * @brief 本地用户恢复接收远端视频流或本地用户启用视频模块
         */
        /** {en}
         * @brief Local user resumes receiving remote video streams or local user enables video modules
         */
        kRemoteVideoStateChangeReasonLocalUnmuted,
        /** {zh}
         * @brief 远端用户停止发送视频流或远端用户禁用视频模块
         */
        /** {en}
         * @brief The remote user stops sending the video stream or the remote user disables the video module
         */
        kRemoteVideoStateChangeReasonRemoteMuted,
        /** {zh}
         * @brief 远端用户恢复发送视频流或远端用户启用视频模块
         */
        /** {en}
         * @brief Remote user resumes sending video stream or remote user enables video module
         */
        kRemoteVideoStateChangeReasonRemoteUnmuted,
        /** {zh}
         * @brief 远端用户离开频道。状态转换参考 `OnUserUnPublishStream`。
         */
        /** {en}
         * @brief The remote user leaves the channel. State transition see `OnUserUnPublishStream`.
         */
        kRemoteVideoStateChangeReasonRemoteOffline,
    };

    /** {zh}
     * @type keytype
     * @brief 暂停/恢复接收远端的媒体流类型。
     */
    /** {en}
     * @type keytype
     * @brief Pause/resume receiving the remote media stream type.
     */
    public enum PauseResumeControlMediaType {
        /** {zh}
         * @brief 只控制音频，不影响视频
         */
        /** {en}
         * @brief Only control audio, not affect video
         */
        kRTCPauseResumeControlMediaTypeAudio = 0,
        /** {zh}
         * @brief 只控制视频，不影响音频
         */
        /** {en}
         * @brief Only control video, not affect audio
         */
        kRTCPauseResumeControlMediaTypeVideo = 1,
        /** {zh}
         * @brief 同时控制音频和视频
         */
        /** {en}
         * @brief Simultaneous control of audio and video
         */
        kRTCPauseResumeControlMediaTypeVideoAndAudio = 2
    };

    /** {zh} 
     * @type keytype
     * @brief 范围语音中用户的位置，坐标系需要自行建立。
     */
    /** {en} 
     * @type keytype
     * @brief  The location of the user in the range speech, the coordinate system needs to be established by itself.
     */
    public struct Position {
        /** {zh} 
         * @brief x 坐标
         */
        /** {en} 
         * @brief X coordinate
         */
        public float x;
        /** {zh} 
         * @brief y 坐标
         */
        /** {en} 
         * @brief Y coordinate
         */
        public float y;
        /** {zh} 
         * @brief z 坐标
         */
        /** {en} 
         * @brief Z coordinate
         */
        public float z;
    };

    /** {zh}
    * @type keytype
    * @type keytype
    * @brief 上行/下行网络质量
    */
    /** {en}
    * @type keytype
    * @brief Tx/Rx network quality
    */
    public struct NetworkQualityStats {
        /** {zh}
        * @brief 用户 ID
        */
        /** {en}
        * @brief User ID
        */
        public string uid;
        /** {zh}
        * @brief 本端的上行/下行的丢包率，范围 [0.0,1.0]
        *        当 `uid` 为本地用户时，代表发布流的上行丢包率。
        *        当 `uid` 为远端用户时，代表接收所有订阅流的下行丢包率。
        */
        /** {en}
        * @brief Packet loss ratio of the local client, ranging [0.0,1.0]
        *        For a local user, it is the sent-packet loss ratio.
        *        For a remote user, it is the loss ratio of all the packets received.
        */
        public double fraction_lost;
        /** {zh}
        * @brief 当 `uid` 为本地用户时有效，客户端到服务端的往返延时（RTT），单位：ms
        */
        /** {en}
        * @brief Round-trip time (RTT) from client to server. Effective for the local user. Unit: ms
        */
        public int rtt;
        /** {zh}
        * @brief 本端的音视频 RTP 包 2 秒内的平均传输速率，单位：bps
        *        当 `uid` 为本地用户时，代表发送速率。
        *        当 `uid` 为远端用户时，代表所有订阅流的接收速率。
        */
        /** {en}
        * @brief Average transmission rate of the media RTP packages in 2s. unit: bps
        *        For a local user, it is the packet-transmitting speed.
        *        For a more user, it is the speed of receiving all the subsribed media.
        */
        public int total_bandwidth;
        /** {zh}
        * @brief 上行网络质量评分，详见 NetworkQuality{@link #NetworkQuality}。
        */
        /** {en}
        * @brief Tx network quality grade. Refer to NetworkQuality{@link #NetworkQuality} for details.
        */
        public NetworkQuality tx_quality;
        /** {zh}
        * @brief 下行网络质量评分，详见 NetworkQuality{@link #NetworkQuality}。
        */
        /** {en}
        * @brief Rx network quality grade. Refer to NetworkQuality{@link #NetworkQuality} for details.
        */
        public NetworkQuality rx_quality;

    };

    /** {zh}
    * @type keytype
    * @brief 媒体流网络质量。
    */
    /** {en}
    * @type keytype
    * @brief Media streaming network quality.
    */
    public enum NetworkQuality {
        /** {zh}
        * @brief 网络质量未知。
        */
        /** {en}
        * @brief Network quality unknown.
        */
        kNetworkQualityUnknown = 0,
        /** {zh}
        * @brief 网络质量极好。
        */
        /** {en}
        * @brief The network quality is excellent.
        */
        kNetworkQualityExcellent,
        /** {zh}
        * @brief 主观感觉和 kNetworkQualityExcellent 差不多，但码率可能略低。
        */
        /** {en}
        * @brief The subjective feeling is similar to kNetworkQualityExcellent, but the bit rate may be slightly lower.
        */
        kNetworkQualityGood,
        /** {zh}
        * @brief 主观感受有瑕疵但不影响沟通。
        */
        /** {en}
        * @brief Subjective feelings are flawed but do not affect communication.
        */
        kNetworkQualityPoor,
        /** {zh}
        * @brief 勉强能沟通但不顺畅。
        */
        /** {en}
        * @brief Can barely communicate but not smoothly.
        */
        kNetworkQualityBad,
        /** {zh}
        * @brief 网络质量非常差，基本不能沟通。
        */
        /** {en}
        * @brief The quality of the network is very poor and communication is basically impossible.
        */
        kNetworkQualityVbad,
    };

/** {zh}
 * @type keytype
 * @brief 本地音/视频流统计信息，统计周期为 2s 。  <br>
 *        本地用户发布音/视频流成功后，SDK 会周期性地通过 `OnLocalStreamStatsEventHandler` 通知本地用户发布的音/视频流在此次统计周期内的发送状况。此数据结构即为回调给用户的参数类型。  <br>
 */
/** {en}
 * @type keytype
 * @brief Local audio/video stream statistics and network status, the reference period is 2s. <br>
 *        After the local user publishes the audio/video stream successfully, the SDK will periodically notify the local user through `OnLocalStreamStatsEventHandler` the transmission status of the published audio/video stream during this reference period. This data structure is the type of parameter that is called back to the user. <br>
 */
    public struct LocalStreamStats {
    /** {zh}
     * @brief 本地设备发送音频流的统计信息，详见 LocalAudioStats{@link #LocalAudioStats} 。
     */
    /** {en}
     * @brief For statistics on audio streams sent by local devices. See LocalAudioStats{@link #LocalAudioStats}.
     */
        public LocalAudioStats audio_stats;
    /** {zh}
     * @brief 本地设备发送视频流的统计信息，详见 LocalVideoStats{@link #LocalVideoStats} 。
     */
    /** {en}
     * @brief For statistics on video streams sent by local devices. See LocalVideoStats{@link #LocalVideoStats}.
     */
        public LocalVideoStats video_stats;
    /** {zh}
     * @hidden
     * @brief 本地媒体上行网络质量，详见 NetworkQuality{@link #NetworkQuality} 。
     * @deprecated since 3.36 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     */
    /** {en}
     * @hidden
     * @brief For local media uplink network quality. See NetworkQuality{@link #NetworkQuality}.
     * @deprecated since 3.45 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     */
        public NetworkQuality local_tx_quality;
    /** {zh}
     * @hidden
     * @brief 本地媒体下行网络质量，详见 NetworkQuality{@link #NetworkQuality} 。
     * @deprecated since 3.36 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     */
    /** {en}
     * @hidden
     * @brief Local media downlink network quality. See NetworkQuality{@link #NetworkQuality}.
     * @deprecated since 3.45 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     */
        public NetworkQuality local_rx_quality;
    /** {zh}
     * @brief 所属用户的媒体流是否为屏幕流。你可以知道当前统计数据来自主流还是屏幕流。
     */
    /** {en}
     * @brief Whether the media stream belongs to the user is a screen stream. You can know whether the current statistics come from mainstream or screen stream.
     */
        public bool is_screen;

    };

/** {zh}
 * @type keytype
 * @brief 本地音频流统计信息，统计周期为 2s 。  <br>
 *        本地用户发布音频流成功后，SDK 会周期性地通过 `OnLocalStreamStatsEventHandler` 通知用户发布的音频流在此次统计周期内的发送状况。此数据结构即为回调给用户的参数类型。  <br>
 */
/** {en}
 * @type keytype
 * @brief Local audio stream statistics, reference period 2s. <br>
 *        After the local user publishes the audio stream successfully, the SDK will periodically notify the user through `OnLocalStreamStatsEventHandler` the transmission status of the published audio stream during this reference period. This data structure is the type of parameter that is called back to the user. <br>
 */
    public struct LocalAudioStats {
    /** {zh}
     * @brief 音频丢包率。此次统计周期内的音频上行丢包率，单位为 % ，取值范围为 [0, 1] 。  <br>
     */
    /** {en}
     * @brief Audio packet loss rate. The audio uplink packet loss rate in this reference period is % and the value range is [0,1]. <br>
     */
        public float audio_loss_rate;
    /** {zh}
     * @brief 发送码率。此次统计周期内的音频发送码率，单位为 kbps 。  <br>
     */
    /** {en}
     * @brief Send rate. The audio transmission rate in the reference period is kbps. <br>
     */
        public int send_kbitrate;
    /** {zh}
     * @brief 采集采样率。此次统计周期内的音频采集采样率信息，单位为 Hz 。  <br>
     */
    /** {en}
     * @brief Acquisition sampling rate. Audio sampling rate information collected in the reference period, in units of Hz. <br>
     */
        public int record_sample_rate;
    /** {zh}
     * @brief 统计间隔。此次统计周期的间隔，单位为 ms 。  <br>
     *        此字段用于设置回调的统计周期，默认设置为 2s 。
     */
    /** {en}
     * @brief Statistical interval. The interval of this reference period is in ms. <br>
     *        This field is used to set the reference period for the callback. The default setting is 2s.
     */
        public int stats_interval;
    /** {zh}
     * @brief 往返时延。单位为 ms 。  <br>
     */
    /** {en}
     * @brief Round-trip time. The unit is ms. <br>
     */
        public int rtt;
    /** {zh}
     * @brief 音频声道数。  <br>
     */
    /** {en}
     * @brief Number of audio channels.   <br>
     */
        public int num_channels;
    /** {zh}
     * @brief 音频发送采样率。此次统计周期内的音频发送采样率信息，单位为 Hz 。  <br>
     */
    /** {en}
     * @brief Audio transmission sampling rate. Audio transmission sampling rate information in the reference period, in Hz. <br>
     */
        public int sent_sample_rate;
    /** {zh}
     * @brief 音频上行网络抖动，单位为 ms 。  <br>
     */
    /** {en}
     * @brief Audio uplink network jitter in ms. <br>
     */
        public int jitter;
    };

/** {zh}
 * @type keytype
 * @brief 本地视频流统计信息，统计周期为 2s 。  <br>
 *        本地用户发布视频流成功后，SDK 会周期性地通过 `OnLocalStreamStatsEventHandler` 通知用户发布的视频流在此次统计周期内的发送状况。此数据结构即为回调给用户的参数类型。  <br>
 */
/** {en}
 * @type keytype
 * @brief Local video stream statistics, reference period 2s. <br>
 *        After a local user publishes a video stream successfully, the SDK will periodically notify the user through `OnLocalStreamStatsEventHandler`.
 *        The delivery status of the published video stream during this reference period. This data structure is the type of parameter that is called back to the user. <br>
 */
    public struct LocalVideoStats {
    /** {zh}
     * @brief 发送码率。此次统计周期内实际发送的分辨率最大的视频流的发送码率，单位为 Kbps 
     */
    /** {en}
     * @brief TX bitrate in Kbps of the video stream with the highest resolution within the reference period
     */
        public int sent_kbitrate;
    /** {zh}
     * @brief 采集帧率。此次统计周期内的视频采集帧率，单位为 fps 。
     */
    /** {en}
     * @brief Sampling frame rate in fps of video capture during this reference period
     */
        public int input_frame_rate;
    /** {zh}
     * @brief 发送帧率。此次统计周期内实际发送的分辨率最大的视频流的视频发送帧率，单位为 fps 。
     */
    /** {en}
     * @brief TX frame rate in fps of the video stream with the highest resolution within the reference period
     */
        public int sent_frame_rate;
    /** {zh}
     * @brief 编码器输出帧率。当前编码器在此次统计周期内实际发送的分辨率最大的视频流的输出帧率，单位为 fps 。
     */
    /** {en}
     * @brief Encoder-output frame rate in fps of the video stream with the highest resolution within the reference period
     */
        public int encoder_output_frame_rate;
    /** {zh}
     * @brief 本地渲染帧率。此次统计周期内的本地视频渲染帧率，单位为 fps 。
     */
    /** {en}
     * @brief Local-rendering frame rate in fps during this reference period
     */
        public int renderer_output_frame_rate;
    /** {zh}
     * @hidden
     */
    /** {en}
     * @hidden
     */
        public int target_kbitrate;
    /** {zh}
     * @hidden
     */
    /** {en}
     * @hidden
     */
        public int target_frame_rate;
    /** {zh}
     * @brief 统计间隔，单位为 ms 。
     *        此字段用于设置回调的统计周期，默认设置为 2s 。
     */
    /** {en}
     * @brief Reference period in ms.
     *        This field is used to set the reference period for the callback, which is 2 s by default.
     */
        public int stats_interval;
    /** {zh}
     * @brief 视频丢包率。此次统计周期内的视频上行丢包率，取值范围： [0，1] 。
     */
    /** {en}
     * @brief Video packet loss rate. The video uplink packet loss rate in this reference period ranges from  [0,1].
     */
        public float video_loss_rate;
    /** {zh}
     * @brief 往返时延，单位为 ms 。
     */
    /** {en}
     * @brief Round-trip time in ms.
     */
        public int rtt;
    /** {zh}
     * @brief 视频编码码率。此次统计周期内的实际发送的分辨率最大的视频流视频编码码率，单位为 Kbps 。
     */
    /** {en}
     * @brief Video encoding bitrate in Kbps of the video stream with the highest resolution within the reference period.
     */
        public int encoded_bitrate;
    /** {zh}
     * @brief 实际发送的分辨率最大的视频流的视频编码宽度，单位为 px 。
     */
    /** {en}
     * @brief Video encoding width in px of the video stream with the highest resolution within the reference period
     */
        public int encoded_frame_width;
    /** {zh}
     * @brief 实际发送的分辨率最大的视频流的视频编码高度，单位为 px 。
     */
    /** {en}
     * @brief Video encoding height in px of the video stream with the highest resolution within the reference period
     */
        public int encoded_frame_height;
    /** {zh}
     * @brief 此次统计周期内实际发送的分辨率最大的视频流的发送的视频帧总数。
     */
    /** {en}
     * @brief The total number of the video stream with the highest resolution within the reference period sent in the reference period.
     */
        public int encoded_frame_count;
    /** {zh}
     * @brief 视频的编码类型，具体参考 VideoCodecType{@link #VideoCodecType} 。
     */
    /** {en}
     * @brief For the encoding type of the video, please refer to VideoCodecType{@link #VideoCodecType}.
     */
        public VideoCodecType codec_type;
    /** {zh}
     * @brief 所属用户的媒体流是否为屏幕流。你可以知道当前统计数据来自主流还是屏幕流。
     */
    /** {en}
     * @brief Whether the media stream belongs to the user is a screen stream. You can know whether the current statistics come from mainstream or screen stream.
     */
        public bool is_screen;
    /** {zh}
     * @brief 视频上行网络抖动，单位为 ms 。
     */
    /** {en}
     * @brief Video uplink network jitter in ms.
     */
        public int jitter;
    };

    /** {zh} 
    * @type keytype
    * @brief 视频的编码类型
    */
    /** {en} 
    * @type keytype
    * @brief Video encoding type
    */
    public enum VideoCodecType {
    /** {zh} 
     * @brief 未知类型
     */
    /** {en} 
     * @brief Unknown type
     */
        kVideoCodecTypeUnknown = 0,
    /** {zh} 
     * @brief 标准 H264 编码格式
     */
    /** {en} 
     * @brief H.264 format
     */
        kVideoCodecTypeH264 = 1,
    /** {zh} 
     * @brief ByteVC1 编码格式
     */
    /** {en} 
     * @brief ByteVC1 format
     */
        kVideoCodecTypeByteVC1 = 2,
    };

    /** {zh}
    * @type keytype
    * @brief 用户订阅的远端音/视频流统计信息以及网络状况，统计周期为 2s。  <br>
    *        订阅远端用户发布音/视频流成功后，SDK 会周期性地通过 `OnRemoteStreamStatsEventHandler` 通知本地用户订阅的远端音/视频流在此次统计周期内的接收状况。此数据结构即为回调给本地用户的参数类型。  <br>
    */
    /** {en}
    * @type keytype
    * @brief The remote audio/video stream statistics and network status subscribed by the user, with a reference period of 2s. <br>
    *         After the remote user subscribed to successfully publish the audio/video stream, the SDK will periodically notify local users through `OnRemoteStreamStatsEventHandler` the reception status of the remote audio/video stream subscribed during this reference period. This data structure is the type of parameter that is called back to the local user. <br>
    */
    public struct RemoteStreamStats {
    /** {zh}
     * @brief 用户 ID 。音/视频来源的远端用户 ID 。  <br>
     */
    /** {en}
     * @brief User ID. The remote user ID of the audio/video source. <br>
     */
        public string uid;
    /** {zh}
     * @brief 远端音频流的统计信息，详见 RemoteAudioStats{@link #RemoteAudioStats}
     */
    /** {en}
     * @brief For statistics on remote audio streams. See RemoteAudioStats{@link #RemoteAudioStats}
     */
        public RemoteAudioStats audio_stats;
    /** {zh}
     * @brief 远端视频流的统计信息，详见 RemoteVideoStats{@link #RemoteVideoStats}
     */
    /** {en}
     * @brief For statistics on remote video streams. See RemoteVideoStats{@link #RemoteVideoStats}
     */
        public RemoteVideoStats video_stats;
    /** {zh}
     * @hidden
     * @deprecated since 3.36 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     * @brief 所属用户的媒体流上行网络质量，详见 NetworkQuality{@link #NetworkQuality} 。
     */
    /** {en}
     * @hidden
     * @deprecated since 3.45 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     * @brief For the uplink network quality of the media stream owned by the user. See NetworkQuality{@link #NetworkQuality}.
     */
        public NetworkQuality remote_tx_quality;
    /** {zh}
     * @hidden
     * @deprecated since 3.36 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     * @brief 所属用户的媒体流下行网络质量，详见 NetworkQuality{@link #NetworkQuality} 。
     */
    /** {en}
     * @hidden
     * @deprecated since 3.45 and will be deleted in 3.51, use onNetworkQuality{@link #IRTCRoomEventHandler#onNetworkQuality} instead.
     * @brief The downlink network quality of the media stream belongs to the user. See NetworkQuality{@link #NetworkQuality}.
     */
        public NetworkQuality remote_rx_quality;
    /** {zh}
     * @brief 所属用户的媒体流是否为屏幕流。你可以知道当前统计数据来自主流还是屏幕流。
     */
    /** {en}
     * @brief Whether the media stream belongs to the user is a screen stream. You can know whether the current statistics come from mainstream or screen stream.
     */
        public bool is_screen;
    };

/** {zh}
 * @type keytype
 * @brief 远端音频流统计信息，统计周期为 2s。  <br>
 *        本地用户订阅远端音频流成功后，SDK 会周期性地通过 `OnRemoteStreamStatsEventHandler` 通知本地用户订阅的音频流在此次统计周期内的接收状况。此数据结构即为回调给本地用户的参数类型。  <br>
 */
/** {en}
 * @type keytype
 * @brief Remote audio stream statistics, reference period 2s. <br>
 *         After a local user subscribes to a remote audio stream successfully, the SDK periodically notifies the local user of the reception status of the subscribed audio stream during this reference period through `OnRemoteStreamStatsEventHandler`. This data structure is the type of parameter that is called back to the local user. <br>
 */
    public struct RemoteAudioStats {
    /** {zh}
     * @brief 音频丢包率。统计周期内的音频下行丢包率，取值范围为 [0, 1] 。  <br>
     */
    /** {en}
     * @brief Audio packet loss rate. The audio downlink packet loss rate in the reference period, the value range is  [0,1]. <br>
     */
        public float audio_loss_rate;
    /** {zh}
     * @brief 接收码率。统计周期内的音频接收码率，单位为 kbps 。  <br>
     */
    /** {en}
     * @brief Receiving bit rate. The audio reception rate in the reference period in kbps. <br>
     */
        public int received_kbitrate;
    /** {zh}
     * @brief 音频卡顿次数。统计周期内的卡顿次数。  <br>
     */
    /** {en}
     * @brief Number of audio stalls.
     */
        public int stall_count;
    /** {zh}
     * @brief 音频卡顿时长。统计周期内的卡顿时长，单位为 ms 。  <br>
     */
    /** {en}
     * @brief Audio stall duration. Stall duration in the reference period in ms. <br>
     */
        public int stall_duration;
    /** {zh}
     * @brief 用户体验级别的端到端延时。从发送端采集完成编码开始到接收端解码完成渲染开始的延时，单位为 ms 。  <br>
     */
    /** {en}
     * @brief End-to-end latency at the user experience level. The delay from the start of encoding at the sending end to the start of decoding at the receiving end, in units of ms. <br>
     */
        public long e2e_delay;
    /** {zh}
     * @brief 播放采样率。统计周期内的音频播放采样率信息，单位为 Hz 。  <br>
     */
    /** {en}
     * @brief Play sample rate. Audio playback sample rate information within the reference period in Hz. <br>
     */
        public int playout_sample_rate;
    /** {zh}
     * @brief 统计间隔。此次统计周期的间隔，单位为 ms 。  <br>
     */
    /** {en}
     * @brief Statistical interval. The interval of this reference period is in ms. <br>
     */
        public int stats_interval;
    /** {zh}
     * @brief 客户端到服务端数据传输的往返时延，单位为 ms 。  <br>
     */
    /** {en}
     * @brief Round-trip time for client side to server level data transfer in ms. <br>
     */
        public int rtt;
    /** {zh}
     * @brief 发送端——服务端——接收端全链路数据传输往返时延。单位为 ms 。  <br>
     */
    /** {en}
     * @brief The sender-server level-the receiver-link data transmission round-trip time. The unit is ms. <br>
     */
        public int total_rtt;
    /** {zh}
     * @brief 远端用户发送的音频流质量。值含义参考 NetworkQuality{@link #NetworkQuality} 。  <br>
     */
    /** {en}
     * @brief The quality of the audio stream sent by the remote user. Value meaning reference NetworkQuality{@link #NetworkQuality}. <br>
     */
        public int quality;
    /** {zh}
     * @brief 因引入 jitter buffer 机制导致的延时。单位为 ms 。  <br>
     */
    /** {en}
     * @brief The delay caused by the introduction of the jitter buffer mechanism. The unit is ms. <br>
     */
        public int jitter_buffer_delay;
    /** {zh}
     * @brief 音频声道数。  <br>
     */
    /** {en}
     * @brief Number of audio channels.   <br>
     */
        public int num_channels;
    /** {zh}
     * @brief 音频接收采样率。统计周期内接收到的远端音频采样率信息，单位为 Hz 。  <br>
     */
    /** {en}
     * @brief Audio reception sampling rate. Remote audio sampling rate information received within the reference period, in Hz. <br>
     */
        public int received_sample_rate;
    /** {zh}
     * @brief 远端用户在加入房间后发生音频卡顿的累计时长占音频总有效时长的百分比。音频有效时长是指远端用户进房发布音频流后，除停止发送音频流和禁用音频模块之外的音频时长。
     */
    /** {en}
     * @brief The accumulated length of the audio card occurs after the remote user joins the room as a percentage of the total effective length of the audio. The effective duration of audio refers to the duration of audio other than stopping sending audio streams and disabling audio modules after remote users enter the room to publish audio streams.
     */
        public int frozen_rate;
    /** {zh}
     * @brief 音频丢包补偿(PLC) 样点总个数。  <br>
     */
    /** {en}
     * @brief Audio packet loss compensation (PLC)  total number of sample points. <br>
     */
        public int concealed_samples;
    /** {zh}
     * @brief 音频丢包补偿(PLC) 累计次数。  <br>
     */
    /** {en}
     * @brief Audio packet loss compensation (PLC)  cumulative times. <br>
     */
        public int concealment_event;
    /** {zh}
     * @brief 音频解码采样率。统计周期内的音频解码采样率信息，单位为 Hz 。  <br>
     */
    /** {en}
     * @brief Audio decoding sample rate. Audio decoding sample rate information in the reference period in Hz. <br>
     */
        public int dec_sample_rate;
    /** {zh}
     * @brief 此次订阅中，对远端音频流进行解码的累计耗时。单位为 s。
     */
    /** {en}
     * @brief Cumulative decoding time in seconds of the remote audio stream in this subscription.
     */
        public int dec_duration;
    /** {zh}
     * @brief 音频下行网络抖动，单位为 ms 。  <br>
     */
    /** {en}
     * @brief Audio downlink network jitter in ms. <br>
     */
        public int jitter;
    };

/** {zh}
 * @type keytype
 * @brief 远端视频流统计信息，统计周期为 2s 。  <br>
 *        本地用户订阅远端视频流成功后，SDK 会周期性地通过 `OnRemoteStreamStatsEventHandler`
 *        通知本地用户订阅的远端视频流在此次统计周期内的接收状况。此数据结构即为回调给本地用户的参数类型。  <br>
 */
/** {en}
 * @type keytype
 * @brief Remote video stream statistics, reference period 2s. <br>
 *         After the local user subscribes to the remote video stream successfully, the SDK will periodically notify the local user of the reception status of the remote video stream subscribed by `OnRemoteStreamStatsEventHandler`
 *         During this reference period. This data structure is the type of parameter that is called back to the local user. <br>
 */
   public struct RemoteVideoStats {
    /** {zh}
     * @brief 远端视频流宽度
     */
    /** {en}
     * @brief Remote Video Stream Width
     */
        public int width;
    /** {zh}
     * @brief 远端视频流高度
     */
    /** {en}
     * @brief Remote Video Stream Height
     */
        public int height;
    /** {zh}
     * @brief 视频丢包率。统计周期内的视频下行丢包率，单位为 % ，取值范围为 [0，1] 。
     */
    /** {en}
     * @brief Video packet loss rate. The video downlink packet loss rate in the reference period, in units of %, and the value range is [0,1].
     */
        public float video_loss_rate;
    /** {zh}
     * @brief 接收码率。统计周期内的视频接收码率，单位为 kbps 。
     */
    /** {en}
     * @brief Receiving bit rate. Video reception rate within the reference period, in kbps.
     */
        public int received_kbitrate;
    /** {zh}
     * @brief 解码器输出帧率。统计周期内的视频解码器输出帧率，单位 fps 。
     */
    /** {en}
     * @brief The decoder outputs the frame rate. Video decoder output frame rate within the reference period, in fps.
     */
        public int decoder_output_frame_rate;
    /** {zh}
     * @brief 渲染帧率。统计周期内的视频渲染帧率，单位 fps 。
     */
    /** {en}
     * @brief Render frame rate. The video rendering frame rate in the reference period, in fps.
     */
        public int renderer_output_frame_rate;
    /** {zh}
     * @brief 卡顿次数。统计周期内的卡顿次数。
     */
    /** {en}
     * @brief Number of cards. Number of cards in the reference period.
     */
        public int stall_count;
    /** {zh}
     * @brief 卡顿时长。统计周期内的视频卡顿总时长。单位 ms 。
     */
    /** {en}
     * @brief Catton duration. The total duration of the video card in the reference period. Unit ms.
     */
        public int stall_duration;
    /** {zh}
     * @brief 用户体验级别的端到端延时，从发送端采集完成编码开始到接收端解码完成渲染开始的延时，单位为毫秒
     */
    /** {en}
     * @brief User experience-level end-to-end delay, in milliseconds, from the time when the sender captures the encoding to the time when the receiver decodes the rendering
     */
        public long e2e_delay;
    /** {zh}
     * @brief 所属用户的媒体流是否为屏幕流。你可以知道当前统计数据来自主流还是屏幕流。
     */
    /** {en}
     * @brief Whether the media stream belongs to the user is a screen stream. You can know whether the current statistics come from mainstream or screen stream.
     */
        public bool is_screen;
    /** {zh}
     * @brief 统计间隔，此次统计周期的间隔，单位为 ms 。  <br>
     *        此字段用于设置回调的统计周期，目前设置为 2s 。
     */
    /** {en}
     * @brief Statistical interval, the interval of this reference period, in ms. <br>
     *        This field is used to set the reference period for the callback, currently set to 2s.
     */
        public int stats_interval;
    /** {zh}
     * @brief 往返时延，单位为 ms 。
     */
    /** {en}
     * @brief Round-trip time in ms.
     */
        public int rtt;
    /** {zh}
     * @brief 远端用户在进房后发生视频卡顿的累计时长占视频总有效时长的百分比（%）。视频有效时长是指远端用户进房发布视频流后，除停止发送视频流和禁用视频模块之外的视频时长。
     */
    /** {en}
     * @brief The cumulative duration of the video card of the remote user accounts for the percentage (%) of the total effective duration of the video after entering the room. The effective duration of the video refers to the duration of the video other than stopping sending the video stream and disabling the video module after the remote user enters the room to publish the video stream.
     */
        public int frozen_rate;
    /** {zh}
     * @brief 对应多种分辨率的流的下标。
     */
    /** {en}
     * @brief For subscripts for streams with multiple resolutions.
     */
        public int video_index;
    /** {zh}
     * @brief 视频下行网络抖动，单位为 ms。
     */
    /** {en}
     * @brief Video downlink network jitter in ms.
     */
        public int jitter;

    };
    /** 
 * @type keytype
 * @brief 媒体流跨房间转发的目标房间的相关信息
 */
   public struct ForwardStreamInfo
    {
        /** 
         * @brief 使用转发目标房间 roomID 和 UserID 生成 Token。<br>
         *        测试时可使用控制台生成临时 Token，正式上线需要使用密钥 SDK 在你的服务端生成并下发 Token。<br>
         *        如果 Token 无效，转发失败。
         */
        public string token;
        /** 
         * @brief 跨间转发媒体流过程中目标房间 ID<br>
         */
        public string room_id;
    };

    /** 
     * @type keytype
     * @brief 媒体流跨房间转发的目标房间的相关信息
     */
    [StructLayout(LayoutKind.Sequential)]
    public struct ForwardStreamConfiguration
    {
        /** 
         * @brief 目标房间信息，数组中的每个元素包含一个房间的信息。
         */
        public IntPtr  forward_stream_dests;
        /** 
         * @brief 目标房间数量。媒体流同时转发的目标房间数量建议小于等于 4 个。
         */
        public int dest_count;
    };
}