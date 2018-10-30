'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable1 with dimension of timestep
''' </summary>
''' <remarks></remarks>
Class MVTime
    Inherits ModelVariable1(Of Double)

    Protected Shared m_StartYear As Integer = MinYear   'shared field indicating the start year
    Protected Shared m_EndYear As Integer = MaxYear
    Protected Shared m_StartJday As Integer = MinJDay   'shared field indicating the start julian day
    'Protected Shared m_EndYear As Integer = MaxYear   'shared field indicating the end year
    'Protected Shared m_EndJday As Integer = MaxJDay   'shared field indicating the end julian day
    Protected Shared m_DefaultDimensions() As MBVariableDimension   'shared field indicating the default VariableDimensions for a particular ModelVariable type
    Protected Shared m_DefaultDimensionsSource() As MBVariableDimension   'shared field indicating the default VariableDimensions for the shadow source ModelVariable
    Private m_mvSource As ModelVariable2(Of Double)  'instance variable containing source data with Year & Jday

    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0 and name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
    End Sub

    ''' <summary>
    ''' Gets the MVSource
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MVSource As ModelVariable2(Of Double)
        Get
            Return Me.m_mvSource
        End Get
    End Property

    ''' <summary>
    ''' Shared class variable with the default VariableDimensions for the class
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property DefaultDimensions() As MBVariableDimension()
        Get
            Return m_DefaultDimensions
        End Get
        Set(ByVal value As MBVariableDimension())
            m_DefaultDimensions = value
        End Set
    End Property
    ''' <summary>
    ''' Shared class variable with the default VariableDimensions for the shadow source model variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property DefaultDimensionsSource() As MBVariableDimension()
        Get
            Return m_DefaultDimensionsSource
        End Get
        Set(ByVal value As MBVariableDimension())
            m_DefaultDimensionsSource = value
        End Set
    End Property
    ''' <summary>
    ''' Shared class variable with the Start Year for the class
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property StartYear() As Integer
        Get
            Return m_StartYear
        End Get
        Set(ByVal value As Integer)
            m_StartYear = value
        End Set
    End Property
    ''' <summary>
    ''' Shared class variable with the End Year for the class
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property EndYear() As Integer
        Get
            Return m_EndYear
        End Get
        Set(ByVal value As Integer)
            m_EndYear = value
        End Set
    End Property
    ''' <summary>
    ''' Shared class variable with the Start Julian Day for the class
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property StartJday() As Integer
        Get
            Return m_StartJday
        End Get
        Set(ByVal value As Integer)
            m_StartJday = value
        End Set
    End Property
    ''' <summary>
    ''' Value for the specified timestep
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal timestep As Integer) As Double
        Get
            Return MyBase.Item(timestep)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(timestep) = value
        End Set
    End Property
    ''' <summary>
    ''' Resets dimensions as a multi-dimensional array, using DefaultDimensions for the ModelVariable type
    ''' </summary>
    ''' <remarks></remarks>
    Public Overloads Sub CreateDimensions()
        Me.CreateDimensions(m_DefaultDimensions)   'uses class shared variable
    End Sub
    ''' <summary>
    ''' Completes instantiation as a one-dimensional array, with maximum timestep specified as an Integer
    ''' </summary>
    ''' <param name="maxTimestep">Maximum value of the daily timestep</param>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxTimestep As Integer)
        Dim maxIndexes() As Integer = {maxTimestep}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = TimeStepFieldName   ' set the default field name to a constant
    End Sub
    ''' <summary>
    ''' Establishes the data source specifying the source datatable and the array of field names for filtering
    ''' </summary>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep
    ''' </remarks>
    Public Overrides Sub DataSource(ByVal sourceYearJDayDataTable As DataTable, ByVal filterFieldNames() As String)
        'Sets the source for the internal shadow ModelVariable storing data in Year/Jday format
        m_mvSource = New ModelVariable2(Of Double)(Me.Name)  'instantiate the shadow source MV with same name
        m_mvSource.CreateDimensions(MVTime.DefaultDimensionsSource)  'assign default dimensions for shadow source
        m_mvSource.DataSource(sourceYearJDayDataTable, filterFieldNames)
    End Sub
    ''' <summary>
    ''' Loads, filters and interpolates data from the source datatable - specify only the filter values
    ''' </summary>
    ''' <param name="sourceFilterValues">Array of integer values to filter for</param>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep</remarks>
    Public Overrides Sub Load(ByVal sourceFilterValues As Integer())

        'Only load new values if values exist for this filter; otherwise keep the values from the previous filter
        'Note that we also need to check for a NULL RUN value, and if it is NULL, only use it if there has not already
        'been a value set at that location.  In other words, runs with explicit values always supercede runs with NULL
        'values.  DEVTODO: It would be nice to put this in the base class, but the base class doesn't know what a RUN
        'is, and there is currently no mechanism for more complex queries such as the one above.

        Dim sourceDataView As New DataView
        sourceDataView.Table = Me.m_mvSource.SourceDataTable
        sourceDataView.RowFilter = String.Format("(Iteration={0} OR Iteration IS NULL) AND (Timestep <= {1} OR Timestep IS NULL)", sourceFilterValues(0), MVTime.m_EndYear)

        If sourceDataView.Count > 0 Then

            Me.EraseValues()

            m_SourceDataTableFiltered = sourceDataView.ToTable
            Dim valCol As String = Me.SourceFieldName

            For Each dr As DataRow In Me.SourceDataTableFiltered.Rows

                Dim yr As Integer = MVTime.m_StartYear

                If (dr(DATASHEET_TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                    yr = CInt(dr(DATASHEET_TIMESTEP_COLUMN_NAME))
                End If

                Dim jd As Integer = CInt(dr(DATASHEET_JULIAN_DAY_COLUMN_NAME))
                Dim timestep As Integer = TimestepFromJulianDayYear(yr, jd)
                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)

                If (IterationNull) Then

                    If (Me.HasValue(timestep)) Then
                        Continue For
                    End If

                End If

                Me.FillItem(timestep) = CDbl(dr(valCol))

            Next

            'Converts this source data from Year/Jday to Timestep and interpolates over timesteps
            'Data is also transferred to the regular model variable

            Me.FillFromYearJDay(m_mvSource, m_StartYear, m_StartJday)

        End If

    End Sub

    ''' <summary>
    ''' Fills and interpolates from an existing source model variable with dimensions year, julian day
    ''' </summary>
    ''' <param name="mvYearJDay">source ModelVariable2 with dimensions year, julian day (in order)</param>
    ''' <param name="startYear">year corresponding to timestep 0 for the source model variable</param>
    ''' <param name="startJDay">julian day corresponding to timestep 0 for the source model variable</param>
    ''' <remarks>Assumes that the dimensions of the source model variable are in order of plant, year, julian day</remarks>
    Public Sub FillFromYearJDay(ByVal mvYearJDay As ModelVariable2(Of Double), ByVal startYear As Integer, ByVal startJDay As Integer)

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
        'Me.Dimensions(0).Name = TimeStepFieldName   'Set the fieldname for the time dimension using string constant

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

        'Finally, interpolate between missing values
        InterpolateModelVariableByYear(Me, minYr, maxYr)

        'Copy missing yearly data from nearest year that has data
        CopyMissingYearlyData(Me, minYr, maxYr)

    End Sub

End Class

