'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

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
        Me.AddStandardCommands()

    End Sub

    Private Sub ButtonClearAll_Click(sender As System.Object, e As System.EventArgs) Handles ButtonClearAll.Click
        Me.DataFeed.GetDataSheet("CAREP_RunControl").ClearData()
    End Sub

End Class
