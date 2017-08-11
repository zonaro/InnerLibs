Imports System.Collections.ObjectModel
Imports InnerLibs

''' <summary>
''' Combinação de varios dados de RPG que podem ser rolados ao mesmo tempo
''' </summary>
Public Class DiceRoller
    Inherits List(Of Dice)

    ''' <summary>
    ''' Cria uma nova combinação de Dados
    ''' </summary>
    ''' <param name="Dices">Dados de RPG</param>
    Public Sub New(ParamArray Dices() As Dice)
        Me.AddRange(Dices)
    End Sub

    ''' <summary>
    ''' Cria uma nova combinação de Dados
    ''' </summary>
    ''' <param name="DiceRollers">Dados de RPG</param>
    Public Sub New(ParamArray DiceRollers() As DiceRoller)
        For Each d In DiceRollers
            Me.AddRange(d.ToArray)
        Next
    End Sub

    ''' <summary>
    ''' Cria uma nova combinação de Dados
    ''' </summary>
    ''' <param name="ListOfDices">Lista de dados de RPG</param>
    Public Sub New(ParamArray ListOfDices() As List(Of Dice))
        Me.AddRange(ListOfDices.ToArray)
    End Sub

    ''' <summary>
    ''' Cria uma nova combinação de novos dados a criados a partir de varios tipos
    ''' </summary>
    ''' <param name="DiceType">Tipos Dados de RPG</param>
    Public Sub New(ParamArray DiceType() As DiceType)
        For Each d In DiceType
            Me.Add(New Dice(d))
        Next
    End Sub

    ''' <summary>
    ''' Retorna a soma de todos os valores dos dados
    ''' </summary>
    ''' <returns>Integer</returns>
    Public ReadOnly Property Value As Integer
        Get
            Return _value
        End Get
    End Property

    Private _value As Integer

    ''' <summary>
    ''' Rola todos os dados e retorna a soma de seus valores
    ''' </summary>
    ''' <returns>Retorna a soma de todos os valores dos dados após a rolagem</returns>
    Public Function Roll() As Integer
        _value = 0
        For Each d In Me
            _value.Increment(d.Roll)
        Next
        Return Value
    End Function

    ''' <summary>
    ''' Combina um dado com DiceRoller
    ''' </summary>
    ''' <param name="Combo">Dado 1</param>
    ''' <param name="Dice">Dado 2</param>
    ''' <returns></returns>
    Public Shared Operator +(Combo As DiceRoller, Dice As Dice) As DiceRoller
        Dim s As New DiceRoller(Combo.ToArray)
        s.Add(Dice)
        Return s
    End Operator

    ''' <summary>
    ''' Combina um dado com DiceRoller
    ''' </summary>
    ''' <param name="Combo">Dado 1</param>
    ''' <param name="Dice">Dado 2</param>
    ''' <returns></returns>
    Public Shared Operator +(Dice As Dice, Combo As DiceRoller) As DiceRoller
        Return Combo + Dice
    End Operator

    ''' <summary>
    ''' Combina um dado com DiceRoller
    ''' </summary>
    ''' <param name="Combo1">Combo de Dados 1</param>
    ''' <param name="Combo2">Combo de Dados 2</param>
    ''' <returns></returns>
    Public Shared Operator +(Combo1 As DiceRoller, Combo2 As DiceRoller) As DiceRoller
        Return New DiceRoller(Combo1, Combo2)
    End Operator

End Class

''' <summary>
''' Dado de RPG
''' </summary>
Public Class Dice

    ''' <summary>
    ''' Combina 2 dados em um DiceRoller
    ''' </summary>
    ''' <param name="Dice1">Dado 1</param>
    ''' <param name="Dice2">Dado 2</param>
    ''' <returns></returns>
    Public Shared Operator +(Dice1 As Dice, Dice2 As Dice) As DiceRoller
        Return New DiceRoller(Dice1, Dice2)
    End Operator

    ''' <summary>
    ''' Tipo do dado
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Type As DiceType
        Get
            For Each i In GetEnumValues(Of DiceType)()
                If i = Faces.Count Then Return i
            Next
            Return DiceType.Custom
        End Get
    End Property

    ''' <summary>
    ''' Indica se o dado é um dado com faces customizadas
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsCustom As Boolean
        Get
            Return Me.Type = DiceType.Custom
        End Get
    End Property

    ''' <summary>
    ''' Verifica se o dado possui algum lado viciado
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsVicious As Boolean
        Get
            Return Faces.Where(Function(x) x.IsVicious = True).Count > 0
        End Get
    End Property

    ''' <summary>
    ''' Valor atual deste dado
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Value As Integer
        Get
            Return _value
        End Get
    End Property

    Private _value As Integer = 0

    ''' <summary>
    ''' Numero de vezes que este dado já foi rolado
    ''' </summary>
    ''' <returns>Integer</returns>
    Public ReadOnly Property RolledTimes As Integer
        Get
            Return _rolledtimes
        End Get
    End Property

    Private _rolledtimes As Integer = 0

    ''' <summary>
    ''' Rola o dado e retorna seu valor
    ''' </summary>
    ''' <returns>Integer</returns>
    Public Function Roll() As Integer
        _rolledtimes.Increment
        Dim numfaces As New List(Of Integer)
        For Each f In Faces
            For index = 1 To f.Weight
                numfaces.Add(f.Value)
            Next
        Next
        _value = numfaces.GetRandomItem
        Return Value
    End Function

    ''' <summary>
    ''' Altera o Peso de uma face do dado (Vicia o dado)
    ''' </summary>
    ''' <param name="FaceNumber">Numero da face</param>
    ''' <param name="Weight">Peso da Face</param>
    Public Sub SetFaceWeight(FaceNumber As Integer, Weight As Integer)
        FaceNumber = FaceNumber.LimitRange(1, Faces.Count)
        Faces(FaceNumber - 1)._weight = Weight
        ApplyPercent()
    End Sub

    ''' <summary>
    ''' Normaliza o peso das faces do dado
    ''' </summary>
    Public Sub NormalizeWeight()
        For Each f As Face In _Faces
            f._weight = 1
        Next
    End Sub

    Private Function GetChancePercent(Face As Integer) As Integer
        Face.LimitRange(1, Faces.Count)
        Dim ptotal = 0
        For Each f In Faces
            ptotal.Increment(f.Weight)
        Next
        Return Mathematic.CalculatePercent(Faces(Face - 1).Weight, ptotal)
    End Function

    Private Sub ApplyPercent()
        For Each f In Faces
            f._weightpercent = GetChancePercent(f.Value)
        Next
    End Sub

    ''' <summary>
    ''' Faces do dado
    ''' </summary>
    ''' <returns>Um array com a cópia das faces do dado</returns>
    Public ReadOnly Property Faces As FrozenCollection(Of Face)

    ''' <summary>
    ''' Cria um novo dado de um tipo especifico
    ''' </summary>
    ''' <param name="Type">Tipo de dado</param>
    Public Sub New(Optional Type As DiceType = DiceType.D6)
        Me.New(Type.To(Of Integer))
    End Sub

    ''' <summary>
    ''' Cria um novo dado com um numero customizado de faces
    ''' </summary>
    ''' <param name="CustomFaces">Numero de faces do dado (Minimo de 2 faces)</param>
    Public Sub New(CustomFaces As Integer)
        CustomFaces = CustomFaces.SetMinValue(2)
        Dim f As New List(Of Face)
        For index = 1 To CustomFaces
            f.Add(New Face(index))
        Next
        Faces = New FrozenCollection(Of Face)(f)
        ApplyPercent()
        Me.Roll()
    End Sub

    ''' <summary>
    ''' Face de um dado. Pode ser viciada ou não
    ''' </summary>
    Class Face

        ''' <summary>
        ''' Valor Da Face (numero)
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Value As Integer

        ''' <summary>
        ''' Peso da face (vicia o dado)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Weight As Integer
            Get
                Return _weight
            End Get
        End Property

        Protected Friend _weight As Integer = 1

        ''' <summary>
        ''' Peso da face (vicia o dado)
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property WeightPercent As Integer
            Get
                Return _weightpercent
            End Get
        End Property

        Protected Friend _weightpercent As Integer = 1

        ''' <summary>
        ''' Valor que indica se a face está viciada
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsVicious As Boolean
            Get
                Return Weight > 1
            End Get
        End Property

        ''' <summary>
        ''' Cria uma nova Face
        ''' </summary>
        ''' <param name="FaceNumber">Numero da face</param>
        Public Sub New(FaceNumber As Integer)
            Me.Value = FaceNumber.SetMinValue(1)
        End Sub

    End Class

End Class

''' <summary>
''' Tipos de Dados
''' </summary>
Public Enum DiceType

    ''' <summary>
    ''' Dado customizado
    ''' </summary>
    Custom = 0

    ''' <summary>
    ''' Moeda
    ''' </summary>
    Coin = 2

    ''' <summary>
    ''' Dado de 4 Lados
    ''' </summary>
    D4 = 4

    ''' <summary>
    ''' Dado de 6 Lados
    ''' </summary>
    D6 = 6

    ''' <summary>
    ''' Dado de 8 Lados
    ''' </summary>
    D8 = 8

    ''' <summary>
    ''' Dado de 10 Lados
    ''' </summary>
    D10 = 10

    ''' <summary>
    ''' Dado de 12 Lados
    ''' </summary>
    D12 = 12

    ''' <summary>
    ''' Dado de 20 Lados
    ''' </summary>
    D20 = 20

    ''' <summary>
    ''' Dado de 100 Lados (Util para porcentagem)
    ''' </summary>
    D100 = 100

End Enum