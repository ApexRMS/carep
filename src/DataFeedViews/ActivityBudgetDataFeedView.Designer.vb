<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ActivityBudgetDataFeedView
    Inherits SyncroSim.Core.Forms.DataFeedView

    'UserControl overrides dispose to clean up the component list.
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
        Me.PanelProportionEatingMultiplier = New System.Windows.Forms.Panel()
        Me.LabelActPEM = New System.Windows.Forms.Label()
        Me.LabelActivityBudget = New System.Windows.Forms.Label()
        Me.PanelActivityBudget = New System.Windows.Forms.Panel()
        Me.SuspendLayout()
        '
        'PanelProportionEatingMultiplier
        '
        Me.PanelProportionEatingMultiplier.Location = New System.Drawing.Point(6, 26)
        Me.PanelProportionEatingMultiplier.Name = "PanelProportionEatingMultiplier"
        Me.PanelProportionEatingMultiplier.Size = New System.Drawing.Size(216, 91)
        Me.PanelProportionEatingMultiplier.TabIndex = 1
        '
        'LabelActPEM
        '
        Me.LabelActPEM.AutoSize = True
        Me.LabelActPEM.Location = New System.Drawing.Point(5, 8)
        Me.LabelActPEM.Name = "LabelActPEM"
        Me.LabelActPEM.Size = New System.Drawing.Size(135, 13)
        Me.LabelActPEM.TabIndex = 0
        Me.LabelActPEM.Text = "Proportion Eating Multiplier:"
        '
        'LabelActivityBudget
        '
        Me.LabelActivityBudget.AutoSize = True
        Me.LabelActivityBudget.Location = New System.Drawing.Point(5, 131)
        Me.LabelActivityBudget.Name = "LabelActivityBudget"
        Me.LabelActivityBudget.Size = New System.Drawing.Size(81, 13)
        Me.LabelActivityBudget.TabIndex = 2
        Me.LabelActivityBudget.Text = "Activity Budget:"
        '
        'PanelActivityBudget
        '
        Me.PanelActivityBudget.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelActivityBudget.Location = New System.Drawing.Point(6, 149)
        Me.PanelActivityBudget.Name = "PanelActivityBudget"
        Me.PanelActivityBudget.Size = New System.Drawing.Size(702, 393)
        Me.PanelActivityBudget.TabIndex = 3
        '
        'ActivityBudgetDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.LabelActivityBudget)
        Me.Controls.Add(Me.LabelActPEM)
        Me.Controls.Add(Me.PanelActivityBudget)
        Me.Controls.Add(Me.PanelProportionEatingMultiplier)
        Me.Name = "ActivityBudgetDataFeedView"
        Me.Size = New System.Drawing.Size(713, 547)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelProportionEatingMultiplier As System.Windows.Forms.Panel
    Friend WithEvents LabelActPEM As System.Windows.Forms.Label
    Friend WithEvents LabelActivityBudget As System.Windows.Forms.Label
    Friend WithEvents PanelActivityBudget As System.Windows.Forms.Panel

End Class
