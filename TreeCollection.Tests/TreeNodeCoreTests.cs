using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

public class TreeNodeCoreTests
{
    // ===================== 创建 =====================
    [Fact]
    public void Create_Root_HasCorrectState()
    {
        var root = new TreeNode<string>("root");
        Assert.Equal("root", root.Value);
        Assert.Null(root.Parent);
        Assert.Same(root, root.Root);
        Assert.Equal(0, root.Depth);
        Assert.Equal(-1, root.Index);
        Assert.Empty(root);
    }

    // ===================== 添加子节点 =====================
    [Fact]
    public void Add_OneChild_IncreasesCount()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        Assert.Single(root);
        Assert.Same(root, child.Parent);
        Assert.Same(root, child.Root);
        Assert.Equal(1, child.Depth);
        Assert.Equal(0, child.Index);
    }

    [Fact]
    public void Add_MultipleChildren_HaveCorrectIndices()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.Equal(3, root.Count);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
        Assert.Equal(2, root[2].Index);
    }

    [Fact]
    public void AddAt_SpecificPosition_ShiftsOthers()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "d");
        root.AddAt(2, "c");
        Assert.Equal(["a", "b", "c", "d"], root.Select(n => n.Value));
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
        Assert.Equal(2, root[2].Index);
        Assert.Equal(3, root[3].Index);
    }

    [Fact]
    public void AddAt_End_Appends()
    {
        var root = new TreeNode<string>("root");
        root.Add("a");
        root.AddAt(root.Count, "b");
        Assert.Equal(2, root.Count);
        Assert.Equal("b", root[^1].Value);
    }

    [Fact]
    public void AddAt_IndexZero_Prepends()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root.AddAt(0, "c");
        Assert.Equal(["c", "a", "b"], root.Select(n => n.Value));
    }

    // ===================== 按值添加 =====================
    [Fact]
    public void Add_ByValue_CreatesNodeWithCorrectValue()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("hello");
        Assert.NotNull(child);
        Assert.Equal("hello", child.Value);
        Assert.Same(root, child.Parent);
    }

    [Fact]
    public void Add_ByValueBatch_CreatesAll()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");
        Assert.Equal(3, root.Count);
        Assert.Equal(["x", "y", "z"], root.Select(n => n.Value));
    }

    // ===================== 跨树移动 =====================
    [Fact]
    public void Move_NodeBetweenTrees_RemovesFromOld()
    {
        var root1 = new TreeNode<string>("r1");
        var root2 = new TreeNode<string>("r2");
        var child = root1.Add("child");

        root2.Add(child);

        Assert.Empty(root1);
        Assert.Single(root2);
        Assert.Same(root2, child.Parent);
        Assert.Equal(0, child.Index);
    }

    [Fact]
    public void Move_NodeToSameTreeDifferentParent_Works()
    {
        var root = new TreeNode<string>("root");
        var parent1 = root.Add("p1");
        var parent2 = root.Add("p2");
        var child = parent1.Add("child");

        parent2.Add(child);

        Assert.Empty(parent1);
        Assert.Single(parent2);
        Assert.Same(parent2, child.Parent);
    }

    [Fact]
    public void Move_NodeWithinSameParent_Repositions()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        var c = root.Add("c");

        root.MoveChild(2, 0); // c → 0

        Assert.Equal(["c", "a", "b"], root.Select(n => n.Value));
        Assert.Equal(0, c.Index);
        Assert.Equal(1, a.Index);
        Assert.Equal(2, b.Index);
    }

    // ===================== 索引器 =====================
    [Fact]
    public void Indexer_Get_ReturnsCorrectNode()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        Assert.Same(a, root[0]);
    }

    [Fact]
    public void Indexer_SetNull_RemovesNode()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root[1] = null!;
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void Indexer_SetAtIndexCount_Appends()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root[root.Count] = new TreeNode<string>("c");
        Assert.Equal(3, root.Count);
        Assert.Equal("c", root[2].Value);
    }

    [Fact]
    public void Indexer_Set_ReplacesNode()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        root[1] = new TreeNode<string>("c");
        Assert.Equal(["a", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void Indexer_SetExistingChild_MovesIt()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root[2] = root[0]; // move a to position 2
        Assert.Equal(["b", "c", "a"], root.Select(n => n.Value));
    }

    // ===================== 删除 =====================
    [Fact]
    public void Remove_ByReference_RemovesChild()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        var removed = root.Remove(child);
        Assert.Same(child, removed);
        Assert.Empty(root);
        Assert.Null(child.Parent);
        Assert.Equal(-1, child.Index);
    }

    [Fact]
    public void Remove_NonChild_ReturnsNull()
    {
        var root = new TreeNode<string>("root");
        var orphan = new TreeNode<string>("orphan");
        Assert.Null(root.Remove(orphan));
    }

    [Fact]
    public void Remove_ByValue_RemovesFirstMatch()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "a");
        var removed = root.Remove("a");
        Assert.NotNull(removed);
        Assert.Equal("a", removed!.Value);
        Assert.Equal(2, root.Count);
        Assert.Equal(["b", "a"], root.Select(n => n.Value));
    }

    [Fact]
    public void RemoveAt_ByIndex_RemovesCorrectNode()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var removed = root.RemoveAt(1);
        Assert.Equal("b", removed.Value);
        Assert.Equal(2, root.Count);
        Assert.Equal("a", root[0].Value);
        Assert.Equal("c", root[1].Value);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
    }

    [Fact]
    public void RemoveAll_NullPredicate_RemovesAll()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        int removed = root.RemoveAll(null);
        Assert.Equal(3, removed);
        Assert.Empty(root);
    }

    [Fact]
    public void RemoveAll_WithPredicate_RemovesMatching()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");
        int removed = root.RemoveAll(n => string.Compare(n.Value, "b", StringComparison.Ordinal) > 0);
        Assert.Equal(2, removed); // c, d
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "b"], root.Select(n => n.Value));
    }

    // ===================== 替换 =====================
    [Fact]
    public void Replace_Valid_Succeeds()
    {
        var root = new TreeNode<string>("root");
        var old = root.Add("old");
        var fresh = new TreeNode<string>("new");
        Assert.True(root.Replace(old, fresh));
        Assert.Equal("new", root[0].Value);
        Assert.Same(root, fresh.Parent);
        Assert.Equal(0, fresh.Index);
        Assert.Null(old.Parent);
    }

    [Fact]
    public void Replace_NonChild_ReturnsFalse()
    {
        var root = new TreeNode<string>("root");
        var orphan = new TreeNode<string>("orphan");
        Assert.False(root.Replace(orphan, new TreeNode<string>("x")));
    }

    [Fact]
    public void Replace_ExistingNewNode_Throws()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        Assert.Throws<ArgumentException>(() => root.Replace(a, b));
    }

    // ===================== 查询 =====================
    [Fact]
    public void Contains_ByValue_FindsMatch()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        Assert.True(root.Contains("b"));
        Assert.False(root.Contains("z"));
    }

    [Fact]
    public void IndexOf_ByValue_ReturnsFirstIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "a");
        Assert.Equal(0, root.IndexOf("a"));
    }

    [Fact]
    public void LastIndexOf_ByValue_ReturnsLastIndex()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "a");
        Assert.Equal(2, root.LastIndexOf("a"));
    }

    // ===================== 导航属性 =====================
    [Fact]
    public void Navigation_Siblings_Work()
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
    public void Navigation_FirstLastChild()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        Assert.Same(a, root.FirstChild);
        Assert.Same(b, root.LastChild);
    }

    // ===================== ToString 可视化 =====================
    [Fact]
    public void ToString_RootOnly()
    {
        var root = new TreeNode<string>("root");
        var str = root.ToString();
        Assert.Contains("root", str);
    }

    // ===================== 异常路径 =====================
    [Fact]
    public void AddAncestor_Throws()
    {
        var root = new TreeNode<string>("root");
        var child = root.Add("child");
        var grandchild = child.Add("grandchild");
        Assert.Throws<ArgumentException>(() => grandchild.AddAt(0, root));
        Assert.Throws<ArgumentException>(() => child.AddAt(0, root));
    }

    [Fact]
    public void AddNull_Throws()
    {
        var root = new TreeNode<string>("root");
        Assert.Throws<ArgumentNullException>(() => root.AddAt(0, (TreeNode<string>)null!));
    }

    [Fact]
    public void Indexer_OutOfRange_Throws()
    {
        var root = new TreeNode<string>("root");
        Assert.Throws<ArgumentOutOfRangeException>(() => root[5]);
    }

    // ===================== 空操作 =====================
    [Fact]
    public void EmptyTree_Enumeration_IsEmpty()
    {
        var root = new TreeNode<string>("root");
        Assert.Empty(root);
    }

    [Fact]
    public void RemoveFromEmptyTree_DoesNothing()
    {
        var root = new TreeNode<string>("root");
        Assert.Equal(0, root.RemoveAll(null));
    }

    // ===================== 值类型 =====================
    [Fact]
    public void ValueType_Works()
    {
        var root = new TreeNode<int>(42);
        Assert.Equal(42, root.Value);
        var child = root.Add(100);
        Assert.Equal(100, child.Value);
        Assert.Single(root);
    }

    // ===================== 复杂场景 =====================
    [Fact]
    public void DeepTree_IndicesCorrect()
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

    [Fact]
    public void RemoveAll_AndReAdd_Works()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.RemoveAll(null);
        Assert.Empty(root);

        root.Add("x", "y");
        Assert.Equal(2, root.Count);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
    }

    [Fact]
    public void BatchAdd_MultipleNodes()
    {
        var root = new TreeNode<string>("root");
        var nodes = new[] { new TreeNode<string>("a"), new TreeNode<string>("b") };
        root.AddAt(0, (IEnumerable<TreeNode<string>>)nodes);
        Assert.Equal(2, root.Count);
    }

    [Fact]
    public void MoveChild_SameIndex_NoChange()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        root.MoveChild(1, 1);
        Assert.Equal(["a", "b", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void Replace_MaintainsOtherIndices()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var newNode = new TreeNode<string>("x");
        root.Replace(root[1], newNode);
        Assert.Equal(0, root[0].Index);
        Assert.Equal(1, root[1].Index);
        Assert.Equal(2, root[2].Index);
        Assert.Equal(["a", "x", "c"], root.Select(n => n.Value));
    }

    [Fact]
    public void AddVertically_CreatesNestedChain()
    {
        var root = new TreeNode<string>("root");
        var result = root.AddVertically(0, "a", "b", "c");
        Assert.NotNull(result);
        Assert.Single(root);
        Assert.Equal("a", root[0].Value);
        Assert.Single(root[0]);
        Assert.Equal("b", root[0][0].Value);
        Assert.Single(root[0][0]);
        Assert.Equal("c", root[0][0][0].Value);
    }

    // ===================== EnumTree =====================
    [Fact]
    public void EnumTree_FlatTree_YieldsAllNodes()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");
        var nodes = root.EnumTree().ToList();
        Assert.Equal(4, nodes.Count); // root + a + b + c
        Assert.Equal("root", nodes[0].Value);
        Assert.Equal("a", nodes[1].Value);
        Assert.Equal("b", nodes[2].Value);
        Assert.Equal("c", nodes[3].Value);
    }

    [Fact]
    public void EnumTree_Nested_YieldsPreOrder()
    {
        var root = new TreeNode<string>("root");
        var a = root.Add("a");
        var b = root.Add("b");
        a.Add("a1");
        a.Add("a2");
        b.Add("b1");

        var nodes = root.EnumTree().ToList();
        Assert.Equal(6, nodes.Count);
        Assert.Equal("root", nodes[0].Value);
        Assert.Equal("a", nodes[1].Value);
        Assert.Equal("a1", nodes[2].Value);
        Assert.Equal("a2", nodes[3].Value);
        Assert.Equal("b", nodes[4].Value);
        Assert.Equal("b1", nodes[5].Value);
    }
}
