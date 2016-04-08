﻿using sandboxConsole.EF;
using sandboxConsole.Helpers.Maintenance;
using sandboxConsole.Misc;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sandboxConsole.Helpers.XML
{
    public class Betfred : Company
    {
        public Dictionary<string, string> BetfredFeeds;
        public Betfred(List<EF.Team> teams, List<TeamsNotFound> newTeams) : base(teams, newTeams)
        {
            this.BetfredFeeds = new Dictionary<string, string>();
            BetfredFeeds.Add(CompetitionConstants.PremierLeague, "Football-Premiership");
            BetfredFeeds.Add(CompetitionConstants.Championship, "Football-Championship");
            BetfredFeeds.Add(CompetitionConstants.LeagueOne, "Football-English-League-1");
            BetfredFeeds.Add(CompetitionConstants.LeagueTwo, "Football-League-2");
            //BetfredFeeds.Add(CompetitionConstants.Conference, "Football-Conference");
            BetfredFeeds.Add(CompetitionConstants.FACup, "Football-FA-Cup");
            BetfredFeeds.Add(CompetitionConstants.LeagueCup, "Football-Carling-Cup");
            BetfredFeeds.Add(CompetitionConstants.EnglandJPT, "Football-Johnstones_Paint_Trophy");
            BetfredFeeds.Add(CompetitionConstants.ScottishPremier, "Football-SPL");
            BetfredFeeds.Add(CompetitionConstants.ScottishLeagueOne, "Football-Scottish-League-1");
            BetfredFeeds.Add(CompetitionConstants.ScottishLeagueCup, "Football-Scottish-CIS-Cup");
            BetfredFeeds.Add(CompetitionConstants.ScottishCup, "Football-Scottish-Cup");
            BetfredFeeds.Add(CompetitionConstants.SerieA, "Football-Italian-Serie-A");
            BetfredFeeds.Add(CompetitionConstants.FrenchLeagueOne, "Football-French-Ligue-1");
            BetfredFeeds.Add(CompetitionConstants.ItalianCup, "Football-Italian_Cup");
            BetfredFeeds.Add(CompetitionConstants.LaLiga, "Football-Spanish-Primera");
            BetfredFeeds.Add(CompetitionConstants.SpanishCup, "Football-Spanish_Cup");
            BetfredFeeds.Add(CompetitionConstants.ChampionsLeague, "Football-Champions-League");
            BetfredFeeds.Add(CompetitionConstants.EuropaLeague, "Football-UEFA-Cup");
            BetfredFeeds.Add(CompetitionConstants.WorldCup, "Football-World-Cup");
            BetfredFeeds.Add(CompetitionConstants.InternationalFriendlies, "football-internationalfriendlies");
            BetfredFeeds.Add(CompetitionConstants.AsianCup, "Football-Asian_Cup");
            BetfredFeeds.Add(CompetitionConstants.CopaAmerica, "Football-Copa_America");


        }

        public void ReadBetfredFootball()
        {
            ReadChampionshipFootball();
        }

        public void ReadChampionshipFootball()
        {
            foreach (var feed in BetfredFeeds)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("http://xml.betfred.com/" + feed.Value + ".xml");
                Models.Competition comp = new Models.Competition()
                {
                    Id = 999999,
                    Name = feed.Key
                };
                FootballLogic(doc, comp);
            }
            
        }

        public void FootballLogic(XmlDocument doc, Models.Competition comp)
        {
            
            XmlNodeList xnList = doc.SelectNodes("/category/event/bettype[@name='Match Result']");
            foreach(XmlNode eventNode in xnList)
            {
                List<Models.Team> teamsForMatchFields = new List<Models.Team>();
                foreach (XmlNode betNode in eventNode.ChildNodes)
                {
                    TeamMaintenance.IsTeamNameRecorded(betNode.Attributes["name"].Value.ToString(), NewTeams, CurrentTeams);
                    var team = new Models.Team(betNode.Attributes["name"].Value.ToString(), CurrentTeams);
                    if (team.Name != "Draw")
                        teamsForMatchFields.Add(team);
                }
                foreach (XmlNode betNode in eventNode.ChildNodes)
                {
                    String time = eventNode.ParentNode.Attributes["time"].Value.ToString();
                    string correctTime = (time.Contains(':')) ? time : time.Insert(2, ":");
                    var team = new Models.Team(betNode.Attributes["name"].Value.ToString(), CurrentTeams);
                    Models.Match match = new Models.Match()
                    {
                        Id = Convert.ToInt32(Convert.ToDecimal(eventNode.ParentNode.Attributes["eventid"].Value)),
                        Name = eventNode.ParentNode.Attributes["name"].Value.ToString(),
                        Bookmaker = BookmakersConstants.BetfredName,
                        BookmakerId = BookmakersConstants.BetfredId,
                        Competition = comp,
                        LastUpdated = Convert.ToDateTime(eventNode.ParentNode.ParentNode.Attributes["timeGenerated"].Value),
                        Team1 = teamsForMatchFields.First(),
                        Team2 = teamsForMatchFields.Last(),
                        Date = DateTime.ParseExact(eventNode.ParentNode.Attributes["date"].Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                        Time = correctTime,
                        Bet = team.Name,
                        Odds = Convert.ToDecimal(betNode.Attributes["priceDecimal"].Value)
                    };
                    Matches.Add(match);
                }
            }
            //foreach (XmlNode eventNode in doc.FirstChild.ChildNodes)
            //{
            //    foreach(XmlNode childEventNode in eventNode)
            //    {
            //        if(childEventNode.Attributes["name"].Value.ToString() == "Match Result")
            //        {
            //            foreach (XmlNode betNode in childEventNode.ChildNodes)
            //            {
            //                Models.Match match = new Models.Match()
            //                {
            //                    Id = Convert.ToInt32(eventNode.Attributes["eventid"].Value),
            //                    Name = eventNode.Attributes["name"].Value.ToString(),
            //                    Date = Convert.ToDateTime(eventNode.Attributes["date"].Value),
            //                    Time = eventNode.Attributes["date"].Value.ToString(),

            //                };
            //            }

                        
            //        }
            //    }
            //}
        }
    }
}
