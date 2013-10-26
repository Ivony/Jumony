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
using System.Diagnostics;

/// <summary>
/// DomTreeController 的摘要说明
/// </summary>
public class DomTreeController : Controller
{

  [Cacheable( typeof( CacheProvider ) )]
  public ActionResult Default()
  {
    return View( "Layout" );
  }


  [ChildActionOnly]
  public ActionResult ChooseDocument( string hash )
  {
    var infoFilePath = HttpContext.Request.MapPath( "~/Content/" + hash + ".info" );
    if ( System.IO.File.Exists( infoFilePath ) )
    {
      var info = System.IO.File.ReadAllLines( infoFilePath );
      if ( info[0] == "LocalFile" )
        ViewData["Type"] = "Local";

      else
      {
        ViewData["Type"] = "Internet";
        ViewData["Url"] = info[0];
      }

      ViewData["Encoding"] = info[1];

    }

    return PartialView( "ChooseDocument" );
  }


  [HttpPost]
  [ActionName( "Default" )]
  public ActionResult ChooseDocument( string type, string url, HttpPostedFileBase file, string encoding )
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

      var uri = new Uri( url );
      if ( !uri.Scheme.EqualsIgnoreCase( "http" ) && !uri.Scheme.EqualsIgnoreCase( "https" ) )
        throw new Exception( "只能访问 http 协议资源" );

      var client = new WebClient();
      client.Encoding = Encoding.GetEncoding( encoding );
      content = client.DownloadString( url );
    }

    var hash = FormsAuthentication.HashPasswordForStoringInConfigFile( content, "SHA1" );



    var documentPath = HttpContext.Request.MapPath( "~/Content/" + hash + ".html" );
    var infoFilePath = HttpContext.Request.MapPath( "~/Content/" + hash + ".info" );

    Directory.CreateDirectory( Path.GetDirectoryName( documentPath ) );


    System.IO.File.WriteAllText( documentPath, content, Encoding.UTF8 );

    var info = new StringWriter();

    if ( type == "Local" )
      info.WriteLine( "Local" );
    else
      info.WriteLine( url );

    info.WriteLine( encoding );

    System.IO.File.WriteAllText( infoFilePath, info.ToString(), Encoding.UTF8 );

    return RedirectToAction( "Default", new { hash = hash } );
  }


  [ChildActionOnly]
  public ActionResult PageViewer( string hash, string selector )
  {
    if ( hash == null )
      return null;


    Stopwatch watch = new Stopwatch();
    watch.Start();
    var document = HtmlHelper.LoadDocument( "~/Content/" + hash + ".html" );
    watch.Stop();

    if ( document == null )
      return HttpNotFound();


    ViewData["Document"] = document;
    ViewData["Timespan"] = watch.Elapsed;

    ViewData["SelectorExpression"] = selector;

    var _selector = CssParser.ParseSelector( selector );
    if ( _selector != null )
    {
      ViewData["Selector"] = _selector;
      ViewData["SelectedElements"] = _selector.Filter( document.Descendants() ).Count();
    }

    return PartialView( "PageViewer" );
  }



  protected override void OnException( ExceptionContext filterContext )
  {
    if ( filterContext.IsChildAction )
      return;// base.OnException( filterContext );

    filterContext.Result = View( "Error", filterContext.Exception );
    filterContext.ExceptionHandled = true;
  }

}