Imports System.Reflection
Imports System.ComponentModel.Design               ' in system.design.dll
Imports System.ComponentModel
Imports System.Windows.Forms

'Imports System.Text.RegularExpressions

Public Enum NameServices
    None
    Automatic
    NameProvider
End Enum

' this is overkill, but keeps CA from whining
Public Class EditorCreatedEventArgs
    Inherits EventArgs

    Public Property EditorForm As Form

    Public Sub New(frm As Form)
        EditorForm = frm
    End Sub

End Class

Public Class NewItemCreatedEventArgs
    Inherits EventArgs

    Public Property ItemBaseName As String

    Public Sub New(name As String)
        MyBase.New()
        ItemBaseName = name
    End Sub

End Class

Public MustInherit Class EnhancedCollectionEditor
    Inherits CollectionEditor

    ' control names manifest for Intellisence
    Protected Friend Const CTRL_listbox As String = "listbox"

    Protected Friend Const CTRL_downButton As String = "downButton"
    Protected Friend Const CTRL_upButton As String = "upButton"
    Protected Friend Const CTRL_okButton As String = "okButton"
    Protected Friend Const CTRL_cancelButton As String = "cancelButton"
    Protected Friend Const CTRL_addButton As String = "addButton"
    Protected Friend Const CTRL_removeButton As String = "removeButton"
    Protected Friend Const CTRL_propertyBrowser As String = "propertyBrowser"

    ' the type associated with the property...
    ' could be a collection class or collection depending on what
    ' is exposed in the property getter with the Editor attribute
    Private myType As Type = Nothing

    ' this is the base Type the collection contains...the (Of T) part
    Private mycolBaseType As Type = Nothing

    ' as properties these can be set when inherited
    ' for max reusability
    Protected Friend Property FormCaption As String

    Protected Friend Property ShowPropGridHelp As Boolean
    Protected Friend Property AllowMultipleSelect As Boolean
    Protected Friend Property UsePropGridChangeEvent As Boolean
    Protected Friend Property NameService As NameServices

    Protected Friend ExcludedTypes As New List(Of Type)

    Private _propG As PropertyGrid = Nothing

    Private itemListBox As ListBox

    ' things here need to be done in a certain order.  we cannot hook to the
    ' prop grid event until we create the editor form.  But classes which
    '
    Protected Friend Event PropertyValueChanged(ByVal sender As Object,
                                                ByVal e As PropertyValueChangedEventArgs)

    Protected Friend Event EditorFormCreated(sender As Object, e As EditorCreatedEventArgs)

    Protected Friend Event NewItemCreated(sender As Object, e As NewItemCreatedEventArgs)

    Protected Friend ReadOnly Property BaseCollectionType As Type
        Get
            Return myType
        End Get
    End Property

    Protected Friend ReadOnly Property BaseItemType As Type
        Get
            Return mycolBaseType
        End Get
    End Property

    Protected Friend ReadOnly Property GetVersion As String
        Get
            Dim asm As Assembly = Assembly.GetExecutingAssembly
            Return asm.GetName.Version.ToString
        End Get
    End Property

#Region "  *** ctor and type identification *** "

    ''' <summary>
    ''' Creates a new collection editor
    ''' </summary>
    ''' <param name="t">The type of the collection for this editor to edit.</param>
    Public Sub New(t As Type)
        MyBase.New(t)

        'DisplayError(t.ToString)

        ' initialize defaults
        FormCaption = "Collection Class Editor"
        ShowPropGridHelp = True
        AllowMultipleSelect = True
        UsePropGridChangeEvent = False
        NameService = NameServices.None

        ' somebody, sometime might have need to
        ' do something somewhere with the original Type
        myType = t

        ' cant throw exceptions - VS/VB also wraps them
        If MyBase.CollectionType Is GetType(Microsoft.VisualBasic.Collection) Then
            DisplayError("VB Collection is not supported. Use a .NET Collection (always).")
            Exit Sub
        End If

        ' the actual type in the collection
        ' can return Object on poorly constructed collection classes
        mycolBaseType = MyBase.CollectionItemType

        ' check if it is Object ergo probably poorly typed or constructed
        If mycolBaseType Is GetType(Object) Then
            mycolBaseType = GetCollectionItemType(t)
        End If

        ' this works only for List(of T) (not even for Collection(Of T))
        'Dim xt As Type = t.GetGenericArguments(0)
        'DisplayError(String.Format("mytpe {0} basetype {1}", myType.ToString, mycolBaseType.ToString))

        If (mycolBaseType Is Nothing) Then
            DisplayError(
                String.Format("Underlying Type [{0}] must implement 'Item' as a PROPERTY." _
                                       & "{1}A NullReferenceException will result trying to use [{2}]",
                                       myType.ToString, Environment.NewLine,
                                       Me.GetType.Name))
            ' you have been warned
            Exit Sub
        End If
        If (mycolBaseType Is GetType(Object)) Then
            DisplayError(String.Format("No Editor available for [System.Object] in collection [{0}].",
                                       myType.ToString))
            Exit Sub
        End If

    End Sub

    ' If ITEM is defined as a function, it will work fine in code,
    ' but CollectionItemType wont find it on CollectionBase.  Technically, this is a FAIL,
    ' but check Props and Methods and tell them so
    Private Function GetCollectionItemType(t As Type) As Type
        ' create an instance of the Type passed

        ' get the Item(integer) property
        Dim pInfo As PropertyInfo = t.GetProperty("Item", New Type() {GetType(Integer)})
        If pInfo IsNot Nothing Then
            Return pInfo.PropertyType
        End If

        'maybe it is defined as a method
        Dim minfo As MethodInfo = t.GetMethod("Item")
        If minfo Is Nothing Then

            Return Nothing
        Else
            ' an Item Method will work and we could just scold, but with Collection<T>
            ' but the Property would be used; bugs/confusion could result when the code
            ' in an Item function is never/irregularly executed.  So, force them to use proper
            ' collection construction.

            'DisplayError(String.Format("Collection [{0}] should implement Item as a PROPERTY not Method",
            '                            MyBase.CollectionType.ToString))
            'Return minfo.ReturnType
            Return Nothing
        End If

    End Function

#End Region

#Region " **** (other) baseclass overrides *** "

    ' Override the NET standard function to limit the Types to show in the editor
    Protected NotOverridable Overrides Function CreateNewItemTypes() As Type()
        Dim ValidTypes As New List(Of Type)

        If mycolBaseType.IsAbstract Then

            ' start with all Types contained in the assembly mycolBaseType came from
            ' this may not be the same as Executing Assembly if the UIEditor is in a DLL:

            Dim allTypes As Type() = Assembly.GetAssembly(mycolBaseType).GetTypes()
            ' debug validation that it is polling the app assembly
            'DisplayError(String.Format("AsmName == {0}{1}allTypes.Count={2}{1}mycolBaseType=={3}", Assembly.GetAssembly(mycolBaseType).FullName,
            '                           System.Environment.NewLine, allTypes.Count.ToString,
            '                           mycolBaseType.ToString))

            ' go thru all the types returned
            For Each t As Type In allTypes
                ' if this Type derives from mycolBaseType and itself is
                ' not Abstract/MustInherit it can go in the list

                If t.IsSubclassOf(mycolBaseType) And (t.IsAbstract = False) Then
                    If (ExcludedTypes IsNot Nothing) AndAlso (ExcludedTypes.Contains(t) = False) Then
                        ValidTypes.Add(t)
                    End If
                End If
            Next

            'DisplayError("Types Count == " & ValidTypes.Count.ToString)

            ' return array to NET
            Return ValidTypes.ToArray()
        Else
            ' do nothing special - the baseType is not an abstract class
            Return MyBase.CreateNewItemTypes
        End If

    End Function

    ' do we want multiples selected
    Protected Overrides Function CanSelectMultipleInstances() As Boolean
        Return AllowMultipleSelect
    End Function

    Private collection As Object

    Public NotOverridable Overrides Function EditValue(context As System.ComponentModel.ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object
        collection = value

        Return MyBase.EditValue(context, provider, value)
    End Function

    '' collection form layout:
    ''http://www.dotnetframework.org/default.aspx/FX-1434/FX-1434/1@0/untmp/whidbey/REDBITS/ndp/fx/src/Designer/CompMod/System/ComponentModel/Design/CollectionEditor@cs/1/CollectionEditor@cs
    ''
    Protected NotOverridable Overrides Function CreateCollectionForm() As CollectionEditor.CollectionForm
        Dim EditorForm As CollectionForm = MyBase.CreateCollectionForm

        'based on the layout above, we can get the control refs we need by name
        ' the property grid is also tlpLayout.Controls(5) or EditorForm.Controls(0).Controls(5)
        ' see lines @1233

        _propG = CType(EditorForm.Controls("overArchingTableLayoutPanel").Controls("propertyBrowser"),
                        PropertyGrid)

        itemListBox = CType(EditorForm.Controls("overArchingTableLayoutPanel").Controls("listbox"),
              ListBox)

        ' use FindByName to get other controls, if you like.
        ' these are the exact names of key controls:
        ' listbox, downButton, upButton, okButton, cancelButton,
        ' addButton, removeButton, propertyBrowser.  Eg:
        'propGrid = GetControlByName("propertyBrowser", EditorForm.Controls)

        If _propG IsNot Nothing Then
            _propG.HelpVisible = ShowPropGridHelp
            If UsePropGridChangeEvent Then
                AddHandler _propG.PropertyValueChanged, AddressOf propGridValChanged
            End If
        End If

        ' not needed, form is sizeable...just proof you can change it
        ' I like to make it taller when I turn on the helppane
        EditorForm.Height += 40

        ' set the caption
        EditorForm.Text = FormCaption

        RaiseEvent EditorFormCreated(Me, New EditorCreatedEventArgs(DirectCast(EditorForm, Form)))

        ' return form ref to work with
        Return EditorForm

    End Function

    Protected Overrides Sub Finalize()
        If _propG IsNot Nothing Then
            _propG.Dispose()
        End If

        If itemListBox IsNot Nothing Then
            itemListBox.Dispose()
        End If

        MyBase.Finalize()
    End Sub

    Private Sub propGridValChanged(sender As Object, e As PropertyValueChangedEventArgs)
        RaiseEvent PropertyValueChanged(sender, e)
    End Sub

#End Region

#Region "  *** Naming Service procedures *** "

    Public Shared Function BaseNameFromType(ItemType As Type) As String
        Return BaseNameFromTypeName(ItemType.ToString)
    End Function

    Public Shared Function BaseNameFromTypeName(ItemTypeName As String) As String
        Return ItemTypeName.Remove(0, ItemTypeName.LastIndexOf(".") + 1)
    End Function

    Protected NotOverridable Overrides Function CreateInstance(itemType As Type) As Object
        Dim basename As String = BaseNameFromType(itemType)
        Dim obj As Object = MyBase.CreateInstance(itemType)

        ' use reflection to set the proeprty value
        '   and avoid late binding errors/warnings
        Dim pi As PropertyInfo
        pi = obj.GetType.GetProperty("Name")

        If (NameService <> NameServices.None) Then
            If (pi Is Nothing) Then
                DisplayError("[Name] property must be implemented to use the NamingService")
                Return obj
            End If

            If pi.CanWrite = False Then
                DisplayError("[Name] property cannot be Read-Only using NamingService")
                Return obj
            End If

        End If

        ' if we get here, NS active, PI good to go

        Dim newName As String
        'DisplayError(String.Format("base={0} post={1}", basename, newName))
        Select Case NameService
            Case NameServices.Automatic
                Dim nice As New NewItemCreatedEventArgs(basename)
                RaiseEvent NewItemCreated(Me, nice)
                newName = nice.ItemBaseName

                newName = GetNewName(newName)
                pi.SetValue(obj, newName, Nothing)

            Case NameServices.NameProvider
                newName = GetNameFromProvider(itemType)
                pi.SetValue(obj, newName, Nothing)
        End Select

        Return obj

    End Function

    ' issue a call to the instance method
    Private Function GetNameFromProvider(itemType As Type) As String
        Dim newName As String = ""

        If (collection IsNot Nothing) Then
            ' poll the collection first to see if it is a NameProvider
            If (collection.GetType.GetInterfaces().Contains(GetType(INameProvider))) Then
                newName = CType(collection, INameProvider).GetNewName(itemType.ToString)
            End If
        End If

        ' if either thing was false, newName will still be empty
        If String.IsNullOrEmpty(newName) Then
            If Me.Context.Instance IsNot Nothing Then
                ' collection does not support it, how about the property provider?
                If Me.Context.Instance.GetType.GetInterfaces().Contains(GetType(INameProvider)) Then
                    newName = CType(Me.Context.Instance, INameProvider).GetNewName(itemType.ToString)
                End If
            End If
        End If

        ' if still empty, FUBAR
        If String.IsNullOrEmpty(newName) Then
            ' NMF
            Dim colName As String
            If collection Is Nothing Then
                colName = "???"
            Else
                colName = collection.GetType.ToString
            End If
            DisplayError(String.Format("INameProvider not implemented on property provider: [{0}] {1} or Collection [{2}]",
                                                        (Context.Instance).GetType.ToString,
                                                        Environment.NewLine, colName))
            Return ""
        Else
            Return newName
        End If

    End Function

    ' the 10c solution: look at the listbox and use the highest value + 1
    ' assumes a proper Name implementation
    ' just OFFERS a unique name - if the user can edit, dupes can occur
    Private Function GetNewName(ByVal baseName As String) As String

        Return baseName & GetMaxListVal(baseName).ToString

    End Function

    ' at one time I had in mind to supply the Max Value when using INameProvider
    ' because the List COUNT is not the same as the List SEQUENCE.  In the end, I decided
    ' roll-your-own is roll-your-own
    Private Function GetMaxListVal(basename As String) As Integer
        Dim tmp As String
        Dim maxVal As Integer = 1
        Dim nVal As Integer

        If itemListBox IsNot Nothing Then

            If (itemListBox.Items.Count > 0) Then
                For n As Integer = 0 To itemListBox.Items.Count - 1
                    ' the basename length can vary (Ziggy vs Zoey), so parse chars
                    ' instead of the String.Remove extension
                    tmp = itemListBox.Items(n).ToString

                    ' using RegEx to get the numeric poststring
                    ' because LINQ will pick up any embedded numerals
                    ' e.g. Xoo1Bar2 ==> 12
                    ' ...use "\d+$" ??? seems to work either way
                    'Dim strVal As String = Regex.Match(tmp, "\d+").Value

                    ' LINQ -
                    Dim strVal As String = New String(tmp.ToCharArray().Where(
                                                      Function(c) Char.IsDigit(c)).ToArray())

                    If Integer.TryParse(strVal, nVal) Then
                        If nVal > maxVal Then
                            maxVal = nVal
                        End If
                    End If
                Next
                maxVal += 1
            End If
        Else
            ' could save a var and exit above so this only shows once
            ' but nagging might expedite a fix
            DisplayError("NamingService not available.")

        End If
        Return maxVal

    End Function

#End Region

    ' find a target control on the Ed Form by name
    Protected Friend Function GetControlByName(ctlName As String, ctls As Control.ControlCollection) As Control
        Dim rCtrl As Control = Nothing

        ' try overarching first
        rCtrl = ctls("overArchingTableLayoutPanel").Controls(ctlName)

        If rCtrl Is Nothing Then
            ' they might be looking for add/remove button
            rCtrl = ctls("addRemoveTableLayoutPanel").Controls(ctlName)
        End If

        If rCtrl Is Nothing Then
            ' they might be looking for ok/cancel button
            rCtrl = ctls("okCancelTableLayoutPanel").Controls(ctlName)
        End If

        Return rCtrl

    End Function

    Private Const Caption As String = "Plutonix CollectionEditor"

    ' show a MsgBox because exceptions are a bad idea here
    ' VS/VB catch errors in the base editor for display as a dialog
    Protected Friend Sub DisplayError(msg As String)
        MessageBox.Show(msg, Caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    End Sub

End Class

' it seems that one way to prevent the 2 tiered naming
' (catcher and provider) would be to ask or perhaps
' register the collection as the provider
'
' In this case, the NameProvider call is issued first
'   to NuControl, who returns a XooBars ref
' The Editor then calls XooBars directly for the name.
'
' Fors the nested version,
' XooBars is queried for the provider, which returns
' Me
'
'It falls apart on nested collections though.
' NuControl->XooBars is easy enough because
'  the collection is essentially a sibling object
' XooBars->XooItem->ZItem is problematic
' because the all XooItems have the same typename
' so there is no way to know WHICH Item's SubCollection
' is being tickled.
'

Public Interface INameProvider

    Function GetNewName(typeName As String) As String

End Interface