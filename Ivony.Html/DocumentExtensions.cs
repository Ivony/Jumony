using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.CodeDom;


namespace Ivony.Html
{
  public static class DocumentExtensions
  {

    /// <summary>
    /// 在文档中通过ID来查找元素
    /// </summary>
    /// <param name="document">要查找元素的文档</param>
    /// <param name="id">元素ID</param>
    /// <returns>找到的元素，没有符合要求的则返回null</returns>
    /// <exception cref="System.InvalidOperationException">找到多个ID相同的元素</exception>
    public static IHtmlElement GetElementById( this IHtmlDocument document, string id )
    {
      return document.Descendants().SingleOrDefault( element => element.Attribute( "id" ).Value() == id );
    }


    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element )
    {
      return Identity( element, false );
    }

    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <param name="create">指示当没有唯一ID时是否创建一个</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element, bool create )
    {
      return Identity( element, create, false );
    }

    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <param name="create">指示当没有唯一ID时是否创建一个</param>
    /// <param name="ancestorsCreate">在创建ID的过程中，是否为没有唯一ID的父级也创建ID</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element, bool create, bool ancestorsCreate )
    {
      EnsureAllocated( element );

      var id = element.Attribute( "id" ).Value();

      if ( string.IsNullOrEmpty( id ) || !element.Document.Descendants().Where( e => e.Attribute( "id" ).Value() == id ).OnlyOne() )
        id = null;

      if ( create && id == null )
        element.SetAttribute( "id" ).Value( id = CreateIdentity( element, ancestorsCreate ) );


      return id;
    }

    private static string CreateIdentity( IHtmlElement element, bool ancestorsCreate )
    {
      string parentId;

      var parentElement = element.Parent();
      if ( parentElement != null )
      {
        parentId = Identity( parentElement );
        if ( parentId == null )
        {
          if ( ancestorsCreate )
            parentId = Identity( parentElement, true, true );
          else
            parentId = CreateIdentity( parentElement, false );
        }
      }
      else
      {
        if ( element.Container is IHtmlDocument )
          parentId = null;
        else
          throw new InvalidOperationException();
      }

      var name = GetElementName( element );

      var builder = new StringBuilder();

      if ( !string.IsNullOrEmpty( parentId ) )
        builder.AppendFormat( "{0}_", parentId );

      builder.Append( name );

      if ( element.Siblings().Where( e => GetElementName( e ).EqualsIgnoreCase( GetElementName( element ) ) ).OnlyOne() )
        return builder.ToString();



      var index = element.SiblingsBeforeSelf().Count( e => GetElementName( e ).EqualsIgnoreCase( GetElementName( element ) ) );

      builder.Append( index + 1 );

      var identity = builder.ToString();

      return EnsureUniqueness( identity, element.Document );
    }

    private static string EnsureUniqueness( string identity, IHtmlDocument document )
    {
      return EnsureUniqueness( identity, document.Descendants().Select( element => element.Attribute( "id" ).Value() ).NotNull() );
    }

    private static string EnsureUniqueness( string identity, IEnumerable<string> ExistsIdentities )
    {
      var id = identity;
      var postfix = 0;

      while ( ExistsIdentities.Any( _id => _id == id ) )
        id = identity + "_" + postfix++;

      return id;
    }



    private static void EnsureAllocated( IHtmlNode node )
    {

      if ( node is IFreeNode )
        throw new InvalidOperationException( "无法对没有被分配在文档上的元素或节点进行操作" );

      var container = node.Container;

      if ( container is HtmlFragment )
        throw new InvalidOperationException( "无法对没有被分配在文档上的元素或节点进行操作" );

      node = container as IHtmlNode;
      if ( node == null )
        return;

      else
        EnsureAllocated( node );

    }

    private static string GetElementName( IHtmlElement element )
    {

      switch ( element.Name.ToLowerInvariant() )
      {

        case "html":
        //case "head":
        case "body":
          return null;

        case "a":
          if ( element.Attribute( "name" ) != null )
            return "link";
          else
            return "anchor";

        case "li":
          return "item";

        case "ul":
        case "ol":
          return "list";

        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
          return "header";

        default:
          return element.Name;

      }
    }




    public static CodeMemberMethod GenerateCodeMethod( this IHtmlDocument document, string methodName )
    {
      var constructor = new CodeMemberMethod();
      constructor.Name = methodName;
      constructor.Attributes = MemberAttributes.Public | MemberAttributes.Static;

      constructor.Parameters.Add( new CodeParameterDeclarationExpression( typeof( IHtmlNodeFactory ), "_factory" ) );
      constructor.ReturnType = new CodeTypeReference( typeof( IHtmlDocument ) );

      constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IHtmlDocument ), "_document", new CodeMethodInvokeExpression( new CodeVariableReferenceExpression( "_factory" ), "CreateDocument" ) ) );//var document = factory.CreateDocument();

      constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IFreeNode ), "_node" ) );// var node;

      var documentVariable = new CodeVariableReferenceExpression( "_document" );

      BuildChildNodesStatement( document, documentVariable, constructor.Statements, new List<string>() );//build document

      constructor.Statements.Add( new CodeMethodReturnStatement( documentVariable ) );

      return constructor;
    }

    private static void BuildChildNodesStatement( IHtmlContainer container, CodeVariableReferenceExpression contaienrVariable, CodeStatementCollection statements, IList<string> existsElements )
    {

      int index = 0;

      foreach ( var node in container.Nodes() )
      {
        var nodeVariable = BuildCreateNodeStatement( node, statements, existsElements );

        statements.Add( new CodeMethodInvokeExpression( nodeVariable, "Into", contaienrVariable, new CodePrimitiveExpression( index ) ) );

        index++;
      }
    }

    private static CodeVariableReferenceExpression BuildCreateNodeStatement( IHtmlNode node, CodeStatementCollection statements, IList<string> existsElements )
    {

      var factoryVariable = new CodeVariableReferenceExpression( "_factory" );
      var nodeVariable = new CodeVariableReferenceExpression( "_node" );

      var textNode = node as IHtmlTextNode;
      if ( textNode != null )
      {
        statements.Add( new CodeAssignStatement( nodeVariable, new CodeMethodInvokeExpression( factoryVariable, "CreateTextNode", new CodePrimitiveExpression( textNode.HtmlText ) ) ) );
        return nodeVariable;
      }


      var comment = node as IHtmlComment;
      if ( comment != null )
      {
        statements.Add( new CodeAssignStatement( nodeVariable, new CodeMethodInvokeExpression( factoryVariable, "CreateComment", new CodePrimitiveExpression( comment.Comment ) ) ) );
        return nodeVariable;
      }


      var element = node as IHtmlElement;

      if ( element != null )
      {
        var elementId = CreateIdentity( element, false );

        elementId = EnsureUniqueness( elementId, existsElements );
        existsElements.Add( elementId );

        elementId = "element_" + elementId;

        statements.Add( new CodeCommentStatement( ContentExtensions.GenerateTagHtml( element ) ) );
        statements.Add( new CodeVariableDeclarationStatement( typeof( IFreeElement ), elementId, new CodeMethodInvokeExpression( factoryVariable, "CreateElement", new CodePrimitiveExpression( element.Name ) ) ) );


        var elementVariable = new CodeVariableReferenceExpression( elementId );


        foreach ( var attribute in element.Attributes() )
        {
          var addAttribute = new CodeMethodInvokeExpression( elementVariable, "AddAttribute", new CodePrimitiveExpression( attribute.Name ) );

          if ( attribute.AttributeValue != null )
            statements.Add( new CodeAssignStatement( new CodePropertyReferenceExpression( addAttribute, "AttributeValue" ), new CodePrimitiveExpression( attribute.AttributeValue ) ) );
          else
            statements.Add( addAttribute );
        }



        BuildChildNodesStatement( element, elementVariable, statements, existsElements );

        return elementVariable;

      }

      throw new NotSupportedException();

    }


  }
}
