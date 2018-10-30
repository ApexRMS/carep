'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable2 of type Double with dimension of StratumID and stratumID
''' </summary>
''' <remarks></remarks>
Class MVStratumPlant
    Inherits ModelVariable2(Of Double)

    Protected Shared m_DefaultDimensions() As MBVariableDimension   'shared field indicating the default VariableDimensions for a particular ModelVariable type

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
    ''' Value for the specified stratumID and plantID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal stratumID As Integer, ByVal plantID As Integer) As Double
        Get
            Return MyBase.Item(stratumID, plantID)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(stratumID, plantID) = value
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
    ''' Completes instantiation as a two-dimensional array, with maximum timestep and stratumID specified as Integers
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxStratumID As Integer, ByVal maxPlantID As Integer)
        Dim maxIndexes() As Integer = {maxStratumID, maxPlantID}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = DATASHEET_STRATUM_ID_COLUMN_NAME   'Set the name for the stratum dimension
        Me.Dimensions(0).DisplayName = StratumDisplayName
        Me.Dimensions(1).Name = DATASHEET_PLANT_ID_COLUMN_NAME   'Set the name for the plant dimension
        Me.Dimensions(1).DisplayName = PlantDisplayName
    End Sub

    ''' <summary>
    ''' Loads, filters and interpolates data from the source datatable - specify only the filter values
    ''' </summary>
    ''' <param name="sourceFilterValues">Array of integer values to filter for</param>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep</remarks>
    Public Overrides Sub Load(ByVal sourceFilterValues As Integer())

        Dim sourceDataView As New DataView
        sourceDataView.Table = Me.m_SourceDataTable
        sourceDataView.RowFilter = String.Format("(Iteration={0} OR Iteration IS NULL)", sourceFilterValues(0))

        If sourceDataView.Count > 0 Then

            Me.EraseValues()

            m_SourceDataTableFiltered = sourceDataView.ToTable
            Dim valCol As String = Me.SourceFieldName

            For Each dr As DataRow In Me.SourceDataTableFiltered.Rows

                Dim MinStratumIndex As Integer = Me.Dimensions(0).MinIndex
                Dim MaxStratumIndex As Integer = Me.Dimensions(0).MaxIndex
                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)
                Dim PlantId As Integer = CInt(dr(DATASHEET_PLANT_ID_COLUMN_NAME))
                Dim PlantIndex As Integer = PlantIDTranslator.IndexFromPKID(PlantId)

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
                                If (Me.HasValue(si, PlantIndex)) Then
                                    Continue For
                                End If
                            End If

                            If (Me.HasValue(si, PlantIndex)) Then

                                Dim msg As String = String.Format(
                                    "Values for stratum are defined more than once for:" & vbCrLf & vbCrLf &
                                    "Property: {0}Vegetation Type: {1}Climate Zone: {2}Development: {3}Plant: {4}",
                                    Me.DisplayName & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelX").ValidationTable.GetDisplayName(x) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelY").ValidationTable.GetDisplayName(y) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_StratumLabelZ").ValidationTable.GetDisplayName(z) & vbCrLf,
                                    m_StratumMultiKeyHelper.Project.GetDataSheet("CAREP_Plant").ValidationTable.GetDisplayName(PlantId))

                                Throw New ArgumentException(msg)

                            End If

                            Me.FillItem(si, PlantIndex) = CDbl(dr(valCol))

                        Next
                    Next
                Next

            Next

        End If

    End Sub

End Class

