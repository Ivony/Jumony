using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using AP = HtmlAgilityPack;


namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlAttributeAdapter : IHtmlAttribute
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

    public string AttributeValue
    {
      get { return _attribute.Value; }
      set { _attribute.Value = value; }
    }


    public object NodeObject
    {
      get { return _attribute; }
    }


    public void Remove()
    {
      _attribute.Remove();
    }




    public override string ToString()
    {
      return AttributeValue;
    }


    public override bool Equals( object obj )
    {

      var attribute = obj as IHtmlAttribute;

      if ( attribute == null )
        return false;

      if ( !attribute.Element.Equals( this.Element ) )
        return false;

      if ( attribute.Name.EqualsIgnoreCase( this.Name ) && attribute.AttributeValue == this.AttributeValue )
        return true;

      return base.Equals( obj );
    }

    public override int GetHashCode()
    {
      return Element.GetHashCode() ^ Name.ToLowerInvariant().GetHashCode() ^ AttributeValue.GetHashCode();
    }

  }
}

