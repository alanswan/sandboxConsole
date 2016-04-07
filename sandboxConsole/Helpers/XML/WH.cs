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

namespace sandboxConsole.Helpers.XML
{
    public class WH
    {
        
        public List<Competition> Competitions;
        public List<Models.Match> Matches;
        public List<EF.Team> CurrentTeams;
        public List<EF.TeamsNotFound> NewTeams;

        public WH(List<EF.Team> teams, List<TeamsNotFound> newTeams) 
        {
            this.Competitions = new List<Competition>();
            this.CurrentTeams = teams;
            this.NewTeams = newTeams;
            this.Matches = new List<Models.Match>();
        }
        
        public void ReadWHUKFootball()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("http://cachepricefeeds.williamhill.com/openbet_cdn?action=template&template=getHierarchyByMarketType&classId=1&marketSort=MR&filterBIR=N");
            FootballLogic(doc);
        }

        public void ReadWHInternationalFootball()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("http://cachepricefeeds.williamhill.com/openbet_cdn?action=template&template=getHierarchyByMarketType&classId=36&marketSort=MR&filterBIR=N");
            FootballLogic(doc);
        }

        public void ReadWHEuroFootball()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("http://cachepricefeeds.williamhill.com/openbet_cdn?action=template&template=getHierarchyByMarketType&classId=275&marketSort=MR&filterBIR=N");
            FootballLogic(doc);
        }


        public void FootballLogic(XmlDocument doc)
        {
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.Name == "response")
                {
                    foreach (XmlNode responseNode in doc.DocumentElement.ChildNodes)
                    {
                        var bookNode = responseNode.SelectSingleNode("williamhill");
                        var sportNode = bookNode?.SelectSingleNode("class");

                        foreach (XmlNode compNode in sportNode.ChildNodes)
                        {
                            //store competition ids and names
                            var comp = new Competition()
                            {
                                Id = Convert.ToInt32(compNode.Attributes["id"].Value),
                                Name = compNode.Attributes["name"].Value.ToString()
                            };
                            if (!Competitions.Any(x => x.Id == comp.Id)) { Competitions.Add(comp); }

                            foreach (XmlNode matchNode in compNode.ChildNodes)
                            {
                                var matchNodeName = matchNode.Attributes["name"].Value.ToString();
                                if (matchNodeName.Contains("Match Betting"))
                                {
                                    List<Models.Team> teamsForMatchFields = new List<Models.Team>();
                                    foreach (XmlNode parNode in matchNode.ChildNodes)
                                    {
                                        TeamMaintenance.IsTeamNameRecorded(parNode.Attributes["name"].Value.ToString(), NewTeams, CurrentTeams);
                                        var team = new Models.Team(parNode.Attributes["name"].Value.ToString(), CurrentTeams);
                                        if(team.Name != "Draw")
                                            teamsForMatchFields.Add(team);
                                    }
                                    foreach (XmlNode parNode in matchNode.ChildNodes)
                                    {
                                        var match = new Models.Match()
                                        {
                                            Id = Convert.ToInt32(matchNode.Attributes["id"].Value),
                                            Name = matchNodeName,
                                            Bookmaker = Constants.WilliamHillName,
                                            BookmakerId = Constants.WilliamHillId,
                                            Competition = comp,
                                            LastUpdated = Convert.ToDateTime(matchNode.Attributes["lastUpdateTime"].Value),
                                            Team1 = teamsForMatchFields.First(),
                                            Team2 = teamsForMatchFields.Last(),
                                            Date = Convert.ToDateTime(matchNode.Attributes["date"].Value),
                                            Time = matchNode.Attributes["time"].Value.ToString()
                                        };
                                        //store team if not already there
                                        
                                        var team = new Models.Team(parNode.Attributes["name"].Value.ToString(), CurrentTeams);
                                        match.Bet = team.Name;
                                        match.Odds = Convert.ToDecimal(parNode.Attributes["oddsDecimal"].Value);
                                        
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
