using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

/// <summary>
/// 切片操作的语义测试。切片是父节点子列表的一个只读窗口，但支持自己的修改操作。
/// IndexOf/LastIndexOf 返回的是节点在父节点中的绝对索引。
/// </summary>
public class SliceTests
{
    #region 基本切片

    [Fact]
    public void SliceReturnsCorrectRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4];
        Assert.Equal(3, slice.Count);
        Assert.Equal(["b", "c", "d"], slice.Select(n => n.Value));
    }

    [Fact]
    public void EmptySlice()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        var slice = root[1..1];
        Assert.Empty(slice);
    }

    [Fact]
    public void SliceEntireRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[0..3];
        Assert.Equal(3, slice.Count);
    }

    #endregion

    #region IsValid

    [Fact]
    public void SliceIsValidInitiallyTrue()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.True(root[..2].IsValid);
    }

    [Fact]
    public void SliceIsInvalidAfterModification()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[..2];
        root.Add("d");
        Assert.False(slice.IsValid);
    }

    [Fact]
    public void SliceThrowsAfterInvalidation()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        var slice = root[..1];
        root.Add("c");
        Assert.Throws<InvalidOperationException>(() => slice[0]);
        Assert.Throws<InvalidOperationException>(() => slice.Count);
    }

    #endregion

    #region 切片内修改

    [Fact]
    public void SliceRemoveByValue()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4];
        var removed = slice.Remove("c");
        Assert.NotNull(removed);
        Assert.Equal(4, root.Count);
        Assert.Equal(["a", "b", "d", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void SliceRemoveValueNotFoundReturnsNull()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[1..2];
        Assert.Null(slice.Remove("a"));
    }

    [Fact]
    public void SliceRemoveAllWithPredicate()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4]; // b, c, d
        Assert.Equal(2, slice.RemoveAll(n => string.Compare(n.Value, "c", StringComparison.Ordinal) >= 0));
        Assert.Equal(3, root.Count);
        Assert.Equal(["a", "b", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void SliceRemoveAllNullRemovesAllInSlice()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        var slice = root[1..3];
        Assert.Equal(2, slice.RemoveAll(null));
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "d"], root.Select(n => n.Value));
    }

    [Fact]
    public void SliceRotateForward()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        root[1..4].RotateForward();
        Assert.Equal(["a", "c", "d", "b", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void SliceRotateBackward()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        root[1..4].RotateBackward();
        Assert.Equal(["a", "d", "b", "c", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void SingleElementSliceRotateDoesNothing()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[1..2];
        slice.RotateForward();
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));
        slice.RotateBackward();
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));
    }

    #endregion

    #region 切片内查询

    [Fact]
    public void SliceContainsOnlyWithinRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        var slice = root[1..3];
        Assert.True(slice.Contains("b"));
        Assert.True(slice.Contains("c"));
        Assert.False(slice.Contains("a"));
        Assert.False(slice.Contains("d"));
    }

    [Fact]
    public void SliceIndexOfReturnsAbsoluteIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        var slice = root[1..3];
        Assert.Equal(1, slice.IndexOf("b"));
        Assert.Equal(2, slice.IndexOf("c"));
        Assert.Equal(-1, slice.IndexOf("a"));
    }

    [Fact]
    public void SliceLastIndexOfReturnsAbsoluteIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "b", "d");
        var slice = root[1..4];
        Assert.Equal(3, slice.LastIndexOf("b"));
    }

    [Fact]
    public void SliceEnumerableWorks()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");
        var enumerable = (IEnumerable<TreeNode<string>>)root[1..3];
        Assert.Equal(["y", "z"], enumerable.Select(n => n.Value));
    }

    #endregion
}
