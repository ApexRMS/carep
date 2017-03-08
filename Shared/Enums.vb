'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

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
