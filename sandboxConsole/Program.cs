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
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
using System.Data.SqlClient;

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
            //Smar smar = new Smar();
            Betdaq betdaq = new Betdaq(teams, newTeams);
            Betfred betfred = new Betfred(teams, newTeams);

            //smar.ReadSmarUKFootball();
            betdaq.ReadBetdaqFootball();
            betfred.ReadBetfredFootball();
            // smar.ReadSmarUKFootball();

            wh.ReadWHUKFootball();
            wh.ReadWHEuroFootball();
            wh.ReadWHInternationalFootball();
            
            if (wh.Matches.Count() > 0)
            {
                DeleteData();

                var bulkMatches = new List<EF.Match>();
                foreach (Models.Match match in wh.Matches)
                {
                    bulkMatches.Add(new EF.Match()
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
                        MoneyInMarket = match.MoneyInMarket,
                        URL = match.Url,
                        MobileURL = match.MobileUrl
                    });
                }
                
                foreach (Models.Match match in betfred.Matches)
                {
                    bulkMatches.Add(new EF.Match()
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
                        MoneyInMarket = match.MoneyInMarket,
                        URL = match.Url,
                        MobileURL = match.MobileUrl
                    });
                }

                db.BulkInsert(bulkMatches);

                var bulkExchange = new List<ExchangeMatch>();
                foreach (Models.Match match in betdaq.Matches)
                {
                    if (match.Team1.Name != null)
                    {
                        bulkExchange.Add(new ExchangeMatch()
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
                            MoneyInMarket = match.MoneyInMarket,
                            URL = match.Url,
                            MobileURL = match.MobileUrl
                        });
                    }
                }
                db.BulkInsert(bulkExchange);

                foreach (EF.TeamsNotFound team in newTeams)
                {
                    if (!db.TeamsNotFounds.Any(x => x.TeamName == team.TeamName))
                        db.TeamsNotFounds.Add(team);
                };
                db.SaveChanges();

            }
        }

        public static void DeleteData()
        {
            SqlConnection conn = new SqlConnection();
            var ConnectionString = "data source=mssql2.gear.host;initial catalog=ompro;persist security info=True;user id=ompro;password=Co31?i!8iF74;MultipleActiveResultSets=True;";

            using (SqlConnection sc = new SqlConnection(ConnectionString))
            {
                sc.Open();
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Matches WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.WilliamHillId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Matches WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.BetfredId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ExchangeMatches WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.BetdaqId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
