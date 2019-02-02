'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module Constants

    Public Const MinRun As Integer = 1        'minimum run for simulation model loop
    Public Const MinHour As Integer = 0       'minimum hour for simulation model loop - hour 0 used to store previous day's values for some variables
    Public Const MaxHour As Integer = 24      'maximum hour for simulation model loop
    Public Const MinYear As Integer = 1       'minimum possible value for year (generally don't change here!)
    Public Const MaxYear As Integer = 100     'maximum possible value for year (generally don't change here!)
    Public Const MinJDay As Integer = 1       'minimum possible value for a julian day (generally don't change here!)
    Public Const MaxJDay As Integer = 365     'maximum possible value for a julian day (generally don't change here!)
    Public Const RunFieldName As String = "Iteration"      'default field name for Run
    Public Const PlantPKFieldName As String = "EP_PlantID"      'default field name for Plant Type
    Public Const PlantDisplayName As String = "Plant Type [p]"      'default field name for Plant Type display name
    Public Const StratumPKFieldName As String = "EP_StratumID"      'default field name for Stratum
    Public Const StratumDisplayName As String = "Stratum [s]"      'default field name for Stratum display name
    Public Const StagePKFieldName As String = "EP_StageID"      'default field name for Allocation Stage
    Public Const StageDisplayName As String = "Allocation Stage [a]"      'default field name for Allocation Stage display name
    Public Const HourFieldName As String = "Hour"           'default field name for Hour
    Public Const TimeStepFieldName As String = "Timestep"   'default field name for TimeStep
    Public Const YearFieldName As String = "Year"   'default field name for TimeStep
    Public Const JDayFieldName As String = "JulianDay"   'default field name for TimeStep

End Module
