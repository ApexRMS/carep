'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Module Assignment

    Public Sub ValidateAssignment(ByVal value As Double)

        If (Double.IsInfinity(value) Or Double.IsNaN(value)) Then

            Throw New Exception(
                "The assignment value is not valid.  This can indicate missing data.")

        End If

    End Sub

End Module
