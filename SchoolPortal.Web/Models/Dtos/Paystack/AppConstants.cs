using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Web.Models.Dtos.Paystack
{
    public class AppConstants
    {
        public const string PayStackBaseEndPoint = "https://api.paystack.co/";

        public const string PaystackInitializeTransactionEndPoint = "transaction/initialize";
        public const string PaystackTransactionVerificationEndPoint = "transaction/verify";

        /// <summary>
        /// Application - Json Content Type
        /// </summary>
        public const string ContentTypeHeaderJson = "application/json";


        /// <summary>
        /// Authorization HTTP Header
        /// </summary>
        public const string AuthorizationHeaderType = "Bearer";
    }
}
