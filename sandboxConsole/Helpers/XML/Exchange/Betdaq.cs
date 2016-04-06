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

        public Betdaq()
        {
            this.Competitions = new List<Competition>();
            this.Teams = new List<Team>();
            this.Matches = new List<Models.Match>();
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
                    if(groupNode.Attributes["NAME"].Value.ToString().Contains("Matches"))
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
                                Time = "",
                                Url = ""
                            };
                        }
                    }
                }
                
            }
            //FootballLogic(doc);
        }
        
    }
}
