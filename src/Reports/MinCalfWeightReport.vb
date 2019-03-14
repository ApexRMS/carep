'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
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

        Dim columns As ExportColumnCollection = Me.CreateColumnCollection()
        Dim data As DataTable = Me.GetReportData()

        If (exportType = ExportType.ExcelFile) Then
            ExportTransformer.ExcelExport(location, columns, data, "Minimal - Calf Weight")
        Else

            Me.CSVExport(location, columns, data)
            FormsUtilities.InformationMessageBox("Data saved to '{0}'.", location)

        End If

    End Sub

    Private Function CreateColumnCollection() As ExportColumnCollection

        Dim c As New ExportColumnCollection()

        c.Add(New ExportColumn("ScenarioID", "Scenario ID"))
        c.Add(New ExportColumn("Iteration"))
        c.Add(New ExportColumn("Timestep"))
        c.Add(New ExportColumn("Year"))
        c.Add(New ExportColumn("JDay", "Julian Day"))
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

    Private Function GetReportData() As DataTable

        Dim Query As String = String.Format(
            "SELECT ScenarioID, " &
            "Iteration, " &
            "Timestep, " &
            "NULL AS Year, " &
            "NULL AS JDay, " &
            "WTBODY, " &
            "WTCALF, " &
            "WTPRO, " &
            "WTFAT, " &
            "AGE, " &
            "CASE CAST(CALFFATE AS INTEGER) WHEN 1 THEN 'No Calf' WHEN 2 THEN 'Lactating' WHEN 3 THEN 'Post Natal Weaner' WHEN 4 THEN 'Summer Weaner' WHEN 5 THEN 'Early Weaner' WHEN 6 THEN 'Normal Weaner' WHEN 7 THEN 'Extended Lactator' WHEN 8 THEN 'In Utero' END AS CALFFATETEXT " &
            "FROM CAREP_OutTimestep WHERE ScenarioID IN ({0})",
            Me.CreateActiveResultScenarioFilter())

        Using store As DataStore = Me.Library.CreateDataStore()

            Dim dt As DataTable = store.CreateDataTableFromQuery(Query, "ReportData")

            For Each dr As DataRow In dt.Rows

                Dim Timestep As Integer = CInt(dr("Timestep"))
                dr("Year") = DayTimeUtils.YearFromTimestep(Timestep, 1, 0) 'Start Julian Day is not used in this function
                dr("JDay") = DayTimeUtils.JulianDayFromTimestep(Timestep)

            Next

            Return dt

        End Using

    End Function

End Class
