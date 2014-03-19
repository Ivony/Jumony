using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using Ivony.Html;
using Ivony.Html.Parser;
using Ivony.Html.Forms;
using System.IO;

namespace FormsTest
{
  [TestClass]
  public class FormsTest
  {
    [TestMethod]
    public void FormTest1()
    {
      var document = LoadDocument( "FormTest1.html" );


      Exception e = null;

      try
      {

        document.FindFirst( "form" ).AsForm();

      }
      catch ( Exception exception )
      {
        e = exception;
      }


      Assert.IsNotNull( e, "表单中存在重复的文本输入框未能引发异常" );
      Assert.IsInstanceOfType( e, typeof( InvalidOperationException ), "表单中存在重复的文本输入框未能引发正确的异常" );


    }

    [TestMethod]
    public void TextControlTest()
    {

      var document = LoadDocument( "TextControlTest.html" );

      var form = document.FindFirst( "form" ).AsForm();

      Assert.IsTrue( form.Controls.Contains( "textbox1" ), "未能找到 textbox1 控件" );
      Assert.IsTrue( form.Controls.Contains( "textbox2" ), "未能找到 textbox2 控件" );

      Assert.AreEqual( form.Controls["textbox1"].Value, "Test1", "获取 textbox1 控件值失败" );
      Assert.AreEqual( form.Controls["textbox2"].Value, "Test2", "获取 textbox2 控件值失败" );

    }


    private static IHtmlDocument LoadDocument( string filename )
    {
      return new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, filename ) );
    }



  }
}
