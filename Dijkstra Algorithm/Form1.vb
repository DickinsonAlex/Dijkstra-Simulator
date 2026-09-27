Imports System.Math
Public Class Form1
    Public Nodes As List(Of Button) = New List(Of Button)
    Public NodeWeights As List(Of Label) = New List(Of Label)
    Public AdjacencyMatrix(2, 2) As Integer
    Public SelectedNode As Button
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Open form 2
        Dim f2 As New Form2
        f2.Show()
        AdjMatrix.Font = New Font("Lucida Console", 5 + Me.Height / 50)
        MakeNodes()
    End Sub
    Public Sub MakeNodes()
        For Each node As Button In Nodes
            node.Dispose()
        Next
        Nodes = New List(Of Button)
        Dim mid As Point = New Point(Me.Width / 2 - 40, Me.Height / 2 - 40)
        For x As Integer = 1 To Points.Value
            MakeButton(mid, x)
        Next
        PlaceNodes(mid)
    End Sub
    Public Sub PlaceNodes(mid)
        Dim Count As Integer = Nodes.Count
        Dim Counter As Integer = Nodes.Count
        Dim AngleSize As Double = (360 / Count) * PI / 180
        For Each node As Button In Nodes
            Counter -= 1
            node.Location = New Point((mid.x / 3) * 4 + Cos(PI + AngleSize * Counter) * Me.Height / 3, mid.y - Sin(PI + AngleSize * Counter) * Me.Height / 3)
        Next
    End Sub
    Public Sub MakeButton(point, x)
        Dim Alphabet As New List(Of String)({"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"})
        ' Create a Button object 
        Dim dynamicButton As New Button
        ' Set Button properties
        dynamicButton.Location = New Point(point.x, point.y)
        dynamicButton.Height = 40
        dynamicButton.Width = 40
        ' Set background and foreground
        dynamicButton.BackColor = Color.Red
        dynamicButton.ForeColor = Color.Red
        dynamicButton.FlatStyle = FlatStyle.System
        dynamicButton.Text = Alphabet(x - 1)
        dynamicButton.Name = "DynamicButton"
        dynamicButton.Font = New Font("Georgia", 16)
        AddHandler dynamicButton.Click, AddressOf DynamicButton_Click
        ' Add Button to the Form. Placement of the Button
        ' will be based on the Location and Size of button
        Controls.Add(dynamicButton)
        Nodes.Add(dynamicButton)
    End Sub
    Private Sub DynamicButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
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
                WeightLabel.Text = WeightValue.Value
                WeightLabel.Width = 20

                Dim NewLoc As Point
                NewLoc.X = (Loc1.X + Loc2.X) / 2
                NewLoc.Y = (Loc1.Y + Loc2.Y) / 2
                WeightLabel.Location = NewLoc
                NodeWeights.Add(WeightLabel)
                Controls.Add(WeightLabel)

                surface.DrawLine(pen1, Loc1, Loc2)
                UpdateAdjacencyMatrixValues(sender, SelectedNode)
                SelectedNode = Nothing
            End If
        Else
            SelectedNode = sender
        End If
    End Sub
    Private Sub UpdateAdjacencyMatrixValues(ToNode As Button, FromNode As Button)
        Dim Alphabet As New List(Of String)({"\", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"})
        Dim Fromindex As Integer = Alphabet.IndexOf(FromNode.Text)
        Dim Toindex As Integer = Alphabet.IndexOf(ToNode.Text)
        AdjacencyMatrix(Fromindex, Toindex) = WeightValue.Value
        AdjacencyMatrix(Toindex, Fromindex) = WeightValue.Value
        DisplayAdjacencyMatrix()
    End Sub
    Private Sub UpdateAdjacencyMatrix()
        Dim NewAdjacencyMatrix(Points.Value, Points.Value) As Integer
        For Each a As Integer In NewAdjacencyMatrix
            a = 0
        Next
        For y As Integer = 0 To (Sqrt(AdjacencyMatrix.Length) - 1)
            For x As Integer = 0 To (Sqrt(AdjacencyMatrix.Length) - 1)
                Try
                    NewAdjacencyMatrix(x, y) = AdjacencyMatrix(x, y)
                Catch
                End Try
            Next
        Next

        AdjacencyMatrix = NewAdjacencyMatrix
    End Sub
    Private Sub DisplayAdjacencyMatrix()
        Dim Alphabet As New List(Of String)({"\", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"})
        Dim Text As String = ""
        StartOption.Items.Clear()
        EndOption.Items.Clear()
        For y As Integer = 0 To (Sqrt(AdjacencyMatrix.Length) - 1)
            For x As Integer = 0 To (Sqrt(AdjacencyMatrix.Length) - 1)
                If (y = 0) Then
                    Text += Alphabet(x)
                    If Not (x = 0) Then
                        StartOption.Items.Add(Alphabet(x))
                        EndOption.Items.Add(Alphabet(x))
                    End If
                ElseIf (x = 0) Then
                    Text += Alphabet(y)
                Else
                    Text += AdjacencyMatrix(x, y).ToString
                End If
                Text += " "
            Next
            Text += vbCrLf
        Next
        AdjMatrix.Text = Text
        AdjMatrix.Location = New Point(10, Me.Height - AdjMatrix.Height - 50)
    End Sub
    Private Sub ClearButton_Click(sender As Object, e As EventArgs) Handles ClearButton.Click
        Me.Invalidate()
        For Each x As Label In NodeWeights
            x.Dispose()
        Next

        For y As Integer = 0 To (Sqrt(AdjacencyMatrix.Length) - 1)
            For x As Integer = 0 To (Sqrt(AdjacencyMatrix.Length) - 1)
                AdjacencyMatrix(x, y) = 0
            Next
        Next
        PathLbl.Text = ""
        For Each node As Button In Nodes
            node.BackColor = Color.Red
            node.ForeColor = Color.Red
        Next
        DisplayAdjacencyMatrix()
    End Sub

    Private Sub Points_ValueChanged(sender As Object, e As EventArgs) Handles Points.ValueChanged
        ClearButton_Click(sender, e)
        MakeNodes()
        UpdateAdjacencyMatrix()
        DisplayAdjacencyMatrix()
    End Sub

    Private Sub GetPath_Click(sender As Object, e As EventArgs) Handles GetPath.Click
        PathLbl.Text = ""
        ResetHighlights()
        If StartOption.SelectedIndex < 0 Or EndOption.SelectedIndex < 0 Then
            PathLbl.Text = "Pick a start and an end node."
            Return
        End If

        ' The matrix has a header row/column at index 0, so nodes are 1..n
        Dim n As Integer = Math.Min(CInt(Sqrt(AdjacencyMatrix.Length)) - 1, Nodes.Count)
        Dim source As Integer = StartOption.SelectedIndex + 1
        Dim target As Integer = EndOption.SelectedIndex + 1

        Dim distance(n) As Integer
        Dim previous(n) As Integer
        Dim visited(n) As Boolean
        For i As Integer = 1 To n
            distance(i) = Integer.MaxValue
            previous(i) = 0
            visited(i) = False
        Next
        distance(source) = 0

        ' Dijkstra: repeatedly settle the closest unvisited node, then relax its edges
        For iteration As Integer = 1 To n
            Dim current As Integer = 0
            For i As Integer = 1 To n
                If Not visited(i) And distance(i) <> Integer.MaxValue Then
                    If current = 0 OrElse distance(i) < distance(current) Then current = i
                End If
            Next
            If current = 0 Or current = target Then Exit For
            visited(current) = True

            For neighbour As Integer = 1 To n
                Dim weight As Integer = AdjacencyMatrix(current, neighbour)
                If weight > 0 And Not visited(neighbour) Then
                    If distance(current) + weight < distance(neighbour) Then
                        distance(neighbour) = distance(current) + weight
                        previous(neighbour) = current
                    End If
                End If
            Next
        Next

        If distance(target) = Integer.MaxValue Then
            PathLbl.Text = "No path from " & NodeName(source) & " to " & NodeName(target) & "."
            Return
        End If

        ' Walk back from the target to build the route
        Dim route As New List(Of Integer)
        Dim stepNode As Integer = target
        While stepNode <> 0
            route.Insert(0, stepNode)
            stepNode = previous(stepNode)
        End While

        Dim names As New List(Of String)
        For Each index As Integer In route
            names.Add(NodeName(index))
        Next
        PathLbl.Text = String.Join(" > ", names) & "  (distance " & distance(target) & ")"
        HighlightRoute(route)
    End Sub

    Private Function NodeName(index As Integer) As String
        Return Nodes(index - 1).Text
    End Function

    Private Function NodeCentre(index As Integer) As Point
        Dim node As Button = Nodes(index - 1)
        Return New Point(node.Location.X + node.Width \ 2, node.Location.Y + node.Height \ 2)
    End Function

    Private Sub HighlightRoute(route As List(Of Integer))
        Dim surface As Graphics = CreateGraphics()
        Dim pathPen As New Pen(Color.LimeGreen, 5)
        For i As Integer = 0 To route.Count - 2
            surface.DrawLine(pathPen, NodeCentre(route(i)), NodeCentre(route(i + 1)))
        Next
        For Each index As Integer In route
            Nodes(index - 1).BackColor = Color.LimeGreen
            Nodes(index - 1).ForeColor = Color.LimeGreen
        Next
    End Sub

    Private Sub ResetHighlights()
        For Each node As Button In Nodes
            node.BackColor = Color.Red
            node.ForeColor = Color.Red
        Next
        ' Redraw the plain edges over any previous highlighted path
        Dim surface As Graphics = CreateGraphics()
        Dim edgePen As New Pen(BackColor, 5)
        Dim linePen As New Pen(Color.Black, 2)
        Dim n As Integer = Math.Min(CInt(Sqrt(AdjacencyMatrix.Length)) - 1, Nodes.Count)
        For a As Integer = 1 To n
            For b As Integer = a + 1 To n
                If AdjacencyMatrix(a, b) > 0 Then
                    surface.DrawLine(edgePen, NodeCentre(a), NodeCentre(b))
                    surface.DrawLine(linePen, NodeCentre(a), NodeCentre(b))
                End If
            Next
        Next
    End Sub
End Class
