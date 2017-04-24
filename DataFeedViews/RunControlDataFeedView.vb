'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
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

    End Sub

    Private Sub ButtonClearAll_Click(sender As System.Object, e As System.EventArgs) Handles ButtonClearAll.Click
        Me.DataFeed.GetDataSheet("CAREP_RunControl").ClearData()
    End Sub

End Class
