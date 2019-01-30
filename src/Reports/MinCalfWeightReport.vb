'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Reflection
Imports SyncroSim.Core
Imports SyncroSim.Core.Forms

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class MinCalfWeightReport
    Inherits ExportTransformer

    Protected Overrides Sub Export(location As String, exportType As ExportType)

        Dim query As String = Me.CreateReportQuery()
        Dim columns As ExportColumnCollection = Me.CreateColumnCollection()

        If (exportType = ExportType.ExcelFile) Then
            Me.ExcelExport(location, columns, query, "Minimal - Calf Weight")
        Else

            Me.CSVExport(location, columns, query)
            FormsUtilities.InformationMessageBox("Data saved to '{0}'.", location)

        End If

    End Sub

    Private Function CreateColumnCollection() As ExportColumnCollection

        Dim c As New ExportColumnCollection()

        c.Add(New ExportColumn("ScenarioID", "Scenario ID"))
        c.Add(New ExportColumn("ScenarioName", "Scenario Name"))
        c.Add(New ExportColumn("Iteration"))
        c.Add(New ExportColumn("Timestep", "Julian Day"))
        c.Add(New ExportColumn("WTBODY"))
        c.Add(New ExportColumn("WTCALF"))
        c.Add(New ExportColumn("WTPRO"))
        c.Add(New ExportColumn("WTFAT"))
        c.Add(New ExportColumn("AGE"))
        c.Add(New ExportColumn("CALFFATETEXT"))

        c("WTBODY").DecimalPlaces = 8
        c("WTBODY").Alignment = ColumnAlignment.Right

        c("WTCALF").DecimalPlaces = 8
        c("WTCALF").Alignment = ColumnAlignment.Right

        c("WTPRO").DecimalPlaces = 8
        c("WTPRO").Alignment = ColumnAlignment.Right

        c("WTFAT").DecimalPlaces = 8
        c("WTFAT").Alignment = ColumnAlignment.Right

        Return c

    End Function

    Private Function CreateReportQuery() As String

        Dim Query As String = String.Format(
            "SELECT CAREP_OutTimestep.ScenarioID, " &
            "SSim_Scenario.Name as ScenarioName, " &
            "Iteration, " &
            "Timestep, " &
            "WTBODY, " &
            "WTCALF, " &
            "WTPRO, " &
            "WTFAT, " &
            "AGE, " &
            "CASE CAST(CALFFATE AS INTEGER) WHEN 1 THEN 'No Calf' WHEN 2 THEN 'Lactating' WHEN 3 THEN 'Post Natal Weaner' WHEN 4 THEN 'Summer Weaner' WHEN 5 THEN 'Early Weaner' WHEN 6 THEN 'Normal Weaner' WHEN 7 THEN 'Extended Lactator' WHEN 8 THEN 'In Utero' END AS CALFFATETEXT " &
            "FROM CAREP_OutTimestep " &
            "INNER JOIN SSim_Scenario ON SSim_Scenario.ScenarioID=CAREP_OutTimestep.ScenarioID " &
            "WHERE CAREP_OutTimestep.ScenarioID IN ({0})",
            Me.CreateActiveResultScenarioFilter())

        Return Query

    End Function

End Class
