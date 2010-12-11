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

    public IHtmlHandler Handler
    {
      get;
      private set;
    }




    public virtual IHtmlDocument LoadTemplate()
    {
      var document = HtmlProviders.LoadDocument( new HttpContextWrapper( HttpContext.Current ), TemplatePath );

      if ( document == null )
        throw new InvalidOperationException();

      return document;

    }

  }
}
