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
      BindingElement = element;
    }


    public IHtmlElement BindingElement { get; private set; }


    public ListDataModel DataModel { get; private set; }


    /// <summary>
    /// 重写 DataBind 方法，执行绑定时将被绑定元素复制适当份数，再进行绑定。
    /// </summary>
    public override void DataBind()
    {

      if ( DataModel.Selector == null )
      {
        var count = DataModel.ListData.Length;
        var list = BindingElement.Repeat( count );

        for ( int i = 0; i < count; i++ )
          DataBind( list[i], DataModel.ListData[i] );

        return;
      }




      IHtmlElement[] elements;
      if ( DataModel.BindingMode == ListBindingMode.DynamicContent )
        elements = DynamicContent();
      
      else
        elements = BindingScope.Elements().FilterBy( DataModel.Selector ).ToArray();



      int index = 0;

      foreach ( var e in elements )
      {
        DataBind( e, DataModel.ListData[index++] );
      }




    }


    private IHtmlElement[] DynamicContent()
    {

      var container = BindingScope;

      var dataLength = DataModel.ListData.Length;

      var items = container.Elements().FilterBy( DataModel.Selector ).ToArray();//先找出所有目标元素

      if ( items.Length == 1 )//若目标元素只有一个，则退化到简单复制模式。
        return items.Single().Repeat( dataLength );


      var tail = items.Last().NextNode();//确定尾部


      if ( dataLength < items.Length )//如果数据项少于绑定项
      {



        var lastItem = items[dataLength - 1];

        while ( lastItem.NextNode() != tail )//将最后一个元素到尾部之间的所有元素清除。
          lastItem.NextNode().Remove();


        return items.Take( dataLength ).ToArray();
      }




      var result = new List<IHtmlElement>( dataLength );
      result.AddRange( items );

      int index = items.Length, itemIndex = 1;

      int[] indexer = items.Select( i => i.NodesIndexOfSelf() + 1 ).ToArray();//对所有的目标项位置建立索引
      var nodes = container.Nodes().ToArray();                                //缓存当前所有节点为复制做准备


      while ( index < dataLength )
      {
        if ( itemIndex == items.Length )
          itemIndex = 1;


        var range = nodes.Skip( indexer[itemIndex - 1] ).Take( indexer[itemIndex] - indexer[itemIndex - 1] );

        if ( tail == null )
          result.Add( container.AddCopy( range ).Last().CastTo<IHtmlElement>() );
        
        else
          result.Add( tail.AddCopyBeforeSelf( range ).Last().CastTo<IHtmlElement>() );

        index++;
        itemIndex++;
      }

      return result.ToArray();
    }

    private void DataBind( IHtmlElement element, object dataModel )
    {
      CreateBindingContext( ParentContext, element, dataModel ).DataBind();
    }

  }
}
