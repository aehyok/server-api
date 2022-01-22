using DVS.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Common.Infrastructures
{
    public interface IDVSController
    {
        public LoginUser LoginUser { get;  }
        public void SetLoginUser(LoginUser loginUser);
    }
}
