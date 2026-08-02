using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    #region EventHandler


    #endregion

    /** {zh}
     * @type api
     * @brief 音视频设备相关的信息
     */
    /** {en}
     * @type api
     * @brief  Audio & video device information
     */
    public interface IDeviceCollection
    {
        #region Events


        #endregion

        /** {zh}
         * @type api
         * @brief 获取当前系统内音视频设备数量
         * @return 音视频设备数量
         */
        /** {en}
         * @type api
         * @brief Get the number of audio & video devices in the current system
         * @return Number of audio & video devices
         */
        int GetCount();

        /** {zh}
         * @type api
         * @brief 根据索引号，获取设备信息
         * @param index 设备索引号，从 0 开始，注意需小于 GetCount{@link #IDeviceCollection#GetCount} 返回值。
         * @param deviceName 设备名称
         * @param deviceID 设备 ID
         * @return  <br>
         *        + 0：方法调用成功  <br>
         *        + !0：方法调用失败  <br>
         */
        /** {en}
         * @type api
         * @brief According to the index number, get device information
         * @param index Device index number, starting from 0, note that it must be less than the return value of GetCount{@link #IDeviceCollection#GetCount}.
         * @param deviceName device name
         * @param deviceID device ID
         * @return   <br>
         *         + 0: Success. <br>
         *         +! 0: failure <br>
         */
        int GetDevice(int index, ref string deviceName, ref string deviceID);

        /** {zh}
         * @type api
         * @brief 释放当前 IDeviceCollection{@link #IDeviceCollection} 对象占用的资源。
         * @notes 不需要返回音视频设备相关信息列表时应该调用本方法释放相关资源。
         */
        /** {en}
         * @type api
         * @brief  Releases the resources occupied by the current IDeviceCollection{@link #IDeviceCollection} object.
         * @notes This method should be called to release related resources when you do not need to return a list of audio & video device related information.
         */
        void Release();

    }

}