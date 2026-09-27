<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.AddNode = New System.Windows.Forms.Button()
        Me.RemoveNode = New System.Windows.Forms.Button()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.Weight = New System.Windows.Forms.NumericUpDown()
        CType(Me.Weight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'AddNode
        '
        Me.AddNode.Location = New System.Drawing.Point(16, 94)
        Me.AddNode.Margin = New System.Windows.Forms.Padding(4)
        Me.AddNode.Name = "AddNode"
        Me.AddNode.Size = New System.Drawing.Size(118, 68)
        Me.AddNode.TabIndex = 0
        Me.AddNode.Text = "Add Node"
        Me.AddNode.UseVisualStyleBackColor = True
        '
        'RemoveNode
        '
        Me.RemoveNode.Location = New System.Drawing.Point(15, 169)
        Me.RemoveNode.Margin = New System.Windows.Forms.Padding(4)
        Me.RemoveNode.Name = "RemoveNode"
        Me.RemoveNode.Size = New System.Drawing.Size(118, 68)
        Me.RemoveNode.TabIndex = 1
        Me.RemoveNode.Text = "Remove Node"
        Me.RemoveNode.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(16, 15)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(114, 29)
        Me.CheckBox1.TabIndex = 2
        Me.CheckBox1.Text = "Weighted"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'Weight
        '
        Me.Weight.Location = New System.Drawing.Point(16, 52)
        Me.Weight.Margin = New System.Windows.Forms.Padding(4)
        Me.Weight.Name = "Weight"
        Me.Weight.Size = New System.Drawing.Size(119, 31)
        Me.Weight.TabIndex = 3
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1233, 712)
        Me.Controls.Add(Me.Weight)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.RemoveNode)
        Me.Controls.Add(Me.AddNode)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Form2"
        Me.Text = "Form2"
        CType(Me.Weight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents AddNode As Button
    Friend WithEvents RemoveNode As Button
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents Weight As NumericUpDown
End Class
