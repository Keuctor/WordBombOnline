using LiteNetLib.Utils;
using LiteNetLib;

namespace WordBombServer.Common
{
    public class Player : INetSerializable
    {
        public const int MAX_PLAYER_NAME_LENGTH = 20;
        public int Id { get; set; }
        public string RoomCode { get; set; }
        public string UserName { get; set; }
        public int CrownCount { get; set; }
        public short AvatarId { get; set; }
        public NetPeer Peer { get; set; }
        public int Level
        {
            get => ((int)(Experience < 100 ? 1 : ((Experience / 100) + 1)));
        }
        public bool GameLoaded { get; set; }
        public bool IsDead { get; set; }
        public int Score { get; set; }
        private int _combo;
        public int Combo
        {
            get
            {
                return _combo;
            }
            set
            {
                if (value > MaxCombo)
                {
                    MaxCombo = value;
                }
                _combo = value;
            }
        }
        public int MaxCombo { get; set; }
        public string LongestWord { get; set; }
        public int EliminationOrder { get; set; }
        public float Experience { get; set; }

        public Player()
        {
            RoomCode = null;
            UserName = null;
            AvatarId = 0;
            GameLoaded = false;
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            UserName = reader.GetString();
            AvatarId = reader.GetShort();
            GameLoaded = reader.GetBool();
            CrownCount = reader.GetInt();
            Experience = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(UserName);
            writer.Put(AvatarId);
            writer.Put(GameLoaded);
            writer.Put(CrownCount);
            writer.Put(Experience);
        }
    }

}
