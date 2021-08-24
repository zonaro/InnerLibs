Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Printer
    Friend Module PrinterExtensions

        <Extension()>
        Public Function ToByte(ByVal c As Char) As Byte
            Return Microsoft.VisualBasic.AscW(c)
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As [Enum]) As Byte
            Return Convert.ToInt16(c)
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As Short) As Byte
            Return c
        End Function

        <Extension()>
        Public Function ToByte(ByVal c As Integer) As Byte
            Return c
        End Function

        <Extension()>
        Public Function AddBytes(ByVal bytes As Byte(), ByVal pAddBytes As Byte()) As Byte()
            If pAddBytes Is Nothing Then Return bytes
            Dim list = New List(Of Byte)()
            list.AddRange(bytes)
            list.AddRange(pAddBytes)
            Return list.ToArray()
        End Function

        <Extension()>
        Public Function AddTextBytes(ByVal bytes As Byte(), ByVal value As String, Encoding As Encoding) As Byte()
            If String.IsNullOrEmpty(value) Then Return bytes
            Dim list = New List(Of Byte)()
            list.AddRange(bytes)
            list.AddRange(If(Encoding, Encoding.Default).GetBytes(value))
            Return list.ToArray()
        End Function

        <Extension()>
        Public Function AddLF(ByVal bytes As Byte(), Optional Encoding As Encoding = Nothing) As Byte()
            Return bytes.AddTextBytes(Microsoft.VisualBasic.Constants.vbLf, Encoding)
        End Function

        <Extension()>
        Public Function AddCrLF(ByVal bytes As Byte(), Optional Encoding As Encoding = Nothing) As Byte()
            Return bytes.AddTextBytes(Microsoft.VisualBasic.Constants.vbCrLf, Encoding)
        End Function

        <Extension()>
        Public Function IsNullOrEmpty(ByVal value As String) As Boolean
            Return String.IsNullOrEmpty(value)
        End Function

    End Module
End Namespace