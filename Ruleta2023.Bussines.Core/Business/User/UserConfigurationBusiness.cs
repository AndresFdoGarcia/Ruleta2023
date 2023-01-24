using MongoDB.Bson;
using Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract;
using Ruleta2023.Domain.Data.Users;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Business.Core.Business.User
{
    public class UserConfigurationBusiness
    {
        private readonly IUserConfigurationManager clientConfiguration;

        public UserConfigurationBusiness (IUserConfigurationManager clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration;
        }

        public TResponse CreateUser(UserClass user)
        {
            if (user.UserNickName == null || user.Role == null)
            {
                return TResponse.TemplateErrorResponse(TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS);
            }

            try
            {
                user.Id = ObjectId.GenerateNewId().ToString();
                clientConfiguration.Save(user);
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
                        if (string.IsNullOrEmpty(message)) message = "Template updated OK";
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
