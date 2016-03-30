using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessServer.GuessDao
{
    class DBPlayer
    {
        public string playerName { set; get; }
        public string playerPwd { set; get; }
        public string playerAva { get; set; }
        public string playerSign { get; set; }
    }
}
