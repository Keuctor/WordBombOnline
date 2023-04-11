namespace WordBombServer.Common.Packets.Request
{
    public class ChangeRoomSettingRequest
    {
        public byte Mode { get; set; }
        public byte Language { get; set; }
        public byte Speed { get; set; }
        public bool IsPrivate { get; set; }
    }
}
