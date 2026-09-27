Imports System.Reflection
Imports System.Math
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar

Public Class Node
    Inherits Button
    Private Property connections As List(Of Dictionary(Of Char, Integer))
    Private Property nodeID As Integer

    Public Sub New(nodeCount As Integer)
        connections = New List(Of Dictionary(Of Char, Integer))
        nodeID = 0
        nodeCount += 1
        Name = Convert.ToChar(nodeCount + 64)
        Dim mid As Point = New Point(Me.Width / 2 - 40, Me.Height / 2 - 40)
        Dim AngleSize As Double = (360 / nodeCount) * PI / 180
        Location = New Point((mid.X / 3) * 4 + Cos(PI + AngleSize * nodeCount) * Me.Height / 3, mid.Y - Sin(PI + AngleSize * nodeCount) * Me.Height / 3)
        Height = 40
        Width = 40
        BackColor = Color.Red
        ForeColor = Color.Red
        FlatStyle = FlatStyle.System
        Text = Name
        Font = New Font("Georgia", 16)
    End Sub

End Class