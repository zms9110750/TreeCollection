using System.Diagnostics.CodeAnalysis;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
	#region Add
	/// <summary>
	/// 在指定索引位置添加子节点。
	/// </summary>
	/// <param name="index">要添加子节点的索引位置。</param>
	/// <param name="child">要添加的子节点。</param>
	/// <returns>添加的子节点。</returns> 
	public virtual TreeNodeBase<T> AddAt(Index index, [NotNull] TreeNodeBase<T> node)  
	{
		var offset = ThrowIfArgumentOutOfRange(index, true);
		ValidateElement(node);
		if (offset == Count)
		{
			Children.Add(node);
		}
		else
		{
			Children.Insert(offset, node);
		}
		node.Parent = this;
		UpdateIndex(index..);
		return node;
	}
	/// <summary>
	/// 添加子节点，并递归构建子树。
	/// </summary>
	/// <param name="index">添加位置</param>
	/// <param name="value">值</param>
	/// <param name="childrenFactory">创建子值方法</param>
	/// <returns>直接子节点</returns> 
	public abstract TreeNodeBase<T> AddAt(Index index, T value, Func<T, IEnumerable<T>> childrenFactory); 
 
	#endregion
}