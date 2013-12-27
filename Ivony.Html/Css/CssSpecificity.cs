using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 CSS 选择器的特异性
  /// </summary>
  public struct CssSpecificity : IComparable<CssSpecificity>, IComparable, IEquatable<CssSpecificity>
  {

    private int a, b, c;

    /// <summary>
    /// 创建 CssSpecificity 对象
    /// </summary>
    /// <param name="a">特异性 a 值</param>
    /// <param name="b">特异性 b 值</param>
    /// <param name="c">特异性 c 值</param>
    public CssSpecificity( int a, int b, int c )
    {
      this.a = a;
      this.b = b;
      this.c = c;
    }



    /// <summary>
    /// 检查两个 CSS 特异性是否一致
    /// </summary>
    /// <param name="other">要检查的另一个特异性</param>
    /// <returns>是否一致</returns>
    public bool Equals( CssSpecificity other )
    {
      return this.a == other.a && this.b == other.b && this.c == other.c;
    }


    /// <summary>
    /// 重写 Equals 方法实现特异性的比较
    /// </summary>
    /// <param name="obj">要比较的特异性对象</param>
    /// <returns>两个对象是否相等</returns>
    public override bool Equals( object obj )
    {
      if ( obj == null || !(obj is CssSpecificity) )
        return false;

      else
        return Equals( (CssSpecificity) obj );
    }

    /// <summary>
    /// 重写 GetHashCode 方法，获取哈希值
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode();
    }


    /// <summary>
    /// 比较两个 CSS 选择器特异性之间的优先级
    /// </summary>
    /// <param name="obj">要进行比较的特异性</param>
    /// <returns>比较结果</returns>
    public int CompareTo( object obj )
    {
      if ( obj is CssSpecificity )
        return CompareTo( obj.CastTo<CssSpecificity>() );

      else
        throw new ArgumentException( "只能与 CssSpecificity 类型的参数进行比较" );
    }


    /// <summary>
    /// 比较两个 CSS 选择器特异性之间的优先级
    /// </summary>
    /// <param name="other">要进行比较的特异性</param>
    /// <returns>比较结果</returns>
    public int CompareTo( CssSpecificity other )
    {
      if ( this.a > other.a )
        return 1;
      else if ( this.a < other.a )
        return -1;

      else if ( this.b > other.b )
        return 1;
      else if ( this.b < other.b )
        return -1;

      else if ( this.c > other.c )
        return 1;
      else if ( this.c < other.c )
        return -1;

      else
        return 0;

    }
  }
}
