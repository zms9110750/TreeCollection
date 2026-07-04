using System.Collections;
using System.Diagnostics.CodeAnalysis;
using zms9110750.TreeCollection.Abstract;

namespace zms9110750.TreeCollection.Keyed;

/// <summary>
/// 基于键的有序树节点
/// </summary>
public class KeyedTreeNode<TKey, TValue> : RootNode<TValue, KeyedTreeNode<TKey, TValue>>, IKeyedTree<TKey, TValue, KeyedTreeNode<TKey, TValue>>
    where TKey : notnull
{
    private readonly SortedDictionary<TKey, KeyedTreeNode<TKey, TValue>> _children = [];

    /// <inheritdoc/>
    protected override IEnumerable<KeyedTreeNode<TKey, TValue>> ChildrenNode => _children.Values;

    /// <summary>
    /// 版本号，用于使视图失效
    /// </summary>
    protected int Version { get; set; }

    public KeyedTreeNode(TValue value) : base(value) { }

    // ==================== IDictionary ====================

    public KeyedTreeNode<TKey, TValue> this[TKey key]
    {
        get => _children[key];
        set
        {
            if (_children.TryGetValue(key, out var old))
            {
                old.Parent = null;
            }
            if (value.Parent != null && !ReferenceEquals(value.Parent, this))
            {
                value.Parent.Remove(value);
            }
            value.Parent = this;
            _children[key] = value;
        }
    }

    public ICollection<TKey> Keys => _children.Keys;
    public ICollection<KeyedTreeNode<TKey, TValue>> Values => _children.Values;
    public int Count => _children.Count;
    public bool IsReadOnly => false;

    public void Add(TKey key, KeyedTreeNode<TKey, TValue> node)
    {
        if (node.Parent != null && !ReferenceEquals(node.Parent, this))
        {
            node.Parent.Remove(node);
        }
        node.Parent = this;
        _children.Add(key, node);
    }

    /// <inheritdoc/>
    public KeyedTreeNode<TKey, TValue> Add(TKey key, TValue value)
    {
        var node = new KeyedTreeNode<TKey, TValue>(value);
        Add(key, node);
        return node;
    }

    public bool Remove(TKey key)
    {
        if (_children.TryGetValue(key, out var node))
        {
            node.Parent = null;
        }
        return _children.Remove(key);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out KeyedTreeNode<TKey, TValue> value)
    {
        return _children.TryGetValue(key, out value);
    }

    public bool ContainsKey(TKey key)
    {
        return _children.ContainsKey(key);
    }

    public void Clear()
    {
        foreach (var node in _children.Values)
        {
            node.Parent = null;
        }
        _children.Clear();
    }


    /// <inheritdoc/>
    public KeyedTreeNode<TKey, TValue>? RemoveBy(TKey key)
    {
        if (_children.TryGetValue(key, out var node))
        {
            node.Parent = null;
            _children.Remove(key);
            return node;
        }
        return default;
    }

    /// <inheritdoc/>
    public new IEnumerator<KeyValuePair<TKey, KeyedTreeNode<TKey, TValue>>> GetEnumerator()
    {
        return _children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // ==================== IKeyedTree ====================

    void IKeyedTree<TKey, TValue, KeyedTreeNode<TKey, TValue>>.IncrementVersion()
    {
        Version++;
    }

    // ==================== Tree 语义 ====================

    /// <summary>
    /// 移除子节点（引用）
    /// </summary>
    public KeyedTreeNode<TKey, TValue>? Remove(KeyedTreeNode<TKey, TValue> item)
    {
        foreach (var kvp in _children)
        {
            if (ReferenceEquals(kvp.Value, item))
            {
                item.Parent = null;
                _children.Remove(kvp.Key);
                return item;
            }
        }
        return null;
    }
}
