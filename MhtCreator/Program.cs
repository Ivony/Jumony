using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using Ivony.Html;
using System.Net;
using Ivony.Html.Parser;

namespace MhtCreator
{
  public class Program
  {
    private const string tempDirectory = @"C:\Temp";

    static void Main( string[] args )
    {

      var id = Guid.NewGuid();

      var path = Path.Combine( tempDirectory, id.ToString() );
      Directory.CreateDirectory( path );

      SmtpClient smtp = new SmtpClient();
      smtp.EnableSsl = false;
      smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
      smtp.PickupDirectoryLocation = path;

      var parser = new JumonyParser();
      var document = parser.LoadDocument( "http://blog.sina.com.cn/s/blog_4701280b010183ny.html" );

      MailMessage message = CreateMail( document );

      smtp.Send( message );

      var directory = new DirectoryInfo( path );
      var file = directory.GetFiles().Single();
      file.MoveTo( Path.Combine( tempDirectory, id.ToString() + ".mht" ) );

      directory.Delete( true );

    }



    public static MailMessage CreateMail( IHtmlDocument document )
    {

      MailMessage message = new MailMessage();
      message.From = new MailAddress( "service@mhtgenerator.com" );
      message.To.Add( new MailAddress( "Ivony@live.com" ) );


      var view = CreateView( document );

      message.AlternateViews.Add( view );


      return message;

    }

    public static AlternateView CreateView( IHtmlDocument document )
    {
      var stream = new MemoryStream();
      document.Render( stream, Encoding.UTF8 );

      stream.Seek( 0, SeekOrigin.Begin );


      var resources = GetResources( document );

      var view = new AlternateView( stream, "text/html" );
      view.TransferEncoding = TransferEncoding.Base64;
      view.BaseUri = document.DocumentUri;

      foreach ( var r in resources )
        view.LinkedResources.Add( r );


      return view;
    }

    public static IEnumerable<LinkedResource> GetResources( IHtmlDocument document )
    {
      foreach ( var element in document.Find( "[src]" ) )
      {
        var attribute = element.Attribute( "src" );

        var value = attribute.Value();

        if ( string.IsNullOrWhiteSpace( value ) )
          continue;

        Uri resourceUrl;
        if ( !Uri.TryCreate( document.DocumentUri, value, out resourceUrl ) )
          continue;

        yield return LoadResource( resourceUrl );
      }
    }

    public static LinkedResource LoadResource( Uri resourceUrl )
    {
      using ( var client = new WebClient() )
      {
        var data = client.DownloadData( resourceUrl );
        var stream = new MemoryStream( data );

        var resource = new LinkedResource( stream );
        resource.ContentLink = resourceUrl;

        return resource;
      }
    }

  }


}
