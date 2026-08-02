using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    #region EventHandler


    #endregion

    /** {zh}
     * @type api
     * @region 视频设备管理
     * @brief 主要用于枚举、设置视频采集设备
     */
    /** {en}
     * @type api
     * @region Video Facility Management
     * @brief Mainly used to enumerate and set up video capture devices
     */
    public interface IVideoDeviceManager
    {
        #region Events


        #endregion

        /** {zh}
         * @type api
         * @brief 获取当前系统内视频采集设备列表。
         * @return 包含系统中所有视频采集设备的列表，参看 IDeviceCollection{@link #IDeviceCollection}。  <br>
         */
        /** {en}
         * @type api
         * @brief  Get a list of video capture devices in the current system.
         * @return  Contains a list of all video capture devices in the system. See IDeviceCollection{@link #IDeviceCollection}. <br>
         */
        IDeviceCollection EnumerateVideoCaptureDevices();

        /** {zh}
         * @type api
         * @brief 设置当前视频采集设备
         * @param deviceID 视频设备 ID，可以通过 EnumerateVideoCaptureDevices{@link #IVideoDeviceManager#EnumerateVideoCaptureDevices} 获取
         * @return  <br>
         *        + 0：方法调用成功  <br>
         *        + !0：方法调用失败  <br>
         */
        /** {en}
         * @type api
         * @brief  Set the current video capture device
         * @param deviceID Video device ID, which can be obtained through EnumerateVideoCaptureDevices{@link #IVideoDeviceManager#EnumerateVideoCaptureDevices}
         * @return   <br>
         *         + 0: Success. <br>
         *         +! 0: failure <br>
         */
        int SetVideoCaptureDevice(string deviceID);

        /** {zh}
         * @type api
         * @brief 获取当前 SDK 正在使用的视频采集设备信息
         * @param deviceID 视频设备 ID
         * @return  <br>
         *        + 0：成功  <br>
         *        + !0：失败  <br>
         */
        /** {en}
         * @type api
         * @brief Get the video capture device information currently used by the SDK
         * @param deviceID video device ID
         * @return   <br>
         *         + 0: Success. <br>
         *         +! 0: failure <br>
         */
        int GetVideoCaptureDevice(ref string deviceID);

    }

}