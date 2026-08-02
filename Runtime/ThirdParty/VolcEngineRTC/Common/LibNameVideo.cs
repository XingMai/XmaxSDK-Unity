namespace bytertc
{
    public static class LibName
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        public const string libname = "ByteRTCCWrapper";
#elif UNITY_IOS
        public const string libname = "__Internal";
#elif UNITY_ANDROID
        public const string libname = "ByteRTCCWrapper";
#else
        public const string libname = "libByteRTCCWrapper";
#endif
    }
}