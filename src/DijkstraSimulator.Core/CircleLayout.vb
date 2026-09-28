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
