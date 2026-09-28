Imports DijkstraSimulator.Core
Imports Xunit

Public Class GraphTests

    <Fact>
    Public Sub Edges_are_undirected()
        Dim g As New Graph(3)
        g.SetEdge(0, 2, 5)

        Assert.Equal(5, g.GetWeight(0, 2))
        Assert.Equal(5, g.GetWeight(2, 0))
        Assert.False(g.HasEdge(0, 1))
    End Sub

    <Fact>
    Public Sub Setting_an_existing_edge_changes_its_weight()
        Dim g As New Graph(2)
        g.SetEdge(0, 1, 5)
        g.SetEdge(1, 0, 9)

        Assert.Equal(9, g.GetWeight(0, 1))
        Assert.Single(g.Edges())
    End Sub

    <Fact>
    Public Sub Removing_an_edge_disconnects_both_ends()
        Dim g As New Graph(2)
        g.SetEdge(0, 1, 5)
        g.RemoveEdge(1, 0)

        Assert.False(g.HasEdge(0, 1))
        Assert.Empty(g.Edges())
    End Sub

    <Fact>
    Public Sub Rejects_self_loops_and_non_positive_weights()
        Dim g As New Graph(2)
        Assert.Throws(Of ArgumentException)(Sub() g.SetEdge(1, 1, 3))
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() g.SetEdge(0, 1, 0))
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() g.SetEdge(0, 2, 1))
    End Sub

    <Fact>
    Public Sub Edges_lists_each_edge_once_in_order()
        Dim g As New Graph(4)
        g.SetEdge(3, 1, 2)
        g.SetEdge(0, 2, 7)

        Dim edges = g.Edges()
        Assert.Equal(2, edges.Count)
        Assert.Equal((0, 2, 7), (edges(0).A, edges(0).B, edges(0).Weight))
        Assert.Equal((1, 3, 2), (edges(1).A, edges(1).B, edges(1).Weight))
    End Sub

    <Fact>
    Public Sub Shrinking_drops_edges_to_removed_nodes_but_keeps_the_rest()
        Dim g As New Graph(4)
        g.SetEdge(0, 1, 2)
        g.SetEdge(2, 3, 4)
        g.RemoveLastNode()

        Assert.Equal(3, g.NodeCount)
        Assert.Equal(2, g.GetWeight(0, 1))
        Assert.Single(g.Edges())
    End Sub

    <Fact>
    Public Sub Growing_keeps_existing_edges()
        Dim g As New Graph(2)
        g.SetEdge(0, 1, 3)
        g.AddNode()

        Assert.Equal(3, g.NodeCount)
        Assert.Equal(3, g.GetWeight(1, 0))
        Assert.False(g.HasEdge(0, 2))
    End Sub

    <Fact>
    Public Sub Node_count_is_limited_to_the_alphabet()
        Assert.Throws(Of ArgumentOutOfRangeException)(Function() New Graph(0))
        Assert.Throws(Of ArgumentOutOfRangeException)(Function() New Graph(Graph.MaxNodes + 1))
        Dim g As New Graph(1)
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() g.RemoveLastNode())
    End Sub

    <Fact>
    Public Sub Clearing_keeps_the_nodes()
        Dim g As New Graph(3)
        g.SetEdge(0, 1, 1)
        g.ClearEdges()

        Assert.Equal(3, g.NodeCount)
        Assert.Empty(g.Edges())
    End Sub

    <Fact>
    Public Sub Changes_raise_an_event_but_no_ops_do_not()
        Dim g As New Graph(3)
        Dim count = 0
        AddHandler g.Changed, Sub() count += 1

        g.SetEdge(0, 1, 2)
        g.SetEdge(0, 1, 2) ' same weight
        g.RemoveEdge(1, 2) ' no such edge
        g.Resize(3)        ' same size
        g.AddNode()

        Assert.Equal(2, count)
    End Sub

    <Fact>
    Public Sub Random_graphs_are_connected()
        Dim random As New Random(7)
        For trial = 1 To 50
            Dim g As New Graph(random.Next(2, 16))
            g.Randomise(random, maxWeight:=10, extraEdgeChance:=0)

            Assert.Equal(g.NodeCount - 1, g.Edges().Count)
            For target = 1 To g.NodeCount - 1
                Assert.True(FindShortestPath(g, 0, target).Found)
            Next
        Next
    End Sub

    <Theory>
    <InlineData(0, "A")>
    <InlineData(4, "E")>
    <InlineData(25, "Z")>
    Public Sub Nodes_are_named_by_letter(index As Integer, expected As String)
        Assert.Equal(expected, Graph.NodeName(index))
    End Sub

    <Fact>
    Public Sub Layout_starts_on_the_left_and_goes_clockwise()
        Dim points = CircleLayout.Positions(4, 100, 100, 50)

        AssertPoint(50, 100, points(0))  ' left
        AssertPoint(100, 50, points(1))  ' top
        AssertPoint(150, 100, points(2)) ' right
        AssertPoint(100, 150, points(3)) ' bottom
    End Sub

    <Fact>
    Public Sub Distance_to_segment_handles_ends_and_middle()
        Assert.Equal(5, CircleLayout.DistanceToSegment(5, 5, 0, 0, 10, 0), 6)
        Assert.Equal(5, CircleLayout.DistanceToSegment(-3, 4, 0, 0, 10, 0), 6)
        Assert.Equal(0, CircleLayout.DistanceToSegment(2, 2, 2, 2, 2, 2), 6)
    End Sub

    <Fact>
    Public Sub Weight_labels_on_crossing_diagonals_do_not_overlap()
        ' A square with both diagonals: A–C and B–D both have their middle at the centre
        Dim g As New Graph(4)
        g.SetEdge(0, 1, 9)
        g.SetEdge(1, 2, 4)
        g.SetEdge(2, 3, 2)
        g.SetEdge(3, 0, 7)
        g.SetEdge(0, 2, 1)
        g.SetEdge(1, 3, 11)
        Dim points = CircleLayout.Positions(4, 300, 300, 200)

        Dim labels = CircleLayout.WeightLabelPositions(g.Edges(), points, 30)

        Assert.Equal(6, labels.Count)
        For i = 0 To labels.Count - 1
            For j = i + 1 To labels.Count - 1
                Dim gap = Math.Sqrt((labels(i).X - labels(j).X) ^ 2 + (labels(i).Y - labels(j).Y) ^ 2)
                Assert.True(gap >= 30, $"Labels {i} and {j} are only {gap:0.0} apart")
            Next
        Next
    End Sub

    <Fact>
    Public Sub Weight_labels_stay_in_the_middle_when_there_is_room()
        Dim g As New Graph(3)
        g.SetEdge(0, 1, 5)
        Dim points = CircleLayout.Positions(3, 0, 0, 100)

        Dim label = CircleLayout.WeightLabelPositions(g.Edges(), points, 30).Single()

        AssertPoint((points(0).X + points(1).X) / 2, (points(0).Y + points(1).Y) / 2, label)
    End Sub

    Private Shared Sub AssertPoint(x As Double, y As Double, actual As (X As Double, Y As Double))
        Assert.Equal(x, actual.X, 6)
        Assert.Equal(y, actual.Y, 6)
    End Sub

End Class
