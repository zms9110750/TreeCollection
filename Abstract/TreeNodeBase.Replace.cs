namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
	#region Replace
	/// <summary>
	/// 替换某个节点
	/// </summary>
	/// <param name="index">索引</param>
	/// <param name="node">新节点</param>
	protected virtual void ReplaceNode(Index index, TreeNodeBase<T> node)
	{
		var offset = ThrowIfArgumentOutOfRange(index, true);
		ValidateElement(node);
		this[offset].Parent = null;
		this[offset].Index = 0;
		this[offset].ResertLevelAndRoot();
		Children[offset] = node;
		node.Parent = this;
		node.Index = offset;
	}
	/// <summary>
	/// 替换子节点的值
	/// </summary>
	/// <param name="node">要替换值的节点</param>
	/// <param name="newValue">新值</param>
	protected abstract void ReplaceNodeValue(TreeNodeBase<T> node, T? newValue);
	#endregion
}
