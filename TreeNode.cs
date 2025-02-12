using zms9110750.TreeCollection.Abstract;

namespace zms9110750.TreeCollection;
/// <summary>
/// 树节点
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="initValue">初始值</param>
public class TreeNode<T>(T? initValue) : TreeNodeBase<T>(initValue)
{
	public TreeNode(T? initValue, Func<T, IEnumerable<T>> childrenFactory) : this(initValue, childrenFactory, [initValue])
	{
	}
	protected TreeNode(T? initValue, Func<T, IEnumerable<T>> childrenFactory, HashSet<T> set) : this(initValue)
	{
		foreach (var child in childrenFactory(initValue!))
		{
			if (!set.Add(initValue!))
			{
				throw new ArgumentException("Duplicate child value found.");
			}
			Children.Add(new TreeNode<T>(child, childrenFactory,set));
		}
	}
	/// <summary>
	/// 添加节点
	/// </summary>
	/// <param name="index"></param>
	/// <param name="node"></param>
	/// <returns></returns>
	public TreeNode<T> AddAt(Index index, TreeNode<T> node)
	{
		base.AddAt(index, node);
		return node;
	}
	/// <summary>
	/// 批量添加节点
	/// </summary>
	/// <param name="index"></param>
	/// <param name="nodes"></param>
	public void AddAt(Index index, params ReadOnlySpan<TreeNode<T>> nodes)
	{
		var offset = ThrowIfArgumentOutOfRange(index, true);
		foreach (var node in nodes)
		{
			ValidateElement(node);
		}
		foreach (var node in nodes)
		{
			node.Parent = this;
		}
		if (offset == Count)
		{
#if NET9_0_OR_GREATER
			Children.AddRange(nodes);
#else
			Children.AddRange(nodes.ToArray());
#endif
		}
		else
		{
#if NET9_0_OR_GREATER
			Children.InsertRange(offset, nodes);
#else
			Children.InsertRange(offset, nodes.ToArray());
#endif
		}
		UpdateIndex(index..);
	}
	/// <summary>
	/// 添加节点
	/// </summary>
	/// <param name="index"></param>
	/// <param name="value"></param>
	/// <param name="childrenFactory"></param>
	/// <returns></returns>
	public override TreeNode<T> AddAt(Index index, T value, Func<T, IEnumerable<T>> childrenFactory)
	{
		return AddAt(index, new TreeNode<T>(value, childrenFactory));
	}

	protected override bool PreCheckExistence(T? value)
	{
		return true;
	}

	protected override void ReplaceNodeValue(TreeNodeBase<T> node, T? newValue)
	{
	}
	public static implicit operator TreeNode<T>(T value) => new TreeNode<T>(value);
}
