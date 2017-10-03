

Imports InnerLibs.HtmlParser
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser
    Public MustInherit Class PseudoClass
        Private Shared s_Classes As Dictionary(Of String, PseudoClass) = LoadPseudoClasses()


        Public Overridable Function Filter(nodes As HtmlNodeCollection, parameter As String) As HtmlNodeCollection
            Dim la As New HtmlNodeCollection
            la.AddRange(nodes.Where(Function(i) CheckNode(i, parameter)))
            Return la
        End Function


        Protected MustOverride Function CheckNode(node As HtmlElement, parameter As String) As Boolean




        Public Shared Function GetPseudoClass(pseudoClass As String) As PseudoClass

            If Not s_Classes.ContainsKey(pseudoClass) Then

                Throw New NotSupportedException(Convert.ToString("Pseudo classe não suportada: ") & pseudoClass)
            End If


            Return s_Classes(pseudoClass)

        End Function


        Private Shared Function LoadPseudoClasses() As Dictionary(Of String, PseudoClass)

            Dim rt = New Dictionary(Of String, PseudoClass)(StringComparer.InvariantCultureIgnoreCase)

            ' Try to be resilient against Assembly.GetType() throwing an exception:
            ' - dynamic assemblies will fail
            ' - I have observed the non-dynamic assembly  "DotNetOpenAuth, Version=3.4.7.11121, Culture=neutral, PublicKeyToken=2780ccd10d57b246" also fail with no obvious way of knowing it will
            '  fall ahead of time.  For this reason, I have wrapped "GetTypes" in a try/catch block so that the code can continue on somewhat gracefully
            Dim tryGetTypes As Func(Of System.Reflection.Assembly, Type()) = Function(a)

                                                                                 If Not a.IsDynamic Then

                                                                                     Try
                                                                                         Return a.GetTypes()
                                                                                     Catch generatedExceptionName As Exception
                                                                                     End Try
                                                                                 End If
                                                                                 Return New Type() {}

                                                                             End Function


            Dim types = Reflection.Assembly.GetExecutingAssembly.GetTypes.Where(Function(i) Not i.IsAbstract AndAlso i.IsSubclassOf(GetType(PseudoClass)))


            types = types.OrderBy(Function(i) If(i.Assembly = GetType(PseudoClass).Assembly, 0, 1)).ToList()


            For Each type As Type In types
                Dim attr = type.GetCustomAttributes(GetType(PseudoClassNameAttribute), False).Cast(Of PseudoClassNameAttribute)().FirstOrDefault()

                rt.Add(attr.FunctionName, DirectCast(Activator.CreateInstance(type), PseudoClass))
            Next

            Return rt
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
