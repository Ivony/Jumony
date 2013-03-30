using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Css
{
  public abstract class CssStyleSpecificationBase
  {

    


    public CssStyleProperty[] TransformProperties( CssStyleProperty[] properties )
    {
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
