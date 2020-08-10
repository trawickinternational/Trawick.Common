using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace Trawick.Common.Interfaces
{
  public  interface ITrawickAction
    {
        List<TrawickActionResult> RunAction();
        DateTime NextRunTime { get; }

    }
}

public enum TrawickActionResultType
{
    Message,
    Warning,
    Error,
    Fatal
}

public class TrawickActionResult
{
    public TrawickActionResultType Type { get; set; }
    public string Message { get; set; }
}

namespace Trawick.Common.Configuration
{
    public class TrawickActionsSection : ConfigurationSection
    {
        private TrawickActionsSection() { }

        [ConfigurationProperty("Name", DefaultValue = "")]
        public string Name
        {
            get { return (String)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("SystemType", DefaultValue = "")]
        public string SystemType
        {
            get { return (string)this["SystemType"]; }
            set { this["SystemType"] = value; }
        }



    }


    
}
