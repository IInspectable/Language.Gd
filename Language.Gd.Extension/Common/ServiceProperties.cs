#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class ServiceProperties {

        public static TimeSpan ParserServiceThrottleTime        = TimeSpan.FromMilliseconds(200);
        public static TimeSpan SemanticModelServiceThrottleTime = TimeSpan.FromMilliseconds(200);
        public static TimeSpan BraceMatchingThrottleTime        = TimeSpan.FromMilliseconds(500);
        public static TimeSpan ReferenceHighlighting            = TimeSpan.FromMilliseconds(500);
        public static TimeSpan GoToNavTaggerThrottleTime        = TimeSpan.FromMilliseconds(500);

    }

}