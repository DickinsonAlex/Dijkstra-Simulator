Imports System.Math
Imports System.Reflection
Public Class Node
    Inherits Button
    Private Property connections As List(Of Dictionary(Of Char, Integer))
    Private Property nodeID As Integer

    Public Sub New(nodeCount As Integer)
        connections = New List(Of Dictionary(Of Char, Integer))
        nodeID = 0
        Name = Convert.ToChar(nodeCount + 64)
    End Sub

End Class
Public Class Form2
    Public nodes As List(Of Node)

    'Make a new node
    Public Sub d()
        Dim node As New Node(nodes.Count)
        nodes.Add(node)
    End Sub

    'Remove node
    Public Sub e()
        nodes(nodes.Count - 1).Dispose()
    End Sub

    'On start
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        nodes = New List(Of Node)
    End Sub
End Class
