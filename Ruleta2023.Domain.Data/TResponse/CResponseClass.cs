using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Domain.Data.TResponse
{
    public class CResponseClass
    {
        public string statusmessage { get; set; }
        public int statusCode { get; set; }

        public static CResponseClass TemplateErrorResponse(TemplateErrorResponseCode error)
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

            return new CResponseClass
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
        INTERNAL_ERROR = 1599,
        INVALID_REQUEST = 3,
        NOT_MATCH = 4
    }
}
