using sandboxConsole.EF;
using sandboxConsole.Helpers.Maintenance;
using sandboxConsole.Misc;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sandboxConsole.Helpers.XML.Exchange
{
    public class Betdaq : Company
    {
        public List<string> BetdaqMatchPhrases;
        public Betdaq(List<EF.Team> teams, List<TeamsNotFound> newTeams) : base(teams, newTeams)
        {
            // todo - need to get all relevant leagues/internationals and put in leagues for the next few years.
            this.BetdaqMatchPhrases = new List<string>() { "Serie A 2015/2016", "La Liga 2015/16", "Serie A 2016/2017", "La Liga 2016/17", "Champions League Matches", "Europa League Matches", "The Championship", "Premier League", "English League One", "English League Two", "Scottish Premiership 2015/16", "Scottish Premiership 2016/17" };
        }
        public void ReadBetdaqFootball()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("http://xml.betdaq.com/soccer");
            XmlNode node = doc.SelectSingleNode("root").SelectSingleNode("SPORT");
            foreach(XmlNode compNode in node.ChildNodes)
            {
                var comp = new Models.Competition()
                {
                    Id = Convert.ToInt32(compNode.Attributes["ID"].Value),
                    Name = compNode.Attributes["NAME"].Value.ToString()
                };
                Competitions.Add(comp);

                foreach(XmlNode groupNode in compNode.ChildNodes)
                {
                    var groupName = groupNode.Attributes["NAME"].Value.ToString().Trim();
                    if (BetdaqMatchPhrases.Contains(groupName))
                    {
                        foreach(XmlNode matchNode in groupNode.ChildNodes)
                        {
                            foreach(XmlNode oddsNode in matchNode)
                            {
                                if(oddsNode.Attributes["NAME"].Value.ToString() == "Match Odds")
                                {
                                    var url = "";
                                    var moburl = "";
                                    List<Models.Team> teams = new List<Models.Team>();

                                    foreach (XmlNode oddsChildNode in oddsNode)
                                    {
                                        if (oddsChildNode.Name.ToString() == "LINK")
                                        {
                                            url = oddsChildNode.Attributes["URL"].Value.ToString();
                                            moburl = oddsChildNode.Attributes["MOBILE_URL"].Value.ToString();
                                        }
                                        else {
                                            var name = oddsChildNode.Attributes["NAME"].Value.ToString();
                                            if (name.ToUpper() != "DRAW")
                                            {
                                                TeamMaintenance.IsTeamNameRecorded(name, NewTeams, CurrentTeams);
                                                teams.Add(new Models.Team(name, CurrentTeams));
                                            }
                                        }
                                    }

                                    foreach (XmlNode oddsChildNode in oddsNode)
                                    {
                                        if (oddsChildNode.Name.ToString() != "LINK")
                                        {
                                            if (oddsChildNode.Attributes["NAME"].Value == "Draw")
                                            {
                                                foreach (XmlNode layDrawNode in oddsChildNode)
                                                {   
                                                    if (layDrawNode.Attributes["NAME"].Value.ToString() == "Lay Draw")
                                                    {
                                                        if (layDrawNode.FirstChild.FirstChild != null)
                                                        {
                                                            var match = new Models.Match()
                                                            {
                                                                Id = Convert.ToInt32(matchNode.Attributes["ID"].Value),
                                                                Name = matchNode.Attributes["NAME"].Value.ToString(),
                                                                Bookmaker = BookmakersConstants.BetdaqName,
                                                                BookmakerId = BookmakersConstants.BetdaqId,
                                                                Competition = comp,
                                                                LastUpdated = DateTime.Now,
                                                                Team1 = teams.First(),
                                                                Team2 = teams.Last(),
                                                                Bet = "Draw",
                                                                Odds = Convert.ToDecimal(layDrawNode.FirstChild?.FirstChild?.Attributes["VALUE"]?.Value),
                                                                Date = Convert.ToDateTime(matchNode.Attributes["DATE"].Value),
                                                                Time = ""
                                                            };
                                                            foreach (XmlNode moneyNode in layDrawNode.FirstChild?.FirstChild?.ChildNodes)
                                                            {
                                                                if (moneyNode.Attributes["CURRENCY"].Value == "GBP")
                                                                    match.MoneyInMarket = Convert.ToDecimal(moneyNode.Attributes["VALUE"].Value);
                                                            }
                                                            Matches.Add(match);
                                                        }

                                                    }
                                                }

                                            }
                                            else
                                            {
                                                foreach (XmlNode teamNode in oddsChildNode)
                                                {
                                                    if (teamNode.FirstChild.FirstChild != null)
                                                    { 
                                                        Models.Match match = new Models.Match()
                                                        {
                                                            Id = Convert.ToInt32(matchNode.Attributes["ID"].Value),
                                                            Name = matchNode.Attributes["NAME"].Value.ToString(),
                                                            Bookmaker = BookmakersConstants.BetdaqName,
                                                            BookmakerId = BookmakersConstants.BetdaqId,
                                                            Competition = comp,
                                                            LastUpdated = DateTime.Now,
                                                            Team1 = teams.First(),
                                                            Team2 = teams.Last(),
                                                            Date = Convert.ToDateTime(matchNode.Attributes["DATE"].Value),
                                                            Time = ""
                                                        };

                                                        // todo the below errors sometimes - need to put a check in that the firstchild.firstchild node is there
                                                        foreach (XmlNode moneyNode in teamNode.FirstChild?.FirstChild?.ChildNodes)
                                                        {
                                                            if (moneyNode.Attributes["CURRENCY"].Value == "GBP")
                                                                match.MoneyInMarket = Convert.ToDecimal(moneyNode.Attributes["VALUE"].Value);
                                                        }
                                                        if (teamNode.Attributes["NAME"].Value.ToString() == "Lay " + teams.First().Name)
                                                        {
                                                            match.Bet = teams.First().Name;
                                                            match.Odds = Convert.ToDecimal(teamNode.FirstChild?.FirstChild?.Attributes["VALUE"]?.Value);
                                                        }
                                                        else if (teamNode.Attributes["NAME"].Value.ToString() == "Lay " + teams.Last().Name)
                                                        {
                                                            match.Bet = teams.Last().Name;
                                                            match.Odds = Convert.ToDecimal(teamNode.FirstChild?.FirstChild?.Attributes["VALUE"]?.Value);
                                                        }
                                                        if (match.Bet != null)
                                                            Matches.Add(match);
                                                    }
                                                }

                                            }
                                          }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                
            }
         
        
       
        
        
        
        
         
    }
}
