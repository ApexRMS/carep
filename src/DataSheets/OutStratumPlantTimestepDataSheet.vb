﻿'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Reflection
Imports SyncroSim.Core

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class OutStratumPlantTimestepDataSheet
    Inherits DataSheet

    ''' <summary>
    ''' Writes and clears the data during an auto-save event
    ''' </summary>
    ''' <remarks>
    ''' The writing routines write the indexes so we need to translate them to primary key Ids
    ''' </remarks>
    Protected Overrides Sub WriteAndClearData()

        Dim trxstrat As New IDTranslator(Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME))
        Dim trxplant As New IDTranslator(Me.Project.GetDataSheet(DATAFEED_PLANT_NAME))
        Dim trxmulti As New StratumMultiKeyHelper(Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME))

        For Each dr As DataRow In Me.GetData().Rows

            Dim stratumindex As Integer = CInt(dr(DATASHEET_STRATUM_ID_COLUMN_NAME))
            Dim stratumid As Integer = trxstrat.PKIDFromIndex(stratumindex)

            dr(DATASHEET_STRATUM_ID_COLUMN_NAME) = stratumid

            Dim x As Integer = 0
            Dim y As Integer = 0
            Dim z As Integer = 0

            trxmulti.GetMultiKeys(stratumid, x, y, z)

            dr("StratumLabelXID") = x
            dr("StratumLabelYID") = y
            dr("StratumLabelZID") = z

            Dim plantindex As Integer = CInt(dr(DATASHEET_PLANT_ID_COLUMN_NAME))
            dr(DATASHEET_PLANT_ID_COLUMN_NAME) = trxplant.PKIDFromIndex(plantindex)

        Next

        MyBase.WriteAndClearData()

    End Sub

End Class
