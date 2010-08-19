using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Web.Html
{
  public abstract class HtmlHandler : IHttpHandler
  {
    #region IHttpHandler 成员

    public abstract bool IsReusable
    {
      get;
    }

    public abstract void ProcessRequest();

    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      Context = context;
      OriginUrl = Context.Items["HtmlRewriteModule_OriginUrl"] as Uri;

      if ( OriginUrl == null )
      {
        var builder = new UriBuilder( Request.Url );
        var path = builder.Path;

        if ( !path.EndsWith( ".ashx" ) )
          throw new InvalidOperationException();

        builder.Path = path.Remove( path.Length - 5 );

        Response.Redirect( builder.Uri.AbsoluteUri );
      }

      ProcessRequest();
    }

    protected string GetTemplateContent()
    {
      var physicalPath = GetTemplateFilePath();
      if ( !File.Exists( physicalPath ) )
        throw new HttpException( 404, "未找到模板文件，这可能是直接访问.html.ashx文件所造成的。" );


      var cacheKey = string.Format( "HtmlHandler_TemplateContentCache_{0}", physicalPath );
      var templateContent = Context.Cache[cacheKey] as string;

      if ( templateContent == null )
      {
        using ( var reader = File.OpenText( physicalPath ) )
        {
          templateContent = reader.ReadToEnd();
        }
      }

      return templateContent;
    }

    private static string GetTemplateFilePath()
    {
      return Path.ChangeExtension( HttpContext.Current.Request.PhysicalPath, "" );
    }

    protected HttpContext Context
    {
      get;
      private set;
    }

    protected Uri OriginUrl
    {
      get;
      private set;
    }


    public HttpRequest Request
    {
      get { return Context.Request; }
    }

    public HttpResponse Response
    {
      get { return Context.Response; }
    }

    public HttpServerUtility Server
    {
      get { return Context.Server; }
    }

    public HttpApplicationState Application
    {
      get { return Context.Application; }
    }

    public HttpApplication ApplicationInstance
    {
      get { return Context.ApplicationInstance; }
    }



    #endregion

    #region IDisposable 成员

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
