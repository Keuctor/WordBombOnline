public class LobbySettings
{
    public static int GetLobbySelectedLanguage()
    {
        return NetworkRoomEventListener.CurrentRoom.Language;
    }

    public static int GetSelectedLobbyGameMode()
    {
        return NetworkRoomEventListener.CurrentRoom.Mode;
    }

    public static void ChangeAvatar(int id, int index)
    {
        throw new System.NotImplementedException();
    }
}