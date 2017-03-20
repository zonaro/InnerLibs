Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports IWshRuntimeLibrary

Public Module Shortcuts
    ''' <summary>
    ''' Cria atalhos em um diretõrio especifico
    ''' </summary>
    ''' <param name="Directory">Diretório de destino</param>
    ''' <param name="Name">nome do arquivo de atalho</param>
    ''' <param name="Arguments">Argumentos</param>
    ''' <param name="Target">Destino (se não especificado, aponta para a sua aplicação)</param>
    ''' <param name="Description">Descrição do atalho</param>
    ''' <param name="Icon">Icone do atalho</param>
    ''' <returns>TRUE se foi possivel criar o atalho, caso contrario, FALSE</returns>
    <Extension()>
    Public Function CreateShortcut(Directory As DirectoryInfo, Name As String, Optional Arguments As String = Nothing, Optional Target As String = Nothing, Optional Description As String = Nothing, Optional Icon As String = Nothing) As Boolean
        Try
            Dim location = Directory.FullName & "\" & Name.RemoveLastIf(".lnk") & ".lnk"
            Dim oShell As New WshShell
            Dim shortcut As IWshShortcut
            shortcut = oShell.CreateShortcut(location)
            With shortcut
                .TargetPath = If(Target, Application.ExecutablePath)
                .Description = Description
                .Arguments = Arguments
                .IconLocation = If(Icon, Target & ", 0")
                .Save()
            End With
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Module
