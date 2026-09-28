Imports System.Drawing
Imports DijkstraSimulator.Core

''' <summary>
''' The main window: controls down the left, the graph on the right and the
''' adjacency matrix underneath it.
''' </summary>
Public NotInheritable Class MainForm
    Inherits Form

    Private Const MinNodes As Integer = 2
    Private Const MaxNodes As Integer = 15
    Private Const MaxWeight As Integer = 50

    Private ReadOnly _graph As New Core.Graph(6)
    Private ReadOnly _random As New Random()
    Private _result As PathResult
    ' Which step of the search is on screen; Steps.Count means the finished route
    Private _position As Integer

    Private ReadOnly _canvas As New GraphCanvas With {.Dock = DockStyle.Fill}
    Private ReadOnly _nodeCount As New NumericUpDown With {.Minimum = MinNodes, .Maximum = MaxNodes, .Value = 6, .Dock = DockStyle.Fill}
    Private ReadOnly _weight As New NumericUpDown With {.Minimum = 1, .Maximum = MaxWeight, .Value = 1, .Dock = DockStyle.Fill}
    Private ReadOnly _randomButton As New Button With {.Text = "Random graph", .Dock = DockStyle.Fill, .AutoSize = True}
    Private ReadOnly _clearButton As New Button With {.Text = "Clear edges", .Dock = DockStyle.Fill, .AutoSize = True}
    Private ReadOnly _hint As New Label With {.AutoSize = True, .ForeColor = Palette.MutedInk, .Margin = New Padding(3, 6, 3, 6)}
    Private ReadOnly _from As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Dock = DockStyle.Fill}
    Private ReadOnly _to As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Dock = DockStyle.Fill}
    Private ReadOnly _findButton As New Button With {.Text = "Find shortest path", .Dock = DockStyle.Fill, .AutoSize = True}
    Private ReadOnly _resultLabel As New Label With {.AutoSize = True, .Margin = New Padding(3, 8, 3, 8)}
    Private ReadOnly _backButton As New Button With {.Text = "◀ Step back", .Dock = DockStyle.Fill, .AutoSize = True}
    Private ReadOnly _nextButton As New Button With {.Text = "Step forward ▶", .Dock = DockStyle.Fill, .AutoSize = True}
    Private ReadOnly _stepLabel As New Label With {.AutoSize = True, .ForeColor = Palette.MutedInk, .Margin = New Padding(3, 6, 3, 6)}
    Private ReadOnly _table As New ListView With {
        .View = View.Details, .FullRowSelect = True, .HeaderStyle = ColumnHeaderStyle.Nonclickable,
        .Anchor = AnchorStyles.Left Or AnchorStyles.Right, .Height = 260, .MultiSelect = False}
    Private ReadOnly _matrix As New DataGridView With {
        .Dock = DockStyle.Fill, .AllowUserToAddRows = False, .AllowUserToDeleteRows = False,
        .AllowUserToResizeRows = False, .AllowUserToResizeColumns = False,
        .SelectionMode = DataGridViewSelectionMode.CellSelect, .MultiSelect = False,
        .BackgroundColor = SystemColors.Window, .BorderStyle = BorderStyle.None,
        .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader,
        .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize}

    Public Sub New()
        AutoScaleDimensions = New SizeF(96, 96)
        AutoScaleMode = AutoScaleMode.Dpi
        Text = "Dijkstra Simulator"
        ClientSize = New Size(1180, 820)
        MinimumSize = New Size(820, 620)
        StartPosition = FormStartPosition.CenterScreen

        _table.Columns.Add("Node")
        _table.Columns.Add("Distance")
        _table.Columns.Add("Via")
        _table.Columns.Add("Settled")

        _split = New SplitContainer With {.Dock = DockStyle.Fill, .Orientation = Orientation.Horizontal, .FixedPanel = FixedPanel.Panel2}
        Dim split = _split
        split.Panel1.Controls.Add(_canvas)
        split.Panel2.Controls.Add(_matrix)
        split.Panel2.Controls.Add(New Label With {
            .Text = "Adjacency matrix (edit a cell to change a weight; 0 removes the edge)",
            .Dock = DockStyle.Top, .AutoSize = False, .Height = 26, .Padding = New Padding(8, 6, 0, 0),
            .ForeColor = Palette.MutedInk})

        _sidebar = BuildSidebar()
        _sidebar.Dock = DockStyle.Left
        _sidebar.Width = 300

        Controls.Add(split)
        Controls.Add(_sidebar)

        AddHandler _graph.Changed, AddressOf Graph_Changed
        AddHandler _canvas.NodeClicked, AddressOf Canvas_NodeClicked
        AddHandler _canvas.EdgeClicked, AddressOf Canvas_EdgeClicked
        AddHandler _nodeCount.ValueChanged, Sub() _graph.Resize(CInt(_nodeCount.Value))
        AddHandler _randomButton.Click, Sub() _graph.Randomise(_random, maxWeight:=12)
        AddHandler _clearButton.Click, Sub() _graph.ClearEdges()
        AddHandler _findButton.Click, AddressOf FindButton_Click
        AddHandler _backButton.Click, Sub() ShowPosition(_position - 1)
        AddHandler _nextButton.Click, Sub() ShowPosition(_position + 1)
        AddHandler _from.SelectedIndexChanged, Sub() ClearResult()
        AddHandler _to.SelectedIndexChanged, Sub() ClearResult()
        AddHandler _matrix.CellEndEdit, AddressOf Matrix_CellEndEdit
        AddHandler _sidebar.Resize, Sub() FitToDpi()

        _canvas.Graph = _graph
        _graph.Randomise(_random, maxWeight:=12)
    End Sub

    Private ReadOnly _split As SplitContainer
    Private ReadOnly _sidebar As Control

    Private ReadOnly Property DpiScale As Single
        Get
            Return DeviceDpi / 96.0F
        End Get
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        _split.SplitterDistance = Math.Max(CInt(200 * DpiScale), _split.Height - CInt(250 * DpiScale))
        FitToDpi()
    End Sub

    Protected Overrides Sub OnDpiChanged(e As DpiChangedEventArgs)
        MyBase.OnDpiChanged(e)
        FitToDpi()
        RefreshMatrix(rebuild:=True)
    End Sub

    ''' <summary>Sizes the things that WinForms doesn't scale for high-DPI screens by itself.</summary>
    Private Sub FitToDpi()
        Dim wrapWidth = Math.Max(100, _sidebar.ClientSize.Width - _sidebar.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - CInt(8 * DpiScale))
        For Each label In {_hint, _resultLabel, _stepLabel}
            label.MaximumSize = New Size(wrapWidth, 0)
        Next
        Dim widths = {50, 70, 50, 60}
        For i = 0 To _table.Columns.Count - 1
            _table.Columns(i).Width = CInt(widths(i) * DpiScale)
        Next
    End Sub

    Private Function BuildSidebar() As Control
        Dim table As New TableLayoutPanel With {.ColumnCount = 2, .AutoScroll = True, .Padding = New Padding(12, 8, 12, 12)}
        table.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        table.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))

        AddHeading(table, "Graph")
        AddRow(table, Caption("Nodes"), _nodeCount)
        AddRow(table, _randomButton, _clearButton)
        AddHeading(table, "Edges")
        AddRow(table, Caption("New edge weight"), _weight)
        AddWide(table, _hint)
        AddHeading(table, "Shortest path")
        AddRow(table, Caption("From"), _from)
        AddRow(table, Caption("To"), _to)
        AddWide(table, _findButton)
        AddWide(table, _resultLabel)
        AddRow(table, _backButton, _nextButton)
        AddWide(table, _stepLabel)
        AddWide(table, _table)

        ' A filler row keeps everything pinned to the top
        table.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        table.RowCount += 1
        Return table
    End Function

    Private Shared Function Caption(text As String) As Label
        Return New Label With {.Text = text, .AutoSize = True, .Anchor = AnchorStyles.Left}
    End Function

    Private Shared Sub AddHeading(table As TableLayoutPanel, text As String)
        Dim heading As New Label With {
            .Text = text, .AutoSize = True, .Margin = New Padding(3, 14, 3, 4),
            .Font = New Font("Segoe UI Semibold", 11)}
        AddWide(table, heading)
    End Sub

    Private Shared Sub AddRow(table As TableLayoutPanel, left As Control, right As Control)
        Dim row = NextRow(table)
        table.Controls.Add(left, 0, row)
        table.Controls.Add(right, 1, row)
    End Sub

    Private Shared Sub AddWide(table As TableLayoutPanel, control As Control)
        Dim row = NextRow(table)
        table.Controls.Add(control, 0, row)
        table.SetColumnSpan(control, 2)
    End Sub

    Private Shared Function NextRow(table As TableLayoutPanel) As Integer
        table.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        table.RowCount += 1
        Return table.RowCount - 1
    End Function

    ' ── Editing the graph ────────────────────────────────────────────────

    Private Sub Canvas_NodeClicked(node As Integer)
        Dim selected = _canvas.SelectedNode
        If selected = -1 Then
            _canvas.SelectedNode = node
        ElseIf selected = node Then
            _canvas.SelectedNode = -1
        Else
            _canvas.SelectedNode = -1
            _graph.SetEdge(selected, node, CInt(_weight.Value))
        End If
        UpdateHint()
    End Sub

    Private Sub Canvas_EdgeClicked(edge As Edge)
        _graph.RemoveEdge(edge.A, edge.B)
    End Sub

    Private Sub Matrix_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        Dim cell = _matrix.Rows(e.RowIndex).Cells(e.ColumnIndex)
        Dim text = Convert.ToString(cell.Value)
        Dim weight As Integer
        If String.IsNullOrWhiteSpace(text) Then
            weight = 0
        ElseIf Not Integer.TryParse(text.Trim(), weight) OrElse weight < 0 Then
            RefreshMatrix() ' not a number: put the old value back
            Return
        End If
        If weight = 0 Then
            _graph.RemoveEdge(e.RowIndex, e.ColumnIndex)
        Else
            _graph.SetEdge(e.RowIndex, e.ColumnIndex, weight)
        End If
        RefreshMatrix()
    End Sub

    Private Sub Graph_Changed(sender As Object, e As EventArgs)
        If _nodeCount.Value <> _graph.NodeCount Then _nodeCount.Value = _graph.NodeCount
        If _canvas.SelectedNode >= _graph.NodeCount Then _canvas.SelectedNode = -1
        RefreshNodeLists()
        RefreshMatrix()
        ClearResult()
        UpdateHint()
        _canvas.Invalidate()
    End Sub

    Private Sub UpdateHint()
        If _canvas.SelectedNode = -1 Then
            _hint.Text = "Click two nodes to join them with an edge. Click an edge to remove it."
        Else
            _hint.Text = $"Now click another node to join it to {Core.Graph.NodeName(_canvas.SelectedNode)}, or click {Core.Graph.NodeName(_canvas.SelectedNode)} again to cancel."
        End If
    End Sub

    Private Sub RefreshNodeLists()
        Dim names = Enumerable.Range(0, _graph.NodeCount).Select(AddressOf Core.Graph.NodeName).ToArray()
        For Each box In {_from, _to}
            If box.Items.Count = names.Length Then Continue For
            Dim previous = box.SelectedIndex
            box.Items.Clear()
            box.Items.AddRange(names)
            box.SelectedIndex = Math.Min(If(previous < 0, If(box Is _from, 0, names.Length - 1), previous), names.Length - 1)
        Next
    End Sub

    Private Sub RefreshMatrix(Optional rebuild As Boolean = False)
        Dim n = _graph.NodeCount
        If rebuild OrElse _matrix.ColumnCount <> n Then
            _matrix.Rows.Clear()
            _matrix.Columns.Clear()
            For i = 0 To n - 1
                Dim column As New DataGridViewTextBoxColumn With {
                    .HeaderText = Core.Graph.NodeName(i), .Width = CInt(40 * DpiScale), .SortMode = DataGridViewColumnSortMode.NotSortable}
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                _matrix.Columns.Add(column)
            Next
            If n > 0 Then _matrix.Rows.Add(n)
            For i = 0 To n - 1
                _matrix.Rows(i).HeaderCell.Value = Core.Graph.NodeName(i)
                _matrix.Rows(i).Cells(i).ReadOnly = True
                _matrix.Rows(i).Cells(i).Style.BackColor = SystemColors.Control
            Next
        End If
        For a = 0 To n - 1
            For b = 0 To n - 1
                Dim cell = _matrix.Rows(a).Cells(b)
                Dim weight = _graph.GetWeight(a, b)
                cell.Value = weight.ToString()
                cell.Style.ForeColor = If(weight = 0, SystemColors.GrayText, SystemColors.ControlText)
            Next
        Next
    End Sub

    ' ── Running the search ───────────────────────────────────────────────

    Private Sub FindButton_Click(sender As Object, e As EventArgs)
        _canvas.SelectedNode = -1
        UpdateHint()
        _result = Dijkstra.FindShortestPath(_graph, _from.SelectedIndex, _to.SelectedIndex)
        _canvas.Result = _result
        _resultLabel.Text = _result.Describe()
        _resultLabel.ForeColor = If(_result.Found, Palette.Route, Palette.Node)
        ShowPosition(_result.Steps.Count)
    End Sub

    Private Sub ClearResult()
        _result = Nothing
        _canvas.Result = Nothing
        _resultLabel.Text = "Pick two nodes and find the shortest path between them."
        _resultLabel.ForeColor = Palette.MutedInk
        _stepLabel.Text = ""
        _table.Items.Clear()
        _backButton.Enabled = False
        _nextButton.Enabled = False
    End Sub

    ''' <summary>Shows one step of the search, or the finished route when position = Steps.Count.</summary>
    Private Sub ShowPosition(position As Integer)
        If _result Is Nothing Then Return
        Dim count = _result.Steps.Count
        _position = Math.Clamp(position, 0, count)
        _canvas.StepIndex = If(_position = count, -1, _position)
        _backButton.Enabled = _position > 0
        _nextButton.Enabled = _position < count

        Dim snapshot = _result.Steps(Math.Min(_position, count - 1))
        If _position = count Then
            _stepLabel.Text = $"Finished after settling {count} node{If(count = 1, "", "s")}. Step back to replay the search."
        Else
            Dim name = Core.Graph.NodeName(snapshot.Settled)
            Dim updates = If(snapshot.Updated.Count = 0, "no distances changed",
                             "updated " & String.Join(", ", snapshot.Updated.Select(AddressOf Core.Graph.NodeName)))
            _stepLabel.Text = $"Step {_position + 1} of {count}: settled {name} at distance {snapshot.Distances(snapshot.Settled)}; {updates}."
        End If

        _table.BeginUpdate()
        _table.Items.Clear()
        For i = 0 To _graph.NodeCount - 1
            Dim distance = snapshot.Distances(i)
            Dim via = snapshot.Previous(i)
            Dim item As New ListViewItem({
                Core.Graph.NodeName(i),
                If(distance = Dijkstra.Unreached, "∞", distance.ToString()),
                If(via = -1, "–", Core.Graph.NodeName(via)),
                If(snapshot.Visited(i), "✓", "")})
            If i = snapshot.Settled AndAlso _position < count Then item.BackColor = Color.FromArgb(&HFD, &HF0, &HD5)
            _table.Items.Add(item)
        Next
        _table.EndUpdate()
    End Sub

End Class
