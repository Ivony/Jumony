using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Ivony.Html.Parser;
using Ivony.Html.Binding;
using Ivony.Html;
using System.Linq;

namespace BindingTest
{
  [TestClass]
  public class StyleBinderTest
  {
    [TestMethod]
    public void VisibleTest()
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "StyleTest1.html" ) );
      document.DataBind( null );

      Assert.AreEqual( document.Find( ".invisible" ).Count(), 0 );


    }
  }
}
