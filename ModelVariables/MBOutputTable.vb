'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

''' <summary>
''' Represents a table of output for one or more ModelVariables of type T,
''' each with the same dimensions
''' </summary>
''' <remarks>All ModelVariables must have the same dimensions</remarks>
Class MBOutputTable(Of T)

    Private m_TableName As String
    Private m_DataSource As DataTable
    Private m_OtherFieldNames() As String
    Private m_OtherFieldValues() As Integer
    Private m_ModelVariables() As ModelVariable(Of T)
    Private m_bOutputRequired As Boolean = True
    Private m_sDisplayName As String

    Public ReadOnly Property DataSource As DataTable
        Get
            Return Me.m_DataSource
        End Get
    End Property

    ' ''' <summary>
    ' ''' Initializes this class
    ' ''' </summary>
    ' ''' <param name="sTableName">The name of the output table in the database</param>
    ' ''' <remarks></remarks>
    'Public Sub New(ByVal sTableName As String, ByVal sDisplayName As String)
    '    Me.m_TableName = m_TableName
    '    Me.m_sDisplayName = sDisplayName
    'End Sub

    'Public Sub New(ByVal sTableName As String, ByVal sDisplayName As String, ByVal sScenarioFieldName As String)
    '    Me.m_TableName = m_TableName
    '    Me.m_ScenarioFieldName = sScenarioFieldName
    '    Me.m_sDisplayName = sDisplayName
    'End Sub

    Public Property OutputRequired() As Boolean
        Get
            Return Me.m_bOutputRequired
        End Get
        Set(ByVal value As Boolean)
            Me.m_bOutputRequired = value
        End Set
    End Property

    Public Property DisplayName() As String
        Get
            Return Me.m_sDisplayName
        End Get
        Set(ByVal value As String)
            Me.m_sDisplayName = value
        End Set
    End Property

    ''' <summary>
    ''' Instantiates based on an array of ModelVariables
    ''' </summary>
    ''' <param name="modelVariables">model variable to save</param>
    ''' <remarks></remarks>
    Sub Initialize(ByVal modelVariables() As ModelVariable(Of T), ByVal dataSource As DataTable)

        Debug.Assert(Me.m_DataSource Is Nothing)
        Debug.Assert(dataSource.Rows.Count = 0)

        Me.m_DataSource = dataSource

        Dim dimension As MBVariableDimension
        Dim mv As ModelVariable(Of T)
        Dim mvIndex As Integer
        Dim mvValidIndex As Integer = 0
        Dim countValidMV As Integer = 0   ' count of valid ModelVariables
        Dim allFieldsInDataTable(modelVariables.Length - 1) As Boolean   'one array element for each ModelVariable

        For mvIndex = 0 To modelVariables.Length - 1
            mv = modelVariables(mvIndex)
            allFieldsInDataTable(mvIndex) = True
            'Check all field names for the ModelVariable's dimensions can be found in the target DataTable
            For Each dimension In mv.Dimensions
                If allFieldsInDataTable(mvIndex) And (dimension.OutputFieldName <> Nothing) Then
                    'if flag still true and target field exists, see if next field sets it to false
                    allFieldsInDataTable(mvIndex) = DataSource.Columns.Contains(dimension.OutputFieldName)
                End If
            Next
            'Check the field name for the ModelVariable's value can be found in the target DataTable
            If allFieldsInDataTable(mvIndex) = True And (mv.OutputFieldName <> Nothing) Then
                allFieldsInDataTable(mvIndex) = DataSource.Columns.Contains(mv.OutputFieldName)
            End If
            If allFieldsInDataTable(mvIndex) = True Then countValidMV += 1
        Next

        'Now add all the valid ModelVariables to the MBOutputTable
        If countValidMV > 0 Then ReDim m_ModelVariables(countValidMV - 1)
        For mvIndex = 0 To modelVariables.Length - 1
            If allFieldsInDataTable(mvIndex) = True Then
                m_ModelVariables(mvValidIndex) = modelVariables(mvIndex)
                mvValidIndex += 1
            End If
        Next

    End Sub
    ''' <summary>
    ''' Instantiates based on a single ModelVariable with additional fields
    ''' </summary>
    ''' <param name="modelVariables">model variable to save</param>
    ''' <param name="otherFieldNames">names of additional fields</param>
    ''' <remarks></remarks>
    Sub Initialize( _
        ByVal modelVariables() As ModelVariable(Of T), _
        ByVal otherFieldNames() As String,
        ByVal dataSource As DataTable)

        Me.Initialize(modelVariables, dataSource)
        m_OtherFieldNames = otherFieldNames

    End Sub
    ''' <summary>
    ''' Array of ModelVariables in the MBOutputTable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ModelVariables() As ModelVariable(Of T)()
        Get
            Return m_ModelVariables
        End Get
    End Property
    ''' <summary>
    ''' Array of integer values for additional fields added to MBOutputTable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OtherFieldValues() As Integer()
        Get
            Return m_OtherFieldValues
        End Get
        Set(ByVal value As Integer())
            m_OtherFieldValues = value
        End Set
    End Property
    ''' <summary>
    ''' Array of string names of additional fields added to MBOutputTable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OtherFieldNames() As String()
        Get
            Return m_OtherFieldNames
        End Get
        Set(ByVal value As String())
            m_OtherFieldNames = value
        End Set
    End Property
    ''' <summary>
    ''' Returns a new DataRow for the target datatable with additional field values filled in
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function NewDataRow() As DataRow
        Dim i As Integer
        Dim newRow As DataRow = DataSource.NewRow
        Dim maxOtherFields As Integer
        maxOtherFields = m_OtherFieldNames.Length
        If OtherFieldValues.Length < maxOtherFields Then maxOtherFields = OtherFieldValues.Length
        For i = 0 To (maxOtherFields - 1)
            newRow(m_OtherFieldNames(i)) = OtherFieldValues(i)
        Next
        NewDataRow = newRow
    End Function
    ''' <summary>
    ''' Writes the values for all ModelVariables to the DataTable
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Sub Write()
        'Use the first model variable to define the rows to be added to the datatable
        'Pass the MBOutputTable itself to its first ModelVariable, so that this ModelVariable
        'can figure out the number of rows to create in the database and then append the
        'values to each row for the remaining ModelVariables
        m_ModelVariables(0).Write(Me)
    End Sub
    ''' <summary>
    ''' Writes the values for all ModelVariables to the DataTable
    ''' </summary>
    ''' <param name="otherFieldValues">Values for additional fields</param>
    ''' <remarks>
    ''' </remarks>
    Public Sub Write(ByVal otherFieldValues() As Integer)
        m_OtherFieldValues = otherFieldValues
        m_ModelVariables(0).Write(Me)
    End Sub

    ''' <summary>
    ''' Writes the values for all ModelVariables to the DataTable using the specified values for the min and max of the last index
    ''' This function exists to support writing specific timestep ranges.
    ''' </summary>
    ''' <param name="otherFieldValues"></param>
    ''' <param name="minLastIndex"></param>
    ''' <param name="maxLastIndex"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal otherFieldValues() As Integer, ByVal minLastIndex As Integer, ByVal maxLastIndex As Integer)
        m_OtherFieldValues = otherFieldValues
        m_ModelVariables(0).Write(Me, minLastIndex, maxLastIndex)
    End Sub

End Class



