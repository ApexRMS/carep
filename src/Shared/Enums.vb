﻿'*********************************************************************************************
' carep: SyncroSim Base Package for simulating the energy and protein dynamics of caribou.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module Enums

    Enum OutputLevel
        Low = 1
        Medium = 2
        High = 3
    End Enum

    Enum CalfFate
        NoCalf = 1
        Lactating = 2
        PostNatalWeaner = 3
        SummerWeaner = 4
        EarlyWeaner = 5
        NormalWeaner = 6
        ExtendedLactator = 7
        InUtero = 8
    End Enum

    Enum Stage
        Base = 1
        Parasites = 2
        Coat = 3
        Scurf = 4
        Activity = 5
        SummerProtein = 6
        Gestation = 7
        Lactation = 8
        Antlers = 9
        Protein = 10
        Fat = 11
    End Enum

    Enum LactationStatus
        Lactating = 1
        NotLactating = 2
        AnyStatus = 3
    End Enum

End Module
