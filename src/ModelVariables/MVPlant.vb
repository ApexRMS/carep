'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable1 of type Double with dimension of plantID
''' </summary>
''' <remarks></remarks>
Class MVPlant
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
    ''' Value for the specified plant type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal plantID As Integer) As Double
        Get
            Return MyBase.Item(plantID)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(plantID) = value
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
    ''' Completes instantiation as a one-dimensional array, with maximum plant ID specified as an Integer
    ''' </summary>
    ''' <param name="maxPlantID">Maximum value of the plant ID</param>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxPlantID As Integer)
        Dim maxIndexes() As Integer = {maxPlantID}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = DATASHEET_PLANT_ID_COLUMN_NAME   ' set the default field name to a constant
        Me.Dimensions(0).DisplayName = PlantDisplayName
    End Sub

    ''' <summary>
    ''' Loads, filters and interpolates data from the source datatable - specify only the filter values
    ''' </summary>
    ''' <param name="sourceFilterValues">Array of integer values to filter for</param>
    ''' <remarks>Overrides method in base class to convert Year and Jday in source data to timestep</remarks>
    Public Overrides Sub Load(ByVal sourceFilterValues As Integer())

        Dim sourceDataView As New DataView
        sourceDataView.Table = Me.SourceDataTable
        sourceDataView.RowFilter = String.Format("(Iteration={0} OR Iteration IS NULL)", sourceFilterValues(0))

        If sourceDataView.Count > 0 Then

            Me.EraseValues()

            m_SourceDataTableFiltered = sourceDataView.ToTable
            Dim valCol As String = Me.SourceFieldName

            For Each dr As DataRow In Me.SourceDataTableFiltered.Rows

                Dim IterationNull As Boolean = (dr(DATASHEET_ITERATION_COLUMN_NAME) Is DBNull.Value)
                Dim plantindex As Integer = PlantIDTranslator.IndexFromPKID(CInt(dr(DATASHEET_PLANT_ID_COLUMN_NAME)))

                If (IterationNull) Then

                    If (Me.HasValue(plantindex)) Then
                        Continue For
                    End If

                End If

                Me.FillItem(plantindex) = CDbl(dr(valCol))

            Next

        End If

    End Sub

End Class

