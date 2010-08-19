using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{
  public interface IHtmlNode
  {
    IHtmlContainer Parent
    {
      get;
    }

    object NodeObject
    {
      get;
    }

  }
}
