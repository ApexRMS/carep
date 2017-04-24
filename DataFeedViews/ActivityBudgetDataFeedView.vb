'************************************************************************************
' CAREP: A .NET library for simulating caribou energy protein models.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'************************************************************************************

Imports System.Reflection
Imports SyncroSim.Core
Imports SyncroSim.Core.Forms

''' <summary>
''' Activity Budget data feed view
''' </summary>
''' <remarks>
''' This view contains two nested default views, a single row view for the Proportion Eating Multiplier and
''' a multi-row view for the Activity Budget.
''' </remarks>
<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class ActivityBudgetDataFeedView

    ''' <summary>
    ''' Overrides InitializeView
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub InitializeView()

        MyBase.InitializeView()

        Dim pemView As SingleRowDataFeedView = Me.Session.CreateSingleRowDataFeedView(Me.Scenario, Me.ControllingScenario)
        Me.PanelProportionEatingMultiplier.Controls.Add(pemView)

        Dim actBudgetView As DataFeedView = Me.Session.CreateMultiRowDataFeedView(Me.Scenario, Me.ControllingScenario)
        Me.PanelActivityBudget.Controls.Add(actBudgetView)

        'The single row view has four rows and no space to have more or less.  To make this look
        'good we only want to show the cell borders.  If we also show the border it will conflict 
        'with the top and bottom rows.

        pemView.ShowBorder = False
        pemView.PaintGridCellBorders = True

    End Sub

    ''' <summary>
    ''' Overrides LoadDataFeed
    ''' </summary>
    ''' <param name="dataFeed"></param>
    ''' <remarks>
    ''' This function initializes the data feed views with the correct data sheets
    ''' </remarks>
    Public Overrides Sub LoadDataFeed(ByVal dataFeed As DataFeed)

        MyBase.LoadDataFeed(dataFeed)

        Dim pemView As DataFeedView = CType(Me.PanelProportionEatingMultiplier.Controls(0), DataFeedView)
        pemView.LoadDataFeed(dataFeed, DATASHEET_ACT_PEM_NAME)

        Dim actBudgetView As DataFeedView = CType(Me.PanelActivityBudget.Controls(0), DataFeedView)
        actBudgetView.LoadDataFeed(dataFeed, DATASHEET_ACT_NAME)

    End Sub

    ''' <summary>
    ''' Overrides EnableView
    ''' </summary>
    ''' <param name="enable"></param>
    ''' <remarks>
    ''' We need to do our own enabling or the data feed views will be completely disabled instead of just grayed.
    ''' </remarks>
    Public Overrides Sub EnableView(enable As Boolean)

        If (Me.PanelProportionEatingMultiplier.Controls.Count > 0) Then

            Dim v As DataFeedView = CType(Me.PanelProportionEatingMultiplier.Controls(0), DataFeedView)
            v.EnableView(enable)

        End If

        If (Me.PanelActivityBudget.Controls.Count > 0) Then

            Dim v As DataFeedView = CType(Me.PanelActivityBudget.Controls(0), DataFeedView)
            v.EnableView(enable)

        End If

        Me.LabelActPEM.Enabled = enable
        Me.LabelActivityBudget.Enabled = enable

    End Sub


End Class
