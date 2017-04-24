'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

''' <summary>
''' Model Variable with 4 dimensions
''' </summary>
''' <typeparam name="T">Generic type T</typeparam>
''' <remarks></remarks>
Class ModelVariable4(Of T)
    Inherits ModelVariable(Of T)
    ''' <summary>
    ''' Instantiates as a 4-dimensional array with max indexes of 0
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
        Me.CreateDimensions(0, 0, 0, 0)
    End Sub
    ''' <summary>
    ''' Instantiates as a 4-dimensional array with max indexes of 0 and a name
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal name As String)
        MyBase.New(name)
        Me.CreateDimensions(0, 0, 0, 0)
    End Sub
    ''' <summary>
    ''' Completes instantiation as a 4-dimensional array, with maximum dimensions specified as Integers
    ''' </summary>
    ''' <param name="D0MaxIndex">Maximum value of the dimension 0 index</param>
    ''' <param name="D1MaxIndex">Maximum value of the dimension 1 index</param>
    ''' <param name="D2MaxIndex">Maximum value of the dimension 2 index</param>
    ''' <param name="D3MaxIndex">Maximum value of the dimension 3 index</param>
    ''' <remarks></remarks>
    Public Overridable Overloads Sub CreateDimensions(ByVal D0MaxIndex As Integer, ByVal D1MaxIndex As Integer, ByVal D2MaxIndex As Integer, _
        ByVal D3MaxIndex As Integer)
        Dim maxIndexes() As Integer = {D0MaxIndex, D1MaxIndex, D2MaxIndex, D3MaxIndex}
        Me.CreateDimensions(maxIndexes)
    End Sub
    ''' <summary>
    ''' Value indexed according to the 4-dimension index
    ''' </summary>
    ''' <param name="d0Index">Index value for 0-dimension</param>
    ''' <param name="d1Index">Index value for 1-dimension</param>
    ''' <param name="d2Index">Index value for 2-dimension</param>
    ''' <param name="d3Index">Index value for 3-dimension</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Default Public Overridable Overloads Property Item(ByVal d0Index As Integer, _
        ByVal d1Index As Integer, ByVal d2Index As Integer, ByVal d3Index As Integer) As T
        Get
            Return Me.Values(Me.ArrayIndex(d0Index, d1Index, d2Index, d3Index))
        End Get
        Set(ByVal value As T)
            Me.Values(Me.ArrayIndex(d0Index, d1Index, d2Index, d3Index)) = value
        End Set
    End Property
    ''' <summary>
    ''' Value indexed according to the 4-dimension index pair filled from data source; IsFromSource value set to true
    ''' </summary>
    ''' <param name="d0Index">Index value for 0-dimension</param>
    ''' <param name="d1Index">Index value for 1-dimension</param>
    ''' <param name="d2Index">Index value for 2-dimension</param>
    ''' <param name="d3Index">Index value for 3-dimension</param>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Overridable Overloads WriteOnly Property FillItem(ByVal d0Index As Integer, ByVal d1Index As Integer, ByVal d2Index As Integer, ByVal d3Index As Integer) As T
        Set(ByVal value As T)

            Dim arrIndex As Integer = Me.ArrayIndex(d0Index, d1Index, d2Index, d3Index)

            If (arrIndex < Me.Values.Length) Then

                m_Values(arrIndex) = value
                m_IsFromSource(arrIndex) = True
                m_HasValue(arrIndex) = True

            End If

        End Set

    End Property
    ''' <summary>
    ''' Indicates whether or not each individual value was filled from a source
    ''' </summary>
    ''' <param name="d0Index">Index value for 0-dimension</param>
    ''' <param name="d1Index">Index value for 1-dimension</param>
    ''' <param name="d2Index">Index value for 2-dimension</param>
    ''' <param name="d3Index">Index value for 3-dimension</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Overloads ReadOnly Property FromSource(ByVal d0Index As Integer, _
        ByVal d1Index As Integer, ByVal d2Index As Integer, ByVal d3Index As Integer) As Boolean
        Get
            Return m_IsFromSource(Me.ArrayIndex(d0Index, d1Index, d2Index, d3Index))
        End Get
    End Property
    ''' <summary>
    ''' Converts the 4-dimension set of indexes into a based value array index
    ''' </summary>
    ''' <param name="d0Index">Index value for 0-dimension</param>
    ''' <param name="d1Index">Index value for 1-dimension</param>
    ''' <param name="d2Index">Index value for 2-dimension</param>
    ''' <param name="d3Index">Index value for 3-dimension</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function ArrayIndex(ByVal d0Index As Integer, _
        ByVal d1Index As Integer, ByVal d2Index As Integer, ByVal d3Index As Integer) As Integer

        Dim d0Length, d1Length, d2Length As Integer
        Dim d0IndexBase, d1IndexBase, d2IndexBase, d3IndexBase As Integer

        d0IndexBase = d0Index - m_Dimensions(0).MinIndex
        d1IndexBase = d1Index - m_Dimensions(1).MinIndex
        d2IndexBase = d2Index - m_Dimensions(2).MinIndex
        d3IndexBase = d3Index - m_Dimensions(3).MinIndex

        d0Length = m_UpperBound(0) + 1
        d1Length = m_UpperBound(1) + 1
        d2Length = m_UpperBound(2) + 1

        ArrayIndex = (d3IndexBase * (d2Length * d1Length * d0Length)) + (d2IndexBase * (d1Length * d0Length)) + (d1IndexBase * d0Length) + d0IndexBase

        Debug.Assert(ArrayIndex >= 0)

    End Function
    ''' <summary>
    ''' Fills all elements of a 4-dimensional model variable from the filtered source
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Fill()
        Dim row As DataRow
        Dim d0Index, d1Index, d2Index, d3Index As Integer
        Dim d0Col As String = m_Dimensions(0).SourceFieldName
        Dim d1Col As String = m_Dimensions(1).SourceFieldName
        Dim d2Col As String = m_Dimensions(2).SourceFieldName
        Dim d3Col As String = m_Dimensions(3).SourceFieldName
        Dim valCol As String = Me.SourceFieldName

        For Each row In Me.SourceDataTableFiltered.Rows
            d0Index = CInt(row(d0Col))
            d1Index = CInt(row(d1Col))
            d2Index = CInt(row(d2Col))
            d3Index = CInt(row(d3Col))
            Me.FillItem(d0Index, d1Index, d2Index, d3Index) = CType(row(valCol), T)
        Next
    End Sub
End Class
