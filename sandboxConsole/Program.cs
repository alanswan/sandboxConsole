﻿using System;
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
using System.Xml.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sandboxConsole
{
    class Program
    {
        private static omproEntities db = new omproEntities();
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    List<EF.Team> teams = db.Teams.ToList();
                    List<TeamsNotFound> newTeams = db.TeamsNotFounds.ToList();
                    List<TeamsNotFound> originalTeamsNotFound = db.TeamsNotFounds.ToList(); 
                    List<TeamsNotFound> teamsForDB = new List<TeamsNotFound>();

                    List<EF.Competition> comps = db.Competitions.ToList();
                    List<CompetitionsNotFound> newComps = db.CompetitionsNotFounds.ToList();
                    List<CompetitionsNotFound> originalCompsNotFound = db.CompetitionsNotFounds.ToList();
                    List<CompetitionsNotFound> compsForDB = new List<CompetitionsNotFound>();

                    WH wh = new WH(teams, newTeams, comps, newComps);
                    Smar smar = new Smar(teams, newTeams, comps, newComps);
                    Betdaq betdaq = new Betdaq(teams, newTeams, comps, newComps);
                    Betfred betfred = new Betfred(teams, newTeams, comps, newComps);
                    Coral coral = new Coral(teams, newTeams, comps, newComps);
                    Eight88 eight = new Eight88(teams, newTeams, comps, newComps);
                    Betfair betfair = new Betfair(teams, newTeams, comps, newComps);

                    betfair.Login();
                    betfair.ReadBetfairFootball("GBR");
                   // betfair.RefreshDB();


                    smar.ReadSmarUKFootball();

                    coral.ReadHorseRacing();
                    eight.Read888Football();


                    betdaq.ReadBetdaqHorseRacing();
                    betdaq.ReadBetdaqFootball();

                    coral.ReadCoralFootball();
                    betfred.ReadBetfredFootball();
                    betfred.ReadBetfredHorseRacing();

                    wh.ReadHorseRacing();
                    wh.ReadWHUKFootball();
                    wh.ReadWHEuroFootball();
                    wh.ReadWHInternationalFootball();

                    if (wh.Matches.Count() > 0 || wh.Races.Count() > 0)
                    {
                        

                        var bulkMatches = new List<EF.Match>();
                        foreach (Models.Match match in wh.Matches)
                        {
                            if (match.Team1.Name != null && match.Team1.Id != 999999)
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
                        }

                        foreach (Models.Match match in betfred.Matches)
                        {
                            if (match.Team1.Name != null && match.Team1.Id != 999999)
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
                        }

                        foreach (Models.Match match in coral.Matches)
                        {
                            if (match.Team1.Name != null && match.Team1.Id != 999999)
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
                        }
                        DeleteData();
                        db.BulkInsert(bulkMatches);

                        var bulkRaces = new List<EF.Race>();
                        foreach (Models.Race race in wh.Races)
                        {
                            bulkRaces.Add(new EF.Race()
                            {
                                RaceId = race.Id,
                                Name = race.Name,
                                BookmakerId = race.BookmakerId,
                                CompetitionId = race.Meeting.Id,
                                CompetitionName = race.Meeting.Name,
                                Horse = race.Horse,
                                Odds = race.Odds,
                                Date = race.Date,
                                LastUpdated = race.LastUpdated,
                                Time = race.Time,
                                MoneyInMarket = race.MoneyInMarket,
                                URL = race.Url,
                                MobileURL = race.MobileUrl
                            });
                        }
                        foreach (Models.Race race in betfred.Races)
                        {
                            bulkRaces.Add(new EF.Race()
                            {
                                RaceId = race.Id,
                                Name = race.Name,
                                BookmakerId = race.BookmakerId,
                                CompetitionId = race.Meeting.Id,
                                CompetitionName = race.Meeting.Name,
                                Horse = race.Horse,
                                Odds = race.Odds,
                                Date = race.Date,
                                LastUpdated = race.LastUpdated,
                                Time = race.Time,
                                MoneyInMarket = race.MoneyInMarket,
                                URL = race.Url,
                                MobileURL = race.MobileUrl
                            });
                        }
                        foreach (Models.Race race in coral.Races)
                        {
                            bulkRaces.Add(new EF.Race()
                            {
                                RaceId = race.Id,
                                Name = race.Name,
                                BookmakerId = race.BookmakerId,
                                CompetitionId = race.Meeting.Id,
                                CompetitionName = race.Meeting.Name,
                                Horse = race.Horse,
                                Odds = race.Odds,
                                Date = race.Date,
                                LastUpdated = race.LastUpdated,
                                Time = race.Time,
                                MoneyInMarket = race.MoneyInMarket,
                                URL = race.Url,
                                MobileURL = race.MobileUrl
                            });
                        }

                        db.BulkInsert(bulkRaces);

                        var bulkExchangeRaces = new List<EF.ExchangeRace>();
                        foreach (Models.Race race in betdaq.Races)
                        {
                            bulkExchangeRaces.Add(new EF.ExchangeRace()
                            {
                                RaceId = race.Id,
                                Name = race.Name,
                                BookmakerId = race.BookmakerId,
                                CompetitionId = race.Meeting.Id,
                                CompetitionName = race.Meeting.Name,
                                Horse = race.Horse,
                                Odds = race.Odds,
                                Date = race.Date,
                                LastUpdated = race.LastUpdated,
                                Time = race.Time,
                                MoneyInMarket = race.MoneyInMarket,
                                URL = race.Url,
                                MobileURL = race.MobileUrl
                            });
                        }
                        foreach (Models.Race race in smar.Races)
                        {
                            bulkExchangeRaces.Add(new EF.ExchangeRace()
                            {
                                RaceId = race.Id,
                                Name = race.Name,
                                BookmakerId = race.BookmakerId,
                                CompetitionId = race.Meeting.Id,
                                CompetitionName = race.Meeting.Name,
                                Horse = race.Horse,
                                Odds = race.Odds,
                                Date = race.Date,
                                LastUpdated = race.LastUpdated,
                                Time = race.Time,
                                MoneyInMarket = race.MoneyInMarket,
                                URL = race.Url,
                                MobileURL = race.MobileUrl
                            });
                        }
                        DeleteDataRaces();
                        db.BulkInsert(bulkExchangeRaces);

                        var bulkExchange = new List<ExchangeMatch>();
                        foreach (Models.Match match in betdaq.Matches)
                        {
                            if (match.Team1.Name != null && match.Team1.Id != 999999)
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
                        foreach (Models.Match match in smar.Matches)
                        {
                            if (match.Team1.Name != null && match.Team1.Id != 999999)
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
                        foreach (Models.Match match in betfair.Matches)
                        {
                            if (match.Team1.Name != null && match.Team1.Id != 999999)
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
                            if (!originalTeamsNotFound.Any(x => x.TeamName == team.TeamName))
                               teamsForDB.Add(team);
                        };
                        foreach (EF.CompetitionsNotFound comp in newComps)
                        {
                            if (!originalCompsNotFound.Any(x => x.CompetitionName == comp.CompetitionName))
                                compsForDB.Add(comp);
                        };

                        db.BulkInsert(teamsForDB);
                        db.BulkInsert(compsForDB);
                        
                    }
                    else
                    {
                        ErrorHelper.CreateError(BookmakersConstants.BetfairName, "No matches/races found");
                    }
                    stopwatch.Stop();
                    var elapsedTime = Convert.ToInt32(stopwatch.ElapsedMilliseconds/1000);
                    LogHelper.LogTimes(elapsedTime, BookmakersConstants.BetfairName);
                }
                catch (Exception e)
                {
                    ErrorHelper.CreateError(BookmakersConstants.BetfairName, e.Message);
                }
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
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.WilliamHillId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Matches WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.CoralId);
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
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ExchangeMatches WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.SmarketsId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ExchangeMatches WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.BetfairId);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Races WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.WilliamHillId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Races WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.CoralId);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Races WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.BetfredId);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ExchangeRaces WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.BetdaqId);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = sc.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ExchangeRaces WHERE BookmakerId = @id";
                    cmd.Parameters.AddWithValue("@id", BookmakersConstants.SmarketsId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
