using sandboxConsole.EF;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sandboxConsole.Helpers.XML
{
    public class Company
    {
        public List<Models.Competition> Competitions;
        public List<Models.Match> Matches;
        public List<Models.Race> Races; 
        public List<EF.Team> CurrentTeams;
        public List<EF.TeamsNotFound> NewTeams;
        public List<EF.Competition> CurrentComps;
        public List<EF.CompetitionsNotFound> NewComps;

        public Company(List<EF.Team> teams, List<TeamsNotFound> newTeams, List<EF.Competition> comps, List<EF.CompetitionsNotFound> newComps )
        {
            this.Competitions = new List<Models.Competition>();
            this.CurrentTeams = teams;
            this.NewTeams = newTeams;
            this.CurrentComps = comps;
            this.NewComps = newComps;
            this.Matches = new List<Models.Match>();
            this.Races = new List<Models.Race>();
        }
    }
}
