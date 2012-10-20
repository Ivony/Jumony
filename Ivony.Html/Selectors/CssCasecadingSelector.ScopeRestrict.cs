using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{
    partial class CssCasecadingSelector
    {
      /// <summary>
      /// 返回表示当前选择器的表达式
      /// </summary>
      /// <returns>表示当前选择器的表达式</returns>
      public override string ToString()
      {
        return string.Format( CultureInfo.InvariantCulture, "{0} {1}", RelativeSelector, RightSelector );
      }



      /// <summary>
      /// 定义范畴限定选择器
      /// </summary>
      private class CssScopeRestrictionSelector : ICssSelector
      {

        private readonly IHtmlContainer _scope;


        /// <summary>
        /// 创建范畴限定选择器实例
        /// </summary>
        /// <param name="scope"></param>
        public CssScopeRestrictionSelector( IHtmlContainer scope )
        {
          if ( scope == null )
            throw new ArgumentNullException( "scope" );

          _scope = scope;
        }

        public bool IsEligible( IHtmlElement element )
        {

          if ( element == null )
            return false;

          return element.IsDescendantOf( _scope );
        }

        public override string ToString()
        {

          IHtmlElement element = _scope as IHtmlElement;
          if ( element != null )
            return "#" + element.Unique() + " ";
          else if ( _scope is IHtmlDocument )
            return "#document# ";
          else if ( _scope is IHtmlFragment )
            return "#fragment# ";
          else
            return "#unknow#";

        }

      }

      /// <summary>
      /// 创建层叠选择器
      /// </summary>
      /// <param name="expression">选择器表达式</param>
      /// <param name="elements">作为范畴限定的元素集</param>
      /// <returns>层叠选择器</returns>
      public static ICssSelector Create( string expression, IEnumerable<IHtmlElement> elements )
      {
        var rightSelector = Create( expression );

        if ( elements.IsNullOrEmpty() )
          return rightSelector;

        return new CssCasecadingSelector( new CssElementsRestrictionSelector( elements ), rightSelector );
      }


      public CssCasecadingSelector Clone()
      {
        throw new NotImplementedException();
      }




      private class CssElementsRestrictionSelector : ICssSelector
      {

        private readonly HashSet<IHtmlElement> _elements;

        public CssElementsRestrictionSelector( IEnumerable<IHtmlElement> elements )
        {

          if ( elements == null )
            throw new ArgumentNullException( "elements" );

          _elements = new HashSet<IHtmlElement>( elements );

        }

        public bool IsEligible( IHtmlElement element )
        {
          if ( element == null )
            return false;

          return _elements.Contains( element );
        }

        public override string ToString()
        {
          return "#elements#";
        }
      }

    }
}
