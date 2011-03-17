using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  public class JumonyPartialView : JumonyViewBase
  {

    protected IHtmlContainer ViewContainer
    {
      get;
      private set;
    }

    public void Initialize( string virtualPath, IHtmlDocument document )
    {
      ViewContainer = document.FindSingle( "body" );
    }
    
    protected override void ProcessMain()
    {
      throw new NotImplementedException();
    }

    protected override string RenderContent()
    {
      throw new NotImplementedException();
    }
  }
}
