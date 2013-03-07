using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Html;
using Ivony.Html.Parser;


namespace SelectorTest
{
  [TestClass]
  public class CssClassSelectorTest
  {
    [TestMethod]
    [TestCategory( "选择器" )]
    public void css_class_has_underline()
    {
      var html = "<div class=\"css_class\"></div>";
      var htmlParser = new JumonyParser();
      var doc = htmlParser.Parse( html );

      var css_class = doc.Find( ".css_class" );

      Assert.AreEqual( 1, css_class.Count() );
    }

    [TestMethod]
    [TestCategory( "选择器" )]
    public void css_class_has_hyphen()
    {
      var html = "<div class=\"css-class\"></div>";
      var htmlParser = new JumonyParser();
      var doc = htmlParser.Parse( html );

      var css_class = doc.Find( ".css-class" );

      Assert.AreEqual( 1, css_class.Count() );
    }
  }
}
