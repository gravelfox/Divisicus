using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Divisicus.Persistence
{
    public class UserManager
    {
        public static string getPlayerAlias(string userId)
        {
            Guid guid;
            if (!Guid.TryParse(userId, out guid)) throw new Exception("User Id not Valid.");
            var db = new LevelScoreEntities();
            var player = db.Players.FirstOrDefault(p => p.UserId == guid);
            if (player.Alias != null) return player.Alias.ToString();
            else return "";
            
            
        }
    }
}