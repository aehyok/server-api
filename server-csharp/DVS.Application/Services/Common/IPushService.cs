using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public interface IPushService
    {
        Task<bool> PushMessage(PushMessageDto message);
    }
}
