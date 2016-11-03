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

        If Not System.IO.Directory.Exists(smsSystemDB) Then
            SQLiteConnection.CreateFile(smsSystemDB)
            CreateAdminAccount()
        End If
    End Sub
End Module
