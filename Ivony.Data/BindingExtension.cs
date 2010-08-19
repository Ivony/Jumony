using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public static class BindingExtension
  {

    /// <summary>
    /// 截获定长的字符串
    /// </summary>
    /// <param name="source">源字符串</param>
    /// <param name="length">需要截获的长度</param>
    /// <returns>截获后的字符串</returns>
    public static string FixLenth( this string source, int length )
    {
      return source.FixLenth( length, "..." );
    }
    /// <summary>
    /// 截获定长的字符串
    /// </summary>
    /// <param name="source">源字符串</param>
    /// <param name="length">需要截获的长度</param>
    /// <param name="postfix">如果字符串被截短，需要添加什么样的后缀</param>
    /// <returns>截获后的字符串</returns>
    public static string FixLenth( this string source, int length, string postfix )
    {
      if ( source == null )
        throw new ArgumentNullException( "source" );
      if ( postfix == null )
        postfix = "...";



      int postfixLength = System.Text.Encoding.GetEncoding( "GB2312" ).GetByteCount( postfix );
      int srcLength = System.Text.Encoding.GetEncoding( "GB2312" ).GetByteCount( source );

      if ( length < postfixLength )
        throw new ArgumentOutOfRangeException( "length" );

      if ( srcLength > length )
      {
        for ( int i = source.Length; i > 0; i-- )
        {
          srcLength = System.Text.Encoding.GetEncoding( "GB2312" ).GetByteCount( source.Substring( 0, i ) );

          if ( srcLength <= length - postfixLength )
            return source.Substring( 0, i ) + postfix;
        }
        return "";
      }
      else
        return source;
    }



    public class BindingItem<ItemType, TargetType>
    {

      internal BindingItem( ItemType dataItem, TargetType targetObject, int index )
      {
        DataItem = dataItem;
        TargetObject = targetObject;
        Index = index;
        DataItemUseDefault = null;
      }

      internal BindingItem( ItemType dataItem, TargetType targetObject, int index, bool useDefault )
      {
        DataItem = dataItem;
        TargetObject = targetObject;
        Index = index;
        DataItemUseDefault = useDefault;
      }

      public ItemType DataItem { get; internal set; }
      public TargetType TargetObject { get; internal set; }
      public int Index { get; internal set; }
      public bool? DataItemUseDefault { get; internal set; }

    }


    public static IEnumerable<DataItemType> BindTo<DataItemType, TargetType>( this IEnumerable<DataItemType> dataSource, IEnumerable<TargetType> targets, Action<DataItemType, TargetType> binder )
    {
      return BindTo( dataSource, targets, binding => binder( binding.DataItem, binding.TargetObject ) );
    }

    public static IEnumerable<DataItemType> BindTo<DataItemType, TargetType>( this IEnumerable<DataItemType> dataSource, IEnumerable<TargetType> targets, Action<BindingItem<DataItemType, TargetType>> binder )
    {

      using ( var sourceIterator = dataSource.GetEnumerator() )
      {
        using ( var targetIterator = targets.GetEnumerator() )
        {
          int index = 0;

          while ( sourceIterator.MoveNext() && targetIterator.MoveNext() )
            binder( new BindingItem<DataItemType, TargetType>( sourceIterator.Current, targetIterator.Current, index++ ) );
        }
      }

      return dataSource;
    }


    public static IEnumerable<DataItemType> BindTo<DataItemType, TargetType>( this IEnumerable<DataItemType> dataSource, IEnumerable<TargetType> targets, DataItemType defaultValue, Action<DataItemType, TargetType> binder )
    {
      return BindTo( dataSource, targets, defaultValue, binding => binder( binding.DataItem, binding.TargetObject ) );
    }
    public static IEnumerable<DataItemType> BindTo<DataItemType, TargetType>( this IEnumerable<DataItemType> dataSource, IEnumerable<TargetType> targets, DataItemType defaultValue, Action<BindingItem<DataItemType, TargetType>> binder )
    {

      using ( var sourceIterator = dataSource.GetEnumerator() )
      {
        using ( var targetIterator = targets.GetEnumerator() )
        {

          bool sourceEnded = false;

          while ( targetIterator.MoveNext() )
          {

            int index = 0;

            if ( !sourceEnded )
              sourceEnded = !sourceIterator.MoveNext();

            var dataItem = sourceEnded ? defaultValue : sourceIterator.Current;
            var targetItem = targetIterator.Current;

            binder( new BindingItem<DataItemType, TargetType>( dataItem, targetItem, index++, sourceEnded ) );


          }
        }
      }

      return dataSource;
    }


  }
}
