Imports System.Dynamic
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Xml

Namespace JsonReader

    Public Class JsonReader
        Inherits DynamicObject

        Private Enum JsonTypeEnum
            [string]
            [number]
            [boolean]
            [object]
            [array]
            [null]
        End Enum

        Public Shared Function Parse(ByVal json As String) As Object
            Return Parse(json, Encoding.Unicode)
        End Function

        Public Shared Function Deserialize(ByVal json As String) As Object
            Return Parse(json, Encoding.Unicode)
        End Function

        Public Shared Function Deserialize(Of T)(ByVal json As String) As T
            Return CType(Parse(json, Encoding.Unicode), T)
        End Function

        Public Shared Function Parse(Of T)(ByVal json As String) As T
            Return CType(Parse(json, Encoding.Unicode), T)
        End Function

        Public Shared Function Parse(Of T)(ByVal json As String, ByVal Encoding As Encoding) As T
            Return CType(Parse(json, Encoding), T)
        End Function

        Public Shared Function Parse(ByVal json As String, ByVal encoding As Encoding) As Object
            If json.IsNotBlank Then
                Try
                    Using reader = JsonReaderWriterFactory.CreateJsonReader(encoding.GetBytes(json), XmlDictionaryReaderQuotas.Max)
                        Return ToValue(XElement.Load(reader))
                    End Using
                Catch ex As Exception
                    Return json
                End Try
            End If
            Return Nothing
        End Function

        Public Shared Function Parse(ByVal stream As Stream) As Object
            Using reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max)
                Return ToValue(XElement.Load(reader))
            End Using
        End Function

        Public Shared Function Parse(ByVal stream As Stream, ByVal encoding As Encoding) As Object
            Using reader = JsonReaderWriterFactory.CreateJsonReader(stream, encoding, XmlDictionaryReaderQuotas.Max, Function(__)
                                                                                                                     End Function)
                Return ToValue(XElement.Load(reader))
            End Using
        End Function

        Public Shared Function Serialize(ByVal obj As Object) As String
            Return CreateJsonString(New XStreamingElement("root", CreateTypeAttr(GetJsonType(obj)), CreateJsonNode(obj)))
        End Function

        Private Shared Function ToValue(ByVal element As XElement) As Object
            Dim type = CType([Enum].Parse(GetType(JsonTypeEnum), element.Attribute("type").Value), JsonTypeEnum)

            Select Case type
                Case JsonTypeEnum.boolean
                    Return CBool(element)
                Case JsonTypeEnum.number
                    Return CDbl(element)
                Case JsonTypeEnum.string
                    Return CStr(element)
                Case JsonTypeEnum.object, JsonTypeEnum.array
                    Return New JsonReader(element, type)
                Case Else
                    Return Nothing
            End Select
        End Function

        Private Shared Function GetJsonType(ByVal obj As Object) As JsonTypeEnum
            If obj Is Nothing Then Return JsonTypeEnum.null

            Select Case Type.GetTypeCode(obj.[GetType]())
                Case TypeCode.Boolean
                    Return JsonTypeEnum.boolean
                Case TypeCode.String, TypeCode.Char, TypeCode.DateTime
                    Return JsonTypeEnum.string
                Case TypeCode.Int16, TypeCode.Int32, TypeCode.Int64, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal, TypeCode.SByte, TypeCode.Byte
                    Return JsonTypeEnum.number
                Case TypeCode.Object
                    Return If((TypeOf obj Is IEnumerable), JsonTypeEnum.array, JsonTypeEnum.object)
                Case Else
                    Return JsonTypeEnum.null
            End Select
        End Function

        Private Shared Function CreateTypeAttr(ByVal type As JsonTypeEnum) As XAttribute
            Return New XAttribute("type", type.ToString())
        End Function

        Private Shared Function CreateJsonNode(ByVal obj As Object) As Object
            Dim type = GetJsonType(obj)

            Select Case type
                Case JsonTypeEnum.string, JsonTypeEnum.number
                    Return obj
                Case JsonTypeEnum.boolean
                    Return obj.ToString().ToLower()
                Case JsonTypeEnum.object
                    Return CreateXObject(obj)
                Case JsonTypeEnum.array
                    Return CreateXArray(TryCast(obj, IEnumerable))
                Case Else
                    Return Nothing
            End Select
        End Function

        Private Shared Function CreateXArray(Of T As IEnumerable)(ByVal obj As T) As IEnumerable(Of XStreamingElement)
            Return obj.Cast(Of Object)().[Select](Function(o) New XStreamingElement("item", CreateTypeAttr(GetJsonType(o)), CreateJsonNode(o)))
        End Function

        Private Shared Function CreateXObject(ByVal obj As Object) As IEnumerable(Of XStreamingElement)

            Return obj.[GetType]().GetProperties(BindingFlags.[Public] Or BindingFlags.Instance).Where(Function(x) Not x.HasAttribute(Of ScriptIgnoreAttribute)).[Select](Function(pi) New With {Key .Name = pi.Name, Key .Value = pi.GetValue(obj, Nothing)
            }).[Select](Function(a) New XStreamingElement(a.Name, CreateTypeAttr(GetJsonType(a.Value)), CreateJsonNode(a.Value)))
        End Function

        Private Shared Function CreateJsonString(ByVal element As XStreamingElement) As String
            Using ms = New MemoryStream()

                Using writer = JsonReaderWriterFactory.CreateJsonWriter(ms, Encoding.Unicode)
                    element.WriteTo(writer)
                    writer.Flush()
                    Return Encoding.Unicode.GetString(ms.ToArray())
                End Using
            End Using
        End Function

        ReadOnly xml As XElement
        ReadOnly jsonType As JsonTypeEnum

        Public Sub New()
            xml = New XElement("root", CreateTypeAttr(JsonTypeEnum.object))
            jsonType = JsonTypeEnum.object
        End Sub

        Private Sub New(ByVal element As XElement, ByVal type As JsonTypeEnum)
            Debug.Assert(type = JsonTypeEnum.array OrElse type = JsonTypeEnum.object)
            xml = element
            jsonType = type
        End Sub

        Public ReadOnly Property IsObject As Boolean
            Get
                Return jsonType = JsonTypeEnum.object
            End Get
        End Property

        Public ReadOnly Property IsArray As Boolean
            Get
                Return jsonType = JsonTypeEnum.array
            End Get
        End Property

        Public Function IsDefined(ByVal name As String) As Boolean
            Return IsObject AndAlso (xml.Element(name) IsNot Nothing)
        End Function

        Public Function IsDefined(ByVal index As Integer) As Boolean
            Return IsArray AndAlso (xml.Elements().ElementAtOrDefault(index) IsNot Nothing)
        End Function

        Public Function Delete(ByVal name As String) As Boolean
            Dim elem = xml.Element(name)

            If elem IsNot Nothing Then
                elem.Remove()
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Delete(ByVal index As Integer) As Boolean
            Dim elem = xml.Elements().ElementAtOrDefault(index)

            If elem IsNot Nothing Then
                elem.Remove()
                Return True
            Else
                Return False
            End If
        End Function

        Public Function Deserialize(Of T)() As T
            Return CType(Deserialize(GetType(T)), T)
        End Function

        Private Function Deserialize(ByVal type As Type) As Object
            Return If((IsArray), DeserializeArray(type), DeserializeObject(type))
        End Function

        Private Function DeserializeValue(ByVal element As XElement, ByVal elementType As Type) As Object
            Dim value = ToValue(element)

            If TypeOf value Is JsonReader Then
                value = (CType(value, JsonReader)).Deserialize(elementType)
            End If

            Return Convert.ChangeType(value, elementType)
        End Function

        Private Function DeserializeObject(ByVal targetType As Type) As Object
            Dim result = Activator.CreateInstance(targetType)
            Dim dict = targetType.GetProperties(BindingFlags.[Public] Or BindingFlags.Instance).Where(Function(p) p.CanWrite).ToDictionary(Function(pi) pi.Name, Function(pi) pi)

            For Each item In xml.Elements()
                Dim propertyInfo As PropertyInfo
                If Not dict.TryGetValue(item.Name.LocalName, propertyInfo) Then Continue For
                Dim value = DeserializeValue(item, propertyInfo.PropertyType)
                propertyInfo.SetValue(result, value, Nothing)
            Next

            Return result
        End Function

        Private Function DeserializeArray(ByVal targetType As Type) As Object
            If targetType.IsArray Then
                Dim elemType = targetType.GetElementType()
                Dim array As Object = array.CreateInstance(elemType, xml.Elements().Count())
                Dim index = 0

                For Each item In xml.Elements()
                    array(Math.Min(System.Threading.Interlocked.Increment(index), index - 1)) = DeserializeValue(item, elemType)
                Next

                Return array
            Else
                Dim elemType = targetType.GetGenericArguments()(0)
                Dim list As Object = Activator.CreateInstance(targetType)

                For Each item In xml.Elements()
                    list.Add(DeserializeValue(item, elemType))
                Next

                Return list
            End If
        End Function

        Public Overrides Function TryInvoke(ByVal binder As InvokeBinder, ByVal args As Object(), <Out> ByRef result As Object) As Boolean
            result = If((IsArray), Delete(CInt(args(0))), Delete(CStr(args(0))))
            Return True
        End Function

        Public Overrides Function TryInvokeMember(ByVal binder As InvokeMemberBinder, ByVal args As Object(), <Out> ByRef result As Object) As Boolean
            If args.Length > 0 Then
                result = Nothing
                Return False
            End If

            result = IsDefined(binder.Name)
            Return True
        End Function

        Public Overrides Function TryConvert(ByVal binder As ConvertBinder, <Out> ByRef result As Object) As Boolean
            If binder.Type = GetType(IEnumerable) OrElse binder.Type = GetType(Object()) Then
                Dim ie = If((IsArray), xml.Elements().[Select](Function(x) ToValue(x)), xml.Elements().[Select](Function(x) CType(New KeyValuePair(Of String, Object)(x.Name.LocalName, ToValue(x)), Object)))
                result = If((binder.Type = GetType(Object())), ie.ToArray(), ie)
            Else
                result = Deserialize(binder.Type)
            End If

            Return True
        End Function

        Private Function TryGet(ByVal element As XElement, <Out> ByRef result As Object) As Boolean
            If element Is Nothing Then
                result = Nothing
                Return False
            End If

            result = ToValue(element)
            Return True
        End Function

        Public Overrides Function TryGetIndex(ByVal binder As GetIndexBinder, ByVal indexes As Object(), <Out> ByRef result As Object) As Boolean
            Return If((IsArray), TryGet(xml.Elements().ElementAtOrDefault(CInt(indexes(0))), result), TryGet(xml.Element(CStr(indexes(0))), result))
        End Function

        Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder, <Out> ByRef result As Object) As Boolean
            Return If((IsArray), TryGet(xml.Elements().ElementAtOrDefault(Integer.Parse(binder.Name)), result), TryGet(xml.Element(binder.Name), result))
        End Function

        Private Function TrySet(ByVal name As String, ByVal value As Object) As Boolean
            Dim type = GetJsonType(value)
            Dim element = xml.Element(name)

            If element Is Nothing Then
                xml.Add(New XElement(name, CreateTypeAttr(type), CreateJsonNode(value)))
            Else
                element.Attribute("type").Value = type.ToString()
                element.ReplaceNodes(CreateJsonNode(value))
            End If

            Return True
        End Function

        Private Function TrySet(ByVal index As Integer, ByVal value As Object) As Boolean
            Dim type = GetJsonType(value)
            Dim e = xml.Elements().ElementAtOrDefault(index)

            If e Is Nothing Then
                xml.Add(New XElement("item", CreateTypeAttr(type), CreateJsonNode(value)))
            Else
                e.Attribute("type").Value = type.ToString()
                e.ReplaceNodes(CreateJsonNode(value))
            End If

            Return True
        End Function

        Public Overrides Function TrySetIndex(ByVal binder As SetIndexBinder, ByVal indexes As Object(), ByVal value As Object) As Boolean
            Return If((IsArray), TrySet(CInt(indexes(0)), value), TrySet(CStr(indexes(0)), value))
        End Function

        Public Overrides Function TrySetMember(ByVal binder As SetMemberBinder, ByVal value As Object) As Boolean
            Return If((IsArray), TrySet(Integer.Parse(binder.Name), value), TrySet(binder.Name, value))
        End Function

        Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
            Return If((IsArray), xml.Elements().[Select](Function(x, i) i.ToString()), xml.Elements().[Select](Function(x) x.Name.LocalName))
        End Function

        Public Overrides Function ToString() As String
            For Each elem In xml.Descendants().Where(Function(x) x.Attribute("type").Value = "null")
                elem.RemoveNodes()
            Next

            Return CreateJsonString(New XStreamingElement("root", CreateTypeAttr(jsonType), xml.Elements()))
        End Function

    End Class

End Namespace