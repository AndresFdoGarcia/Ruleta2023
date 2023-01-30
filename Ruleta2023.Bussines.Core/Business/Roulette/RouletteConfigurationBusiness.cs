using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Contract;
using Ruleta2023.Domain.Data.Ruleta;
using Ruleta2023.Domain.Data.TResponse;
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

        public CResponseClass OpenRoulette(string id)
        {
            try
            {
                RouletteClass response = rouletteConfiguration.GetRoulette(id).Result;

                if(response.State == "Cerrada")
                    return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

                if (response == null)
                    return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.NOT_MATCH);


                response.State = "Abierta";
                rouletteConfiguration.Update(response);
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return(null);
            }            
        }

        public CResponseClass CloseRoulette(string id)
        {
            try
            {
                RouletteClass response = rouletteConfiguration.GetRoulette(id).Result;

                if (response.State == "Cerrada")
                    return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.INVALID_REQUEST);

                if (response == null)
                    return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.NOT_MATCH);


                response.State = "Cerrada";
                rouletteConfiguration.Update(response);
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
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
    }
}
