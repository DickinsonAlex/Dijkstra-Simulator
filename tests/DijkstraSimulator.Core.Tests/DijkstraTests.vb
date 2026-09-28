Imports DijkstraSimulator.Core
Imports Xunit

Public Class DijkstraTests

    ' A ─4─ B ─1─ C
    ' │           │
    ' 1           1
    ' │           │
    ' D ────7──── E        F (not connected)
    Private Shared Function SampleGraph() As Graph
        Dim g As New Graph(6)
        g.SetEdge(0, 1, 4) ' A-B
        g.SetEdge(1, 2, 1) ' B-C
        g.SetEdge(0, 3, 1) ' A-D
        g.SetEdge(3, 4, 7) ' D-E
        g.SetEdge(2, 4, 1) ' C-E
        Return g
    End Function

    <Fact>
    Public Sub Finds_the_shortest_route_rather_than_the_fewest_edges()
        Dim result = FindShortestPath(SampleGraph(), 0, 4)

        Assert.True(result.Found)
        Assert.Equal({0, 1, 2, 4}, result.Route)
        Assert.Equal(6, result.Distance)
        Assert.Equal("A → B → C → E (distance 6)", result.Describe())
    End Sub

    <Fact>
    Public Sub Works_in_either_direction()
        Dim result = FindShortestPath(SampleGraph(), 4, 0)

        Assert.Equal({4, 2, 1, 0}, result.Route)
        Assert.Equal(6, result.Distance)
    End Sub

    <Fact>
    Public Sub Reports_when_there_is_no_route()
        Dim result = FindShortestPath(SampleGraph(), 0, 5)

        Assert.False(result.Found)
        Assert.Empty(result.Route)
        Assert.Equal(Unreached, result.Distance)
        Assert.Equal("No route from A to F.", result.Describe())
    End Sub

    <Fact>
    Public Sub A_node_is_zero_distance_from_itself()
        Dim result = FindShortestPath(SampleGraph(), 2, 2)

        Assert.Equal({2}, result.Route)
        Assert.Equal(0, result.Distance)
        Assert.Single(result.Steps)
    End Sub

    <Fact>
    Public Sub Settles_nodes_in_order_of_distance_and_stops_at_the_target()
        Dim result = FindShortestPath(SampleGraph(), 0, 4)

        ' A(0), D(1), B(4), C(5), E(6)
        Assert.Equal({0, 3, 1, 2, 4}, result.Steps.Select(Function(s) s.Settled))
        Assert.Equal(Unreached, result.Steps.Last().Distances(5))
    End Sub

    <Fact>
    Public Sub Steps_record_the_table_as_it_was_at_the_time()
        Dim result = FindShortestPath(SampleGraph(), 0, 4)
        Dim afterA = result.Steps(0)
        Dim afterD = result.Steps(1)

        ' After settling A, E hasn't been reached yet…
        Assert.Equal(Unreached, afterA.Distances(4))
        Assert.Equal({1, 3}, afterA.Updated)
        ' …then D reaches it with a distance of 8, which C later improves to 6
        Assert.Equal(8, afterD.Distances(4))
        Assert.Equal(3, afterD.Previous(4))
        Assert.Equal(6, result.Steps.Last().Distances(4))
        Assert.Equal(2, result.Steps.Last().Previous(4))
    End Sub

    <Fact>
    Public Sub Knows_which_edges_the_route_uses()
        Dim result = FindShortestPath(SampleGraph(), 0, 4)

        Assert.True(result.UsesEdge(1, 0))
        Assert.True(result.UsesEdge(2, 4))
        Assert.False(result.UsesEdge(0, 3))
    End Sub

    <Theory>
    <InlineData(1, "A → C (distance 1)")>
    <InlineData(11, "A → D → C (distance 9)")>
    Public Sub Takes_a_direct_edge_only_when_it_is_shorter(directWeight As Integer, expected As String)
        ' A square A-B-C-D with both diagonals, as drawn in the simulator
        Dim g As New Graph(4)
        g.SetEdge(0, 1, 9)
        g.SetEdge(1, 2, 4)
        g.SetEdge(2, 3, 2)
        g.SetEdge(3, 0, 7)
        g.SetEdge(1, 3, 11)
        g.SetEdge(0, 2, directWeight)

        Assert.Equal(expected, FindShortestPath(g, 0, 2).Describe())
    End Sub

    <Fact>
    Public Sub Rejects_nodes_outside_the_graph()
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() FindShortestPath(SampleGraph(), 0, 6))
        Assert.Throws(Of ArgumentOutOfRangeException)(Sub() FindShortestPath(SampleGraph(), -1, 0))
    End Sub

    <Fact>
    Public Sub Matches_a_brute_force_search_on_random_graphs()
        Dim random As New Random(2022)
        For trial = 1 To 200
            Dim g As New Graph(random.Next(2, 9))
            g.Randomise(random, maxWeight:=20, extraEdgeChance:=0.3)
            If random.NextDouble() < 0.3 Then g.RemoveEdge(0, 1)

            Dim source = random.Next(g.NodeCount)
            Dim target = random.Next(g.NodeCount)
            Dim result = FindShortestPath(g, source, target)

            Assert.Equal(BruteForce(g, source, target), result.Distance)
            If result.Found Then
                Assert.Equal(source, result.Route.First())
                Assert.Equal(target, result.Route.Last())
                Dim total = 0
                For i = 0 To result.Route.Count - 2
                    total += g.GetWeight(result.Route(i), result.Route(i + 1))
                Next
                Assert.Equal(result.Distance, total)
            End If
        Next
    End Sub

    ''' <summary>Tries every simple path: slow, but obviously correct.</summary>
    Private Shared Function BruteForce(g As Graph, source As Integer, target As Integer) As Integer
        Dim best = Unreached
        Dim visited(g.NodeCount - 1) As Boolean
        Dim walk As Action(Of Integer, Integer) = Nothing
        walk = Sub(node, soFar)
                   If node = target Then
                       best = Math.Min(best, soFar)
                       Return
                   End If
                   visited(node) = True
                   For nextNode = 0 To g.NodeCount - 1
                       If Not visited(nextNode) AndAlso g.HasEdge(node, nextNode) Then
                           walk(nextNode, soFar + g.GetWeight(node, nextNode))
                       End If
                   Next
                   visited(node) = False
               End Sub
        walk(source, 0)
        Return best
    End Function

End Class
