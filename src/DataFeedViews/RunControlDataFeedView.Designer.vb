<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RunControlDataFeedView
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxStartJulianDay = New System.Windows.Forms.TextBox()
        Me.TextBoxEndYear = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxEndJulianDay = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxNumIterations = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ComboBoxOutputLevel = New System.Windows.Forms.ComboBox()
        Me.ButtonClearAll = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Start julian day:"
        '
        'TextBoxStartJulianDay
        '
        Me.TextBoxStartJulianDay.Location = New System.Drawing.Point(154, 18)
        Me.TextBoxStartJulianDay.Name = "TextBoxStartJulianDay"
        Me.TextBoxStartJulianDay.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxStartJulianDay.TabIndex = 1
        '
        'TextBoxEndYear
        '
        Me.TextBoxEndYear.Location = New System.Drawing.Point(154, 41)
        Me.TextBoxEndYear.Name = "TextBoxEndYear"
        Me.TextBoxEndYear.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxEndYear.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(18, 42)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "End year:"
        '
        'TextBoxEndJulianDay
        '
        Me.TextBoxEndJulianDay.Location = New System.Drawing.Point(154, 65)
        Me.TextBoxEndJulianDay.Name = "TextBoxEndJulianDay"
        Me.TextBoxEndJulianDay.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxEndJulianDay.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(18, 67)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "End julian day:"
        '
        'TextBoxNumIterations
        '
        Me.TextBoxNumIterations.Location = New System.Drawing.Point(154, 89)
        Me.TextBoxNumIterations.Name = "TextBoxNumIterations"
        Me.TextBoxNumIterations.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxNumIterations.TabIndex = 7
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(18, 92)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(104, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Number of iterations:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(18, 130)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(67, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Output level:"
        '
        'ComboBoxOutputLevel
        '
        Me.ComboBoxOutputLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxOutputLevel.FormattingEnabled = True
        Me.ComboBoxOutputLevel.Location = New System.Drawing.Point(154, 127)
        Me.ComboBoxOutputLevel.Name = "ComboBoxOutputLevel"
        Me.ComboBoxOutputLevel.Size = New System.Drawing.Size(109, 21)
        Me.ComboBoxOutputLevel.TabIndex = 9
        '
        'ButtonClearAll
        '
        Me.ButtonClearAll.Location = New System.Drawing.Point(154, 154)
        Me.ButtonClearAll.Name = "ButtonClearAll"
        Me.ButtonClearAll.Size = New System.Drawing.Size(109, 23)
        Me.ButtonClearAll.TabIndex = 10
        Me.ButtonClearAll.Text = "Clear All"
        Me.ButtonClearAll.UseVisualStyleBackColor = True
        '
        'RunControlDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ButtonClearAll)
        Me.Controls.Add(Me.ComboBoxOutputLevel)
        Me.Controls.Add(Me.TextBoxNumIterations)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxEndJulianDay)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBoxEndYear)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxStartJulianDay)
        Me.Controls.Add(Me.Label1)
        Me.Name = "RunControlDataFeedView"
        Me.Size = New System.Drawing.Size(281, 189)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBoxStartJulianDay As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxEndYear As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxEndJulianDay As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBoxNumIterations As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxOutputLevel As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonClearAll As System.Windows.Forms.Button

End Class
