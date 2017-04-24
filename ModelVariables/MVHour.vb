'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

''' <summary>
''' ModelVariable1 of type Double with dimension of hour
''' </summary>
''' <remarks></remarks>
Class MVHour
    Inherits ModelVariable1(Of Double)

    Protected Shared m_DefaultDimensions() As MBVariableDimension   'shared field indicating the default VariableDimensions for a particular ModelVariable type

    ''' <summary>
    ''' Instantiates as a one-dimensional array with max index of 0 and name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
        'm_Name = name
        'If Not MVHour.DefaultDimensions.Equals(Nothing) Then
        '    Me.CreateDimensions(MVHour.DefaultDimensions)
        'End If
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
    ''' Value for the specified hour
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overloads Property Item(ByVal hour As Integer) As Double
        Get
            Return MyBase.Item(hour)
        End Get
        Set(ByVal value As Double)
            ValidateAssignment(value)
            MyBase.Item(hour) = value
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
    ''' Completes instantiation as a one-dimensional array, with maximum hour index specified as an Integer
    ''' </summary>
    ''' <param name="maxHour">Maximum value for the hour index</param>
    ''' <remarks></remarks>
    Public Overrides Sub CreateDimensions(ByVal maxHour As Integer)
        Dim maxIndexes() As Integer = {maxHour}
        MyBase.CreateDimensions(maxIndexes)
        Me.Dimensions(0).Name = HourFieldName   ' set the default field name to a constant
    End Sub
End Class
