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

      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "CssStyleSettingTest1.html" ) );

      Assert.AreEqual( document.FindFirst( "div" ).Style().GetValue( "display" ), "none", "无法正确解析丢失分号的属性值" );
      Assert.AreEqual( document.FindFirst( "a" ).Style().GetValue( "display" ), null, "无法正确解析没有style属性的元素" );
      Assert.AreEqual( document.FindFirst( "p" ).Style().GetValue( "width" ), "12px", "无法正确解析空白样式设置值后随的正确表达式" );
      Assert.AreEqual( document.FindFirst( "p" ).Style().GetValue( "display" ), null, "无法正确解析空白样式设置值" );



    }
  }
}
