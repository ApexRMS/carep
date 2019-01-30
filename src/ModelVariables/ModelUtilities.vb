'*********************************************************************************************
' Caribou Energy Protein: A SyncroSim Package for the Caribou Energy Protein Model.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module ModelUtilities

    ''' <summary>
    ''' Gets the year to copy data from for the specified year
    ''' </summary>
    ''' <param name="dict">A dictionary containing all the years that have data</param>
    ''' <param name="year">The year that has no data</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCopyYear(
        ByVal dict As Dictionary(Of Integer, Boolean),
        ByVal year As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer) As Integer

        Debug.Assert(Not dict.ContainsKey(year))

        'If the year is the first year then we only need to search right.
        'And if the year is the last year then we only need to search left.
        'But if the year is in the middle, then we search for the nearest
        'year with data, alternating left to right.

        If (year = minYear) Then

            For i As Integer = minYear + 1 To maxYear

                If (dict.ContainsKey(i)) Then
                    Return i
                End If

            Next

        ElseIf (year = maxYear) Then

            For i As Integer = maxYear - 1 To minYear Step -1

                If (dict.ContainsKey(i)) Then
                    Return i
                End If

            Next

        Else

            Dim l As Integer = year - 1
            Dim r As Integer = year + 1

            Debug.Assert(maxYear > 2)
            Debug.Assert(l >= minYear And r <= maxYear)

            While (True)

                If (dict.ContainsKey(l)) Then
                    Return l
                End If

                If (dict.ContainsKey(r)) Then
                    Return r
                End If

                If (l = minYear And r = maxYear) Then
                    Throw New Exception("GetCopyYear failed!  Cannot continue.")
                End If

                If (l <> minYear) Then
                    l -= 1
                End If

                If (r <> maxYear) Then
                    r += 1
                End If

            End While

        End If

        Throw New Exception("GetCopyYear failed!  Cannot continue.")
        Return -1

    End Function

    ''' <summary>
    ''' Interpolates between fixed values in a zero-based 1-dimensional array of type Double
    ''' </summary>
    ''' <param name="valuesToInterpolate">1-dimensional array of Double values to fill in through linear interpolation</param>
    ''' <param name="isFixed">indicates whether or not each element in the valuesToInterpolate array is fixed prior to interpolation</param>
    ''' <remarks>Assigns first and last fixed values to all elements before and after these positions</remarks>
    Public Sub InterpolateArray(ByVal valuesToInterpolate() As Double, ByVal isFixed() As Boolean)
        Dim index, maxIndex, lowerIndex, upperIndex, intervalIndex As Integer
        Dim lowerValue, upperValue, valueUnitChange As Double
        maxIndex = valuesToInterpolate.GetUpperBound(0)
        upperIndex = maxIndex 'Load this way so that if no fixed values, then no interpolation occurs
        For index = 0 To maxIndex
            'find the first fixed value
            If isFixed(index) Then
                'found the first fixed value in the array
                'set all prior values to the first first value
                upperIndex = index
                upperValue = valuesToInterpolate(index)
                For intervalIndex = 0 To (upperIndex - 1)
                    valuesToInterpolate(intervalIndex) = valuesToInterpolate(upperIndex)
                Next
                Exit For
            End If
        Next
        For index = (upperIndex + 1) To maxIndex
            'find the next fixed value
            If isFixed(index) Then
                'found next fixed value in the array - set up a new interval for interpolation
                'reset the previous upper index & value to the lower position
                lowerIndex = upperIndex
                lowerValue = upperValue
                'set the new upper index & value to the position just found
                upperIndex = index
                upperValue = valuesToInterpolate(index)
                'calculate the unit change in value over this interval
                valueUnitChange = (upperValue - lowerValue) / CType((upperIndex - lowerIndex), Double)
                'now interpolate over interval
                For intervalIndex = (lowerIndex + 1) To (upperIndex - 1)
                    valuesToInterpolate(intervalIndex) = valuesToInterpolate(lowerIndex) + ((intervalIndex - lowerIndex) * valueUnitChange)
                Next
            ElseIf index = maxIndex Then
                'reached the last index and it is not fixed
                'set all remaining values to the last fixed value
                For intervalIndex = (upperIndex + 1) To maxIndex
                    valuesToInterpolate(intervalIndex) = valuesToInterpolate(upperIndex)
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' Gets the years that have data for the specified model variable
    ''' </summary>
    ''' <param name="mv1Double">The model variable</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetYearsWithData(
        ByVal mv1Double As ModelVariable1(Of Double),
        ByVal minYear As Integer,
        ByVal maxYear As Integer) As Dictionary(Of Integer, Boolean)

        Dim DayStart As Integer = 1
        Dim DayEnd As Integer = 365
        Dim dict As New Dictionary(Of Integer, Boolean)

        For yr As Integer = minYear To maxYear

            For d As Integer = DayStart To DayEnd

                If (mv1Double.IsFromSource(d)) Then

                    dict.Add(yr, True)
                    Exit For

                End If

            Next

            DayStart += 365
            DayEnd += 365

        Next

        Return dict

    End Function

    ''' <summary>
    ''' Copies missing yearly data for the specified model variable
    ''' </summary>
    ''' <param name="mv1Double">The model variable</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <remarks></remarks>
    Public Sub CopyMissingYearlyData(
        ByVal mv1Double As ModelVariable1(Of Double),
        ByVal minYear As Integer,
        ByVal maxYear As Integer)

        Debug.Assert(minYear = 1)

        'If there is only one year, there is nothing to do
        If (minYear = maxYear) Then
            Return
        End If

        'Get the years with data for this X. If no years have data then we can just continue on.
        Dim dict As Dictionary(Of Integer, Boolean) = GetYearsWithData(mv1Double, minYear, maxYear)

        If (dict.Count = 0) Then
            Return
        End If

        'For each year that has no data, copy the data from the nearest year
        For TargetYear As Integer = minYear To maxYear

            If (dict.ContainsKey(TargetYear)) Then
                Continue For
            End If

            Dim CopyYear As Integer = GetCopyYear(dict, TargetYear, minYear, maxYear)
            Dim TargetDay As Integer = ((TargetYear - 1) * 365) + 1
            Dim CopyDay As Integer = ((CopyYear - 1) * 365) + 1

            Debug.Assert(CopyYear <> TargetYear)
            Debug.Assert(TargetDay <> CopyDay)

            For i As Integer = 1 To 365

                mv1Double(TargetDay) = mv1Double(CopyDay)

                TargetDay += 1
                CopyDay += 1

            Next

            dict.Add(TargetYear, True)

        Next

    End Sub

    ''' <summary>
    ''' Interpolates the data for the specified model variable on a yearly basis
    ''' </summary>
    ''' <param name="mv1Double">The model variable</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <remarks></remarks>
    Public Sub InterpolateModelVariableByYear(
        ByVal mv1Double As ModelVariable1(Of Double),
        ByVal minYear As Integer,
        ByVal maxYear As Integer)

        Debug.Assert(minYear = 1)

        Dim DayStart As Integer = 1
        Dim DayEnd As Integer = 365
        Dim valuesToInterpolate(364) As Double
        Dim valuesFixed(364) As Boolean

        For yr As Integer = minYear To maxYear

            Dim Index As Integer = 0

            For x = DayStart To DayEnd
                valuesToInterpolate(Index) = mv1Double(x)
                valuesFixed(Index) = mv1Double.IsFromSource(x)
                Index += 1
            Next

            InterpolateArray(valuesToInterpolate, valuesFixed)

            Index = 0

            For x = DayStart To DayEnd
                mv1Double(x) = valuesToInterpolate(Index)
                Index += 1
            Next

            DayStart += 365
            DayEnd += 365

        Next

    End Sub

    ''' <summary>
    ''' Interpolates between values set from a source in a ModelVariable1 of type Double
    ''' </summary>
    ''' <param name="mv1Double">ModelVariable1 of Double values to fill in</param>
    ''' <remarks></remarks>
    Public Sub InterpolateModelVariable(ByVal mv1Double As ModelVariable1(Of Double))
        InterpolateArray(mv1Double.Values, mv1Double.IsFromSource)
    End Sub

    ''' <summary>
    ''' Gets the years that have data for the specified model variable and outer X
    ''' </summary>
    ''' <param name="mv2Double">The model variable</param>
    ''' <param name="x">The outer dimension</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetYearsWithData(
        ByVal mv2Double As ModelVariable2(Of Double),
        ByVal x As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer) As Dictionary(Of Integer, Boolean)

        Dim DayStart As Integer = 1
        Dim DayEnd As Integer = 365
        Dim dict As New Dictionary(Of Integer, Boolean)

        For yr As Integer = minYear To maxYear

            For d As Integer = DayStart To DayEnd

                If (mv2Double.FromSource(x, d)) Then

                    dict.Add(yr, True)
                    Exit For

                End If

            Next

            DayStart += 365
            DayEnd += 365

        Next

        Return dict

    End Function

    ''' <summary>
    ''' Copies missing yearly data for the specified model variable
    ''' </summary>
    ''' <param name="mv2Double">The model variable</param>
    ''' <param name="minX">The outer dimension minimum</param>
    ''' <param name="maxX">The outer dimension maximum</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <remarks></remarks>
    Public Sub CopyMissingYearlyData(
        ByVal mv2Double As ModelVariable2(Of Double),
        ByVal minX As Integer,
        ByVal maxX As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer)

        Debug.Assert(minYear = 1)

        'If there is only one year, there is nothing to do
        If (minYear = maxYear) Then
            Return
        End If

        For x As Integer = minX To maxX

            'Get the years with data for this X. If no years have data then we can just continue on.
            Dim dict As Dictionary(Of Integer, Boolean) = GetYearsWithData(mv2Double, x, minYear, maxYear)

            If (dict.Count = 0) Then
                Continue For
            End If

            'For each year that has no data, copy the data from the nearest year
            For TargetYear As Integer = minYear To maxYear

                If (dict.ContainsKey(TargetYear)) Then
                    Continue For
                End If

                Dim CopyYear As Integer = GetCopyYear(dict, TargetYear, minYear, maxYear)
                Dim TargetDay As Integer = ((TargetYear - 1) * 365) + 1
                Dim CopyDay As Integer = ((CopyYear - 1) * 365) + 1

                Debug.Assert(CopyYear <> TargetYear)
                Debug.Assert(TargetDay <> CopyDay)

                For i As Integer = 1 To 365

                    mv2Double(x, TargetDay) = mv2Double(x, CopyDay)

                    TargetDay += 1
                    CopyDay += 1

                Next

                dict.Add(TargetYear, True)

            Next

        Next

    End Sub

    ''' <summary>
    ''' Interpolates the data for the specified model variable on a yearly basis
    ''' </summary>
    ''' <param name="mv2Double">The model variable</param>
    ''' <param name="minX">The outer dimension minimum</param>
    ''' <param name="maxX">The outer dimension maximum</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <remarks></remarks>
    Public Sub InterpolateModelVariableByYear(
        ByVal mv2Double As ModelVariable2(Of Double),
        ByVal minX As Integer,
        ByVal maxX As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer)

        Debug.Assert(minYear = 1)

        For x As Integer = minX To maxX

            Dim DayStart As Integer = 1
            Dim DayEnd As Integer = 365

            For yr As Integer = minYear To maxYear

                InterpolateModelVariable(mv2Double, x, DayStart, x, DayEnd)

                DayStart += 365
                DayEnd += 365

            Next

        Next

    End Sub

    ''' <summary>
    ''' Interpolates between values set from a source in a Double ModelVariable2 across the last dimension
    ''' </summary>
    ''' <param name="mv2Double">ModelVariable2 of Double values to fill in</param>
    ''' <param name="xMinIndex">The minimum value for the first dimension</param>
    ''' <param name="yMinIndex">The maximum value for the second dimension</param>
    ''' <param name="xMaxIndex">The minimum value for the first dimension</param>
    ''' <param name="yMaxIndex">The maximum value for the second dimension</param>
    ''' <remarks>Holds the x index constant while interpolating values across the y dimension</remarks>
    Public Sub InterpolateModelVariable(
        ByVal mv2Double As ModelVariable2(Of Double),
        ByVal xMinIndex As Integer,
        ByVal yMinIndex As Integer,
        ByVal xMaxIndex As Integer,
        ByVal yMaxIndex As Integer)

        Dim xIndex, yIndex As Integer
        Dim valuesToInterpolate(yMaxIndex) As Double
        Dim valuesFixed(yMaxIndex) As Boolean

        For xIndex = xMinIndex To xMaxIndex
            'hold the x index constant and interpolate values across the y dimension
            For yIndex = yMinIndex To yMaxIndex
                'copy over values to 1-dimensional arrays suitable for interpolation
                valuesToInterpolate(yIndex) = mv2Double(xIndex, yIndex)
                valuesFixed(yIndex) = mv2Double.FromSource(xIndex, yIndex)  'interpolate between values that come for a source
            Next
            InterpolateArray(valuesToInterpolate, valuesFixed)
            For yIndex = yMinIndex To yMaxIndex
                'now put the interpolated values back into the model variable
                mv2Double(xIndex, yIndex) = valuesToInterpolate(yIndex)
            Next
        Next

    End Sub

    ''' <summary>
    ''' Interpolates between values set from a source in a Double ModelVariable2 across the last dimension
    ''' </summary>
    ''' <param name="mv2Double">ModelVariable2 of Double values to fill in</param>
    ''' <remarks></remarks>
    Public Sub InterpolateModelVariable(ByVal mv2Double As ModelVariable2(Of Double))

        InterpolateModelVariable(
            mv2Double,
            mv2Double.Dimensions(0).MinIndex,
            mv2Double.Dimensions(1).MinIndex,
            mv2Double.Dimensions(0).MaxIndex,
            mv2Double.Dimensions(1).MaxIndex)

    End Sub

    ''' <summary>
    ''' Gets the years that have data for the specified model variable and outer X and Y
    ''' </summary>
    ''' <param name="mv3Double">The model variable</param>
    ''' <param name="x">The outer dimension 0</param>
    ''' <param name="y">The outer dimension 1</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetYearsWithData(
        ByVal mv3Double As ModelVariable3(Of Double),
        ByVal x As Integer,
        ByVal y As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer) As Dictionary(Of Integer, Boolean)

        Dim DayStart As Integer = 1
        Dim DayEnd As Integer = 365
        Dim dict As New Dictionary(Of Integer, Boolean)

        For yr As Integer = minYear To maxYear

            For d As Integer = DayStart To DayEnd

                If (mv3Double.FromSource(x, y, d)) Then

                    dict.Add(yr, True)
                    Exit For

                End If

            Next

            DayStart += 365
            DayEnd += 365

        Next

        Return dict

    End Function

    ''' <summary>
    ''' Copies missing yearly data for the specified model variable
    ''' </summary>
    ''' <param name="mv3Double">The model variable</param>
    ''' <param name="minX">The outer dimension 0 minimum</param>
    ''' <param name="maxX">The outer dimension 0 maximum</param>
    ''' <param name="minY">The outer dimension 1 minimum</param>
    ''' <param name="maxY">The outer dimension 1 maximum</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <remarks></remarks>
    Public Sub CopyMissingYearlyData(
        ByVal mv3Double As ModelVariable3(Of Double),
        ByVal minX As Integer,
        ByVal maxX As Integer,
        ByVal minY As Integer,
        ByVal maxY As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer)

        Debug.Assert(minYear = 1)

        'If there is only one year, there is nothing to do
        If (minYear = maxYear) Then
            Return
        End If

        For x As Integer = minX To maxX

            For y As Integer = minY To maxY

                'Get the years with data for this X and Y. If no years have data then we can just continue on.
                Dim dict As Dictionary(Of Integer, Boolean) = GetYearsWithData(mv3Double, x, y, minYear, maxYear)

                If (dict.Count = 0) Then
                    Continue For
                End If

                'For each year that has no data, copy the data from the nearest year
                For TargetYear As Integer = minYear To maxYear

                    If (dict.ContainsKey(TargetYear)) Then
                        Continue For
                    End If

                    Dim CopyYear As Integer = GetCopyYear(dict, TargetYear, minYear, maxYear)
                    Dim TargetDay As Integer = ((TargetYear - 1) * 365) + 1
                    Dim CopyDay As Integer = ((CopyYear - 1) * 365) + 1

                    Debug.Assert(CopyYear <> TargetYear)
                    Debug.Assert(TargetDay <> CopyDay)

                    For i As Integer = 1 To 365

                        mv3Double(x, y, TargetDay) = mv3Double(x, y, CopyDay)

                        TargetDay += 1
                        CopyDay += 1

                    Next

                    dict.Add(TargetYear, True)

                Next

            Next

        Next

    End Sub

    ''' <summary>
    ''' Interpolates the data for the specified model variable on a yearly basis
    ''' </summary>
    ''' <param name="mv3Double">The model variable</param>
    ''' <param name="minX">The outer dimension 0 minimum</param>
    ''' <param name="maxX">The outer dimension 0 maximum</param>
    ''' <param name="minY">The outer dimension 1 minimum</param>
    ''' <param name="maxY">The outer dimension 1 maximum</param>
    ''' <param name="minYear">The minimum year</param>
    ''' <param name="maxYear">The maximum year</param>
    ''' <remarks></remarks>
    Public Sub InterpolateModelVariableByYear(
        ByVal mv3Double As ModelVariable3(Of Double),
        ByVal minX As Integer,
        ByVal maxX As Integer,
        ByVal minY As Integer,
        ByVal maxY As Integer,
        ByVal minYear As Integer,
        ByVal maxYear As Integer)

        Debug.Assert(minYear = 1)

        For x As Integer = minX To maxX

            For y As Integer = minY To maxY

                Dim DayStart As Integer = 1
                Dim DayEnd As Integer = 365

                For yr As Integer = minYear To maxYear

                    InterpolateModelVariable(mv3Double, x, y, DayStart, x, y, DayEnd)

                    DayStart += 365
                    DayEnd += 365

                Next

            Next

        Next

    End Sub

    ''' <summary>
    ''' Interpolates between values set from a source in a Double ModelVariable3 across the last dimension
    ''' </summary>
    ''' <param name="mv3Double">ModelVariable3 of Double values to fill in</param>
    ''' <param name="xMinIndex">The minimum value for the first dimension</param>
    ''' <param name="yMinIndex">The maximum value for the second dimension</param>
    ''' <param name="zMinIndex">The maximum value for the third dimension</param>
    ''' <param name="xMaxIndex">The minimum value for the first dimension</param>
    ''' <param name="yMaxIndex">The maximum value for the second dimension</param>
    ''' <param name="zMaxIndex">The maximum value for the third dimension</param>
    ''' <remarks>Holds the x and y index constant while interpolating values across the z dimension</remarks>
    Public Sub InterpolateModelVariable(
        ByVal mv3Double As ModelVariable3(Of Double),
        ByVal xMinIndex As Integer,
        ByVal yMinIndex As Integer,
        ByVal zMinIndex As Integer,
        ByVal xMaxIndex As Integer,
        ByVal yMaxIndex As Integer,
        ByVal zMaxIndex As Integer)

        Dim xIndex, yIndex, zIndex As Integer
        Dim valuesToInterpolate(zMaxIndex) As Double
        Dim valuesFixed(zMaxIndex) As Boolean

        For xIndex = xMinIndex To xMaxIndex
            For yIndex = yMinIndex To yMaxIndex
                'hold the x and y index constant and interpolate values across the z dimension
                For zIndex = zMinIndex To zMaxIndex
                    'copy over values to 1-dimensional arrays suitable for interpolation
                    valuesToInterpolate(zIndex) = mv3Double(xIndex, yIndex, zIndex)
                    valuesFixed(zIndex) = mv3Double.FromSource(xIndex, yIndex, zIndex)  'interpolate between values that come for a source
                Next
                InterpolateArray(valuesToInterpolate, valuesFixed)
                For zIndex = zMinIndex To zMaxIndex
                    'now put the interpolated values back into the model variable
                    mv3Double(xIndex, yIndex, zIndex) = valuesToInterpolate(zIndex)
                Next
            Next
        Next

    End Sub

    ''' <summary>
    ''' Interpolates between values set from a source in a Double ModelVariable3 across the last dimension
    ''' </summary>
    ''' <param name="mv3Double">ModelVariable3 of Double values to fill in</param>
    ''' <remarks></remarks>
    Public Sub InterpolateModelVariable(ByVal mv3Double As ModelVariable3(Of Double))

        InterpolateModelVariable(
            mv3Double,
            mv3Double.Dimensions(0).MinIndex,
            mv3Double.Dimensions(1).MinIndex,
            mv3Double.Dimensions(2).MinIndex,
            mv3Double.Dimensions(0).MaxIndex,
            mv3Double.Dimensions(1).MaxIndex,
            mv3Double.Dimensions(2).MaxIndex)

    End Sub

End Module
