''' <summary>
''' An undirected, weighted graph stored as an adjacency matrix.
''' Nodes are numbered from 0 and named A, B, C… A weight of 0 means "no edge".
''' </summary>
Public NotInheritable Class Graph

    ''' <summary>The most nodes a graph can hold (one per letter of the alphabet).</summary>
    Public Const MaxNodes As Integer = 26

    Private _weights As Integer(,)
    Private _nodeCount As Integer

    ''' <summary>Raised whenever a node or edge is added, changed or removed.</summary>
    Public Event Changed As EventHandler

    Public Sub New(nodeCount As Integer)
        CheckNodeCount(nodeCount)
        _nodeCount = nodeCount
        _weights = New Integer(nodeCount - 1, nodeCount - 1) {}
    End Sub

    Public ReadOnly Property NodeCount As Integer
        Get
            Return _nodeCount
        End Get
    End Property

    ''' <summary>The letter used to label a node: 0 → "A", 1 → "B", and so on.</summary>
    Public Shared Function NodeName(index As Integer) As String
        If index < 0 OrElse index >= MaxNodes Then
            Throw New ArgumentOutOfRangeException(NameOf(index))
        End If
        Return ChrW(AscW("A"c) + index).ToString()
    End Function

    ''' <summary>The weight of the edge between two nodes, or 0 if they aren't connected.</summary>
    Public Function GetWeight(a As Integer, b As Integer) As Integer
        CheckNode(a, NameOf(a))
        CheckNode(b, NameOf(b))
        Return _weights(a, b)
    End Function

    Public Function HasEdge(a As Integer, b As Integer) As Boolean
        Return GetWeight(a, b) > 0
    End Function

    ''' <summary>Connects two different nodes, or changes the weight if they're already connected.</summary>
    Public Sub SetEdge(a As Integer, b As Integer, weight As Integer)
        CheckNode(a, NameOf(a))
        CheckNode(b, NameOf(b))
        If a = b Then Throw New ArgumentException("A node can't be connected to itself.")
        If weight <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(weight), "Weights must be positive.")
        If _weights(a, b) = weight Then Return
        _weights(a, b) = weight
        _weights(b, a) = weight
        OnChanged()
    End Sub

    Public Sub RemoveEdge(a As Integer, b As Integer)
        If Not HasEdge(a, b) Then Return
        _weights(a, b) = 0
        _weights(b, a) = 0
        OnChanged()
    End Sub

    ''' <summary>Every edge once, with the lower-numbered node first.</summary>
    Public Function Edges() As IReadOnlyList(Of Edge)
        Dim result As New List(Of Edge)
        For a = 0 To _nodeCount - 1
            For b = a + 1 To _nodeCount - 1
                If _weights(a, b) > 0 Then result.Add(New Edge(a, b, _weights(a, b)))
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' Changes the number of nodes. Edges between nodes that still exist are kept;
    ''' edges touching removed nodes are dropped.
    ''' </summary>
    Public Sub Resize(nodeCount As Integer)
        CheckNodeCount(nodeCount)
        If nodeCount = _nodeCount Then Return
        Dim resized = New Integer(nodeCount - 1, nodeCount - 1) {}
        Dim keep = Math.Min(nodeCount, _nodeCount)
        For a = 0 To keep - 1
            For b = 0 To keep - 1
                resized(a, b) = _weights(a, b)
            Next
        Next
        _weights = resized
        _nodeCount = nodeCount
        OnChanged()
    End Sub

    Public Sub AddNode()
        Resize(_nodeCount + 1)
    End Sub

    Public Sub RemoveLastNode()
        Resize(_nodeCount - 1)
    End Sub

    ''' <summary>Removes every edge but keeps the nodes.</summary>
    Public Sub ClearEdges()
        Array.Clear(_weights)
        OnChanged()
    End Sub

    ''' <summary>
    ''' Replaces the edges with a random connected graph: a random spanning tree
    ''' (so every node is reachable) plus a few extra edges.
    ''' </summary>
    Public Sub Randomise(random As Random, maxWeight As Integer, Optional extraEdgeChance As Double = 0.25)
        ArgumentNullException.ThrowIfNull(random)
        If maxWeight < 1 Then Throw New ArgumentOutOfRangeException(NameOf(maxWeight))
        Array.Clear(_weights)

        Dim order = Enumerable.Range(0, _nodeCount).OrderBy(Function(i) random.Next()).ToArray()
        For i = 1 To order.Length - 1
            Link(order(i), order(random.Next(i)), random.Next(1, maxWeight + 1))
        Next
        For a = 0 To _nodeCount - 1
            For b = a + 1 To _nodeCount - 1
                If _weights(a, b) = 0 AndAlso random.NextDouble() < extraEdgeChance Then
                    Link(a, b, random.Next(1, maxWeight + 1))
                End If
            Next
        Next
        OnChanged()
    End Sub

    Private Sub Link(a As Integer, b As Integer, weight As Integer)
        _weights(a, b) = weight
        _weights(b, a) = weight
    End Sub

    Private Sub OnChanged()
        RaiseEvent Changed(Me, EventArgs.Empty)
    End Sub

    Private Sub CheckNode(index As Integer, paramName As String)
        If index < 0 OrElse index >= _nodeCount Then
            Throw New ArgumentOutOfRangeException(paramName, $"There is no node {index} in a graph of {_nodeCount} nodes.")
        End If
    End Sub

    Private Shared Sub CheckNodeCount(count As Integer)
        If count < 1 OrElse count > MaxNodes Then
            Throw New ArgumentOutOfRangeException(NameOf(count), $"A graph needs between 1 and {MaxNodes} nodes.")
        End If
    End Sub

End Class

''' <summary>An undirected edge between two nodes.</summary>
Public Structure Edge
    Public ReadOnly Property A As Integer
    Public ReadOnly Property B As Integer
    Public ReadOnly Property Weight As Integer

    Public Sub New(a As Integer, b As Integer, weight As Integer)
        Me.A = a
        Me.B = b
        Me.Weight = weight
    End Sub

    Public Function Connects(x As Integer, y As Integer) As Boolean
        Return (A = x AndAlso B = y) OrElse (A = y AndAlso B = x)
    End Function
End Structure
