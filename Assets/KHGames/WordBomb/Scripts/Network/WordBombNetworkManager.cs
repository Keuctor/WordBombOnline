using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
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

public struct userAttributes
{
}

public struct appAttributes
{
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


    public static bool CheckForInternetConnection()
    {
        try
        {
            using (var client = new WebClient())
            using (client.OpenRead("https://keugames.com/privacy-policy.html"))
                return true;
        }
        catch
        {
            return false;
        }
    }

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (CheckForInternetConnection())
        {
            InitializeLocalization();
            await InitializeRemoteConfigAsync();
        }
    }

    public async Task InitializeRemoteConfigAsync()
    {
        CanvasUtilities.Instance.Toggle(true, "Services...");
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        CanvasUtilities.Instance.Toggle(true, "Authenticated...");
        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigLoaded;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    public static string Text;

    public static bool _clientStarted;
    private static bool _isServerInMaintence;

    private void OnRemoteConfigLoaded(ConfigResponse obj)
    {
        CanvasUtilities.Instance.Toggle(true, "Remote Config Loaded...");
        switch (obj.requestOrigin)
        {
            case ConfigOrigin.Default:
            case ConfigOrigin.Cached:
            case ConfigOrigin.Remote:
                var showInfo = RemoteConfigService.Instance.appConfig.GetBool("show_info");
                if (showInfo)
                {
                    string message = string.Empty;
                    if (UserData.UILanguage == 2)
                    {
                        message = RemoteConfigService.Instance.appConfig.GetString("info_message_tr");
                    }
                    else
                    {
                        message = RemoteConfigService.Instance.appConfig.GetString("info_message_en");
                    }

                    PopupManager.Instance.Show(message);
                }

                _isServerInMaintence = RemoteConfigService.Instance.appConfig.GetBool("is_server_closed");
                if (_isServerInMaintence && !showInfo)
                {
                    PopupManager.Instance.Show(Language.Get("SERVER_MAINTENANCE"));
                }

                if (_isServerInMaintence)
                {
                    CanvasUtilities.Instance.Toggle(true, Language.Get("SERVER_MAINTENANCE"));
                }
                else
                {
                    StartConnection();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void OnDestroy()
    {
        if (Instance != this)
            return;
        _client?.Stop();
    }

    private void StartConnection()
    {
        Id = -1;
        _client = new NetManager(this);
        NetPacketProcessor.RegisterNestedType(() => { return new Player(); });
        NetPacketProcessor.RegisterNestedType(() => { return new LobbyInfo(); });
        NetPacketProcessor.RegisterNestedType(() => { return new LeaderboardData(); });


        NetPacketProcessor.SubscribeReusable<PlayerConnectionResponse>(PlayerConnectionPacketReceived);
        EventListener = new NetworkRoomEventListener(this);
        _client.Start();
        _clientStarted = true;
        Connect();
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
        if (!_clientStarted)
            return;

        if (_isServerInMaintence)
            return;

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


    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
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

    public void DeleteAccount()
    {
        WordBombNetworkManager.Instance.SendPacket(new RemoveAccountRequest()
        {
            Password = MenuController.EncyrptPassword(MenuController.DevicePassword)
        });
    }
}