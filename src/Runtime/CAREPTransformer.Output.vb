'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core

Partial Class CAREPTransformer

    Private otOUT_TIMESTEP As New MBOutputTable(Of Double)
    Private otOUT_PLANT_TIMESTEP As New MBOutputTable(Of Double)
    Private otOUT_STRATUM_TIMESTEP As New MBOutputTable(Of Double)
    Private otOUT_STRATUM_PLANT_TIMESTEP As New MBOutputTable(Of Double)
    Private otOUT_STRATUM_LACT_TIMESTEP As New MBOutputTable(Of Double)
    Private otOUT_STRATUM_PLANT As New MBOutputTable(Of Double)
    Private otOUT_STAGE_TIMESTEP As New MBOutputTable(Of Double)
    Private otOUT_HOUR As New MBOutputTable(Of Double)
    Private otOUT_PLANT_HOUR As New MBOutputTable(Of Double)
    Private otOUT_PLANT_WITHIN_TIMESTEP As New MBOutputTable(Of Double)

    ''' <summary>
    ''' Writes the data for the specified output table
    ''' </summary>
    ''' <param name="ot"></param>
    ''' <param name="minTimestep"></param>
    ''' <param name="maxTimestep"></param>
    ''' <param name="otherValues"></param>
    ''' <remarks></remarks>
    Private Shared Sub WriteTimesteps(
        ByVal ot As MBOutputTable(Of Double),
        ByVal minTimestep As Integer,
        ByVal maxTimestep As Integer,
        ByVal otherValues() As Integer)

        ot.Write(otherValues, minTimestep, maxTimestep)

    End Sub

    ''' <summary>
    ''' Setup for data output at the start of the simulation
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeOutputTables()

        'Create arrays of ModelVariables to be assigned to each MBOutputTable
        Dim mvOUT_HOUR() As MVHour = _
            {ACCPOOL, ACWPOOL, ANDPOOL, ATPOOL, EXCEI, EXCFI, MCDOMMX, MCNIMX, MPFI, PMCNI, RCCPOOL, RCWPOOL, REI, RENBY, RENCC, RENCW, RPFI}

        Dim mvOUT_PLANT_WITHIN_TIMESTEP() As MVPlant = _
            {ABYFI, ACCDFI, ACCFI, ACWDFI, ACWFI, AFI, ANDFI, KPND, PCC, PCCDIG, PCCDIGIN, PCWDIG, PDIG, PFIP, RCCTAN}

        Dim mvOUT_PLANT_HOUR() As MVPlantHour = _
            {BYFECHR, BYFIHR, BYMEIHR, BYMNIHR, BYNFECHR, BYNIHR, CCDFIHR, CCDNIHR, CCFECHR, CCMEIHR, CCMNIHR, CCNFECHR, CCNIHR, CCNPLHR, CCNPRIOR, _
            CCPOOLHR, CCPRIOR, CWDFIHR, CWDNIHR, CWFECHR, CWMEIHR, CWMNIHR, CWNDFECHR, CWNIHR, CWNPLHR, CWNPRIOR, CWPOOLHR, CWPRIOR, DPMNIHR, DPNFECHR, _
            DPNIHR, DPNPLHR, DPNPRIOR, FECHR, FIHR, MCMNIHR, MCNFECHR, MCNPLHR, MCNPRIOR, MEIHR, MNIHR, NDFECHR, NDFIHR, NDPOOLHR, NDNFECHR, _
            NDNIHR, NDNPLHR, NDNPRIOR, NDPRIOR, NFECHR, NIHR, NPNIHR, NPNURNHR, RBYFI, RCCDFI, RCWDFI, RNDFI, TOTDIG, TPNIHR, UPMNIHR, UPNFECHR, _
            UPNIHR, UPNPLHR, UPNPRIOR}

        Dim mvOUT_PLANT_TIMESTEP() As MVPlantTime = _
            {BYFEC, BYFI, BYMEI, BYMNI, BYNFEC, BYNI, CCDFI, CCDNI, CCFEC, CCMEI, CCMNI, CCNFEC, CCNI, CCNPL, CCPOOL, CWDFI, CWDNI, CWFEC, CWMEI, _
            CWMNI, CWNFEC, CWNI, CWNPL, CWPOOL, DFI, DPMNI, DPNFEC, DPNI, DPNPL, FECP, FIMET, FIP, MCMNI, MCNFEC, MCNPL, MEIP, MNIP, NDFEC, NDFI, NDNFEC, _
            NDNI, NDNPL, NDPOOL, NFEC, NI, NPNI, NPNURN, PCCDNIBY, PCCIBY, PTPCW, RNCCTAN, TPNI, UPMNI, UPNFEC, UPNI, UPNPL}

        Dim mvOUT_STAGE_TIMESTEP() As MVStageTime = _
            {EA, EAFAT, EAINT, EAPRO, EAPRORE, EDFAT, EDPRO, ER, ERFAT, ERINT, ERMET, ERPRO, ERTAR, EU, EUFAT, EUINT, EUPRO, EUPRORE, _
            HP, HPDFAT, HPDPRO, HPUFAT, HPUINT, HPUPRO, HPUPRORE, MNL, NA, NAINT, NAPRO, NAPRORE, NDPRO, NLDPRO, NLUINT, NLUPRO, _
            NLUPRORE, NR, NRINT, NRMET, NRPRO, NRTAR, NU, NUINT, NUPRO, NUPRORE, PERTAR, PNRTAR, PRTAR, STAGEPR}

        Dim mvOUT_STRATUM_PLANT_TIMESTEP() As MVStratumPlantTime = _
            {ADF, BSA, DIET, FB, NDF, PCMAX, PDEG, PNIT}

        Dim mvOUT_STRATUM_TIMESTEP() As MVStratumTime = _
            {SNODEP}

        Dim mvOUT_STRATUM_LACT_TIMESTEP() As MVStratumLactTime = _
            {PFOR, PFOREAT, PFORPAW, PLIE, PRUN, PSTD, PWLK}

        'TODO: TKR - Mod this ? A175-15
        ' module.config change and DBUpdate to add field to Output table
        Dim mvOUT_TIMESTEP() As MVTime = _
            {AGEOUT, CALFFATEOUT, DAYSGES, DAYSLAC, EB, ECONMLK, EFATCHG, ERACT, ERANTL, ERBASE, ERCOAT, EREAT, ERFDEP, ERFET, ERGES, ERGESFAT, ERLAC, ERMA, ERNP, ERPARA, _
            ERPAW, ERPDEP, ERRUN, ERSCRF, ERSTD, ERSUM, ERWLK, EUN, EXCEISUM, EXCFISUM, FEC, FI, FMMAX, KPCC, KPCW, LACT, MAXSNODEP, MAXWTFET, _
            MCDOMMXD, MCNIMXD, MEI, MFN, MNI, MPFIAVG, NB, NPROCHG, NRACT, NRANTL, NRBASE, NRCOAT, NRFDEP, NRFET, NRGES, _
            NRLAC, NRMA, NRPARA, NRPDEP, NRSCRF, NRSUM, NRUM, PCONMLK, PFAT, PMCNIAVG, PMMAX, PPRO, PREG, PTARSUM, _
            QM, RCAP, RPFIAVG, SINKDEP, SNOWX, LOCATION, TER, TGRCALF, TGRFET, TMLKPR, TMLKPRWT, TWTCALF, TWTFET, UNL, WTBODY, _
            WTCALF, WTCON, WTEMPTY, WTFAT, WTFATCHG, WTFATMIN, WTFET, WTFFIF, WTGUT, WTMAT, WTMUS, WTPRO, WTPROCHG, _
            WTPROMIN, WTRUM, WTRUMWET, WTWAT, WTWATFAT, WTWATMUS}

        'Initialize the OutputTables
        Dim otherFieldsRun() As String = {RunFieldName}   'additional fields for each output table
        Dim otherFieldsRunTimestep() As String = {RunFieldName, TimeStepFieldName}   'additional fields for each output table

        Using store As DataStore = Me.Library.CreateDataStore()

            otOUT_HOUR.Initialize(mvOUT_HOUR, otherFieldsRunTimestep, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_HOUR_NAME).GetData(store))
            otOUT_PLANT_WITHIN_TIMESTEP.Initialize(mvOUT_PLANT_WITHIN_TIMESTEP, otherFieldsRunTimestep, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_PLANT_WITHIN_TIMESTEP_NAME).GetData(store))
            otOUT_PLANT_HOUR.Initialize(mvOUT_PLANT_HOUR, otherFieldsRunTimestep, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_PLANT_HOUR_NAME).GetData(store))
            otOUT_PLANT_TIMESTEP.Initialize(mvOUT_PLANT_TIMESTEP, otherFieldsRun, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_PLANT_TIMESTEP_NAME).GetData(store))
            otOUT_STAGE_TIMESTEP.Initialize(mvOUT_STAGE_TIMESTEP, otherFieldsRun, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_STAGE_TIMESTEP_NAME).GetData(store))
            otOUT_STRATUM_PLANT_TIMESTEP.Initialize(mvOUT_STRATUM_PLANT_TIMESTEP, otherFieldsRun, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_STRATUM_PLANT_TIMESTEP_NAME).GetData(store))
            otOUT_STRATUM_LACT_TIMESTEP.Initialize(mvOUT_STRATUM_LACT_TIMESTEP, otherFieldsRun, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_STRATUM_LACT_TIMESTEP_NAME).GetData(store))
            otOUT_STRATUM_TIMESTEP.Initialize(mvOUT_STRATUM_TIMESTEP, otherFieldsRun, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_STRATUM_TIMESTEP_NAME).GetData(store))
            otOUT_TIMESTEP.Initialize(mvOUT_TIMESTEP, otherFieldsRun, Me.ResultScenario.GetDataSheet(DATAFEED_OUT_TIMESTEP_NAME).GetData(store))

        End Using

    End Sub

End Class
