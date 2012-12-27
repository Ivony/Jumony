using System;
using System.IO;
using Ivony.Html;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SelectorTest
{
  [TestClass]
  public class CssStyleTest
  {
    [TestMethod]
    public void StyleSettingParseTest()
    {

      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "SelectorTest1.html" ) );

      Assert.AreEqual( document.FindSingle( "div" ).Style().GetValue( "display" ), "none", "无法正确解析丢失分号的属性值" );

    }
  }
}
