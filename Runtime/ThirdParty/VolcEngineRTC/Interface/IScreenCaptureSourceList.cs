using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace bytertc
{
    /** {zh}
     * @type api
     * @brief 屏幕源类型信息列表
     */
    /** {en}
     * @type api
     * @brief  Screen source type information list
     */
    public interface IScreenCaptureSourceList
    {
        /** {zh}
         * @brief 获取列表大小
         * @return 获取列表大小
         */
        /** {en}
         * @brief  Get list size
         * @return  Get list size
         */
        int GetCount();

        /** {zh}
         * @brief 根据索引号，获取屏幕共享列表中的元素。
         * @param index 列表索引号
         * @return 屏幕源类型信息，参看 ScreenCaptureSourceInfo{@link #ScreenCaptureSourceInfo}。
         * @notes 返回类型中有 char* 类型的字符串，该字符串在本对象 Release 后释放，请注意内存管理。
         */
        /** {en}
         * @brief According to the index, get the elements in the screen share list.
         * @param index Index number.
         * @return Screen source type information. See ScreenCaptureSourceInfo{@link #ScreenCaptureSourceInfo}.
         * @notes Return a string of type char* in the type, which is released after this object is released, please pay attention to memory management.
         */
        ScreenCaptureSourceInfo GetSourceInfo(int index);

        /** {zh}
         * @brief 析构当前对象，释放内存
         * @notes 严禁调用 delete 该结构体，该结构不需要的时候应该调用本函数释放资源
         */
        /** {en}
         * @brief Destruct the current object and free up memory
         * @notes Call delete the structure is strictly prohibited, this function should be called to free resources when the structure is not needed
         */
        void Release();
    }
}