

using System.Collections.Generic;
using WordBombServer.Common;

public class Lobby
{
    public byte Language { get; set; }
    public byte Mode { get; set; }
    public byte Speed { get; set; }
    public string Title { get; set; }
    public int HostId { get; set; }
    public string Code { get; set; }
    public bool IsPrivate { get; set; }
    public int MaxPlayers { get => 8; }
    public List<Player> Players { get; set; }
    public List<Player> InGamePlayers { get; set; }
    public Player GetPlayer(int id) {
        foreach (var p in Players)
            if (p.Id == id)
                return p;
        return null;
    }
}

