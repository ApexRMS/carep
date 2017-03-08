'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization

Module ValidationTables

    Public Function CreateOutputLevelValidationTable() As ValidationTable

        Dim dt As New DataTable("EP_OutputLevel")
        dt.Locale = CultureInfo.InvariantCulture

        dt.Columns.Add(New DataColumn("Value", GetType(Long)))
        dt.Columns.Add(New DataColumn("Display", GetType(String)))

        dt.Rows.Add({CLng(OutputLevel.Low), "Low"})
        dt.Rows.Add({CLng(OutputLevel.Medium), "Medium"})
        dt.Rows.Add({CLng(OutputLevel.High), "High"})

        Return New ValidationTable(dt, "Value", "Display")

    End Function

    Public Function CreateStageValidationTable() As ValidationTable

        Dim dt As New DataTable("EP_Stage")
        dt.Locale = CultureInfo.InvariantCulture

        dt.Columns.Add(New DataColumn("Value", GetType(Long)))
        dt.Columns.Add(New DataColumn("Display", GetType(String)))

        dt.Rows.Add(New Object() {CLng(Stage.Base), "Base"})
        dt.Rows.Add(New Object() {CLng(Stage.Parasites), "Parasites"})
        dt.Rows.Add(New Object() {CLng(Stage.Coat), "Summer Coat"})
        dt.Rows.Add(New Object() {CLng(Stage.Scurf), "Winter Scurf"})
        dt.Rows.Add(New Object() {CLng(Stage.Activity), "Activity"})
        dt.Rows.Add(New Object() {CLng(Stage.SummerProtein), "Summer Protein Dep."})
        dt.Rows.Add(New Object() {CLng(Stage.Gestation), "Gestation"})
        dt.Rows.Add(New Object() {CLng(Stage.Lactation), "Lactation"})
        dt.Rows.Add(New Object() {CLng(Stage.Antlers), "Antlers"})
        dt.Rows.Add(New Object() {CLng(Stage.Protein), "Add. Protein Dep."})
        dt.Rows.Add(New Object() {CLng(Stage.Fat), "Fat Deposition"})

        Return New ValidationTable(dt, "Value", "Display")

    End Function

    Public Function CreateLactationStatusValidationTable() As ValidationTable

        Dim dt As New DataTable("EP_LactationStatus")
        dt.Locale = CultureInfo.InvariantCulture

        dt.Columns.Add(New DataColumn("Value", GetType(Long)))
        dt.Columns.Add(New DataColumn("Display", GetType(String)))

        dt.Rows.Add(New Object() {CLng(LactationStatus.Lactating), "Lactating"})
        dt.Rows.Add(New Object() {CLng(LactationStatus.NotLactating), "Not Lactating"})
        dt.Rows.Add(New Object() {CLng(LactationStatus.AnyStatus), "Any Status"})

        Return New ValidationTable(dt, "Value", "Display")

    End Function

End Module
