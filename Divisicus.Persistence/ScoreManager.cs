using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Divisicus.Persistence
{
    public class ScoreManager
    {

        public static string GetHighScore()
        {
            var db = new LevelScoreEntities();
            int highScore = db.Players.Max(p => p.HighScore);
            string champ = db.Players.FirstOrDefault(p => p.HighScore == highScore).Alias;
            return highScore.ToString() + " - " + champ;
        }
    }
}