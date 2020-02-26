using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Trawick.Common.Extensions
{
    public static class HttpBaseExtensions
    {

        public static HttpContextBase AsBase(this HttpContext context)
        {
            return new HttpContextWrapper(context);
        }

        public static HttpRequestBase AsBase(this HttpRequest request)
        {
            return new HttpRequestWrapper(request);
        }

    }
}
