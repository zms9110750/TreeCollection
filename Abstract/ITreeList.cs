using System.Collections;

namespace zms9110750.TreeCollection.Abstract;

public interface ITreeList<T> : IList<T>
{
	/// <summary>
	/// 添加一堆项目到指定位置
	/// </summary>
	/// <param name="index">要放置的位置</param>
	/// <param name="item">要添加的项目</param>
	void AddAt(Index index, params IEnumerable<T> item);
	/// <summary>
	/// 添加一个项目到指定位置
	/// </summary>
	/// <param name="index">要放置位置</param>
	/// <param name="item">要添加的项目</param>
	/// <returns>添加进去的值</returns>
	T AddAt(Index index, T item);
	/// <summary>
	/// 查询项目是否存在于该列表中
	/// </summary>
	/// <param name="item">查询项目</param>
	/// <param name="range">查询范围</param>
	/// <returns>若存在则为true，否则为false。</returns>
	bool Contains(T item, Range? range = null);
	/// <summary>
	/// 查询项目在列表中的位置
	/// </summary>
	/// <param name="item">查找项目</param>
	/// <param name="range">查询范围</param>
	/// <returns>项目的索引。若不存在，则返回-1。</returns>
	int IndexOf(T item, Range? range = null);
	/// <summary>
	/// 移除第一个匹配的的项目
	/// </summary>
	/// <param name="item">查找项目</param>
	/// <param name="range">查询范围</param>
	/// <returns>被移除的项目。若没有找到，则返回null。</returns>
	T? Remove(T item, Range? range = null);
	/// <summary>
	/// 移除符合条件的项目
	/// </summary>
	/// <param name="match">条件</param>
	/// <param name="range">仅筛选以下范围</param>
	/// <returns>被移除的项目列表</returns>
	List<T> RemoveAll(Predicate<T>? match = null, Range? range = null);
	/// <summary>
	/// 移除指定位置的项目
	/// </summary>
	/// <param name="index">要移除的位置</param>
	/// <returns></returns>
	T RemoveAt(Index index);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	int IList<T>.IndexOf(T item) => IndexOf(item, default);

	void IList<T>.Insert(int index, T item) => AddAt(index, item);

	void IList<T>.RemoveAt(int index) => RemoveAt(index);

	void ICollection<T>.Add(T item) => AddAt(^0, item);

	void ICollection<T>.Clear() => RemoveAll();

	bool ICollection<T>.Contains(T item) => Contains(item, default);

	bool ICollection<T>.Remove(T item)
	{
		var index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}
}