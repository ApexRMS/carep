'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module Strings

    'Error messages
    Public Const NO_RESULT_SCENARIOS_ERROR As String = "There must be at least one selected result scenario to create this report."

    'Common column names
    Public Const DATASHEET_SCENARIO_ID_COLUMN_NAME As String = "ScenarioID"
    Public Const DATASHEET_STRATUM_ID_COLUMN_NAME As String = "StratumID"
    Public Const DATASHEET_PLANT_ID_COLUMN_NAME As String = "PlantID"
    Public Const DATASHEET_STAGE_ID_COLUMN_NAME As String = "StageID"
    Public Const DATASHEET_ITERATION_COLUMN_NAME As String = "Iteration"
    Public Const DATASHEET_TIMESTEP_COLUMN_NAME As String = "Timestep"
    Public Const DATASHEET_JULIAN_DAY_COLUMN_NAME As String = "JulianDay"
    Public Const DATASHEET_HOUR_COLUMN_NAME As String = "Hour"

    'Stratum Data Feed
    Public Const DATAFEED_STRATUM_NAME As String = "CAREP_Stratum"

    'Plant Data Feed
    Public Const DATAFEED_PLANT_NAME As String = "CAREP_Plant"

    'Run Control Data Feed
    Public Const DATAFEED_RUN_CONTROL_NAME As String = "CAREP_RunControl"
    Public Const DATASHEET_RUN_CONTROL_START_YEAR_COLUMN_NAME As String = "StartYear"
    Public Const DATASHEET_RUN_CONTROL_START_JULIAN_DAY_COLUMN_NAME As String = "StartJulianDay"
    Public Const DATASHEET_RUN_CONTROL_END_YEAR_COLUMN_NAME As String = "EndYear"
    Public Const DATASHEET_RUN_CONTROL_END_JULIAN_DAY_COLUMN_NAME As String = "EndJulianDay"
    Public Const DATASHEET_RUN_CONTROL_NUM_ITERATIONS_COLUMN_NAME As String = "NumIterations"
    Public Const DATASHEET_RUN_CONTROL_OUTPUT_LEVEL_COLUMN_NAME As String = "OutputLevel"

    'Snow Depth Data Feed
    Public Const DATAFEED_SNOW_DEPTH_NAME As String = "CAREP_SNODEP"

    'ADF Data Feed
    Public Const DATAFEED_ADF_NAME As String = "CAREP_ADF"

    'BSA Data Feed
    Public Const DATAFEED_BSA_NAME As String = "CAREP_BSA"

    'NDF Data Feed
    Public Const DATAFEED_NDF_NAME As String = "CAREP_NDF"

    'PNIT Data Feed
    Public Const DATAFEED_PNIT_NAME As String = "CAREP_PNIT"

    'PDEG Data Feed
    Public Const DATAFEED_PDEG_NAME As String = "CAREP_PDEG"

    'FB Data Feed
    Public Const DATAFEED_FB_NAME As String = "CAREP_FB"

    'DIET Data Feed
    Public Const DATAFEED_DIET_NAME As String = "CAREP_DIET"

    'PCMAX Data Feed
    Public Const DATAFEED_PCMAX_NAME As String = "CAREP_PCMAX"

    'ACT Data Feed  (Activity Budget)
    'Activity Budget  / Proportion Eating Multiplier
    Public Const DATASHEET_ACT_PEM_NAME As String = "CAREP_ACT_PEM"
    Public Const DATASHEET_ACT_NAME As String = "CAREP_ACT"

    'RUMEN Data Feed
    Public Const DATAFEED_RUMEN_NAME As String = "CAREP_RUMEN"

    'KP Data Feed
    Public Const DATAFEED_KP_NAME As String = "CAREP_KP"

    'PCCIBY Data Feed
    Public Const DATAFEED_PCCIBY_NAME As String = "CAREP_PCCIBY"

    'AR Data Feed
    Public Const DATAFEED_AR_NAME As String = "CAREP_AR"

    'Stage Input Data Feed
    Public Const DATAFEED_STAGE_INPUT_NAME As String = "CAREP_StageInput"

    'PTARSUM Feed
    Public Const DATAFEED_PTARSUM_NAME As String = "CAREP_PTARSUM"

    'PARA Feed
    Public Const DATAFEED_PARA_NAME As String = "CAREP_PARA"

    'Location Input Data Feed
    Public Const DATAFEED_LOCATION_NAME As String = "CAREP_Location"

    'PREGLAC Data Feed
    Public Const DATAFEED_PREGLAC_NAME As String = "CAREP_PREGLAC"

    'Out Hour data feed
    Public Const DATAFEED_OUT_HOUR_NAME As String = "CAREP_OutHour"

    'Out Plant Hour data feed
    Public Const DATAFEED_OUT_PLANT_HOUR_NAME As String = "CAREP_OutPlantHour"

    'Out Plant Timestep
    Public Const DATAFEED_OUT_PLANT_TIMESTEP_NAME As String = "CAREP_OutPlantTimestep"

    'Out Plant Within Timestep
    Public Const DATAFEED_OUT_PLANT_WITHIN_TIMESTEP_NAME As String = "CAREP_OutPlantWithinTimestep"

    'Out Stage Timestep
    Public Const DATAFEED_OUT_STAGE_TIMESTEP_NAME As String = "CAREP_OutStageTimestep"

    'Out Stratum Plant Timestep
    Public Const DATAFEED_OUT_STRATUM_PLANT_TIMESTEP_NAME As String = "CAREP_OutStratumPlantTimestep"

    'Out Stratum Lact Timestep
    Public Const DATAFEED_OUT_STRATUM_LACT_TIMESTEP_NAME As String = "CAREP_OutStratumLactTimestep"

    'Out Stratum Timestep
    Public Const DATAFEED_OUT_STRATUM_TIMESTEP_NAME As String = "CAREP_OutStratumTimestep"

    'Out Timestep
    Public Const DATAFEED_OUT_TIMESTEP_NAME As String = "CAREP_OutTimestep"

    'OTHER Data Feed
    Public Const DATAFEED_OTHER_NAME As String = "CAREP_OTHER"
    Public Const IN_OTHER_AGE_COLUMN_NAME As String = "AGE"
    Public Const IN_OTHER_CALFFATE_COLUMN_NAME As String = "CALFFATE"
    Public Const IN_OTHER_DEBY_COLUMN_NAME As String = "DEBY"
    Public Const IN_OTHER_DECC_COLUMN_NAME As String = "DECC"
    Public Const IN_OTHER_DECW_COLUMN_NAME As String = "DECW"
    Public Const IN_OTHER_EANTLCO_COLUMN_NAME As String = "EANTLCO"
    Public Const IN_OTHER_EBMRCO_COLUMN_NAME As String = "EBMRCO"
    Public Const IN_OTHER_ECONCOAT_COLUMN_NAME As String = "ECONCOAT"
    Public Const IN_OTHER_ECONFAT_COLUMN_NAME As String = "ECONFAT"
    Public Const IN_OTHER_ECONFI_COLUMN_NAME As String = "ECONFI"
    Public Const IN_OTHER_ECONPRO_COLUMN_NAME As String = "ECONPRO"
    Public Const IN_OTHER_ENDAYAN_COLUMN_NAME As String = "ENDAYAN"
    Public Const IN_OTHER_ENDAYCT_COLUMN_NAME As String = "ENDAYCT"
    Public Const IN_OTHER_ENDAYGES_COLUMN_NAME As String = "ENDAYGES"
    Public Const IN_OTHER_ENDAYLAC_COLUMN_NAME As String = "ENDAYLAC"
    Public Const IN_OTHER_ENDAYSC_COLUMN_NAME As String = "ENDAYSC"
    Public Const IN_OTHER_ENDAYSUM_COLUMN_NAME As String = "ENDAYSUM"
    Public Const IN_OTHER_ESCRFCO_COLUMN_NAME As String = "ESCRFCO"
    Public Const IN_OTHER_FATMOBRT_COLUMN_NAME As String = "FATMOBRT"
    Public Const IN_OTHER_HREEAT_COLUMN_NAME As String = "HREEAT"
    Public Const IN_OTHER_HRELIE_COLUMN_NAME As String = "HRELIE"
    Public Const IN_OTHER_HREPAW_COLUMN_NAME As String = "HREPAW"
    Public Const IN_OTHER_HRERUN_COLUMN_NAME As String = "HRERUN"
    Public Const IN_OTHER_HRESTD_COLUMN_NAME As String = "HRESTD"
    Public Const IN_OTHER_HREWLK_COLUMN_NAME As String = "HREWLK"
    Public Const IN_OTHER_KPNDMIN_COLUMN_NAME As String = "KPNDMIN"
    Public Const IN_OTHER_LACTIN_COLUMN_NAME As String = "LACTIN"
    Public Const IN_OTHER_LACTYR_COLUMN_NAME As String = "LACTYR"
    Public Const IN_OTHER_MFNCO_COLUMN_NAME As String = "MFNCO"
    Public Const IN_OTHER_NANTLCO_COLUMN_NAME As String = "NANTLCO"
    Public Const IN_OTHER_NCOATCO_COLUMN_NAME As String = "NCOATCO"
    Public Const IN_OTHER_NCONPRO_COLUMN_NAME As String = "NCONPRO"
    Public Const IN_OTHER_NEUNCO_COLUMN_NAME As String = "NEUNCO"
    Public Const IN_OTHER_NSCRFCO_COLUMN_NAME As String = "NSCRFCO"
    Public Const IN_OTHER_NFETCO_COLUMN_NAME As String = "NFETCO"
    Public Const IN_OTHER_PBYN_COLUMN_NAME As String = "PBYN"
    Public Const IN_OTHER_PCWN_COLUMN_NAME As String = "PCWN"
    Public Const IN_OTHER_PCONFET_COLUMN_NAME As String = "PCONFET"
    Public Const IN_OTHER_PCONIN_COLUMN_NAME As String = "PCONIN"
    Public Const IN_OTHER_PDOM_COLUMN_NAME As String = "PDOM"
    Public Const IN_OTHER_PDPN_COLUMN_NAME As String = "PDPN"
    Public Const IN_OTHER_PFATDEP_COLUMN_NAME As String = "PFATDEP"
    Public Const IN_OTHER_PFATMIN_COLUMN_NAME As String = "PFATMIN"
    Public Const IN_OTHER_PFATWAT_COLUMN_NAME As String = "PFATWAT"
    Public Const IN_OTHER_PFETFAT_COLUMN_NAME As String = "PFETFAT"
    Public Const IN_OTHER_PMCDOM_COLUMN_NAME As String = "PMCDOM"
    Public Const IN_OTHER_PMCN_COLUMN_NAME As String = "PMCN"
    Public Const IN_OTHER_PMEBY_COLUMN_NAME As String = "PMEBY"
    Public Const IN_OTHER_PMECC_COLUMN_NAME As String = "PMECC"
    Public Const IN_OTHER_PMECW_COLUMN_NAME As String = "PMECW"
    Public Const IN_OTHER_PMUSPRO_COLUMN_NAME As String = "PMUSPRO"
    Public Const IN_OTHER_PMUSWAT_COLUMN_NAME As String = "PMUSWAT"
    Public Const IN_OTHER_PPROMIN_COLUMN_NAME As String = "PPROMIN"
    Public Const IN_OTHER_PRCAP_COLUMN_NAME As String = "PRCAP"
    Public Const IN_OTHER_PREGIN_COLUMN_NAME As String = "PREGIN"
    Public Const IN_OTHER_PREGYR_COLUMN_NAME As String = "PREGYR"
    Public Const IN_OTHER_PROMOBRT_COLUMN_NAME As String = "PROMOBRT"
    Public Const IN_OTHER_PRUMDRY_COLUMN_NAME As String = "PRUMDRY"
    Public Const IN_OTHER_PRUMGUT_COLUMN_NAME As String = "PRUMGUT"
    Public Const IN_OTHER_PTPCW_XMAX_COLUMN_NAME As String = "PTPCW_XMAX"
    Public Const IN_OTHER_PTPCW_XMIN_COLUMN_NAME As String = "PTPCW_XMIN"
    Public Const IN_OTHER_PTPCW_YMAX_COLUMN_NAME As String = "PTPCW_YMAX"
    Public Const IN_OTHER_PTPCW_YMIN_COLUMN_NAME As String = "PTPCW_YMIN"
    Public Const IN_OTHER_PTPNDIG_COLUMN_NAME As String = "PTPNDIG"
    Public Const IN_OTHER_PUPN_COLUMN_NAME As String = "PUPN"
    Public Const IN_OTHER_PWATIN_COLUMN_NAME As String = "PWATIN"
    Public Const IN_OTHER_PWATMAX_COLUMN_NAME As String = "PWATMAX"
    Public Const IN_OTHER_PFATIN_COLUMN_NAME As String = "PFATIN"
    Public Const IN_OTHER_PPROIN_COLUMN_NAME As String = "PPROIN"
    Public Const IN_OTHER_RNCCTAN_XMAX_COLUMN_NAME As String = "RNCCTAN_XMAX"
    Public Const IN_OTHER_RNCCTAN_XMIN_COLUMN_NAME As String = "RNCCTAN_XMIN"
    Public Const IN_OTHER_RNCCTAN_YMAX_COLUMN_NAME As String = "RNCCTAN_YMAX"
    Public Const IN_OTHER_RNCCTAN_YMIN_COLUMN_NAME As String = "RNCCTAN_YMIN"
    Public Const IN_OTHER_SDPROP_COLUMN_NAME As String = "SDPROP"
    Public Const IN_OTHER_STDAYAN_COLUMN_NAME As String = "STDAYAN"
    Public Const IN_OTHER_STDAYCT_COLUMN_NAME As String = "STDAYCT"
    Public Const IN_OTHER_STDAYGES_COLUMN_NAME As String = "STDAYGES"
    Public Const IN_OTHER_STDAYLAC_COLUMN_NAME As String = "STDAYLAC"
    Public Const IN_OTHER_STDAYSC_COLUMN_NAME As String = "STDAYSC"
    Public Const IN_OTHER_STDAYSUM_COLUMN_NAME As String = "STDAYSUM"
    Public Const IN_OTHER_WTBODYIN_COLUMN_NAME As String = "WTBODYIN"
    Public Const IN_OTHER_WTTBIR_COLUMN_NAME As String = "WTTBIR"

End Module
