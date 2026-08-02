using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    #region EventHandler


    #endregion

    /** {zh}
     * @type api
     * @brief 音频设备管理
     */
    /** {en}
     * @type api
     * @brief Audio device management.
     */
    public interface IAudioDeviceManager
    {
        /** {zh}
         * @type api
         * @brief 获取当前系统内音频播放设备列表。如果后续设备有变更，你需要重新调用本接口以获得新的设备列表。
         * @return 包含系统中所有音频播放设备的列表，参看 IDeviceCollection{@link #IDeviceCollection}。
         */
        /** {en}
         * @type api
         * @brief Gets a list of the audio playback device.  If there are subsequent device changes, you need to call this interface again to get a new device list.
         * @return A list of all audio playback devices. See IDeviceCollection{@link #IDeviceCollection}.
         */
        IDeviceCollection EnumerateAudioPlaybackDevices();

        /** {zh}
         * @type api
         * @brief 获取当前系统内音频采集设备列表。如果后续设备有变更，你需要重新调用本接口以获得新的设备列表。
         * @return 包含系统中所有音频采集设备的列表，参看 IDeviceCollection{@link #IDeviceCollection}。
         */
        /** {en}
         * @type api
         * @brief  Get a list of audio acquisition devices in the current system. If there are subsequent device changes, you need to call this interface again to get a new device list.
         * @return An object that contains a list of all audio capture devices in the system. See IDeviceCollection{@link #IDeviceCollection}.
         */
        IDeviceCollection EnumerateAudioCaptureDevices();

        /** {zh}
         * @type api
         * @brief 设置音频播放路由是否跟随系统。
         * @param followed <br>
         *        + true: 跟随。此时，调用 SetAudioPlaybackDevice{@link #IAudioDeviceManager#SetAudioPlaybackDevice} 会失败。
         *        + false: 不跟随系统。此时，可以调用 SetAudioPlaybackDevice{@link #IAudioDeviceManager#SetAudioPlaybackDevice} 进行设置。
         */
        /** {en}
         * @type api
         * @brief Set the audio playback device to follow the OS default setting or not.
         * @param followed
         *        + true: follow the os default setting. You can not call SetAudioPlaybackDevice{@link #IAudioDeviceManager#SetAudioPlaybackDevice} at the same time.
         *        + false: do not follow the os default setting. You can call SetAudioPlaybackDevice{@link #IAudioDeviceManager#SetAudioPlaybackDevice} to set the audio playback device.
         */
        void FollowSystemPlaybackDevice(Boolean followed);

        /** {zh}
         * @type api
         * @brief 设置音频采集路由是否跟随系统。
         * @param followed <br>
         *        + true: 跟随。此时，调用 SetAudioCaptureDevice{@link #IAudioDeviceManager#SetAudioCaptureDevice} 会失败。
         *        + false: 不跟随系统。此时，可以调用 SetAudioCaptureDevice{@link #IAudioDeviceManager#SetAudioCaptureDevice} 进行设置。
         */
        /** {en}
         * @type api
         * @brief Set the audio capture device to follow the OS default setting or not.
         * @param followed
         *        + true: follow the os default setting. You can not call SetAudioCaptureDevice{@link #IAudioDeviceManager#SetAudioCaptureDevice} at the same time.
         *        + false: do not follow the os default setting. You can call SetAudioCaptureDevice{@link #IAudioDeviceManager#SetAudioCaptureDevice} to set the audio capture device.
         */
        void FollowSystemCaptureDevice(Boolean followed);

        /** {zh}
         * @type api
         * @brief 设置音频播放设备。
         * @param deviceID 音频播放设备 ID，可通过 EnumerateAudioPlaybackDevices{@link #EnumerateAudioPlaybackDevices} 获取。
         * @return   <br>
         *        + 0： 成功  <br>
         *        + < 0：失败 
         */
        /** {en}
         * @type api
         * @brief Sets the audio playback device.
         * @param deviceID Audio playback device's ID. You can get the ID by calling
         * EnumerateAudioPlaybackDevices{@link #EnumerateAudioPlaybackDevices}.
         * @return    <br>
         *         + 0: Success <br>
         *         + < 0: Failure 
         */
        int SetAudioPlaybackDevice(string deviceID);

        /** {zh}
         * @type api
         * @brief 设置音频采集设备。
         * @param deviceID 音频采集设备 ID，可通过 EnumerateAudioCaptureDevices{@link #EnumerateAudioCaptureDevices} 获取。
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Set up audio capture devices.
         * @param  deviceID Audio capture device ID, available through EnumerateAudioCaptureDevices{@link #EnumerateAudioCaptureDevices}.
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int SetAudioCaptureDevice(string deviceID);

        /** {zh}
         * @type api
         * @brief 设置当前音频播放设备音量
         * @param volume 音频播放设备音量，取值范围为 [0,255], 超出此范围设置无效。
         *       + [0,25] 接近无声；  <br>
         *       + [25,75] 为低音量；  <br>
         *       + [76,204] 为中音量；  <br>
         *       + [205,255] 为高音量。  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Sets the current audio playback device volume
         * @param  volume Audio playback device volume, the value range is [0,255], the setting beyond this range is invalid.
         *        + [0,25] Is nearly silent; <br>
         *        + [25,75] Is low volume; <br>
         *        + [76,204] Is medium volume; <br>
         *        + [205,255] Is high volume. <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int SetAudioPlaybackDeviceVolume(uint volume);

        /** {zh}
         * @type api
         * @brief 获取当前音频播放设备音量
         * @param volume 音频播放设备音量，范围应在 [0,255] 内。
         *       + [0,25] 接近无声；  <br>
         *       + [25,75] 为低音量；  <br>
         *       + [76,204] 为中音量；  <br>
         *       + [205,255] 为高音量。  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Get the current audio playback device volume
         * @param Volume Audio playback device volume, the range should be within [0,255].
         *        + [0,25] Is nearly silent; <br>
         *        + [25,75] Is low volume; <br>
         *        + [76,204] Is medium volume; <br>
         *        + [205,255] Is high volume. <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int GetAudioPlaybackDeviceVolume(ref uint volume);

        /** {zh}
         * @type api
         * @brief 设置当前音频采集设备音量
         * @param volume 音频采集设备音量，取值范围为 [0,255], 超出此范围设置无效。
         *       + [0,25] 接近无声；  <br>
         *       + [25,75] 为低音量；  <br>
         *       + [76,204] 为中音量；  <br>
         *       + [205,255] 为高音量。  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         * @notes 如果音频采集设备通过 SetAudioCaptureDeviceMute{@link #IAudioDeviceManager#SetAudioCaptureDeviceMute} 被静音，则音量调节会在取消静音后生效。
         */
        /** {en}
         * @type api
         * @brief Set the current audio acquisition device volume
         * @param  volume Audio acquisition device volume, the value range is [0,255], the setting beyond this range is invalid.
         *        + [0,25] Is nearly silent; <br>
         *        + [25,75] Is low volume; <br>
         *        + [76,204] Is medium volume; <br>
         *        + [205,255] Is high volume. <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int SetAudioCaptureDeviceVolume(uint volume);

        /** {zh}
         * @type api
         * @brief 获取当前音频采集设备音量
         * @param volume 音频采集设备音量，范围应在 [0,255] 内。
         *       + [0,25] 接近无声；  <br>
         *       + [25,75] 为低音量；  <br>
         *       + [76,204] 为中音量；  <br>
         *       + [205,255] 为高音量。  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Get the current audio capture device volume
         * @param Volume Audio capture device volume, the range should be within [0,255].
         *        + [0,25] Is nearly silent; <br>
         *        + [25,75] Is low volume; <br>
         *        + [76,204] Is medium volume; <br>
         *        + [205,255] Is high volume. <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int GetAudioCaptureDeviceVolume(ref uint volume);

        /** {zh}
         * @type api
         * @brief 设置当前音频播放设备静音状态，默认为非静音。
         * @param mute  <br>
         *       + true：静音  <br>
         *       + false：非静音  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Sets the current audio playback device to be mute, and the default is non-mute.
         * @param  mute <br>
         *        + True: mute <br>
         *        + False: non-mute <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int SetAudioPlaybackDeviceMute(bool mute);

        /** {zh}
         * @type api
         * @brief 获取当前音频播放设备是否静音的信息。
         * @param mute  <br>
         *       + true：静音  <br>
         *       + false：非静音  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Gets information about whether the current audio playback device is muted.
         * @param  mute <br>
         *        + True: mute <br>
         *        + False: non-mute <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int GetAudioPlaybackDeviceMute(ref bool mute);

        /** {zh}
         * @type api
         * @brief 设置当前音频采集设备静音状态，默认为非静音。
         * @param mute  <br>
         *       + true：静音  <br>
         *       + false：非静音  <br>
         * @return  
         *        + 0： 成功  <br>  
         *        + < 0：失败  <br>
         * @notes 
         * + 该方法用于静音整个系统的音频采集。
         * + 设该方法为 `true` 静音后仍可通过 SetAudioCaptureDeviceVolume{@link #IAudioDeviceManager#SetAudioCaptureDeviceVolume} 调整采集音量，调整后的音量会在取消静音后生效。
         */
        /** {en}
         * @type api
         * @brief Sets the current audio acquisition device to be mute, and the default is non-mute.
         * @param mute <br>
         *        + True: mute <br>
         *        + False: non-mute <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int SetAudioCaptureDeviceMute(bool mute);

        /** {zh}
         * @type api
         * @brief 获取当前音频采集设备是否静音的信息。
         * @param mute  <br>
         *       + true：静音  <br>
         *       + false：非静音  <br>
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Gets information about whether the current audio capture device is silent.
         * @param  mute <br>
         *        + True: mute <br>
         *        + False: non-mute <br>
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int GetAudioCaptureDeviceMute(ref bool mute);

        /** {zh}
         * @type api
         * @brief 获取当前音频播放设备 ID。
         * @param deviceID 音频播放设备 ID
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Gets the current audio playback device ID.
         * @param deviceID audio playback device ID
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int GetAudioPlaybackDevice(ref string deviceID);

        /** {zh}
         * @type api
         * @brief 获取当前音频采集设备 ID。
         * @param deviceID 音频采集设备 ID，使用方负责按 MAX_DEVICE_ID_LENGTH 大小，分配与释放内存
         * @return  
         *        + 0： 成功  <br>
         *        + < 0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Gets the current audio capture device ID.
         * @param deviceID audio capture device ID, the user is responsible for allocating and freeing memory according to the MAX_DEVICE_ID_LENGTH size
         * @return 
         *         + 0: Success. <br>
         *         + < 0: failure <br>
         */
        int GetAudioCaptureDevice(ref string deviceID);

    }

}