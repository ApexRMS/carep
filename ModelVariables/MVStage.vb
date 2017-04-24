'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

''' <summary>
''' ModelVariable1 of type Double with dimension of StageID
''' </summary>
''' <remarks></remarks>
Class MVStage
    Inherits ModelVariable1(Of Double)

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
    ''' Value for the specified allocation stage
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal stageID As Integer) As Double
        Get
            Return MyBase.Item(stageID)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(stageID) = value
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
    ''' Completes instantiation as a one-dimensional array, with maximum stage ID specified as an Integer
    ''' </summary>
    ''' <param name="maxStageID">Maximum value of the stage ID</param>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxStageID As Integer)
        Dim maxIndexes() As Integer = {maxStageID}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = DATASHEET_STAGE_ID_COLUMN_NAME   ' set the default field name to a constant
        Me.Dimensions(0).DisplayName = StageDisplayName
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
        sourceDataView.Table = Me.SourceDataTable
        sourceDataView.RowFilter = String.Format("(Iteration={0} OR Iteration IS NULL)", sourceFilterValues(0))

        If sourceDataView.Count > 0 Then

            Me.EraseValues()

            m_SourceDataTableFiltered = sourceDataView.ToTable
            Dim valCol As String = Me.SourceFieldName

            For Each dr As DataRow In Me.SourceDataTableFiltered.Rows

                Dim stageindex As Integer = CInt(dr(DATASHEET_STAGE_ID_COLUMN_NAME))
                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)

                If (IterationNull) Then

                    If (Me.HasValue(stageindex)) Then
                        Continue For
                    End If

                End If

                Me.FillItem(stageindex) = CDbl(dr(valCol))

            Next

        End If

    End Sub

End Class

