<%@ WebHandler Language="C#" Class="SelectorAndBind_html" %>

using System;
using System.Linq;
using System.Web;
using Ivony.Web.Html;
using HtmlAgilityPack;
using Ivony.Fluent;


//这个示例文件简单的描述了CSS选择器支持和数据绑定支持。
public class SelectorAndBind_html : Ivony.Web.Html.HtmlAgilityPackAdaptor.HtmlHandlerAdapter
{
  //ashx文件除了扩展名部分的ashx，其余的要与模板html文件保持完全一致，这样请求就会自动转发到ashx文件。

  protected override void Process()
  {
    //基本的选择器，都是完全支持的。
    Find( "html title" ).Bind( "@:text", "测试页面" );
    Find( "div.title" ).Bind( "@:text", "测试标题" );
    Find( ".content" ).Bind( "@:text", "测试内容" );

    //稍微解释一下Bind扩展方法，Bind方法的第一个参数是绑定路径，你可以用@attributeName来对属性执行绑定。
    //以:开头的是伪属性:text表示绑定到元素的innerText上，:html则表示绑定到元素的innerHTML上。

    Find( "ul li:empty" )
      .Bind( "@:text", 123 );

    //">"表示直接子代匹配，只有li直属的div才会被匹配，请注意CSS选择器现在有严格的格式规范，在关系运算符之间必须留有空白，即li>div不是合法的CSS选择器。
    Find( "li > div[style]" ).Bind( "@:text", "style" );//仅当style存在时匹配
    Find( "li > div[style!=]" ).Bind( "@:text", "style!=" );//仅当style不等于空字符串时匹配，注意如果style不存在，也会匹配。
    Find( "li > div[style='a]]']" ).Bind( "@:text", "style=a]]" );//选择器的正则分析能够很好的处理这种情况
    //Find( "li > div[abc='a]]']]" ).Bind( "@:text", "123" );//选择器的正则分析会认为这是错误的格式
    Find( "li > div[style^=font][class]" ).Bind( "@:text", "style^=font" );//当属性style以font开头，且存在class属性时匹配


    //现在支持的CSS选择器包括：*、E、E E、E + E、E > E、E ~ E、#identity、.class-name
    //[attr]、[attr=value]、[attr!=value]、[attr^=value]、[attr$=value]、[attr*=value]、[attr~=value]
    //暂不支持的选择器包括：所有的伪类选择器、伪对象选择器、[attr|=value]、E , E


    //Bind方法会延迟执行，如果去掉下面这行注释，看看会发生什么：
    //HtmlBindingContext.Current.Discard();


    //Bind方法延迟执行的原因在于，在执行Bind时，有可能会对文档对象树造成破坏性修改（例如设置InnerHTML或是移除节点），而枚举器不能在遍历的时候修改集合。
    //Bind提供了一个简单的方式让你可以延迟安全的执行你想要做的操作，如果你需要延迟安全的执行自定义的操作，可以用BindAction扩展方法，例如下面这行代码删掉我们自己加在页面上的诡异元素<special>
    Find( "special" ).ForAll( element => element.BindAction( e => e.NodeObject.Cast<HtmlNode>().Remove() ) );
    //如果直接执行下面的代码，则会报错：
    //Find( "special" ).ForAll( element => element.NodeObject.Cast<HtmlNode>().Remove() );




  }
}