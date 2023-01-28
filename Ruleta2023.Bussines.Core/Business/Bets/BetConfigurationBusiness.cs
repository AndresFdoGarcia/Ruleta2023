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

namespace Ruleta2023.Business.Core.Business.Bets
{
    public class BetConfigurationBusiness
    {
        private readonly IRouletteConfigurationManager rouletteConfiguration;
        private readonly IUserConfigurationManager _UserConfiguration;
        private readonly ICacheHelper _RedisCache;
        private readonly ICacheClient _CacheClient;
        private readonly string RedisTtlSeconds;
        

        public BetConfigurationBusiness(
            IRouletteConfigurationManager rouletteConfiguration,
            IUserConfigurationManager userConfiguration,
            ICacheHelper cacheHelper,
            ICacheClient cacheClient,
            [KeyFilter("RedisTtlSeconds")] string redisTtlSeconds)
        {
            this.rouletteConfiguration = rouletteConfiguration;
            _RedisCache = cacheHelper;
            _CacheClient = cacheClient;
            _UserConfiguration = userConfiguration;
            RedisTtlSeconds = redisTtlSeconds;            
        }

        public async Task<TResponse> MakeBet(BetClass bet, string idRoulette)
        {
            MiddleDataClient middleData = new MiddleDataClient();
            middleData.IdClient = bet.ClientId;
            var ActualDataMoney = Int32.Parse(await PopulateDataMoney(bet.ClientId));

            if (ActualDataMoney < Int32.Parse(bet.MoneyBet.ToString()))
                 return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

            middleData.DataMoneyRn = ActualDataMoney - Int32.Parse(bet.MoneyBet.ToString());
            await _CacheClient.Set(middleData.IdClient, middleData.DataMoneyRn.ToString(), int.Parse(RedisTtlSeconds));

            RouletteClass responseRoulette = rouletteConfiguration.GetRoulette(idRoulette).Result;         

            if (responseRoulette.State == "Cerrada")
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

            if (responseRoulette == null)
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.NOT_MATCH);
       

            try
            {
                bet.Id = ObjectId.GenerateNewId().ToString();
                await _RedisCache.Set(responseRoulette.Id.ToString(), JsonConvert.SerializeObject(bet), int.Parse(RedisTtlSeconds));
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.INTERNAL_ERROR);
            }
            
        }

        public class TResponse
        {
            public string statusmessage { get; set; }
            public int statusCode { get; set; }

            public static TResponse TemplateErrorResponse(TemplateErrorResponseCode error)
            {
                int errorCode = 0;
                string message = "";

                switch (error)
                {
                    case TemplateErrorResponseCode.DATA_OK:
                        errorCode = 200;
                        if (string.IsNullOrEmpty(message)) message = "Success";
                        break;

                    case TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS:
                        errorCode = 400;
                        if (string.IsNullOrEmpty(message)) message = "A mandatory data is missing";
                        break;                    

                    case TemplateErrorResponseCode.INTERNAL_ERROR:
                        errorCode = 500;
                        if (string.IsNullOrEmpty(message)) message = "Internal server error";
                        break;

                    case TemplateErrorResponseCode.INVALID_REQUEST:
                        errorCode = 400;
                        if (string.IsNullOrEmpty(message)) message = "The roulette is already closed";
                        break;

                    case TemplateErrorResponseCode.NOT_MATCH:
                        errorCode = 400;
                        if (string.IsNullOrEmpty(message)) message = "The roulette doesn't exist";
                        break;
                }

                return new TResponse
                {
                    statusCode = errorCode,
                    statusmessage = message
                };
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

        public enum TemplateErrorResponseCode
        {
            MISSING_MANDATORY_PARAMETERS = 1,
            DATA_OK = 0,            
            INTERNAL_ERROR = 1599,
            INVALID_REQUEST = 3,
            NOT_MATCH = 4
        }

        public class MiddleDataClient
        {
            public string IdClient { get; set; }
            public int DataMoneyRn { get; set; }
        }
    }
}
