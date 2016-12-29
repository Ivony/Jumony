using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html
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


    /// <summary>
    /// 获取 CSS 样式缩写规则
    /// </summary>
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


    /// <summary>
    /// 检查指定的样式属性是否为一个缩写形式
    /// </summary>
    /// <param name="name">指定的属性名</param>
    /// <returns>是否为缩写形式</returns>
    public bool IsShorthandStyle( string name )
    {
      return StyleShorthandRules.Contains( name );
    }

    /// <summary>
    /// 获取缩写形式的样式属性
    /// </summary>
    /// <param name="name">样式属性名</param>
    /// <param name="style">所有的样式设置</param>
    /// <returns>返回缩写形式，如果可能</returns>
    public CssStyleProperty TryGetShorthandProperty( string name, CssStyle style )
    {
      if ( StyleShorthandRules.Contains( name ) )
      {
        var rule = StyleShorthandRules[name];
        return rule.TryGetShorthandProperty( style );
      }

      else
        return null;


    }
  }


  /// <summary>
  /// CSS 2.1 样式规范
  /// </summary>
  public class Css21StyleSpecification : CssStyleSpecificationBase
  {


    /// <summary>
    /// 创建 Css21StyleSpecification 对象
    /// </summary>
    public Css21StyleSpecification()
    {
      StyleShorthandRules.Add( new StandardBoxShorthandRule( "padding" ) );
      StyleShorthandRules.Add( new StandardBoxShorthandRule( "margin" ) );
      StyleShorthandRules.Add( new StandardBoxShorthandRule( "border-width" ) );
      StyleShorthandRules.Add( new StandardBoxShorthandRule( "border-style" ) );
      StyleShorthandRules.Add( new StandardBoxShorthandRule( "border-color" ) );
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


  /// <summary>
  /// 定义 CSS 样式缩写规则容器
  /// </summary>
  public class CssStyleShorthandRuleCollection : KeyedCollection<string, ICssStyleShorthandRule>
  {

    /// <summary>
    /// 重写此方法从样式规则中提取键
    /// </summary>
    /// <param name="item">要提取键（即样式名）的样式缩写规则</param>
    /// <returns>提取的键</returns>
    protected override string GetKeyForItem( ICssStyleShorthandRule item )
    {
      return item.Name;
    }
  }
}
