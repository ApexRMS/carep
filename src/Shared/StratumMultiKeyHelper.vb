﻿'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

Class StratumMultiKeyHelper

    Private m_Project As Project
    Private m_MultiToSingle As New Dictionary(Of String, Integer)
    Private m_SingleToMulti As New Dictionary(Of Integer, String)

    Public ReadOnly Property Project As Project
        Get
            Return Me.m_Project
        End Get
    End Property

    Public Sub New(ByVal stratumDataSheet As DataSheet)

        Debug.Assert(stratumDataSheet.Name = "CAREP_Stratum")
        Me.m_Project = stratumDataSheet.Project

        For Each dr As DataRow In stratumDataSheet.GetData.Rows

            Dim sid As Integer = dr(stratumDataSheet.PrimaryKeyColumn.Name)

            Dim key As String = String.Format(
                "{0}-{1}-{2}",
                dr("StratumLabelXID"),
                dr("StratumLabelYID"),
                dr("StratumLabelZID"))

            Me.m_MultiToSingle.Add(key, sid)
            Me.m_SingleToMulti.Add(sid, key)

        Next

    End Sub

    Public Function CanGetStratumID(ByVal x As Integer, ByVal y As Integer, ByVal z As Integer) As Boolean
        Return Me.m_MultiToSingle.ContainsKey(String.Format("{0}-{1}-{2}", x, y, z))
    End Function

    Public Function GetStratumID(ByVal x As Integer, ByVal y As Integer, ByVal z As Integer) As Integer
        Return Me.m_MultiToSingle(String.Format("{0}-{1}-{2}", x, y, z))
    End Function

    Public Sub GetMultiKeys(ByVal stratumId As Integer, ByRef x As Integer, ByRef y As Integer, ByRef z As Integer)

        Dim s() As String = Me.m_SingleToMulti(stratumId).Split("-")

        x = CInt(s(0))
        y = CInt(s(1))
        z = CInt(s(2))

    End Sub

End Class
