using MongoDB.Bson;
using Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract;
using Ruleta2023.Domain.Data.TResponse;
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

        public CResponseClass CreateUser(UserClass user)
        {
            if (user.UserNickName == null || user.Role == null)
            {
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS);
            }

            try
            {
                user.Id = ObjectId.GenerateNewId().ToString();
                clientConfiguration.Save(user);
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.DATA_OK);
            }
            catch (Exception ex)
            {
                Log.Error("Exception {0}", ex);
                return CResponseClass.TemplateErrorResponse(TemplateErrorResponseCode.MISSING_MANDATORY_PARAMETERS);
            }
        }        
    }
}
