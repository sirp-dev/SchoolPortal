using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Web.Models.Dtos.Paystack.Data
{
    public class RequestAuthorizationModel
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("reauthorization_url")]
        public string Reauthorization_url { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}
