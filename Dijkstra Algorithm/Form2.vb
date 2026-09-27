Imports System.ComponentModel.Design.ObjectSelectorEditor
Imports System.Math
Public Class Form2

    Public SelectedNode As Button
    Public nodes As List(Of Node)
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        nodes = New List(Of Node)
    End Sub

    Private Sub relocateNodes()
        Dim nodeCount As Integer = nodes.Count()
        Dim mid As Point = New Point(Me.Width / 2 - 40, Me.Height / 2 - 40)
        Dim AngleSize As Double = (360 / nodeCount) * PI / 180
        Dim counter As Integer = 0
        For Each node As Node In nodes
            node.Location = New Point((mid.X / 3) * 4 + Cos(PI - AngleSize * counter) * Me.Height / 3, mid.Y - Sin(PI - AngleSize * counter) * Me.Height / 3)
            counter += 1
        Next
    End Sub

    Private Sub Node_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If SelectedNode IsNot Nothing Then
            If (SelectedNode.Text = sender.Text) Then
                SelectedNode = Nothing
            Else
                Dim surface As Graphics = CreateGraphics()
                Dim pen1 As Pen = New Pen(Color.Black, 2)

                Dim Loc1 As Point = SelectedNode.Location
                Loc1.X += SelectedNode.Width / 2
                Loc1.Y += SelectedNode.Height / 2
                Dim Loc2 As Point = sender.Location
                Loc2.X += SelectedNode.Width / 2
                Loc2.Y += SelectedNode.Height / 2

                Dim WeightLabel As New Label
                WeightLabel.Text = Weight.Value
                WeightLabel.Width = 20

                Dim NewLoc As Point
                NewLoc.X = (Loc1.X + Loc2.X) / 2
                NewLoc.Y = (Loc1.Y + Loc2.Y) / 2
                WeightLabel.Location = NewLoc
                Controls.Add(WeightLabel)

                surface.DrawLine(pen1, Loc1, Loc2)
                SelectedNode = Nothing
            End If
        Else
            SelectedNode = sender
        End If
    End Sub

    Private Sub AddNode_Click(sender As Object, e As EventArgs) Handles AddNode.Click
        Dim node As New Node(nodes.Count)
        AddHandler node.Click, AddressOf Node_Click
        Controls.Add(node)
        nodes.Add(node)
        relocateNodes()
    End Sub

    Private Sub RemoveNode_Click(sender As Object, e As EventArgs) Handles RemoveNode.Click
        nodes(nodes.Count - 1).Dispose()
        nodes.RemoveAt(nodes.Count - 1)
        relocateNodes()
    End Sub
End Class
