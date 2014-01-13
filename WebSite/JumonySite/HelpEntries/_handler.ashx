<%@ WebHandler Language="C#" Class="_handler" %>

using System;
using System.Web;
using System.Web.Mvc;
using Ivony.Html;
using Ivony.Fluent;
using Ivony.Html.Web;
using Ivony.Web;
using System.Text.RegularExpressions;

public class _handler : IHtmlHandler, IHttpHandler
{


  public void Dispose()
  {

  }

  public bool IsReusable
  {
    get { return false; }
  }

  public void ProcessRequest( HttpContext context )
  {
    throw new HttpException( 404, "不能直接访问" );
  }

  public void ProcessScope( HtmlRequestContext context )
  {
  }


  public void ProcessCode( IHtmlElement element )
  {
    var code = element.InnerText();
    var language = element.Attribute( "language" ).Value();
    switch ( language )
    {
      default:
        code = ProcessCSharpCode( code );
        break;
    }

    element.ClearNodes();
    element.AddFragment( code );
  }


  private string ProcessCSharpCode( string code )
  {
    code = ProcessCSharpKeywords( code );
    code = ProcessCSharpTypeNames( code );
    return code;
  }


  private Regex keywords = new Regex( @"\b(string|int|void|class|public|protected|private|bool)\b" );

  private string ProcessCSharpKeywords( string code )
  {
    return keywords.Replace( code, match => "<span class='keyword'>" + match.Value + "</span>" );
  }

  private Regex typeNames = new Regex( @"\b(IHtmlElement|IHtmlContainer|IHtmlTextNode|IHtmlComment|IHtmlSpecial|IHtmlDocument|IHtmlAttribute|IEnumerable|IHtmlNode|IHtmlFragment|IHtmlDomProvider|CodeMemberMethod|Func)\b" );

  private string ProcessCSharpTypeNames( string code )
  {
    return typeNames.Replace( code, match => "<span class='typeName'>" + match.Value + "</span>" );
  }
}