using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Contract;
using Ruleta2023.Domain.Data.Ruleta;
using Serilog;

namespace Ruleta2023.Business.Core.Business.Roulette
{
    public class RouletteConfigurationBusiness
    {
        private readonly IRouletteConfigurationManager rouletteConfiguration;

        public RouletteConfigurationBusiness(IRouletteConfigurationManager rouletteConfiguration)
        {
            this.rouletteConfiguration = rouletteConfiguration;
        }

        public TResponse CreateRoulette(RouletteClass roulette)
        {
            if (roulette.State == null)
            {
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS);
            }

            try
            {
                roulette.Id = ObjectId.GenerateNewId().ToString();
                rouletteConfiguration.Save(roulette);
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS);
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
                        if (string.IsNullOrEmpty(message)) message = "Resgistered";
                        break;

                    case TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS:
                        errorCode = 400;
                        if (string.IsNullOrEmpty(message)) message = "A mandatory data is missing";
                        break;

                    case TemplateErrorResponseCode.UPDATE_SUCCESS:
                        errorCode = 200;
                        if (string.IsNullOrEmpty(message)) message = "Roulette state updated OK";
                        break;

                    case TemplateErrorResponseCode.INTERNAL_ERROR:
                        errorCode = 500;
                        if (string.IsNullOrEmpty(message)) message = "Internal server error";
                        break;

                }

                return new TResponse
                {
                    statusCode = errorCode,
                    statusmessage = message
                };
            }
        }

        public enum TemplateErrorResponseCode
        {
            MISSING_MANDATORY_PARAMETERS = 1,
            DATA_OK = 0,
            UPDATE_SUCCESS = 2,
            INTERNAL_ERROR = 1599
        }
    }
}
