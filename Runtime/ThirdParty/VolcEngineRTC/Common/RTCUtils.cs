using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

namespace bytertc
{
    public class RTCUtils
    {
        public static IntPtr ArrayToIntPtr<T>(T[] myStructs)
        {
           // int size = Marshal.SizeOf(array[0])*array.Length;
             // 计算结构体大小（字节）
            int sizeOfMyStruct = Marshal.SizeOf<T>();
        
            // 分配内存空间
            IntPtr ptr = Marshal.AllocHGlobal(sizeOfMyStruct * myStructs.Length);
        
            try
            {
                // 逐个复制结构体到指定位置
                for (int i = 0; i < myStructs.Length; i++)
                {
                    IntPtr currentPtr = new IntPtr((long)ptr + i * sizeOfMyStruct);
                
                    Marshal.StructureToPtr(myStructs[i], currentPtr, false);
                }
            
                Debug.Log("success transform");
            }
            catch (Exception e)
            {
                Debug.LogError($"transform error：{e}");
            }
     
            return ptr;
        }
        public static IntPtr StructToIntPtr<T>(T info)
        {
            int size = Marshal.SizeOf(info);
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(info, intPtr, false);
            }catch
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return intPtr;
        }
      
    }
}
