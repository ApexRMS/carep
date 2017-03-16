'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class RunControlDataFeedView

    Public Overrides Sub LoadDataFeed(dataFeed As DataFeed)

        MyBase.LoadDataFeed(dataFeed)

        Me.SetTextBoxBinding(Me.TextBoxStartJulianDay, "StartJulianDay")
        Me.SetTextBoxBinding(Me.TextBoxEndYear, "EndYear")
        Me.SetTextBoxBinding(Me.TextBoxEndJulianDay, "EndJulianDay")
        Me.SetTextBoxBinding(Me.TextBoxNumIterations, "NumIterations")
        Me.SetComboBoxBinding(Me.ComboBoxOutputLevel, "OutputLevel")

        Me.RefreshBoundControls()

    End Sub

    Private Sub ButtonClearAll_Click(sender As System.Object, e As System.EventArgs) Handles ButtonClearAll.Click
        Me.DataFeed.GetDataSheet("CAREP_RunControl").ClearData()
    End Sub

End Class
