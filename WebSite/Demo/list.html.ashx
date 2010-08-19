<%@ WebHandler Language="C#" Class="list_html" %>

using System;
using System.Web;
using Ivony.Fluent;
using Ivony.Web.Html;
using Ivony.Web.Html.HtmlAgilityPackAdaptor;
using System.Linq;
using HtmlAgilityPack;

public class list_html : Ivony.Web.Html.HtmlAgilityPackAdaptor.HtmlHandlerAdapter
{

  protected override void Process()
  {
    //这个范例文件主要来讲述列表的绑定
    //暂时我只认可一种数据列表类型，也就是IEnumerable或IEnumerable<T>，不过几乎所有的列表都是从这个继承。

    //Ivony.Fluent命名空间下提供了一个BindTo方法来辅助我们将数据源绑定到某个东西的列表。
    //我们尝试用BindTo在列表1上绑定从1到10的自然数：
    Enumerable.Range( 1, 10 )
      .BindTo( Find( "#list1 li" ), ( dataItem, target ) => target.Bind( "@:text", dataItem ) );
    //BindTo方法会返回数据源支持连写，将一份数据绑定到多个目标


    //下面来看看数据不够的情况：
    Enumerable.Range( 1, 5 )
      .Select( i => i.ToString() )
      .BindTo( Find( "#list2 li" ), null,//null代表缺少数据源时用来替代的默认值
        ( dataItem, target ) => target.Bind( "@:text", dataItem, BindingNullBehavior.Remove )
      //最后一个参数指定如果数据对象是null作何处理，这里指示删除元素。
      );

    //用代码来指定这些东西，会觉得有些别扭，因为BindingNullBehavior主要是为绑定样式表设置而准备的。


    //有时候我们会希望把数据附着到一个元素上，以便后面的绑定使用，这里就要借助HtmlBindingContext，当然，我们也有现成的扩展方法：
    Find( "#list1" ).Single().DataContext( "列表一" );
    Find( "#list2" ).Single().DataContext( "列表二" );


    //使用DataContext方法可以附着任何数据到元素上，然后我们可以在附着了数据的子元素中直接取得这些数据：
    Find( "li" ).ForAll( element =>
      element.Bind( "@onclick", element.DataContext(), "window.alert( '{0}' );" )
    );
    //不带参数的DataContext方法用于获取附着的数据，如果当前元素找不到，则会自动上溯。


  }

}