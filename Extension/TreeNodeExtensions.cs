using System.Diagnostics.CodeAnalysis;
using zms9110750.TreeCollection.Abstract;

namespace zms9110750.TreeCollection.Extension;
public static class TreeNodeExtensions
{
	public static TreeNodeBase<T> AddLast<T>(this TreeNodeBase<T> treeNode, TreeNodeBase<T> node)
	{
		return treeNode.AddAt(^1, node);
	}
	public static TreeNodeBase<T> AddFirst<T>(this TreeNodeBase<T> treeNode, TreeNodeBase<T> node)
	{
		return treeNode.AddAt(0, node);
	}
}
