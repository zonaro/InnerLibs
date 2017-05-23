Imports System.Web.Script.Serialization

' Your Organization serializer. Override the key methods for the desired date format.
' This example formats the date as MM/dd/yyyy
Public Class JsonSerializer
    Inherits JavaScriptSerializer
    Public Sub New(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss")
        MyBase.New()
        Me.RegisterConverters(New JavaScriptConverter() {New DateStringJSONConverter() With {.DateFormat = DateFormat}})
    End Sub

    ''' <summary>
    ''' Converte um objeto para JSON
    ''' </summary>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    Default ReadOnly Property ToJSON(Obj As Object) As String
        Get
            Return Serialize(Obj)
        End Get
    End Property

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
End Class

' In your xhr layer, replace the standard Javascript serializer with your own.
' var javaScriptSerializer = new JavaScriptSerializer(); Don't use the standard serializer use below

'var javaScriptSerializer = new YourOrg_JavaScriptSerializer();
' Serialize your model data: javaScriptSerializer.Serialize(model.Data);