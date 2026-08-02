using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bytertc
{
    /** {zh}
     * @type errorcode
     * @brief 回调警告码。
     *        警告码说明 SDK 内部遇到问题正在尝试恢复。警告码仅作通知。
     */
    /** {en}
     * @type errorcode
     * @brief Callback warning code. The warning code indicates that there is a problem within the SDK and is trying to recover. Warning codes are for notification only.
     */
    public class WarningCode {
        /** {zh}
         * @hidden
         * @brief 获取房间信息失败警告
         * @notes SDK 获取房间信息失败（包含超时，返回非 200 的错误码），每隔两秒重试一次。  <br>
         *        连续失败 5 次后，报该 warning，并继续重试。  <br>
         *        建议提示用户：进入房间失败，请稍后再试
         */
        /** {en}
         * @hidden
         * @brief Failed to get room information Warning
         * @notes SDK Failed to get room information (including timeout, returning an error code other than 200), try again every two seconds. <br>
         *         After 5 consecutive failures, report the warning and continue to try again. <br>
         *        Suggest to prompt the user: Failed to enter the room, please try again later
         */
        public static int WARNING_CODE_GET_ROOM_FAILED = -2000;
        /** {zh}
         * @brief 进房失败。  <br>
         *        初次进房或者由于网络状况不佳断网重连时，由于服务器错误导致进房失败。SDK 会自动重试进房。
         */
        /** {en}
         * @brief Failed to enter the room.   <br>
         *        When you enter the room for the first time or disconnect and reconnect due to poor network condition, the room entry failed due to a server error. The SDK automatically retries to join the room.
         */
        public static int WARNING_CODE_JOIN_ROOM_FAILED = -2001;
        /** {zh}
         * @brief 发布音视频流失败。  <br>
         *        当你在所在房间中发布音视频流时，由于服务器错误导致发布失败。SDK 会自动重试发布。
         */
        /** {en}
         * @brief Release audio & video stream failed.   <br>
         *        When you publish audio & video streams in your room, the publication fails due to a server error. The SDK automatically retries the release.
         */
        public static int WARNING_CODE_PUBLISH_STREAM_FAILED = -2002;
        /** {zh}
         * @brief 订阅音视频流失败。  <br>
         *        当前房间中找不到订阅的音视频流导致订阅失败。SDK 会自动重试订阅，若仍订阅失败则建议你退出重试。
         */
        /** {en}
         * @brief Subscription to audio & video stream failed.   <br>
         *         The subscription failed because the audio & video stream for the subscription could not be found in the current room. The SDK will automatically retry the subscription. If the subscription fails, it is recommended that you exit the retry.
         */
        public static int WARNING_CODE_SUBSCRIBE_STREAM_FAILED404 = -2003;
        /** {zh}
         * @brief 订阅音视频流失败。  <br>
         *        当你订阅所在房间中的音视频流时，由于服务器错误导致订阅失败。SDK 会自动重试订阅。
         */
        /** {en}
         * @brief Subscription to audio & video stream failed.   <br>
         *        When you subscribe to audio & video streams in your room, the subscription fails due to a server error. The SDK automatically retries the subscription.
         */
        public static int WARNING_CODE_SUBSCRIBE_STREAM_FAILED5XX = -2004;
        /** {zh}
         * @hidden
         * @brief 函数调用顺序错误。
         */
        /** {en}
         * @hidden
         * @brief  The function call order is wrong.
         */
        public static int WARNING_CODE_INVOKE_ERROR = -2005;
        /** {zh}
         * @hidden
         * @brief 调度异常，服务器返回的媒体服务器地址不可用。
         */
        /** {en}
         * @hidden
         * @brief Scheduling exception, the media server address returned by the server is unavailable.
         */
        public static int WARNING_CODE_INVALID_EXPECT_MEDIA_SERVER_ADDRESS = -2007;
        /** {zh}
         * @brief 当调用 `setUserVisibility` 将自身可见性设置为 false 后，再尝试发布流会触发此警告。
         */
        /** {en}
         * @brief This warning is triggered when a call to `setUserVisibility` sets its own visibility to false and then attempts to publish the flow.
         */
        public static int WARNING_CODE_PUBLISH_STREAM_FORBIDEN = -2009;
        /** {zh}
         * @hidden
         * @brief 自动订阅模式未关闭时，尝试开启手动订阅模式会触发此警告。  <br>
         *        你需在进房前调用 enableAutoSubscribe{@link #RTCVideo#enableAutoSubscribe} 方法关闭自动订阅模式，再调用 subscribeUserStream{@link #RTCVideo#subscribeUserStream} 方法手动订阅音视频流。
         */
        /** {en}
         * @hidden
         * @brief When automatic subscription mode is not turned off, trying to turn on manual subscription mode will trigger this warning.   <br>
         *        You need to call the enableAutoSubscribe{@link #RTCVideo#enableAutoSubscribe} method to turn off the automatic subscription mode before entering the room, and then call the subscribeUserStream{@link #RTCVideo#subscribeUserStream} method to manually subscribe to the audio & video stream.
         */
        public static int WARNING_CODE_SUBSCRIBE_STREAM_FORBIDEN = -2010;
        /** {zh}
         * @brief 发送自定义广播消息失败, 当前你未在房间中。
         */
        /** {en}
         * @brief Sending a custom broadcast message failed, you are not currently in the room.
         */
        public static int WARNING_CODE_SEND_CUSTOM_MESSAGE = -2011;
        /** {zh}
         * @brief 当房间内人数超过 500 人时，停止向房间内已有用户发送 `onUserJoined` 和 `onUserLeave` 回调，并通过广播提示房间内所有用户。
         */
        /** {en}
         * @brief When the number of people in the room exceeds 500, stop sending `onUserJoined` and `onUserLeave` callbacks to existing users in the room, and prompt all users in the room via broadcast.
         */
        public static int WARNING_CODE_RECEIVE_USER_NOTIFY_STOP = -2013;
        /** {zh}
         * @brief 用户已经在其他房间发布过流，或者用户正在发布公共流。
         */
        /** {en}
         * @brief User had published in other room.
         */
        public static int WARNING_CODE_USER_IN_PUBLISH = -2014;
        /** {zh}
         * @hidden
         * @brief 已存在同样 roomId 的房间。
         */
        /** {en}
         * @hidden
         * @brief there is a room with the same roomId
         */
        public static int WARNING_CODE_ROOM_ID_ALREADY_EXIST = -2015;

        /** {zh}
         * @brief 已存在相同 roomId 的房间，新创建的房间实例已替换旧房间实例。
         */
        /** {en}
         * @brief A room with the same roomId already exists. The newly created room instance has replaced the old one.
         */
        public static int WARNING_CODE_OLD_ROOM_BEEN_REPLACED = -2016;

        /** {zh}
         * @brief 当前正在进行回路测试，该接口调用无效
         */
        /** {en}
         * @brief this inteface has been banned for the engine is in echo testing mode
         */
        public static int WARNING_CODE_IN_ECHO_TEST_MODE = -2017;
        /** {zh}
         * @brief 摄像头权限异常，当前应用没有获取摄像头权限。
         */
        /** {en}
         * @brief The camera permission is abnormal, and the current application does not obtain the camera permission.
         */
        public static int WARNING_CODE_NO_CAMERA_PERMISSION = -5001;
        /** {zh}
         * @hidden
         * @brief 麦克风权限异常，当前应用没有获取麦克风权限。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief The microphone permission is abnormal, and the current application does not obtain microphone permission.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_NO_MICROPHONE_PERMISSION = -5002;
        /** {zh}
         * @hidden
         * @brief 音频采集设备启动失败。  <br>
         *        启动音频采集设备失败，当前设备可能被其他应用占用。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief The audio capture device failed to start.   <br>
         *        Failed to start the audio capture device, the current device may be occupied by other applications.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_RECODING_DEVICE_START_FAILED = -5003;
        /** {zh}
         * @hidden
         * @brief 音频播放设备启动失败警告。   <br>
         *      可能由于系统资源不足，或参数错误。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief Audio playback device startup failure warning.    <br>
         *       It may be due to insufficient system resources or wrong parameters.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_PLAYOUT_DEVICE_START_FAILED = -5004;
        /** {zh}
         * @hidden
         * @brief 无可用音频采集设备。  <br>
         *        启动音频采集设备失败，请插入可用的音频采集设备。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief No audio acquisition equipment available.   <br>
         *        Failed to start the audio capture device, please insert the available audio capture device.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_NO_RECORDING_DEVICE = -5005;
        /** {zh}
         * @hidden
         * @brief 无可用音频播放设备。  <br>
         *        启动音频播放设备失败，请插入可用的音频播放设备。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief No audio playback device available.   <br>
         *        Failed to start the audio playback device, please insert the available audio playback device.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_NO_PLAYOUT_DEVICE = -5006;
        /** {zh}
         * @hidden
         * @brief 当前音频设备没有采集到有效的声音数据，请检查更换音频采集设备。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief The current audio equipment has not collected valid sound data, please check and replace the audio acquisition equipment.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_RECORDING_SILENCE = -5007;
        /** {zh}
         * @hidden
         * @brief 媒体设备误操作警告。  <br>
         *        使用自定义采集时，不可调用内部采集开关，调用时将触发此警告。
         * @deprecated since 3.33, use MediaDeviceWarning instead
         */
        /** {en}
         * @hidden
         * @brief Media device misoperation warning.   <br>
         *        When using custom acquisition, the internal acquisition switch cannot be called, and this warning will be triggered when called.
         * @deprecated since 3.45, use MediaDeviceWarning instead
         */
        public static int WARNING_CODE_MEDIA_DEVICE_OPERATION_DENIED = -5008;
        /** {zh}
         * @brief 不支持在 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 之后，调用 SetScreenAudioSourceType{@link #IRTCVideo#SetScreenAudioSourceType} 设置屏幕音频采集类型
         */
        /** {en}
         * @brief Setting the screen audio capture type by calling SetScreenAudioSourceType{@link #IRTCVideo#SetScreenAudioSourceType} after PublishScreen{@link #IRTCVideoRoom#PublishScreen} is not supported
         */
        public static int WARNING_CODE_SET_SCREEN_AUDIO_SOURCE_TYPE_FAILED = -5009;
        /** {zh}
         * @brief 不支持在 PublishScreen{@link #IRTCVideoRoom#PublishScreen} 之后，调用 SetScreenAudioStreamIndex{@link #IRTCVideo#SetScreenAudioStreamIndex} 设置屏幕音频共享发布类型
         */
        /** {en}
         * @brief After PublishScreen{@link #IRTCVideoRoom#PublishScreen}, calling SetScreenAudioStreamIndex{@link #IRTCVideo#SetScreenAudioStreamIndex} to set the screen audio share publication type is not supported
         */
        public static int WARNING_CODE_SET_SCREEN_STREAM_INDEX_FAILED = -5010;
        /** {zh}
         * @brief 设置语音音高不合法
         */
        /** {en}
         * @brief Invalid pitch value setting
         */
        public static int WARNING_CODE_SET_SCREEN_STREAM_INVALID_VOICE_PITCH = -5011;

        /** {zh}
         * @brief 外部音频源新旧接口混用
         */
        /** {en}
         * @brief Mixed use of old and new interfaces for external audio sources
         */
        public static int WARNING_CODE_INVALID_CALL_FOR_EXT_AUDIO = -5013;
        /** {zh}
         * @brief 非法的远端流索引
         */
        /** {en}
         * @brief Invalid remote streamKey 
         */
        public static int WARNING_CODE_INVALID_REMOTE_STREAM_KEY = -5014;

        /** {zh}
         * @hidden
         * @brief 警告码构造函数
         */
        /** {en}
         * @hidden
         * @brief  Warning code constructor
         */
        public WarningCode() { }
    }
}
