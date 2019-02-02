'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable2 of type Double with dimension of plantID and daily hour index
''' </summary>
''' <remarks></remarks>
Class MVPlantHour
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
    ''' Value for the specified plantID and hour
    ''' </summary>
    ''' <param name="plantID">Index value for plant ID</param>
    ''' <param name="hour">Index value for hour</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal plantID As Integer, ByVal hour As Integer) As Double
        Get
            Return MyBase.Item(plantID, hour)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(plantID, hour) = value
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
    ''' Completes instantiation as a two-dimensional array, with maximum hour and plantID specified as Integers
    ''' </summary>
    ''' <param name="maxHour">Maximum value of the hour index</param>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxPlantID As Integer, ByVal maxHour As Integer)
        Dim maxIndexes() As Integer = {maxPlantID, maxHour}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = DATASHEET_PLANT_ID_COLUMN_NAME   ' set the default field name to a constant
        Me.Dimensions(0).DisplayName = PlantDisplayName
        Me.Dimensions(1).Name = HourFieldName   ' set the default field name to a constant
    End Sub
End Class
