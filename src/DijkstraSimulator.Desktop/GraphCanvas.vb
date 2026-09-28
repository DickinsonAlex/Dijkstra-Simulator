Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports DijkstraSimulator.Core

''' <summary>
''' Draws the graph and handles clicks on it. Everything is repainted from the
''' <see cref="Graph"/> in <see cref="OnPaint"/>, so nothing is lost when the
''' window is resized, covered or minimised.
''' </summary>
Friend NotInheritable Class GraphCanvas
    Inherits Control

    Private _graph As Graph
    Private _result As PathResult
    Private _stepIndex As Integer = -1
    Private _selectedNode As Integer = -1
    Private _hoverNode As Integer = -1
    Private _hoverEdge As Edge?

    ''' <summary>Raised when a node is clicked.</summary>
    Public Event NodeClicked(node As Integer)
    ''' <summary>Raised when an edge (but not a node) is clicked.</summary>
    Public Event EdgeClicked(edge As Edge)

    Public Sub New()
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        BackColor = Palette.Background
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Graph As Graph
        Get
            Return _graph
        End Get
        Set(value As Graph)
            _graph = value
            Invalidate()
        End Set
    End Property

    ''' <summary>The search to show, or Nothing.</summary>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Result As PathResult
        Get
            Return _result
        End Get
        Set(value As PathResult)
            _result = value
            _stepIndex = -1
            Invalidate()
        End Set
    End Property

    ''' <summary>Which step of <see cref="Result"/> to show, or -1 for the finished route.</summary>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property StepIndex As Integer
        Get
            Return _stepIndex
        End Get
        Set(value As Integer)
            _stepIndex = value
            Invalidate()
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SelectedNode As Integer
        Get
            Return _selectedNode
        End Get
        Set(value As Integer)
            _selectedNode = value
            Invalidate()
        End Set
    End Property

    Private ReadOnly Property DpiScale As Single
        Get
            Return DeviceDpi / 96.0F
        End Get
    End Property

    Private ReadOnly Property NodeRadius As Single
        Get
            Return 22 * DpiScale
        End Get
    End Property

    Private Function Centres() As List(Of PointF)
        Dim count = If(_graph Is Nothing, 0, _graph.NodeCount)
        Dim radius = Math.Max(10, Math.Min(ClientSize.Width, ClientSize.Height) / 2 - NodeRadius - 30 * DpiScale)
        Return CircleLayout.Positions(count, ClientSize.Width / 2, ClientSize.Height / 2, radius).
            Select(Function(p) New PointF(CSng(p.X), CSng(p.Y))).ToList()
    End Function

    Private Function NodeAt(location As Point) As Integer
        Dim points = Centres()
        For i = 0 To points.Count - 1
            Dim dx = location.X - points(i).X
            Dim dy = location.Y - points(i).Y
            If dx * dx + dy * dy <= NodeRadius * NodeRadius Then Return i
        Next
        Return -1
    End Function

    Private Function EdgeAt(location As Point) As Edge?
        If _graph Is Nothing Then Return Nothing
        Dim points = Centres()
        Dim closest As Edge? = Nothing
        Dim closestDistance = 7.0 * DpiScale
        For Each e In _graph.Edges()
            Dim d = CircleLayout.DistanceToSegment(location.X, location.Y, points(e.A).X, points(e.A).Y, points(e.B).X, points(e.B).Y)
            If d < closestDistance Then
                closest = e
                closestDistance = d
            End If
        Next
        Return closest
    End Function

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Dim node = NodeAt(e.Location)
        Dim edge = If(node = -1, EdgeAt(e.Location), Nothing)
        If node <> _hoverNode OrElse Not Nullable.Equals(edge, _hoverEdge) Then
            _hoverNode = node
            _hoverEdge = edge
            Cursor = If(node <> -1 OrElse edge.HasValue, Cursors.Hand, Cursors.Default)
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _hoverNode = -1
        _hoverEdge = Nothing
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        MyBase.OnMouseClick(e)
        If e.Button <> MouseButtons.Left OrElse _graph Is Nothing Then Return
        Dim node = NodeAt(e.Location)
        If node <> -1 Then
            RaiseEvent NodeClicked(node)
            Return
        End If
        Dim edge = EdgeAt(e.Location)
        If edge.HasValue Then
            _hoverEdge = Nothing
            RaiseEvent EdgeClicked(edge.Value)
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        If _graph Is Nothing Then Return
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit

        Dim points = Centres()
        Dim current As SearchStep = Nothing
        If _result IsNot Nothing AndAlso _stepIndex >= 0 AndAlso _stepIndex < _result.Steps.Count Then
            current = _result.Steps(_stepIndex)
        End If
        Dim showRoute = _result IsNot Nothing AndAlso current Is Nothing AndAlso _result.Found

        ' Edges first, so the nodes sit on top of them
        For Each edge In _graph.Edges()
            Dim colour = Palette.EdgeLine
            Dim width = 2.0F
            If showRoute AndAlso _result.UsesEdge(edge.A, edge.B) Then
                colour = Palette.Route
                width = 5
            ElseIf current IsNot Nothing AndAlso
                   ((edge.A = current.Settled AndAlso current.Updated.Contains(edge.B)) OrElse
                    (edge.B = current.Settled AndAlso current.Updated.Contains(edge.A))) Then
                colour = Palette.Current
                width = 3.5F
            ElseIf _hoverEdge.HasValue AndAlso _hoverEdge.Value.Connects(edge.A, edge.B) Then
                colour = Palette.EdgeHover
                width = 3
            End If
            Using pen As New Pen(colour, width * DpiScale)
                g.DrawLine(pen, points(edge.A), points(edge.B))
            End Using
        Next

        ' Weight labels at the middle of each edge
        Using font As New Font("Segoe UI", 9, FontStyle.Bold),
              fill As New SolidBrush(Palette.Background),
              ink As New SolidBrush(Palette.Ink)
            ' Labels slide along their edge rather than sit on top of each other
            Dim edges = _graph.Edges()
            Dim labelPositions = CircleLayout.WeightLabelPositions(
                edges, points.Select(Function(p) (CDbl(p.X), CDbl(p.Y))).ToList(), 30 * DpiScale)
            For i = 0 To edges.Count - 1
                Dim at As New PointF(CSng(labelPositions(i).X), CSng(labelPositions(i).Y))
                DrawLabel(g, edges(i).Weight.ToString(), font, at, fill, ink)
            Next
        End Using

        ' Nodes
        Dim centre As New PointF(ClientSize.Width / 2.0F, ClientSize.Height / 2.0F)
        Dim r = NodeRadius
        Using nodeFont As New Font("Segoe UI Semibold", 12),
              badgeFont As New Font("Segoe UI", 8.5F, FontStyle.Bold),
              white As New SolidBrush(Color.White),
              badgeFill As New SolidBrush(Palette.Background),
              badgeInk As New SolidBrush(Palette.Ink),
              format As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            For i = 0 To points.Count - 1
                Dim fillColour = Palette.Node
                If showRoute AndAlso _result.Route.Contains(i) Then
                    fillColour = Palette.Route
                ElseIf current IsNot Nothing Then
                    If i = current.Settled Then
                        fillColour = Palette.Current
                    ElseIf current.Visited(i) Then
                        fillColour = Palette.Settled
                    End If
                End If

                Dim bounds As New RectangleF(points(i).X - r, points(i).Y - r, 2 * r, 2 * r)
                Using brush As New SolidBrush(If(i = _hoverNode, ControlPaint.Dark(fillColour, 0.05F), fillColour))
                    g.FillEllipse(brush, bounds)
                End Using
                If i = _selectedNode Then
                    Using ring As New Pen(Palette.Selected, 3 * DpiScale)
                        g.DrawEllipse(ring, RectangleF.Inflate(bounds, 4 * DpiScale, 4 * DpiScale))
                    End Using
                End If
                g.DrawString(Core.Graph.NodeName(i), nodeFont, white, bounds, format)

                ' While stepping, show each node's current distance just outside it
                If current IsNot Nothing Then
                    Dim dx = points(i).X - centre.X
                    Dim dy = points(i).Y - centre.Y
                    Dim length = CSng(Math.Max(1, Math.Sqrt(dx * dx + dy * dy)))
                    Dim offset = r + 16 * DpiScale
                    Dim at As New PointF(points(i).X + dx / length * offset, points(i).Y + dy / length * offset)
                    Dim distance = current.Distances(i)
                    DrawLabel(g, If(distance = Dijkstra.Unreached, "∞", distance.ToString()), badgeFont, at, badgeFill, badgeInk)
                End If
            Next
        End Using
    End Sub

    Private Sub DrawLabel(g As Graphics, text As String, font As Font, middle As PointF, fill As Brush, ink As Brush)
        Dim size = g.MeasureString(text, font)
        Dim box As New RectangleF(middle.X - size.Width / 2 - 3 * DpiScale, middle.Y - size.Height / 2 - 1 * DpiScale,
                                  size.Width + 6 * DpiScale, size.Height + 2 * DpiScale)
        Using path = RoundedRectangle(box, 5 * DpiScale)
            g.FillPath(fill, path)
        End Using
        Using format As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            g.DrawString(text, font, ink, box, format)
        End Using
    End Sub

    Private Shared Function RoundedRectangle(box As RectangleF, radius As Single) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d = radius * 2
        path.AddArc(box.X, box.Y, d, d, 180, 90)
        path.AddArc(box.Right - d, box.Y, d, d, 270, 90)
        path.AddArc(box.Right - d, box.Bottom - d, d, d, 0, 90)
        path.AddArc(box.X, box.Bottom - d, d, d, 90, 90)
        path.CloseFigure()
        Return path
    End Function

End Class
