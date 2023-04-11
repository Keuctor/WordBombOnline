using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordBombServer.Common.Packets.Request
{
    public class SuggestWordRequest
    {
        public byte Language { get; set; }
        public string Word { get; set; }
    }
}
