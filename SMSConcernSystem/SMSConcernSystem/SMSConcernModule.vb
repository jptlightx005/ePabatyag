Imports System.Data.SQLite
Module SMSConcernModule
    Public myDocumentsFolder As String
    Public smsSystemFolder As String
    Public smsSystemDB As String

    Sub Main()
        myDocumentsFolder = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        smsSystemFolder = System.IO.Path.Combine(myDocumentsFolder, "ePabatyag")
        smsSystemDB = System.IO.Path.Combine(smsSystemFolder, "ePabatyag.db")

        CheckDB()

        Dim app As New System.Windows.Application
        app.Run(New LogInWindow)

    End Sub

    Public Sub CheckDB()
        If Not System.IO.Directory.Exists(smsSystemFolder) Then
            System.IO.Directory.CreateDirectory(smsSystemFolder)
        End If

        If Not System.IO.File.Exists(smsSystemDB) Then
            SQLiteConnection.CreateFile(smsSystemDB)
            CreateAdminAccount()
            CreateContactsTable()
        End If
    End Sub

    Public Function AllTrim(ByVal text As String) As String
        Return text.Trim
    End Function

    Public Function SQLInject(ByVal text As String) As String
        Return "'" & AllTrim(text).Replace("'", "''") & "'"
    End Function

    Public Function pList(enumerable As IEnumerable(Of String))
        Dim list As New List(Of String)(enumerable)

        Return String.Join(",", list.ToArray)
    End Function
End Module
