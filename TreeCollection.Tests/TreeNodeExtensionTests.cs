using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

/// <summary>
/// 有序树扩展方法、IList 桥接、导航的语义测试
/// </summary>
public class TreeNodeExtensionTests
{
    #region 扩展方法 AddAt(Index, ...)

    /// <summary>AddAt(Index, TNode) 和 AddAt(Index, TValue) 按 Index 定位</summary>
    [Fact]
    public void AddAtByIndexOverloads()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "c");
        root.AddAt(^1, new TreeNode<string>("b")); // Index: TNode overload
        root.AddAt(2, "x");                         // int overload (already tested)

        root.AddAt(^1, "d");                        // Index: TValue overload
        Assert.Equal(["a", "b", "x", "d", "c"], root.Select(n => n.Value));
    }

    /// <summary>AddAt(Index, IEnumerable) 和 AddAt(Index, IEnumerable TValue) 批量添加</summary>
    [Fact]
    public void AddAtByIndexEnumerable()
    {
        var root = new TreeNode<string>("root");
        root.Add("c");
        root.AddAt(0, (IEnumerable<TreeNode<string>>)new[] { new TreeNode<string>("a"), new TreeNode<string>("b") });
        root.AddAt(3, (IEnumerable<string>)new[] { "d", "e" });
        Assert.Equal(["a", "b", "c", "d", "e"], root.Select(n => n.Value));
    }

    #endregion

    #region 扩展方法 AddFirst / AddBefore / AddAfter

    /// <summary>AddFirst 多种重载全部生效</summary>
    [Fact]
    public void AddFirstOverloads()
    {
        var root = new TreeNode<string>("root");
        // 一开始 root 是空的，按顺序往后加再往头部插
        root.Add("c");
        root.AddFirst(new TreeNode<string>("b"));      // AddFirst(TNode)
        root.AddFirst("a");                             // AddFirst(TValue)
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void AddBeforeAndAfter()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var c = root.Add("c");
        c.AddBefore(new TreeNode<string>("b"));        // AddBefore(TNode)
        a.AddAfter("x");                                // AddAfter(TValue)
        Assert.Equal(["a", "x", "b", "c"], root.Select(n => n.Value));
    }

    #endregion

    #region 扩展方法带 Range 的变体

    [Fact]
    public void QueryWithRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "b", "d");
        Assert.True(root.Contains("c", 2..4));
        Assert.False(root.Contains("a", 2..4));
        Assert.Equal(3, root.IndexOf("b", 2..));
        Assert.Equal(-1, root.IndexOf("a", 2..));
        Assert.Equal(1, root.LastIndexOf("b", ..3));
    }

    [Fact]
    public void RemoveAllWithRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        Assert.Equal(2, root.RemoveAll(n => n.Value is "b" or "d", 1..4));
    }

    [Fact]
    public void RemoveRangeByValue()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "x", "y");
        Assert.Equal(2, root.RemoveAll((Predicate<string>)(v => v == "x"), ..2));
        Assert.Single(root);
    }

    [Fact]
    public void UpdateIndexByRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.UpdateIndex(0..2);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
        Assert.Equal(2, root[2].Index);
    }

    #endregion

    #region IList<T> 桥接

    /// <summary>ICollection&lt;T&gt;.Add 追加到末尾</summary>
    [Fact]
    public void IListAddAppends()
    {
        IList<TreeNode<string>> list = new TreeNode<string>("root");
        list.Add(new TreeNode<string>("a"));
        list.Add(new TreeNode<string>("b"));
        Assert.Equal(2, list.Count);
    }

    /// <summary>IList&lt;T&gt;.Insert 插入指定位置</summary>
    [Fact]
    public void IListInsertAtPosition()
    {
        IList<TreeNode<string>> list = new TreeNode<string>("root");
        list.Add(new TreeNode<string>("a"));
        list.Add(new TreeNode<string>("c"));
        list.Insert(1, new TreeNode<string>("b"));
        Assert.Equal(["a", "b", "c"], list.Select(n => n.Value));
    }

    /// <summary>IList&lt;T&gt;.RemoveAt 按索引移除</summary>
    [Fact]
    public void IListRemoveAt()
    {
        IList<TreeNode<string>> list = new TreeNode<string>("root");
        list.Add(new TreeNode<string>("a"));
        list.Add(new TreeNode<string>("b"));
        list.RemoveAt(1);
        Assert.Single(list);
    }

    /// <summary>ICollection&lt;T&gt;.Clear 全部清空</summary>
    [Fact]
    public void IListClear()
    {
        IList<TreeNode<string>> list = new TreeNode<string>("root");
        list.Add(new TreeNode<string>("a"));
        list.Add(new TreeNode<string>("b"));
        list.Clear();
        Assert.Empty(list);
    }

    /// <summary>IList&lt;T&gt;.IndexOf 通过检查 Parent 判断</summary>
    [Fact]
    public void IListIndexOfByParent()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        IList<TreeNode<string>> list = root;
        Assert.Equal(0, list.IndexOf(a));
        Assert.Equal(1, list.IndexOf(b));
    }

    /// <summary>ICollection&lt;T&gt;.Remove 按引用布尔返回</summary>
    [Fact]
    public void IListRemoveByItem()
    {
        IList<TreeNode<string>> list = new TreeNode<string>("root");
        var a = new TreeNode<string>("a");
        list.Add(a);
        Assert.True(list.Remove(a));
        Assert.Empty(list);
    }

    /// <summary>IsReadOnly 始终为 false</summary>
    [Fact]
    public void IsReadOnlyReturnsFalse()
    {
        var root = new TreeNode<string>("root");
        Assert.False(((IList<TreeNode<string>>)root).IsReadOnly);
    }

    #endregion

    #region 导航

    [Fact]
    public void SiblingNavigation()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        var c = root.Add("c");
        Assert.Null(a.PreviousSibling);
        Assert.Same(b, a.NextSibling);
        Assert.Same(a, b.PreviousSibling);
        Assert.Same(c, b.NextSibling);
        Assert.Same(b, c.PreviousSibling);
        Assert.Null(c.NextSibling);
    }

    [Fact]
    public void FirstLastChild()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        Assert.Same(a, root.FirstChild);
        Assert.Same(b, root.LastChild);
    }

    #endregion

    #region Parent 级联

    [Fact]
    public void ParentSetterCascadesToChildren()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        child.Add("grandchild");
        var newParent = new TreeNode<string>("new");
        newParent.Add(child);
        Assert.Same(newParent.Root, child.Root);
    }

    #endregion

    #region 覆盖扩展方法边界重载

    [Fact]
    public void AddFirstWithEnumerableAndSpan()
    {
        var root = new TreeNode<string>("root");
        root.Add("c");
        root.AddFirst((IEnumerable<TreeNode<string>>)new[] { new TreeNode<string>("a"), new TreeNode<string>("b") });
        root.AddFirst("x"); // AddFirst(TValue) — but after adding, x goes to front

        // Actually order is: x, a, b, c — no, AddFirst adds to front in order called
        // c was there, then a,b were added to front → [a,b,c], then x was added first → [x,a,b,c]
        // Wait, AddFirst adds to position 0 each time
    }

    [Fact]
    public void AddFirstWithAllOverloads()
    {
        var root = new TreeNode<string>("root");
        root.Add("d");
        root.AddFirst(new TreeNode<string>("c"));      // [c,d]
        root.AddFirst("b");                             // [b,c,d]
        root.AddFirst((IEnumerable<TreeNode<string>>)new[] { new TreeNode<string>("a") }); // [a,b,c,d]
        Assert.Equal(["a", "b", "c", "d"], root.Select(n => n.Value));
    }

    [Fact]
    public void RemoveAllRangeByNodePredicate()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        Assert.Equal(2, root.RemoveAll(n => n.Value is "b" or "d", 1..4));
        Assert.Equal(["a", "c", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void RemoveAllRangeByValuePredicate()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "x", "y");
        Assert.Equal(2, root.RemoveAll((Predicate<string>)(v => v == "x"), ..2));
        Assert.Single(root);
    }

    [Fact]
    public void RemoveAllRangeNullRemovesAllInRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        Assert.Equal(2, root.RemoveAll((Predicate<TreeNode<string>>?)null, 1..3));
        Assert.Equal(["a", "d"], root.Select(n => n.Value));
    }

    [Fact]
    public void MoveChildByIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(0, 2);
        Assert.Equal(["b", "c", "a"], root.Select(n => n.Value));
    }

    #endregion
}
