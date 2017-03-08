'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core

Class OutPlantTimestepDataSheet
    Inherits DataSheet

    ''' <summary>
    ''' Writes and clears the data during an auto-save event
    ''' </summary>
    ''' <remarks>
    ''' The writing routines write the indexes so we need to translate them to primary key Ids
    ''' </remarks>
    Protected Overrides Sub WriteAndClearData()

        Dim trxplant As New IDTranslator(Me.Project.GetDataSheet(DATAFEED_PLANT_NAME))

        For Each dr As DataRow In Me.GetData().Rows

            Dim plantindex As Integer = CInt(dr(DATASHEET_PLANT_ID_COLUMN_NAME))
            dr(DATASHEET_PLANT_ID_COLUMN_NAME) = trxplant.PKIDFromIndex(plantindex)

        Next

        MyBase.WriteAndClearData()

    End Sub

End Class
