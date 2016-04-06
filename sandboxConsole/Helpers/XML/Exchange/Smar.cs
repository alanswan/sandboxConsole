using sandboxConsole.Misc;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sandboxConsole.Helpers.XML.Exchange
{
    public class Smar
    {
        public List<Competition> Competitions;
        public List<Models.Match> Matches;
        public List<Team> Teams;

        public Smar()
        {
            this.Competitions = new List<Competition>();
            this.Teams = new List<Team>();
            this.Matches = new List<Models.Match>();
        }
        
        public void ReadSmarUKFootball()
        {
            XmlDocument doc = new XmlDocument();
            //var webRequest = WebRequest.Create(@"http://odds.smarkets.com/oddsfeed.xml");

            //using (var response = webRequest.GetResponse())
            //using (var content = response.GetResponseStream())
            //using (var reader = new StreamReader(content))
            //{
            //    var strContent = reader.ReadToEnd();
            //    var test = "";
            //}
            //doc.
            
            doc.Load("http://odds.smarkets.com/oddsfeed.xml");
            

            FootballLogic(doc);
            
        }

        public void FootballLogic(XmlDocument doc)
        {
            
            foreach(XmlNode node in doc.SelectSingleNode("odds").ChildNodes)
            {
                if(node.Attributes["type"].Value.ToString().ToUpper() == "FOOTBALL MATCH")
                {
                    var match = new Models.Match()
                    {
                        Id = Convert.ToInt32(node.Attributes["id"].Value),
                        Name = node.Attributes["name"].Value.ToString(),
                        Bookmaker = Constants.SmarketsName,
                        BookmakerId = Constants.SmarketsId,
                        //Competition = comp,
                        //LastUpdated = Convert.ToDateTime(matchNode.Attributes["lastUpdateTime"].Value),
                        Team1 = new Team(),
                        Team2 = new Team(),
                        Odds = new Odds(),
                        //Date = Convert.ToDateTime(matchNode.Attributes["date"].Value),
                        //Time = matchNode.Attributes["time"].Value.ToString()
                    };
                    foreach (XmlNode matchNode in node.ChildNodes)
                    {
                        
                        if (matchNode.Attributes["slug"].Value.ToString() == "winner")
                        {
                            foreach (XmlNode winnerNode in matchNode.ChildNodes)
                            {
                                var teamName = winnerNode.Attributes["name"].Value.ToString();
                                var homeaway = winnerNode.Attributes["slug"].Value.ToString();
                                var odds = Convert.ToDecimal(winnerNode.SelectSingleNode("bids").FirstChild.Attributes["decimal"]);
                                
                            }
                        }
                    }
                }
            }
        }
    }
}
