'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

''' <summary>
''' ModelVariable with a single element
''' </summary>
''' <typeparam name="T">Generic type T</typeparam>
''' <remarks></remarks>
Class ModelVariable0(Of T)
    Inherits ModelVariable1(Of T)
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
    ''' Returns single ModelVariable value at index position 0
    ''' </summary>
    ''' <param name="zero">Index value - always set to 0</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overridable Overloads Property Item(ByVal zero As Integer) As T
        Get
            Return Me.Values(0)
        End Get
        Set(ByVal value As T)
            Me.Values(0) = value
        End Set
    End Property
    ''' <summary>
    ''' Fills the single element for the model variable from the filtered source
    ''' </summary>
    ''' <remarks>Assumes there is only one row in the source datatable</remarks>
    Public Overrides Sub Fill()
        Dim row As DataRow
        For Each row In m_SourceDataTableFiltered.Rows
            Me.FillItem(0) = row(Me.SourceFieldName)
        Next
    End Sub

    ''' <summary>
    ''' Establishes the data source for a single element using the field name for a single record
    ''' </summary>
    ''' <param name="sourceDataTable">Datatable containing the source data</param>
    ''' <param name="sourceFieldName">Field name for the single element's value</param>
    ''' <remarks>
    ''' SourceDataTable is the datatable containing the source data;
    ''' SourceFieldName is the field name identifying the column in the sourceDataTable's single record corresponding to
    ''' the single element
    ''' </remarks>
    Public Overridable Overloads Sub Source(ByVal sourceDataTable As DataTable, ByVal sourceFieldName As String)
        Dim sourceFieldNames() As String = {sourceFieldName}  'create an array with only one element for the value field
        Me.DataSource(sourceDataTable, sourceFieldNames)
    End Sub
    ''' <summary>
    ''' Writes contents of all the MBOutputTable's ModelVariables
    ''' where all model variables have a single element at index 0
    ''' </summary>
    ''' <param name="targetOutputTable">MBOutputTable to which ModelVariable contents are written</param>
    ''' <remarks>
    ''' This ModelVariable creates a new row for the MBOutputTable
    ''' and then appends the values to this row for all of the MBOutputTable's ModelVariables
    ''' </remarks>
    Public Overrides Sub Write(ByVal targetOutputTable As MBOutputTable(Of T))
        Dim newRow As DataRow = targetOutputTable.NewDataRow  'new empty row for the MBOutputTable's datatable
        Dim valCol As String
        Dim mv As ModelVariable(Of T)
        For Each mv In targetOutputTable.ModelVariables()
            'loop over all ModelVariables in the MBOutputTable
            valCol = mv.OutputFieldName  'get the fieldname for the target output
            If targetOutputTable.DataSource.Columns.Contains(valCol) Then
                newRow(valCol) = mv.Values(0)
            End If
        Next
        targetOutputTable.DataSource.Rows.Add(newRow)
    End Sub

End Class

