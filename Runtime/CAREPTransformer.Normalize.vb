'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
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

        Me.m_StartYear = dr(DATASHEET_RUN_CONTROL_START_YEAR_COLUMN_NAME)
        Me.m_StartJDay = dr(DATASHEET_RUN_CONTROL_START_JULIAN_DAY_COLUMN_NAME)
        Me.m_EndYear = dr(DATASHEET_RUN_CONTROL_END_YEAR_COLUMN_NAME)
        Me.m_EndJDay = dr(DATASHEET_RUN_CONTROL_END_JULIAN_DAY_COLUMN_NAME)
        Me.m_OutputLevel = CType(dr(DATASHEET_RUN_CONTROL_OUTPUT_LEVEL_COLUMN_NAME), OutputLevel)

        dr("MinimumIteration") = 1
        dr("MaximumIteration") = dr(DATASHEET_RUN_CONTROL_NUM_ITERATIONS_COLUMN_NAME)
        dr("MinimumTimestep") = Me.m_StartYear
        dr("MaximumTimestep") = TimestepFromJulianDayYear(m_EndYear, m_EndJDay)

        Me.MinimumIteration = 1
        Me.MaximumIteration = CInt(dr(DATASHEET_RUN_CONTROL_NUM_ITERATIONS_COLUMN_NAME))
        Me.MinimumTimestep = Me.m_StartYear
        Me.MaximumTimestep = TimestepFromJulianDayYear(m_EndYear, m_EndJDay)

        Debug.Assert(Me.m_StartYear > 0)

        If (DefaultsAdded) Then
            Me.RecordStatus(StatusType.Warning, "Some run control values were not specified.  Using defaults.")
        End If

    End Sub

End Class

