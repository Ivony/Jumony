using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivony.Html.Parser;
using Ivony.Html;
using System.Net;
using System.Web.Security;
using System.IO;
using System.Text;
using Ivony.Fluent;
using Ivony.Html.Web;
using Ivony.Html.Web.Mvc;
using System.Diagnostics;

/// <summary>
/// DomTreeController 的摘要说明
/// </summary>
public class DomTreeController : Controller
{

  [ActionName( "Enter" )]
  public ActionResult EnterUrl()
  {
    return View( "EnterUrl" );
  }

  [ActionName( "Enter" )]
  [HttpPost]
  public ActionResult EnterUrl( string type, string url, HttpPostedFileBase file, string encoding )
  {
    string content;

    if ( type == "Local" )
    {
      if ( file == null )
        return View( "Error", new Exception( "没有选择文件上传" ) );

      using ( var reader = new StreamReader( file.InputStream, Encoding.GetEncoding( encoding ) ) )
      {
        content = reader.ReadToEnd();
      }
    }
    else
    {

      try
      {
        var uri = new Uri( url );
        if ( !uri.Scheme.EqualsIgnoreCase( "http" ) && !uri.Scheme.EqualsIgnoreCase( "https" ) )
          throw new Exception( "只能访问 http 协议资源" );

        var client = new WebClient();
        client.Encoding = Encoding.GetEncoding( encoding );
        content = client.DownloadString( url );
      }
      catch ( WebException e )
      {
        return View( "Error", e );
      }
      catch ( Exception e )
      {
        return View( "Error", e );
      }
    }

    var hash = FormsAuthentication.HashPasswordForStoringInConfigFile( content, "SHA1" );

    System.IO.File.WriteAllText( HttpContext.Request.MapPath( "~/Content/" + hash + ".html" ), content, Encoding.UTF8 );

    return RedirectToAction( "ShowDomTree", new { hash = hash } );

  }


  [Cacheable( typeof( CacheProvider ) )]
  public ActionResult ShowDomTree( string hash, string selector )
  {

    Stopwatch watch = new Stopwatch();
    watch.Start();
    var document = HtmlProviders.LoadDocument( "~/Content/" + hash + ".html" );
    watch.Stop();

    if ( document == null )
      return HttpNotFound();

    ViewData["Timespan"] = watch.Elapsed;

    try
    {
      ViewData["Selector"] = CssParser.ParseSelector( selector );
    }
    catch ( FormatException e )
    {
      return View( "Error", e );
    }

    return View( "DomTree", document );
  }

}