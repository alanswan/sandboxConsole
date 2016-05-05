using sandboxConsole.EF;
using sandboxConsole.Helpers.Maintenance;
using sandboxConsole.Misc;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sandboxConsole.Helpers.XML
{
    public class Eight88 : Company
    {
        public Eight88(List<EF.Team> teams, List<TeamsNotFound> newTeams, List<EF.Competition> comps, List<EF.CompetitionsNotFound> newComps) : base(teams, newTeams, comps, newComps)
        {}

        public void Read888Football()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://www.smart-feeds.com/getfeeds.aspx?Param=event/group/1000093656");
            }
            
        }
        
        
    }
}
