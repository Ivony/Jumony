using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace Ivony.Html.Web
{
  public class RawResponse : ICachedResponse
  {

    public NameValueCollection Headers
    {
      get;
      set;
    }

    public string Content
    {
      get;
      set;
    }

    public Encoding Encoding
    {
      get;
      set;
    }



    public virtual void Apply( HttpResponseBase response )
    {
      response.Clear();


    }

  }
}
