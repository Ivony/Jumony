<%@ WebHandler Language="C#" Class="Beginning" %>

using System;
using System.Web;
using Ivony.Fluent;
using Ivony.Web.Html;
using Ivony.Web.Html.HtmlAgilityPackAdaptor;

public class Beginning : HtmlHandlerAdapter
{
  
  //是的，这里是全部的真相。
  
  //Jumony是一个基于HTML文档分析的引擎，当进入到Process方法的时候，引擎和框架已经将HTML模板文件加载分析完毕
  //只需要一些简单的Bind方法，您就可以随意的修改HTML文档了，您的这些修改，都会在应用到模板，并输出。
  protected override void Process()
  {
    //在这里，我们通过CSS选择器捕获到了class里面包含title的元素，并将其文本设置成了您看到的标题。同时我们还顺便设置了字体。
    Find( ".title" ).Bind( "@:text", "Jumony 引擎，从这里开始" ).Bind( "@style", "font-family: 黑体;" );

    //这便是提示您查看HTML原文件的文本，他们是在这里绑定上去的。
    Find( "p#preface" ).Bind( "@:text", "在开始认识这个神奇的框架之前，请您先用VS或者其他编辑器打开您所看到的这个HTML文档的物理文件。即解决方案中的htm文件，现在请不要在浏览器中打开。" );

    //那么您在HTML原文件里面看到的文字，则是在这里被删除的。
    Find( "p#invisible" ).Bind( "@:html", null, BindingNullBehavior.Remove );
    
    //如果您想了解更多关于选择器和Bind方法的使用，可以打开SelectorAndBind.htm文件继续Jumony的探索旅程。
    //记住，Jumony引擎的模板必须是html文件，而同名的ashx文件，就是模板的处理程序，这个映射关系是由HtmlRewriteModule自动维护的。
    //任何时候都不应该直接访问ashx文件。
    
    //对于这个引擎而言，我们始终觉得，用它，是最好的学习方法。

  }
}