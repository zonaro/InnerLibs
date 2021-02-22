
Imports System
Imports System.Drawing
Imports System.Windows.Media
Public Class ColorEx



    Public MaxHue As Short = 360
    Public MaxSaturation As Short = 100

    Private _IsRGBDirty As Boolean = False
    Private _IsHSVDirty As Boolean = False
    Private _Color As Color = Color.FromArgb(255, 128, 128, 128)


    Public ReadOnly Property Color As Color
        Get
            If _IsRGBDirty Then
                HSV2RGB()
            End If
            Return _Color
        End Get

    End Property


    Public Property R As Byte
        Get

            If _IsRGBDirty Then
                HSV2RGB()
            End If

            Return (_Color.R)
        End Get
        Set(ByVal value As Byte)
            '_Color.R = value
            _IsHSVDirty = True
        End Set
    End Property

    Public Property G As Byte
        Get

            If _IsRGBDirty Then
                HSV2RGB()
            End If

            Return (_Color.G)
        End Get
        Set(ByVal value As Byte)
            '_Color.G = value
            _IsHSVDirty = True
        End Set
    End Property

    Public Property B As Byte
        Get

            If _IsRGBDirty Then
                HSV2RGB()
            End If

            Return (_Color.B)
        End Get
        Set(ByVal value As Byte)
            '_Color.B = value
            _IsHSVDirty = True
        End Set
    End Property

    Private _H As Short = 0

    Public Property H As Short
        Get

            If _IsHSVDirty Then
                RGB2HSV()
            End If

            Return (_H)
        End Get
        Set(ByVal value As Short)
            _H = CShort(((If(value < 0, 360, 0)) + (value Mod 360)))
            _IsRGBDirty = True
        End Set
    End Property

    Private _S As Byte = 0

    Public Property S As Byte
        Get

            If _IsHSVDirty Then
                RGB2HSV()
            End If

            Return (_S)
        End Get
        Set(ByVal value As Byte)

            If value >= 0 AndAlso value <= 100 Then
                _S = value
                _IsRGBDirty = True
            End If
        End Set
    End Property

    Private _V As Byte = 0

    Public Property V As Byte
        Get

            If _IsHSVDirty Then
                RGB2HSV()
            End If

            Return (_V)
        End Get
        Set(ByVal value As Byte)

            If value >= 0 AndAlso value <= 100 Then
                _V = value
                _IsRGBDirty = True
            End If
        End Set
    End Property


    Public Function Clone() As ColorEx
        Return New ColorEx()
    End Function


    Private Sub RGB2HSV()
        Throw New NotImplementedException()
    End Sub

    Private Sub HSV2RGB()
        Throw New NotImplementedException()
    End Sub
End Class
