using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sandboxConsole.Helpers.XML;
using sandboxConsole.EF;
using sandboxConsole.Misc;
using sandboxConsole.Helpers.XML.Exchange;
using sandboxConsole.Models;

namespace sandboxConsole
{
    class Program
    {
        private static omproEntities db = new omproEntities();
        static void Main(string[] args)
        {
            List<EF.Team> teams = db.Teams.ToList();
            List<TeamsNotFound> newTeams = db.TeamsNotFounds.ToList();

            WH wh = new WH(teams, newTeams);
            Smar smar = new Smar();
            Betdaq betdaq = new Betdaq(teams, newTeams);

            smar.ReadSmarUKFootball();

            betdaq.ReadBetdaqFootball();
           // smar.ReadSmarUKFootball();

            wh.ReadWHUKFootball();
            wh.ReadWHEuroFootball();
            wh.ReadWHInternationalFootball();

            if (wh.Matches.Count() > 0)
            {
                db.Matches.RemoveRange(db.Matches.Where(x => x.BookmakerId == Constants.WilliamHillId));
                db.ExchangeMatches.RemoveRange(db.ExchangeMatches.Where(x => x.BookmakerId == Constants.BetdaqId));
            }

            foreach (Models.Match match in wh.Matches)
            {
                db.Matches.Add(new EF.Match()
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
                    Bet = match.Bet,
                    Odds = match.Odds,
                    Date = match.Date,
                    LastUpdated = match.LastUpdated,
                    Time = match.Time,
                    MoneyInMarket = match.MoneyInMarket
                });
            }

            foreach (Models.Match match in betdaq.Matches)
            {
                if (match.Team1.Name != null)
                {
                    db.ExchangeMatches.Add(new ExchangeMatch()
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
                        Bet = match.Bet,
                        Odds = match.Odds,
                        Date = match.Date,
                        LastUpdated = match.LastUpdated,
                        Time = match.Time,
                        MoneyInMarket = match.MoneyInMarket
                    });
                }
            }

            foreach(EF.TeamsNotFound team in newTeams)
            {
                if(!db.TeamsNotFounds.Any(x=>x.TeamName == team.TeamName))
                db.TeamsNotFounds.Add(team);
            }
            
            db.SaveChanges();

            var test = "end";
            
        }
    }
}
