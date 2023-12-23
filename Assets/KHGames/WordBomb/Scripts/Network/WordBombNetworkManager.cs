using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using WordBombServer.Common;
using WordBombServer.Common.Packets.Request;
using WordBombServer.Common.Packets.Response;

[Serializable]
public struct ConnectionSettings
{
    public int PORT;
    public enum ConnectionType
    {
        LocalHost,
        ServerIP
    }

    public string IPAdress;
    public ConnectionType Type;
}

public enum Connection
{
    Connecting,
    Connected,
    Disconnected,
}

public class WordBombNetworkManager : MonoBehaviour, INetEventListener
{
    private NetManager _client;
    private EventBasedNetListener _listener;

    public static event Action<NetPeer> OnConnectedToServer;

    public static event Action OnDisconnectedFromServer;
    public Connection ConnectionStatus { get; set; }

    public ConnectionSettings ConnectionSettings;

    public NetPeer Server;

    public static WordBombNetworkManager Instance;

    public readonly NetPacketProcessor NetPacketProcessor = new();
    public static NetworkRoomEventListener EventListener;


    public int Id { get; set; } = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeLocalization();
        StartConnection();
    }


    private void OnDestroy()
    {
        if (Instance != this)
            return;
        _client.Stop();
    }

    private void StartConnection()
    {

        Id = -1;
        _client = new NetManager(this);
        NetPacketProcessor.RegisterNestedType(() =>
        {
            return new Player();
        });
        NetPacketProcessor.RegisterNestedType(() =>
        {
            return new LobbyInfo();
        });
        NetPacketProcessor.RegisterNestedType(() =>
        {
            return new LeaderboardData();
        });




        NetPacketProcessor.SubscribeReusable<PlayerConnectionResponse>(PlayerConnectionPacketReceived);
        EventListener = new NetworkRoomEventListener(this);
        _client.Start();
    }

    private static void InitializeLocalization()
    {
        try
        {
            LocalizationSettings.InitializationOperation.WaitForCompletion();
        }
        catch (Exception e)
        {
        }
        LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[UserData.UILanguage];
    }

    public static void SendWordSuggestion(string word, byte language)
    {
        WordBombNetworkManager.Instance.SendPacket(new SuggestWordRequest()
        {
            Language = language,
            Word = word
        });
    }

    public void Connect()
    {
        _client.Connect(
            ConnectionSettings.Type == ConnectionSettings.ConnectionType.LocalHost
                ? "localhost"
                : ConnectionSettings.IPAdress, ConnectionSettings.PORT, GameInfo.VERSION);
    }

    private void PlayerConnectionPacketReceived(PlayerConnectionResponse connectionPacket)
    {
        this.Id = connectionPacket.Id;
    }

    public void SendPacket<T>(T packet) where T : class, new()
    {
        Server.Send(this.NetPacketProcessor.Write(packet), DeliveryMethod.ReliableOrdered);
    }


    private void Update()
    {
        _client.PollEvents();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Server = peer;
        ConnectionStatus = Connection.Connected;
        OnConnectedToServer?.Invoke(peer);
    }


    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Id = -1;
        ConnectionStatus = Connection.Disconnected;
        if (disconnectInfo.Reason == DisconnectReason.ConnectionRejected)
        {
            var messagePopup = new MessagePopup(Language.Get("VERSION_ERROR"));
            CanvasUtilities.Instance.Toggle(true, Language.Get("VERSION_ERROR"));
            messagePopup.OnSubmit += () =>
            {
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.khgames.wordbomb");
            };
            PopupManager.Instance.Show(messagePopup);
            return;
        }

        OnDisconnectedFromServer?.Invoke();
        Debug.Log("Disconnected from server.");
        if (!SceneManager.GetActiveScene().name.Equals("Offline")
            && !SceneManager.GetActiveScene().name.Equals("Intro"))
        {
            SceneManager.LoadScene("Offline");
        }

    }
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        NetPacketProcessor.ReadAllPackets(reader, peer);
    }


    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Debug.LogError(socketError);
    }


    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        Debug.LogError(messageType);

    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        //ping => latency
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.Log("Connection Request");
    }


}