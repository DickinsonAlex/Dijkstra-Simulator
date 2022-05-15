Imports System.Math
Public Class Form1
    Public Nodes As List(Of Button) = New List(Of Button)
    Public NodeWeights As List(Of Label) = New List(Of Label)
    Public AdjacencyMatrix(2, 2) As Integer
    Public SelectedNode As Button
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
        If StartOption.SelectedIndex >= 0 And EndOption.SelectedIndex >= 0 Then
            Dim count As Integer = AdjacencyMatrix.Length
            Dim vistedVertex(count) As Boolean
            Dim distance(count) As Integer
            For x As Integer = 0 To count
                vistedVertex(x) = False
                distance(x) = Integer.MaxValue
            Next

            Debug.WriteLine(count)


        End If

    End Sub
End Class
