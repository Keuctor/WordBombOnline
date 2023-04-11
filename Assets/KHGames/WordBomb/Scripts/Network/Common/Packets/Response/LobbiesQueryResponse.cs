using System;


namespace WordBombServer.Common.Packets.Response
{
    public class LobbiesQueryResponse
    {
        public LobbyInfo[] Lobbies { get; set; }
    }
}
