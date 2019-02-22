using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Trawick.Common.ApiRequest
{
    public class ApiRequestFactorySection : ConfigurationSection
    {
        private ApiRequestFactorySection() { }

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
