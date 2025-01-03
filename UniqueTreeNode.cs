using System.Diagnostics.CodeAnalysis;
using zms9110750.TreeCollection.Abstract;
using zms9110750.TreeCollection.Extension;
namespace zms9110750.TreeCollection;

public class UniqueTreeNode<T>(T? initValue) : TreeNodeBase<T>(initValue)
{
	public UniqueTreeNode(T? initValue, Func<T, IEnumerable<T>> childrenFactory) : this(initValue)
	{
		foreach (var child in childrenFactory(initValue!))
		{
			Children.Add(new UniqueTreeNode<T>(child, childrenFactory));
		}
	}

	protected HashSet<T> Set { get; } = [];
	protected TreeNodeBase<T>? NullValueNodeHolder { get; set; }
	protected static ArgumentException HasValueException { get; } = new ArgumentException("这个值在父结点中已经存在");
	public UniqueTreeNode<T> AddAt(Index index, [NotNull] UniqueTreeNode<T> node)
	{
		AddAt(index, (TreeNodeBase<T>)node);
		return node;
	}
	public override TreeNodeBase<T> AddAt(Index index, [NotNull] TreeNodeBase<T> node)
	{
		ValidateElement(node);
		switch (node.Value)
		{
			case null:
				NullValueNodeHolder = node;
				break;
			default:
				Set.Add(node.Value);
				break;
		}
		return base.AddAt(index, node);
	}
	protected override void ValidateElement([NotNull] TreeNodeBase<T> node)
	{
		base.ValidateElement(node);
		if (PreCheckExistence(node.Value))
		{
			throw HasValueException;
		}
	}
	public override TreeNodeBase<T>? Remove(TreeNodeBase<T> node)
	{
		switch (node.Value)
		{
			case null:
				NullValueNodeHolder = null;
				break;
			default:
				Set.Remove(node.Value);
				break;
		}
		return base.Remove(node);
	}
	protected override bool PreCheckExistence(T? value)
	{
		return value == null ? NullValueNodeHolder != null : Set.Contains(value);
	}
	protected override void ReplaceNode(Index index, TreeNodeBase<T> newNode)
	{
		var oldNode = this[index];
		switch ((oldNode.Value, newNode.Value))
		{
			case (null, null):
				NullValueNodeHolder = newNode;
				break;
			case (null, not null):
				NullValueNodeHolder = null;
				Set.Add(newNode.Value);
				break;
			case (not null, null):
				NullValueNodeHolder = newNode;
				Set.Remove(oldNode.Value);
				break;
			case (not null, not null):
				Set.Remove(oldNode.Value);
				Set.Add(newNode.Value);
				break;
		}
		base.ReplaceNode(index, newNode);
	}

	protected override void ReplaceNodeValue(TreeNodeBase<T> node, T? newValue)
	{
		PreCheckExistence(newValue);
		var field = node.Value;
		switch ((field, newValue))
		{
			case (null, not null):
				NullValueNodeHolder = null;
				Set.Add(newValue);
				break;
			case (not null, null):
				NullValueNodeHolder = this;
				Set.Remove(field);
				break;
			case (not null, not null):
				Set.Remove(field);
				Set.Add(newValue);
				break;
			default:
				break;
		}
	}

	public override UniqueTreeNode<T> AddAt(Index index, T value, Func<T, IEnumerable<T>> childrenFactory)
	{
		return AddAt(index, new UniqueTreeNode<T>(value, childrenFactory));
	}
	public static implicit operator UniqueTreeNode<T>(T value) => new UniqueTreeNode<T>(value);
}
