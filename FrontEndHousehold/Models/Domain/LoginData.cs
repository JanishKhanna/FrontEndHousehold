using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontEndHousehold.Models.Domain
{
    public class LoginData
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}