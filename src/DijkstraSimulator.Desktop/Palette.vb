Imports System.Drawing

''' <summary>The colours used to draw the graph (the same ones the web version uses).</summary>
Friend NotInheritable Class Palette
    Private Sub New()
    End Sub

    Public Shared ReadOnly Background As Color = Color.FromArgb(&HFA, &HFA, &HF7)
    Public Shared ReadOnly EdgeLine As Color = Color.FromArgb(&H9A, &HA0, &HAA)
    Public Shared ReadOnly EdgeHover As Color = Color.FromArgb(&HE5, &H48, &H4D)
    Public Shared ReadOnly Ink As Color = Color.FromArgb(&H1F, &H23, &H28)
    Public Shared ReadOnly MutedInk As Color = Color.FromArgb(&H6B, &H72, &H80)
    Public Shared ReadOnly Node As Color = Color.FromArgb(&HE5, &H48, &H4D)
    Public Shared ReadOnly Settled As Color = Color.FromArgb(&H5B, &H6B, &H7F)
    Public Shared ReadOnly Current As Color = Color.FromArgb(&HE8, &H91, &H0C)
    Public Shared ReadOnly Route As Color = Color.FromArgb(&H2F, &HA3, &H6B)
    Public Shared ReadOnly Selected As Color = Color.FromArgb(&H3B, &H82, &HF6)
End Class
