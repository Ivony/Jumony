using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Compilation;

public partial class Test : System.Web.UI.Page
{
  protected void Page_Load( object sender, EventArgs e )
  {
     BuildManager.CreateInstanceFromVirtualPath( "~/Test.aspx.cs", typeof( Page ) );
  }
}