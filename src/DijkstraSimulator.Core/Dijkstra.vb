''' <summary>Dijkstra's shortest-path algorithm over a <see cref="Graph"/>.</summary>
''' <remarks>
''' Finding the closest unvisited node is a simple linear scan, so a search is O(V²).
''' With at most 26 nodes that's faster in practice than a priority queue.
''' </remarks>
Public Module Dijkstra

    ''' <summary>Used in distance tables for a node that hasn't been reached yet.</summary>
    Public Const Unreached As Integer = Integer.MaxValue

    ''' <summary>
    ''' Finds the shortest route from <paramref name="source"/> to <paramref name="target"/>,
    ''' recording a snapshot of the working table each time a node is settled.
    ''' </summary>
    Public Function FindShortestPath(graph As Graph, source As Integer, target As Integer) As PathResult
        ArgumentNullException.ThrowIfNull(graph)
        Dim n = graph.NodeCount
        If source < 0 OrElse source >= n Then Throw New ArgumentOutOfRangeException(NameOf(source))
        If target < 0 OrElse target >= n Then Throw New ArgumentOutOfRangeException(NameOf(target))

        Dim distance(n - 1) As Integer
        Dim previous(n - 1) As Integer
        Dim visited(n - 1) As Boolean
        For i = 0 To n - 1
            distance(i) = Unreached
            previous(i) = -1
        Next
        distance(source) = 0

        Dim steps As New List(Of SearchStep)

        Do
            ' Settle the closest node we haven't visited yet
            Dim current = -1
            For i = 0 To n - 1
                If Not visited(i) AndAlso distance(i) <> Unreached AndAlso
                   (current = -1 OrElse distance(i) < distance(current)) Then
                    current = i
                End If
            Next
            If current = -1 Then Exit Do ' everything reachable has been settled
            visited(current) = True

            ' Relax each edge out of it
            Dim updated As New List(Of Integer)
            If current <> target Then
                For neighbour = 0 To n - 1
                    Dim weight = graph.GetWeight(current, neighbour)
                    If weight > 0 AndAlso Not visited(neighbour) AndAlso distance(current) + weight < distance(neighbour) Then
                        distance(neighbour) = distance(current) + weight
                        previous(neighbour) = current
                        updated.Add(neighbour)
                    End If
                Next
            End If

            steps.Add(New SearchStep(current, updated, distance, previous, visited))
            If current = target Then Exit Do ' the target's distance is now final
        Loop

        If distance(target) = Unreached Then
            Return New PathResult(source, target, Array.Empty(Of Integer)(), Unreached, steps)
        End If

        ' Follow the previous-node links back from the target
        Dim route As New List(Of Integer)
        Dim node = target
        While node <> -1
            route.Insert(0, node)
            node = previous(node)
        End While
        Return New PathResult(source, target, route, distance(target), steps)
    End Function

End Module

''' <summary>The outcome of a search, including every step taken to get there.</summary>
Public NotInheritable Class PathResult
    Public ReadOnly Property Source As Integer
    Public ReadOnly Property Target As Integer
    ''' <summary>The nodes on the shortest route, from source to target. Empty if there's no route.</summary>
    Public ReadOnly Property Route As IReadOnlyList(Of Integer)
    ''' <summary>The total weight of the route, or <see cref="Dijkstra.Unreached"/>.</summary>
    Public ReadOnly Property Distance As Integer
    Public ReadOnly Property Steps As IReadOnlyList(Of SearchStep)

    Public Sub New(source As Integer, target As Integer, route As IReadOnlyList(Of Integer), distance As Integer, steps As IReadOnlyList(Of SearchStep))
        Me.Source = source
        Me.Target = target
        Me.Route = route
        Me.Distance = distance
        Me.Steps = steps
    End Sub

    Public ReadOnly Property Found As Boolean
        Get
            Return Route.Count > 0
        End Get
    End Property

    ''' <summary>A readable summary such as "A → C → D (distance 7)".</summary>
    Public Function Describe() As String
        If Not Found Then
            Return $"No route from {Graph.NodeName(Source)} to {Graph.NodeName(Target)}."
        End If
        Return $"{String.Join(" → ", Route.Select(AddressOf Graph.NodeName))} (distance {Distance})"
    End Function

    ''' <summary>True if the route travels along the edge between two nodes.</summary>
    Public Function UsesEdge(a As Integer, b As Integer) As Boolean
        For i = 0 To Route.Count - 2
            If (Route(i) = a AndAlso Route(i + 1) = b) OrElse (Route(i) = b AndAlso Route(i + 1) = a) Then Return True
        Next
        Return False
    End Function
End Class

''' <summary>A snapshot of the working table after one node has been settled.</summary>
Public NotInheritable Class SearchStep
    ''' <summary>The node settled in this step.</summary>
    Public ReadOnly Property Settled As Integer
    ''' <summary>Neighbours whose distance got shorter in this step.</summary>
    Public ReadOnly Property Updated As IReadOnlyList(Of Integer)
    Public ReadOnly Property Distances As IReadOnlyList(Of Integer)
    ''' <summary>The node each one was reached from, or -1.</summary>
    Public ReadOnly Property Previous As IReadOnlyList(Of Integer)
    Public ReadOnly Property Visited As IReadOnlyList(Of Boolean)

    Friend Sub New(settled As Integer, updated As IReadOnlyList(Of Integer), distances As Integer(), previous As Integer(), visited As Boolean())
        Me.Settled = settled
        Me.Updated = updated
        Me.Distances = CType(distances.Clone(), Integer())
        Me.Previous = CType(previous.Clone(), Integer())
        Me.Visited = CType(visited.Clone(), Boolean())
    End Sub
End Class
