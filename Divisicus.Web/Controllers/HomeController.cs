using Divisicus.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Divisicus.Persistence;

namespace Divisicus.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Guid userId;
            if(!Guid.TryParse(User.Identity.GetUserId(), out userId))
            {
                throw new Exception("User Id not Valid.");
            }
            var player = getUserInfo(userId);
            if(player == null){
                player = createNewPlayer(userId);
            }
            if (player.Alias == null) return RedirectToAction("InputAlias");
            return View(player);
        }

        public ActionResult InputAlias()
        {
            Guid userId;
            if (!Guid.TryParse(User.Identity.GetUserId(), out userId))
            {
                throw new Exception("User Id not Valid.");
            }
            var player = getUserInfo(userId);
            return View(player);
        }

        [HttpPost]
        public string InputAlias(string user, string alias)
        {
            Guid guid;
            if (!Guid.TryParse(user, out guid)) throw new Exception("User Id not Valid.");
            var db = new LevelScoreEntities();
            db.Players.FirstOrDefault(p => p.UserId == guid).Alias = alias;
            db.SaveChanges();
            string output = "Thanks " + alias + "!\nEnjoy Divisicus!";
            return output;
        }

        [HttpPost]
        public string DebriefGame(string user, string score)
        {
            string output = "";
            Guid guid;
            int intScore;
            if (!int.TryParse(score, out intScore)) throw new Exception("Score not valid.");
            if (!Guid.TryParse(user, out guid)) throw new Exception("User Id not Valid.");
            output += checkForHighScore(guid, intScore);
            output += checkForLevelUp(guid, intScore);
            return output;
        }

        private string checkForLevelUp(Guid guid, int intScore)
        {
            int[] levelThresholds = new int[] { 3000, 3500, 4100, 5900, 7150, 10000, 11250, 13200, 14350, 16600, 17450, 18950, 19400, 21700, 22750, 24550, 25350, 100000 };
            var player = getUserInfo(guid);
            string output = "";
            var currentThreshold = levelThresholds[player.Level];
            if (intScore >= currentThreshold)
            {
                levelUp(guid);
                player = getUserInfo(guid);
                output = "\nCongratulations!\nYou are now Level " + player.Level.ToString();
            }
            else
            {
                output = "\nGet a score of " + currentThreshold.ToString() + " to advance to Level " + (player.Level + 1).ToString();
            }
            return output;
        }

        private void levelUp(Guid guid)
        {
            var db = new LevelScoreEntities();
            db.Players.FirstOrDefault(p => p.UserId == guid).Level++;
            db.SaveChanges();
        }

        private string checkForHighScore(Guid userId, int score)
        {
            var player = getUserInfo(userId);
            if (score > player.HighScore)
            {
                setNewHighScore(player, score);
                return "New High Score Set!";
            }
            else return "";
        }

        private void setNewHighScore(Player player, int score)
        {
            var db = new LevelScoreEntities();
            db.Players.FirstOrDefault(p => p.UserId == player.UserId).HighScore = score;
            db.SaveChanges();
        }

        private Player getUserInfo(Guid userId)
        {
            var db = new LevelScoreEntities();
            var player = db.Players.FirstOrDefault(p => p.UserId == userId);
            return player;
        }

        private Player createNewPlayer(Guid userId)
        {
            var db = new LevelScoreEntities();
            Player player = new Player();
            player.UserId = userId;
            player.Level = 0;
            player.HighScore = 0;
            db.Players.Add(player);
            db.SaveChanges();
            return player;
        }


    }
}
