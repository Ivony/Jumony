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


    public CssStyleProperty[] TransformProperties( CssStyleProperty[] properties )
    {

      var result = properties.SelectMany( p => ExtractShorthand( p ) );
      return result.Where( p => ValidateProperty( p ) ).ToArray();


    }

    public object SyncRoot
    {
      get;
      private set;
    }


    protected abstract bool ValidateProperty( CssStyleProperty property );

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

  public class Css21StyleSpecification : CssStyleSpecificationBase
  {

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
