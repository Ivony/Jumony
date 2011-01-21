using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Ivony.Html.Web
{
  public class HtmlMetaCollection : NameValueCollection
  {

    public HtmlHead Head
    {
      get;
      private set;
    }

    public HtmlMetaCollection( HtmlHead head )
    {
      Head = head;
    }

  }
}
