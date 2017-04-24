'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports SyncroSim.Core

''' <summary>
''' Translates between Primary Key IDs and Indexes
''' </summary>
''' <remarks>
''' An assumption here is that the indexes we are dealing with start at 1 - which is
''' how a model variable dimension arranges things for data feeds...
''' </remarks>
Class IDTranslator

    Private m_IndexToPKID As New Dictionary(Of Integer, Integer)
    Private m_PKIDToIndex As New Dictionary(Of Integer, Integer)

    Public Sub New(ByVal dataSheet As DataSheet)

        Debug.Assert(dataSheet.DataScope = DataScope.Project)
        Dim dt As DataTable = dataSheet.GetData()

        For Index As Integer = 0 To dt.Rows.Count - 1

            Dim pkid As Integer = CInt(dt.Rows(Index)(dataSheet.PrimaryKeyColumn.Name))
            Dim AdjustedIndex As Integer = Index + 1

            Me.m_IndexToPKID.Add(AdjustedIndex, pkid)
            Me.m_PKIDToIndex.Add(pkid, AdjustedIndex)

            Debug.Assert(pkid > 0)

        Next

    End Sub

    Public Function PKIDFromIndex(ByVal index As Integer) As Integer

        Debug.Assert(Me.m_IndexToPKID(index) > 0)
        Return Me.m_IndexToPKID(index)

    End Function

    Public Function IndexFromPKID(ByVal pkid As Integer) As Integer

        Debug.Assert(Me.m_PKIDToIndex(pkid) > 0)
        Return Me.m_PKIDToIndex(pkid)

    End Function

End Class
