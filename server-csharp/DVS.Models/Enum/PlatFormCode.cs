using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Enum
{
    public enum PlatFormCode
    {
        UNKNOWN = 0,
        CONSOLE = 1,
        APP = 2,
        WECHAT = 3
    }

    public class PlatFormName {
        public const string Console= "Console";
        public const string App= "App";
    }
}
