using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace Ivony.Web.Html
{


  /// <summary>
  /// 保存数据绑定操作的上下文
  /// </summary>
  public class HtmlBindingContext : IDisposable
  {

    private HtmlBindingActionCollection _actionList = new HtmlBindingActionCollection();

    public IEnumerable<IHtmlBindingAction> Actions
    {
      get { return _actionList.GetActions(); }
    }


    /// <summary>
    /// 添加一个绑定操作
    /// </summary>
    /// <typeparam name="T">绑定对象类型</typeparam>
    /// <param name="target">绑定对象</param>
    /// <param name="action">绑定操作方法</param>
    public void Action<T>( T target, Action<T> action ) where T : IHtmlElement
    {
      _actionList.Add( new HtmlBindingAction<T>()
      {
        Target = target,
        Action = action
      } );
    }

    /// <summary>
    /// 添加一个绑定操作
    /// </summary>
    /// <typeparam name="T">绑定对象类型</typeparam>
    /// <param name="action">一个 BindAction 类型的对象，他保存了执行绑定操作所需的数据</param>
    public void Action<T>( HtmlBindingAction<T> action ) where T : IHtmlElement
    {
      _actionList.Add( action );
    }



    private Hashtable _dataContexts = new Hashtable();

    /// <summary>
    /// 为一个容器对象设置数据上下文
    /// </summary>
    /// <param name="container">要设置数据上下文的容器对象</param>
    /// <param name="dataContext">数据上下文</param>
    public void DataContext( IHtmlContainer container, object dataContext )
    {
      _dataContexts[container] = dataContext;
    }

    /// <summary>
    /// 获取指定节点或元素最近的数据上下文
    /// </summary>
    /// <returns>最近的数据上下文，如果没找到则返回null</returns>
    public object DataContext( IHtmlNode node )
    {

      if ( _dataContexts.ContainsKey( node ) )
        return _dataContexts[node];

      IHtmlContainer container;
      while ( (container = node.Parent) != null )
      {
        if ( _dataContexts.ContainsKey( container ) )
          return _dataContexts[container];

        node = container;
      }

      return null;
    }



    /// <summary>
    /// 执行数据绑定操作
    /// </summary>
    public void Commit()
    {

      if ( Current != this )
        throw new InvalidOperationException();

      foreach ( var element in PostOrderTraverse( Scope ) )
      {
        var actions = _actionList.GetActions( element );

        foreach ( var a in actions )
          a.Bind();

      }
    }


    /// <summary>
    /// 后序遍历所有元素用于绑定
    /// </summary>
    /// <param name="container">根元素</param>
    /// <returns></returns>
    private static IEnumerable<IHtmlElement> PostOrderTraverse( IHtmlContainer container )
    {
      var elementList = new List<IHtmlElement>();

      PostOrderTraverse( container, elementList );
      return elementList;
    }


    /// <summary>
    /// 后序遍历所有元素，并将找到的元素放入集合中
    /// </summary>
    /// <param name="container">根元素</param>
    /// <param name="elementList">存放元素的集合</param>
    /// <returns></returns>
    private static void PostOrderTraverse( IHtmlContainer container, IList<IHtmlElement> elementList )
    {
      foreach ( var node in container.Nodes().OfType<IHtmlContainer>() )
      {
        PostOrderTraverse( node, elementList );

        IHtmlElement element = node as IHtmlElement;
        if ( element != null )
          elementList.Add( element );
      }
    }



    private IHtmlContainer _scope;

    /// <summary>
    /// 绑定的范围
    /// </summary>
    public IHtmlContainer Scope
    {
      get { return _scope; }
    }



    private HtmlBindingContext( IHtmlContainer scope )
    {
      _scope = scope;
    }

    /// <summary>
    /// 创建并进入一个绑定上下文
    /// </summary>
    /// <param name="scope">绑定范围，超出此范围的绑定都不会被提交</param>
    /// <returns></returns>
    public static HtmlBindingContext EnterContext( IHtmlContainer scope )
    {
      var context = new HtmlBindingContext( scope );
      context.Enter();
      return context;
    }

    /// <summary>
    /// 获取当前的绑定上下文
    /// </summary>
    public static HtmlBindingContext Current
    {
      get
      {
        var contextStack = GetContextStack();
        if ( contextStack == null )
          throw new InvalidOperationException();

        if ( contextStack.Count == 0 )
          return null;

        return contextStack.Peek();
      }
    }


    private const string contextName = "HtmlBindingContexts";


    private bool _entered = false;

    private void Enter()
    {
      if ( _entered )
        throw new InvalidOperationException();

      var contextStack = GetContextStack();

      if ( contextStack == null )
      {
        contextStack = new Stack<HtmlBindingContext>();
        System.Runtime.Remoting.Messaging.CallContext.SetData( contextName, contextStack );
      }

      if ( contextStack.Contains( this ) )
        throw new InvalidOperationException();

      contextStack.Push( this );
    }

    private static Stack<HtmlBindingContext> GetContextStack()
    {
      return System.Runtime.Remoting.Messaging.CallContext.GetData( contextName ) as Stack<HtmlBindingContext>;
    }


    /// <summary>
    /// 放弃所有的绑定
    /// </summary>
    public void Discard()
    {
      _actionList.Clear();
    }


    public void Exit()
    {
      Exit( false );
    }

    /// <summary>
    /// 退出上下文
    /// </summary>
    /// <param name="discard">是否放弃上下文中存在的绑定操作</param>
    public void Exit( bool discard )
    {

      var contextStack = GetContextStack();
      if ( contextStack == null )
        throw new InvalidOperationException();

      if ( !contextStack.Contains( this ) )
        throw new InvalidOperationException();

      while ( true )
      {
        var context = contextStack.Pop();

        if ( discard )
          context.Discard();
        else
          context.Commit();

        if ( context == this )
          break;
      }

      _entered = false;
    }


    #region IDisposable 成员

    public void Dispose()
    {
      Exit( true );
    }

    #endregion




    private class HtmlBindingActionCollection
    {

      private Dictionary<object, List<IHtmlBindingAction>> _targetSet = new Dictionary<object, List<IHtmlBindingAction>>();

      public void Add( IHtmlBindingAction action )
      {
        if ( _targetSet.ContainsKey( action.Target ) )
          _targetSet[action.Target].Add( action );
        else
          _targetSet.Add( action.Target, new List<IHtmlBindingAction>() { action } );
      }


      public bool Contains( object target )
      {
        return _targetSet.ContainsKey( target );
      }


      public IEnumerable<IHtmlBindingAction> GetActions( object target )
      {
        List<IHtmlBindingAction> actions;
        if ( _targetSet.TryGetValue( target, out actions ) )
          return actions;
        else
          return new IHtmlBindingAction[0];
      }


      public IEnumerable<IHtmlBindingAction> GetActions()
      {
        return _targetSet.Values.SelectMany( value => value );
      }

      public void Clear()
      {
        _targetSet.Clear();
      }
    }

  }



  public interface IHtmlBindingAction
  {
    object Target { get; }

    void Bind();
  }

  public class HtmlBindingAction<T> : IHtmlBindingAction where T : IHtmlElement
  {

    public HtmlBindingAction()
    {
      DataBound = false;
    }

    public T Target
    {
      get;
      internal set;
    }

    public Action<T> Action
    {
      get;
      internal set;
    }

    object IHtmlBindingAction.Target { get { return Target; } }

    void IHtmlBindingAction.Bind() { Action( Target ); }

    public bool DataBound
    {
      get;
      internal set;
    }
  }
}
