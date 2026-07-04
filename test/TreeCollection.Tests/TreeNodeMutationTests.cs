using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

/// <summary>
/// 有序树添加、移动、替换、删除操作的语义测试
/// </summary>
public class TreeNodeMutationTests
{
    #region 添加

    /// <summary>AddAt 指定位置插入，后续节点右移</summary>
    [Fact]
    public void AddAtSpecificPositionShiftsOthers()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "d");
        root.AddAt(2, "c");
        Assert.Equal(["a", "b", "c", "d"], root.Select(n => n.Value));
    }

    /// <summary>AddAt 末尾追加 = Append</summary>
    [Fact]
    public void AddAtEndAppends()
    {
        var root = new TreeNode<string>("root");
        root.Add("a");
        root.AddAt(root.Count, "b");
        Assert.Equal(2, root.Count);
        Assert.Equal(1, root[1].Index);
        Assert.Equal("b", root[1].Value);
    }

    /// <summary>AddAt(0) = Prepend</summary>
    [Fact]
    public void AddAtZeroPrepends()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root.AddAt(0, "c");
        Assert.Equal(["c", "a", "b"], root.Select(n => n.Value));
    }

    /// <summary>Add(TValue) 按值创建节点</summary>
    [Fact]
    public void AddByValueCreatesNode()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("hello");
        Assert.Equal("hello", child.Value);
        Assert.Same(root, child.Parent);
    }

    /// <summary>Add 批量值</summary>
    [Fact]
    public void AddBatchValues()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");
        Assert.Equal(3, root.Count);
        Assert.Equal(["x", "y", "z"], root.Select(n => n.Value));
    }

    /// <summary>AddVertically 链式嵌套创建</summary>
    [Fact]
    public void AddVerticallyCreatesNestedChain()
    {
        var root = new TreeNode<string>("root");
        root.AddVertically(0, "a", "b", "c");
        Assert.Single(root);
        Assert.Equal("a", root[0].Value);
        Assert.Single(root[0]);
        Assert.Equal("b", root[0][0].Value);
        Assert.Single(root[0][0]);
        Assert.Equal("c", root[0][0][0].Value);
    }

    /// <summary>批量 Add 保持 Index 正确</summary>
    [Fact]
    public void BatchAddMultipleNodes()
    {
        var root = new TreeNode<string>("root");
        root.AddAt(0, (IEnumerable<TreeNode<string>>)new[] { new TreeNode<string>("a"), new TreeNode<string>("b") });
        Assert.Equal(2, root.Count);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
    }

    #endregion

    #region 移动

    /// <summary>MoveChild 向前移动（from &lt; to）</summary>
    [Fact]
    public void MoveChildForwardShiftsRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        root.MoveChild(1, 3);
        Assert.Equal(["a", "c", "d", "b", "e"], root.Select(n => n.Value));
    }

    /// <summary>MoveChild 向后移动（from &gt; to）</summary>
    [Fact]
    public void MoveChildBackwardShiftsRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        root.MoveChild(3, 1);
        Assert.Equal(["a", "d", "b", "c", "e"], root.Select(n => n.Value));
    }

    /// <summary>MoveChild 移动到开头</summary>
    [Fact]
    public void MoveChildToStart()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(2, 0);
        Assert.Equal(["c", "a", "b"], root.Select(n => n.Value));
    }

    /// <summary>MoveChild 移动到末尾</summary>
    [Fact]
    public void MoveChildToEnd()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(0, 2);
        Assert.Equal(["b", "c", "a"], root.Select(n => n.Value));
    }

    /// <summary>MoveChild 相同索引不产生变化</summary>
    [Fact]
    public void MoveChildSameIndexNoChange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(1, 1);
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));
    }

    /// <summary>MoveChild 索引越界抛出异常</summary>
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(5, 0)]
    [InlineData(0, 5)]
    public void MoveChildInvalidIndexThrows(int from, int to)
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.Throws<ArgumentOutOfRangeException>(() => root.MoveChild(from, to));
    }

    /// <summary>跨树移动节点，旧父节点列表不再包含该节点</summary>
    [Fact]
    public void MoveNodeBetweenTreesRemovesFromOld()
    {
        var root1 = new TreeNode<string>("r1");
        var root2 = new TreeNode<string>("r2");
        var child = root1.Add("child");
        root2.Add(child);
        Assert.Empty(root1);
        Assert.Single(root2);
        Assert.Same(root2, child.Parent);
    }

    /// <summary>跨父节点移动，旧父节点列表不再包含该节点</summary>
    [Fact]
    public void MoveNodeBetweenParents()
    {
        var root = new TreeNode<string>("root");
        var p1 = root.Add("p1");
        var p2 = root.Add("p2");
        var child = p1.Add("child");
        p2.Add(child);
        Assert.Empty(p1);
        Assert.Single(p2);
        Assert.Same(p2, child.Parent);
    }

    #endregion

    #region 替换

    /// <summary>Replace 成功后旧节点被孤立、新节点接入</summary>
    [Fact]
    public void ReplaceValidOldNodeIsOrphaned()
    {
        var root = new TreeNode<string>("root");
        var old = root.Add("old");
        var fresh = new TreeNode<string>("new");
        Assert.True(root.Replace(old, fresh));
        Assert.Equal("new", root[0].Value);
        Assert.Same(root, fresh.Parent);
        Assert.Null(old.Parent);
    }

    /// <summary>Replace 非子节点返回 false</summary>
    [Fact]
    public void ReplaceNonChildReturnsFalse()
    {
        var root = new TreeNode<string>("root");
        Assert.False(root.Replace(new TreeNode<string>("orphan"), new TreeNode<string>("x")));
    }

    /// <summary>Replace 新节点已在树中则抛出</summary>
    [Fact]
    public void ReplaceExistingNewNodeThrows()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        Assert.Throws<ArgumentException>(() => root.Replace(a, b));
    }

    /// <summary>Replace 后 Index 数据一致</summary>
    [Fact]
    public void ReplaceMaintainsIndices()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.Replace(root[1], new TreeNode<string>("x"));
        Assert.Equal(["a", "x", "c"], root.Select(n => n.Value));
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
        Assert.Equal(2, root[2].Index);
    }

    #endregion

    #region 删除

    /// <summary>Remove 按引用移除节点并孤立</summary>
    [Fact]
    public void RemoveByReferenceOrphansNode()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        var removed = root.Remove(child);
        Assert.Same(child, removed);
        Assert.Empty(root);
        Assert.Null(child.Parent);
        Assert.Equal(-1, child.Index);
    }

    /// <summary>Remove 非子节点返回 null</summary>
    [Fact]
    public void RemoveNonChildReturnsNull()
    {
        var root = new TreeNode<string>("root");
        Assert.Null(root.Remove(new TreeNode<string>("orphan")));
    }

    /// <summary>Remove(TValue) 删除第一个匹配的值</summary>
    [Fact]
    public void RemoveByValueRemovesFirstMatch()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "a");
        root.Remove("a");
        Assert.Equal(["b", "a"], root.Select(n => n.Value));
    }

    /// <summary>Remove(TValue) 找不到值返回 null</summary>
    [Fact]
    public void RemoveByValueNotFoundReturnsNull()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        Assert.Null(root.Remove("z"));
    }

    /// <summary>RemoveAt 按索引移除指定节点</summary>
    [Fact]
    public void RemoveAtByIndexCorrect()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var removed = root.RemoveAt(1);
        Assert.Equal("b", removed.Value);
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "c"], root.Select(n => n.Value));
    }

    /// <summary>RemoveAll(null) 全部移除</summary>
    [Fact]
    public void RemoveAllNullRemovesAll()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.Equal(3, root.RemoveAll(null));
        Assert.Empty(root);
    }

    /// <summary>RemoveAll 按谓词只移除匹配项</summary>
    [Fact]
    public void RemoveAllWithPredicateMatchesOnly()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        Assert.Equal(2, root.RemoveAll(n => string.Compare(n.Value, "b", StringComparison.Ordinal) > 0));
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "b"], root.Select(n => n.Value));
    }

    /// <summary>RemoveAll 无匹配返回 0</summary>
    [Fact]
    public void RemoveAllNoMatchReturnsZero()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        Assert.Equal(0, root.RemoveAll(n => n.Value == "z"));
    }

    /// <summary>RemoveAll 后重新 Add 仍正确</summary>
    [Fact]
    public void RemoveAllThenReAddWorks()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root.RemoveAll(null);
        root.Add("x", "y");
        Assert.Equal(2, root.Count);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
    }

    /// <summary>RemoveAll 时旧父节点 Parent 置 null</summary>
    [Fact]
    public void RemoveAllOrphansNodes()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        root.RemoveAll(null);
        Assert.Null(a.Parent);
        Assert.Null(b.Parent);
    }

    #endregion
}
