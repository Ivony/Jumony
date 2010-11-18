using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web;

namespace Ivony.Html.Web
{


  public class MapInfo
  {

    public MapInfo( string rewritePath, IHtmlParser parser, string templatePath )
    {
      _parser = parser;
      _templatePath = templatePath;
    }


    private IHtmlParser _parser;
    private string _templatePath;



    public Uri OriginUrl
    {
      get;
      internal set;
    }


    public IRequestMapper Mapper
    {
      get;
      internal set;
    }


    protected virtual IHtmlParser Parser
    {
      get { return _parser; }
    }

    protected virtual string TemplatePath
    {
      get { return _templatePath; }
    }




    public virtual IHtmlDocument LoadTemplate()
    {
      return ParseDocument( LoadTemplateContent( _templatePath ) );
    }

    protected virtual IHtmlDocument ParseDocument( string content )
    {
      return _parser.Parse( content );
    }



    protected HttpContext Context
    {
      get { return HttpContext.Current; }
    }


    private static readonly string[] executionExtensions = new[] { ".aspx" };

    protected virtual string LoadTemplateContent( string path )
    {
      if ( !File.Exists( _templatePath ) )
      {
        var exception = new HttpException( 404, "未找到模板文件。" );

        Context.Trace.Warn( "Core", "template not found!", exception );
        throw exception;
      }

      var extension = Path.GetExtension( _templatePath );
      if ( executionExtensions.Contains( extension, StringComparer.InvariantCultureIgnoreCase ) )
      {
        var writer = new StringWriter();
        Context.Server.Execute( _templatePath, writer );
        return writer.ToString();
      }



      var cacheKey = string.Format( "HtmlHandler_TemplateContentCache_{0}", path );
      var templateContent = Context.Cache[cacheKey] as string;

      if ( templateContent == null )
      {
        Context.Trace.Warn( "Core", "template file cache miss." );
        Context.Trace.Write( "Core", "Begin Load Template" );
        using ( var reader = File.OpenText( path ) )
        {
          templateContent = reader.ReadToEnd();
        }

        Context.Cache.Insert( cacheKey, templateContent, new System.Web.Caching.CacheDependency( path ) );

        Context.Trace.Write( "Core", "End Load Template" );
      }

      return templateContent;
    }


    public IHtmlHandler Handler
    {
      get;
      private set;
    }

  }
}
