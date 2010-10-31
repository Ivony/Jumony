using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;
using System.IO;

namespace Ivony.Html.Binding
{
  public class HtmlBindingModule : IHttpModule
  {



    #region IHttpModule 成员

    public void Dispose()
    {
    }

    public void Init( HttpApplication context )
    {
      context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
    }


    public HtmlHandler Handler
    {
      get { return HttpContext.Current.CurrentHandler as HtmlHandler; }
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

    private void ProcessBindingSheets( HtmlHandler handler )
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
        HttpContext.Current.Trace.Warn( "Core", string.Format( "在 \"{0}\" 找不到样式表文件", physicalPath ) );
        return null;
      }

      return HtmlBindingSheet.Load( physicalPath );
    }




    #endregion
  }
}
