
using Newtonsoft.Json;
using sandboxConsole.Helpers.Maintenance;
using sandboxConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;
using sandboxConsole.EF;
using EntityFramework.BulkInsert.Extensions;
using System.Diagnostics;
using sandboxConsole.Helpers.XML;
using sandboxConsole.Misc;

namespace sandboxConsole.Helpers.XML.Exchange
{
    public class Betfair : Company
    {
        //Prem 31
        // 
        //public Betfair(List<EF.Team> teams, List<TeamsNotFound> newTeams, List<EF.Competition> comps, List<EF.CompetitionsNotFound> newComps) : base(teams, newTeams, comps, newComps)
        //{
        //    BetfairLogin();
        //}
        private BetfairLoginResponse LoginResponse;
        private List<BFCompetition> BfCompetitions;
        private List<string> EventIds;
        private List<BFMarket> BfMarkets;
        private List<BFMarketBook> BfMarketBooks;
        private List<BFEvent> BfEvents;
        private List<string> CompetitionRegions;
        private omproEntities db = new omproEntities();
        public Betfair(List<sandboxConsole.EF.Team> teams, List<sandboxConsole.EF.TeamsNotFound> newTeams, List<sandboxConsole.EF.Competition> comps, List<sandboxConsole.EF.CompetitionsNotFound> newComps) : base(teams, newTeams, comps, newComps)
        {
            this.LoginResponse = new BetfairLoginResponse();
            this.BfCompetitions = new List<BFCompetition>();
            this.BfMarkets = new List<BFMarket>();
            this.BfMarketBooks = new List<BFMarketBook>();
            this.BfEvents = new List<BFEvent>();
            //this.CompetitionRegions = new List<string>() {"GBR", "ESP", "ITA", "DEU", "International"};

            this.EventIds = new List<string>();
        }

        public void Login()
        {
            BetfairLoginResponse loginReponse = new BetfairLoginResponse();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://identitysso.betfair.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Application", "MoHFRmBODsw9VxE1");

                var loginContent = new StringContent("username=kevin@wearedandify.com&password=forest99", Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = client.PostAsync("api/login", loginContent).Result;
                LoginResponse = (BetfairLoginResponse)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(BetfairLoginResponse));

                //db.Errors.Add(new Error() { Error1 = LoginResponse.status + "|||" + LoginResponse.error });
            }
        }

        public void ReadBetfairFootball(string region)
        {
            this.CompetitionRegions = new List<string>() { region };
            GetCompetitions();
            GetEvents();
            GetMarkets();
            GetMarketBooks();

            FootballLogic();
        }

        public void FootballLogic()
        {
            foreach (var market in BfMarkets)
            {
                var competitionName = market.Competition.Name;
                CompetitionMaintenance.IsCompetitionRecorded(competitionName, NewComps, CurrentComps);
                sandboxConsole.Models.Competition comp = new sandboxConsole.Models.Competition(competitionName, CurrentComps);

                var marketBook = BfMarketBooks.Where(x => x.MarketId == market.MarketId).SingleOrDefault();

                List<sandboxConsole.Models.Team> teamsForMatchFields = new List<sandboxConsole.Models.Team>();
                foreach (var marketRunner in market.Runners)
                {
                    TeamMaintenance.IsTeamNameRecorded(marketRunner.RunnerName, NewTeams, CurrentTeams);
                    var team = new sandboxConsole.Models.Team(marketRunner.RunnerName, CurrentTeams);
                    if (team.Name != "The Draw")
                        teamsForMatchFields.Add(team);
                }

                foreach (var marketRunner in market.Runners)
                {
                    var exchangeOdds =
                        marketBook.Runners.Where(x => x.SelectionId == marketRunner.SelectionId)
                            .Select(x => x.ExchangePrices)
                            .Select(x => x.AvailableToLay)
                            .FirstOrDefault();
                    if (exchangeOdds != null && exchangeOdds.Count != 0)
                    {
                        var odds = exchangeOdds.Min(x => x.Price);
                        var money = exchangeOdds.Where(x => x.Price == odds).Select(x => x.Size).SingleOrDefault();

                        var match = new sandboxConsole.Models.Match()
                        {
                            Id = Convert.ToInt32(Convert.ToDecimal(market.MarketId)),
                            Name = market.Event.Name,
                            Bookmaker = BookmakersConstants.BetfairName,
                            BookmakerId = BookmakersConstants.BetfairId,
                            Competition = comp,
                            LastUpdated = DateTime.Now,
                            Team1 = teamsForMatchFields.First(),
                            Team2 = teamsForMatchFields.Last(),
                            Date = market.Event.OpenDate.Date,
                            Time = market.Event.OpenDate.AddHours(1).TimeOfDay.ToString().Substring(0, 5),
                            Bet = (marketRunner.RunnerName == "The Draw") ? "Draw" : marketRunner.RunnerName,
                            Odds = Convert.ToDecimal(odds),
                            MoneyInMarket = Convert.ToDecimal(money)
                        };
                        this.Matches.Add(match);
                    }
                }
            }
        }

        

        public void GetCompetitions()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    client.BaseAddress = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-Application", "MoHFRmBODsw9VxE1");
                    client.DefaultRequestHeaders.Add("X-Authentication", LoginResponse.token);

                    //football
                    int[] eventTypeIds = new int[1];
                    eventTypeIds[0] = 1;

                    BetfairEventRequest request = new BetfairEventRequest()
                    {
                        Filter = new Filter() { EventTypeIds = eventTypeIds },
                        MaxResults = 999
                    };
                    //BetfairEventRequest request = new BetfairEventRequest() { filter = new Filter() {  } };

                    var convertedObject = JsonConvert.SerializeObject(request);
                    var httpContent = new StringContent(convertedObject, Encoding.UTF8, "application/json");
                    //var httpContent = new StringContent("filter=", Encoding.UTF8, "application/x-www-form-urlencoded");
                    //error = client.PostAsync("listCompetitions/", httpContent).Result.ToString();
                    response = client.PostAsync("listCompetitions/", httpContent).Result;
                    BfCompetitions =
                        (List<BFCompetition>)
                            JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result,
                                typeof(List<BFCompetition>));
                    BfCompetitions =
                        BfCompetitions.Where(x => CompetitionRegions.Contains(x.CompetitionRegion)).ToList();
                }
                catch (Exception e)
                {
                    ErrorHelper.CreateError(BookmakersConstants.BetfairName,response.Content.ReadAsStringAsync().Result);
                }
            }
        }
        public void GetEvents()
        {
            var compIds = BfCompetitions.Where(x => x.MarketCount > 0).Select(x => x.CompetitionDetails.Id).ToList();
            //foreach (var compId in compIds)
            //{
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    client.BaseAddress = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-Application", "MoHFRmBODsw9VxE1");
                    client.DefaultRequestHeaders.Add("X-Authentication", LoginResponse.token);

                    int[] eventTypeIds = new int[1];
                    eventTypeIds[0] = 1;

                    BetfairEventRequest request = new BetfairEventRequest()
                    {
                        Filter = new Filter() { EventTypeIds = eventTypeIds, CompetitionIds = compIds.ToArray() },
                        MaxResults = 999
                    };
                    //BetfairEventRequest request = new BetfairEventRequest() { filter = new Filter() {  } };

                    var convertedObject = JsonConvert.SerializeObject(request);
                    var httpContent = new StringContent(convertedObject, Encoding.UTF8, "application/json");
                    //var httpContent = new StringContent("filter=", Encoding.UTF8, "application/x-www-form-urlencoded");
                    response = client.PostAsync("listEvents/", httpContent).Result;
                    var events = (List<BFEvent>)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<BFEvent>));
                    BfEvents.AddRange(events);
                    EventIds.AddRange(events.Select(x => x.eventDetails.Id).ToList());
                }
                catch (Exception e)
                {
                    ErrorHelper.CreateError(BookmakersConstants.BetfairName, response.Content.ReadAsStringAsync().Result);
                }
            }
        }
        public void GetMarkets()
        {
            var compIds = BfCompetitions.Where(x => x.MarketCount > 0).Select(x => x.CompetitionDetails.Id).ToList();
            //foreach (var comp in compIds)
            //{
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    client.BaseAddress = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-Application", "MoHFRmBODsw9VxE1");
                    client.DefaultRequestHeaders.Add("X-Authentication", LoginResponse.token);

                    int[] eventTypeIds = new int[1];
                    eventTypeIds[0] = 1;

                    string[] marketTypeCodes = new string[1];
                    marketTypeCodes[0] = "MATCH_ODDS";

                    BetfairEventRequest request = new BetfairEventRequest()
                    {
                        Filter =
                            new Filter()
                            {
                                EventTypeIds = eventTypeIds,
                                CompetitionIds = compIds.ToArray(),
                                MarketTypeCodes = marketTypeCodes
                            },
                        MaxResults = 999,
                        MarketProjection = new string[] { "COMPETITION", "EVENT", "RUNNER_DESCRIPTION", "RUNNER_METADATA" }

                    };
                    //BetfairEventRequest request = new BetfairEventRequest() { filter = new Filter() {  } };

                    var convertedObject = JsonConvert.SerializeObject(request);
                    var httpContent = new StringContent(convertedObject, Encoding.UTF8, "application/json");
                    //var httpContent = new StringContent("filter=", Encoding.UTF8, "application/x-www-form-urlencoded");
                    response = client.PostAsync("listMarketCatalogue/", httpContent).Result;
                    //there are more details available in the description property of a BFMarket - not currently called as deemed as not needed - see MarketCatalogue in the api
                    var markets = (List<BFMarket>)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(List<BFMarket>));
                    this.BfMarkets.AddRange(markets);
                }
                catch (Exception e)
                {
                    ErrorHelper.CreateError(BookmakersConstants.BetfairName, response.Content.ReadAsStringAsync().Result);
                }
            }
            //}
        }

        public void GetMarketBooks()
        {
            for (int i = 0; i < this.BfMarkets.Count; i = i + 10)
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    try
                    {
                        System.Net.ServicePointManager.Expect100Continue = false;
                        client.BaseAddress = new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("X-Application", "MoHFRmBODsw9VxE1");
                        client.DefaultRequestHeaders.Add("X-Authentication", LoginResponse.token);
                        decimal[] marketIds = this.BfMarkets.Select(x => x.MarketId).ToArray();

                        BetfairMarketRequest request = new BetfairMarketRequest()
                        {
                            MarketIds = this.BfMarkets.Select(x => x.MarketId.ToString()).Skip(i).Take(10).ToArray(),
                            PriceProjection = new PriceProjection()
                            {
                                PriceData = new string[2] { "EX_BEST_OFFERS", "EX_TRADED" },
                                Virtualise = "true"
                            },
                            //OrderProjection = "ALL",
                            //MatchProjection = "NO_ROLLUP"

                        };
                        var convertedObject = JsonConvert.SerializeObject(request);
                        var httpContent = new StringContent(convertedObject, Encoding.UTF8, "application/json");
                        response = client.PostAsync("listMarketBook/", httpContent).Result;
                        var marketBooks =
                            (List<BFMarketBook>)
                                JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result,
                                    typeof(List<BFMarketBook>));
                        this.BfMarketBooks.AddRange(marketBooks);
                    }
                    catch (Exception e)
                    {
                        ErrorHelper.CreateError(BookmakersConstants.BetfairName, response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
        }




    }
}
