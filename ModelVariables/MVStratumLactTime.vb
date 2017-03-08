'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

''' <summary>
''' ModelVariable3 of type Double with dimension of stratumID, lactation status and daily timestep
''' </summary>
''' <remarks></remarks>
Class MVStratumLactTime
    Inherits ModelVariable3(Of Double)

    Protected Shared m_StartYear As Integer = MinYear   'shared field indicating the start year
    Protected Shared m_EndYear As Integer = MaxYear
    Protected Shared m_StartJday As Integer = MinJDay   'shared field indicating the start julian day
    Protected Shared m_DefaultDimensionsSource() As MBVariableDimension   'shared field indicating the default VariableDimensions for the shadow source ModelVariable
    Protected Shared m_DefaultDimensions() As MBVariableDimension   'shared field indicating the default VariableDimensions for a particular ModelVariable type
    Private m_mvSource As ModelVariable4(Of Double)  'instance variable containing source data with Year & Jday

    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0 and name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
    End Sub
    ''' <summary>
    ''' Shared class variable with the default VariableDimensions for the type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Shared Property DefaultDimensions() As MBVariableDimension()
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
    ''' Value for the specified stratumID, lactStatus and timestep
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal stratumID As Integer, ByVal lactStatus As Integer, ByVal timestep As Integer) As Double
        Get
            Return MyBase.Item(stratumID, lactStatus, timestep)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(stratumID, lactStatus, timestep) = value
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
    ''' Completes instantiation as a thre-dimensional array, with maximum stratumID, lactStatus and timestep specified as Integers
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxStratumID As Integer, ByVal maxLactStatus As Integer, ByVal maxTimestep As Integer)
        Dim maxIndexes() As Integer = {maxStratumID, maxLactStatus, maxTimestep}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = DATASHEET_STRATUM_ID_COLUMN_NAME   'Set the name for the stratum dimension
        Me.Dimensions(0).DisplayName = StratumDisplayName
        Me.Dimensions(1).Name = "LactationStatus"
        Me.Dimensions(1).DisplayName = "Lactation Status"
        Me.Dimensions(2).Name = TimeStepFieldName   'Set the name for the time dimension using string constant
    End Sub
    ''' <summary>
    ''' Establishes the data source specifying the source datatable and the array of field names for filtering
    ''' </summary>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep
    ''' </remarks>
    Public Overrides Sub DataSource(ByVal sourceYearJDayDataTable As DataTable, ByVal filterFieldNames() As String)
        'Sets the source for the internal shadow ModelVariable storing data in Year/Jday format
        m_mvSource = New ModelVariable4(Of Double)(Me.Name)  'instantiate the shadow source MV with same name
        m_mvSource.CreateDimensions(MVStratumLactTime.DefaultDimensionsSource)  'assign default dimensions for shadow source
        m_mvSource.DataSource(sourceYearJDayDataTable, filterFieldNames)
    End Sub
    ''' <summary>
    ''' Loads, filters and interpolates data from the source datatable - specify only the filter values
    ''' </summary>
    ''' <param name="sourceFilterValues">Array of integer values to filter for</param>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep</remarks>
    Public Overrides Sub Load(ByVal sourceFilterValues As Integer())

        Debug.Assert(CInt(LactationStatus.Lactating) = 1)
        Debug.Assert(CInt(LactationStatus.NotLactating) = 2)
        Debug.Assert(CInt(LactationStatus.AnyStatus) = 3)

        Dim sourceDataView As New DataView
        sourceDataView.Table = Me.m_mvSource.SourceDataTable
        sourceDataView.RowFilter = String.Format("(Iteration={0} OR Iteration IS NULL) AND (Timestep <= {1} OR Timestep IS NULL)", sourceFilterValues(0), MVStratumLactTime.m_EndYear)

        'Only load new values if values exist for this filter; otherwise keep the values from the previous filter
        'Note that we also need to check for a NULL RUN value, and if it is NULL, only use it if there has not already
        'been a value set at that location.  In other words, runs with explicit values always supercede runs with NULL
        'values.  DEVTODO: It would be nice to put this in the base class, but the base class doesn't know what a RUN
        'is, and there is currently no mechanism for more complex queries such as the one above.

        If sourceDataView.Count > 0 Then

            Me.EraseValues()

            m_SourceDataTableFiltered = sourceDataView.ToTable
            Dim valCol As String = Me.SourceFieldName

            For Each dr As DataRow In Me.SourceDataTableFiltered.Rows

                Dim yr As Integer = MVStratumLactTime.m_StartYear

                If (dr(DATASHEET_TIMESTEP_COLUMN_NAME) IsNot DBNull.Value) Then
                    yr = CInt(dr(DATASHEET_TIMESTEP_COLUMN_NAME))
                End If

                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)
                Dim MinStratumIndex As Integer = Me.Dimensions(0).MinIndex
                Dim MaxStratumIndex As Integer = Me.Dimensions(0).MaxIndex
                Dim jd As Integer = CInt(dr(DATASHEET_JULIAN_DAY_COLUMN_NAME))
                Dim timestep As Integer = TimestepFromJulianDayYear(yr, jd)
                Dim LactStatus As LactationStatus = CType(dr("LactationStatus"), LactationStatus)

                Dim GetMultiKeyList As Func(Of String, String, List(Of Integer)) =
                    Function(sheetName As String, colName As String)

                        Dim l As New List(Of Integer)

                        If (dr(colName) Is DBNull.Value) Then

                            For Each drs As DataRow In StratumMultiKeyHelper.Project.GetDataSheet(sheetName).GetData.Rows
                                l.Add(drs(colName))
                            Next

                        Else
                            l.Add(dr(colName))
                        End If

                        Return l

                    End Function

                Dim SLXList As List(Of Integer) = GetMultiKeyList("CAREP_StratumLabelX", "StratumLabelXID")
                Dim SLYList As List(Of Integer) = GetMultiKeyList("CAREP_StratumLabelY", "StratumLabelYID")
                Dim SLZList As List(Of Integer) = GetMultiKeyList("CAREP_StratumLabelZ", "StratumLabelZID")

                For Each x As Integer In SLXList
                    For Each y As Integer In SLYList
                        For Each z As Integer In SLZList

                            If (Not m_StratumMultiKeyHelper.CanGetStratumID(x, y, z)) Then

                                Dim msg As String = String.Format(
                                    "No stratum has been defined for:" & vbCrLf & vbCrLf &
                                    "Vegetation Type: {0}Climate Zone: {1}Development: {2}",
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelX").ValidationTable.GetDisplayName(x) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelY").ValidationTable.GetDisplayName(y) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelZ").ValidationTable.GetDisplayName(z))

                                Throw New ArgumentException(msg)

                            End If

                            Dim id As Integer = m_StratumMultiKeyHelper.GetStratumID(x, y, z)
                            Dim si As Integer = StratumIDTranslator.IndexFromPKID(id)

                            If (IterationNull) Then

                                If (Me.HasValue(si, LactStatus, timestep)) Then
                                    Continue For
                                End If

                            End If

                            If (Me.HasValue(si, LactStatus, timestep)) Then

                                Dim la As String = Nothing

                                If (LactStatus = LactationStatus.AnyStatus) Then
                                    la = "Any Status"
                                ElseIf (LactStatus = LactationStatus.Lactating) Then
                                    la = "Lactating"
                                Else
                                    la = "Not Lactating"
                                End If

                                Dim msg As String = String.Format(
                                    "Values for stratum are defined more than once for:" & vbCrLf & vbCrLf &
                                    "Property: {0}Vegetation Type: {1}Climate Zone: {2}Development: {3}Lactation Status: {4}Year: {5}",
                                    Me.DisplayName & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelX").ValidationTable.GetDisplayName(x) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelY").ValidationTable.GetDisplayName(y) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelZ").ValidationTable.GetDisplayName(z) & vbCrLf,
                                    la & vbCrLf,
                                    timestep)

                                Throw New ArgumentException(msg)

                            End If

                            If (LactStatus = LactationStatus.AnyStatus) Then

                                Me.FillItem(si, LactationStatus.Lactating, timestep) = CDbl(dr(valCol))
                                Me.FillItem(si, LactationStatus.NotLactating, timestep) = CDbl(dr(valCol))

                            Else
                                Me.FillItem(si, LactStatus, timestep) = CDbl(dr(valCol))
                            End If

                        Next
                    Next
                Next

            Next

            'Converts this source data from Year/Jday to Timestep and interpolates over timesteps
            'Data is also transferred to the regular model variable

            Me.FillFromYearJDay(m_mvSource, m_StartYear, m_StartJday)

        End If

    End Sub

    ''' <summary>
    ''' Fills and interpolates from an existing source Double model variable with dimensions stratum, LactStatus, year, julian day
    ''' </summary>
    ''' <param name="mvStratumLactYearJDay">source ModelVariable4 with dimensions stratum, lactStatus, year, julian day (in order)</param>
    ''' <param name="startYear">year corresponding to timestep 0 for the source model variable</param>
    ''' <param name="startJDay">julian day corresponding to timestep 0 for the source model variable</param>
    ''' <remarks>Assumes that the dimensions of the source model variable are in order of stratum, LactStatus, year, julian day</remarks>
    Public Sub FillFromYearJDay(ByVal mvStratumLactYearJDay As ModelVariable4(Of Double), ByVal startYear As Integer, ByVal startJDay As Integer)
        Dim s, p, yr, jDay, timestep As Integer
        Dim minS, maxS, minP, maxP, minTimestep, maxTimestep, minYr, maxYr, minJday, maxJDay As Integer

        minS = Me.Dimensions(0).MinIndex
        maxS = Me.Dimensions(0).MaxIndex
        minP = Me.Dimensions(1).MinIndex
        maxP = Me.Dimensions(1).MaxIndex
        minTimestep = Me.Dimensions(2).MinIndex
        maxTimestep = Me.Dimensions(2).MaxIndex
        minYr = mvStratumLactYearJDay.Dimensions(2).MinIndex
        maxYr = mvStratumLactYearJDay.Dimensions(2).MaxIndex
        minJday = mvStratumLactYearJDay.Dimensions(3).MinIndex
        maxJDay = mvStratumLactYearJDay.Dimensions(3).MaxIndex

        m_SourceFieldName = mvStratumLactYearJDay.SourceFieldName   ' pass the source field name to the new ModelVariable

        For s = minS To maxS
            For p = minP To maxP
                For yr = minYr To maxYr
                    For jDay = minJday To maxJDay
                        timestep = TimestepFromJulianDayYear(yr, jDay)
                        If timestep >= 0 And timestep <= maxTimestep Then
                            If mvStratumLactYearJDay.FromSource(s, p, yr, jDay) Then
                                'source element was filled from a data source
                                Me.FillItem(s, p, timestep) = mvStratumLactYearJDay(s, p, yr, jDay)
                            End If
                        End If
                    Next
                Next
            Next
        Next

        'Finally, interpolate between missing values
        InterpolateModelVariableByYear(Me, minS, maxS, minP, maxP, minYr, maxYr)

        'Copy missing yearly data from nearest year that has data
        CopyMissingYearlyData(Me, minS, maxS, minP, maxP, minYr, maxYr)

    End Sub

End Class
