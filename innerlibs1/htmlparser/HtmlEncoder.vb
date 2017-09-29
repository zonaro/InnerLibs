
Imports System.IO
Imports System.Text

Namespace HtmlParser
    ''' <summary>
    ''' HTML 4 Entity coding routines
    ''' </summary>
    Friend MustInherit Class HtmlEncoder
        Public Shared Function EncodeValue(value As String) As String
            Return System.Net.WebUtility.HtmlEncode(value)
        End Function

        Public Shared Function DecodeValue(value As String) As String
            Return System.Net.WebUtility.HtmlDecode(value)
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
