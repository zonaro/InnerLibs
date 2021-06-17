Imports System.Runtime.CompilerServices
Imports InnerLibs.LINQ

Namespace Select2

    Public Class Select2Data

        Sub New()

        End Sub

        Sub New(Options As IEnumerable(Of ISelect2Option))
            Me.Results = If(Options, {})
        End Sub

        Sub New(Groups As IEnumerable(Of Select2Group))
            Me.Results = If(Groups, {})
        End Sub

        Public Property Results As IEnumerable(Of ISelect2Result) = {}
        Public Property Pagination As New Pagination
    End Class

    Public Interface ISelect2Result
        Property Text As String

    End Interface

    Public Class Pagination
        Public Property More As Boolean = False
    End Class

    Public Class Select2Group
        Implements ISelect2Result

        Sub New(Text As String)
            Me.Text = Text
        End Sub

        Sub New(Text As String, Children As IEnumerable(Of ISelect2Option))
            Me.Text = Text
            Me.Children = If(Children, {})
        End Sub

        Sub New()

        End Sub

        Public Property Children As IEnumerable(Of ISelect2Option) = {}

        Public Property Text As String Implements ISelect2Result.Text

    End Class

    Public Interface ISelect2Option
        Inherits ISelect2Result

        Property ID As String
        Property Selected As Boolean

        Property Disabled As Boolean
    End Interface

    Public NotInheritable Class Select2Option
        Implements ISelect2Option

        Sub New()

        End Sub

        Sub New(Text As String, Value As String)
            Me.ID = Value.IfBlank(Text)
            Me.Text = Text
        End Sub

        Public Property ID As String Implements ISelect2Option.ID

        Property Selected As Boolean Implements ISelect2Option.Selected

        Property Disabled As Boolean Implements ISelect2Option.Disabled

        Public Property Text As String Implements ISelect2Result.Text

        Public Overrides Function ToString() As String
            Return $"<option value='{ID}' {Disabled.AsIf("disabled")} {Selected.AsIf("selected")}>{Text}</option>"
        End Function

    End Class

    Public Module Select2Extensions

        <Extension> Public Function CreateSelect2Data(Of OptionType As ISelect2Option, T)(List As IEnumerable(Of T), TextSelector As Func(Of T, String), IdSelector As Func(Of T, String), Optional OtherSelectors As Action(Of T, OptionType) = Nothing, Optional GroupSelector As Func(Of T, String) = Nothing) As Select2Data
            If GroupSelector IsNot Nothing Then
                Dim itens = List.GroupBy(GroupSelector).Select(Function(x) New Select2Group(x.Key, x.Select(Function(c) c.CreateSelect2Option(TextSelector, IdSelector, OtherSelectors))))
                Return New Select2Data(itens)
            Else
                Dim itens = List.Select(Function(c) c.CreateSelect2Option(TextSelector, IdSelector, OtherSelectors))
                Return New Select2Data(itens)
            End If
        End Function

        <Extension()> Function CreateSelect2Option(Of OptionType As ISelect2Option, T)(item As T, TextSelector As Func(Of T, String), IdSelector As Func(Of T, String), Optional OtherSelectors As Action(Of T, OptionType) = Nothing) As OptionType
            If GetType(T) Is GetType(OptionType) Then
                Return item.ChangeType(Of OptionType)
            Else
                IdSelector = If(IdSelector, TextSelector)
                TextSelector = If(TextSelector, IdSelector)
                Dim Optionitem = Activator.CreateInstance(Of OptionType)
                Optionitem.ID = IdSelector(item)
                Optionitem.Text = TextSelector(item)
                If OtherSelectors IsNot Nothing Then
                    OtherSelectors(item, Optionitem)
                End If
                Return Optionitem
            End If
        End Function

        <Extension> Public Function CreateSelect2Data(Of OptionsType As ISelect2Option, T1 As Class, T2)(Filter As PaginationFilter(Of T1, T2), TextSelector As Func(Of T2, String), IdSelector As Func(Of T2, String), Optional GroupSelector As Func(Of T2, String) = Nothing, Optional OtherSelectors As Action(Of T2, OptionsType) = Nothing) As Select2Data
            Dim d = Filter.GetPage().CreateSelect2Data(TextSelector, IdSelector, OtherSelectors, GroupSelector)
            d.Pagination.More = Filter.PageCount > 1 AndAlso Filter.IsLastPage = False
            Return d
        End Function

        <Extension> Public Function CreateSelect2Data(Of OptionsType As ISelect2Option, T1 As Class, T2)(Filter As PaginationFilter(Of T1, T2), TextSelector As Func(Of T2, String), IdSelector As Func(Of T2, String), OtherSelectors As Action(Of T2, OptionsType)) As Select2Data
            Return Filter.CreateSelect2Data(TextSelector, IdSelector, Nothing, OtherSelectors)
        End Function

        <Extension> Public Function CreateSelect2Data(Of OptionsType As ISelect2Option, T1 As Class)(Filter As PaginationFilter(Of T1, OptionsType)) As Select2Data
            Return Filter.CreateSelect2Data(Of OptionsType)(Function(x As OptionsType) x.Text, Function(x As OptionsType) x.ID)
        End Function

        <Extension> Public Function CreateSelect2Data(Of OptionsType As ISelect2Option, T1 As Class)(Filter As PaginationFilter(Of T1, OptionsType), GroupBySelector As Func(Of OptionsType, String)) As Select2Data
            Return Filter.CreateSelect2Data(Of OptionsType)(Function(x As OptionsType) x.Text, Function(x As OptionsType) x.ID, GroupBySelector)
        End Function

    End Module
End Namespace