using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;
using System.IO;
using System.Diagnostics;
using Ivony.Html.Web;

namespace Ivony.Html.Binding
{
  public class HtmlBindingModule : IHttpModule
  {



    private const string inited_token = "Jumony_Binding_EnvironmentExpression_Inited";

    #region IHttpModule 成员

    public void Dispose()
    {
    }


    private static readonly object _sync = new object();

    public void Init( HttpApplication context )
    {
      context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;


      lock ( _sync )
      {
        if ( context.Application[inited_token] == null )
        {
          EnvironmentExpressions.RegisterProvider( "Application", name => HttpContext.Current.Application[name] );
          EnvironmentExpressions.RegisterProvider( "Session", name => HttpContext.Current.Session[name] );
          EnvironmentExpressions.RegisterProvider( "Get", name => HttpContext.Current.Request.QueryString[name] );
          EnvironmentExpressions.RegisterProvider( "Post", name => HttpContext.Current.Request.Form[name] );
          EnvironmentExpressions.RegisterProvider( "Server", name => HttpContext.Current.Request.ServerVariables[name] );
          EnvironmentExpressions.RegisterProvider( "Context", name => HttpContext.Current.Items[name] );

          EnvironmentExpressions.RegisterProvider( new CookiesProvider() );

          context.Application[inited_token] = new object();
        }
      }
    }

    private class CookiesProvider : IEnvironmentVariableProvider
    {
      public string Name { get { return "Cookies"; } }
      public object EvaluateExpression( string expression )
      {
        var cookie = HttpContext.Current.Request.Cookies[expression];
        if ( cookie != null )
          return cookie.Value;
        else
          return null;
      }
    }


    public JumonyHandler Handler
    {
      get { return HttpContext.Current.CurrentHandler as JumonyHandler; }
    }


    private void OnPreRequestHandlerExecute( object sender, EventArgs e )
    {
      if ( Handler != null )
      {
        Handler.PreProcessDocument += OnPreProcessDocument;
        Handler.PostProcessDocument += OnPostProcessDocument;
      }
    }

    private BindingContext globalBinding;

    private void OnPreProcessDocument( object sender, EventArgs e )
    {
      ProcessBindingSheets( Handler );

      globalBinding = BindingContext.Enter( Handler.Document, "Global" );
    }

    private void OnPostProcessDocument( object sender, EventArgs e )
    {
      globalBinding.Exit();
    }

    private void ProcessBindingSheets( JumonyHandler handler )
    {
      var bindingSheets = handler.Document.Find( "link[rel=Bindingsheet]" )
        .Select( link => link.Attribute( "href" ).Value() )
        .Where( href => !string.IsNullOrEmpty( href ) )
        .Select( href => LoadBindingSheet( handler.Context.Request.MapPath( href ) ) )
        .Where( sheet => sheet != null );


      using ( var bindingContext = BindingContext.Enter( handler.Document, "ApplyBindingSheet" ) )
      {
        bindingSheets
          .ForAll( sheet => sheet.Apply( bindingContext ) );

        bindingContext.Exit();
      }
    }

    /// <summary>
    /// 从指定文件加载样式表
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    private static IHtmlBindingSheet LoadBindingSheet( string physicalPath )
    {


      if ( !File.Exists( physicalPath ) )
      {
        Trace.TraceWarning( "Jumony Web", string.Format( "在 \"{0}\" 找不到样式表文件", physicalPath ) );
        return null;
      }

      return HtmlBindingSheet.Load( physicalPath );
    }

    #endregion
  }
}
