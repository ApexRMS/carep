'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Reflection
Imports SyncroSim.Core

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class CAREPUpdates
    Inherits UpdateProvider

    ''' <summary>
    ''' Performs the database updates for Caribou Cumulative Effects
    ''' </summary>
    ''' <param name="store"></param>
    ''' <param name="currentSchemaVersion"></param>
    ''' <remarks>
    ''' </remarks>
    Public Overrides Sub PerformUpdate(store As DataStore, currentSchemaVersion As Integer)

        If (currentSchemaVersion < 1) Then
            CAREP0000001(store)
        End If

    End Sub

    ''' <summary>
    ''' CAREP0000001
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' Adds fields to the following tables:
    ''' 
    ''' CAREP_OTHER
    ''' 
    ''' </remarks>
    Private Shared Sub CAREP0000001(ByVal store As DataStore)

        If (store.TableExists("CAREP_OTHER")) Then

            ' Add support for  Baseline Cow Weight for Milk Production 
            store.ExecuteNonQuery("ALTER TABLE CAREP_OTHER ADD COLUMN WTTMLKPR DOUBLE")
            store.ExecuteNonQuery("update CAREP_OTHER set WTTMLKPR = 110")

            'Add support for Bite Size ratio compensation
            store.ExecuteNonQuery("ALTER TABLE CAREP_OTHER ADD COLUMN BITESZADULT Integer")
            store.ExecuteNonQuery("update CAREP_OTHER set BITESZADULT = 3")

            store.ExecuteNonQuery("ALTER TABLE CAREP_OTHER ADD COLUMN BITESZCOEF1 DOUBLE")
            store.ExecuteNonQuery("update CAREP_OTHER set BITESZCOEF1 = 0.2273")

            store.ExecuteNonQuery("ALTER TABLE CAREP_OTHER ADD COLUMN BITESZCOEF2  DOUBLE")
            store.ExecuteNonQuery("update CAREP_OTHER set BITESZCOEF2  = 0.354")

        End If

    End Sub

End Class

