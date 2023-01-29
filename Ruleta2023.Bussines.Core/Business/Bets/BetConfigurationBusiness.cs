using RedLockNet.SERedis;
using Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Contract;
using Ruleta2023.Domain.Data.Bets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Autofac.Features.AttributeFilters;
using MongoDB.Bson;
using Ruleta2023.Domain.Data.Ruleta;
using Serilog;
using Ruleta2023.Data.Access.Redis.RedisCacheRoulette.Contract;
using Ruleta2023.Data.Access.Redis.RedisCacheClient.Contract;
using Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract;
using Ruleta2023.Domain.Data.Users;
using Ruleta2023.Domain.Data.TResponse;
using System.Drawing;

namespace Ruleta2023.Business.Core.Business.Bets
{
    public class BetConfigurationBusiness
    {
        private readonly IRouletteConfigurationManager rouletteConfiguration;
        private readonly IUserConfigurationManager _UserConfiguration;
        private readonly ICacheHelper _RedisCache;
        private readonly ICacheClient _CacheClient;
        private readonly string TtlSeconds;
        

        public BetConfigurationBusiness(
            IRouletteConfigurationManager rouletteConfiguration,
            IUserConfigurationManager userConfiguration,
            ICacheHelper cacheHelper,
            ICacheClient cacheClient,
            string TtlSeconds)
        {
            this.rouletteConfiguration = rouletteConfiguration;
            _RedisCache = cacheHelper;
            _CacheClient = cacheClient;
            _UserConfiguration = userConfiguration;
            this.TtlSeconds = TtlSeconds;            
        }

        public async Task<CResponseClass> MakeBet(BetClass bet, string idRoulette)
        {
            MiddleDataClient middleData = new MiddleDataClient();
            middleData.IdClient = bet.ClientId;
            var ActualDataMoney = Int32.Parse(await PopulateDataMoney(bet.ClientId));

            if (ActualDataMoney < Int32.Parse(bet.MoneyBet.ToString()))
                 return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

            middleData.DataMoneyRn = ActualDataMoney - Int32.Parse(bet.MoneyBet.ToString());
            await _CacheClient.Set(middleData.IdClient, middleData.DataMoneyRn.ToString(), int.Parse(TtlSeconds));

            RouletteClass responseRoulette = rouletteConfiguration.GetRoulette(idRoulette).Result;         

            if (responseRoulette.State == "Cerrada")
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

            if (responseRoulette == null)
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.NOT_MATCH);
       

            try
            {
                bet.Id = ObjectId.GenerateNewId().ToString();
                await _RedisCache.Set(responseRoulette.Id.ToString(), JsonConvert.SerializeObject(bet), int.Parse(TtlSeconds));
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.INTERNAL_ERROR);
            }
            
        }

        public async Task<EventResponse> ResolveBetProcess(string idRoulette)
        {
            try
            {
                List<BetClass> bets = await _RedisCache.GetAllBets(idRoulette);
                Random r = new Random();

                int Rbet = r.Next(0, 36);
                BetData resultBet = BetResultData(Rbet);

                EventResponse FinalResult = new EventResponse();
                FinalResult.Wins = resultBet;
                FinalResult.betTotal = bets;

                await ChooseWinner(bets, resultBet);          

                RouletteClass response = rouletteConfiguration.GetRoulette(idRoulette).Result;
                response.State = "Cerrada";
                await rouletteConfiguration.Update(response);

                await _RedisCache.DeleteKey(idRoulette);

                return FinalResult;
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return new EventResponse { };
            }
            
        }        

        public async Task<string> PopulateDataMoney(string id)
        {
            var TempClietn = await _CacheClient.Get(id);
            string response;

            if (String.IsNullOrEmpty(TempClietn))
            {
                UserClass usertemp = await _UserConfiguration.GetClient(id);
                response = usertemp.MoneyClient.ToString();
                return response;
            }

            response = TempClietn.ToString();
            return response;
        }

        public BetData BetResultData(int i)
        {
            string result;
            if (i % 2 == 0)
            {
                result = "Rojo";
            }
            else
            {
                result = "Negro";
            }

            return new BetData
            {
                NumberSelected = i,
                ColorSelected = result
            };
        }

        public async Task ChooseWinner(List<BetClass> bets, BetData resultBet)
        {
            foreach(BetClass beti in bets) {

                var ActualDataMoney = Int32.Parse(await PopulateDataMoney(beti.ClientId));

                switch (beti.typeBet)
                {
                    case TypeBet.COLOR:
                        if(resultBet.ColorSelected == beti.BetDone.ColorSelected)
                            await _CacheClient.Set(beti.ClientId, (ActualDataMoney + (beti.MoneyBet * 1.8)).ToString(), int.Parse(TtlSeconds));
                        break;

                    case TypeBet.NUMBER:
                        if(resultBet.NumberSelected == beti.BetDone.NumberSelected)
                            await _CacheClient.Set(beti.ClientId, (ActualDataMoney + (beti.MoneyBet * 5)).ToString(), int.Parse(TtlSeconds));
                        break;
                }

            }            
        }

        private class MiddleDataClient
        {
            public string IdClient { get; set; }
            public int DataMoneyRn { get; set; }
        }
        
        public class EventResponse
        {
            public BetData Wins { get; set; }
            public List<BetClass> betTotal { get; set; }
        }
    }
}
