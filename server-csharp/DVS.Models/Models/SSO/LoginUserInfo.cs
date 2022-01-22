using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Models.SSO
{
    public class LoginUserInfo
    {
        public string Id { get; set; }
        public string Account { get; set; }

        public int TokenValidSeconds { get; set; }
    }
}
