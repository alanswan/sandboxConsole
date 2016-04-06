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
    public class Betdaq
    {
        public List<Competition> Competitions;
        public List<Models.Match> Matches;
        public List<Team> Teams;

        public List<string> BetdaqMatchPhrases;

        public Betdaq()
        {
            this.Competitions = new List<Competition>();
            this.Teams = new List<Team>();
            this.Matches = new List<Models.Match>();
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
                var comp = new Competition()
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
                            Match match = new Match()
                            {
                                Id = Convert.ToInt32(matchNode.Attributes["ID"].Value),
                                Name = matchNode.Attributes["NAME"].Value.ToString(),
                                Bookmaker = Constants.BetdaqName,
                                BookmakerId = Constants.BetdaqId,
                                Competition = comp,
                                LastUpdated = DateTime.Now,
                                Team1 = new Team(),
                                Team2 = new Team(),
                                Odds = new Odds(),
                                Date = Convert.ToDateTime(matchNode.Attributes["DATE"].Value),
                                Time = ""
                            };
                            foreach(XmlNode oddsNode in matchNode)
                            {
                                if(oddsNode.Attributes["NAME"].Value.ToString() == "Match Odds")
                                {
                                    foreach(XmlNode oddsChildNode in oddsNode)
                                    {
                                        if (oddsChildNode.Name.ToString() == "LINK")
                                        {
                                            match.Url = oddsChildNode.Attributes["URL"].Value.ToString();
                                            match.MobileUrl = oddsChildNode.Attributes["MOBILE_URL"].Value.ToString();
                                        }
                                        else if(oddsChildNode.Attributes["NAME"].Value == "Draw")
                                        {
                                            foreach(XmlNode layDrawNode in oddsChildNode)
                                            {
                                                if(layDrawNode.Attributes["NAME"].Value.ToString() == "Lay Draw")
                                                {
                                                    match.Odds.Draw = Convert.ToDecimal(layDrawNode.FirstChild?.FirstChild?.Attributes["VALUE"]?.Value);
                                                   // match.Odds.Draw = (match.Odds.Draw == null) ? 0.00 : match.Odds.Draw;
                                                }
                                            }
                                             
                                        }
                                        else if(match.Team1.Name == null)
                                        {
                                            match.Team1.Id = Convert.ToInt32(oddsChildNode.Attributes["ID"]?.Value);
                                            match.Team1.Name = oddsChildNode.Attributes["NAME"]?.Value.ToString();
                                            match.Team1.Name = (match.Team1.Name == null) ? "" : match.Team1.Name;
                                            foreach (XmlNode team1Node in oddsChildNode)
                                            {
                                                if (team1Node.Attributes["NAME"].Value.ToString() == "Lay " + match.Team1.Name)
                                                {
                                                    match.Odds.Team1 = Convert.ToDecimal(team1Node.FirstChild?.FirstChild?.Attributes["VALUE"]?.Value);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            match.Team2.Id = Convert.ToInt32(oddsChildNode.Attributes["ID"].Value);
                                            match.Team2.Name = oddsChildNode.Attributes["NAME"].Value.ToString();
                                            match.Team2.Name = (match.Team2.Name == null) ? "" : match.Team2.Name;
                                            foreach (XmlNode team2Node in oddsChildNode)
                                            {
                                                if (team2Node.Attributes["NAME"].Value.ToString() == "Lay " + match.Team2.Name)
                                                {
                                                    match.Odds.Team2 = Convert.ToDecimal(team2Node.FirstChild?.FirstChild?.Attributes["VALUE"]?.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            Matches.Add(match);

                        }
                    }
                }
                
            }
            //FootballLogic(doc);
        }
       
        
        
        
        
         
    }
}
