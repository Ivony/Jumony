using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.CodeDom;
using System.Reflection.Emit;
using System.Reflection;


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



    public static void EnsureAllocated( this IHtmlNode node )
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

      constructor.Parameters.Add( new CodeParameterDeclarationExpression( typeof( IHtmlDomProvider ), "provider" ) );
      constructor.ReturnType = new CodeTypeReference( typeof( IHtmlDocument ) );

      var providerVariable =  new CodeVariableReferenceExpression( "provider" );

      constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IHtmlDocument ), "document", new CodeMethodInvokeExpression( providerVariable, "CreateDocument" ) ) );//var document = factory.CreateDocument();

      constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IHtmlNode ), "node" ) );// var node;
      constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IDictionary<string, string> ), "attributes" ) );//var attributes

      var documentVariable = new CodeVariableReferenceExpression( "document" );

      BuildChildNodesStatement( document, documentVariable, constructor.Statements, new List<string>() );//build document

      constructor.Statements.Add( new CodeMethodReturnStatement( documentVariable ) );

      return constructor;
    }

    private static void BuildChildNodesStatement( IHtmlContainer container, CodeVariableReferenceExpression contaienrVariable, CodeStatementCollection statements, IList<string> existsElements )
    {


      var providerVariable = new CodeVariableReferenceExpression( "provider" );
      var nodeVariable = new CodeVariableReferenceExpression( "node" );
      var attributesVariable = new CodeVariableReferenceExpression( "attributes" );

      int index = 0;

      foreach ( var node in container.Nodes() )
      {

        var textNode = node as IHtmlTextNode;
        if ( textNode != null )
          statements.Add( new CodeAssignStatement( nodeVariable, new CodeMethodInvokeExpression( providerVariable, "AddTextNode", contaienrVariable, new CodePrimitiveExpression( index ), new CodePrimitiveExpression( textNode.HtmlText ) ) ) );


        var comment = node as IHtmlComment;
        if ( comment != null )
          statements.Add( new CodeAssignStatement( nodeVariable, new CodeMethodInvokeExpression( providerVariable, "AddComment", contaienrVariable, new CodePrimitiveExpression( index ), new CodePrimitiveExpression( comment.Comment ) ) ) );


        var element = node as IHtmlElement;

        if ( element != null )
        {
          var elementId = CreateIdentity( element, false );

          elementId = EnsureUniqueness( elementId, existsElements );
          existsElements.Add( elementId );

          elementId = "element_" + elementId;


          statements.Add( new CodeCommentStatement( ContentExtensions.GenerateTagHtml( element ) ) );

          statements.Add( new CodeAssignStatement( attributesVariable, new CodeObjectCreateExpression( typeof( Dictionary<string, string> ) ) ) );

          foreach ( var attribute in element.Attributes() )
            statements.Add( new CodeMethodInvokeExpression( attributesVariable, "Add", new CodePrimitiveExpression( attribute.Name ), new CodePrimitiveExpression( attribute.AttributeValue ) ) );

          statements.Add( new CodeVariableDeclarationStatement( typeof( IHtmlElement ), elementId, new CodeMethodInvokeExpression( providerVariable, "AddElement", contaienrVariable, new CodePrimitiveExpression( index ), new CodePrimitiveExpression( element.Name ), attributesVariable ) ) );

          var elementVariable = new CodeVariableReferenceExpression( elementId );


          BuildChildNodesStatement( element, elementVariable, statements, existsElements );

        }

        index++;
      }
    }


    public static Func<IHtmlDomProvider, IHtmlDocument> Compile( this IHtmlDocument document )
    {
      return DocumentCompiler.Compile( document );
    }

    private static class DocumentCompiler
    {

      public static Func<IHtmlDomProvider, IHtmlDocument> Compile( IHtmlDocument document )
      {
        //create document

        //dup            document, document   
        //st container   document

        //begin create element

        //ld provider    document, provider
        //ld container   document, provider, document
        //ld index       document, provider, document, 0
        //ld ElementName document, provider, document, 0, name
        //create element document, element

        //dup            document, element, element   
        //st container   document, element

        //begin create textNode

        //ld provider    document, element, provider
        //ld container   document, element, provider, element
        //ld index       document, element, provider, element, 0
        //ld text        document, element, provider, element, 0, text
        //create text    document, element, textNode
        //pop            document, element

        //end create textNode

        //begin create textNode

        //ld provider    document, element, provider
        //ld container   document, element, provider, element
        //ld index       document, element, provider, element, 1
        //ld text        document, element, provider, element, 1, text
        //create text    document, element, textNode
        //pop            document, element

        //end create textNode

        //end create element
        //pop            document
        //dup            document, document
        //st container   document


        //end create document
        //ret


        var method = new DynamicMethod( "", typeof( IHtmlDocument ), new[] { typeof( IHtmlDomProvider ) } );

        var il = method.GetILGenerator();

        il.DeclareLocal( typeof( IHtmlContainer ) );

        il.Emit( OpCodes.Ldarg_0 );
        il.Emit( OpCodes.Callvirt, CreateDocument );

        int index = 0;

        il.Emit( OpCodes.Dup );
        il.Emit( OpCodes.Stloc_0 );// set container;

        foreach ( var node in document.Nodes() )
          EmitCreateNode( il, node, index++ );

        il.Emit( OpCodes.Ret );

        return (Func<IHtmlDomProvider, IHtmlDocument>) method.CreateDelegate( typeof( Func<IHtmlDomProvider, IHtmlDocument> ) );
      }

      private static void EmitCreateNode( ILGenerator il, IHtmlNode node, int index )
      {

        il.Emit( OpCodes.Ldarg_0 );       //ld provider
        il.Emit( OpCodes.Ldloc_0 );       //ld container
        il.Emit( OpCodes.Ldc_I4, index ); //ld index

        var textNode = node as IHtmlTextNode;
        if ( textNode != null )
        {
          il.Emit( OpCodes.Ldstr, textNode.HtmlText );
          il.Emit( OpCodes.Callvirt, AddTextNode );
          il.Emit( OpCodes.Pop );
          return;
        }

        var comment = node as IHtmlComment;
        if ( textNode != null )
        {
          il.Emit( OpCodes.Ldstr, comment.Comment );
          il.Emit( OpCodes.Callvirt, AddComment );
          il.Emit( OpCodes.Pop );
          return;
        }

        return;

        var element = node as IHtmlElement;
        if ( textNode != null )
        {
          il.Emit( OpCodes.Ldstr, element.Name );

          il.Emit( OpCodes.Newobj, NewDictionary );

          foreach ( IHtmlAttribute attribute in element.Attributes() )
          {
            il.Emit( OpCodes.Dup );

            il.Emit( OpCodes.Ldstr, attribute.Name );

            var value = attribute.AttributeValue;

            if ( value != null )
              il.Emit( OpCodes.Ldstr, value );
            else
              il.Emit( OpCodes.Ldnull );

            il.Emit( OpCodes.Callvirt, DictionaryAdd );
          }

          il.Emit( OpCodes.Callvirt, AddElement );

          il.Emit( OpCodes.Dup );
          il.Emit( OpCodes.Stloc_0 );// set container;

          /*
          int childIndex = 0;
          foreach ( var childNode in element.Nodes() )
            EmitCreateNode( il, childNode, childIndex++ );
          */

          il.Emit( OpCodes.Pop );    //pop element
          il.Emit( OpCodes.Dup );
          il.Emit( OpCodes.Stloc_0 );//set container
          return;
        }
      }

      private static readonly MethodInfo CreateDocument = typeof( IHtmlDomProvider ).GetMethod( "CreateDocument" );
      private static readonly MethodInfo AddTextNode = typeof( IHtmlDomProvider ).GetMethod( "AddTextNode" );
      private static readonly MethodInfo AddComment = typeof( IHtmlDomProvider ).GetMethod( "AddComment" );
      private static readonly MethodInfo AddElement = typeof( IHtmlDomProvider ).GetMethod( "AddElement" );
      private static readonly ConstructorInfo NewDictionary = typeof( Dictionary<string, string> ).GetConstructor( new Type[0] );
      private static readonly MethodInfo DictionaryAdd = typeof( IDictionary<string, string> ).GetMethod( "Add" );
    }
  }
}
