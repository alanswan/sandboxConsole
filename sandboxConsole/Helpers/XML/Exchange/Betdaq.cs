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

                foreach(XmlNode matchNode in compNode.ChildNodes)
                {

                }
                
            }
            //FootballLogic(doc);
        }
        
    }
}
