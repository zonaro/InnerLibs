Public Class Reme

    Property Dialetos As New Dictionary(Of String, Dialeto)



End Class


Public Class Dialeto

    Property Iniciadores As IEnumerable(Of String)
    Property Conectores As IEnumerable(Of String)
    Property Objetos As IEnumerable(Of String)

End Class


Public MustInherit Class Personalidade




End Class


Public MustInherit Class Mensagem

    Sub New(Resposta As String)
        'processasearch
    End Sub

    Sub New(Resposta As String, Executar As EventArgs)


    End Sub

    Sub executa()

    End Sub

    Public Funcao As Func(Of String, Integer, Mensagem) = Nothing

    Public Resposta As Mensagem




End Class








