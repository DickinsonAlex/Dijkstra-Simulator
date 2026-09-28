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
        Me.Weight = New System.Windows.Forms.Label()
        Me.WeightValue = New System.Windows.Forms.NumericUpDown()
        Me.RandomButton = New System.Windows.Forms.Button()
        Me.ClearButton = New System.Windows.Forms.Button()
        Me.StartOption = New System.Windows.Forms.ListBox()
        Me.EndOption = New System.Windows.Forms.ListBox()
        Me.GetPath = New System.Windows.Forms.Button()
        Me.PathLbl = New System.Windows.Forms.Label()
        Me.BackButton = New System.Windows.Forms.Button()
        Me.NextButton = New System.Windows.Forms.Button()
        Me.StepLbl = New System.Windows.Forms.Label()
        Me.AdjMatrix = New System.Windows.Forms.DataGridView()
        Me.DistanceTable = New System.Windows.Forms.ListView()
        CType(Me.Points, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WeightValue, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AdjMatrix, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.Points.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'Weight
        '
        Me.Weight.Location = New System.Drawing.Point(13, 61)
        Me.Weight.Name = "Weight"
        Me.Weight.Size = New System.Drawing.Size(94, 38)
        Me.Weight.TabIndex = 2
        Me.Weight.Text = "Weight"
        '
        'WeightValue
        '
        Me.WeightValue.Location = New System.Drawing.Point(113, 61)
        Me.WeightValue.Maximum = New Decimal(New Integer() {50, 0, 0, 0})
        Me.WeightValue.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.WeightValue.Name = "WeightValue"
        Me.WeightValue.Size = New System.Drawing.Size(77, 31)
        Me.WeightValue.TabIndex = 3
        Me.WeightValue.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'RandomButton
        '
        Me.RandomButton.Location = New System.Drawing.Point(13, 102)
        Me.RandomButton.Name = "RandomButton"
        Me.RandomButton.Size = New System.Drawing.Size(112, 34)
        Me.RandomButton.TabIndex = 4
        Me.RandomButton.Text = "Random"
        Me.RandomButton.UseVisualStyleBackColor = True
        '
        'ClearButton
        '
        Me.ClearButton.Location = New System.Drawing.Point(137, 102)
        Me.ClearButton.Name = "ClearButton"
        Me.ClearButton.Size = New System.Drawing.Size(112, 34)
        Me.ClearButton.TabIndex = 5
        Me.ClearButton.Text = "Clear"
        Me.ClearButton.UseVisualStyleBackColor = True
        '
        'StartOption
        '
        Me.StartOption.FormattingEnabled = True
        Me.StartOption.ItemHeight = 25
        Me.StartOption.Location = New System.Drawing.Point(13, 162)
        Me.StartOption.Name = "StartOption"
        Me.StartOption.Size = New System.Drawing.Size(110, 79)
        Me.StartOption.TabIndex = 6
        '
        'EndOption
        '
        Me.EndOption.FormattingEnabled = True
        Me.EndOption.ItemHeight = 25
        Me.EndOption.Location = New System.Drawing.Point(139, 162)
        Me.EndOption.Name = "EndOption"
        Me.EndOption.Size = New System.Drawing.Size(110, 79)
        Me.EndOption.TabIndex = 7
        '
        'GetPath
        '
        Me.GetPath.Location = New System.Drawing.Point(78, 247)
        Me.GetPath.Name = "GetPath"
        Me.GetPath.Size = New System.Drawing.Size(112, 34)
        Me.GetPath.TabIndex = 8
        Me.GetPath.Text = "Calculate"
        Me.GetPath.UseVisualStyleBackColor = True
        '
        'PathLbl
        '
        Me.PathLbl.Location = New System.Drawing.Point(12, 289)
        Me.PathLbl.Name = "PathLbl"
        Me.PathLbl.Size = New System.Drawing.Size(237, 56)
        Me.PathLbl.TabIndex = 9
        '
        'BackButton
        '
        Me.BackButton.Enabled = False
        Me.BackButton.Location = New System.Drawing.Point(13, 348)
        Me.BackButton.Name = "BackButton"
        Me.BackButton.Size = New System.Drawing.Size(112, 34)
        Me.BackButton.TabIndex = 10
        Me.BackButton.Text = "< Back"
        Me.BackButton.UseVisualStyleBackColor = True
        '
        'NextButton
        '
        Me.NextButton.Enabled = False
        Me.NextButton.Location = New System.Drawing.Point(137, 348)
        Me.NextButton.Name = "NextButton"
        Me.NextButton.Size = New System.Drawing.Size(112, 34)
        Me.NextButton.TabIndex = 11
        Me.NextButton.Text = "Next >"
        Me.NextButton.UseVisualStyleBackColor = True
        '
        'StepLbl
        '
        Me.StepLbl.Location = New System.Drawing.Point(12, 388)
        Me.StepLbl.Name = "StepLbl"
        Me.StepLbl.Size = New System.Drawing.Size(237, 84)
        Me.StepLbl.TabIndex = 12
        '
        'AdjMatrix
        '
        Me.AdjMatrix.AllowUserToAddRows = False
        Me.AdjMatrix.AllowUserToDeleteRows = False
        Me.AdjMatrix.AllowUserToResizeColumns = False
        Me.AdjMatrix.AllowUserToResizeRows = False
        Me.AdjMatrix.BackgroundColor = System.Drawing.SystemColors.Control
        Me.AdjMatrix.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.AdjMatrix.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.AdjMatrix.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.AdjMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AdjMatrix.EnableHeadersVisualStyles = False
        Me.AdjMatrix.Location = New System.Drawing.Point(13, 480)
        Me.AdjMatrix.MultiSelect = False
        Me.AdjMatrix.Name = "AdjMatrix"
        Me.AdjMatrix.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.AdjMatrix.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        Me.AdjMatrix.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.AdjMatrix.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.AdjMatrix.Size = New System.Drawing.Size(236, 150)
        Me.AdjMatrix.TabIndex = 13
        '
        'DistanceTable
        '
        Me.DistanceTable.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DistanceTable.FullRowSelect = True
        Me.DistanceTable.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.DistanceTable.Location = New System.Drawing.Point(1517, 13)
        Me.DistanceTable.MultiSelect = False
        Me.DistanceTable.Name = "DistanceTable"
        Me.DistanceTable.Size = New System.Drawing.Size(270, 440)
        Me.DistanceTable.TabIndex = 14
        Me.DistanceTable.UseCompatibleStateImageBehavior = False
        Me.DistanceTable.View = System.Windows.Forms.View.Details
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1800, 1100)
        Me.Controls.Add(Me.DistanceTable)
        Me.Controls.Add(Me.AdjMatrix)
        Me.Controls.Add(Me.StepLbl)
        Me.Controls.Add(Me.NextButton)
        Me.Controls.Add(Me.BackButton)
        Me.Controls.Add(Me.PathLbl)
        Me.Controls.Add(Me.GetPath)
        Me.Controls.Add(Me.EndOption)
        Me.Controls.Add(Me.StartOption)
        Me.Controls.Add(Me.ClearButton)
        Me.Controls.Add(Me.RandomButton)
        Me.Controls.Add(Me.WeightValue)
        Me.Controls.Add(Me.Weight)
        Me.Controls.Add(Me.Points)
        Me.Controls.Add(Me.Label1)
        Me.DoubleBuffered = True
        Me.MinimumSize = New System.Drawing.Size(1400, 1150)
        Me.Name = "Form1"
        Me.Text = "Dijkstra Simulator"
        CType(Me.Points, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WeightValue, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AdjMatrix, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Points As NumericUpDown
    Friend WithEvents Weight As Label
    Friend WithEvents WeightValue As NumericUpDown
    Friend WithEvents RandomButton As Button
    Friend WithEvents ClearButton As Button
    Friend WithEvents StartOption As ListBox
    Friend WithEvents EndOption As ListBox
    Friend WithEvents GetPath As Button
    Friend WithEvents PathLbl As Label
    Friend WithEvents BackButton As Button
    Friend WithEvents NextButton As Button
    Friend WithEvents StepLbl As Label
    Friend WithEvents AdjMatrix As DataGridView
    Friend WithEvents DistanceTable As ListView
End Class
