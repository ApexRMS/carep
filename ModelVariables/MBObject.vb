'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Public MustInherit Class MBObject
    Implements IDisposable

#Region "Class Member Variables"

    Private m_ID As Integer = -1
    Private m_Name As String = Nothing
    Private m_IsDisposed As Boolean = False

#End Region

#Region "Constructors"

    Public Sub New(ByVal Name As String)
        Me.New(0, Name)
    End Sub

    Public Sub New(ByVal ID As Integer, ByVal Name As String)
        m_ID = ID
        m_Name = Name
    End Sub

#End Region

#Region "Properties"

    Public Property ID() As Integer
        Get
            Return m_ID
        End Get
        Set(ByVal value As Integer)
            m_ID = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property

    Public ReadOnly Property IsDisposed() As Boolean
        Get
            Return Me.m_IsDisposed
        End Get
    End Property

#End Region

#Region "Disposable Support"

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        Return
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class






