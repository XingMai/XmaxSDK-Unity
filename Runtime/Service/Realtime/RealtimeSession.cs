namespace Xmax.SDK
{
    internal sealed class XmaxSession
    {
        internal string SessionUid;
        internal string UserUid;
        internal string Status;
        internal RtcJoinInfo JoinInfo;
    }

    internal sealed class RtcJoinInfo
    {
        internal string AppId;
        internal string RoomId;
        internal string UserId;
        internal string Token;
        internal string BotName;
    }

}
