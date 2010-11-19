using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  internal class DomElement : DomContainer, IHtmlElement
  {

    private readonly string _name;
    private readonly DomAttributeCollection _attributes;



    internal DomElement( string name, IDictionary<string, string> attributes )
    {


      if ( name == null )
        throw new ArgumentNullException( "name" );

      _name = name;
      _attributes = new DomAttributeCollection( this );

      if ( attributes != null )
        attributes.ForAll( item => _attributes.Add( new DomAttribute( this, item.Key, item.Value ) ) );
    }


    protected override string ObjectName
    {
      get { return "Element"; }
    }


    public string Name
    {
      get
      {
        CheckDisposed();

        return _name;
      }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      CheckDisposed();

      return _attributes.Cast<IHtmlAttribute>();
    }


    public IHtmlAttribute AddAttribute( string attributeName )
    {
      CheckDisposed();

      if ( _attributes.Contains( attributeName.ToLowerInvariant() ) )//执行不区分大小写的查找
        throw new InvalidOperationException();

      var attribute = new DomAttribute( this, attributeName, null );
      _attributes.Add( attribute );

      return attribute;
    }



    private class DomAttribute : IHtmlAttribute
    {

      private readonly DomElement _element;
      private readonly string _name;
      private string _value;

      private bool disposed = false;


      public DomAttribute( DomElement element, string name, string value )
      {
        _element = element;
        _name = name;
        _value = value;
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

      #region IHtmlAttribute 成员

      public IHtmlElement Element
      {
        get
        {
          if ( disposed )
            throw new ObjectDisposedException( "Attribute" );

          return _element;
        }
      }

      public string Name
      {
        get
        {
          if ( disposed )
            throw new ObjectDisposedException( "Attribute" );

          return _name;
        }
      }

      public string AttributeValue
      {
        get
        {
          if ( disposed )
            throw new ObjectDisposedException( "Attribute" );

          return _value;
        }
        set
        {
          if ( disposed )
            throw new ObjectDisposedException( "Attribute" );

          lock ( Element.SyncRoot )
          {
            _value = value;
          }
        }
      }

      public void Remove()
      {

        if ( disposed )
          throw new ObjectDisposedException( "Attribute" );

        lock ( Element.SyncRoot )
        {
          _element._attributes.Remove( this );
          disposed = true;
        }
      }

      #endregion
    }


    private class DomAttributeCollection : SynchronizedKeyedCollection<string, DomAttribute>
    {

      public DomAttributeCollection( DomElement element )
        : base( element.SyncRoot )
      {
      }


      protected override string GetKeyForItem( DomAttribute item )
      {
        return item.Name.ToLowerInvariant();
      }
    }



  }


  internal class DomFreeElement : HtmlElementWrapper, IFreeElement
  {

    private DomElement _element;
    private readonly DomFactory _factory;

    private bool disposed = false;

    public DomFreeElement( DomFactory factory, string name )
    {
      _element = new DomElement(  name, null );
      _factory = factory;
    }


    private void CheckDisposed()
    {
      if ( disposed )
        throw new ObjectDisposedException( "FreeElement" );
    }


    protected override IHtmlElement Element
    {
      get
      {
        CheckDisposed();

        return _element;
      }
    }


    IHtmlDocument IHtmlDomObject.Document
    {
      get
      {
        CheckDisposed();

        return _factory.Document;
      }
    }


    #region IFreeNode 成员

    public IHtmlNode Into( IHtmlContainer container, int index )
    {
      CheckDisposed();

      if ( container == null )
        throw new ArgumentNullException( "container" );

      var domContainer = container as DomContainer;
      if ( domContainer == null )
        throw new InvalidOperationException();


      domContainer.InsertNode( index, _element );
      disposed = true;

      return _element;

    }

    public IHtmlNodeFactory Factory
    {
      get
      {
        CheckDisposed();

        return _factory;
      }
    }

    #endregion
  }

}
