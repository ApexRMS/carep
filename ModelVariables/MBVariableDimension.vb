'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports SyncroSim.Core

''' <summary>
''' Characterizes a single dimension of a ModelVariable
''' </summary>
''' <remarks></remarks>
Class MBVariableDimension
    Private m_UpperBound As Integer 'max zero-based index value
    Private m_Name As String 'Name of the dimension
    Private m_DisplayName As String 'the name displayed to users for the dimension
    Private m_SourceFieldName As String 'the field name associated with the dimension in the source datatable
    Private m_TargetFieldName As String  ' the field name associated with the dimension in the target datatable
    Private m_MinIndex As Integer 'the min value of the non-zero based index
    Private m_MaxIndex As Integer 'the max value of the non-zero based index

    ''' <summary>
    ''' Instantiates based on a maximum value for the dimension's index
    ''' </summary>
    ''' <remarks></remarks>
    Sub New(ByVal maxIndex As Integer)
        Me.New(0, maxIndex, "")
    End Sub
    ''' <summary>
    ''' Instantiates based on a minimum index value, maximum index value and name
    ''' </summary>
    ''' <remarks></remarks>
    Sub New(ByVal minIndex As Integer, ByVal maxIndex As Integer, ByVal name As String)

        m_MinIndex = minIndex
        m_MaxIndex = maxIndex
        m_UpperBound = m_MaxIndex - m_MinIndex
        m_Name = name

    End Sub

    ''' <summary>
    ''' Instantiates based on a data sheet
    ''' </summary>
    ''' <remarks></remarks>
    Sub New(ByVal ds As DataSheet)
        Me.New(ds.GetData(), ds.Name, ds.DisplayName)
    End Sub

    ''' <summary>
    ''' Instantiates based on a table
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="name"></param>
    ''' <param name="displayName"></param>
    ''' <remarks></remarks>
    Sub New(ByVal dt As DataTable, ByVal name As String, ByVal displayName As String)

        m_MinIndex = 1
        m_MaxIndex = dt.Rows.Count
        m_UpperBound = m_MaxIndex - m_MinIndex
        m_Name = name
        m_DisplayName = displayName

    End Sub

    ''' <summary>
    ''' Maximum possible index value for the dimension
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property UpperBound() As Integer
        Get
            Return m_UpperBound
        End Get
    End Property
    ''' <summary>
    ''' 'String name of the dimension
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property
    ''' <summary>
    ''' 'String name displayed to users for the dimension
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>If not set explicitly, by default returns the model variable name</remarks>
    Public Overridable Property DisplayName() As String
        Get
            If m_DisplayName = Nothing Then
                Return m_Name
            Else
                Return m_DisplayName
            End If
        End Get
        Set(ByVal value As String)
            m_DisplayName = value
        End Set
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
                Return m_Name
            Else
                Return m_SourceFieldName
            End If
        End Get
        Set(ByVal value As String)
            m_SourceFieldName = value
        End Set
    End Property
    ''' <summary>
    '''     Minimum value for the index
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MinIndex() As Integer
        Get
            Return m_MinIndex
        End Get
        Set(ByVal value As Integer)
            m_MinIndex = value
        End Set
    End Property
    ''' <summary>
    '''     Maximum value for the index
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MaxIndex() As Integer
        Get
            Return m_MaxIndex
        End Get
        Set(ByVal value As Integer)
            m_MaxIndex = value
        End Set
    End Property
    ''' <summary>
    ''' 'Name of the value field for the target datatable
    ''' </summary>
    ''' <value>String field name that will be used to store the value in the target datatable</value>
    ''' <returns></returns>
    ''' <remarks>If not set explicitly, by default returns the model variable name</remarks>
    Public Overridable Property OutputFieldName() As String
        Get
            If m_TargetFieldName = Nothing Then
                Return m_Name
            Else
                Return m_TargetFieldName
            End If
        End Get
        Set(ByVal value As String)
            m_TargetFieldName = value
        End Set
    End Property

End Class

