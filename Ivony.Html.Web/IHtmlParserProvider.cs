using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public interface IHtmlParserProvider
  {

    HtmlParserResult GetParser( HttpContextBase context, Uri contentUri, string htmlContent );


    void ReleaseParser( IHtmlParser parser );

  }


  public class HtmlParserResult
  {

    public IHtmlParser Parser
    {
      get;
      set;
    }

    public IHtmlDomProvider DomProvider
    {
      get;
      set;
    }

    public IHtmlParserProvider Provider
    {
      get;
      internal set;
    }



  }


}
