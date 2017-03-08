'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Class MVSingleValueOfDouble
    Inherits ModelVariable0(Of Double)

    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0 and name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
        Me.CreateDimensions(0)
    End Sub

    Public Overrides Sub Load(sourceFilterValues() As Integer)

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

                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)

                If (IterationNull) Then

                    If (Me.HasValue(0)) Then
                        Continue For
                    End If

                End If

                Me.FillItem(0) = CDbl(dr(valCol))

            Next

        End If

    End Sub

End Class
