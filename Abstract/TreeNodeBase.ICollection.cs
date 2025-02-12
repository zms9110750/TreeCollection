using System.Collections;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
	#region ICollection
	bool ICollection<TreeNodeBase<T>>.IsReadOnly => false;
	void ICollection<TreeNodeBase<T>>.Add(TreeNodeBase<T> item) => AddAt(^1, item);
	void ICollection<TreeNodeBase<T>>.Clear() => RemoveAll();
	bool ICollection<TreeNodeBase<T>>.Remove(TreeNodeBase<T> item) => Remove(item) != null;
	void IList<TreeNodeBase<T>>.Insert(int index, TreeNodeBase<T> item) => AddAt(index, item);
	void IList<TreeNodeBase<T>>.RemoveAt(int index) => RemoveAt(index);
	public IEnumerable<TreeNodeBase<T>> EnumTree()
	{
		var parent = Parent;
		var index = Index;
		yield return this;
		foreach (var item in FirstChild ?? Enumerable.Empty<TreeNodeBase<T>>())
		{
			if (parent != Parent)
			{
				break;
			}
			index = Index;
			yield return item;
		}
		if (parent != null && parent.Count > index)
		{
			foreach (var item in parent[index].EnumTree() ?? Enumerable.Empty<TreeNodeBase<T>>())
			{
				yield return item;
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); 
	#endregion
}