using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sandboxConsole.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Match
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bookmaker { get; set; }
        public int BookmakerId { get; set; }
        public Competition Competition { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public Odds Odds { get; set; }

        public DateTime Date { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Time { get; set; }
        public string Url { get; set; }
        public string MobileUrl { get; set; }

    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Odds
    {
        public decimal Team1 { get; set; }
        public decimal Team2 { get; set; }
        public decimal Draw { get; set; }
    }
}
