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

        public string CreateRoulette()
        {           
            RouletteClass roulette = new RouletteClass();
            try
            {
                roulette.Id = ObjectId.GenerateNewId().ToString();
                rouletteConfiguration.Save(roulette);
                return roulette.Id;
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return ("Failed to create");
            }
        }

        public TResponse OpenRoulette(string id)
        {
            try
            {
                RouletteClass response = rouletteConfiguration.GetRoulette(id).Result;

                if(response.State == "Cerrada")
                    return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

                if (response == null)
                    return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.NOT_MATCH);


                response.State = "Abierta";
                rouletteConfiguration.Update(response);
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return(null);
            }            
        }

        public TResponse CloseRoulette(string id)
        {
            try
            {
                RouletteClass response = rouletteConfiguration.GetRoulette(id).Result;

                if (response.State == "Cerrada")
                    return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

                if (response == null)
                    return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.NOT_MATCH);


                response.State = "Cerrada";
                rouletteConfiguration.Update(response);
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return (null);
            }
        }

        public List<RouletteClass> GetAllRouletts()
        {
            List<RouletteClass> response = rouletteConfiguration.GetAll().Result;
            return response;
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
                        if (string.IsNullOrEmpty(message)) message = "Roulette oppend";
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

                    case TemplateErrorResponseCode.INVALID_REQUEST:
                        errorCode= 400;
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

        public enum TemplateErrorResponseCode
        {
            MISSING_MANDATORY_PARAMETERS = 1,
            DATA_OK = 0,
            UPDATE_SUCCESS = 2,
            INTERNAL_ERROR = 1599,
            INVALID_REQUEST = 3,
            NOT_MATCH = 4
        }
    }
}
