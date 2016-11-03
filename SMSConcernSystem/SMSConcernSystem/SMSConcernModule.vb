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

    Public Sub CreateAdminAccount()
        Dim tableQuery As String = "CREATE TABLE `tbl_admin` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`usrn`	TEXT NOT NULL," & _
                                    "`pssw`	TEXT NOT NULL," & _
                                    "`email`	TEXT NOT NULL" & _
                                    ");"
        Dim adminQuery As String = "INSERT INTO `tbl_admin` VALUES (NULL, 'admin', 'password', 'admin@password.com')"

        Dim cnstr As String = String.Format("DataSource={0};Version=3;New=False;Compress=True;", smsSystemDB)

        Using cn As New SQLiteConnection(cnstr)
            cn.Open()

            Using cmd As New SQLiteCommand(tableQuery, cn)
                cmd.ExecuteNonQuery()
            End Using

            Debug.Print(String.Format("Test: {0}", adminQuery))
            Using cmd As New SQLiteCommand(adminQuery, cn)
                cmd.ExecuteNonQuery()
            End Using

        End Using
        
    End Sub
End Module
