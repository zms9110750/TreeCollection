using zms9110750.TreeCollection.Abstract;
using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

/// <summary>
/// 有序树索引器和查询操作的语义测试
/// </summary>
public class TreeNodeQueryTests
{
    #region 索引器

    [Fact]
    public void IndexerGetReturnsCorrectNode()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        Assert.Same(a, root[0]);
    }

    [Fact]
    public void IndexerSetNullRemovesNode()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root[1] = null!;
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void IndexerSetAtIndexCountAppends()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root[root.Count] = new TreeNode<string>("c");
        Assert.Equal(3, root.Count);
    }

    [Fact]
    public void IndexerSetExistingChildMovesIt()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root[2] = root[0]; // move a to position 2
        Assert.Equal(["b", "c", "a"], root.Select(n => n.Value));
    }

    [Fact]
    public void IndexerSetNewChildReplaces()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root[1] = new TreeNode<string>("c");
        Assert.Equal(["a", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void IndexerOutOfRangeThrows()
    {
        var root = new TreeNode<string>("root");
        Assert.Throws<ArgumentOutOfRangeException>(() => root[5]);
    }

    #endregion

    #region 查询

    [Fact]
    public void ContainsByValueReturnsTrueIfExists()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.True(root.Contains("b"));
        Assert.False(root.Contains("z"));
    }

    [Fact]
    public void IndexOfByValueReturnsFirstIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "a");
        Assert.Equal(0, root.IndexOf("a"));
        Assert.Equal(-1, root.IndexOf("z"));
    }

    [Fact]
    public void LastIndexOfByValueReturnsLastIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "a");
        Assert.Equal(2, root.LastIndexOf("a"));
        Assert.Equal(-1, root.LastIndexOf("z"));
    }

    #endregion

    #region 枚举

    [Fact]
    public void EnumTreeFlatYieldsPreOrder()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var nodes = root.EnumTree().Select(n => n.Value).ToList();
        Assert.Equal(["root", "a", "b", "c"], nodes);
    }

    [Fact]
    public void EnumTreeNestedYieldsPreOrder()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        a.Add("a1", "a2");
        b.Add("b1");
        var vals = root.EnumTree().Select(n => n.Value).ToList();
        Assert.Equal(["root", "a", "a1", "a2", "b", "b1"], vals);
    }

    [Fact]
    public void IterateToRootFromRootReturnsSelf()
    {
        var root = new TreeNode<string>("root");
        Assert.Single(root.IterateToRoot());
        Assert.Same(root, root.IterateToRoot().First());
    }

    [Fact]
    public void IterateToRootFromDeepNodeReturnsPath()
    {
        var root = new TreeNode<string>("root");
        var l1 = root.Add("l1");
        var l2 = l1.Add("l2");
        var path = l2.IterateToRoot().Select(n => n.Value).ToList();
        Assert.Equal(["l2", "l1", "root"], path);
    }

    [Fact]
    public void CopyToCopiesChildrenToArray()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        var arr = new TreeNode<string>[2];
        root.CopyTo(arr, 0);
        Assert.Equal("a", arr[0].Value);
        Assert.Equal("b", arr[1].Value);
    }

    #endregion

    #region ToString

    [Fact]
    public void ToStringContainsValue()
    {
        var root = new TreeNode<string>("root");
        Assert.Contains("root", root.ToString());
    }

    #endregion

    #region 异常处理

    [Fact]
    public void AddAncestorThrows()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        var grandchild = child.Add("grandchild");
        Assert.Throws<ArgumentException>(() => grandchild.AddAt(0, root));
        Assert.Throws<ArgumentException>(() => child.AddAt(0, root));
    }

    [Fact]
    public void AddNullThrows()
    {
        var root = new TreeNode<string>("root");
        Assert.Throws<ArgumentNullException>(() => root.AddAt(0, (TreeNode<string>)null!));
    }

    #endregion
}
