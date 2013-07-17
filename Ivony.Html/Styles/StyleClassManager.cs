using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{
  /// <summary>
  /// 样式类管理器
  /// </summary>
  public class StyleClassManager
  {


    private IHtmlElement _element;

    private StyleClassManager( IHtmlElement element )
    {
      _element = element;
    }

    /// <summary>
    /// 添加一个样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>被操作的元素</returns>
    public IHtmlElement AddClass( string className )
    {
      lock ( _element.SyncRoot )
      {
        var classes = GetClasses();

        if ( !classes.Contains( className ) )
          classes.Add( className );

        SetClasses( classes );
      }

      return _element;
    }


    /// <summary>
    /// 移除一个样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>被操作的元素</returns>
    public IHtmlElement RemoveClass( string className )
    {
      lock ( _element.SyncRoot )
      {
        var classes = GetClasses();

        if ( classes.Contains( className ) )
          classes.Remove( className );

        SetClasses( classes );
      }

      return _element;
    }


    /// <summary>
    /// 获取当前应用的所有样式类
    /// </summary>
    /// <returns>样式类名集合</returns>
    public IEnumerable<string> Classes()
    {
      return GetClasses();
    }



    private void SetClasses( HashSet<string> classes )
    {
      _element.SetAttribute( "class", string.Join( " ", classes.ToArray() ) );
    }

    private HashSet<string> GetClasses()
    {
      var classSet = new HashSet<string>();


      var classes = _element.Attribute( "class" ).Value();
      if ( classes == null )
        return classSet;

      foreach ( var c in classes.Split( ' ' ) )
      {
        if ( string.IsNullOrEmpty( c ) )
          continue;

        if ( classSet.Contains( c ) )
          continue;


        classSet.Add( c );
      }

      return classSet;
    }


  }
}
