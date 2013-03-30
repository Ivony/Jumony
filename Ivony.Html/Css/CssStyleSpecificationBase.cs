using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Css
{
  public abstract class CssStyleSpecificationBase
  {


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

  }

  public class CssStyleShorthandRuleCollection : KeyedCollection<string, ICssStyleShorthandRule>
  {
    protected override string GetKeyForItem( ICssStyleShorthandRule item )
    {
      return item.Name;
    }
  }




}
