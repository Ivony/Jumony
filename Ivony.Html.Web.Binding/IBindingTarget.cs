using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IBindingTarget
  {
    Type ValueType { get; }

    void BindValue( object value );
  }


  public abstract class BindingTarget<T> : IBindingTarget
  {

    public Type ValueType
    {
      get { return typeof( T ); }
    }

    public void BindValue( object value )
    {
      if ( value is T )
        BindValue( (T) value );

      else
        throw new InvalidOperationException();

    }

    protected abstract void BindValue( T value );

  }


  /// <summary>
  /// 文本属性绑定目标
  /// </summary>
  public class TextAttributeBindingTarget : BindingTarget<string>
  {


    protected IHtmlAttribute TargetAttribute
    {
      get;
      private set;
    }

    public TextAttributeBindingTarget( IHtmlAttribute attribute )
    {
      TargetAttribute = attribute;
    }

    protected override void BindValue( string value )
    {
      if ( value == null )
        TargetAttribute.Remove();

      else
        TargetAttribute.SetValue( value );
    }
  }


  /// <summary>
  /// 标记属性绑定目标
  /// </summary>
  public class MarkupAttributeBidningTarget : BindingTarget<bool>
  {
    protected IHtmlAttribute TargetAttribute
    {
      get;
      private set;
    }

    public MarkupAttributeBidningTarget( IHtmlAttribute attribute )
    {
      TargetAttribute = attribute;
    }

    protected override void BindValue( bool value )
    {
      if ( !value )
        TargetAttribute.Remove();
    }
  }






}
