using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 标准的　HTML 列表元素，如 ul 或是 ol 绑定器
  /// </summary>
  public class GeneralListBinder : ListBinderBase
  {


    private static readonly HashSet<string> generalListType = new HashSet<string>( StringComparer.OrdinalIgnoreCase ) { "ul", "ol" };


    /// <summary>
    /// 创建 GeneralListBinder 对象
    /// </summary>
    /// <param name="container"></param>
    /// <param name="dataSource"></param>
    public GeneralListBinder( IHtmlElement container, object[] dataSource )
      : base( dataSource, null )
    {
      Container = container;

      if ( !generalListType.Contains( container.Name ) )
        throw new NotSupportedException( "GeneralListBinder 只支持 ul 或者 ol 元素绑定" );
    }


    protected override bool IsListItem( IHtmlElement element )
    {
      if ( element.Container.Equals( Container ) && element.Name.EqualsIgnoreCase( "li" ) )
        return true;

      else
        return false;
    }


    protected IHtmlContainer Container
    {
      get;
      private set;
    }



    /// <summary>
    /// 检查容器是否可以被 GeneralListBinder 处理
    /// </summary>
    /// <param name="container">要进行列表绑定的容器</param>
    /// <returns>是否可以进行列表绑定</returns>
    internal static bool CanBind( IHtmlElement container )
    {
      if ( generalListType.Contains( container.Name ) )
        return true;

      else
        return false;
    }
  }
}
