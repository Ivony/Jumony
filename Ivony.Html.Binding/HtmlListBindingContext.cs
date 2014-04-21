using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;
using Ivony.Html.ExpandedAPI;
using Ivony.Html;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 进行列表绑定的绑定上下文
  /// </summary>
  internal sealed class HtmlListBindingContext : HtmlBindingContext
  {

    internal HtmlListBindingContext( HtmlBindingContext context, IHtmlElement element, ListDataModel dataModel )
      : base( context, element, dataModel )
    {
      DataModel = dataModel;
    }


    private IHtmlElement _element;



    public ListDataModel DataModel
    {
      get;
      private set;
    }


    /// <summary>
    /// 重写 DataBind 方法，执行绑定时将被绑定元素复制适当份数，再进行绑定。
    /// </summary>
    public override void DataBind()
    {

      if ( DataModel.Selector == null )
      {
        var count = DataModel.ListData.Length;
        var list = _element.Repeat( count );

        for ( int i = 0; i < count; i++ )
          DataBind( list[i], DataModel.ListData[i] );

        return;
      }


      if ( DataModel.BindingMode == ListBindingMode.DynamicContent )
        DynamicContent();



      int index = 0;

      foreach ( var element in BindingScope.Elements() )
      {
        if ( DataModel.Selector.IsEligible( element ) )
          DataBind( element, DataModel.ListData[index++] );
      }




    }


    private void DynamicContent()
    {
      var dataLength = DataModel.ListData.Length;

      var items = BindingScope.Elements().FilterBy( DataModel.Selector ).ToArray();

      if ( items.Length == 1 )
        items.Single().Repeat( dataLength );

      else if ( dataLength != items.Length )
      {
        if ( dataLength < items.Length )//如果数据项少于绑定项
        {

          var tail = items.Last().NextNode();//确定尾部


          var lastItem = items.ElementAt( dataLength );

          while ( lastItem.NextNode() != tail )//将最后一个元素到尾部之间的所有元素清除。
            lastItem.NextNode().Remove();

        }

        else
        { 


        
        }

      }
    }

    private void DataBind( IHtmlElement element, object dataModel )
    {
      CreateBindingContext( ParentContext, element, dataModel ).DataBind();
    }

  }
}
