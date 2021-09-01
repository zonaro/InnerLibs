Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices


Public Module AsciiArt


    <Extension()>
    Public Function ToAsciiArt(ByVal image As Bitmap, ByVal ratio As Integer) As String
        image = image.Negative()
        Dim toggle As Boolean = False

        Dim sb As StringBuilder = New StringBuilder()
        Dim h As Integer = 0

        While h < image.Height
            Dim w As Integer = 0

            While w < image.Width
                Dim pixelColor As Color = image.GetPixel(w, h)
                Dim red, green, blue As Integer
                red = ((pixelColor.R.ToInteger() + pixelColor.G.ToInteger() + pixelColor.B.ToInteger()) / 3)
                green = red
                blue = green
                Dim grayColor As Color = Color.FromArgb(red, green, blue)

                If Not toggle Then
                    Dim index As Integer = (grayColor.R * 10) / 255
                    sb.Append(asciiChars(index))
                End If

                w += ratio
            End While

            If Not toggle Then
                sb.AppendLine()
                toggle = True
            Else
                toggle = False
            End If

            h += ratio
        End While

        Return sb.ToString()
    End Function

    Private asciiChars As String() = {"#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " "}

    <Extension()>
    Public Function ToAsciiArt(ByVal sourceBitmap As Bitmap, ByVal pixelBlockSize As Integer, ByVal Optional colorCount As Integer = 0) As String
        Dim sourceData As BitmapData = sourceBitmap.LockBits(New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.[ReadOnly], PixelFormat.Format32bppArgb)
        Dim pixelBuffer As Byte() = New Byte(sourceData.Stride * sourceData.Height - 1) {}
        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)
        sourceBitmap.UnlockBits(sourceData)
        Dim asciiArt As StringBuilder = New StringBuilder()
        Dim avgBlue As Integer = 0
        Dim avgGreen As Integer = 0
        Dim avgRed As Integer = 0
        Dim offset As Integer = 0
        Dim rows As Integer = sourceBitmap.Height / pixelBlockSize
        Dim columns As Integer = sourceBitmap.Width / pixelBlockSize

        If colorCount > 0 Then
            colorCharacters = Generate.RandomWord(colorCount)
        End If

        For y As Integer = 0 To rows - 1

            For x As Integer = 0 To columns - 1
                avgBlue = 0
                avgGreen = 0
                avgRed = 0

                For pY As Integer = 0 To pixelBlockSize - 1

                    For pX As Integer = 0 To pixelBlockSize - 1
                        offset = y * pixelBlockSize * sourceData.Stride + x * pixelBlockSize * 4
                        offset += pY * sourceData.Stride
                        offset += pX * 4
                        Try
                            avgBlue += pixelBuffer(offset)
                            avgGreen += pixelBuffer(offset + 1)
                            avgRed += pixelBuffer(offset + 2)
                        Catch ex As Exception

                        End Try

                    Next
                Next

                avgBlue = avgBlue / (pixelBlockSize * pixelBlockSize)
                avgGreen = avgGreen / (pixelBlockSize * pixelBlockSize)
                avgRed = avgRed / (pixelBlockSize * pixelBlockSize)
                asciiArt.Append(GetColorCharacter(avgBlue, avgGreen, avgRed))
            Next

            asciiArt.Append(vbCrLf)
        Next

        Return asciiArt.ToString()
    End Function



    Private colorCharacters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

    Private Function GetColorCharacter(ByVal blue As Integer, ByVal green As Integer, ByVal red As Integer) As String
        Dim colorChar As String = ""
        Dim intensity As Integer = (blue + green + red) / 3 * (colorCharacters.Length - 1) / 255
        colorChar = colorCharacters.Substring(intensity, 1).ToUpper()
        colorChar += colorChar.ToLower()
        Return colorChar
    End Function




End Module

