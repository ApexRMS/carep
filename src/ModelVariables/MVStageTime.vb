'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable2 of type Double with dimension of StageID and daily timestep
''' </summary>
''' <remarks></remarks>
Class MVStageTime
    Inherits ModelVariable2(Of Double)
    Protected Shared m_StartYear As Integer = MinYear   'shared field indicating the start year
    Protected Shared m_EndYear As Integer = MaxYear
    Protected Shared m_StartJday As Integer = MinJDay   'shared field indicating the start julian day
    Protected Shared m_DefaultDimensions() As MBVariableDimension   'shared field indicating the default VariableDimensions for a particular ModelVariable type
    Protected Shared m_DefaultDimensionsSource() As MBVariableDimension   'shared field indicating the default VariableDimensions for the shadow source ModelVariable
    Private m_mvSource As ModelVariable3(Of Double)  'instance variable containing source data with Year & Jday

    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0 and name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
    End Sub
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
    ''' Value for the specified StageID and timestep
    ''' </summary>
    ''' <param name="StageID">Index value for Stage ID</param>
    ''' <param name="timestep">Index value for daily timestep</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal StageID As Integer, ByVal timestep As Integer) As Double
        Get
            Return MyBase.Item(StageID, timestep)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(StageID, timestep) = value
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
    ''' Completes instantiation as a two-dimensional array, with maximum timestep and StageID specified as Integers
    ''' </summary>
    ''' <param name="maxTimestep">Maximum value of the daily timestep</param>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxStageID As Integer, ByVal maxTimestep As Integer)
        Dim maxIndexes() As Integer = {maxStageID, maxTimestep}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = DATASHEET_STAGE_ID_COLUMN_NAME   'Set the name for the Stage dimension
        Me.Dimensions(0).DisplayName = StageDisplayName
        Me.Dimensions(1).Name = TimeStepFieldName   'Set the name for the time dimension using string constant
    End Sub
    ''' <summary>
    ''' Establishes the data source specifying the source datatable and the array of field names for filtering
    ''' </summary>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep
    ''' </remarks>
    Public Overrides Sub DataSource(ByVal sourceYearJDayDataTable As DataTable, ByVal filterFieldNames() As String)
        'Sets the source for the internal shadow ModelVariable storing data in Year/Jday format
        m_mvSource = New ModelVariable3(Of Double)(Me.Name)  'instantiate the shadow source MV with same name
        m_mvSource.CreateDimensions(MVStageTime.DefaultDimensionsSource)  'assign default dimensions for shadow source
        m_mvSource.DataSource(sourceYearJDayDataTable, filterFieldNames)
    End Sub
    ''' <summary>
    ''' Loads, filters and interpolates data from the source datatable - specify only the filter values
    ''' </summary>
    ''' <param name="sourceFilterValues">Array of integer values to filter for</param>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep</remarks>
    Public Overrides Sub Load(ByVal sourceFilterValues As Integer())
        'Loads to the internal shadow ModelVariable storing data in Year/Jday format
        Me.EraseValues()
        m_mvSource.Load(sourceFilterValues)
        'Converts this source data from Year/Jday to Timestep and interpolates over timesteps
        'Data is also transferred to the regular model variable
        Me.FillFromYearJDay(m_mvSource, m_StartYear, m_StartJday)
    End Sub
    ''' <summary>
    ''' Fills and interpolates from an existing source model variable with dimensions Stage, year, julian day
    ''' </summary>
    ''' <param name="mvStageYearJDay">source ModelVariable3 with dimensions Stage, year, julian day (in order)</param>
    ''' <param name="startYear">year corresponding to timestep 0 for the source model variable</param>
    ''' <param name="startJDay">julian day corresponding to timestep 0 for the source model variable</param>
    ''' <remarks>Assumes that the dimensions of the source model variable are in order of Stage, year, julian day</remarks>
    Public Sub FillFromYearJDay(ByVal mvStageYearJDay As ModelVariable3(Of Double), ByVal startYear As Integer, ByVal startJDay As Integer)

        Dim p, yr, jDay, timestep As Integer
        Dim minP, maxP, minTimestep, maxTimestep, minYr, maxYr, minJday, maxJDay As Integer

        minP = Me.Dimensions(0).MinIndex
        maxP = Me.Dimensions(0).MaxIndex
        minTimestep = Me.Dimensions(1).MinIndex
        maxTimestep = Me.Dimensions(1).MaxIndex
        minYr = mvStageYearJDay.Dimensions(1).MinIndex
        maxYr = mvStageYearJDay.Dimensions(1).MaxIndex
        minJday = mvStageYearJDay.Dimensions(2).MinIndex
        maxJDay = mvStageYearJDay.Dimensions(2).MaxIndex

        m_SourceFieldName = mvStageYearJDay.SourceFieldName   ' pass the source field name to the new ModelVariable

        For p = minP To maxP
            For yr = minYr To maxYr
                For jDay = minJday To maxJDay
                    timestep = TimestepFromJulianDayYear(yr, jDay)
                    If timestep >= 0 And timestep <= maxTimestep Then
                        If mvStageYearJDay.FromSource(p, yr, jDay) Then
                            'source element was filled from a data source
                            Me.FillItem(p, timestep) = mvStageYearJDay(p, yr, jDay)
                        End If
                    End If
                Next
            Next
        Next

        'Finally, interpolate between missing values
        InterpolateModelVariableByYear(Me, minP, maxP, minYr, maxYr)

        'Copy missing yearly data from nearest year that has data
        CopyMissingYearlyData(Me, minP, maxP, minYr, maxYr)

    End Sub
End Class

