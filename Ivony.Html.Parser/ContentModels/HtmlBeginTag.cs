using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlBeginTag : HtmlContentFragment
  {

    public HtmlBeginTag( HtmlContentFragment fragment, string tagName, bool selfClosed, IEnumerable<HtmlAttributeSetting> attibutes )
      : base( fragment )
    {
      if ( tagName == null )
        throw new ArgumentNullException( "tagName" );

      if ( attibutes == null )
        throw new ArgumentNullException( "attibutes" );

      TagName = tagName;
      Attributes = attibutes;
      SelfClosed = selfClosed;
    }


    public string TagName
    {
      get;
      private set;
    }

    public bool SelfClosed
    {
      get;
      private set;
    }



    public IEnumerable<HtmlAttributeSetting> Attributes
    {
      get;
      private set;
    }


  }


}
