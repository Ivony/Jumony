using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// IRequestMapper 产生的映射结果
  /// </summary>
  public class RequestMapResult
  {

    public RequestMapResult( string templatePath, IHtmlHandler handler )
    {

      if ( !VirtualPathUtility.IsAppRelative( templatePath ) )
        throw new ArgumentException( "模版文件路径必须是相对于应用程序根路径", "templatePath" );

      TemplatePath = templatePath;
      Handler = handler;
    }


    public IRequestMapper Mapper
    {
      get;
      internal set;
    }

    protected virtual string TemplatePath
    {
      get;
      private set;
    }


    private bool _templateLoaded;
    private string _templateCacheKey;

    public virtual string TemplateCacheKey
    {
      get
      {
        if ( _templateLoaded )
          return _templateCacheKey;

        else
          throw new InvalidOperationException( "模版尚未加载" );

      }
    }

    public virtual IHtmlDocument LoadDocument()
    {
      var document = LoadDocument( out _templateCacheKey );

      _templateLoaded = true;
      return document;
    }

    protected virtual IHtmlDocument LoadDocument( out string cacheKey )
    {
      var document = HtmlProviders.LoadDocument( new HttpContextWrapper( HttpContext.Current ), TemplatePath, out cacheKey );
      return document;
    }

    public IHtmlHandler Handler
    {
      get;
      private set;
    }


    public virtual WebPage LoadPage()
    {
      var page = HtmlProviders.LoadPage( new HttpContextWrapper( HttpContext.Current ), TemplatePath );

      return page;
    }


  }
}
