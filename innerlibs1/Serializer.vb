Imports System.Web.Script.Serialization

' Your Organization serializer. Override the key methods for the desired date format. This example
' formats the date as MM/dd/yyyy
Public NotInheritable Class Json
    Inherits JavaScriptSerializer

    Friend Sub New(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss")
        MyBase.New()
        Me.RegisterConverters(New JavaScriptConverter() {New DateStringJSONConverter() With {.DateFormat = DateFormat}, New BytesConverter()})
    End Sub

    ''' <summary>
    ''' Converte um objeto para JSON
    ''' </summary>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    Public Shared Function SerializeJSON(Obj As Object, Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
        If IsNothing(Obj) Then
            Return ""
        Else
            Return New Json(DateFormat).Serialize(Obj)
        End If
    End Function

    Private Class DateStringJSONConverter
        Inherits JavaScriptConverter
        Private m_supportedTypes As List(Of Type)

        Public DateFormat As String = "yyyy-MM-dd hh:mm:ss"

        Public Sub New()
            m_supportedTypes = New List(Of Type)(1)
            m_supportedTypes.Add(GetType(DateTime))
        End Sub

        Public Overrides Function Deserialize(dictionary As IDictionary(Of String, Object), type As Type, serializer As JavaScriptSerializer) As Object
            Dim dt As DateTime = dictionary("DateString").ToString().To(Of Date)
            Return dt
        End Function

        Public Overrides Function Serialize(obj As Object, serializer As JavaScriptSerializer) As IDictionary(Of String, Object)
            Dim dt As DateTime = Convert.ToDateTime(obj)
            Dim dicDateTime As New Dictionary(Of String, Object)
            dicDateTime.Add("DateString", dt.ToString(DateFormat))
            dicDateTime.Add("DayOfYear", dt.DayOfYear)
            dicDateTime.Add("DayOfWeek", dt.ToString("dddd"))
            dicDateTime.Add("Year", dt.Year)
            dicDateTime.Add("Month", dt.Month)
            dicDateTime.Add("Day", dt.Day)
            dicDateTime.Add("Hour", dt.Hour)
            dicDateTime.Add("Minute", dt.Minute)
            dicDateTime.Add("Second", dt.Second)
            dicDateTime.Add("Millisecond", dt.Millisecond)
            dicDateTime.Add("Ticks", dt.Ticks)
            Return dicDateTime
        End Function

        Public Overrides ReadOnly Property SupportedTypes() As IEnumerable(Of Type)
            Get
                Return Me.m_supportedTypes
            End Get
        End Property

    End Class

    Private Class BytesConverter
        Inherits JavaScriptConverter
        Private m_supportedTypes As List(Of Type)

        Public Sub New()
            m_supportedTypes = New List(Of Type)(1)
            m_supportedTypes.Add(GetType(Byte()))

        End Sub

        Public Overrides Function Deserialize(dictionary As IDictionary(Of String, Object), type As Type, serializer As JavaScriptSerializer) As Object
            Dim dt As Byte() = Converter.To(Of Byte())(dictionary("Content"))
            Return dt
        End Function

        Public Overrides Function Serialize(obj As Object, serializer As JavaScriptSerializer) As IDictionary(Of String, Object)
            Dim bt = Converter.To(Of Byte())(obj)
            Dim dicByte As New Dictionary(Of String, Object)
            dicByte.Add("Size", bt.ToFileSizeString)
            dicByte.Add("Content", bt)
            Return dicByte
        End Function

        Public Overrides ReadOnly Property SupportedTypes() As IEnumerable(Of Type)
            Get
                Return Me.m_supportedTypes
            End Get
        End Property

    End Class

End Class