using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

namespace Trawick.Common.Email
{
    public class EmailFactory
    {
        public static Trawick.Common.Interfaces.IEmailSender GetEmailFactory()
        {
            System.Configuration.Configuration config = null;

            if (System.Web.HttpContext.Current != null)
            {
                config =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            }
            else
            {
                config =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            Trawick.Common.Email.EmailFactorySettings settings = (Trawick.Common.Email.EmailFactorySettings)config.Sections["EmailFactorySettings"];

            return (Trawick.Common.Interfaces.IEmailSender)Activator.CreateInstance(Type.GetType(settings.SystemType));

        }
    }
}
