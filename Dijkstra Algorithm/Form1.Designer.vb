<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Points = New System.Windows.Forms.NumericUpDown()
        Me.AdjMatrix = New System.Windows.Forms.Label()
        Me.Weight = New System.Windows.Forms.Label()
        Me.WeightValue = New System.Windows.Forms.NumericUpDown()
        Me.ClearButton = New System.Windows.Forms.Button()
        Me.GetPath = New System.Windows.Forms.Button()
        Me.PathLbl = New System.Windows.Forms.Label()
        Me.StartOption = New System.Windows.Forms.ListBox()
        Me.EndOption = New System.Windows.Forms.ListBox()
        CType(Me.Points, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WeightValue, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Nodes"
        '
        'Points
        '
        Me.Points.Location = New System.Drawing.Point(113, 13)
        Me.Points.Maximum = New Decimal(New Integer() {15, 0, 0, 0})
        Me.Points.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.Points.Name = "Points"
        Me.Points.Size = New System.Drawing.Size(77, 31)
        Me.Points.TabIndex = 1
        Me.Points.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'AdjMatrix
        '
        Me.AdjMatrix.AutoSize = True
        Me.AdjMatrix.Location = New System.Drawing.Point(13, 648)
        Me.AdjMatrix.Name = "AdjMatrix"
        Me.AdjMatrix.Size = New System.Drawing.Size(0, 25)
        Me.AdjMatrix.TabIndex = 2
        '
        'Weight
        '
        Me.Weight.Location = New System.Drawing.Point(13, 61)
        Me.Weight.Name = "Weight"
        Me.Weight.Size = New System.Drawing.Size(94, 38)
        Me.Weight.TabIndex = 6
        Me.Weight.Text = "Weight"
        '
        'WeightValue
        '
        Me.WeightValue.Location = New System.Drawing.Point(113, 61)
        Me.WeightValue.Maximum = New Decimal(New Integer() {50, 0, 0, 0})
        Me.WeightValue.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.WeightValue.Name = "WeightValue"
        Me.WeightValue.Size = New System.Drawing.Size(77, 31)
        Me.WeightValue.TabIndex = 7
        Me.WeightValue.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'ClearButton
        '
        Me.ClearButton.Location = New System.Drawing.Point(78, 102)
        Me.ClearButton.Name = "ClearButton"
        Me.ClearButton.Size = New System.Drawing.Size(112, 34)
        Me.ClearButton.TabIndex = 8
        Me.ClearButton.Text = "Clear"
        Me.ClearButton.UseVisualStyleBackColor = True
        '
        'GetPath
        '
        Me.GetPath.Location = New System.Drawing.Point(78, 247)
        Me.GetPath.Name = "GetPath"
        Me.GetPath.Size = New System.Drawing.Size(112, 34)
        Me.GetPath.TabIndex = 11
        Me.GetPath.Text = "Calculate"
        Me.GetPath.UseVisualStyleBackColor = True
        '
        'PathLbl
        '
        Me.PathLbl.Location = New System.Drawing.Point(12, 305)
        Me.PathLbl.Name = "PathLbl"
        Me.PathLbl.Size = New System.Drawing.Size(237, 34)
        Me.PathLbl.TabIndex = 12
        '
        'StartOption
        '
        Me.StartOption.FormattingEnabled = True
        Me.StartOption.ItemHeight = 25
        Me.StartOption.Location = New System.Drawing.Point(13, 162)
        Me.StartOption.Name = "StartOption"
        Me.StartOption.Size = New System.Drawing.Size(110, 79)
        Me.StartOption.TabIndex = 13
        '
        'EndOption
        '
        Me.EndOption.FormattingEnabled = True
        Me.EndOption.ItemHeight = 25
        Me.EndOption.Location = New System.Drawing.Point(139, 162)
        Me.EndOption.Name = "EndOption"
        Me.EndOption.Size = New System.Drawing.Size(110, 79)
        Me.EndOption.TabIndex = 14
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1078, 1044)
        Me.Controls.Add(Me.EndOption)
        Me.Controls.Add(Me.StartOption)
        Me.Controls.Add(Me.PathLbl)
        Me.Controls.Add(Me.GetPath)
        Me.Controls.Add(Me.ClearButton)
        Me.Controls.Add(Me.WeightValue)
        Me.Controls.Add(Me.Weight)
        Me.Controls.Add(Me.AdjMatrix)
        Me.Controls.Add(Me.Points)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Form1"
        Me.Text = "Dijkstra Simulator"
        CType(Me.Points, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WeightValue, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Points As NumericUpDown
    Friend WithEvents AdjMatrix As Label
    Friend WithEvents Weight As Label
    Friend WithEvents WeightValue As NumericUpDown
    Friend WithEvents ClearButton As Button
    Friend WithEvents GetPath As Button
    Friend WithEvents PathLbl As Label
    Friend WithEvents StartOption As ListBox
    Friend WithEvents EndOption As ListBox
End Class
