using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Interfaces
{
    public interface IEmailSender
    {
        Trawick.Common.Email.EmailResponse SendMail(Trawick.Common.Email.EmailArgs args);
    }


}


namespace Trawick.Common.Email
{
    public class EmailArgs
    {
        public string EmailTo { get; set; }
        public string EmaillCC { get; set; }
        public string EmailBCC { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public Boolean IsHtml { get; set; }
        public Dictionary<string,object> Args { get; set; }

        public int EmailId { get; set; }
        public int MasterEnrollmentId { get; set; }

        public int MemberId { get; set; }

        public bool IsTest { get; set; }
    }

    public class EmailResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }

    }

}
