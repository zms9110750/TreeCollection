
namespace zms9110750.TreeCollection.Extension;
public static class TreeNodeExtensions
{
	public static TreeNode<T> AddLast<T>(this TreeNode<T> treeNode, TreeNode<T> node)
	{
		return treeNode.AddAt(^0, node);
	}
	public static TreeNode<T> AddFirst<T>(this TreeNode<T> treeNode, TreeNode<T> node)
	{
		return treeNode.AddAt(0, node);
	}
}
