using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{
  public interface IHtmlAttribute
  {

    IHtmlElement Element
    {
      get;
    }

    string Name
    {
      get;
    }

    string Value
    {
      get;
      set;
    }

  }
}
