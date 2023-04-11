
using ilasm.WordBomb.Chat;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public ChatController ChatController;

    private void OnEnable()
    {
        WordBombNetworkManager.EventListener.OnChatMessageReceive += MessageReceived;
    }
    private void OnDisable()
    {
        WordBombNetworkManager.EventListener.OnChatMessageReceive -= MessageReceived;
    }
    public void SendChatMessage(int playerId, string inputText)
    {
        WordBombNetworkManager.EventListener.SendChatMessage(playerId, inputText);
    }
    public void MessageReceived(int playerId, string inputText)
    {
        ChatController.OnReceivedMessage(playerId, inputText);
    }
}
