'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' Abstract class representing a variable used in a model algorithm
''' </summary>
''' <typeparam name="T">Generic type T</typeparam>
''' <remarks>
''' This base class exposes data as a one dimensional array of values of type T
''' </remarks>
MustInherit Class ModelVariable(Of T)
    Inherits MBObject
    'Protected Shared m_DefaultDimensions() As MBVariableDimension   'shared field indicating the default VariableDimensions for a particular ModelVariable type

    Protected m_Values() As T 'one-dimensional internal array storing all values
    Protected m_Dimensions() As MBVariableDimension ' array specifiying the properties of each dimension
    Protected m_LengthBaseArray As Integer 'count of the total number of values stored
    Protected m_Rank As Integer   'number of dimensions to the variable
    Protected m_Length() As Integer 'Length for each dimension
    Protected m_UpperBound() As Integer 'Upper bound for each dimension
    Protected m_DisplayName As String 'the name to be displayed

    Protected m_HasSource As Boolean 'indicates whether or not variable has a source
    Protected m_IsFromSource() As Boolean 'one-dimensional array indicating whether or not each value element was filled from a source
    Protected m_HasValue() As Boolean
    Protected m_SourceDataTable As DataTable 'the datatable containing the source data for initialization
    Protected m_SourceDataTableFiltered As DataTable 'filtered version of m_SourceDataTable, based on filter set in m_Dimensions
    Protected m_SourceFieldName As String 'the field name associated with the value field in the source datatable
    Protected m_SourceFilterFieldNames() As String 'the name of every field that is eligible to be filtered on in the source datatable

    Protected m_OutputFieldName As String  ' the field name associated with the value field in the MBOutputTable

    Protected Shared m_PlantIDTranslator As IDTranslator
    Protected Shared m_StratumIDTranslator As IDTranslator
    Protected Shared m_StratumMultiKeyHelper As StratumMultiKeyHelper

    Public Shared Property PlantIDTranslator As IDTranslator
        Get
            Return m_PlantIDTranslator
        End Get
        Set(value As IDTranslator)
            m_PlantIDTranslator = value
        End Set
    End Property

    Public Shared Property StratumIDTranslator As IDTranslator
        Get
            Return m_StratumIDTranslator
        End Get
        Set(value As IDTranslator)
            m_StratumIDTranslator = value
        End Set
    End Property

    Public Shared Property StratumMultiKeyHelper As StratumMultiKeyHelper
        Get
            Return m_StratumMultiKeyHelper
        End Get
        Set(value As StratumMultiKeyHelper)
            m_StratumMultiKeyHelper = value
        End Set
    End Property

    ''' <summary>
    ''' Instantiates as 1-dimensional array with a single value at index=0
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        MyBase.New(Nothing)

        m_LengthBaseArray = 0
        m_Rank = 0
        m_HasSource = False
        Me.Name = ""
        ReDim m_SourceFilterFieldNames(0)
        Dim maxIndexes() As Integer = {0}
        Me.CreateDimensions(maxIndexes)
    End Sub
    ''' <summary>
    ''' Instantiates and sets the name
    ''' </summary>
    ''' <param name="name">Default field name for output</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
    End Sub

    ''' <summary>
    ''' Fills one-dimensional model variable with a single element at index 0
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Overridable Sub Fill()
        Dim row As DataRow
        Dim valCol As String = Me.SourceFieldName

        Me.EraseValues()

        For Each row In m_SourceDataTableFiltered.Rows
            Me.FillItem(0) = CType(row(valCol), T)
        Next
    End Sub

    ''' <summary>
    ''' Single value at index position in base array filled from a data source; IsFromSource value set to true
    ''' </summary>
    ''' <param name="index">Index value in base array</param>
    ''' <value></value>
    ''' <remarks></remarks>
    Protected Overridable Overloads WriteOnly Property FillItem(ByVal index As Integer) As T
        Set(ByVal value As T)

            Debug.Assert(index >= 0)

            If (index < Me.Values.Length) Then

                m_Values(index) = value
                m_IsFromSource(index) = True
                m_HasValue(index) = True

            End If

        End Set
    End Property

    ''' <summary>
    ''' Base array of the values
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property Values() As T()
        Get
            Return m_Values
        End Get
        Set(ByVal value As T())
            m_Values = value
        End Set
    End Property

    ''' <summary>
    ''' 'Name to be displayed
    ''' </summary>
    ''' <value>String to be displayed</value>
    ''' <returns></returns>
    ''' <remarks>If not set explicitly, by default returns the model variable name</remarks>
    Public Overridable Property DisplayName() As String
        Get
            If m_DisplayName = Nothing Then
                Return Me.Name
            Else
                Return m_DisplayName
            End If
        End Get
        Set(ByVal value As String)
            m_DisplayName = value
        End Set
    End Property

    ''' <summary>
    ''' Indicates whether or not each individual value was filled from a source
    ''' </summary>
    ''' <param name="index">Index value in base array</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property IsFromSource(ByVal index As Integer) As Boolean
        Get
            Return m_IsFromSource(index)
        End Get
    End Property
    ''' <summary>
    ''' Array indicating whether or not each individual value was filled from a source
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property IsFromSource() As Boolean()
        Get
            Return m_IsFromSource
        End Get
        'Set(ByVal value As Boolean())
        '    m_IsFromSource = value
        'End Set
    End Property

    Public Overridable ReadOnly Property HasValue(ByVal index As Integer) As Boolean
        Get
            Return Me.m_HasValue(index)
        End Get
    End Property
    ''' <summary>
    ''' Was the model variable filled from source data
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property HasSource() As Boolean
        Get
            Return m_HasSource
        End Get
        Set(ByVal value As Boolean)
            m_HasSource = value
        End Set
    End Property
    ''' <summary>
    ''' Unfiltered datatable containing all possible source data
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property SourceDataTable() As DataTable
        Get
            Return m_SourceDataTable
        End Get
    End Property
    ''' <summary>
    ''' Filtered datatable containing the source data for initialization
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property SourceDataTableFiltered() As DataTable
        Get
            Return m_SourceDataTableFiltered
        End Get
    End Property
    ''' <summary>
    ''' 'Name of the value field for the source datatable
    ''' </summary>
    ''' <value>String field name that provided the value field from the source datatable</value>
    ''' <returns></returns>
    ''' <remarks>If not set explicitly, by default returns the model variable name</remarks>
    Public Overridable Property SourceFieldName() As String
        Get
            If m_SourceFieldName = Nothing Then
                Return Me.Name
            Else
                Return m_SourceFieldName
            End If
        End Get
        Set(ByVal value As String)
            m_SourceFieldName = value
        End Set
    End Property
    ''' <summary>
    ''' 'Array of the name of every field that is eligible to be filtered on in the source datatable
    ''' </summary>
    ''' <value>Array of String field names that will be used for filtering the source datatable</value>
    ''' <returns>Array of String field names that will used for filtering the source datatable</returns>
    ''' <remarks></remarks>
    Public Overridable Property SourceFilterFieldNames() As String()
        Get
            Return m_SourceFilterFieldNames
        End Get
        Set(ByVal value As String())
            m_SourceFilterFieldNames = value
        End Set
    End Property
    ''' <summary>
    ''' 'Name of the value field for the output datatable
    ''' </summary>
    ''' <value>String field name that will be used to store the value in the MBOutputTable</value>
    ''' <returns></returns>
    ''' <remarks>If not set explicitly, by default returns the model variable name</remarks>
    Public Overridable Property OutputFieldName() As String
        Get
            If m_OutputFieldName = Nothing Then
                Return Me.Name
            Else
                Return m_OutputFieldName
            End If
        End Get
        Set(ByVal value As String)
            m_OutputFieldName = value
        End Set
    End Property
    ''' <summary>
    ''' Number of dimensions
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property Rank() As Integer
        Get
            Return m_Rank
        End Get
    End Property
    ''' <summary>
    ''' Array of upper bounds for each dimension
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property UpperBound() As Integer()
        Get
            Return m_UpperBound
        End Get
    End Property

    ''' <summary>
    ''' Total number of values in the base array
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property LengthBaseArray() As Integer
        Get
            Return m_LengthBaseArray
        End Get
    End Property
    ''' <summary>
    ''' Array with a MBVariableDimension for each dimension
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property Dimensions() As MBVariableDimension()
        Get
            Return m_Dimensions
        End Get
    End Property
    ''' <summary>
    ''' Writes contents of all the MBOutputTable's ModelVariables - always overriden by subclasses
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Overridable Sub Write(ByVal targetOutputTable As MBOutputTable(Of T))
    End Sub

    Public Overridable Sub Write(ByVal targetOutputTable As MBOutputTable(Of T), ByVal minIndex As Integer, ByVal maxIndex As Integer)
    End Sub

    ''' <summary>
    ''' Completes instantiation as a multi-dimensional array, with each dimension specified in the dimensions array
    ''' </summary>
    ''' <param name="dimensions">Array of MBVariableDimension, with one element per dimension</param>
    ''' <remarks></remarks>
    Public Overridable Sub CreateDimensions(ByVal dimensions() As MBVariableDimension)
        Dim vd As MBVariableDimension
        Dim elementCount As Integer = 1
        Dim dimensionIndex As Integer = 0
        Dim i As Integer
        m_Dimensions = dimensions
        m_Rank = dimensions.Length   'one dimension for each element in the dimensions array
        ReDim m_UpperBound(m_Rank - 1)
        'ReDim m_SourceDataFieldNames(m_Rank)  ' one element for each dimension, plus an extra for the value field
        'ReDim m_SourceDataFieldOffsets(m_Rank - 1)  ' one element for each dimension
        For Each vd In dimensions
            m_UpperBound(dimensionIndex) = vd.UpperBound
            'vd.SourceFieldOffset = 0     'default is no offsets for each dimension
            dimensionIndex += 1
        Next vd
        For i = 0 To m_Rank - 1
            elementCount = elementCount * (m_UpperBound(i) + 1)
        Next
        m_LengthBaseArray = elementCount
        Me.EraseValues()   ' erases all existing values in the base array
    End Sub
    ''' <summary>
    ''' Erases all of the values by redimensioning base value array
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EraseValues()
        ReDim m_Values(m_LengthBaseArray - 1)
        ReDim m_IsFromSource(m_LengthBaseArray - 1)
        ReDim m_HasValue(m_LengthBaseArray - 1)
    End Sub
    ''' <summary>
    ''' Resets dimensions as a multi-dimensional array, using array specifying maxIndex for each dimension
    ''' </summary>
    ''' <param name="maxIndexes">Array of maximum index values, one element per dimension</param>
    ''' <remarks></remarks>
    Protected Overridable Sub CreateDimensions(ByVal maxIndexes() As Integer)
        Dim dimensionIndex As Integer = 0
        Dim maxIndex As Integer
        Dim dimensions(maxIndexes.Length - 1) As MBVariableDimension
        For Each maxIndex In maxIndexes
            dimensions(dimensionIndex) = New MBVariableDimension(maxIndex)
            dimensionIndex += 1
        Next
        Me.CreateDimensions(dimensions)
    End Sub
    ''' <summary>
    ''' Establishes the data source specifying the source datatable and the array of field names for filtering
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Overridable Sub DataSource(ByVal sourceDataTable As DataTable, ByVal filterFieldNames() As String)
        m_SourceFilterFieldNames = filterFieldNames
        Me.DataSource(sourceDataTable)
    End Sub
    ''' <summary>
    ''' Establishes the data source specifying only the source datatable
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Overridable Sub DataSource(ByVal sourceDataTable As DataTable)
        m_SourceDataTable = sourceDataTable
        m_SourceDataTableFiltered = m_SourceDataTable 'assume no filtering when source first established
        m_HasSource = True   'tag that this ModelVariable has a source
    End Sub

    ''' <summary>
    ''' Loads and filters data from the source datatable - specify only the filter values
    ''' </summary>
    ''' <remarks>sourceFilterValues is an array of integer indexes identifying the value to apply as a filter for each field
    ''' in the source datatable when initializing; a negative value for the index filter indicate no filter to be applied
    ''' for that source field. Filter values are specified in order for each field in sourceFilterFieldNames - trailing
    ''' filter values can be omitted</remarks>
    Public Overridable Sub Load(ByVal sourceFilterValues As Integer())

        Dim sourceDataView As New DataView
        Me.EraseValues()
        sourceDataView.Table = m_SourceDataTable

        'Only load new values if values exist for this filter; otherwise keep the values from the previous filter
        If sourceDataView.Count > 0 Then
            'Reset the filtered source datatable
            m_SourceDataTableFiltered = sourceDataView.ToTable
            'Fill the model variable with new values
            Me.Fill()
        End If

    End Sub

    Private Function GetSourceDataFieldNames() As String()
        Dim dimension As MBVariableDimension
        Dim fieldNames(m_Rank + 1) As String
        Dim dimensionIndex As Integer = 0
        For Each dimension In m_Dimensions
            fieldNames(dimensionIndex) = dimension.SourceFieldName
            dimensionIndex += 1
        Next
        fieldNames(dimensionIndex) = m_SourceFieldName
        GetSourceDataFieldNames = fieldNames
    End Function

End Class






