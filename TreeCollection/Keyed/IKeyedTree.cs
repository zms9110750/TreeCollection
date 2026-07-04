using zms9110750.TreeCollection.Abstract;
using zms9110750.InterfaceImplAsExtensionGenerator;

namespace zms9110750.TreeCollection.Keyed;

/// <summary>
/// 基于键的有序树节点接口。子节点通过键访问，按键排序。
/// </summary>
[ExtensionSource]
public interface IKeyedTree<TKey, TValue, TNode> : IValue<TValue>, INode<TNode>, IDictionary<TKey, TNode>
	where TKey : notnull
	where TNode : IKeyedTree<TKey, TValue, TNode>
{
	private static EqualityComparer<TValue> ValueComparer => EqualityComparer<TValue>.Default;

	/// <summary>
	/// 用值创建新节点并添加到指定键
	/// </summary>
	TNode Add(TKey key, TValue value);

	/// <summary>
	/// 移除指定键的子节点并返回。需要同时置 Parent = null。
	/// </summary>
	TNode? RemoveBy(TKey key);

	/// <summary>
	/// 把子节点从旧键改为新键（重新设置 key）。
	/// </summary>
	/// <param name="sourceKey">当前键</param>
	/// <param name="targetKey">目标键</param>
	/// <exception cref="KeyNotFoundException"><paramref name="sourceKey"/> 不存在</exception>
	/// <exception cref="ArgumentException"><paramref name="targetKey"/> 已存在</exception>
	void ChangeKey(TKey sourceKey, TKey targetKey)
	{
		if (ContainsKey(targetKey))
		{
			throw new ArgumentException($"Target key '{targetKey}' already exists.", nameof(targetKey));
		}
		var node = RemoveBy(sourceKey) ?? throw new KeyNotFoundException($"Source key '{sourceKey}' not found.");
		Add(targetKey, node);
		IncrementVersion();
	}

	/// <summary>
	/// 交换两个子节点的键
	/// </summary>
	void SwapChildren(TKey keyA, TKey keyB)
	{
		var nodeA = RemoveBy(keyA) ?? throw new KeyNotFoundException($"Key '{keyA}' not found.");
		var nodeB = RemoveBy(keyB) ?? throw new KeyNotFoundException($"Key '{keyB}' not found.");
		Add(keyA, nodeB);
		Add(keyB, nodeA);
		IncrementVersion();
	}

	/// <summary>
	/// 替换子节点（保留键）。由实现类通过 this[TKey] setter 处理 Parent 变更。
	/// </summary>
	bool Replace(TNode oldNode, TNode newNode)
	{
		foreach (var kvp in this)
		{
			if (EqualityComparer<TNode>.Default.Equals(kvp.Value, oldNode))
			{
				this[kvp.Key] = newNode;
				IncrementVersion();
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 移除所有匹配的子节点
	/// </summary>
	int RemoveAll(Predicate<TNode>? match = null)
	{
		var keys = new List<TKey>();
		foreach (var kvp in this)
		{
			if (match == null || match(kvp.Value))
			{
				keys.Add(kvp.Key);
			}
		}
		foreach (var key in keys)
		{
			RemoveBy(key);
		}
		if (keys.Count > 0)
		{
			IncrementVersion();
		}
		return keys.Count;
	}

	/// <summary>
	/// 查询是否存在具有指定值的子节点
	/// </summary>
	bool Contains(TValue value)
	{
		foreach (var kvp in this)
		{
			if (ValueComparer.Equals(kvp.Value.Value, value))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 改变版本号
	/// </summary>
	internal void IncrementVersion();
}
