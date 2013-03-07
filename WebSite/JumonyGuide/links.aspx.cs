using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;


public partial class links : System.Web.UI.Page
{
  protected void Page_Load( object sender, EventArgs e )
  {

    var client = new WebClient();
    var html = client.DownloadString( "http://www.cnblogs.com/" );

    var parser = new JumonyParser();
    var document = parser.Parse( html );

    var links = document.Find( "a[href]" );

    var baseUrl = new Uri( "http://www.cnblogs.com" );

    var data = from hyperLink in links
               let url = new Uri( baseUrl, hyperLink.Attribute( "href" ).Value() )
               orderby url.AbsoluteUri
               select new
               {
                 Url = url.AbsoluteUri,
                 IsLinkingOut = !url.Host.EndsWith( "cnblogs.com" ),
                 Target = hyperLink.Attribute( "target" ).Value() ?? "_self"
               };

    DataList.DataSource = data;
    DataBind();

  }
}