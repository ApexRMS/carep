'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Math
Imports System.Reflection
Imports SyncroSim.StochasticTime

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class CAREPTransformer
    Inherits StochasticTimeTransformer

    Private m_StartYear As Integer
    Private m_StartJDay As Integer
    Private m_EndYear As Integer
    Private m_EndJDay As Integer
    Private m_MinPlantIndex As Integer
    Private m_MaxPlantIndex As Integer
    Private m_MinStageIndex As Integer
    Private m_MaxStageIndex As Integer
    Private m_OutputLevel As OutputLevel = OutputLevel.Medium

    ''' <summary>
    ''' Overrides Configure
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Configure()

        MyBase.Configure()
        Me.NormalizeRunControl()

    End Sub

    ''' <summary>
    ''' Overrides Initialize
    ''' </summary>
    Public Overrides Sub Initialize()

        MyBase.Initialize()

        Me.InitializeModel()
        Me.InitializeInputTables()
        Me.InitializeDefinitions()
        Me.InitializeModelVariables()
        Me.InitializeOutputTables()
        Me.TimestepUnits = "Julian Day"

    End Sub

    ''' <summary>
    ''' Overrides OnBeforeIteration
    ''' </summary>
    ''' <param name="iteration"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnBeforeIteration(iteration As Integer)

        Me.LoadModelVariables(iteration)

        'Initialize the variables required by the model
        Dim d As Integer = Me.MinimumTimestep
        Dim p As Integer  'plant type index

        'Todo: Estimate initial feces production FEC(d) based on initial body weight WTBODYIN
        'This is required in order to estimate initial MFN; add PFECIN to IN_OTHER
        'Alternatively could estimate MFN directly (as a propn. of WTBODYIN)
        ' FEC(d) = PFECIN * WTBODYIN
        FEC(d) = 0.0

        WTBODY(d) = WTBODYIN
        AGEOUT(d) = AGEIN
        CALFFATEOUT(d) = CALFFATEIN
        WTFAT(d) = WTBODY(d) * PFATIN
        WTWAT(d) = WTBODY(d) * PWATIN   'Todo: Initialize water weight automatically

        'Set the initial bite size modifier based on body weight.
        'Later update for juveniles but keep constant for adults.
        BITESZWTMOD(d) = BITESZCOEF1 * (WTBODY(d) ^ BITESZCOEF2)

        'Determine the animal's pregnancy and lactation status
        CalculatePregnancyLactation(d)

        'Initialize the fetus weight
        TWTFET(d) = CalculateTargetFetusWeight(d)
        'WTFET(d) = TWTFET(d) * PWTFETIN    Todo: Calculate inititial fetus weight as a user-set proportion of target
        WTFET(d) = TWTFET(d)

        If PCONFET > 0.0 Then
            WTCON(d) = WTFET(d) / PCONFET
        Else
            WTCON(d) = 0.0
        End If

        'Initialize the rumen weight
        'Todo: Automate rumen pool initialization by running the model forward 10 days to get equilibrium pool composition
        WTRUM(d) = 0.0
        For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
            WTRUM(d) = WTRUM(d) + CCPOOLIN(p) + CWPOOLIN(p) + NDPOOLIN(p)
        Next p

        If PRUMDRY > 0.0 Then
            WTRUMWET(d) = WTRUM(d) / (PRUMDRY * 1000.0)
        Else
            WTRUMWET(d) = 0.0
        End If

        If PRUMGUT > 0.0 Then
            WTGUT(d) = WTRUMWET(d) / PRUMGUT
        Else
            WTGUT(d) = 0.0
        End If

        WTFFIF(d) = WTBODY(d) - WTFAT(d) - WTGUT(d) - WTCON(d) - WTWAT(d)
        If WTFFIF(d) < 0.0 Then WTFFIF(d) = 0.0

        WTEMPTY(d) = WTFFIF(d) + WTFAT(d) + WTWAT(d)

        WTPRO(d) = (WTFFIF(d) - 0.343) / 4.184
        If WTPRO(d) < 0.0 Then WTPRO(d) = 0.0

        RCAP(d) = (WTEMPTY(d) - WTWAT(d)) * PRCAP * 1000.0
        If RCAP(d) < 0.0 Then RCAP(d) = 0.0

        MFN(d) = MFNCO * FEC(d) / 1000.0
        EUN(d) = NEUNCO * WTBODY(d) ^ 0.75

        'Generate initial estimate of Target Energy Requirement (TER)
        CalculateRequirements(d, True)

    End Sub

    ''' <summary>
    ''' Overrides OnAfterIteration
    ''' </summary>
    ''' <param name="iteration"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnAfterIteration(iteration As Integer)

        Dim otherValuesRun() As Integer = {iteration}

        If (Me.m_OutputLevel = OutputLevel.Low) Then
            WriteTimesteps(otOUT_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)
        Else

            WriteTimesteps(otOUT_PLANT_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)
            WriteTimesteps(otOUT_STAGE_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)
            WriteTimesteps(otOUT_STRATUM_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)
            WriteTimesteps(otOUT_STRATUM_PLANT_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)
            WriteTimesteps(otOUT_STRATUM_LACT_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)
            WriteTimesteps(otOUT_TIMESTEP, Me.MinimumTimestep, Me.MaximumTimestep, otherValuesRun)

        End If

    End Sub

    ''' <summary>
    ''' Overrides OnTimestep
    ''' </summary>
    ''' <param name="iteration"></param>
    ''' <param name="timestep"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnTimestep(iteration As Integer, timestep As Integer)

        CalculateAgeOut(timestep)
        CalculateCalfFateOut(timestep)
        UpdateMaxSnodep(timestep)
        CalculateIntake(timestep)
        CalculateRequirements(timestep, False)
        CalculateAllocation(timestep)
        CalculateAllocationIndicators(timestep)

        Dim otherValuesRunTimestep() As Integer = {iteration, timestep}

        If (Me.m_OutputLevel = OutputLevel.High) Then

            otOUT_HOUR.Write(otherValuesRunTimestep)
            otOUT_PLANT_HOUR.Write(otherValuesRunTimestep)
            otOUT_PLANT_WITHIN_TIMESTEP.Write(otherValuesRunTimestep)

        End If

    End Sub

    Private Sub CalculateAgeOut(ByVal nTimestep As Integer)

        If nTimestep = Me.MinimumTimestep Then
            AGEOUT(nTimestep) = AGEIN
        Else

            If (JulianDayFromTimestep(nTimestep) = ENDAYGES) Then
                AGEOUT(nTimestep) = AGEOUT(nTimestep - 1) + 1
            Else
                AGEOUT(nTimestep) = AGEOUT(nTimestep - 1)
            End If

        End If

    End Sub

    Private Sub CalculateCalfFateOut(ByVal nTimestep As Integer)

        If nTimestep = Me.MinimumTimestep Then
            CALFFATEOUT(nTimestep) = CALFFATEIN
        ElseIf JulianDayFromTimestep(nTimestep) = ENDAYGES + 1 Then
            CALFFATEOUT(nTimestep) = CalfFate.NoCalf
        Else
            CALFFATEOUT(nTimestep) = CALFFATEOUT(nTimestep - 1)
        End If

    End Sub

    Private Sub UpdateMaxSnodep(ByVal nTimestep As Integer)

        Dim s As Integer = CInt(LOCATION(nTimestep))

        If ((nTimestep = Me.MinimumTimestep) Or (JulianDayFromTimestep(nTimestep) = 1)) Then
            MAXSNODEP(Me.MinimumTimestep) = SNODEP(s, nTimestep)
        Else

            If (MAXSNODEP(nTimestep - 1) < SNODEP(s, nTimestep)) Then
                MAXSNODEP(nTimestep) = SNODEP(s, nTimestep)
            Else
                MAXSNODEP(nTimestep) = MAXSNODEP(nTimestep - 1)
            End If

        End If

    End Sub

    ''' <summary>
    ''' Calculates the Forage Intake within each Timestep
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateIntake(ByVal nTimestep As Integer)
        'Declare any temporary variables
        Dim d As Integer = nTimestep   'day index - set using argument
        Dim h As Integer   'hour index
        Dim p As Integer  'plant type index
        Dim s As Integer = CInt(LOCATION(d))  'stratum index - set based on user selection for the day
        Dim sumAFI As Double   'temporary variable
        Dim sumRBYFI As Double  'temporary variable
        'Dim sumCCPRIOR As Double 'temporary variable
        'Dim sumCWPRIOR As Double 'temporary variable
        'Dim nonTPNIHR As Double ' temporary variable to store non True Protein-N
        'Dim sumFECES As Double 'temporary variable
        Dim previousDayRCAP As Double  ' previous day's RCAP
        Dim previousDayTER As Double  ' previous day's TER
        Dim hoursInDay As Double = CDbl(MaxHour - MinHour)  ' used in calculations
        Dim LactStatus As Integer = CInt(LactationStatus.NotLactating)

        If (d > 1) Then
            If (CALFFATEOUT(d - 1) = CalfFate.Lactating) Then
                LactStatus = CInt(LactationStatus.Lactating)
            End If
        End If

        '---------------------------------------------------
        'VARIABLES THAT ARE CONSTANT OVER ALL HOURS OF A DAY
        '---------------------------------------------------

        FI(d) = 0.0

        If d = Me.MinimumTimestep Then
            previousDayRCAP = RCAP(Me.MinimumTimestep)
            previousDayTER = TER(Me.MinimumTimestep)
        Else
            previousDayRCAP = RCAP(d - 1)
            previousDayTER = TER(d - 1)
        End If

        '--------------------------------------------------------------------
        'CALCULATIONS BY PLANT TYPE THAT ARE CONSTANT OVER ALL HOURS OF A DAY
        '--------------------------------------------------------------------
        For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
            'Initialize variables by plant type that will accumulate across hours for the day
            CCDFI(p, d) = 0.0
            CWDFI(p, d) = 0.0
            NDFI(p, d) = 0.0
            DFI(p, d) = 0.0
            FIP(p, d) = 0.0
            FECP(p, d) = 0.0

            'Initialize values for the hourly rumen pools in the initial hour
            'h=MinHour used only to initialize variables each day
            If d = Me.MinimumTimestep Then
                CCPOOLHR(p, 0) = CCPOOLIN(p)
                CWPOOLHR(p, 0) = CWPOOLIN(p)
                NDPOOLHR(p, 0) = NDPOOLIN(p)

                'Assume the initial N pools contain N in the same concentration as the intake
                UPNPLHR(p, 0) = PTPNDIG * (1 - PDEG(s, p, d)) * PNIT(s, p, d) * CCPOOLIN(p)
                'Todo: Add a minimum amount of N in the DPN Pool as user input (DPNPLMIN)
                DPNPLHR(p, 0) = 0.0
                MCNPLHR(p, 0) = (PTPNDIG * PDEG(s, p, d) * PNIT(s, p, d) * CCPOOLIN(p)) - DPNPLHR(p, 0)
                CWNPLHR(p, 0) = PTPNDIG * PNIT(s, p, d) * CWPOOLIN(p)
                NDNPLHR(p, 0) = (1 - PTPNDIG) * PNIT(s, p, d) * (CCPOOLIN(p) + CWPOOLIN(p))

            Else
                CCPOOLHR(p, MinHour) = CCPOOL(p, d - 1)
                CWPOOLHR(p, MinHour) = CWPOOL(p, d - 1)
                NDPOOLHR(p, MinHour) = NDPOOL(p, d - 1)
                UPNPLHR(p, MinHour) = UPNPL(p, d - 1)
                DPNPLHR(p, MinHour) = DPNPL(p, d - 1)
                CWNPLHR(p, MinHour) = CWNPL(p, d - 1)
                MCNPLHR(p, MinHour) = MCNPL(p, d - 1)
                NDNPLHR(p, MinHour) = NDNPL(p, d - 1)

            End If

            'Calculate potential forage intake rate
            PFIP(p) = (AR(s, p) * FB(s, p, d)) / (1 + ((AR(s, p) * FB(s, p, d)) / PCMAX(s, p, d)))

            ' Bite Size ratio compensation
            If AGEOUT(d) >= BITESZADULT Then
                If d = Me.MinimumTimestep Then
                    'note that for the first day use the BITESZMOD value specified in the Other table
                    ' This is set in OnBeforeIteration
                Else
                    BITESZWTMOD(d) = BITESZWTMOD(d - 1)
                End If
            Else
                BITESZWTMOD(d) = BITESZCOEF1 * (WTBODY(d) ^ BITESZCOEF2)
            End If

            If d > Me.MinimumTimestep Then
                If BITESZWTMOD(d) < BITESZWTMOD(d - 1) Then
                    BITESZWTMOD(d) = BITESZWTMOD(d - 1)
                End If
            End If

            Dim PFOREATMod As Double = PFOREAT(s, LactStatus, d)

            '  Modify PFOREAT with the "Not Pregnant" Value 
            If (JulianDayFromTimestep(d) >= STDAYGES Or JulianDayFromTimestep(d) <= ENDAYGES) And PREG(d) = 0.0 Then
                PFOREATMod *= ACT_PEM_NOT_PREGNANT
            End If
            'Modify PFOREAT with the "Not Lactating" Value 
            If (JulianDayFromTimestep(d) >= STDAYLAC And JulianDayFromTimestep(d) <= ENDAYLAC) And LACT(d) = 0.0 Then
                PFOREATMod *= ACT_PEM_NOT_LACT
            End If

            'Available forage intake
            AFI(p) = PFIP(p) * 60.0 * DIET(s, p, d) * PFOR(s, LactStatus, d) * PFOREATMod * BITESZWTMOD(d)

            'Available cell wall intake
            ACWFI(p) = AFI(p) * NDF(s, p, d)

            'Available cell content intake
            ACCFI(p) = AFI(p) - ACWFI(p)

            'Cell wall digestibility
            If ADF(s, p, d) <= 0.129 Then
                PCWDIG(p) = 0.3
            ElseIf ADF(s, p, d) < 0.421 Then
                PCWDIG(p) = (93.0 * ADF(s, p, d) ^ 3) - (91.88 * ADF(s, p, d) ^ 2) + (27.103 * ADF(s, p, d)) - 1.8653
            Else
                PCWDIG(p) = 0.2
            End If

            'Cell content digestibility
            PCC(p) = 1 - NDF(s, p, d)
            If PCC(p) <= 0.31 Then
                PCCDIGIN(p) = 0.5
            ElseIf PCC(p) < 0.691 Then
                PCCDIGIN(p) = (-1.83 * PCC(p) ^ 2) + (2.53 * PCC(p)) - 0.10847
            Else
                PCCDIGIN(p) = 0.766
            End If
            RCCTAN(p) = (-0.64065 * BSA(s, p, d) ^ 2) + (0.7031 * BSA(s, p, d))
            PCCDIG(p) = PCCDIGIN(p) - RCCTAN(p)

            'Digestible bypass, cell content and cell wall intake
            ABYFI(p) = ACCFI(p) * PCCIBY(p, d)

            ACCDFI(p) = ACCFI(p) - ABYFI(p) - (ACCFI(p) * (1 - PCCDIG(p)))
            If ACCDFI(p) < 0 Then ACCDFI(p) = 0

            ACWDFI(p) = ACWFI(p) * PCWDIG(p)

            'Nondigestible intake
            ANDFI(p) = AFI(p) - (ACCDFI(p) + ACWDFI(p) + ABYFI(p))

            'Overall dry matter digestibility of forage intake
            If AFI(p) = 0.0 Then
                PDIG(p) = 0.0
            Else
                PDIG(p) = (AFI(p) - ANDFI(p)) / AFI(p)
            End If

            'Non-digestible passage rate
            KPND(p) = 1.0 / ((1.0 / KPNDMIN) - ((1.0 / KPNDMIN) * PDIG(p)))

            'Proportion of true-protein N intake that is cell wall
            If PNIT(s, p, d) >= PTPCW_XMAX Then
                PTPCW(p, d) = PTPCW_YMAX
            ElseIf PNIT(s, p, d) <= PTPCW_XMIN Then
                PTPCW(p, d) = PTPCW_YMIN
            Else
                PTPCW(p, d) = PTPCW_YMIN + ((PNIT(s, p, d) - PTPCW_XMIN) * (PTPCW_YMAX - PTPCW_YMIN) / (PTPCW_XMAX - PTPCW_XMIN))
            End If

            'Reduction in N digestibility of cell content due to the presence of tannins 
            If BSA(s, p, d) >= RNCCTAN_XMAX Then
                RNCCTAN(p, d) = RNCCTAN_YMAX
            ElseIf BSA(s, p, d) <= RNCCTAN_XMIN Then
                RNCCTAN(p, d) = RNCCTAN_YMIN
            Else
                RNCCTAN(p, d) = RNCCTAN_YMIN + ((BSA(s, p, d) - RNCCTAN_XMIN) * (RNCCTAN_YMAX - RNCCTAN_YMIN) / (RNCCTAN_XMAX - RNCCTAN_XMIN))
            End If

        Next p

        '------------------------------------------------------
        'CALCULATIONS THAT VARY BY HOUR
        '------------------------------------------------------

        'Zero daily indicators that accumulate
        For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex

            'Forage intake
            BYFI(p, d) = 0.0
            CCDFI(p, d) = 0.0
            CWDFI(p, d) = 0.0
            NDFI(p, d) = 0.0
            'MEI
            BYMEI(p, d) = 0.0
            CCMEI(p, d) = 0.0
            CWMEI(p, d) = 0.0
            'Fecal production
            BYFEC(p, d) = 0.0
            CCFEC(p, d) = 0.0
            CWFEC(p, d) = 0.0
            NDFEC(p, d) = 0.0
            'Protein nitrogen intake
            NI(p, d) = 0.0
            TPNI(p, d) = 0.0
            NPNI(p, d) = 0.0
            'Cell content and cell wall nitrogen intake
            CWNI(p, d) = 0.0
            CCNI(p, d) = 0.0
            CWDNI(p, d) = 0.0
            CCDNI(p, d) = 0.0
            NDNI(p, d) = 0.0
            'Bypass, undegradable and degradable nitrogen intake
            BYNI(p, d) = 0.0
            UPNI(p, d) = 0.0
            DPNI(p, d) = 0.0
            'Metabolizable nitrogen intake
            BYMNI(p, d) = 0.0
            UPMNI(p, d) = 0.0
            DPMNI(p, d) = 0.0
            CWMNI(p, d) = 0.0
            MCMNI(p, d) = 0.0
            'Fecal N loss
            BYNFEC(p, d) = 0.0
            UPNFEC(p, d) = 0.0
            DPNFEC(p, d) = 0.0
            CWNFEC(p, d) = 0.0
            MCNFEC(p, d) = 0.0
            NDNFEC(p, d) = 0.0
            'Urinary N loss
            NPNURN(p, d) = 0.0
        Next p
        'Additional indicators
        EXCEISUM(d) = 0.0
        EXCFISUM(d) = 0.0
        MPFIAVG(d) = 0.0
        RPFIAVG(d) = 0.0
        PMCNIAVG(d) = 0.0
        FI(d) = 0.0
        MEI(d) = 0.0
        FEC(d) = 0.0
        WTRUM(d) = 0.0
        MNI(d) = 0.0
        NRUM(d) = 0.0

        For h = MinHour + 1 To MaxHour
            'Determine the various pools across plant types for this hour, applying
            'the appropriate digestion or passage rate
            ACCPOOL(h) = 0.0
            ACWPOOL(h) = 0.0
            ANDPOOL(h) = 0.0
            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                ACCPOOL(h) = ACCPOOL(h) + ((CCPOOLHR(p, h - 1) + ACCDFI(p)) * (1 - KPCC(d)))
                ACWPOOL(h) = ACWPOOL(h) + ((CWPOOLHR(p, h - 1) + ACWDFI(p)) * (1 - KPCW(d)))
                ANDPOOL(h) = ANDPOOL(h) + ((NDPOOLHR(p, h - 1) + ANDFI(p)) * (1 - KPND(p)))
            Next
            ATPOOL(h) = ACCPOOL(h) + ACWPOOL(h) + ANDPOOL(h)

            'Calculate the excess forage intake based on rumen capacity
            EXCFI(h) = ATPOOL(h) - previousDayRCAP
            If (EXCFI(h) < 0.0) Then EXCFI(h) = 0.0

            'Calculate the proportion reduction required in forage intake
            sumAFI = 0.0 'set sum over plant types to 0
            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                sumAFI = sumAFI + AFI(p)
            Next
            If (sumAFI = 0.0) Or (sumAFI < EXCFI(h)) Then
                RPFI(h) = 0.0
            Else
                RPFI(h) = (sumAFI - EXCFI(h)) / sumAFI
            End If

            'Calculate the new forage intake, based on rumen capacity constraint
            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                RBYFI(p, h) = RPFI(h) * ABYFI(p)
                RCCDFI(p, h) = RPFI(h) * ACCDFI(p)
                RCWDFI(p, h) = RPFI(h) * ACWDFI(p)
                RNDFI(p, h) = RPFI(h) * ANDFI(p)
            Next

            'Recalculate the CC and CW pool sizes, based on the rumen capacity constrained forage
            'Sum the bypass intake over plant groups
            RCCPOOL(h) = 0.0
            RCWPOOL(h) = 0.0
            sumRBYFI = 0.0
            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                sumRBYFI = sumRBYFI + RBYFI(p, h)
                RCCPOOL(h) = RCCPOOL(h) + ((CCPOOLHR(p, h - 1) + RCCDFI(p, h)) * (1 - KPCC(d)))
                RCWPOOL(h) = RCWPOOL(h) + ((CWPOOLHR(p, h - 1) + RCWDFI(p, h)) * (1 - KPCW(d)))
            Next

            'Energy yield from rumen capacity constrained Bypass, CC and CW pools
            RENBY(h) = sumRBYFI * DEBY * PMEBY
            RENCC(h) = RCCPOOL(h) * KPCC(d) * DECC * PMECC
            RENCW(h) = RCWPOOL(h) * KPCW(d) * DECW * PMECW
            REI(h) = RENBY(h) + RENCC(h) + RENCW(h)

            'Calculate the excess forage intake based on target metabolic requirement
            EXCEI(h) = REI(h) - (previousDayTER / 24.0)
            If (EXCEI(h) < 0.0) Then EXCEI(h) = 0.0

            'Calculate the proportion reduction required in rumen capacity constrained intake
            'due to metabolic constraint
            If (REI(h) = 0.0) Or (REI(h) < EXCEI(h)) Then
                MPFI(h) = 0.0
            Else
                MPFI(h) = (REI(h) - EXCEI(h)) / REI(h)
            End If

            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                'Calculate the new forage intake, based on combination of rumen capacity and metabolic constraints
                BYFIHR(p, h) = MPFI(h) * RBYFI(p, h)
                CCDFIHR(p, h) = MPFI(h) * RCCDFI(p, h)
                CWDFIHR(p, h) = MPFI(h) * RCWDFI(p, h)
                NDFIHR(p, h) = MPFI(h) * RNDFI(p, h)
                FIHR(p, h) = BYFIHR(p, h) + CCDFIHR(p, h) + CWDFIHR(p, h) + NDFIHR(p, h)

                'Recalculate the pool sizes, based on the rumen capacity and metabolic constrained forage
                CCPRIOR(p, h) = CCPOOLHR(p, h - 1) + CCDFIHR(p, h)
                CWPRIOR(p, h) = CWPOOLHR(p, h - 1) + CWDFIHR(p, h)
                NDPRIOR(p, h) = NDPOOLHR(p, h - 1) + NDFIHR(p, h)

                'MEI from bypass and CC and CW pools recalculated (based on pools prior to digestion)
                BYMEIHR(p, h) = BYFIHR(p, h) * DEBY * PMEBY
                CCMEIHR(p, h) = CCPRIOR(p, h) * KPCC(d) * DECC * PMECC
                CWMEIHR(p, h) = CWPRIOR(p, h) * KPCW(d) * DECW * PMECW
                MEIHR(p, h) = BYMEIHR(p, h) + CCMEIHR(p, h) + CWMEIHR(p, h)

                'Update the rumen pool sizes after digestion and passage
                CCPOOLHR(p, h) = CCPRIOR(p, h) * (1 - KPCC(d))
                CWPOOLHR(p, h) = CWPRIOR(p, h) * (1 - KPCW(d))
                NDPOOLHR(p, h) = NDPRIOR(p, h) * (1 - KPND(p))

                'Fecal production
                BYFECHR(p, h) = BYFIHR(p, h) * (1 - PMEBY)
                CCFECHR(p, h) = CCPRIOR(p, h) * KPCC(d) * (1 - PMECC)
                CWFECHR(p, h) = CWPRIOR(p, h) * KPCW(d) * (1 - PMECW)
                NDFECHR(p, h) = NDPRIOR(p, h) * KPND(p)
                FECHR(p, h) = BYFECHR(p, h) + CCFECHR(p, h) + CWFECHR(p, h) + NDFECHR(p, h)

                'Total digested material
                TOTDIG(p, h) = BYFIHR(p, h) + (CCPRIOR(p, h) * KPCC(d)) + (CWPRIOR(p, h) * KPCW(d))

            Next p

            '---------------------------------------------
            ' NITROGEN INTAKE
            '---------------------------------------------

            'Zero-out hourly variables that accumulate
            MCDOMMX(h) = 0
            MCNIMX(h) = 0

            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                'Protein-N intake
                NIHR(p, h) = PNIT(s, p, d) * FIHR(p, h)
                TPNIHR(p, h) = PTPNDIG * NIHR(p, h)
                NPNIHR(p, h) = NIHR(p, h) - TPNIHR(p, h)

                'Cell content and cell wall nitrogen intake
                CWNIHR(p, h) = PTPCW(p, d) * TPNIHR(p, h)
                CCNIHR(p, h) = TPNIHR(p, h) - CWNIHR(p, h)

                'Digestible and non-digestible nitrogen intake
                CCDNIHR(p, h) = CCNIHR(p, h) * (1 - RNCCTAN(p, d))
                CWDNIHR(p, h) = CWNIHR(p, h) * PCWDIG(p)
                NDNIHR(p, h) = CCNIHR(p, h) + CWNIHR(p, h) - CCDNIHR(p, h) - CWDNIHR(p, h)

                'Bypass, undegradable and degradable nitrogen intake

                '*****************************
                'Todo: hookup PCCDNIBY as an input property - for now use PCCIBY
                'BYNIHR(p, h) = CCDNIHR(p, h) * PCCDNIBY(p, d)
                BYNIHR(p, h) = CCDNIHR(p, h) * PCCIBY(p, d)
                '*****************************
                DPNIHR(p, h) = PDEG(s, p, d) * (CCDNIHR(p, h) - BYNIHR(p, h))
                UPNIHR(p, h) = CCDNIHR(p, h) - BYNIHR(p, h) - DPNIHR(p, h)

                'Microbial nitrogen intake
                MCDOMMX(h) = MCDOMMX(h) + ((CCPOOLHR(p, h) + CWPOOLHR(p, h)) * PDOM * PMCDOM)
                MCNIMX(h) = MCNIMX(h) + MCNPLHR(p, h - 1) + DPNIHR(p, h) + NPNIHR(p, h)

                PMCNI(h) = MCDOMMX(h) / MCNIMX(h)
                If PMCNI(h) > 1 Then PMCNI(h) = 1

                'Nitrogen rumen pool sizes prior to passage
                UPNPRIOR(p, h) = UPNPLHR(p, h - 1) + UPNIHR(p, h)
                DPNPRIOR(p, h) = DPNPLHR(p, h - 1) + (DPNIHR(p, h) * (1 - PMCNI(h)))
                CWNPRIOR(p, h) = CWNPLHR(p, h - 1) + CWDNIHR(p, h)
                MCNPRIOR(p, h) = MCNPLHR(p, h - 1) + (PMCNI(h) * (DPNIHR(p, h) + NPNIHR(p, h)))
                NDNPRIOR(p, h) = NDNPLHR(p, h - 1) + NDNIHR(p, h)

                'Metabolizable nitrogen intake
                BYMNIHR(p, h) = BYNIHR(p, h) * PBYN
                UPMNIHR(p, h) = UPNPRIOR(p, h) * KPCC(d) * PUPN
                DPMNIHR(p, h) = DPNPRIOR(p, h) * KPCC(d) * PDPN
                CWMNIHR(p, h) = CWNPRIOR(p, h) * KPCW(d) * PCWN
                MCMNIHR(p, h) = MCNPRIOR(p, h) * KPCC(d) * PMCN
                MNIHR(p, h) = BYMNIHR(p, h) + UPMNIHR(p, h) + DPMNIHR(p, h) + CWMNIHR(p, h) + MCMNIHR(p, h)

                'Nitrogen rumen pool sizes after passage
                UPNPLHR(p, h) = UPNPRIOR(p, h) * (1 - KPCC(d))
                DPNPLHR(p, h) = DPNPRIOR(p, h) * (1 - KPCC(d))
                CWNPLHR(p, h) = CWNPRIOR(p, h) * (1 - KPCW(d))
                MCNPLHR(p, h) = MCNPRIOR(p, h) * (1 - KPCC(d))
                NDNPLHR(p, h) = NDNPRIOR(p, h) * (1 - KPND(p))

                'Fecal nitrogen loss
                BYNFECHR(p, h) = BYNIHR(p, h) * (1 - PBYN)
                UPNFECHR(p, h) = UPNPRIOR(p, h) * KPCC(d) * (1 - PUPN)
                DPNFECHR(p, h) = DPNPRIOR(p, h) * KPCC(d) * (1 - PDPN)
                CWNDFECHR(p, h) = CWNPRIOR(p, h) * KPCW(d) * (1 - PCWN)
                MCNFECHR(p, h) = MCNPRIOR(p, h) * KPCC(d) * (1 - PMCN)
                NDNFECHR(p, h) = NDNPRIOR(p, h) * KPND(p)
                NFECHR(p, h) = BYNFECHR(p, h) + UPNFECHR(p, h) + DPNFECHR(p, h) + CWNDFECHR(p, h) + MCNFECHR(p, h) + NDNFECHR(p, h)

                'Urinary nitrogen loss from non-protein nitrogen
                NPNURNHR(p, h) = NPNIHR(p, h) * (1 - PMCNI(h))

            Next p

            '---------------------------------------------
            'CALCULATE DAILY INDICATORS
            '---------------------------------------------
            For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
                'Forage intake
                BYFI(p, d) = BYFI(p, d) + BYFIHR(p, h)
                CCDFI(p, d) = CCDFI(p, d) + CCDFIHR(p, h)
                CWDFI(p, d) = CWDFI(p, d) + CWDFIHR(p, h)
                NDFI(p, d) = NDFI(p, d) + NDFIHR(p, h)

                'MEI
                BYMEI(p, d) = BYMEI(p, d) + BYMEIHR(p, h)
                CCMEI(p, d) = CCMEI(p, d) + CCMEIHR(p, h)
                CWMEI(p, d) = CWMEI(p, d) + CWMEIHR(p, h)

                'Fecal production
                BYFEC(p, d) = BYFEC(p, d) + BYFECHR(p, h)
                CCFEC(p, d) = CCFEC(p, d) + CCFECHR(p, h)
                CWFEC(p, d) = CWFEC(p, d) + CWFECHR(p, h)
                NDFEC(p, d) = NDFEC(p, d) + NDFECHR(p, h)

                'Protein nitrogen intake
                NI(p, d) = NI(p, d) + NIHR(p, h)
                TPNI(p, d) = TPNI(p, d) + TPNIHR(p, h)
                NPNI(p, d) = NPNI(p, d) + NPNIHR(p, h)

                'Cell content and cell wall nitrogen intake
                CWNI(p, d) = CWNI(p, d) + CWNIHR(p, h)
                CCNI(p, d) = CCNI(p, d) + CCNIHR(p, h)
                CWDNI(p, d) = CWDNI(p, d) + CWDNIHR(p, h)
                CCDNI(p, d) = CCDNI(p, d) + CCDNIHR(p, h)
                NDNI(p, d) = NDNI(p, d) + NDNIHR(p, h)

                'Bypass, undegradable and degradable nitrogen intake
                BYNI(p, d) = BYNI(p, d) + BYNIHR(p, h)
                UPNI(p, d) = UPNI(p, d) + UPNIHR(p, h)
                DPNI(p, d) = DPNI(p, d) + DPNIHR(p, h)

                'Metabolizable nitrogen intake
                BYMNI(p, d) = BYMNI(p, d) + BYMNIHR(p, h)
                UPMNI(p, d) = UPMNI(p, d) + UPMNIHR(p, h)
                DPMNI(p, d) = DPMNI(p, d) + DPMNIHR(p, h)
                CWMNI(p, d) = CWMNI(p, d) + CWMNIHR(p, h)
                MCMNI(p, d) = MCMNI(p, d) + MCMNIHR(p, h)

                'Fecal N loss
                BYNFEC(p, d) = BYNFEC(p, d) + BYNFECHR(p, h)
                UPNFEC(p, d) = UPNFEC(p, d) + UPNFECHR(p, h)
                DPNFEC(p, d) = DPNFEC(p, d) + DPNFECHR(p, h)
                CWNFEC(p, d) = CWNFEC(p, d) + CWNDFECHR(p, h)
                MCNFEC(p, d) = MCNFEC(p, d) + MCNFECHR(p, h)
                NDNFEC(p, d) = NDNFEC(p, d) + NDNFECHR(p, h)

                'Urinary N loss
                NPNURN(p, d) = NPNURN(p, d) + NPNURNHR(p, h)

            Next p
            'Additional indicators
            EXCEISUM(d) = EXCEISUM(d) + EXCEI(h)
            EXCFISUM(d) = EXCFISUM(d) + EXCFI(h)
            MPFIAVG(d) = MPFIAVG(d) + (MPFI(h) / hoursInDay)
            RPFIAVG(d) = RPFIAVG(d) + (RPFI(h) / hoursInDay)
            PMCNIAVG(d) = PMCNIAVG(d) + (PMCNI(h) / hoursInDay)

        Next h
        '---------------------------------------------
        'END OF HOURLY LOOP
        '---------------------------------------------

        '---------------------------------------------
        'ADDITIONAL DAILY INDICATORS
        '---------------------------------------------

        For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex

            'Total the forage intake
            DFI(p, d) = CCDFI(p, d) + CWDFI(p, d) + BYFI(p, d)
            FIP(p, d) = DFI(p, d) + NDFI(p, d)
            FI(d) = FI(d) + FIP(p, d)
            'GEI(d) = GEI(d) + (CCDFI(p, d) * DECC) + (CWDFI(p, d) * DECW) + (NDFI(p, d) * DEND)

            'Total MEI
            MEIP(p, d) = BYMEI(p, d) + CCMEI(p, d) + CWMEI(p, d)
            MEI(d) = MEI(d) + MEIP(p, d)

            'Total Fecal production
            FECP(p, d) = BYFEC(p, d) + CCFEC(p, d) + CWFEC(p, d) + NDFEC(p, d)
            FEC(d) = FEC(d) + FECP(p, d)

            'Update the daily values for the rumen pools as the values at the last hour of the day
            CCPOOL(p, d) = CCPOOLHR(p, MaxHour)
            CWPOOL(p, d) = CWPOOLHR(p, MaxHour)
            NDPOOL(p, d) = NDPOOLHR(p, MaxHour)

            'Calculate the total rumen contents on the last hour of the day
            'Todo: change WTRUM to be by plant type
            WTRUM(d) = WTRUM(d) + CCPOOL(p, d) + CWPOOL(p, d) + NDPOOL(p, d)

            'Total MNI
            MNIP(p, d) = BYMNI(p, d) + UPMNI(p, d) + DPMNI(p, d) + CWMNI(p, d) + MCMNI(p, d)
            MNI(d) = MNI(d) + MNIP(p, d)

            'Update the daily values for the rumen N pools as the values at the last hour of the day
            UPNPL(p, d) = UPNPLHR(p, MaxHour)
            DPNPL(p, d) = DPNPLHR(p, MaxHour)
            CWNPL(p, d) = CWNPLHR(p, MaxHour)
            MCNPL(p, d) = MCNPLHR(p, MaxHour)
            NDNPL(p, d) = NDNPLHR(p, MaxHour)

            'Calculate the total N rumen contents on the last hour of the day
            'Todo: change NRUM to be by plant type
            NRUM(d) = NRUM(d) + UPNPL(p, d) + DPNPL(p, d) + CWNPL(p, d) + MCNPL(p, d) + NDNPL(p, d)

            'Total Fecal N loss
            'Todo: Create NFECTOT(d) variable and add MFN(d) to it
            NFEC(p, d) = BYNFEC(p, d) + UPNFEC(p, d) + DPNFEC(p, d) + CWNFEC(p, d) + MCNFEC(p, d) + NDNFEC(p, d)

        Next

        'Maximum microbial nitrogen intake on the last hour of the day
        MCDOMMXD(d) = MCDOMMX(MaxHour)
        MCNIMXD(d) = MCNIMX(MaxHour)

    End Sub

    ''' <summary>
    ''' Calculates the energy and N requirements for allocation
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateRequirements(ByVal nTimestep As Integer, ByVal initializing As Boolean)

        Dim d As Integer = nTimestep   'day index - set using argument
        Dim s As Integer = CInt(LOCATION(d))  'stratum index - set based on user selection for the day
        Dim a As Integer  'allocation stage index
        Dim LactStatus As Integer = CInt(LactationStatus.NotLactating)

        If (d > 1) Then
            If (CALFFATEOUT(d - 1) = CalfFate.Lactating) Then
                LactStatus = CInt(LactationStatus.Lactating)
            End If
        End If

        'Dim dg As Double 'days of gestation

        Dim previousDayWTBODY As Double
        Dim previousDayWTEMPTY As Double
        Dim previousDayWTFET As Double
        Dim previousDayTWTFET As Double
        Dim previousDayMFN As Double
        Dim previousDayEUN As Double

        'Get variable values from previous day
        If d = Me.MinimumTimestep Then
            previousDayWTBODY = WTBODY(d)
            previousDayWTEMPTY = WTEMPTY(d)
            previousDayWTFET = WTFET(d)
            previousDayTWTFET = WTFET(d)
            previousDayMFN = MFN(d)
            previousDayEUN = EUN(d)
        Else
            previousDayWTBODY = WTBODY(d - 1)
            previousDayWTEMPTY = WTEMPTY(d - 1)
            previousDayWTFET = WTFET(d - 1)
            previousDayTWTFET = TWTFET(d - 1)
            previousDayMFN = MFN(d - 1)
            previousDayEUN = EUN(d - 1)
        End If

        'Determine pregnancy and lacatation status
        CalculatePregnancyLactation(nTimestep)

        'Base requirement
        ERBASE(d) = Round(EBMRCO * previousDayWTEMPTY ^ 0.75, precision)
        NRBASE(d) = Round(previousDayMFN + previousDayEUN, precision)

        'Requirement for coat production
        If (JulianDayFromTimestep(d) >= STDAYCT) And (JulianDayFromTimestep(d) <= ENDAYCT) And STDAYCT <> 0 Then
            NRCOAT(d) = Round(NCOATCO * previousDayWTBODY ^ 0.75, precision)
            If NCONPRO = 0.0 Then
                ERCOAT(d) = 0.0
            Else
                'Todo: check with Bob on correct calculation of ERCOAT (see email Mar 23/11)
                'May need to remove ECONCOAT from model
                'ERCOAT(d) = Round(NRCOAT(d) * ECONCOAT / NCONPRO, precision)
                ERCOAT(d) = Round(NRCOAT(d) * ECONPRO / NCONPRO, precision)
            End If
        Else
            NRCOAT(d) = 0.0
            ERCOAT(d) = 0.0
        End If

        'Requirement for scurf production
        'Todo: fix the handling of start/end dates to run multiple years
        'If (d >= STDAYSC) And (d <= ENDAYSC) Then
        If Not ((JulianDayFromTimestep(d) > ENDAYSC) And (JulianDayFromTimestep(d) < STDAYSC)) And STDAYSC <> 0 Then
            NRSCRF(d) = Round(NSCRFCO * previousDayWTBODY ^ 0.6, precision)
            'ERSCRF(d) = Round(ESCRFCO * previousDayWTBODY ^ 0.75, precision)
            ERSCRF(d) = Round(NRSCRF(d) * ECONPRO / NCONPRO, precision)

        Else
            NRSCRF(d) = 0.0
            ERSCRF(d) = 0.0
        End If

        'Added cost of locomotion in snow
        SINKDEP(d) = SDPROP * SNODEP(s, d)
        SNOWX(d) = 1 + 0.0241623 * Exp(0.0635 * SINKDEP(d) * 1.587)

        'Requirement of activity
        NRACT(d) = 0.0

        Dim PFOREATMod As Double = PFOREAT(s, LactStatus, d)

        '  Modify PFOREAT with the "Not Pregnant" Value 
        If (JulianDayFromTimestep(d) >= STDAYGES Or JulianDayFromTimestep(d) <= ENDAYGES) And PREG(d) = 0.0 Then
            PFOREATMod *= ACT_PEM_NOT_PREGNANT
        End If
        'Modify PFOREAT with the "Not Lactating" Value 
        If (JulianDayFromTimestep(d) >= STDAYLAC And JulianDayFromTimestep(d) <= ENDAYLAC) And LACT(d) = 0.0 Then
            PFOREATMod *= ACT_PEM_NOT_LACT
        End If


        'Todo: add ERLIE back in as a variable to allow users to add a cost for this if desired
        'ERLIE(d) = PLIE(s, d) * HRELIE * previousDayWTBODY * 24.0
        ERSTD(d) = PSTD(s, LactStatus, d) * HRESTD * previousDayWTBODY * 24.0
        ERRUN(d) = PRUN(s, LactStatus, d) * SNOWX(d) * HRERUN * previousDayWTBODY * 24.0
        ERPAW(d) = PFOR(s, LactStatus, d) * PFORPAW(s, LactStatus, d) * HREPAW * previousDayWTBODY * 24.0
        ERWLK(d) = (PWLK(s, LactStatus, d) + (PFOR(s, LactStatus, d) * (1 - PFOREATMod - PFORPAW(s, LactStatus, d)))) * SNOWX(d) * HREWLK * previousDayWTBODY * 24.0

        'Todo: Calculate EREAT as a function of FIP
        'If initializing Then
        '    'Assume user-entered cost for eating when when initializing (as FIP not known)
        EREAT(d) = (PFOR(s, LactStatus, d) * PFOREATMod * HREEAT * previousDayWTBODY * 24.0)
        'Else
        '    EREAT(d) = (PFOR(s, d) * PFOREAT(s, d) * HRESTD * previousDayWTBODY * 24.0) + ((557 - 1.595 * previousDayWTBODY) * FI(d) / 1000.0)

        'End If
        'Todo: add ERLIE back in as a variable to allow users to add a cost for this if desired
        'ERACT( d) = ERLIE(d) + ERSTD(d) + ERRUN(d) + ERPAW(d) + ERWLK(d) + EREAT(d)
        ERACT(d) = Round(ERSTD(d) + ERRUN(d) + ERPAW(d) + ERWLK(d) + EREAT(d), precision)

        'Requirement for Summer Protein Deposition
        If (JulianDayFromTimestep(d) >= STDAYSUM) And (JulianDayFromTimestep(d) <= ENDAYSUM) And STDAYSUM <> 0 Then
            NRSUM(d) = Round(PTARSUM(d) * NCONPRO, precision)
            If NCONPRO = 0.0 Then
                ERSUM(d) = 0.0
            Else
                ERSUM(d) = Round(NRSUM(d) * ECONPRO / NCONPRO, precision)
            End If
        Else
            NRSUM(d) = 0.0
            ERSUM(d) = 0.0
        End If

        If PREG(d) <> 0.0 Then
            TWTFET(d) = CalculateTargetFetusWeight(d)
            TGRFET(d) = (TWTFET(d) - previousDayTWTFET)
            If TGRFET(d) < 0.0 Then TGRFET(d) = 0.0

            'Nitrogen Requirement for fetus and gestation
            'NRFET(d) = Round(NFETCO * TWTFET(d), precision)
            NRFET(d) = Round(NFETCO * (1 + TGRFET(d)) * previousDayWTFET, precision)
            NRGES(d) = Round(NRFET(d) / PCONFET, precision)

            'Energy Requirement for Gestation
            ERGESFAT(d) = Round(PFETFAT * (TWTFET(d) - previousDayWTFET) * ECONFAT, precision)
            If NCONPRO = 0.0 Then
                ERFET(d) = 0.0
            Else
                ERFET(d) = Round((NRFET(d) * (ECONPRO / NCONPRO)) + ERGESFAT(d), precision)
            End If
            ERGES(d) = Round(ERFET(d) / PCONFET, precision)
        Else
            'No gestation
            DAYSGES(d) = 0.0
            MAXWTFET(d) = 0.0
            TGRFET(d) = 0.0
            TWTFET(d) = 0.0
            NRFET(d) = 0.0
            NRGES(d) = 0.0
            ERFET(d) = 0.0
            ERGES(d) = 0.0
        End If

        If LACT(d) <> 0.0 Then
            'Requirement for Lactation
            DAYSLAC(d) = CDbl(JulianDayFromTimestep(d) - STDAYLAC)
            If DAYSLAC(d) < 0.0 Then DAYSLAC(d) = DAYSLAC(d) + 365.0

            'Target milk production
            If DAYSLAC(d) < 0.0 Then
                DAYSLAC(d) = DAYSLAC(d) + 365.0
            End If
            If DAYSLAC(d) <= 1.0 Then
                TMLKPR(d) = 0.731
            ElseIf DAYSLAC(d) <= 2.0 Then
                TMLKPR(d) = 1.91
            Else
                TMLKPR(d) = (1.606 * Exp(-0.0072 * DAYSLAC(d))) + (0.374 * Exp(-0.0617 * DAYSLAC(d)))
            End If
            If previousDayWTBODY > 0.0 Then
                TMLKPRWT(d) = (TMLKPR(d) * 1000.0) / previousDayWTBODY
            Else
                TMLKPRWT(d) = 0.0
            End If

            ' Adjust target milk production by the ratio of the Cow Weight:Baseline Cow Weight.
            TMLKPR(d) *= previousDayWTBODY / WTTMLKPR

            'Nitrogen requirement for lactation
            PCONMLK(d) = 0.14 - (0.0034 * TMLKPRWT(d))
            NRLAC(d) = Round(TMLKPR(d) * 1000 * PCONMLK(d) * NCONPRO, precision)

            'Energy requirement for lactation
            ECONMLK(d) = 12.24 - (0.0032334 * TMLKPR(d) * 1000)
            ERLAC(d) = Round(TMLKPR(d) * 1000 * ECONMLK(d), precision)

        Else
            'No lactation
            DAYSLAC(d) = 0.0
            TMLKPR(d) = 0.0
            TMLKPRWT(d) = 0.0
            PCONMLK(d) = 0.0
            NRLAC(d) = 0.0
            ECONMLK(d) = 0.0
            ERLAC(d) = 0.0
        End If

        'Requirement for antler production
        If (JulianDayFromTimestep(d) >= STDAYAN) And (JulianDayFromTimestep(d) <= ENDAYAN) And STDAYAN <> 0 Then
            NRANTL(d) = Round(NANTLCO * previousDayWTBODY ^ 0.75, precision)
            ERANTL(d) = Round(EANTLCO * previousDayWTBODY ^ 0.75, precision)
        Else
            NRANTL(d) = 0.0
            ERANTL(d) = 0.0
        End If

        'Requirement for Additional Protein Deposition
        NRPDEP(d) = Round(PROMOBRT * NCONPRO, precision)
        If NCONPRO = 0.0 Then
            ERPDEP(d) = 0.0
        Else
            ERPDEP(d) = Round(NRPDEP(d) * (ECONPRO / NCONPRO), precision)
        End If

        'Requirement for Fat Deposition
        ERFDEP(d) = Round(FATMOBRT * ECONPRO, precision)
        NRFDEP(d) = 0.0

        'Assign Target Energy and Nitrogen Requirements to each stage
        ER(Stage.Base, d) = ERBASE(d)
        ER(Stage.Parasites, d) = ERPARA(d)
        ER(Stage.Coat, d) = ERCOAT(d)
        ER(Stage.Scurf, d) = ERSCRF(d)
        ER(Stage.Activity, d) = ERACT(d)
        ER(Stage.SummerProtein, d) = ERSUM(d)
        ER(Stage.Gestation, d) = ERGES(d)
        ER(Stage.Lactation, d) = ERLAC(d)
        ER(Stage.Antlers, d) = ERANTL(d)
        ER(Stage.Protein, d) = ERPDEP(d)
        ER(Stage.Fat, d) = ERFDEP(d)

        NR(Stage.Base, d) = NRBASE(d)
        NR(Stage.Parasites, d) = NRPARA(d)
        NR(Stage.Coat, d) = NRCOAT(d)
        NR(Stage.Scurf, d) = NRSCRF(d)
        NR(Stage.Activity, d) = NRACT(d)
        NR(Stage.SummerProtein, d) = NRSUM(d)
        NR(Stage.Gestation, d) = NRGES(d)
        NR(Stage.Lactation, d) = NRLAC(d)
        NR(Stage.Antlers, d) = NRANTL(d)
        NR(Stage.Protein, d) = NRPDEP(d)
        NR(Stage.Fat, d) = NRFDEP(d)

        'Efficiencies of use of intake (KEINT) that are not user-supplied
        'Initial values are taken as supplied by the user
        If Not initializing Then

            'Calculate KEINT as a function of QM

            'Metabolizability Coefficient (QM)
            If (FI(d) * ECONFI) <> 0.0 Then
                QM(d) = MEI(d) / (FI(d) * ECONFI)
            Else
                QM(d) = 0.0
            End If

            'KEINT for Base, Gestation and Lactation
            'Todo: make KEINT a function of QM
            'a = Stage.Base
            'KEINT(a) = (0.35 * QM(d)) + 0.503
            'a = Stage.Gestation
            'KEINT(a) = (0.78 * QM(d)) + 0.006
            'a = Stage.Lactation
            'KEINT(a) = (0.35 * QM(d)) + 0.42
        End If

        'KEINT for activity set to be the same as for base
        KEINT(Stage.Activity) = KEINT(Stage.Base)

        'Target Metabolic Energy Requirement
        TER(d) = 0.0
        For a = Me.m_MinStageIndex To Me.m_MaxStageIndex
            If KEINT(a) > 0.0 Then TER(d) = TER(d) + (ER(a, d) / KEINT(a))
        Next a

    End Sub
    ''' <summary>
    ''' Calculates the allocation of energy and N
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateAllocation(ByVal nTimestep As Integer)

        Dim d As Integer = nTimestep   'day index - set using argument
        Dim a As Integer  'allocation stage index

        Dim previousDayWTBODY As Double
        Dim previousDayWTPRO As Double
        Dim previousDayWTFAT As Double

        If d = Me.MinimumTimestep Then
            previousDayWTBODY = WTBODY(d)
            previousDayWTPRO = WTPRO(d)
            previousDayWTFAT = WTFAT(d)
        Else
            previousDayWTBODY = WTBODY(d - 1)
            previousDayWTPRO = WTPRO(d - 1)
            previousDayWTFAT = WTFAT(d - 1)
        End If

        Dim SumEDFAT As Double = 0.0
        Dim SumEDPRO As Double = 0.0
        Dim SumEUFAT As Double = 0.0
        Dim SumEUPRO As Double = 0.0
        Dim SumNDPRO As Double = 0.0
        Dim SumNUPRO As Double = 0.0

        'First pass over allocation stages to calculate availability and use based on initial requirements
        For a = Me.m_MinStageIndex To Me.m_MaxStageIndex

            'Determine the availability and use for this stage
            CalculateAllocationStage(a, d)

            'Sum fat and protein deposited/used
            SumEDFAT = SumEDFAT + EDFAT(a, d)
            SumEDPRO = SumEDPRO + EDPRO(a, d)
            SumEUFAT = SumEUFAT + EUFAT(a, d)
            SumEUPRO = SumEUPRO + EUPRO(a, d)
            SumNDPRO = SumNDPRO + NDPRO(a, d)
            SumNUPRO = SumNUPRO + NUPRO(a, d)
        Next a

        'Check to see if energy used to deposit fat is less than user-supplied target
        Dim EUINTFatProtein As Double
        Dim PropEUINTFat As Double
        Dim TarERFat As Double

        EUINTFatProtein = EUINT(Stage.Fat, d) + EUINT(Stage.Protein, d)

        If EUINTFatProtein > 0.0 Then
            PropEUINTFat = EUINT(Stage.Fat, d) / EUINTFatProtein
        Else
            PropEUINTFat = 0.0
        End If

        If PropEUINTFat < PFATDEP And EUINTFatProtein > 0.0 Then
            'Try to increase the energy used for fat
            'Set target for ER for fat deposition based on user-set target
            TarERFat = PFATDEP * EUINTFatProtein * KEINT(Stage.Fat)
            If TarERFat > ER(Stage.Fat, d) Then
                'Previous E requirement for fat is an upper limit due to mobilization rate
                TarERFat = ER(Stage.Fat, d)
            End If
            ER(Stage.Fat, d) = TarERFat
            ER(Stage.Protein, d) = (EUINTFatProtein - (TarERFat / KEINT(Stage.Fat))) * KEINT(Stage.Protein)
            If ECONPRO > 0.0 Then
                NR(Stage.Protein, d) = ER(Stage.Protein, d) * (NCONPRO / ECONPRO)
            Else
                NR(Stage.Protein, d) = 0.0
            End If
        End If

        'Second pass over allocation with adjusted fat and protein deposition requirements
        SumEDFAT = 0.0
        SumEUFAT = 0.0
        SumEUPRO = 0.0
        SumNDPRO = 0.0
        SumNUPRO = 0.0
        For a = Me.m_MinStageIndex To Me.m_MaxStageIndex

            'Determine the availability and use for this stage
            CalculateAllocationStage(a, d)

            'Sum fat and protein deposited/used
            SumEDFAT = SumEDFAT + EDFAT(a, d)
            SumEDPRO = SumEDPRO + EDPRO(a, d)
            SumEUFAT = SumEUFAT + EUFAT(a, d)
            SumEUPRO = SumEUPRO + EUPRO(a, d)
            SumNDPRO = SumNDPRO + NDPRO(a, d)
            SumNUPRO = SumNUPRO + NUPRO(a, d)
        Next a

        'Energy and N Balance (as indicators)
        EB(d) = MEI(d) - (ER(Stage.Base, d) / KEINT(Stage.Base)) - (ER(Stage.Activity, d) / KEINT(Stage.Activity)) _
                - (ER(Stage.Gestation, d) / KEINT(Stage.Gestation)) - (ER(Stage.Lactation, d) / KEINT(Stage.Lactation))
        NB(d) = SumNDPRO - SumNUPRO

        '---------------------------------------------
        'WEIGHT CHANGE
        '---------------------------------------------

        'Fat weight
        EFATCHG(d) = SumEDFAT - SumEUFAT
        If ECONFAT = 0.0 Then
            WTFATCHG(d) = 0.0
        Else
            WTFATCHG(d) = EFATCHG(d) / (ECONFAT * 1000.0)
        End If
        WTFAT(d) = previousDayWTFAT + WTFATCHG(d)
        If WTFAT(d) < 0.0 Then WTFAT(d) = 0.0

        'Protein weight
        NPROCHG(d) = SumNDPRO - SumNUPRO
        If NCONPRO = 0.0 Then
            WTPROCHG(d) = 0.0
        Else
            WTPROCHG(d) = NPROCHG(d) / (NCONPRO * 1000.0)
        End If
        WTPRO(d) = previousDayWTPRO + WTPROCHG(d)
        If WTPRO(d) < 0.0 Then WTPRO(d) = 0.0

    End Sub

    ''' <summary>
    ''' Calculates the allocation for a stage
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateAllocationStage(ByVal allocationStage As Integer, ByVal nTimestep As Integer)

        Dim d As Integer = nTimestep   'day index - set using argument
        Dim a As Integer = allocationStage  'allocation stage index

        Dim previousDayWTBODY As Double
        Dim previousDayWTPRO As Double
        Dim previousDayWTFAT As Double

        If d = Me.MinimumTimestep Then
            previousDayWTBODY = WTBODY(d)
            previousDayWTPRO = WTPRO(d)
            previousDayWTFAT = WTFAT(d)
        Else
            previousDayWTBODY = WTBODY(d - 1)
            previousDayWTPRO = WTPRO(d - 1)
            previousDayWTFAT = WTFAT(d - 1)
        End If

        Dim EofNUPRO As Double
        Dim EofNDPRO As Double
        Dim NofEUPRO As Double
        Dim NofEDPRO As Double

        '---------------------------------------------
        'NITROGEN AVAILABLE
        '---------------------------------------------

        'Nitrogen available from previously mobilized protein reserves 
        If a = Me.m_MinStageIndex Or ECONPRO = 0.0 Then
            NAPRORE(a, d) = 0.0
        Else
            NAPRORE(a, d) = NAPRORE(a - 1, d) - NUPRORE(a - 1, d)
        End If

        'Nitrogen available from intake
        If a = Me.m_MinStageIndex Then
            NAINT(a, d) = MNI(d)
        Else
            NAINT(a, d) = NAINT(a - 1, d) - NUINT(a - 1, d)
        End If

        'Nitrogen available from protein reserves
        WTPROMIN(d) = PPROMIN * WTBODYIN
        If previousDayWTPRO < WTPROMIN(d) Then WTPROMIN(d) = previousDayWTPRO
        PMMAX(d) = 1000.0 * (previousDayWTPRO - WTPROMIN(d))
        If PMMAX(d) > PROMOBRT Then
            PMMAX(d) = PROMOBRT
        End If
        If a = Me.m_MinStageIndex Or ECONPRO = 0.0 Then
            NAPRO(a, d) = PMMAX(d) * NCONPRO
        Else
            NAPRO(a, d) = NAPRO(a - 1, d) - NUPRO(a - 1, d)
        End If

        '---------------------------------------------
        'ENERGY AVAILABLE
        '---------------------------------------------

        'Efficiency with which MEI can be used for base metabolism
        If (FI(d) * ECONFI) = 0.0 Then
            QM(d) = 0.0
        Else
            QM(d) = MEI(d) / (FI(d) * ECONFI)
        End If
        'Todo: make KEINT a function of QM
        'KEINT(Stage.Base) = (0.35 * QM(d)) + 0.503

        'Energy available from previously mobilized protein reserves
        If NCONPRO = 0.0 Then
            EAPRORE(a, d) = 0.0
        ElseIf a = Me.m_MinStageIndex Then
            'EAPRORE(a, d) = (NUPRO(a, d) * (ECONPRO / NCONPRO))
            EAPRORE(a, d) = 0.0
        Else
            EAPRORE(a, d) = EAPRORE(a - 1, d) - EUPRORE(a - 1, d)
        End If

        'Energy available from intake
        If a = Me.m_MinStageIndex Then
            EAINT(a, d) = MEI(d)
        Else
            EAINT(a, d) = EAINT(a - 1, d) - EUINT(a - 1, d)
        End If

        'Energy available from fat reserves
        WTFATMIN(d) = PFATMIN * previousDayWTBODY
        If previousDayWTFAT < WTFATMIN(d) Then WTFATMIN(d) = previousDayWTFAT
        FMMAX(d) = (1000.0 * (previousDayWTFAT - WTFATMIN(d)))
        If FMMAX(d) > FATMOBRT Then
            FMMAX(d) = FATMOBRT
        End If
        If a = Me.m_MinStageIndex Then
            EAFAT(a, d) = FMMAX(d) * ECONFAT
        Else
            EAFAT(a, d) = EAFAT(a - 1, d) - EUFAT(a - 1, d)
        End If

        'Energy available from protein reserves
        If NCONPRO = 0.0 Then
            EAPRO(a, d) = 0.0
        ElseIf a = Me.m_MinStageIndex Then
            EAPRO(a, d) = PMMAX(d) * ECONPRO
        Else
            EAPRO(a, d) = EAPRO(a - 1, d) - EUPRO(a - 1, d)
        End If

        '---------------------------------------------
        'ENERGY & NITROGEN USED/DEPOSITED
        '---------------------------------------------
        'First pass at calculating E & N used with full requirements for this stage
        CalculateAllocationStageUsed(a, d, ER, NR)

        'Calculate the portion of the target requirement met on first pass
        NRMET(a, d) = (NUPRORE(a, d) * KNPRO(a)) + (NUINT(a, d) * KNINT(a)) + (NUPRO(a, d) * KNPRO(a))
        ERMET(a, d) = (EUPRORE(a, d) * KEPRO(a)) + (EUINT(a, d) * KEINT(a)) + (EUPRO(a, d) * KEPRO(a)) + (EUFAT(a, d) * KEFAT(a))

        If (NR(a, d) = 0.0) And ER(a, d) = 0.0 Then
            'No target set for either N or E
            PERTAR(a, d) = 1.0
            PNRTAR(a, d) = 1.0
            PRTAR(a, d) = 1.0
        ElseIf (NR(a, d) = 0.0) And ER(a, d) <> 0.0 Then
            'Target only set for E
            PERTAR(a, d) = Round(ERMET(a, d) / ER(a, d), precision)
            PNRTAR(a, d) = 1.0
            PRTAR(a, d) = PERTAR(a, d)
        ElseIf (ER(a, d) = 0.0) And NR(a, d) <> 0.0 Then
            'Target only set for N
            PERTAR(a, d) = 1.0
            PNRTAR(a, d) = Round(NRMET(a, d) / NR(a, d), precision)
            PRTAR(a, d) = PNRTAR(a, d)
        ElseIf (NRMET(a, d) / NR(a, d)) > (ERMET(a, d) / ER(a, d)) Then
            'Energy is most limiting, so calculate proportion of target that was met based on energy
            PERTAR(a, d) = Round(ERMET(a, d) / ER(a, d), precision)
            PNRTAR(a, d) = Round(NRMET(a, d) / NR(a, d), precision)
            PRTAR(a, d) = PERTAR(a, d)
        Else
            'N is most limiting (or both are equally limiting), so calculate proportion of target that was met based on N
            PERTAR(a, d) = Round(ERMET(a, d) / ER(a, d), precision)
            PNRTAR(a, d) = Round(NRMET(a, d) / NR(a, d), precision)
            PRTAR(a, d) = PNRTAR(a, d)
        End If

        'Scale back the final requirements for both E and N to create constrained requirements
        ERTAR(a, d) = PRTAR(a, d) * ER(a, d)
        NRTAR(a, d) = PRTAR(a, d) * NR(a, d)

        If (PRTAR(a, d) < 1.0) Then
            'Either N or E is limiting
            'Recalculate the E & N used based on the revised targets
            CalculateAllocationStageUsed(a, d, ERTAR, NRTAR)
        End If

        'Reconcile E&N when body protein reserves change
        If NCONPRO > 0.0 Then
            'Calculate E and N equivalents
            EofNUPRO = NUPRO(a, d) * (ECONPRO / NCONPRO)
            NofEUPRO = EUPRO(a, d) * (NCONPRO / ECONPRO)
            EofNDPRO = NDPRO(a, d) * (ECONPRO / NCONPRO)
            NofEDPRO = EDPRO(a, d) * (NCONPRO / ECONPRO)
        End If
        If (EofNUPRO > EUPRO(a, d)) Then
            'More N used than E so excess E released and E used adjusted upwards
            EAPRORE(a, d) = EAPRORE(a, d) + EofNUPRO - EUPRO(a, d)
            EUPRO(a, d) = EofNUPRO
        Else
            'More E used than N so excess N released and N used adjusted upwards
            NAPRORE(a, d) = NAPRORE(a, d) + NofEUPRO - NUPRO(a, d)
            NUPRO(a, d) = NofEUPRO
        End If
        If (EofNDPRO > EDPRO(a, d)) Then
            'More N deposited than E so additional E is used
            EDPRO(a, d) = EofNDPRO
        Else
            'More E deposited than N so additional N is used
            NDPRO(a, d) = NofEDPRO
        End If
    End Sub


    ''' <summary>
    ''' Calculates the energy and N used/deposited for an allocation stage
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateAllocationStageUsed(ByVal allocationStage As Integer, ByVal nTimestep As Integer, _
                ByVal energyRequirement As MVStageTime, ByVal nitrogenRequirement As MVStageTime)

        Dim d As Integer = nTimestep   'day index - set using argument
        Dim a As Integer = allocationStage  'allocation stage index

        '---------------------------------------------
        'NITROGEN USED/DEPOSITED
        '---------------------------------------------

        'Nitrogen used from previously mobilized protein reserves
        If depositsProtein(a) Or (KNPRO(a) = 0.0) Or (nitrogenRequirement(Stage.SummerProtein, d) > 0.001) Then
            NUPRORE(a, d) = 0.0
        Else
            NUPRORE(a, d) = (nitrogenRequirement(a, d) / KNPRO(a))
        End If
        If (NUPRORE(a, d) > NAPRORE(a, d)) Then
            NUPRORE(a, d) = NAPRORE(a, d)
        End If
        NLUPRORE(a, d) = NUPRORE(a, d) * (1 - KNPRO(a))

        'Nitrogen used from intake
        NRINT(a, d) = Round(nitrogenRequirement(a, d) - (NUPRORE(a, d) * KNPRO(a)), precision)

        If KNINT(a) = 0.0 Then
            NUINT(a, d) = 0.0
        Else
            NUINT(a, d) = (NRINT(a, d) / KNINT(a))
        End If
        If (NUINT(a, d) > NAINT(a, d)) Then
            NUINT(a, d) = NAINT(a, d)
        End If
        NLUINT(a, d) = NUINT(a, d) * (1 - KNINT(a))

        'Nitrogen used from newly mobilized body protein reserves
        If (mobilizesProtein(a) And nitrogenRequirement(Stage.SummerProtein, d) < 0.001) Then
            NRPRO(a, d) = Round(NRINT(a, d) - (NUINT(a, d) * KNINT(a)), precision)
            If KNPRO(a) = 0.0 Then
                NUPRO(a, d) = 0.0
            Else
                NUPRO(a, d) = NRPRO(a, d) / KNPRO(a)
            End If
            If (NUPRO(a, d) > NAPRO(a, d)) Then
                NUPRO(a, d) = NAPRO(a, d)
            End If
            NLUPRO(a, d) = NUPRO(a, d) * (1 - KNPRO(a))
        Else
            NRPRO(a, d) = 0.0
            NUPRO(a, d) = 0.0
            NLUPRO(a, d) = 0.0
        End If

        'Nitrogen deposited to body protein reserves
        If (depositsProtein(a)) Then
            NDPRO(a, d) = (NUINT(a, d) * KNINT(a))
            NLDPRO(a, d) = NUINT(a, d) * (1 - KNINT(a))
        Else
            NDPRO(a, d) = 0.0
            NLDPRO(a, d) = 0.0
        End If

        '---------------------------------------------
        'ENERGY USED/DEPOSITED
        '---------------------------------------------

        'Energy used from previously mobilized protein reserves
        If depositsProtein(a) Or (KNPRO(a) = 0.0) Or (nitrogenRequirement(Stage.SummerProtein, d) > 0.001) Then
            EUPRORE(a, d) = 0.0
        Else
            EUPRORE(a, d) = energyRequirement(a, d) / KEPRO(a)
        End If

        If EUPRORE(a, d) > EAPRORE(a, d) Then
            EUPRORE(a, d) = EAPRORE(a, d)
        End If
        HPUPRORE(a, d) = EUPRORE(a, d) * (1 - KEPRO(a))

        'Energy used from intake
        ERINT(a, d) = Round(energyRequirement(a, d) - (EUPRORE(a, d) * KEPRO(a)), precision)
        'If depositsProtein(a) Then
        '    If NCONPRO = 0.0 Then
        '        EUINT(a, d) = 0.0
        '    Else
        '        EUINT(a, d) = NUINT(a, d) * ECONPRO / NCONPRO
        '    End If
        'Else
        If KEINT(a) = 0.0 Then
            EUINT(a, d) = 0.0
        Else
            EUINT(a, d) = ERINT(a, d) / KEINT(a)
        End If
        If EUINT(a, d) > EAINT(a, d) Then
            EUINT(a, d) = EAINT(a, d)
        End If
        'End If
        HPUINT(a, d) = EUINT(a, d) * (1 - KEINT(a))

        'Energy used from mobilized body fat reserves
        If mobilizesFat(a) Then
            ERFAT(a, d) = Round(ERINT(a, d) - (EUINT(a, d) * KEINT(a)), precision)
            If KEFAT(a) = 0.0 Then
                EUFAT(a, d) = 0.0
            Else
                EUFAT(a, d) = ERFAT(a, d) / KEFAT(a)
            End If
            If EUFAT(a, d) > EAFAT(a, d) Then
                EUFAT(a, d) = EAFAT(a, d)
            End If
            HPUFAT(a, d) = EUFAT(a, d) * (1 - KEFAT(a))
        Else
            ERFAT(a, d) = 0.0
            EUFAT(a, d) = 0.0
            HPUFAT(a, d) = 0.0
        End If

        'Energy used from newly mobilized body protein reserves
        If (mobilizesProtein(a) And nitrogenRequirement(Stage.SummerProtein, d) < 0.001) Then
            ERPRO(a, d) = Round(ERFAT(a, d) - (EUFAT(a, d) * KEFAT(a)), precision)
            If KEPRO(a) = 0.0 Then
                EUPRO(a, d) = 0.0
            Else
                EUPRO(a, d) = ERPRO(a, d) / KEPRO(a)
            End If
            If EUPRO(a, d) > EAPRO(a, d) Then
                EUPRO(a, d) = EAPRO(a, d)
            End If
            HPUPRO(a, d) = EUPRO(a, d) * (1 - KEPRO(a))
        Else
            ERPRO(a, d) = 0.0
            EUPRO(a, d) = 0.0
            HPUPRO(a, d) = 0.0
        End If

        'Energy deposited to body fat reserves
        If depositsFat(a) Then
            EDFAT(a, d) = (EUPRORE(a, d) * KEPRO(a)) + (EUINT(a, d) * KEINT(a))
            HPDFAT(a, d) = (EUPRORE(a, d) * (1 - KEPRO(a))) + (EUINT(a, d) * (1 - KEINT(a)))
        Else
            EDFAT(a, d) = 0.0
            HPDFAT(a, d) = 0.0
        End If

        'Energy deposited to body protein reserves (as an indicator only)
        If depositsProtein(a) Then
            EDPRO(a, d) = EUINT(a, d) * KEINT(a)
            HPDPRO(a, d) = EUINT(a, d) * (1 - KEINT(a))
        Else
            EDPRO(a, d) = 0.0
            HPDPRO(a, d) = 0.0
        End If

    End Sub

    ''' <summary>
    ''' Calculates additional indicators at the end of each Timestep
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateAllocationIndicators(ByVal nTimestep As Integer)

        Dim d As Integer = nTimestep   'day index - set using argument
        Dim a As Integer   ' allocation stage index
        Dim p As Integer   ' plant type index

        Dim sumNPNURN As Double
        Dim previousDayWTWAT As Double
        Dim previousDayWTCALF As Double
        Dim previousDayWTFET As Double

        If d = Me.MinimumTimestep Then
            previousDayWTWAT = PWATIN * WTBODYIN
            'Todo: Allow user to set initial weight of calf for multi-year runs
            previousDayWTCALF = 0.0
            previousDayWTFET = WTFET(d)
        Else
            'Set previous day's values
            previousDayWTWAT = WTWAT(d - 1)
            previousDayWTCALF = WTCALF(d - 1)
            previousDayWTFET = WTFET(d - 1)
        End If

        'Net nitrogen requirement for maintenance
        NRMA(d) = NR(Stage.Base, d) + NR(Stage.Coat, d) + NR(Stage.Scurf, d) + NR(Stage.Parasites, d)

        'Net energy requirement for maintenance
        ERMA(d) = ER(Stage.Base, d) + ER(Stage.Coat, d) + ER(Stage.Scurf, d) + ER(Stage.Parasites, d)

        'Net energy requirement for a non-productive animal
        ERNP(d) = ERMA(d) + ER(Stage.Activity, d)

        'Muscle Weight
        If PMUSPRO > 0.0 Then
            WTMUS(d) = WTPRO(d) / PMUSPRO
        Else
            WTMUS(d) = 0.0
        End If

        'Water Weight
        'Todo: fix water weight calculation
        WTWAT(d) = 0.0
        'If (PREG(d) <> 0.0) Then
        'If Not ((d > ENDAYGES) And (d < STDAYGES)) And STDAYGES <> 0 Then
        '    'Pregnant
        '    'Water weight replacing mobilized fat
        '    WTWATFAT(d) = -WTFATCHG(d) * PFATWAT
        '    If WTWATFAT(d) < 0.0 Then WTWATFAT(d) = 0.0

        '    'Water weight replacing mobilized muscle
        '    If PMUSPRO > 0.0 Then
        '        WTWATMUS(d) = -WTPROCHG(d) * PMUSWAT / PMUSPRO
        '    Else
        '        WTWATMUS(d) = 0.0
        '    End If
        '    If WTWATMUS(d) < 0.0 Then WTWATMUS(d) = 0.0

        '    'New water weight
        '    WTWAT(d) = previousDayWTWAT + WTWATFAT(d) + WTWATMUS(d)
        '    If (WTWAT(d) < (PWATMAX * WTBODY(d))) Then WTWAT(d) = (PWATMAX * WTBODY(d))

        'ElseIf (d > ENDAYGES) And (d < STDAYGES) And (WTFATCHG(d) > 0.0 Or WTPROCHG(d) > 0.0) Then
        '    'Not pregnant and depositing body reserves
        '    WTWAT(d) = 0.0
        'Else
        '    'Not pregnant and not depositing body reserves
        '    WTWAT(d) = previousDayWTWAT
        'End If

        'Rumen contents as wet weight
        If PRUMDRY <= 0.0 Then
            WTRUMWET(d) = WTRUM(d)
        Else
            WTRUMWET(d) = WTRUM(d) / (PRUMDRY * 1000.0)
        End If

        'Gut Content Weight
        If PRUMGUT > 0.0 Then
            WTGUT(d) = WTRUMWET(d) / PRUMGUT
        Else
            WTGUT(d) = 0.0
        End If

        'Calf Weight
        If LACT(d) = 0.0 Then
            TGRCALF(d) = 0.0
            TWTCALF(d) = 0.0
            WTCALF(d) = 0.0
        Else
            If DAYSLAC(d) = 1 Then
                previousDayWTCALF = previousDayWTFET
            End If
            If DAYSLAC(d) <= 21.0 Then
                TGRCALF(d) = ((TMLKPR(d) * 1000.0) - 653.0) / (2.79 * 1000.0)
            ElseIf DAYSLAC(d) <= 42.0 Then
                TGRCALF(d) = TMLKPR(d) / 3.13
            Else
                TGRCALF(d) = TMLKPR(d) / 2.0
            End If

            TWTCALF(d) = previousDayWTCALF + TGRCALF(d)
            WTCALF(d) = previousDayWTCALF + (PRTAR(Stage.Lactation, d) * TGRCALF(d))

        End If

        'Fetus and Conceptus Weight
        If PREG(d) = 0.0 Then
            WTCON(d) = 0.0
            WTFET(d) = 0.0
        Else
            WTFET(d) = previousDayWTFET + (PRTAR(Stage.Gestation, d) * TGRFET(d))
            If PCONFET <> 0.0 Then
                WTCON(d) = WTFET(d) / PCONFET
            End If
        End If

        'Fat-free ingesta-free Body Weight
        WTFFIF(d) = (4.184 * WTPRO(d)) + 0.343

        'Body weight
        WTBODY(d) = WTFFIF(d) + WTFAT(d) + WTGUT(d) + WTWAT(d) + WTCON(d)

        'Total the forage intake per unit metabolic weight
        For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
            FIMET(p, d) = FIP(p, d) / (WTBODY(d) ^ 0.75)
        Next p

        'Empty body weight, maternal weight
        WTEMPTY(d) = WTFFIF(d) + WTFAT(d) + WTWAT(d)
        WTMAT(d) = WTBODY(d) - WTWAT(d) - WTCON(d)

        'Propn fat and protein weight
        PFAT(d) = WTFAT(d) / WTBODY(d)
        PPRO(d) = WTPRO(d) / WTBODY(d)

        'Rumen capacity
        RCAP(d) = (WTEMPTY(d) - WTWAT(d)) * PRCAP * 1000.0

        'Endogenous urinary nitrogen loss
        EUN(d) = NEUNCO * WTBODY(d) ^ 0.75

        UNL(d) = 0.0
        For a = Me.m_MinStageIndex To Me.m_MaxStageIndex

            'Total energy & nitrogen available and used
            EA(a, d) = EAINT(a, d) + EAFAT(a, d) + EAPRO(a, d) + EAPRORE(a, d)
            NA(a, d) = NAINT(a, d) + NAPRO(a, d) + NAPRORE(a, d)
            EU(a, d) = EUINT(a, d) + EUFAT(a, d) + EUPRO(a, d) + EUPRORE(a, d)
            NU(a, d) = NUINT(a, d) + NUPRO(a, d) + NUPRORE(a, d)

            'Heat production
            HP(a, d) = HPUINT(a, d) + HPUFAT(a, d) + HPDFAT(a, d) + HPUPRO(a, d) + HPUPRORE(a, d) + HPDPRO(a, d)

            'Metabolic urinary nitrogen loss
            MNL(a, d) = NLUINT(a, d) + NLUPRO(a, d) + NLUPRORE(a, d) + NLDPRO(a, d)

            'Accumulate metabolic urinary nitrogen loss
            UNL(d) = UNL(d) + MNL(a, d)
        Next a

        'Urinary Non protein N
        sumNPNURN = 0.0
        For p = Me.m_MinPlantIndex To Me.m_MaxPlantIndex
            sumNPNURN = sumNPNURN + NPNURN(p, d)
        Next

        'Total urinary N loss
        UNL(d) = UNL(d) + EUN(d) + sumNPNURN

        'Metabolic fecal nitrogen loss
        MFN(d) = MFNCO * FEC(d) / 1000.0

        'Todo: add MFN to NFEC
        'Todo: add calf weight as an indicator

    End Sub

    Private Function CalculateTargetFetusWeight(ByVal nTimestep As Integer) As Double

        Dim d As Integer = nTimestep   'day index - set using argument
        Dim dg As Double

        If CInt(PREG(d)) <> 0 Then
            'Target fetus weight
            dg = DAYSGES(d)
            'MAXWTFET(d) = 0.04597 * Exp(0.0222 * DAYSGES(d))

            If dg <= 76 Then
                MAXWTFET(d) = (0.00036 * dg ^ 3) + (0.053 * dg ^ 2) - (1.58 * dg) - 0.000096
            Else
                MAXWTFET(d) = (0.0000000605254 * dg ^ 4) - (0.0000306828 * dg ^ 3) + (0.05719 * dg ^ 2) - (0.44743 * dg) + 64.152
            End If
            If MAXWTFET(d) < 1.075 Then MAXWTFET(d) = 1.075

            CalculateTargetFetusWeight = (MAXWTFET(d) / 1000.0) * (WTTBIR / 2.9)
        Else
            CalculateTargetFetusWeight = 0.0
        End If

    End Function

    ''' <summary>
    ''' Calculates the pregnancy and lactation status of the animal
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculatePregnancyLactation(ByVal nTimestep As Integer)

        Dim d As Integer = nTimestep   'day index - set using argument

        'Determine whether this is a conception day
        Dim bConception As Boolean = False
        If JulianDayFromTimestep(d) = STDAYGES Then
            If PREGLAC_USERPREG Then
                If PREGYR <> 0 Then
                    bConception = True
                End If
            Else
                If WTBODY(d - 1) >= PREGLAC_THPREGWT Then
                    bConception = True
                End If
            End If
        End If

        'Determine whehter the animal is already pregnant
        Dim bPregnant As Boolean = False
        If d = Me.MinimumTimestep Then
            If PREGIN <> 0 Then
                bPregnant = True
                CALFFATEOUT(d) = CalfFate.InUtero
            End If
        Else
            If PREG(d - 1) <> 0 Then
                bPregnant = True
                CALFFATEOUT(d) = CalfFate.InUtero
            End If
        End If

        If (bPregnant And (JulianDayFromTimestep(d) <= ENDAYGES Or JulianDayFromTimestep(d) >= STDAYGES)) Or (bConception) Then
            PREG(d) = 1.0
            CALFFATEOUT(d) = CalfFate.InUtero
        Else
            PREG(d) = 0.0
            If (bConception) Then
                CALFFATEOUT(d) = CalfFate.NoCalf
            End If
        End If

        'Determine whether the animal is already or about to lactate
        Dim bLact As Boolean = False
        If d = Me.MinimumTimestep Then
            If LACTIN <> 0 Then
                bLact = True
                CALFFATEOUT(d) = CalfFate.Lactating
            End If
        Else
            If LACT(d - 1) <> 0 Then
                bLact = True
                CALFFATEOUT(d) = CalfFate.Lactating
            Else
                If (PREG(d - 1) <> 0) And (PREG(d) = 0) Then
                    bLact = True
                    CALFFATEOUT(d) = CalfFate.Lactating
                End If
            End If
        End If

        'Determine lactation status.
        If PREGLAC_USERWEAN Then
            If bLact Then
                If JulianDayFromTimestep(d) < ENDAYLAC Then
                    LACT(d) = 1.0
                Else
                    LACT(d) = 0.0
                End If
            Else
                LACT(d) = 0.0
            End If
        Else
            If bLact Then
                If DetermineWeaning(d) Then
                    LACT(d) = 0.0
                Else
                    LACT(d) = 1.0
                End If
            Else
                LACT(d) = 0.0
            End If
        End If

        'Days of Gestation
        If CInt(PREG(d)) <> 0 Then
            DAYSGES(d) = CDbl(JulianDayFromTimestep(d) - STDAYGES + 1)
            If DAYSGES(d) < 0.0 Then DAYSGES(d) = DAYSGES(d) + 365.0
        Else
            DAYSGES(d) = 0.0
        End If

        'Days of Lactation
        If CInt(LACT(d)) <> 0 Then
            DAYSLAC(d) = CDbl(JulianDayFromTimestep(d) - STDAYLAC)
            If DAYSLAC(d) < 0.0 Then DAYSLAC(d) = DAYSLAC(d) + 365.0
        Else
            DAYSLAC(d) = 0.0
        End If

    End Sub

    Private Function DetermineWeaning(ByVal day As Integer) As Boolean

        If PREGLAC_USERWEAN Then
            Return False
        End If

        Dim bWean As Boolean = False

        'Weaning during the post natal period if weight gain is too low
        If (DAYSLAC(day - 1) + 1) = PREGLAC_POWEAND Then
            Dim startDay As Integer = day - (PREGLAC_POWEAND - PREGLAC_PSTD)
            Dim endDay As Integer = day - (PREGLAC_POWEAND - PREGLAC_PEND)
            Dim AVGCALFWTGN As Double = 1000 * ((WTCALF(endDay) - WTCALF(startDay)) / (endDay - startDay))
            If AVGCALFWTGN < PREGLAC_THPCWTGN Then

                CALFFATEOUT(day) = CalfFate.PostNatalWeaner
                Return True

            End If
        End If

        'Weaning during the summer period because cow protein gain is too low
        If (DAYSLAC(day - 1) + 1) = PREGLAC_SWEAND Then
            Dim startDay As Integer = day - (PREGLAC_SWEAND - PREGLAC_SSTD)
            Dim endDay As Integer = day - (PREGLAC_SWEAND - PREGLAC_SEND)
            Dim ACTSUMPRTGN As Double = 1000 * ((WTPRO(endDay) - WTPRO(startDay)) / (endDay - startDay))
            If ACTSUMPRTGN < PREGLAC_THSPRTGN Then

                CALFFATEOUT(day) = CalfFate.SummerWeaner
                Return True

            End If
        End If

        'Early fall weaning
        If (DAYSLAC(day - 1) + 1) = PREGLAC_EAWEAND Then
            If WTCALF(day - 1) < PREGLAC_THFMNCWT Then

                CALFFATEOUT(day) = CalfFate.EarlyWeaner
                Return True

            End If
        End If

        'Normal fall weaning
        If (DAYSLAC(day - 1) + 1) = PREGLAC_NOWEAND Then
            'If WTCALF(day - 1) > PREGLAC_THFMXCWT Then
            CALFFATEOUT(day) = CalfFate.NormalWeaner
            Return True
            'End If
        End If

        'Extended weaning
        'DEVTODO: Still need to look into the implementation of extended weaning which below was not working properly
        'If (DAYSLAC(day - 1) + 1) > PREGLAC_NOWEAND Then
        '    'If Cow is pregnant
        '    If PREG(day) <> 0.0 Then
        '        If JulianDayFromTimestep(day) = ENDAYGES + PREGLAC_EPWEAND Then
        '            Return True
        '        End If
        '    End If
        '    'If Cow is not pregnant
        '    If PREG(day) = 0.0 Then
        '        If JulianDayFromTimestep(day) = STDAYGES + PREGLAC_ENPWEAND Then
        '            Return True
        '        End If
        '    End If
        'End If


        Return bWean

    End Function

End Class
