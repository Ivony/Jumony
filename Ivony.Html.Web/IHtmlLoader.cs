using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace Ivony.Html.Web
{

  public interface IHtmlLoader
  {

    string Load( HttpContextBase context, string virtualPath );

  }



  public class StaticFileLoader : IHtmlLoader
  {

    private static readonly string[] allowsExtensions = new[] { ".html", ".htm" };

    public string Load( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      var file = HostingEnvironment.VirtualPathProvider.GetFile( virtualPath );

      using ( var reader = new StreamReader( file.Open(), true ) )
      {
        return reader.ReadToEnd();
      }

    }
  }


  public class AspxFileLoader : IHtmlLoader
  {

    private static readonly string[] allowsExtensions = new[] { ".aspx" };

    public string Load( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      using ( var writer = new StringWriter() )
      {
        context.Server.Execute( virtualPath, writer, false );

        return writer.ToString();
      }
    }
  }

}
