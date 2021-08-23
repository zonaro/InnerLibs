Imports System.Drawing
Imports InnerLibs.Printer
Imports InnerLibs.Printer.Command

Namespace EscPosCommands

    Friend Class Image
        Implements IImage

        Public Function Print(ByVal image As Drawing.Image, ByVal highDensity As Boolean) As Byte() Implements IImage.Print
            Dim list = New List(Of Byte)()
            Dim bmp = New Bitmap(image)

            ' Set character line spacing to n dotlines
            Dim send = "" & Microsoft.VisualBasic.ChrW(27) & Microsoft.VisualBasic.ChrW(51) & Microsoft.VisualBasic.ChrW(0)
            Dim data = New Byte(send.Length - 1) {}

            For i = 0 To send.Length - 1
                data(i) = Microsoft.VisualBasic.AscW(send(i))
            Next

            list.AddRange(data)
            data(0) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            data(1) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
            data(2) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0)) ' Clear

            ' ESC * m nL nH d1…dk   Select bitmap mode
            Dim escBmp As Byte() = {&H1B, &H2A, &H0, &H0, &H0}
            escBmp(2) = Microsoft.VisualBasic.AscW("!"c)
            'nL, nH
            escBmp(3) = CByte(bmp.Width Mod 256)
            escBmp(4) = CByte(bmp.Width / 256)

            ' Cycle picture pixel print
            ' High cycle
            For i = 0 To bmp.Height / 24 + 1 - 1
                ' Set the bitmap mode
                list.AddRange(escBmp)

                ' Width
                For j = 0 To bmp.Width - 1

                    For k = 0 To 24 - 1

                        If i * 24 + k < bmp.Height Then ' if within the BMP size
                            Dim pixelColor = bmp.GetPixel(j, i * 24 + k)
                            If Not (pixelColor.R > 160 AndAlso pixelColor.G > 160 AndAlso pixelColor.B > 160) Then data(k / 8) += CByte(128 >> k Mod 8)
                            If highDensity Then Continue For
                            If pixelColor.R = 0 Then data(k / 8) += CByte(128 >> k Mod 8)
                        End If
                    Next

                    ' Write data，24dots
                    list.AddRange(data)
                    data(0) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                    data(1) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0))
                    data(2) = Microsoft.VisualBasic.AscW(Microsoft.VisualBasic.Strings.ChrW(0)) ' Clear
                Next

                Dim data2 As Byte() = {&HA}
                list.AddRange(data2)
            Next

            list.AddRange(New Byte() {27, "@"c.ToByte()})
            Return list.ToArray()
        End Function

    End Class

End Namespace