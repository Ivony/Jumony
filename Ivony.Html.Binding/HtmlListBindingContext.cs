using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;
using Ivony.Html.ExpandedAPI;
using Ivony.Html;
using System.Diagnostics.Contracts;

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


    /// <summary>
    /// 列表数据模型
    /// </summary>
    public new ListDataModel DataModel { get; private set; }



    public override void DataBind( IHtmlElement element )
    {
      if ( DataModel.Selector == null )//如果没有选择器，执行简单绑定
      {
        var count = DataModel.Count;
        var list = element.Repeat( count );

        for ( int i = 0; i < count; i++ )
          DataBind( list[i], DataModel[i] );
      }

      else
      {
        if ( GetElementBinder( element ) != null )
          throw new NotSupportedException( string.Format( "高级列表绑定不支持 \"{0}\" 元素，因为该元素已经自定义绑定逻辑。", element.Name ) );

        base.DataBind( element );
      }

    }



    /// <summary>
    /// 重写 BindChilds 方法，进行高级绑定。
    /// </summary>
    protected override void BindChilds( IHtmlContainer container )
    {

      if ( DataModel.Selector == null || container != BindingScope )   //考虑改为Contract。
        throw new InvalidOperationException();


      //Contract.Assert( DataModel.Selector != null && container == BindingScope );


      HashSet<IHtmlElement> dataItemElements;
      if ( DataModel.BindingMode == ListBindingMode.DynamicContent )
        dataItemElements = new HashSet<IHtmlElement>( DynamicContent() );

      else
        dataItemElements = new HashSet<IHtmlElement>( BindingScope.Elements().FilterBy( DataModel.Selector ) );



      int index = 0;

      foreach ( var e in BindingScope.Elements() )  //遍历所有子元素，对其进行数据绑定。
      {
        if ( dataItemElements.Contains( e ) )       //若该元素是数据项元素，则取出相应数据项进行绑定
          DataBind( e, DataModel[index++] );

        else
          DataBind( e, DataModel.RawObject );       //若该元素不是数据项元素，则使用列表的原始对象作为数据上下文进行绑定
      }

    }



    private IEnumerable<IHtmlElement> DynamicContent()
    {

      var container = BindingScope;

      var dataLength = DataModel.Count;

      var items = container.Elements().FilterBy( DataModel.Selector ).ToArray();//先找出所有目标元素

      if ( items.Length == 1 )//若目标元素只有一个，则退化到简单复制模式。
        return items.Single().Repeat( dataLength );


      var tail = items.Last().NextNode();//确定尾部


      if ( dataLength < items.Length )//如果数据项少于绑定项
      {

        var lastItem = items[dataLength - 1];

        while ( lastItem.NextNode() != tail )//将最后一个元素到尾部之间的所有元素清除。
          lastItem.NextNode().Remove();


        return items.Take( dataLength );
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

      return result;
    }

    private void DataBind( IHtmlElement element, object dataModel )
    {
      CreateBindingContext( ParentContext, element, dataModel ).DataBind();
    }

  }
}
