'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

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
