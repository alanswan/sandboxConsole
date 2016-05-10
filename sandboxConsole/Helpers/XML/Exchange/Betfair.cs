using sandboxConsole.EF;
using sandboxConsole.Helpers.Maintenance;
using sandboxConsole.Misc;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sandboxConsole.Helpers.XML.Exchange
{
    public class Betfair : Company
    {
        
        public Betfair(List<EF.Team> teams, List<TeamsNotFound> newTeams, List<EF.Competition> comps, List<EF.CompetitionsNotFound> newComps) : base(teams, newTeams, comps, newComps)
        {
            BetfairLogin();
        }

        public void BetfairLogin()
        {
            using (var client = new HttpClient())
            {
                BetfairLoginParams loginParams = new BetfairLoginParams()
                {
                    username = "alanswan",
                    password = "forest99"
                };
                
                //client.BaseAddress = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/listEventTypes/");
                client.BaseAddress = new Uri("https://identitysso.betfair.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Application", "MoHFRmBODsw9VxE1");
                
                
                var response = client.PostAsJsonAsync("api/login", loginParams).Result; 
                var test = response.Content.ReadAsStringAsync().Result;
            }
        }


    }
}
