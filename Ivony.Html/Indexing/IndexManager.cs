using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Indexing
{
  public class IndexManager : IDisposable
  {

    /// <summary>
    /// 对容器创建索引管理器
    /// </summary>
    /// <param name="container">要创建索引的容器</param>
    public IndexManager( IHtmlContainer container )
    {
      Container = container;

      Initialize();
      Rebuild();

    }

    /// <summary>
    /// 索引管理器所管理的容器
    /// </summary>
    public IHtmlContainer Container
    {
      get;
      private set;
    }


    private List<ElementIndex> _indexes = new List<ElementIndex>();

    /// <summary>
    /// 所管理的所有元素索引
    /// </summary>
    public IEnumerable<ElementIndex> ElementIndexes
    {
      get { return _indexes; }
    }


    /// <summary>
    /// 重建索引
    /// </summary>
    protected void Rebuild()
    {
      InitializeIndexes();

      lock ( SyncRoot )
      {

        Container.Descendants()
          .ForAll( element =>
            {
              AddElement( element );
            }
          );
      }
    }

    /// <summary>
    /// 对指定索引对象重建索引
    /// </summary>
    /// <param name="index">指定的索引对象</param>
    protected void Rebuild( ElementIndex index )
    {
      lock ( SyncRoot )
      {
        _elements.ForAll( e => index.AddElement( e ) );
      }
    }



    private readonly object _sync = new object();
    /// <summary>
    /// 用于同步的对象
    /// </summary>
    protected object SyncRoot
    {
      get { return _sync; }
    }


    private const string DataKey = "Jumony_IndexManager";


    private INotifyDomChanged notify;
    private IDataContainer dataContainer;

    private void Initialize()
    {

      dataContainer = Container as IDataContainer;
      if ( dataContainer == null )
        throw new NotSupportedException( "容器不支持保存额外数据，索引管理器无法附加" );


      notify = Container as INotifyDomChanged;
      if ( notify == null )
        notify = Container.Document as INotifyDomChanged;

      if ( notify == null )
        notify = Container.Document.DomModifier as INotifyDomChanged;

      if ( notify == null )
        throw new NotSupportedException( "容器所属文档不支持 DOM 事件监控，无法建立索引" );

      lock ( dataContainer.Data.SyncRoot )
      {
        if ( dataContainer.Data.Contains( DataKey ) )
          throw new InvalidOperationException( "容器已存在索引管理器或冲突" );

        dataContainer.Data.Add( DataKey, this );
      }

      notify.HtmlDomChanged += OnHtmlDomChanged;

    }

    private void OnHtmlDomChanged( object sender, HtmlDomChangedEventArgs e )
    {

      lock ( SyncRoot )
      {
        if ( !InScope( e.Node ) )//不在索引范围内的节点忽略。
          return;


        var textNode = e.Node as IHtmlTextNode;
        if ( textNode != null )
        {
          _textNodes.Add( textNode );
          return;
        }

        var comment = e.Node as IHtmlComment;
        if ( comment != null )
        {
          _comments.Add( comment );
          return;
        }

        var element = e.Node as IHtmlElement;
        if ( element != null )
        {

          if ( !e.IsAttributeChanged )
            OnElementChanged( sender, e.Action, element );
          else
            OnAttributeChanged( sender, e.Action, e.Attribute, element );
        }
      }
    }

    /// <summary>
    /// 当元素被修改
    /// </summary>
    /// <param name="sender">引发事件的对象</param>
    /// <param name="action">引发事件的操作</param>
    /// <param name="element">属性所属的元素</param>
    protected virtual void OnElementChanged( object sender, HtmlDomChangedAction action, IHtmlElement element )
    {
      switch ( action )
      {

        case HtmlDomChangedAction.Add:
          AddElement( element );
          break;

        case HtmlDomChangedAction.Remove:
          RemoveElement( element );
          break;

        default:
          throw new InvalidOperationException( "未知的 DOM 结构变化" );
      }
    }


    /// <summary>
    /// 当属性被修改
    /// </summary>
    /// <param name="sender">引发事件的对象</param>
    /// <param name="action">引发事件的操作</param>
    /// <param name="attribute">被修改的属性</param>
    /// <param name="element">属性所属的元素</param>
    protected virtual void OnAttributeChanged( object sender, HtmlDomChangedAction action, IHtmlAttribute attribute, IHtmlElement element )
    {

      switch ( action )
      {

        case HtmlDomChangedAction.Add:
          AddAttribute( element, attribute );
          break;

        case HtmlDomChangedAction.Remove:
          RemoveAttribute( element, attribute );
          break;

        default:
          throw new InvalidOperationException( "未知的 DOM 结构变化" );
      }
    }


    /// <summary>
    /// 向索引添加一个元素
    /// </summary>
    /// <param name="element"></param>
    protected void AddElement( IHtmlElement element )
    {
      ElementIndexes.ForAll( index => index.AddElement( element ) );
      _elements.Add( element );
    }

    /// <summary>
    /// 从索引移除一个元素
    /// </summary>
    /// <param name="element"></param>
    protected void RemoveElement( IHtmlElement element )
    {
      ElementIndexes.ForAll( index => index.RemoveElement( element ) );
      _elements.Remove( element );
    }



    /// <summary>
    /// 索引内的元素添加一个属性
    /// </summary>
    /// <param name="element">被添加属性的元素</param>
    /// <param name="attribute">添加的属性</param>
    protected void AddAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      ElementIndexes.ForAll( index => index.AddAttribute( element, attribute ) );
    }

    /// <summary>
    /// 索引内的元素移除一个属性
    /// </summary>
    /// <param name="element">被移除属性的元素</param>
    /// <param name="attribute">移除的属性</param>
    protected void RemoveAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      ElementIndexes.ForAll( index => index.RemoveAttribute( element, attribute ) );
    }






    private HashSet<IHtmlTextNode> _textNodes = new HashSet<IHtmlTextNode>();
    private HashSet<IHtmlComment> _comments = new HashSet<IHtmlComment>();
    private HashSet<IHtmlElement> _elements = new HashSet<IHtmlElement>();


    /// <summary>
    /// 容器所有子代元素
    /// </summary>
    protected ICollection<IHtmlElement> DescendantElements
    {
      get { return _elements; }
    }

    /// <summary>
    /// 确定一个节点是否在索引范围内
    /// </summary>
    /// <param name="node">要检测的节点</param>
    /// <returns></returns>
    public bool InScope( IHtmlNode node )
    {

      lock ( SyncRoot )
      {
        if ( node == null )
          throw new ArgumentNullException( "node" );

        if ( node.Container.Equals( Container ) )
          return true;

        var element = node.Container as IHtmlElement;
        if ( element == null )
          return false;

        return DescendantElements.Contains( element );
      }
    }

    private void InitializeIndexes()
    {

      _indexes.Add( new ElementNameIndex( this ) );
      _indexes.Add( new ElementClassIndex( this ) );
      _indexes.Add( new ElementIdentityIndex( this ) );
      //UNDONE
    }


    /// <summary>
    /// 获取指定类型的元素索引
    /// </summary>
    /// <typeparam name="T">索引类型</typeparam>
    /// <returns>该类型元素索引</returns>
    public T ElementIndex<T>() where T : ElementIndex
    {
      return ElementIndex<T>( false );
    }

    /// <summary>
    /// 获取指定类型的元素索引
    /// </summary>
    /// <typeparam name="T">索引类型</typeparam>
    /// <param name="create">当该类型索引不存在时是否应当创建该类型索引</param>
    /// <returns>该类型元素索引</returns>
    public T ElementIndex<T>( bool create ) where T : ElementIndex
    {
      var index = _indexes.OfType<T>().FirstOrDefault();
      if ( index == null && create )
      {
        index = CreateIndex<T>();
        Rebuild( index );
      }

      return index;
    }



    /// <summary>
    /// 创建指定类型的元素索引
    /// </summary>
    /// <typeparam name="T">元素索引类型</typeparam>
    /// <returns></returns>
    protected T CreateIndex<T>()
    {
      return Activator.CreateInstance( typeof( T ) ).CastTo<T>();
    }





    void IDisposable.Dispose()
    {
      notify.HtmlDomChanged -= OnHtmlDomChanged;
      dataContainer.Data.Remove( DataKey );

      foreach ( var index in _indexes )
      {
        var disposable = index as IDisposable;
        if ( disposable != null )
          disposable.Dispose();
      }
    }
  }
}
