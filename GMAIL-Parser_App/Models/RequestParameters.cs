﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GMAIL_Parser_App.Models
{
    public class RequestParameters
    {
        public string scope { get; set; }
        public string access_type{ get; set; }
        public string included_granted_scopes{ get; set; }
        public string response_type{ get; set; }
        public string state{ get; set; }
        public string redirect_uri{ get; set; }
        public string client_id { get; set; }
        public string code{ get; set; }
        public string client_secret{ get; set; }
        public string grant_type{ get; set; }
    }
}