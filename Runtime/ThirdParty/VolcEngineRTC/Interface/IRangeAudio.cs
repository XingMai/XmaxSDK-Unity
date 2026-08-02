using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    #region EventHandler
    /** {zh}
     * @type callback
     * @brief 范围语音信息
     * @param roomID 房间 ID
     * @param info 范围语音信息，参看 RangeAudioInfo{@link #RangeAudioInfo}。
     */
    /** {en}
     * @type callback
     * @brief Range audio information.
     * @param roomID Room ID.
     * @param info Range audio information. See RangeAudioInfo{@link #RangeAudioInfo}.
     */
   // public delegate void OnRangeAudioInfoEventHandler(string roomID, List<RangeAudioInfo> info);

    #endregion

    /** {zh}
     * @type api
     * @brief 范围语音接口。
     */
    /** {en}
     * @type api
     * @brief  Range audio interface
     */
    public interface IRangeAudio
    {
        #region Events

        /** {zh}
         * @hidden
         * @brief 范围语音信息回调
         */
        /** {en}
         * @hidden
         * @brief Range audio information callback
         */
      //  event OnRangeAudioInfoEventHandler OnRangeAudioInfoEvent;

        #endregion

        /** {zh}
         * @type api
         * @brief 开启/关闭范围语音功能。  <br>
         *        范围语音是指，在同一 RTC 房间中设定的音频接收距离范围内，本地用户收听到的远端用户音频音量会随着远端用户的靠近/远离而放大/衰减；若远端用户在房间内的位置超出设定范围，则本地用户无法接收其音频。音频接收范围设置参看 UpdateReceiveRange{@link #IRangeAudio#UpdateReceiveRange}。
         * @param enable 是否开启范围语音功能：  <br>
         *        + true: 开启  <br>
         *        + false: 关闭（默认）
         * @notes 该方法进房前后都可调用，为保证进房后范围语音效果的平滑切换，你需在该方法前先调用 UpdateReceiveRange{@link #IRangeAudio#UpdateReceiveRange} 设置自身位置坐标，然后开启该方法收听范围语音效果。
         */
        /** {en}
         * @type api
         * @brief Enable/disable the range audio function.  <br>
         *        Range audio means that within a certain range in a same RTC room, the audio volume of the remote user received by the local user will be amplified/attenuated as the remote user moves closer/away. The audio coming from out of the range cannot be heard. See UpdateReceiveRange{@link #IRangeAudio#UpdateReceiveRange} to set audio receiving range.
         * @param enable Whether to enable audio range funcion：  <br>
         *        + true: Enable  <br>
         *        + false: Disable（Defaulting setting）
         * @notes You can call this API anytime before or after entering a room. To ensure a smooth switch to the range audio mode after entering the room, you need to call UpdateReceiveRange{@link #IRangeAudio#UpdateReceiveRange} before this API to set your own position coordinates, and then enable the range audio function.
         */
        void EnableRangeAudio(bool enable);

        /** {zh}
         * @type api
         * @brief 更新本地用户的音频收听范围。
         * @param range 音频收听范围，参看 ReceiveRange{@link #ReceiveRange}。
         * @return 方法调用结果：  <br>
         *        + 0：成功；  <br>
         *        + !0: 失败。
         */
        /** {en}
         * @type api
         * @brief Updates the audio receiving range for the local user.
         * @param range Audio receiving range, see ReceiveRange{@link #ReceiveRange}.
         * @return API call result:  <br>
         *        + 0: Success  <br>
         *        + !0: Failure
         */
        int UpdateReceiveRange(ReceiveRange range);

        /** {zh} 
         * @type api
         * @brief 更新本地用户在房间内空间直角坐标系中的位置坐标。
         * @param pos 三维坐标的值，默认为 [0, 0, 0]，参看 Position{@link #Position}。
         * @return  <br>
         *        + 0：成功；
         *        + !0：失败。
         * @notes  调用该接口更新坐标后，你需调用 EnableRangeAudio{@link #IRangeAudio#EnableRangeAudio} 开启范围语音功能以收听范围语音效果。
         */
        /** {en} 
         * @type api
         * @brief Updates the coordinate of the local user's position in the rectangular coordinate system in the current room.
         * @param pos 3D coordinate values, the default value is [0, 0, 0]. See Position{@link #Position}.
         * @return  <br>
         *        + 0: Success.
         *        + !0: Failure.
         * @notes  After calling this API, you should call EnableRangeAudio{@link #IRangeAudio#EnableRangeAudio} to enable range audio function to actually enjoy the range audio effect.  <br>
         */
        int UpdatePosition(Position pos);

        /** {zh}
       * @type api
       * @region 范围语音
       * @author huangshouqin
       * @brief 设置范围语音的音量衰减模式。<br>
       * @param type 音量衰减模式。默认为线性衰减。详见 AttenuationType{@link #AttenuationType}。
       * @param coefficient 指数衰减模式下的音量衰减系数，默认值为 1。范围 [0.1,100]，推荐设置为 `50`。数值越大，音量的衰减速度越快。
       * @return 调用是否成功<br>
       *         + `0`:调用成功<br>
       *         + `-1`:调用失败。原因为在调用 EnableRangeAudio{@link #IRangeAudio#EnableRangeAudio} 开启范围语音前或进房前调用本接口
       * @notes 音量衰减范围通过 UpdateReceiveRange{@link #IRangeAudio#UpdateReceiveRange} 进行设置。
       */
        /** {en}
         * @type api
         * @region Range Audio
         * @author huangshouqin
         * @brief Set the volume roll-off mode that a 3D sound has in an audio source when using the Range Audio feature.<br>
         * @param type Volume roll-off mode. It is linear roll-off mode by default. Refer to AttenuationType{@link #AttenuationType} for more details.
         * @param coefficient Coefficient for the exponential roll-off mode. The default value is 1. It ranges [0.1,100]. We recommended to set it to `50`. The volume roll-off speed gets faster as this value increases.
         * @return Result of the call<br>
         *         + `0`: Success<br>
         *         + `-1`: Failure because of calling this API before the user has joined a room or before enabling the Range Audio feature by calling EnableRangeAudio{@link #IRangeAudio#EnableRangeAudio}.
         * @notes Call UpdateReceiveRange{@link #IRangeAudio#UpdateReceiveRange} to set the range outside which the volume of the sound does not attenuate.
         */
        int SetAttenuationModel(AttenuationType type, float coefficient);

        /** {zh}
     * @type api
     * @region 范围语音
     * @author chuzhongtao
     * @brief 添加标签组，用于标记相互之间通话不衰减的用户组。<br>
     *        在同一个 RTC 房间中，如果多个用户的标签组之间有交集，那么，他们之间互相通话时，通话不衰减。<br>
     *        比如，用户身处多个队伍，队伍成员间通话不衰减。那么，可以为每个队伍绑定专属标签，每个用户的标签组包含用户所属各个队伍的标签。
     * @param flags 标签组  <br>
     */
        /** {en}
         * @type api
         * @region Range Audio
         * @author chuzhongtao
         * @brief Set the flags to mark the user groups, within which the users talk without attenuation. <br>
         *        In the RTC room, if the flags of the users intersects with each other, the users talk without attenuation. <br>
         *        For example, the user is a member of multiple teams, and teammates of the same team talks without attentuation. You can set the flag for each team, and includes the flags of the user's teams in the user's flags. 
         * @param flags Array of flags.
         */
        void SetNoAttenuationFlags(string[] flags, int len);
        /** {zh}
         * @type api
         * @brief 注册范围语音信息回调观察者
         * @param handler 范围语音信息观察者，参看 OnRangeAudioInfoEventOnRangeAudioInfoEventHandler{@link #EventHandler#OnRangeAudioInfoEventHandler}。
         */
        /** {en}
         * @type api
         * @brief Register range audio information observer.
         * @param handler Range audio information observer. See OnRangeAudioInfoEventOnRangeAudioInfoEventHandler{@link #EventHandler#OnRangeAudioInfoEventHandler}.
         */
        //void RegisterRangeAudioObserver(OnRangeAudioInfoEventHandler handler);

        /** {zh}
         * @type api
         * @brief 释放当前 IRangeAudio{@link #IRangeAudio} 对象占用的资源。
         */
        /** {en}
         * @type api
         * @brief  Releases the resources occupied by the current IRangeAudio{@link #IRangeAudio} object.
         */
        void Release();
    }

    /** {zh}
     * @type keytype
     * @brief 使用范围语音功能时，语音的接收范围
     */
    /** {en}
     * @type keytype
     * @brief When using the range speech function, the range of speech reception
     */
    public struct ReceiveRange {
        /** {zh}
         * @brief 收听声音无衰减的最小范围值。<br>
         *        当收听者和声源距离小于 min 的时候，收听到的声音完全无衰减。
         */
        /** {en}
         * @brief The minimum range value for listening to sound without attenuation. <br>
         *         When the distance between the listener and the sound source is less than min, the received sound has no attenuation at all.
         */
        public int min;
        /** {zh}
         *  @brief 能够收听到声音的最大范围。<br>
         *        当收听者和声源距离大于 max 的时候，无法收听到声音。<br>
         *        当收听者和声源距离处于 [min, max) 之间时，收听到的音量根据距离有衰减。
         */
        /** {en}
         *  @brief The maximum range of sound that can be heard. <br>
         *         When the distance between the listener and the sound source is greater than max, the sound cannot be heard. <br>
         *         When the distance between the listener and the sound source is between [min, max), the volume heard is attenuated according to the distance.
         */
        public int max;
    };

    /** {zh}
     * @type keytype
     * @brief 范围语音信息
     */
    /** {en}
     * @type keytype
     * @brief Range audio information.
     */
    public struct RangeAudioInfo {
        /** {zh} 
         * @brief 用户 ID。
         */
        /** {en} 
         * @brief User ID.
         */
        public string user_id;
        /** {zh} 
         * @brief 音量衰减量。取值范围是 `[0,100]`，随距离呈线性衰减，当 factor 为 0 时，表示听不到声音。
         */
        /** {en} 
         * @brief Volume attenuation. The value range is  '[0,100]', which attenuates linearly with distance. When the factor is 0, it means that the sound cannot be heard.
         */
        public int factor;
    };

    /** {zh}
    * @type keytype
    * @brief 空间音频音量随距离衰减模式
    */
    /** {en}
    * @type keytype
    * @brief Volume Roll-off modes that a sound has in an audio source
    */
    public enum AttenuationType {
        /** {zh}
           * @brief 不随距离衰减
           */
        /** {en}
         * @brief Disable Volume Attenuation
         */
        kAttenuationTypeNone = 0,
        /** {zh}
           * @brief 线性衰减，音量随距离增大而线性减小
           */
        /** {en}
         * @brief Linear roll-off mode which lowers the volume of the sound over the distance
         */
        kAttenuationTypeLinear = 1,
        /** {zh}
           * @brief 指数型衰减，音量随距离增大进行指数衰减
           */
        /** {en}
         * @brief Exponential roll-off mode which exponentially decreases the volume of the sound with the distance raising
         */
        kAttenuationTypeExponential = 2,
    };

}  // namespace bytertc
