Imports System.Drawing
Imports DijkstraSimulator.Core

''' <summary>
''' The simulator window. Nodes are buttons placed around a circle, edges are drawn
''' onto the form, and the controls down the left build the graph and run the search.
''' </summary>
Public Class Form1

    Private ReadOnly _graph As New Graph(5)
    Private ReadOnly _random As New Random()
    Private ReadOnly Nodes As New List(Of Button)

    Private _selectedNode As Integer = -1
    Private _hoverEdge As Edge?
    Private _result As PathResult
    ' Which step of the search is on screen; Steps.Count means the finished route
    Private _position As Integer

    ' Node colours. Unvisited nodes keep the normal button look.
    Private Shared ReadOnly SelectedColour As Color = Color.LightSkyBlue
    Private Shared ReadOnly CurrentColour As Color = Color.Orange
    Private Shared ReadOnly SettledColour As Color = Color.LightSteelBlue
    Private Shared ReadOnly RouteColour As Color = Color.LimeGreen

    Public Sub New()
        InitializeComponent()
        DistanceTable.Columns.Add("Node")
        DistanceTable.Columns.Add("Distance")
        DistanceTable.Columns.Add("Via")
        DistanceTable.Columns.Add("Settled")
        AdjMatrix.Font = New Font("Lucida Console", 9)
        AdjMatrix.DefaultCellStyle.BackColor = SystemColors.Control
        AdjMatrix.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        AdjMatrix.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control
        AdjMatrix.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        AdjMatrix.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control
        AddHandler _graph.Changed, AddressOf Graph_Changed
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _graph.Randomise(_random, maxWeight:=12)
    End Sub

    Private ReadOnly Property DpiScale As Single
        Get
            Return DeviceDpi / 96.0F
        End Get
    End Property

    Private ReadOnly Property NodeSize As Integer
        Get
            Return CInt(40 * DpiScale)
        End Get
    End Property

    ' ── Keeping the display in step with the graph ───────────────────────

    Private Sub Graph_Changed(sender As Object, e As EventArgs)
        If Points.Value <> _graph.NodeCount Then Points.Value = _graph.NodeCount
        If _selectedNode >= _graph.NodeCount Then _selectedNode = -1
        MakeNodes()
        RefreshNodeLists()
        DisplayAdjacencyMatrix()
        ClearResult()
        PlaceNodes()
    End Sub

    ''' <summary>Adds or removes node buttons so there's one per node in the graph.</summary>
    Private Sub MakeNodes()
        While Nodes.Count > _graph.NodeCount
            Nodes(Nodes.Count - 1).Dispose()
            Nodes.RemoveAt(Nodes.Count - 1)
        End While
        While Nodes.Count < _graph.NodeCount
            Dim index = Nodes.Count
            Dim node As New Button With {
                .Text = Graph.NodeName(index),
                .Font = New Font("Georgia", 14),
                .Size = New Size(NodeSize, NodeSize),
                .UseVisualStyleBackColor = True,
                .TabStop = False,
                .Tag = index}
            AddHandler node.Click, AddressOf Node_Click
            Controls.Add(node)
            Nodes.Add(node)
        End While
    End Sub

    ''' <summary>The part of the window the graph is drawn in: between the controls and the distance table.</summary>
    Private Function GraphArea() As Rectangle
        Dim left = Math.Max(EndOption.Right, AdjMatrix.Right) + CInt(20 * DpiScale)
        Dim right = DistanceTable.Left - CInt(20 * DpiScale)
        Return Rectangle.FromLTRB(left, 0, Math.Max(left + 1, right), ClientSize.Height)
    End Function

    ''' <summary>Puts the node buttons evenly around a circle in the graph area.</summary>
    Private Sub PlaceNodes()
        Dim area = GraphArea()
        Dim radius = Math.Max(20, Math.Min(area.Width, area.Height) / 2 - NodeSize)
        Dim centres = CircleLayout.Positions(Nodes.Count, area.Left + area.Width / 2, area.Top + area.Height / 2, radius)
        For i = 0 To Nodes.Count - 1
            Nodes(i).Size = New Size(NodeSize, NodeSize)
            Nodes(i).Location = New Point(CInt(centres(i).X - NodeSize / 2), CInt(centres(i).Y - NodeSize / 2))
        Next
        ColourNodes()
        Invalidate()
    End Sub

    Private Function NodeCentre(index As Integer) As PointF
        Dim node = Nodes(index)
        Return New PointF(node.Left + node.Width / 2.0F, node.Top + node.Height / 2.0F)
    End Function

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Nodes.Count > 0 Then PlaceNodes()
    End Sub

    Protected Overrides Sub OnDpiChanged(e As DpiChangedEventArgs)
        MyBase.OnDpiChanged(e)
        DisplayAdjacencyMatrix(rebuild:=True)
        PlaceNodes()
    End Sub

    Private Sub RefreshNodeLists()
        Dim names = Enumerable.Range(0, _graph.NodeCount).Select(AddressOf Graph.NodeName).ToArray()
        For Each list In {StartOption, EndOption}
            If list.Items.Count = names.Length Then Continue For
            Dim previous = list.SelectedIndex
            list.Items.Clear()
            list.Items.AddRange(names)
            Dim fallback = If(list Is StartOption, 0, names.Length - 1)
            list.SelectedIndex = Math.Min(If(previous < 0, fallback, previous), names.Length - 1)
        Next
    End Sub

    ''' <summary>Fills the adjacency matrix grid and sizes it to fit.</summary>
    Private Sub DisplayAdjacencyMatrix(Optional rebuild As Boolean = False)
        Dim n = _graph.NodeCount
        If rebuild OrElse AdjMatrix.ColumnCount <> n Then
            AdjMatrix.Rows.Clear()
            AdjMatrix.Columns.Clear()
            For i = 0 To n - 1
                AdjMatrix.Columns.Add(New DataGridViewTextBoxColumn With {
                    .HeaderText = Graph.NodeName(i), .Width = CInt(28 * DpiScale),
                    .SortMode = DataGridViewColumnSortMode.NotSortable})
            Next
            AdjMatrix.Rows.Add(n)
            For i = 0 To n - 1
                AdjMatrix.Rows(i).HeaderCell.Value = Graph.NodeName(i)
                AdjMatrix.Rows(i).Cells(i).ReadOnly = True
            Next
        End If
        For a = 0 To n - 1
            For b = 0 To n - 1
                Dim cell = AdjMatrix.Rows(a).Cells(b)
                Dim value = _graph.GetWeight(a, b)
                cell.Value = If(a = b, "-", value.ToString())
                cell.Style.ForeColor = If(value = 0, SystemColors.GrayText, SystemColors.ControlText)
            Next
        Next

        ' Size the grid to its contents so it reads like the original text matrix
        Dim width = AdjMatrix.RowHeadersWidth + AdjMatrix.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 2
        Dim height = AdjMatrix.ColumnHeadersHeight + AdjMatrix.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + 2
        AdjMatrix.Size = New Size(width, height)
    End Sub

    Private Sub AdjMatrix_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles AdjMatrix.CellEndEdit
        Dim text = Convert.ToString(AdjMatrix.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
        Dim value As Integer
        If String.IsNullOrWhiteSpace(text) Then
            _graph.RemoveEdge(e.RowIndex, e.ColumnIndex)
        ElseIf Integer.TryParse(text.Trim(), value) AndAlso value >= 0 Then
            If value = 0 Then
                _graph.RemoveEdge(e.RowIndex, e.ColumnIndex)
            Else
                _graph.SetEdge(e.RowIndex, e.ColumnIndex, Math.Min(value, 999))
            End If
        End If
        DisplayAdjacencyMatrix() ' puts the old value back if the entry wasn't a number
    End Sub

    ' ── Editing the graph ────────────────────────────────────────────────

    Private Sub Points_ValueChanged(sender As Object, e As EventArgs) Handles Points.ValueChanged
        _graph.Resize(CInt(Points.Value))
    End Sub

    Private Sub RandomButton_Click(sender As Object, e As EventArgs) Handles RandomButton.Click
        _graph.Randomise(_random, maxWeight:=12)
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As EventArgs) Handles ClearButton.Click
        _graph.ClearEdges()
    End Sub

    Private Sub Node_Click(sender As Object, e As EventArgs)
        Dim node = CInt(DirectCast(sender, Button).Tag)
        If _selectedNode = -1 Then
            _selectedNode = node
        ElseIf _selectedNode = node Then
            _selectedNode = -1
        Else
            Dim first = _selectedNode
            _selectedNode = -1
            _graph.SetEdge(first, node, CInt(WeightValue.Value))
        End If
        ColourNodes()
        If _result Is Nothing Then ShowHint()
    End Sub

    Private Function EdgeAt(location As Point) As Edge?
        For Each edge In _graph.Edges()
            Dim a = NodeCentre(edge.A)
            Dim b = NodeCentre(edge.B)
            If CircleLayout.DistanceToSegment(location.X, location.Y, a.X, a.Y, b.X, b.Y) <= 6 * DpiScale Then Return edge
        Next
        Return Nothing
    End Function

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        Dim edge = EdgeAt(e.Location)
        If Not Nullable.Equals(edge, _hoverEdge) Then
            _hoverEdge = edge
            Cursor = If(edge.HasValue, Cursors.Hand, Cursors.Default)
            Invalidate()
        End If
    End Sub

    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles MyBase.MouseClick
        Dim edge = EdgeAt(e.Location)
        If e.Button = MouseButtons.Left AndAlso edge.HasValue Then
            _hoverEdge = Nothing
            Cursor = Cursors.Default
            _graph.RemoveEdge(edge.Value.A, edge.Value.B)
        End If
    End Sub

    ' ── Running the search ───────────────────────────────────────────────

    Private Sub GetPath_Click(sender As Object, e As EventArgs) Handles GetPath.Click
        If StartOption.SelectedIndex < 0 Or EndOption.SelectedIndex < 0 Then
            PathLbl.Text = "Pick a start and an end node."
            Return
        End If
        _selectedNode = -1
        _result = Dijkstra.FindShortestPath(_graph, StartOption.SelectedIndex, EndOption.SelectedIndex)
        PathLbl.Text = _result.Describe()
        ShowPosition(_result.Steps.Count)
    End Sub

    Private Sub StartOption_SelectedIndexChanged(sender As Object, e As EventArgs) Handles StartOption.SelectedIndexChanged, EndOption.SelectedIndexChanged
        ClearResult()
    End Sub

    Private Sub BackButton_Click(sender As Object, e As EventArgs) Handles BackButton.Click
        ShowPosition(_position - 1)
    End Sub

    Private Sub NextButton_Click(sender As Object, e As EventArgs) Handles NextButton.Click
        ShowPosition(_position + 1)
    End Sub

    Private Sub ClearResult()
        _result = Nothing
        _position = 0
        BackButton.Enabled = False
        NextButton.Enabled = False
        DistanceTable.Items.Clear()
        ShowHint()
        ColourNodes()
        Invalidate()
    End Sub

    Private Sub ShowHint()
        PathLbl.Text = ""
        If _selectedNode = -1 Then
            StepLbl.Text = "Click two nodes to join them. Click a line to remove it."
        Else
            StepLbl.Text = $"Now click another node to join it to {Graph.NodeName(_selectedNode)}."
        End If
    End Sub

    ''' <summary>The step on screen, or Nothing when showing the finished route.</summary>
    Private Function CurrentStep() As SearchStep
        If _result Is Nothing OrElse _position >= _result.Steps.Count Then Return Nothing
        Return _result.Steps(_position)
    End Function

    Private Function ShowingRoute() As Boolean
        Return _result IsNot Nothing AndAlso _result.Found AndAlso _position >= _result.Steps.Count
    End Function

    ''' <summary>Shows one step of the search, or the finished route when position = Steps.Count.</summary>
    Private Sub ShowPosition(position As Integer)
        If _result Is Nothing Then Return
        Dim count = _result.Steps.Count
        _position = Math.Clamp(position, 0, count)
        BackButton.Enabled = _position > 0
        NextButton.Enabled = _position < count

        Dim snapshot = _result.Steps(Math.Min(_position, count - 1))
        If _position = count Then
            StepLbl.Text = $"Finished after settling {count} node{If(count = 1, "", "s")}. Press Back to step through the search."
        Else
            Dim updates = If(snapshot.Updated.Count = 0, "no distances changed",
                             "updated " & String.Join(", ", snapshot.Updated.Select(AddressOf Graph.NodeName)))
            StepLbl.Text = $"Step {_position + 1} of {count}: settled {Graph.NodeName(snapshot.Settled)} at distance {snapshot.Distances(snapshot.Settled)}; {updates}."
        End If

        DistanceTable.BeginUpdate()
        DistanceTable.Items.Clear()
        For i = 0 To _graph.NodeCount - 1
            Dim distance = snapshot.Distances(i)
            Dim via = snapshot.Previous(i)
            Dim item As New ListViewItem({
                Graph.NodeName(i),
                If(distance = Dijkstra.Unreached, "∞", distance.ToString()),
                If(via = -1, "-", Graph.NodeName(via)),
                If(snapshot.Visited(i), "✓", "")})
            If i = snapshot.Settled AndAlso _position < count Then item.BackColor = CurrentColour
            DistanceTable.Items.Add(item)
        Next
        DistanceTable.EndUpdate()
        Dim columnWidth = (DistanceTable.ClientSize.Width - 4) \ DistanceTable.Columns.Count
        For Each column As ColumnHeader In DistanceTable.Columns
            column.Width = columnWidth
        Next

        ColourNodes()
        Invalidate()
    End Sub

    Private Sub ColourNodes()
        Dim current = CurrentStep()
        Dim route = ShowingRoute()
        For i = 0 To Nodes.Count - 1
            Dim colour = Color.Empty
            If i = _selectedNode Then
                colour = SelectedColour
            ElseIf route AndAlso _result.Route.Contains(i) Then
                colour = RouteColour
            ElseIf current IsNot Nothing AndAlso i = current.Settled Then
                colour = CurrentColour
            ElseIf current IsNot Nothing AndAlso current.Visited(i) Then
                colour = SettledColour
            End If
            If colour.IsEmpty Then
                Nodes(i).BackColor = SystemColors.Control
                Nodes(i).UseVisualStyleBackColor = True
            Else
                Nodes(i).BackColor = colour
            End If
        Next
    End Sub

    ' ── Drawing ──────────────────────────────────────────────────────────

    ''' <summary>Draws the edges, their weights and (while stepping) each node's distance.</summary>
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        If Nodes.Count <> _graph.NodeCount Then Return
        Dim g = e.Graphics
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias

        Dim current = CurrentStep()
        Dim route = ShowingRoute()
        Dim edges = _graph.Edges()

        For Each edge In edges
            Dim colour = Color.Black
            Dim width = 2.0F
            If route AndAlso _result.UsesEdge(edge.A, edge.B) Then
                colour = RouteColour
                width = 5
            ElseIf current IsNot Nothing AndAlso
                   ((edge.A = current.Settled AndAlso current.Updated.Contains(edge.B)) OrElse
                    (edge.B = current.Settled AndAlso current.Updated.Contains(edge.A))) Then
                colour = CurrentColour
                width = 4
            ElseIf _hoverEdge.HasValue AndAlso _hoverEdge.Value.Connects(edge.A, edge.B) Then
                colour = Color.Red
                width = 3
            End If
            Using pen As New Pen(colour, width * DpiScale)
                g.DrawLine(pen, NodeCentre(edge.A), NodeCentre(edge.B))
            End Using
        Next

        ' Weights, moved along their line if another weight is already in the middle
        Dim centres = Enumerable.Range(0, Nodes.Count).Select(Function(i) (CDbl(NodeCentre(i).X), CDbl(NodeCentre(i).Y))).ToList()
        Dim labels = CircleLayout.WeightLabelPositions(edges, centres, 28 * DpiScale)
        For i = 0 To edges.Count - 1
            DrawLabel(g, edges(i).Weight.ToString(), Font, New PointF(CSng(labels(i).X), CSng(labels(i).Y)), SystemColors.ControlText)
        Next

        ' While stepping, show each node's current distance next to it
        If current IsNot Nothing Then
            Dim area = GraphArea()
            Dim middle As New PointF(area.Left + area.Width / 2.0F, area.Top + area.Height / 2.0F)
            Using bold As New Font(Font, FontStyle.Bold)
                For i = 0 To Nodes.Count - 1
                    Dim c = NodeCentre(i)
                    Dim dx = c.X - middle.X
                    Dim dy = c.Y - middle.Y
                    Dim length = CSng(Math.Max(1, Math.Sqrt(dx * dx + dy * dy)))
                    Dim offset = NodeSize * 0.5F + 16 * DpiScale
                    Dim distance = current.Distances(i)
                    Dim colour = If(current.Updated.Contains(i), Color.DarkOrange, SystemColors.GrayText)
                    DrawLabel(g, If(distance = Dijkstra.Unreached, "∞", distance.ToString()), bold,
                              New PointF(c.X + dx / length * offset, c.Y + dy / length * offset), colour)
                Next
            End Using
        End If
    End Sub

    Private Sub DrawLabel(g As Graphics, text As String, font As Font, centre As PointF, colour As Color)
        Dim size = g.MeasureString(text, font)
        Dim box As New RectangleF(centre.X - size.Width / 2, centre.Y - size.Height / 2, size.Width, size.Height)
        Using fill As New SolidBrush(BackColor), ink As New SolidBrush(colour)
            g.FillRectangle(fill, box)
            g.DrawString(text, font, ink, box.Location)
        End Using
    End Sub

End Class
