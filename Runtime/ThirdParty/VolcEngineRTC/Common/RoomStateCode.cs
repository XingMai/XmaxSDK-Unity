using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bytertc
{
    /**
     * @brief 加入房间错误码
     */
    public enum RoomStateCode
    {
        /**
         * @brief 加入房间成功
         */
        JOIN_ROOM_SUCCESS = 0,
        /**
         * @brief Token 无效。
         *        调用 JoinRoom 方法时使用的 Token 无效或过期失效。需要用户重新获取 Token，并调用
         *        RenewToken 方法更新 Token。
         */
        JOIN_ROOM_INVALID_TOKEN = -1000,
        /**
         * @brief 加入房间错误。
         *        调用 JoinRoom 方法时发生未知错误导致加入房间失败。需要用户重新加入房间。
         */
        JOIN_ROOM_ERROR = -1001,
        /**
         * @brief 加入房间失败。
         *        用户调用 JoinRoom 加入房间或由于网络状况不佳断网重连时，由于服务器错误导致用户加入房间失败，
         *        SDK 会自动重试加入房间。
         */
        JOIN_ROOM_FAILED = -2001,
        /**
         * @brief 本端用户所在房间中有相同用户ID的用户登录，导致本端用户被踢出房间
         */
        JOIN_ROOM_DUPLICATE_LOGIN = -1004

    }
}