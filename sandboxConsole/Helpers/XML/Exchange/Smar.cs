using sandboxConsole.Helpers.DataManipulation;
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
            //XmlDocument doc = new XmlDocument();
            //var webRequest = WebRequest.Create(@"http://odds.smarkets.com/oddsfeed.xml");

            //using (var response = webRequest.GetResponse())
            //using (var content = response.GetResponseStream())
            //using (var reader = new StreamReader(content))
            //{
            //    var strContent = reader.ReadToEnd();
            //    var test = "";
            //}
            //doc.

            //using (var client = new WebClient())
            //{
            //    var xml = client.DownloadString("http://odds.smarkets.com/oddsfeed.xml");
            //    using (var strReader = new StringReader(xml))
            //    using (var reader = XmlReader.Create(strReader))
            //    {

            //    }
            //}

            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                byte[] data = client.DownloadData("http://odds.smarkets.com/oddsfeed.xml.gz ");
                byte[] decompress = FileManipulation.Decompress(data);

                // doc.Load("http://odds.smarkets.com/oddsfeed.xml");
                XmlDocument doc = new XmlDocument();
                MemoryStream ms = new MemoryStream(decompress);
                doc.Load(ms);
                FootballLogic(doc);
            }
            //FootballLogic(doc);
            
        }

        public void FootballLogic(XmlDocument doc)
        {
            
            foreach(XmlNode node in doc.SelectSingleNode("odds").ChildNodes)
            {
                if(node.Attributes["type"].Value.ToString().ToUpper() == "FOOTBALL MATCH")
                {
                    var comp = new Competition()
                    {
                        Id = 0,
                        Name = node.Attributes["parent"].Value.ToString()
                    };

                    var match = new Models.Match()
                    {
                        Id = Convert.ToInt32(node.Attributes["id"].Value),
                        Name = node.Attributes["name"].Value.ToString(),
                        Bookmaker = Constants.SmarketsName,
                        BookmakerId = Constants.SmarketsId,
                        Competition = comp,
                        LastUpdated = DateTime.Now,
                        Team1 = new Team(),
                        Team2 = new Team(),
                       // Odds = new Odds(),
                        Date = Convert.ToDateTime(node.Attributes["date"].Value),
                        Time = node.Attributes["time"].Value.ToString(),
                        Url = node.Attributes["url"].Value.ToString()
                    };
                    foreach (XmlNode matchNode in node.ChildNodes)
                    {
                        
                        if (matchNode.Attributes["slug"].Value.ToString() == "winner")
                        {
                            foreach (XmlNode winnerNode in matchNode.ChildNodes)
                            {
                                var teamName = winnerNode.Attributes["name"].Value.ToString();
                                var teamId = Convert.ToInt32(winnerNode.Attributes["id"].Value);
                                var homeaway = winnerNode.Attributes["slug"].Value.ToString();

                                XmlNode bidsNode = winnerNode.SelectSingleNode("bids").FirstChild;
                                if (bidsNode != null)
                                {
                                    var odds = Convert.ToDecimal(bidsNode.Attributes["decimal"].Value);
                                    //_________________________________________________________________
                                    // Commeted out due to data structure change
                                    //if (homeaway == "home")
                                    //{
                                    //    match.Team1.Id = teamId;
                                    //    match.Team1.Name = teamName;
                                    //    match.Odds.Team1 = odds;
                                    //}
                                    //else if (homeaway == "away")
                                    //{
                                    //    match.Team2.Id = teamId;
                                    //    match.Team2.Name = teamName;
                                    //    match.Odds.Team2 = odds;
                                    //}
                                    //else
                                    //{
                                    //    match.Odds.Draw = odds;
                                    //}
                                    //________________________________________________________
                                }
                            }
                        }
                    }
                    Matches.Add(match);
                }
            }
        }







    }
}
