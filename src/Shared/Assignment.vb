'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module Assignment

    Public Sub ValidateAssignment(ByVal value As Double)

        If (Double.IsInfinity(value) Or Double.IsNaN(value)) Then

            Throw New Exception(
                "The assignment value is not valid.  This can indicate missing data.")

        End If

    End Sub

End Module
