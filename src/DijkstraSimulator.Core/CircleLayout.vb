''' <summary>Places nodes evenly around a circle, starting on the left and going clockwise.</summary>
Public Module CircleLayout

    ''' <summary>The centre of each node, in screen coordinates (y grows downwards).</summary>
    Public Function Positions(count As Integer, centreX As Double, centreY As Double, radius As Double) As IReadOnlyList(Of (X As Double, Y As Double))
        If count < 0 Then Throw New ArgumentOutOfRangeException(NameOf(count))
        Dim result As New List(Of (X As Double, Y As Double))(count)
        Dim angleStep = 2 * Math.PI / Math.Max(count, 1)
        For i = 0 To count - 1
            Dim angle = Math.PI - angleStep * i
            result.Add((centreX + Math.Cos(angle) * radius, centreY - Math.Sin(angle) * radius))
        Next
        Return result
    End Function

    ''' <summary>
    ''' Where to draw each edge's weight label. Labels go at the middle of their edge
    ''' unless that would put them on top of another label (for example where two
    ''' diagonals cross at the centre), in which case they slide along the edge.
    ''' </summary>
    ''' <param name="points">The centre of each node, as returned by <see cref="Positions"/>.</param>
    ''' <param name="minGap">How far apart two labels' centres must be to not overlap.</param>
    Public Function WeightLabelPositions(edges As IReadOnlyList(Of Edge), points As IReadOnlyList(Of (X As Double, Y As Double)), minGap As Double) As IReadOnlyList(Of (X As Double, Y As Double))
        ArgumentNullException.ThrowIfNull(edges)
        ArgumentNullException.ThrowIfNull(points)
        Dim fractions = {0.5, 0.38, 0.62, 0.3, 0.7}
        Dim placed As New List(Of (X As Double, Y As Double))(edges.Count)
        For Each e In edges
            Dim a = points(e.A)
            Dim b = points(e.B)
            Dim best As (X As Double, Y As Double) = Nothing
            Dim bestClearance = Double.NegativeInfinity
            For Each t In fractions
                Dim candidate = (X:=a.X + (b.X - a.X) * t, Y:=a.Y + (b.Y - a.Y) * t)
                Dim clearance = Double.PositiveInfinity
                For Each other In placed
                    clearance = Math.Min(clearance, Math.Sqrt((candidate.X - other.X) ^ 2 + (candidate.Y - other.Y) ^ 2))
                Next
                If clearance >= minGap Then
                    best = candidate
                    Exit For
                End If
                If clearance > bestClearance Then
                    best = candidate
                    bestClearance = clearance
                End If
            Next
            placed.Add(best)
        Next
        Return placed
    End Function

    ''' <summary>How far a point is from the line segment between two others.</summary>
    Public Function DistanceToSegment(px As Double, py As Double, x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
        Dim dx = x2 - x1
        Dim dy = y2 - y1
        Dim lengthSquared = dx * dx + dy * dy
        Dim t = If(lengthSquared = 0, 0.0, ((px - x1) * dx + (py - y1) * dy) / lengthSquared)
        t = Math.Clamp(t, 0.0, 1.0)
        Dim nearestX = x1 + t * dx
        Dim nearestY = y1 + t * dy
        Return Math.Sqrt((px - nearestX) ^ 2 + (py - nearestY) ^ 2)
    End Function

End Module
