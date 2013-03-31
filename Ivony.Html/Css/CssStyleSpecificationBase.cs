using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Css
{
  
  /// <summary>
  /// 定义 CSS 样式规范抽象基类
  /// </summary>
  public abstract class CssStyleSpecificationBase
  {

    /// <summary>
    /// 创建 CssStyleSpecificationBase 对象
    /// </summary>
    protected CssStyleSpecificationBase()
    {
      StyleShorthandRules = new CssStyleShorthandRuleCollection();
      SyncRoot = new object();
    }

    protected CssStyleShorthandRuleCollection StyleShorthandRules
    {
      get;
      private set;
    }

    /// <summary>
    /// 转换 CSS 样式属性为其最终形式
    /// </summary>
    /// <param name="properties">设置的 CSS 样式属性</param>
    /// <returns>转换后的结果</returns>
    public CssStyleProperty[] TransformProperties( CssStyleProperty[] properties )
    {

      var result = properties.SelectMany( p => ExtractShorthand( p ) );
      return result.Where( p => ValidateProperty( p ) ).ToArray();


    }


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get;
      private set;
    }


    /// <summary>
    /// 检查 CSS 样式属性设置是否合法有效
    /// </summary>
    /// <param name="property">要检查的 CSS 样式属性设置</param>
    /// <returns>是否合法有效</returns>
    protected abstract bool ValidateProperty( CssStyleProperty property );

    /// <summary>
    /// 展开 CSS 样式属性的缩写形式
    /// </summary>
    /// <param name="property">样式属性设置</param>
    /// <returns>展开后的形式，若该设置不是缩写形式，则原样返回</returns>
    protected virtual IEnumerable<CssStyleProperty> ExtractShorthand( CssStyleProperty property )
    {
      lock ( SyncRoot )
      {
        if ( StyleShorthandRules.Contains( property.Name ) )
          return StyleShorthandRules[property.Name].ExtractProperties( property.Value );

        else
          return new[] { property };
      }
    }
  }


  /// <summary>
  /// CSS 2.1 样式规范
  /// </summary>
  public class Css21StyleSpecification : CssStyleSpecificationBase
  {


    public Css21StyleSpecification()
    {
      StyleShorthandRules.Add( new PaddingShorthandRule() );
      StyleShorthandRules.Add( new MarginShorthandRule() );
      StyleShorthandRules.Add( new BorderWidthShorthandRule() );
      StyleShorthandRules.Add( new BorderStyleShorthandRule() );
      StyleShorthandRules.Add( new BorderColorShorthandRule() );
    }

    /// <summary>
    /// 检查 CSS 样式属性设置是否合法有效
    /// </summary>
    /// <param name="property">要检查的 CSS 样式属性设置</param>
    /// <returns>是否合法有效</returns>
    protected override bool ValidateProperty( CssStyleProperty property )
    {
      return true;
    }
  }

  public class CssStyleShorthandRuleCollection : KeyedCollection<string, ICssStyleShorthandRule>
  {
    protected override string GetKeyForItem( ICssStyleShorthandRule item )
    {
      return item.Name;
    }
  }




}
