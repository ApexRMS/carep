'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

Partial Class CAREPTransformer

    Private Sub NormalizeRunControl()

        Dim DefaultsAdded As Boolean
        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATAFEED_RUN_CONTROL_NAME)
        Dim dr As DataRow = ds.GetDataRow()

        If (dr Is Nothing) Then

            dr = ds.GetData().NewRow()
            ds.GetData().Rows.Add(dr)
            DefaultsAdded = True

        End If

        'The start year is always 1
        dr(DATASHEET_RUN_CONTROL_START_YEAR_COLUMN_NAME) = 1

        If (dr(DATASHEET_RUN_CONTROL_START_JULIAN_DAY_COLUMN_NAME) Is DBNull.Value) Then
            dr(DATASHEET_RUN_CONTROL_START_JULIAN_DAY_COLUMN_NAME) = 1
            DefaultsAdded = True
        End If

        If (dr(DATASHEET_RUN_CONTROL_END_YEAR_COLUMN_NAME) Is DBNull.Value) Then
            dr(DATASHEET_RUN_CONTROL_END_YEAR_COLUMN_NAME) = 1
            DefaultsAdded = True
        End If

        If (dr(DATASHEET_RUN_CONTROL_END_JULIAN_DAY_COLUMN_NAME) Is DBNull.Value) Then
            dr(DATASHEET_RUN_CONTROL_END_JULIAN_DAY_COLUMN_NAME) = 365
            DefaultsAdded = True
        End If

        If (dr(DATASHEET_RUN_CONTROL_NUM_ITERATIONS_COLUMN_NAME) Is DBNull.Value) Then
            dr(DATASHEET_RUN_CONTROL_NUM_ITERATIONS_COLUMN_NAME) = 1
            DefaultsAdded = True
        End If

        If (dr(DATASHEET_RUN_CONTROL_OUTPUT_LEVEL_COLUMN_NAME) Is DBNull.Value) Then
            dr(DATASHEET_RUN_CONTROL_OUTPUT_LEVEL_COLUMN_NAME) = CInt(OutputLevel.Medium)
            DefaultsAdded = True
        End If

        Dim EndYear As Integer = dr(DATASHEET_RUN_CONTROL_END_YEAR_COLUMN_NAME)
        Dim EndJDay As Integer = dr(DATASHEET_RUN_CONTROL_END_JULIAN_DAY_COLUMN_NAME)

        dr("MinimumIteration") = 1
        dr("MaximumIteration") = dr(DATASHEET_RUN_CONTROL_NUM_ITERATIONS_COLUMN_NAME)
        dr("MinimumTimestep") = dr(DATASHEET_RUN_CONTROL_START_YEAR_COLUMN_NAME)
        dr("MaximumTimestep") = TimestepFromJulianDayYear(EndYear, EndJDay)

        If (DefaultsAdded) Then
            Me.RecordStatus(StatusType.Warning, "Some run control values were not specified.  Using defaults.")
        End If

    End Sub

End Class

