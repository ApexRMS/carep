'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' Special case derivation of MVTime for STRATUM_ID
''' </summary>
''' <remarks></remarks>
Class MVStratum_ID_Time
    Inherits MVTime

    Public Sub New(ByVal name As String)
        MyBase.New(name)
    End Sub

    Public Overrides Sub CreateDimensions(dimensions() As MBVariableDimension)
        MyBase.CreateDimensions(dimensions)
    End Sub

    Public Overrides Sub Load(sourceFilterValues() As Integer)

        Debug.Assert(Me.Values.Count > 0)

        'Only load new values if values exist for this filter; otherwise keep the values from the previous filter
        'Note that we also need to check for a NULL RUN value, and if it is NULL, only use it if there has not already
        'been a value set at that location.  In other words, runs with explicit values always supercede runs with NULL
        'values.  DEVTODO: It would be nice to put this in the base class, but the base class doesn't know what a RUN
        'is, and there is currently no mechanism for more complex queries such as the one above.

        Dim sourceDataView As New DataView
        sourceDataView.Table = Me.MVSource.SourceDataTable
        sourceDataView.RowFilter = String.Format("(Iteration={0} OR Iteration IS NULL)", sourceFilterValues(0))

        If sourceDataView.Count > 0 Then

            Me.EraseValues()

            m_SourceDataTableFiltered = sourceDataView.ToTable
            Dim valCol As String = Me.SourceFieldName

            For Each dr As DataRow In Me.SourceDataTableFiltered.Rows

                Dim yr As Integer = CInt(dr(DATASHEET_TIMESTEP_COLUMN_NAME))
                Dim jd As Integer = CInt(dr(DATASHEET_JULIAN_DAY_COLUMN_NAME))
                Dim timestep As Integer = TimestepFromJulianDayYear(yr, jd)
                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)
                Dim value As Integer = StratumIDTranslator.IndexFromPKID(CInt(dr(valCol)))

                If (IterationNull) Then

                    If (Me.HasValue(timestep)) Then
                        Continue For
                    End If

                End If

                Me.FillItem(timestep) = CDbl(value)

            Next

        End If

        Me.FillStratumValues(MVSource, m_StartYear, m_StartJday)

    End Sub

    Public Sub FillStratumValues(ByVal mvYearJDay As ModelVariable2(Of Double), ByVal startYear As Integer, ByVal startJDay As Integer)

        Dim yr, jDay, timestep As Integer
        Dim minTimestep, maxTimestep As Integer
        Dim minYr, maxYr As Integer
        Dim minJDay, maxJDay As Integer

        'Set bounds of each dimension
        minTimestep = Me.Dimensions(0).MinIndex
        maxTimestep = Me.Dimensions(0).MaxIndex
        minYr = mvYearJDay.Dimensions(0).MinIndex
        maxYr = mvYearJDay.Dimensions(0).MaxIndex
        minJDay = mvYearJDay.Dimensions(1).MinIndex
        maxJDay = mvYearJDay.Dimensions(1).MaxIndex

        m_SourceFieldName = mvYearJDay.SourceFieldName   ' pass the source field name to the new ModelVariable

        For yr = minYr To maxYr
            For jDay = minJDay To maxJDay
                timestep = TimestepFromJulianDayYear(yr, jDay)
                If timestep >= 0 And timestep <= maxTimestep Then
                    If mvYearJDay.FromSource(yr, jDay) Then
                        'source element was filled from a data source
                        Me.FillItem(timestep) = mvYearJDay(yr, jDay)
                    End If
                End If
            Next
        Next

        Dim CurrentValue As Double = Me.Values(0)
        Dim ValueToSet As Double = CurrentValue

        If (ValueToSet = 0) Then
            ValueToSet = 1.0
        End If

        For i As Integer = 0 To Me.Values.Count - 1

            If (Me.Values(i) <> CurrentValue And Me.Values(i) <> 0) Then

                CurrentValue = Me.Values(i)
                ValueToSet = CurrentValue

            End If

            Me.Values(i) = ValueToSet

        Next

    End Sub

End Class
