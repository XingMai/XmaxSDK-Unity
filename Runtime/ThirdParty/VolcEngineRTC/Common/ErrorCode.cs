using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bytertc
{
    /** {zh}
     * @type errorcode
     * @brief 回调错误码。
     *        SDK 内部遇到不可恢复的错误时，会通过 `onError` 回调通知用户。
     */
    /** {en}
     * @type errorcode
     * @brief Callback error code.
     *        When an unrecoverable error is encountered inside the SDK, the user is notified via the `onError` callback.
     */
    public class ErrorCode {
        /** {zh}
         * @brief Token 无效。
         *        进房时使用的 Token 无效或过期失效。需要用户重新获取 Token，并调用 `updateToken` 方法更新 Token。
         */
        /** {en}
         * @brief Token  is invalid.
         *        The token used when entering the room is invalid or expired. The user is required to retrieve the token and call the `updateToken` method to update the token.
         */
        public static int ERROR_CODE_INVALID_TOKEN = -1000;
        /** {zh}
         * @brief 加入房间错误。
         * 进房时发生未知错误导致加入房间失败。需要用户重新加入房间。
         */
        /** {en}
         * @brief Join room error.
         * An unknown error occurred while entering the room, which caused the joining room to fail. Users are required to rejoin the room.
         */
        public static int ERROR_CODE_JOIN_ROOM = -1001;
        /** {zh}
         * @brief 没有发布音视频流权限。
         *        用户在所在房间中发布音视频流失败，失败原因为用户没有发布流的权限。
         */
        /** {en}
         * @brief No permission to publish audio & video streams.
         *        The user failed to publish the audio & video stream in the room. The reason for the failure is that the user does not have permission to publish the stream.
         */
        public static int ERROR_CODE_NO_PUBLISH_PERMISSION = -1002;
        /** {zh}
         * @brief 没有订阅音视频流权限。
         *        用户订阅所在房间中的音视频流失败，失败原因为用户没有订阅流的权限。
         */
        /** {en}
         * @brief No subscription permissions for audio & video streams.
         *        The user failed to subscribe to the audio & video stream in the room where the user is located. The reason for the failure is that the user does not have permission to subscribe to the stream.
         */
        public static int ERROR_CODE_NO_SUBSCRIBE_PERMISSION = -1003;
        /** {zh}
         * @brief 相同用户 ID 的用户加入本房间，当前用户被踢出房间
         */
        /** {en}
         * @brief The user has been removed from the room because the same user joined the room on the other client.
         */
        public static int ERROR_CODE_DUPLICATE_LOGIN = -1004;
        /** {zh}
         * @brief App ID 参数异常。
         *        创建引擎时传入的 App ID 参数为空。
         */
        /** {en}
         * @brief The App ID parameter is abnormal.
         *        The App ID parameter passed in when the engine was created is empty.
         */
        public static int ERROR_CODE_APP_ID_NULL = -1005;

        /** {zh}
         * @brief 服务端调用 OpenAPI 将当前用户踢出房间
         */
        /** {en}
         * @brief The user has been remove from the room by the administrator via a OpenAPI call.
         */
        public static int ERROR_CODE_KICKED_OUT = -1006;

        /** {zh}
         * @brief 当调用 `createRoom` ，如果 roomId 非法，会返回 null，并抛出该错误
         */
        /** {en}
         * @brief When calling `createRoom`, if the roomId is illegal, it will return null and throw the error
         */
        public static int ERROR_CODE_ROOM_ID_ILLEGAL = -1007;

        /** {zh}
         * @brief Token 过期。调用 `joinRoom` 使用新的 Token 重新加入房间。
         */
        /** {en}
         * @brief Token expired. Call `joinRoom` to rejoin with a valid Token.
         */
        public static int ERROR_CODE_TOKEN_EXPIRED = -1009;
        /** {zh}
         * @brief 调用 `updateToken` 传入的 Token 无效
         */
        /** {en}
         * @brief The Token you provided when calling `updateToken` is invalid.
         */
        public static int ERROR_CODE_UPDATE_TOKEN_WITH_INVALID_TOKEN = -1010;

        /** {zh}
         * @brief 服务端调用 OpenAPI 解散房间，所有用户被移出房间。
         */
        /** {en}
         * @brief Users have been removed from the room because the administrator dismissed the room by calling OpenAPI.
         */
        public static int ERROR_CODE_ROOM_DISMISS = -1011;

        /** {zh}
         * @brief 加入房间错误。  <br>
         *        调用 `joinRoom` 方法时, LICENSE 计费账号未使用 LICENSE_AUTHENTICATE SDK，加入房间错误。
         */
        /** {en}
         * @brief Join room error. <br>
         *        The LICENSE billing account does not use the LICENSE_AUTHENTICATE SDK while calling `joinRoom`, which caused the joining room to fail.
         */
        public static int ERROR_CODE_JOIN_ROOM_WITHOUT_LICENSE_AUTHENTICATE_SDK = -1012;

        /** {zh}
         * @brief 通话回路检测已经存在同样 roomId 的房间了
         */
        /** {en}
         * @brief A room with the same roomId already exists during the call echo test.
         */
        public static int ERROR_CODE_ROOM_ALREADY_EXIST = -1013;
        /** {zh}
         * @brief 加入多个房间时使用了不同的 uid。<br>
         *        同一个引擎实例中，用户需使用同一个 uid 加入不同的房间。
         */
        /** {en}
         * @brief The local user joins multiple rooms with different uid.<br>
         *        In the same engine instance, users need to use the same uid to join different rooms.
         */
        public static int ERROR_CODE_USER_ID_DIFFERENT = -1014;

        /** {zh}
         * @brief 服务端license过期，拒绝进房。 <br>
         */
        /** {en}
         * @brief Server license expired, refused to enter the room. <br>
         */
        public static int ERROR_CODE_SERVER_LICENSE_EXPIRE = -1017;
        /** {zh}
         * @brief 超过服务端license许可的并发量上限，拒绝进房。 <br>
         */
        /** {en}
         * @brief Exceeds the upper limit of the concurrency allowed by the server license, and refuses to enter the room. <br>
         */
        public static int ERROR_CODE_EXCEEDS_THE_UPPER_LIMIT = -1018;
        /** {zh}
         * @brief license参数错误，拒绝进房。 <br>
         */
        /** {en}
         * @brief The license parameter is wrong and refuses to enter the room. <br>
         */
        public static int ERROR_CODE_LICENSE_PARAMETER_ERROR = -1019;
        /** {zh}
         * @brief license证书路径错误。 <br>
         */
        /** {en}
         * @brief wrong license certificate path. <br>
         */
        public static int ERROR_CODE_LICENSE_FILE_PATH_ERROR = -1020;
        /** {zh}
         * @brief license证书不合法。 <br>
         */
        /** {en}
         * @brief The license certificate is illegal, refuse to enter the room. <br>
         */
        public static int ERROR_CODE_LICENSE_ILLEGAL = -1021;
        /** {zh}
         * @brief license证书已经过期，拒绝进房。 <br>
         */
        /** {en}
         * @brief License certificate has expired, refused to enter the room. <br>
         */
        public static int ERROR_CODE_LICENSE_EXPIRED = -1022;
        /** {zh}
         * @brief license证书内容不匹配。 <br>
         */
        /** {en}
         * @brief The content of the license certificate does not match. <br>
         */
        public static int ERROR_CODE_LICENSE_INFORMATION_NOT_MATCH = -1023;
        /** {zh}
         * @brief license当前证书与缓存证书不匹配。 <br>
         */
        /** {en}
         * @brief licenseThe current certificate does not match the cached certificate. <br>
         */
        public static int ERROR_CODE_LICENSE_NOT_MATCH_WITH_CACHE = -1024;
        /** {zh}
         * @brief 订阅音视频流失败，订阅音视频流总数超过上限。
         *        游戏场景下为了保证音视频通话的性能和质量，服务器会限制用户订阅的音视频流的总数。当用户订阅的音视频流总数已达上限时，继续订阅更多流时会失败，同时用户会收到此错误通知。
         */
        /** {en}
         * @brief Failed to subscribe to streams because the total number of subscribed streams has exceeded the upper limit.
         *        In order to ensure the performance and quality of audio & video calls in the game scenario, the server will limit the total number of audio & video streams subscribed by the user. When the total number of audio & video streams subscribed by the user has reached the maximum, continuing to subscribe to more streams will fail, and the user will receive this error notification.
         */
        public static int ERROR_CODE_OVER_SUBSCRIBE_LIMIT = -1070;
        /**
         * @hidden
         */
        public static int ERROR_CODE_LOAD_SO_LIB = -1072;
        /** {zh}
         * @brief 发布流失败，发布流总数超过上限。
         *        RTC 系统会限制单个房间内发布的总流数，总流数包括视频流、音频流和屏幕流。如果房间内发布流数已达上限时，本地用户再向房间中发布流时会失败，同时会收到此错误通知。
         */
        /** {en}
         * @brief Failed to publish video stream because the total number of published streams has exceeded the upper limit.
         *        RTC will limit the total number of streams published in a single room, including video, audio, and screen streams. Local users will fail to publish streams to the room when the maximum number of published streams in the room has been reached, and will receive this error notification.
         */
        public static int ERROR_CODE_OVER_STREAM_PUBLISH_LIMIT = -1080;
        /** {zh}
         * @brief 发布屏幕流失败，发布流总数超过上限。
         *        RTC 系统会限制单个房间内发布的总流数，总流数包括视频流、音频流和屏幕流。如果房间内发布流数已达上限时，本地用户再向房间中发布流时会失败，同时会收到此错误通知。
         */
        /** {en}
         * @brief Publishing the screen stream failed, and the total number of publishing streams exceeded the upper limit. The
         *        RTC will limit the total number of streams published in a single room, including video, audio, and screen streams. <br>
         *        Local users will fail to publish streams to the room when the maximum number of published streams in the room has been reached, and will receive this error notification.
         */
        public static int ERROR_CODE_OVER_SCREEN_PUBLISH_LIMIT = -1081;
        /** {zh}
         * @brief 发布视频流总数超过上限。
         *        RTC 系统会限制单个房间内发布的视频流数。如果房间内发布视频流数已达上限时，本地用户再向房间中发布视频流时会失败，同时会收到此错误通知。
         */
        /** {en}
         * @brief The total number of published video streams exceeds the upper limit.
         *        The RTC system limits the number of video streams posted in a single room. If the maximum number of video streams posted in the room has been reached, local users will fail to post video streams to the room again and will receive this error notification.
         */
        public static int ERROR_CODE_OVER_VIDEO_PUBLISH_LIMIT = -1082;
        /** {zh}
         * @brief 音视频同步失败。  <br>
         *        当前音频源已与其他视频源关联同步关系。  <br>
         *        单个音频源不支持与多个视频源同时同步。
         */
        /** {en}
         * @brief A/V synchronization failed.  <br>
         *        Current source audio ID has been set by other video publishers in the same room.  <br>
         *        One single audio source cannot be synchronized with multiple video sources at the same time.
         */
        public static int ERROR_CODE_INVALID_AUDIO_SYNC_USERID_REPEATED = -1083;
        /** {zh}
         * @brief 服务端异常状态导致退出房间。  <br>
         *        SDK与信令服务器断开，并不再自动重连，可联系技术支持。  <br>
         */
        /** {en}
         * @brief The user has been removed from the room due to the abnormal status of server. <br>
         *        SDK  is disconnected with the signaling server. It will not reconnect automatically. Please contact technical support.<br>
         */
        public static int ERROR_CODE_ABNORMAL_SERVER_STATUS = -1084;
        /** {zh}
         * @hidden
         * @brief 错误码构造函数
         */
        /** {en}
         * @hidden
         * @brief  Error code constructor
         */
        ErrorCode() { }
    }
}
