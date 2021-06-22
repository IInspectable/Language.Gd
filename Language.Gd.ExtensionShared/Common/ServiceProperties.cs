#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class ServiceProperties {

        public static TimeSpan ParserServiceThrottleTime         = TimeSpan.FromMilliseconds(200);
        public static TimeSpan OutlineControllerSyncThrottleTime = TimeSpan.FromMilliseconds(200);

    }

}