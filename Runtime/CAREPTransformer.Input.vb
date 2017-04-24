'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports SyncroSim.Core

Partial Class CAREPTransformer

    Private dtDEF_PLANT As DataTable
    Private dtDEF_STRATUM As DataTable
    Private dtIN_ACT As DataTable
    Private dtIN_ACT_PEM As DataTable
    Private dtIN_ADF As DataTable
    Private dtIN_AR As DataTable
    Private dtIN_BSA As DataTable
    Private dtIN_DIET As DataTable
    Private dtIN_FB As DataTable
    Private dtIN_KP As DataTable
    Private dtIN_NDF As DataTable
    Private dtIN_OTHER As DataTable
    Private dtIN_PREGLAC As DataTable
    Private dtIN_PARA As DataTable
    Private dtIN_PCCIBY As DataTable
    Private dtIN_PCMAX As DataTable
    Private dtIN_PDEG As DataTable
    Private dtIN_PNIT As DataTable
    Private dtIN_PTARSUM As DataTable
    Private dtIN_RUMEN As DataTable
    Private dtIN_RUN As DataTable
    Private dtIN_SNODEP As DataTable
    Private dtIN_STAGE As DataTable
    Private dtIN_LOCATION As DataTable

    Private mobilizesFat() As Boolean
    Private mobilizesProtein() As Boolean
    Private depositsFat() As Boolean
    Private depositsProtein() As Boolean
    Private precision As Integer = 3 'max # of decimals for requirements

    ''' <summary>
    ''' Initializes the definitions and run control parameters for the simulation model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeDefinitions()

        If (dtDEF_STRATUM.Rows.Count = 0) Then
            Throw New ArgumentException("There are no strata defined.  Cannot continue.")
        End If

        If (dtDEF_PLANT.Rows.Count = 0) Then
            Throw New ArgumentException("There are no plant types defined.  Cannot continue.")
        End If

        'Declare any temporary model variables used to read in data from a source and then pass it on to an 
        'instance variable ready to be used in equations

        m_MinPlantIndex = 1
        m_MaxPlantIndex = dtDEF_PLANT.Rows.Count

        m_MinStageIndex = CInt(Stage.Base)
        m_MaxStageIndex = CInt(Stage.Fat)

        Debug.Assert(Stage.Base = 1 And Stage.Fat = 11)

        'Set the properties of each allocation stage
        ReDim mobilizesFat(m_MaxStageIndex)
        ReDim mobilizesProtein(m_MaxStageIndex)
        ReDim depositsFat(m_MaxStageIndex)
        ReDim depositsProtein(m_MaxStageIndex)

        For a = m_MinStageIndex To m_MaxStageIndex
            If (a = Stage.SummerProtein) Or (a = Stage.Protein) Then
                mobilizesFat(a) = False
                mobilizesProtein(a) = False
                depositsFat(a) = False
                depositsProtein(a) = True
            ElseIf (a = Stage.Fat) Then
                mobilizesFat(a) = False
                mobilizesProtein(a) = False
                depositsFat(a) = True
                depositsProtein(a) = False
            Else
                mobilizesFat(a) = True
                mobilizesProtein(a) = True
                depositsFat(a) = False
                depositsProtein(a) = False
            End If
        Next a

        'Determine default dimensions for each type of ModelVariable
        'For dimensions with lookup values, use the property to initialize
        'ToDo: name the property the same as the dimension (e.g. STRATUM)
        'Dim vdStratum As New MBVariableDimension(m_MinStratumID, m_MaxStratumID, StratumFieldName)
        'Dim vdPlant As New MBVariableDimension(m_MinPlantID, m_MaxPlantID, PlantFieldName)

        Dim vdDefPlant As New MBVariableDimension(Me.Project.GetDataSheet(DATAFEED_PLANT_NAME))
        vdDefPlant.Name = PlantPKFieldName
        vdDefPlant.DisplayName = PlantDisplayName

        Dim vdDefStratum As New MBVariableDimension(Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME))
        vdDefStratum.Name = StratumPKFieldName
        vdDefStratum.DisplayName = StratumDisplayName

        Dim vdPlant As New MBVariableDimension(Me.Project.GetDataSheet(DATAFEED_PLANT_NAME))
        vdPlant.Name = DATASHEET_PLANT_ID_COLUMN_NAME
        vdPlant.DisplayName = PlantDisplayName

        Dim vdStage As New MBVariableDimension(CreateStageValidationTable().DataSource, "Stage", "Stage")
        vdStage.Name = DATASHEET_STAGE_ID_COLUMN_NAME
        vdStage.DisplayName = StageDisplayName

        Dim vdStratum As New MBVariableDimension(Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME))
        vdStratum.Name = DATASHEET_STRATUM_ID_COLUMN_NAME
        vdStratum.DisplayName = StratumDisplayName

        Dim vdLact As New MBVariableDimension(CreateLactationStatusValidationTable().DataSource, "LactationStatus", "Lactation Status")
        vdLact.Name = "LactationStatus"
        vdLact.DisplayName = "Lactation Status"

        Dim vdHour As New MBVariableDimension(MinHour, MaxHour, HourFieldName)
        Dim vdTime As New MBVariableDimension(MinJDay, MaxJDay * m_EndYear, TimeStepFieldName)
        'Dimensions required for shadow source model variables used to read in time-related data
        Dim vdYear As New MBVariableDimension(m_StartYear, m_EndYear, YearFieldName)
        Dim vdJday As New MBVariableDimension(MinJDay, MaxJDay, JDayFieldName)

        'Create arrays of dimensions for each type of ModelVariable
        Dim vdMVHour() As MBVariableDimension = {vdHour}
        Dim vdMVDefPlant() As MBVariableDimension = {vdDefPlant}
        Dim vdMVPlant() As MBVariableDimension = {vdPlant}
        Dim vdMVPLantHour() As MBVariableDimension = {vdPlant, vdHour}
        Dim vdMVPLantTime() As MBVariableDimension = {vdPlant, vdTime}
        Dim vdMVStage() As MBVariableDimension = {vdStage}
        Dim vdMVStageTime() As MBVariableDimension = {vdStage, vdTime}
        Dim vdMVStratumPlant() As MBVariableDimension = {vdStratum, vdPlant}
        Dim vdMVStratumTime() As MBVariableDimension = {vdStratum, vdTime}
        Dim vdMVStratumPlantTime() As MBVariableDimension = {vdStratum, vdPlant, vdTime}
        Dim vdMVStratumLactTime() As MBVariableDimension = {vdStratum, vdLact, vdTime}
        Dim vdMVTime() As MBVariableDimension = {vdTime}
        'Set default dimensions for shadow source ModelVariables using Year/JDay
        Dim vdSourceMVTime() As MBVariableDimension = {vdYear, vdJday}
        Dim vdSourceMVPLantTime() As MBVariableDimension = {vdPlant, vdYear, vdJday}
        Dim vdSourceMVStageTime() As MBVariableDimension = {vdStage, vdYear, vdJday}
        Dim vdSourceMVStratumTime() As MBVariableDimension = {vdStratum, vdYear, vdJday}
        Dim vdSourceMVStratumPlantTime() As MBVariableDimension = {vdStratum, vdPlant, vdYear, vdJday}
        Dim vdSourceMVStratumLactTime() As MBVariableDimension = {vdStratum, vdLact, vdYear, vdJday}

        'Assign default dimensions for each ModelVariable type
        MVHour.DefaultDimensions = vdMVHour
        MVPlant.DefaultDimensions = vdMVPlant
        MVPlantHour.DefaultDimensions = vdMVPLantHour
        MVPlantTime.DefaultDimensions = vdMVPLantTime
        MVStage.DefaultDimensions = vdMVStage
        MVStageTime.DefaultDimensions = vdMVStageTime
        MVStratumTime.DefaultDimensions = vdMVStratumTime
        MVStratumPlant.DefaultDimensions = vdMVStratumPlant
        MVStratumPlantTime.DefaultDimensions = vdMVStratumPlantTime
        MVStratumLactTime.DefaultDimensions = vdMVStratumLactTime
        MVTime.DefaultDimensions = vdMVTime

        'Translators
        Dim trxplant As New IDTranslator(Me.Project.GetDataSheet(DATAFEED_PLANT_NAME))
        Dim trxstrat As New IDTranslator(Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME))
        Dim trxmult As New StratumMultiKeyHelper(Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME))

        MVHour.PlantIDTranslator = trxplant
        MVPlant.PlantIDTranslator = trxplant
        MVPlantHour.PlantIDTranslator = trxplant
        MVPlantTime.PlantIDTranslator = trxplant
        MVStage.PlantIDTranslator = trxplant
        MVStageTime.PlantIDTranslator = trxplant
        MVStratumTime.PlantIDTranslator = trxplant
        MVStratumPlant.PlantIDTranslator = trxplant
        MVStratumPlantTime.PlantIDTranslator = trxplant
        MVTime.PlantIDTranslator = trxplant

        MVHour.StratumIDTranslator = trxstrat
        MVPlant.StratumIDTranslator = trxstrat
        MVPlantHour.StratumIDTranslator = trxstrat
        MVPlantTime.StratumIDTranslator = trxstrat
        MVStage.StratumIDTranslator = trxstrat
        MVStageTime.StratumIDTranslator = trxstrat
        MVStratumTime.StratumIDTranslator = trxstrat
        MVStratumPlant.StratumIDTranslator = trxstrat
        MVStratumPlantTime.StratumIDTranslator = trxstrat
        MVStratumLactTime.StratumIDTranslator = trxstrat
        MVTime.StratumIDTranslator = trxstrat

        MVStratumTime.StratumMultiKeyHelper = trxmult
        MVStratumPlant.StratumMultiKeyHelper = trxmult
        MVStratumPlantTime.StratumMultiKeyHelper = trxmult
        MVStratumLactTime.StratumMultiKeyHelper = trxmult

        'Assign default dimensions for the shadow source ModelVariables using Year/Jday
        MVTime.DefaultDimensionsSource = vdSourceMVTime
        MVPlantTime.DefaultDimensionsSource = vdSourceMVPLantTime
        MVStageTime.DefaultDimensionsSource = vdSourceMVStageTime
        MVStratumTime.DefaultDimensionsSource = vdSourceMVStratumTime
        MVStratumPlantTime.DefaultDimensionsSource = vdSourceMVStratumPlantTime
        MVStratumLactTime.DefaultDimensionsSource = vdSourceMVStratumLactTime

        'Set Start Year and Start Julian Day for each type of time-dependent ModelVariable
        MVTime.StartYear = m_StartYear
        MVPlantTime.StartYear = m_StartYear
        MVStageTime.StartYear = m_StartYear
        MVStratumTime.StartYear = m_StartYear
        MVStratumPlantTime.StartYear = m_StartYear
        MVStratumLactTime.StartYear = m_StartYear

        MVTime.EndYear = m_EndYear
        MVPlantTime.EndYear = m_EndYear
        MVStageTime.EndYear = m_EndYear
        MVStratumTime.EndYear = m_EndYear
        MVStratumPlantTime.EndYear = m_EndYear
        MVStratumLactTime.EndYear = m_EndYear

        MVTime.StartJday = m_StartJDay
        MVPlantTime.StartJday = m_StartJDay
        MVStageTime.StartJday = m_StartJDay
        MVStratumTime.StartJday = m_StartJDay
        MVStratumPlantTime.StartJday = m_StartJDay
        MVStratumLactTime.StartJday = m_StartJDay

    End Sub

    Private Sub InitializeInputTables()

        Using store As DataStore = Me.Library.CreateDataStore()

            dtDEF_PLANT = Me.Project.GetDataSheet(DATAFEED_PLANT_NAME).GetData(store)
            dtDEF_STRATUM = Me.Project.GetDataSheet(DATAFEED_STRATUM_NAME).GetData(store)
            dtIN_ACT = Me.ResultScenario.GetDataSheet(DATASHEET_ACT_NAME).GetData(store)
            dtIN_ACT_PEM = Me.ResultScenario.GetDataSheet(DATASHEET_ACT_PEM_NAME).GetData(store)
            dtIN_ADF = Me.ResultScenario.GetDataSheet(DATAFEED_ADF_NAME).GetData(store)
            dtIN_AR = Me.ResultScenario.GetDataSheet(DATAFEED_AR_NAME).GetData(store)
            dtIN_BSA = Me.ResultScenario.GetDataSheet(DATAFEED_BSA_NAME).GetData(store)
            dtIN_DIET = Me.ResultScenario.GetDataSheet(DATAFEED_DIET_NAME).GetData(store)
            dtIN_FB = Me.ResultScenario.GetDataSheet(DATAFEED_FB_NAME).GetData(store)
            dtIN_KP = Me.ResultScenario.GetDataSheet(DATAFEED_KP_NAME).GetData(store)
            dtIN_NDF = Me.ResultScenario.GetDataSheet(DATAFEED_NDF_NAME).GetData(store)
            dtIN_OTHER = Me.ResultScenario.GetDataSheet(DATAFEED_OTHER_NAME).GetData(store)
            dtIN_PREGLAC = Me.ResultScenario.GetDataSheet(DATAFEED_PREGLAC_NAME).GetData(store)
            dtIN_PARA = Me.ResultScenario.GetDataSheet(DATAFEED_PARA_NAME).GetData(store)
            dtIN_PCCIBY = Me.ResultScenario.GetDataSheet(DATAFEED_PCCIBY_NAME).GetData(store)
            dtIN_PCMAX = Me.ResultScenario.GetDataSheet(DATAFEED_PCMAX_NAME).GetData(store)
            dtIN_PDEG = Me.ResultScenario.GetDataSheet(DATAFEED_PDEG_NAME).GetData(store)
            dtIN_PNIT = Me.ResultScenario.GetDataSheet(DATAFEED_PNIT_NAME).GetData(store)
            dtIN_PTARSUM = Me.ResultScenario.GetDataSheet(DATAFEED_PTARSUM_NAME).GetData(store)
            dtIN_RUMEN = Me.ResultScenario.GetDataSheet(DATAFEED_RUMEN_NAME).GetData(store)
            dtIN_RUN = Me.ResultScenario.GetDataSheet(DATAFEED_RUN_CONTROL_NAME).GetData(store)
            dtIN_SNODEP = Me.ResultScenario.GetDataSheet(DATAFEED_SNOW_DEPTH_NAME).GetData(store)
            dtIN_LOCATION = Me.ResultScenario.GetDataSheet(DATAFEED_LOCATION_NAME).GetData(store)
            dtIN_STAGE = Me.ResultScenario.GetDataSheet(DATAFEED_STAGE_INPUT_NAME).GetData(store)

        End Using

    End Sub

End Class
