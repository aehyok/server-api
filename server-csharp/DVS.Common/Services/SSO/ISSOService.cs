using DVS.Models.Models.SSO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Common.Services.SSO
{
   public  interface ISSOService 
    {
        public Task<T> GetSSOInfoByToken<T>(string token) where T : LoginUserInfo;
    }
}
