'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Module for the Caribou Energy Protein Model.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module DayTimeUtils

    ''' <summary>
    ''' Returns the timestep equivalent of a Year and Julian Day pair, where first timestep is minTimeStep
    ''' </summary>
    ''' <param name="Year">Year to convert</param>
    ''' <param name="JulianDay">Julian Day to convert</param>
    ''' <param name="startYear">Start Year for timesteps</param>
    ''' <param name="startJulianDay">Start Julian Day for timesteps</param>
    ''' <param name="minTimeStep">Timestep value for the Year=0, Julian Day = 1</param>
    ''' <returns></returns>
    ''' <remarks>Assumes 365 days for all years. Does not return timestep less than 0</remarks>
    Public Function TimestepFromJulianDayYear(ByVal year As Integer, ByVal julianDay As Integer, _
            ByVal startYear As Integer, ByVal startJulianDay As Integer, ByVal minTimeStep As Integer) As Integer

        Dim endDay, startDay As Integer
        endDay = (year * 365) + julianDay
        startDay = (startYear * 365) + startJulianDay
        TimestepFromJulianDayYear = endDay - startDay + minTimeStep
        If TimestepFromJulianDayYear < 0 Then TimestepFromJulianDayYear = 0

    End Function

    Public Function TimestepFromJulianDayYear(ByVal year As Integer, ByVal julianDay As Integer) As Integer

        Debug.Assert(year > 0)
        Return ((year - 1) * 365) + julianDay

    End Function

    ''' <summary>
    ''' Returns the year corresponding to the timestep
    ''' </summary>
    ''' <param name="timestep">Timestep to convert</param>
    ''' <param name="startYear">Start Year for timesteps</param>
    ''' <param name="startJulianDay">Start Julian Day for timesteps</param>
    ''' <returns></returns>
    ''' <remarks>Assumes 365 days for  all years. Does not convert timestep less than 0</remarks>
    Public Function YearFromTimestep(ByVal timestep As Integer, ByVal startYear As Integer, ByVal startJulianDay As Integer) As Integer
        If timestep < 0 Then timestep = 0
        YearFromTimestep = startYear + ((timestep - 1) \ 365)
    End Function
    ''' <summary>
    ''' Returns the julian day corresponding to the timestep starting year and day
    ''' </summary>
    ''' <param name="timestep">Timestep to convert</param>
    ''' <param name="startYear">Start Year for timesteps</param>
    ''' <param name="startJulianDay">Start Julian Day for timesteps</param>
    ''' <returns></returns>
    ''' <remarks>Assumes 365 days for all years. Does not convert timestep less than 0</remarks>
    Public Function JulianDayFromTimestepStartYearDay(ByVal timestep As Integer, ByVal startYear As Integer, ByVal startJulianDay As Integer) As Integer
        If timestep < 0 Then timestep = 0
        JulianDayFromTimestepStartYearDay = startJulianDay + ((timestep - 1) Mod 365)
    End Function

    ''' <summary>
    ''' Returns the julian day corresponding to the timestep.
    ''' </summary>
    ''' <param name="timestep"> the simulation timestep which is assumed to start on julian day 1</param>
    ''' <returns></returns>
    ''' <remarks>Assumes 365 days for all simulations and that timestep 1 is julian day 1</remarks>
    Public Function JulianDayFromTimestep(ByVal timestep As Integer) As Integer
        Dim jd As Integer = timestep Mod 365 'julian day index
        If jd = 0 Then
            jd = 365
        End If
        Return jd
    End Function

End Module
