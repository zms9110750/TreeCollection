using zms9110750.TreeCollection.Ordered;
using zms9110750.TreeCollection.Abstract;

namespace TreeCollection.Tests;

/// <summary>
/// 有序树创建与基础状态测试
/// </summary>
public class TreeNodeCreationTests
{
    /// <summary>创建根节点，验证 Parent=null、Root=自己、Depth=0、Index=-1</summary>
    [Fact]
    public void RootHasCorrectState()
    {
        var root = new TreeNode<string>("root");
        Assert.Equal("root", root.Value);
        Assert.Null(root.Parent);
        Assert.Same(root, root.Root);
        Assert.Equal(0, root.Depth);
        Assert.Equal(-1, root.Index);
        Assert.Empty(root);
    }

    /// <summary>添加子节点后验证：Count、Parent、Root、Depth、Index 全部正确</summary>
    [Fact]
    public void AddOneChildUpdatesState()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        Assert.Single(root);
        Assert.Same(root, child.Parent);
        Assert.Same(root, child.Root);
        Assert.Equal(1, child.Depth);
        Assert.Equal(0, child.Index);
    }

    /// <summary>添加多个子节点后 Index 连续递增</summary>
    [Fact]
    public void AddMultipleChildrenIndicesAreSequential()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.Equal(3, root.Count);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
        Assert.Equal(2, root[2].Index);
    }

    /// <summary>深层树验证 Depth 和 Root 链</summary>
    [Fact]
    public void DeepTreeDepthAndRootAreCorrect()
    {
        var root = new TreeNode<string>("root");
        var l1 = root.Add("l1");
        var l2 = l1.Add("l2");
        var l3 = l2.Add("l3");
        Assert.Equal(0, root.Depth);
        Assert.Equal(1, l1.Depth);
        Assert.Equal(2, l2.Depth);
        Assert.Equal(3, l3.Depth);
        Assert.Same(root, l3.Root);
    }

    /// <summary>值类型作为 TValue 也能正常工作</summary>
    [Fact]
    public void ValueTypeTValueWorks()
    {
        var root = new TreeNode<int>(42);
        Assert.Equal(42, root.Value);
        var child = root.Add(100);
        Assert.Equal(100, child.Value);
        Assert.Single(root);
    }
}
