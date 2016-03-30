using SecretGardenServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessServer.GuessDao
{
    class UserDao
    {
        protected DBPlayer player;

        /// <summary>
        /// 获取某玩家信息
        /// </summary>
        /// <param name="playerName">玩家姓名</param>
        /// <returns>玩家</returns>
        public DBPlayer getPlayer(string playerName)
        {
            player = new DBPlayer();
            try
            {
                using (var context = new SecretGardenDatabaseEntities())
                {
                    var v = from t in context.player
                            where t.playerName == playerName
                            select t;
                    foreach (var item in v)
                    {
                        player.playerName = item.playerName;
                        player.playerPwd = item.playerPwd;
                        player.playerSign = item.playerSign;
                        player.playerAva = item.playerAva;
                    }
                }
            }
            catch (Exception)
            {
                player = null;
            }
            return player;
        }

        
    }
}
