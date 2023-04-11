using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordBombServer.Common.Packets.Response
{
    public class UpdateUserData
    {
        public int Id { get; set; }
        public byte Coin { get; set; }
        public byte Emerald { get; set; }
        public short XP { get; set; }
    }
}
