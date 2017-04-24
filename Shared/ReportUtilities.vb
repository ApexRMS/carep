'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Text
Imports System.Globalization
Imports SyncroSim.Core

Module ReportUtilities

    Public Function CreateScenarioFilter(ByVal scenarios As IEnumerable(Of Scenario)) As String

        Dim lst As New List(Of Integer)

        For Each s As Scenario In scenarios
            lst.Add(s.Id)
        Next

        Return CreateScenarioFilter(lst)

    End Function

    Public Function CreateScenarioFilter(ByVal scenarioIds As IEnumerable(Of Integer)) As String

        Dim sb As New StringBuilder()

        For Each id As Integer In scenarioIds
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0},", id)
        Next

        Debug.Assert(scenarioIds.Count > 0)
        Return sb.ToString.TrimEnd(CChar(","))

    End Function

End Module
