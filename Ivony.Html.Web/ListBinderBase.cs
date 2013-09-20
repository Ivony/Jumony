using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 为列表绑定器提供抽象基类
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ListBinderBase : IHtmlElementBinder
  {

    public ListBinderBase( object[] dataSource, object defaultItem )
    {
      DataSource = DataSource;
      DefaultItem = defaultItem;
    }


    public object[] DataSource
    {
      get;
      private set;
    }

    private int index = 0;

    public object DefaultItem
    {
      get;
      private set;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="context"></param>
    /// <param name="dataContext"></param>
    /// <returns></returns>
    public bool BindElement( IHtmlElement element, HtmlBindingContext context, out object dataContext )
    {

      dataContext = null;

      if ( IsListItem( element ) )
      {

        if ( CurrentItem == RemoveListItemBehavior )
        {
          element.Remove();
          return true;
        }

        else
        {
          dataContext = CurrentItem;
          MoveNext();
        }
      }

      return false;
    }



    /// <summary>
    /// 获取当前需要绑定的列表项
    /// </summary>
    protected object CurrentItem
    {
      get
      {
        if ( index < DataSource.Length )
          return DataSource[index];

        else
          return DefaultItem;
      }
    }


    /// <summary>
    /// 移动到下一个列表项
    /// </summary>
    protected void MoveNext()
    {
      index++;
    }



    /// <summary>
    /// 检测元素是否为列表项容器
    /// </summary>
    /// <param name="element">要检测的元素</param>
    /// <returns>是否为可以处理的列表项容器</returns>
    protected abstract bool IsListItem( IHtmlElement element );




    public bool BindAttribute( IHtmlAttribute attribute, HtmlBindingContext context )
    {
      return false;
    }




    private static readonly RemoveListItem removeListItemBehavior = new RemoveListItem();

    protected static object RemoveListItemBehavior
    {
      get { return removeListItemBehavior; }
    }


    private class RemoveListItem { }

  }
}
