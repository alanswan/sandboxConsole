using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sandboxConsole.Helpers.XML;
using sandboxConsole.EF;
using sandboxConsole.Misc;
using sandboxConsole.Helpers.XML.Exchange;

namespace sandboxConsole
{
    class Program
    {
        private static omproEntities db = new omproEntities();
        static void Main(string[] args)
        {
            WH wh = new WH();
            Smar smar = new Smar();
            Betdaq betdaq = new Betdaq();

            betdaq.ReadBetdaqFootball();
            smar.ReadSmarUKFootball();

            wh.ReadWHUKFootball();
            wh.ReadWHEuroFootball();
            wh.ReadWHInternationalFootball();

            if (wh.Matches.Count() > 0)
            {
                db.Matches.RemoveRange(db.Matches.Where(x => x.BookmakerId == Constants.WilliamHillId));
            }

            foreach (Models.Match match in wh.Matches)
            {
                db.Matches.Add(new Match()
                {
                    MatchId = match.Id,
                    Name = match.Name,
                    BookmakerId = match.BookmakerId,
                    CompetitionId = match.Competition.Id,
                    CompetitionName = match.Competition.Name,
                    Team1Id = match.Team1.Id,
                    Team1Name = match.Team1.Name,
                    Team2Id = match.Team2.Id,
                    Team2Name = match.Team2.Name,
                    Team1Odds = match.Odds.Team1,
                    Team2Odds = match.Odds.Team2,
                    DrawOdds = match.Odds.Draw,
                    Date = match.Date,
                    LastUpdated = match.LastUpdated,
                    Time = match.Time
                });
            }

            db.SaveChanges();


            //smar.ReadSmarUKFootball();
        }
    }
}
