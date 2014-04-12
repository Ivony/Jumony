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
      var document = LoadDocument( "FormsTest1.html" );


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

      form.Controls["textbox1"].Value = "Test";
      form.Controls["textbox2"].Value = "Test";

      Assert.AreEqual( form.Controls["textbox1"].Value, "Test", "设置 textbox1 控件值失败" );
      Assert.AreEqual( form.Controls["textbox2"].Value, "Test", "设置 textbox2 控件值失败" );

      Assert.AreEqual( form.Element.FindFirst( "[name=textbox1]" ).Attribute( "value" ).Value(), "Test", "设置 textbox1 控件值失败" );
      Assert.AreEqual( form.Element.FindFirst( "[name=textbox2]" ).InnerText(), "Test", "设置 textbox2 控件值失败" );


      var value = "Text\nText";

      Assert.IsFalse( form.Controls["textbox1"].CanSetValue( value ), "尝试设置多行文本值成功，这是错误的" );

      try
      {
        form.Controls["textbox1"].Value = value;
      }
      catch ( Exception e )
      {
        Assert.IsInstanceOfType( e, typeof( FormatException ), "尝试设置多行文本值引发了错误的异常" );

        return;
      }

      Assert.Fail( "尝试设置多行文本值未能引发异常" );
    }


    private static IHtmlDocument LoadDocument( string filename )
    {
      return new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, filename ) );
    }



    [TestMethod]
    public void ValidationTest()
    {
      var document = LoadDocument( "FormValidationTest.html" );

      var form = document.FindFirst( "form" ).AsForm();

      var presenter = new FormPresenter();

      var result = new FormValidationResult( form, new[] { new FormValidationError( "FirstName", "First Name is required!" ) } );

      presenter.ShowValidationResult( result );

      Assert.AreEqual( form.Element.FindFirst( "#error_FirstName ul li" ).InnerText(), "First Name is required!" );

    }


  }
}
