using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public class BindingManager
  {


    public IHtmlDocument Document
    {
      get;
      private set;
    }

    public BindingManager( IHtmlDocument document )
    {
      Document = document;
    }




  }
}
