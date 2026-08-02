using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    /** {zh} 
     * @type api
     * @brief 空间音频接口
     */
    /** {en} 
     * @type api
     * @brief Position audio interface
     */
    public interface ISpatialAudio
    {
        /** {zh}
         * @type api
         * @brief 设置是否开启空间语音
         * @param enable
         *        + true：开启<br>
         *        + false: 关闭（默认设置）  <br>
         * @notes 开启空间语音后，非同一小队的队友之间收听的声音会根据设置的坐标和方位不同而产生空间音效，如果不需要设置为 false 即可。
         */
        /** {en} 
         * @type api
         * @brief Enable or disable spatial audio effects.  <br>
         * @param enable Spatial audio effect switch, off by default. <br>
         *       + True: enable spatial voice; <br>
         *       + False: disable spatial voice. <br>
         */
        void EnableSpatialAudio(bool enable);

        /** {zh}
         * @type api
         * @brief 更新本地用户发声时，在房间内空间直角坐标系中的位置坐标。
         * @param pos 三维坐标的值，默认为 [0, 0, 0]。参看 Position{@link #Position}。
         * @return  <br>
         *        + 0: 成功；
         *        + !0: 失败。
         * @notes 调用该接口更新坐标前，你需调用 EnableSpatialAudio{@link #ISpatialAudio#EnableSpatialAudio} 开启空间音频功能。空间音频相关 API 和调用时序详见[空间音频](https://www.volcengine.com/docs/6348/93903)。
         */
        /** {en}
         * @type api
         * @brief Updates the coordinate of the local user's position as a sound source in the rectangular coordinate system in the current room.
         * @param pos 3D coordinate values, the default value is [0, 0, 0], see Position{@link #Position}.
         * @return  <br>
         *        + 0: Success.
         *        + !0: Failure.
         * @notes Before calling this API, you should call EnableSpatialAudio{@link #ISpatialAudio#EnableSpatialAudio} first to enable spatial audio function.
         */
        int UpdatePosition(Position pos);

        /** {zh} 
         * @type api
         * @brief 更新自己的朝向。本地朝向的设置只影响本地听到的音频效果，不影响本地发出的音频效果。
         * @param orientation 朝向信息，必须是单位向量。参看 HumanOrientation{@link #HumanOrientation}。当朝向的正方向使用默认值时，朝向的坐标系和位置信息使用的坐标系一致。此时，正前方朝向是：`[1, 0, 0]`，正右方朝向是：`[0, 1, 0]`，正上方朝向是：`[0, 0, 1]`。
         * @return 0: 更新成功
         * @notes 本地用户调用 EnableSpatialAudio{@link #ISpatialAudio#EnableSpatialAudio} 使用空间音频功能时，需要调用 UpdatePosition{@link #ISpatialAudio#UpdatePosition} 接口更新位置坐标后，才能调用此接口。  <br>
         */
        /** {en} 
         * @type api
         * @brief Update own orientation. The local orientation setting only affects the audio effects heard locally, not the audio effects emitted locally.  <br>
         * @param orientation Orientation information, which must be a unit vector. See HumanOrientation{@link #HumanOrientation}. When the default value is used for the positive direction of the orientation, the coordinate system of the orientation is the same as the coordinate system used by the position information. At this time, the front facing is: `[1, 0, 0]`, the right facing is: `[0, 1, 0]`, and the top facing is: `[0, 0, 1]`. <br>
         * @return 0: Represents a successful update
         * @notes The local user calls EnableSpatialAudio{@link #ISpatialAudio#EnableSpatialAudio} When using the spatial audio function, you need to call the UpdatePosition{@link #ISpatialAudio#UpdatePosition} interface to update the position coordinates before Call this interface. <br>
         */
        int UpdateSelfOrientation(HumanOrientation orientation);

        /** {zh}
       * @type api
       * @region 音频管理
       * @author luomingkang.264
       * @brief 关闭本地用户朝向对本地用户发声效果的影响。<br>
       *        调用此接口后，房间内的其他用户收听本地发声时，声源都在收听者正面。
       * @notes <br>
       *        + 调用本接口关闭朝向功能后，在当前的空间音频实例的生命周期内无法再次开启。<br>
       *        + 调用此接口不影响本地用户收听朝向的音频效果。要改变本地用户收听朝向，参看 UpdateSelfOrientation{@link #ISpatialAudio#UpdateSelfOrientation}。
       */
        /** {en}
         * @type api
         * @region Audio management
         * @author luomingkang.264
         * @brief Turn off the effect of the orientation of the local user as the sound source. <br>
         *        After the effect is off, all the other users in the room listen to the local user as if the local user is in right front of each of them.
         * @notes <br>
         *        + After the orientation effect as the sound source is disabled, you cannot enable it during the lifetime of the `SpatialAudio` instance. <br>
         *        + Calling this API does not affect the orientation effect of the local user as a listener. See UpdateSelfOrientation{@link #ISpatialAudio#UpdateSelfOrientation}.
         */
        void DisableRemoteOrientation();

        /** {zh}
         * @type api
         * @brief 释放当前 ISpatialAudio{@link #ISpatialAudio} 对象占用的资源。
         */
        /** {en}
         * @type api
         * @brief  Releases the resources occupied by the current ISpatialAudio{@link #ISpatialAudio} object.
         */
        void Release();
    }

    /** {zh} 
     * @type keytype
     * @brief 坐标
     */
    /** {en} 
     * @type keytype
     * @brief  Coordinate
     */
    public struct Orientation {
        /** {zh} 
         * @brief x 方向向量
         */
        /** {en} 
         * @brief X direction vector
         */
        public float x;
        /** {zh} 
         * @brief y 方向向量
         */
        /** {en} 
         * @brief Y direction vector
         */
        public float y;
        /** {zh} 
         * @brief z 方向向量
         */
        /** {en} 
         * @brief Z direction vector
         */
        public float z;
        /**
         * @hidden
         */
		public Orientation(float x, float y, float z){
			this.x = x;
			this.y = y;
			this.z = z;
		}
    };

    /** {zh}
     * @type keytype
     * @brief 三维朝向信息，三个向量需要两两垂直。参看 Orientation{@link #Orientation}。
     */
    /** {en}
     * @type keytype
     * @brief Three-dimensional orientation information, each pair of vectors need to be perpendicular. See Orientation{@link #Orientation}.
     */
    public struct HumanOrientation {
        /** {zh} 
         * @brief 正前方朝向，默认值为 {1,0,0}，即正前方朝向 x 轴正方向
         */
        /** {en} 
         * @brief Forward orientation, the default value is {1,0,0}, i.e., the forward orientation is in the positive direction of x-axis.
         */
        public Orientation forward ;
        /** {zh} 
         * @brief 正右方朝向，默认值为 {0,1,0}，即右手朝向 y 轴正方向
         */
        /** {en} 
         * @brief Rightward orientation, the default value is {0,1,0}, i.e., the rightward orientation is in the positive direction of y-axis.
         */
        public Orientation right ;
        /** {zh} 
         * @brief 正上方朝向，默认值为 {0,0,1}，即头顶朝向 z 轴正方向
         */
        /** {en} 
         * @brief Upward orientation, the default value is {0,0,1}, i.e., the upward orientation is in the positive direction of z-axis.
         */
        public Orientation up ;
        /**
         * @hidden
         */
		public HumanOrientation(Orientation forward, Orientation right, Orientation up){
			this.forward = forward;
			this.right = right;
			this.up = up;
		}
    };


}  // namespace bytertc
