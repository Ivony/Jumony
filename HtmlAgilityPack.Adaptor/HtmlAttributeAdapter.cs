using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;


namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlAttributeAdapter : IHtmlAttribute, IEquatable<IHtmlAttribute>
  {

    private AP.HtmlAttribute _attribute;

    internal HtmlAttributeAdapter( AP.HtmlAttribute attribute )
    {
      _attribute = attribute;
    }

    public IHtmlElement Element
    {
      get { return _attribute.OwnerNode.AsElement(); }
    }

    public string Name
    {
      get { return _attribute.Name; }
    }

    public string Value
    {
      get { return _attribute.Value; }
      set { _attribute.Value = value; }
    }


    public object NodeObject
    {
      get { return _attribute; }
    }

    public override string ToString()
    {
      return Value;
    }


    #region IEquatable<IHtmlAttribute> 成员

    bool IEquatable<IHtmlAttribute>.Equals( IHtmlAttribute other )
    {
      var attribute = other as HtmlAttributeAdapter;

      if ( attribute == null )
        return false;

      return NodeObject == attribute.NodeObject;
    }

    #endregion


    public override bool Equals( object obj )
    {

      if ( obj == null )
        return false;

      var element = obj as IHtmlElement;
      if ( element != null )
        return NodeObject.Equals( element.NodeObject );


      var node = obj as AP.HtmlNode;
      if ( node != null )
        return NodeObject.Equals( node );


      return base.Equals( obj );
    }

    public override int GetHashCode()
    {
      return NodeObject.GetHashCode();
    }

  }
}
