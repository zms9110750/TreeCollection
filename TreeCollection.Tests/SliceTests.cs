using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

public class SliceTests
{
    // ===================== 基本切片操作 =====================
    [Fact]
    public void Slice_ReturnsCorrectRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4];

        Assert.Equal(3, slice.Count);
        Assert.Equal("b", slice[0].Value);
        Assert.Equal("c", slice[1].Value);
        Assert.Equal("d", slice[2].Value);
    }

    [Fact]
    public void Slice_Empty_WhenRangeIsEmpty()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        var slice = root[1..1];
        Assert.Empty(slice);
    }

    // ===================== IsValid =====================
    [Fact]
    public void Slice_IsValid_InitiallyTrue()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[..2];
        Assert.True(slice.IsValid);
    }

    [Fact]
    public void Slice_IsInvalid_AfterParentModification()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[..2];
        root.Add("d");
        Assert.False(slice.IsValid);
    }

    [Fact]
    public void Slice_Throws_WhenAccessingAfterInvalidation()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        var slice = root[..1];
        root.Add("c");
        Assert.Throws<InvalidOperationException>(() => slice[0]);
        Assert.Throws<InvalidOperationException>(() => slice.Count);
    }

    // ===================== 切片内 Remove =====================
    [Fact]
    public void Slice_Remove_ByValue()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4]; // b, c, d

        var removed = slice.Remove("c");

        Assert.NotNull(removed);
        Assert.Equal("c", removed!.Value);
        Assert.Equal(4, root.Count);
        Assert.Equal(["a", "b", "d", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void Slice_Remove_ValueNotInSlice_ReturnsNull()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[1..2]; // b only
        Assert.Null(slice.Remove("a"));
        Assert.Null(slice.Remove("c"));
    }

    // ===================== 切片内 RemoveAll =====================
    [Fact]
    public void Slice_RemoveAll_WithPredicate()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4]; // b, c, d

        int removed = slice.RemoveAll(n => string.Compare(n.Value, "c", StringComparison.Ordinal) >= 0);
        // removes c(≥), d(≥) → removes 2

        Assert.Equal(2, removed);
        Assert.Equal(3, root.Count);
        Assert.Equal(["a", "b", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void Slice_RemoveAll_NullPredicate_RemovesAllInSlice()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        var slice = root[1..3]; // b, c

        int removed = slice.RemoveAll(null);

        Assert.Equal(2, removed);
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "d"], root.Select(n => n.Value));
    }

    // ===================== 切片内查询 =====================
    [Fact]
    public void Slice_Contains_OnlyWithinRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        var slice = root[1..3]; // b, c

        Assert.True(slice.Contains("b"));
        Assert.True(slice.Contains("c"));
        Assert.False(slice.Contains("a"));
        Assert.False(slice.Contains("d"));
    }

    [Fact]
    public void Slice_IndexOf_ReturnsAbsoluteIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        var slice = root[1..3]; // b, c

        Assert.Equal(1, slice.IndexOf("b"));
        Assert.Equal(2, slice.IndexOf("c"));
        Assert.Equal(-1, slice.IndexOf("a"));
    }

    [Fact]
    public void Slice_LastIndexOf_ReturnsAbsoluteIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "b", "d");
        var slice = root[1..4]; // b, c, b

        Assert.Equal(3, slice.LastIndexOf("b")); // last b in slice is at absolute index 3
    }

    // ===================== RotateForward =====================
    [Fact]
    public void Slice_RotateForward()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4]; // b, c, d

        slice.RotateForward(); // → c, d, b

        Assert.Equal(["a", "c", "d", "b", "e"], root.Select(n => n.Value));
    }

    // ===================== RotateBackward =====================
    [Fact]
    public void Slice_RotateBackward()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        var slice = root[1..4]; // b, c, d

        slice.RotateBackward(); // → d, b, c

        Assert.Equal(["a", "d", "b", "c", "e"], root.Select(n => n.Value));
    }

    // ===================== 单元素切片 =====================
    [Fact]
    public void Slice_SingleElement_RotateDoesNothing()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[1..2]; // b only

        slice.RotateForward();
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));

        slice.RotateBackward();
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));
    }

    // ===================== 整树切片 =====================
    [Fact]
    public void Slice_EntireRange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var slice = root[0..3];

        Assert.Equal(3, slice.Count);
        Assert.Equal("a", slice[0].Value);
        Assert.Equal("c", slice[2].Value);
    }

    // ===================== 枚举器 =====================
    [Fact]
    public void Slice_Enumerable_Works()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");
        var slice = root[1..3];
        var enumerable = (IEnumerable<TreeNode<string>>)slice;

        Assert.Equal(["y", "z"], enumerable.Select(n => n.Value));
    }

    // ===================== MoveChild 边界 =====================
    [Fact]
    public void MoveChild_Backward()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        root.MoveChild(3, 1); // d → position 1

        Assert.Equal(["a", "d", "b", "c", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void MoveChild_Forward()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d", "e");
        root.MoveChild(1, 3); // b → position 3

        Assert.Equal(["a", "c", "d", "b", "e"], root.Select(n => n.Value));
    }

    [Fact]
    public void MoveChild_ToStart()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(2, 0); // c → position 0

        Assert.Equal(["c", "a", "b"], root.Select(n => n.Value));
    }

    [Fact]
    public void MoveChild_ToEnd()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(0, 2); // a → position 2

        Assert.Equal(["b", "c", "a"], root.Select(n => n.Value));
    }

    [Fact]
    public void MoveChild_InvalidIndex_Throws()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.Throws<ArgumentOutOfRangeException>(() => root.MoveChild(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => root.MoveChild(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => root.MoveChild(5, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => root.MoveChild(0, 5));
    }
}
