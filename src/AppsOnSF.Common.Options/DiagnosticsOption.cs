using System;
using System.Collections.Generic;
using System.Text;

namespace AppsOnSF.Common.Options
{
    public class DiagnosticsOption
    {
        public string ElasticsearchHost { get; set; }

        public string EventSourceLevel { get; set; }

        public Serilog.Events.LogEventLevel SerilogEventLevel { get; set; }
    }
}
