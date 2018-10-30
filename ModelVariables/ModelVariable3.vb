'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' Model Variable with 3 dimensions
''' </summary>
''' <typeparam name="T">Generic type T</typeparam>
''' <remarks></remarks>
Class ModelVariable3(Of T)
    Inherits ModelVariable(Of T)
    ''' <summary>
    ''' Instantiates as a three-dimensional array with max indexes of 0
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        Me.CreateDimensions(0, 0, 0)
    End Sub
    ''' <summary>
    ''' Instantiates as a three-dimensional array with max indexes of 0 and a name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
        Me.CreateDimensions(0, 0, 0)
    End Sub
    ''' <summary>
    ''' Completes instantiation as a three-dimensional array, with maximum dimensions specified as Integers
    ''' </summary>
    ''' <param name="XMaxIndex">Maximum value of the x index</param>
    ''' <param name="YMaxIndex">Maximum value of the y index</param>
    ''' <param name="ZMaxIndex">Maximum value of the z index</param>
    ''' <remarks></remarks>
    Public Overridable Overloads Sub CreateDimensions(ByVal XMaxIndex As Integer, ByVal YMaxIndex As Integer, ByVal ZMaxIndex As Integer)
        Dim maxIndexes() As Integer = {XMaxIndex, YMaxIndex, ZMaxIndex}
        Me.CreateDimensions(maxIndexes)
    End Sub
    ''' <summary>
    ''' Value indexed according to the 3-dimension index triplet
    ''' </summary>
    ''' <param name="xIndex">Index value for x-dimension</param>
    ''' <param name="yIndex">Index value for y-dimension</param>
    ''' <param name="zIndex">Index value for z-dimension</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overridable Overloads Property Item(ByVal xIndex As Integer, _
        ByVal yIndex As Integer, ByVal zIndex As Integer) As T
        Get
            Return Me.Values(Me.ArrayIndex(xIndex, yIndex, zIndex))
        End Get
        Set(ByVal value As T)
            Me.Values(Me.ArrayIndex(xIndex, yIndex, zIndex)) = value
        End Set
    End Property
    ''' <summary>
    ''' Fills all elements of a three-dimensional model variable from the filtered source
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Fill()
        Dim row As DataRow
        Dim xIndex, yIndex, zIndex As Integer
        Dim xCol As String = m_Dimensions(0).SourceFieldName
        Dim yCol As String = m_Dimensions(1).SourceFieldName
        Dim zCol As String = m_Dimensions(2).SourceFieldName
        Dim valCol As String = Me.SourceFieldName

        For Each row In Me.SourceDataTableFiltered.Rows
            xIndex = CInt(row(xCol))
            yIndex = CInt(row(yCol))
            zIndex = CInt(row(zCol))
            Me.FillItem(xIndex, yIndex, zIndex) = CType(row(valCol), T)
        Next
    End Sub
    ''' <summary>
    ''' Value indexed according to the 3-dimension index pair filled from data source; IsFromSource value set to true
    ''' </summary>
    ''' <param name="xIndex">Index value for x-dimension</param>
    ''' <param name="yIndex">Index value for y-dimension</param>
    ''' <param name="zIndex">Index value for y-dimension</param>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Overridable Overloads WriteOnly Property FillItem(ByVal xIndex As Integer, ByVal yIndex As Integer, ByVal zIndex As Integer) As T
        Set(ByVal value As T)

            Dim arrIndex As Integer = Me.ArrayIndex(xIndex, yIndex, zIndex)

            If (arrIndex < Me.Values.Length) Then

                m_Values(arrIndex) = value
                m_IsFromSource(arrIndex) = True
                m_HasValue(arrIndex) = True

            End If

        End Set
    End Property
    ''' <summary>
    ''' Indicates whether or not each individual value was filled from a source
    ''' </summary>
    ''' <param name="xIndex">Index value for x-dimension</param>
    ''' <param name="yIndex">Index value for y-dimension</param>
    ''' <param name="zIndex">Index value for z-dimension</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Overloads ReadOnly Property FromSource(ByVal xIndex As Integer, _
        ByVal yIndex As Integer, ByVal zIndex As Integer) As Boolean
        Get
            Return m_IsFromSource(Me.ArrayIndex(xIndex, yIndex, zIndex))
        End Get
    End Property

    ''' <summary>
    ''' Indicates whether a value has already been set at the specified location
    ''' </summary>
    ''' <param name="xIndex">Index value for x-dimension</param>
    ''' <param name="yIndex">Index value for y-dimension</param>
    ''' <param name="zIndex">Index value for z-dimension</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Overloads ReadOnly Property HasValue(ByVal xIndex As Integer, _
        ByVal yIndex As Integer, ByVal zIndex As Integer) As Boolean
        Get
            Return m_HasValue(Me.ArrayIndex(xIndex, yIndex, zIndex))
        End Get
    End Property

    ''' <summary>
    ''' Converts the 3-dimension index triplet into a based value array index
    ''' </summary>
    ''' <param name="xIndex">Index value for x-dimension</param>
    ''' <param name="yIndex">Index value for y-dimension</param>
    ''' <param name="zIndex">Index value for z-dimension</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function ArrayIndex(ByVal xIndex As Integer, ByVal yIndex As Integer, ByVal zIndex As Integer) As Integer

        Dim xLength, yLength, zlength, xIndexBase, yIndexBase, zIndexBase As Integer

        xIndexBase = xIndex - m_Dimensions(0).MinIndex
        yIndexBase = yIndex - m_Dimensions(1).MinIndex
        zIndexBase = zIndex - m_Dimensions(2).MinIndex

        xLength = m_UpperBound(0) + 1
        yLength = m_UpperBound(1) + 1
        zlength = m_UpperBound(2) + 1

        ArrayIndex = (xIndexBase * yLength * zlength) + (yIndexBase * zlength) + zIndexBase
        Debug.Assert(ArrayIndex >= 0)

    End Function

    'Protected Function ArrayIndex(ByVal xIndex As Integer, ByVal yIndex As Integer, ByVal zIndex As Integer) As Integer

    '    Dim xLength, yLength, xIndexBase, yIndexBase, zIndexBase As Integer

    '    xIndexBase = xIndex - m_Dimensions(0).MinIndex
    '    yIndexBase = yIndex - m_Dimensions(1).MinIndex
    '    zIndexBase = zIndex - m_Dimensions(2).MinIndex
    '    xLength = m_UpperBound(0) + 1
    '    yLength = m_UpperBound(1) + 1
    '    ArrayIndex = (zIndexBase * (yLength * xLength)) + (yIndexBase * xLength) + xIndexBase

    'End Function

    ''' <summary>
    ''' Writes contents of all the MBOutputTable's ModelVariables
    ''' where all model variables have 3-dimensions
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
        Dim yIndex, yIndexMin, yIndexMax As Integer
        Dim zIndex, zIndexMin, zIndexMax As Integer
        Dim xCol, yCol, zCol As String
        Dim mv As ModelVariable3(Of T)
        xCol = targetOutputTable.ModelVariables(0).Dimensions(0).OutputFieldName  'get the x dimension field name
        yCol = targetOutputTable.ModelVariables(0).Dimensions(1).OutputFieldName  'get the y dimension field name
        zCol = targetOutputTable.ModelVariables(0).Dimensions(2).OutputFieldName  'get the z dimension field name
        xIndexMin = Me.Dimensions(0).MinIndex
        xIndexMax = Me.Dimensions(0).MaxIndex
        yIndexMin = Me.Dimensions(1).MinIndex
        yIndexMax = Me.Dimensions(1).MaxIndex
        zIndexMin = Me.Dimensions(2).MinIndex
        zIndexMax = Me.Dimensions(2).MaxIndex
        For xIndex = xIndexMin To xIndexMax
            For yIndex = yIndexMin To yIndexMax
                For zIndex = zIndexMin To zIndexMax
                    'loop over all x, y and z index values in 3-d array
                    newRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
                    newRow(xCol) = xIndex    'add the index value for the x dimension column
                    newRow(yCol) = yIndex    'add the index value for the y dimension column
                    newRow(zCol) = zIndex    'add the index value for the z dimension column
                    For Each mv In targetOutputTable.ModelVariables()
                        'loop over all ModelVariables in the MBOutputTable
                        valCol = mv.OutputFieldName  'get the fieldname for the ModelVariable's column
                        If targetOutputTable.DataSource.Columns.Contains(valCol) Then
                            newRow(valCol) = mv(xIndex, yIndex, zIndex)  'add a column of data for the value of this ModelVariable
                        End If
                    Next
                    targetOutputTable.DataSource.Rows.Add(newRow)
                Next
            Next
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
        Dim yIndex, yIndexMin, yIndexMax As Integer
        Dim zIndex, zIndexMin, zIndexMax As Integer
        Dim xCol, yCol, zCol As String
        Dim mv As ModelVariable3(Of T)
        xCol = targetOutputTable.ModelVariables(0).Dimensions(0).OutputFieldName  'get the x dimension field name
        yCol = targetOutputTable.ModelVariables(0).Dimensions(1).OutputFieldName  'get the y dimension field name
        zCol = targetOutputTable.ModelVariables(0).Dimensions(2).OutputFieldName  'get the z dimension field name
        xIndexMin = Me.Dimensions(0).MinIndex
        xIndexMax = Me.Dimensions(0).MaxIndex
        yIndexMin = Me.Dimensions(1).MinIndex
        yIndexMax = Me.Dimensions(1).MaxIndex
        zIndexMin = minLastIndex
        zIndexMax = maxLastIndex
        For xIndex = xIndexMin To xIndexMax
            For yIndex = yIndexMin To yIndexMax
                For zIndex = zIndexMin To zIndexMax
                    'loop over all x, y and z index values in 3-d array
                    newRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
                    newRow(xCol) = xIndex    'add the index value for the x dimension column
                    newRow(yCol) = yIndex    'add the index value for the y dimension column
                    newRow(zCol) = zIndex    'add the index value for the z dimension column
                    For Each mv In targetOutputTable.ModelVariables()
                        'loop over all ModelVariables in the MBOutputTable
                        valCol = mv.OutputFieldName  'get the fieldname for the ModelVariable's column
                        If targetOutputTable.DataSource.Columns.Contains(valCol) Then
                            newRow(valCol) = mv(xIndex, yIndex, zIndex)  'add a column of data for the value of this ModelVariable
                        End If
                    Next
                    targetOutputTable.DataSource.Rows.Add(newRow)
                Next
            Next
        Next
    End Sub

End Class
