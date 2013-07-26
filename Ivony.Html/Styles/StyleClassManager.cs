using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ivony.Fluent;

namespace Ivony.Html.Styles
{
  /// <summary>
  /// 样式类管理器
  /// </summary>
  public class StyleClassManager : ICollection<string>
  {


    private IHtmlElement _element;

    private string _rawValue;


    private HashSet<string> _classes;

    private StyleClassManager( IHtmlElement element )
    {
      _element = element;
    }


    /// <summary>
    /// 获取指定元素的样式类管理器
    /// </summary>
    /// <param name="element">要获取样式类管理器的元素</param>
    /// <returns>样式类管理器</returns>
    internal static StyleClassManager GetStyleClassManager( IHtmlElement element )
    {
      var dataContainer = element as IDataContainer;


      //如果 style 属性支持缓存数据容器，则尝试缓存获取。
      if ( dataContainer != null )
      {
        var manager = dataContainer.Data[typeof( StyleClassManager )] as StyleClassManager;
        if ( manager != null )
          return manager;

        dataContainer.Data[typeof( StyleClassManager )] = manager = new StyleClassManager( element );
        return manager;
      }

      return new StyleClassManager( element );
    }



    /// <summary>
    /// 确保跟踪到了最新的样式类信息。
    /// </summary>
    private void EnsureUpdated()
    {
      var classValue = _element.Attribute( "class" ).Value();

      if ( _rawValue != classValue || _classes == null )
      {
        if ( string.IsNullOrEmpty( classValue ) )
          _classes = new HashSet<string>();
        else
          _classes = new HashSet<string>( Regulars.whiteSpaceSeparatorRegex.Split( classValue ).Where( c => c != "" ) );
        _rawValue = classValue;
      }

    }

    /// <summary>
    /// 添加一个样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Add( string className )
    {
      lock ( _element.SyncRoot )
      {

        EnsureUpdated();

        if ( _classes.Add( className ) )
          UpdateClass();

      }

      return this;
    }


    /// <summary>
    /// 添加多个样式类
    /// </summary>
    /// <param name="classNames">要添加的样式类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Add( params string[] classNames )
    {
      return Add( classNames.AsEnumerable() );
    }



    /// <summary>
    /// 添加多个样式类
    /// </summary>
    /// <param name="classNames">要添加的样式类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Add( IEnumerable<string> classNames )
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();
        bool flag = false;

        classNames.ForAll( delegate( string c )
        {
          if ( _classes.Add( c ) ) flag = true;
        } );


        if ( flag )
          UpdateClass();

        return this;
      }
    }



    /// <summary>
    /// 移除一个样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Remove( string className )
    {
      lock ( _element.SyncRoot )
      {
        if ( _classes.Remove( className ) )
          UpdateClass();
      }

      return this;
    }



    /// <summary>
    /// 移除多个样式类
    /// </summary>
    /// <param name="classNames">要移除的样式类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Remove( params string[] classNames )
    {
      return Remove( classNames.AsEnumerable() );
    }


    /// <summary>
    /// 移除多个样式类
    /// </summary>
    /// <param name="classNames">要添加的样式类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Remove( IEnumerable<string> classNames )
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();
        bool flag = false;

        classNames.ForAll( delegate( string c )
        {
          if ( _classes.Remove( c ) ) flag = true;
        } );


        if ( flag )
          UpdateClass();

        return this;
      }
    }


    /// <summary>
    /// 清除元素所有的样式类
    /// </summary>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Clear()
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();

        if ( _classes.Any() )
        {
          _classes.Clear();
          UpdateClass();
        }
      }

      return this;
    }

    /// <summary>
    /// 切换样式类（如果没有设置这个样式类则设置，否则移除这个样式类）
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>样式类管理器</returns>
    public StyleClassManager Toggle( string className )
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();

        if ( _classes.Contains( className ) )
          _classes.Remove( className );

        else
          _classes.Add( className );


        UpdateClass();
      }

      return this;
    }



    /// <summary>
    /// 更新 class 属性
    /// </summary>
    private void UpdateClass()
    {

      if ( _classes.Any() )
        _element.SetAttribute( "class", _rawValue = string.Join( " ", _classes.ToArray() ) );

      else
        _element.RemoveAttribute( "class" );
    }


    /// <summary>
    /// 检测是否包含指定名称的样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>是否包含指定名称的样式类</returns>
    public bool Contains( string className )
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();

        return _classes.Contains( className );
      }
    }

    void ICollection<string>.CopyTo( string[] array, int arrayIndex )
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();

        _classes.CopyTo( array, arrayIndex );
      }
    }

    int ICollection<string>.Count
    {
      get
      {
        lock ( _element.SyncRoot )
        {
          EnsureUpdated();

          return _classes.Count;
        }
      }
    }

    bool ICollection<string>.IsReadOnly
    {
      get { return false; }
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();

        return _classes.GetEnumerator();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();

        return _classes.GetEnumerator();
      }
    }

    void ICollection<string>.Add( string item )
    {
      Add( item );
    }

    void ICollection<string>.Clear()
    {
      Clear();
    }

    bool ICollection<string>.Remove( string item )
    {
      lock ( _element.SyncRoot )
      {
        EnsureUpdated();
        if ( _classes.Remove( item ) )
        {
          UpdateClass();
          return true;
        }

        return false;
      }
    }
  }
}
