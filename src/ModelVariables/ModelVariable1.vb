'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable with 1 dimension
''' </summary>
''' <typeparam name="T">Generic type T</typeparam>
''' <remarks></remarks>
Class ModelVariable1(Of T)
    Inherits ModelVariable(Of T)
    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        Me.CreateDimensions(0)
    End Sub
    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0 and name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
        Me.CreateDimensions(0)
    End Sub
    ''' <summary>
    ''' Single value at index position
    ''' </summary>
    ''' <remarks></remarks>
    Default Public Property Item(ByVal index As Integer) As T
        Get
            Return Me.Values(index - m_Dimensions(0).MinIndex)
        End Get
        Set(ByVal value As T)
            Me.Values(index - m_Dimensions(0).MinIndex) = value
        End Set
    End Property
    ''' <summary>
    ''' Resets dimensions for a one-dimensional array, with maximum dimension specified as an Integer
    ''' </summary>
    ''' <param name="maxIndex">Maximum value of the index</param>
    ''' <remarks></remarks>
    Public Overridable Overloads Sub CreateDimensions(ByVal maxIndex As Integer)
        Dim maxIndexes() As Integer = {maxIndex}
        Me.CreateDimensions(maxIndexes)
    End Sub
    ''' <summary>
    ''' Fills all elements of a one-dimensional model variable based on filtered source
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Fill()
        Dim xCol As String = m_Dimensions(0).SourceFieldName
        Dim valCol As String = Me.SourceFieldName
        Dim xIndex As Integer
        Dim row As DataRow
        For Each row In m_SourceDataTableFiltered.Rows
            xIndex = CInt(row(xCol))
            Me.FillItem(xIndex) = CType(row(valCol), T)
        Next
    End Sub

    Public Overrides ReadOnly Property IsFromSource(index As Integer) As Boolean
        Get
            Dim xIndexBase As Integer = index - m_Dimensions(0).MinIndex
            Debug.Assert(xIndexBase >= 0)
            Return Me.m_IsFromSource(xIndexBase)
        End Get
    End Property

    Public Overrides ReadOnly Property HasValue(index As Integer) As Boolean
        Get
            Dim xIndexBase As Integer = index - m_Dimensions(0).MinIndex
            Debug.Assert(xIndexBase >= 0)
            Return Me.m_HasValue(xIndexBase)
        End Get
    End Property

    ''' <summary>
    ''' Value indexed according to the 2-dimension index pair filled from data source; IsFromSource value set to true
    ''' </summary>
    ''' <param name="xIndex">Index value for x-dimension</param>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Overridable Overloads WriteOnly Property FillItem(ByVal xIndex As Integer) As T
        Set(ByVal value As T)

            Dim xIndexBase As Integer = xIndex - m_Dimensions(0).MinIndex
            Debug.Assert(xIndexBase >= 0)

            If (xIndexBase < Me.m_Values.Length) Then

                m_Values(xIndexBase) = value
                m_IsFromSource(xIndexBase) = True
                m_HasValue(xIndexBase) = True

            End If

        End Set
    End Property

    ''' <summary>
    ''' Writes contents of all the MBOutputTable's ModelVariables
    ''' where all model variables have 1-dimension
    ''' </summary>
    ''' <param name="targetOutputTable">MBOutputTable to which ModelVariable contents are written</param>
    ''' <remarks>
    ''' This ModelVariable creates a new row for the MBOutputTable
    ''' and then appends the values to this row for all of the MBOutputTable's ModelVariables
    ''' </remarks>
    Public Overrides Sub Write(ByVal targetOutputTable As MBOutputTable(Of T))
        Dim newRow As DataRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
        Dim valCol As String
        Dim xIndex, xIndexMin, xIndexMax As Integer
        Dim xCol As String
        Dim mv As ModelVariable1(Of T)
        xCol = targetOutputTable.ModelVariables(0).Dimensions(0).OutputFieldName  'get the x dimension field name
        xIndexMin = Me.Dimensions(0).MinIndex
        xIndexMax = Me.Dimensions(0).MaxIndex
        For xIndex = xIndexMin To xIndexMax
            'loop over all x index values in 1-d array
            newRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
            newRow(xCol) = xIndex    'add the index value for the x dimension column
            For Each mv In targetOutputTable.ModelVariables()
                'loop over all ModelVariables in the MBOutputTable
                valCol = mv.OutputFieldName  'get the fieldname for the ModelVariable's column
                If targetOutputTable.DataSource.Columns.Contains(valCol) Then
                    newRow(valCol) = mv(xIndex)  'add a column of data for the value of this ModelVariable
                End If
            Next
            targetOutputTable.DataSource.Rows.Add(newRow)
        Next
    End Sub

    ''' <summary>
    ''' Writes contents of all the MBOutputTable's ModelVariables using the specified min and max for the last index
    ''' This function exists to support printing timestep ranges
    ''' </summary>
    ''' <param name="targetOutputTable"></param>
    ''' <param name="minLastIndex"></param>
    ''' <param name="maxLastIndex"></param>
    ''' <remarks></remarks>
    Public Overrides Sub Write(ByVal targetOutputTable As MBOutputTable(Of T), ByVal minLastIndex As Integer, ByVal maxLastIndex As Integer)
        Dim newRow As DataRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
        Dim valCol As String
        Dim xIndex, xIndexMin, xIndexMax As Integer
        Dim xCol As String
        Dim mv As ModelVariable1(Of T)
        xCol = targetOutputTable.ModelVariables(0).Dimensions(0).OutputFieldName  'get the x dimension field name
        xIndexMin = minLastIndex
        xIndexMax = maxLastIndex
        For xIndex = xIndexMin To xIndexMax
            'loop over all x index values in 1-d array
            newRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
            newRow(xCol) = xIndex    'add the index value for the x dimension column
            For Each mv In targetOutputTable.ModelVariables()
                'loop over all ModelVariables in the MBOutputTable
                valCol = mv.OutputFieldName  'get the fieldname for the ModelVariable's column
                If targetOutputTable.DataSource.Columns.Contains(valCol) Then
                    newRow(valCol) = mv(xIndex)  'add a column of data for the value of this ModelVariable
                End If
            Next
            targetOutputTable.DataSource.Rows.Add(newRow)
        Next
    End Sub

End Class


