Imports System.Data.SQLite
Module SMSConcernModule
    Public myDocumentsFolder As String
    Public smsSystemFolder As String
    Public smsSystemDB As String
    Public smsSystemImages As String

    Public keywords As List(Of String)
    Public profanities As List(Of String)
    Sub Main()
        myDocumentsFolder = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        smsSystemFolder = System.IO.Path.Combine(myDocumentsFolder, "ePabatyag")
        smsSystemDB = System.IO.Path.Combine(smsSystemFolder, "ePabatyag.db")
        smsSystemImages = System.IO.Path.Combine(smsSystemFolder, "Contact Images")
        CheckDB()
        LoadSettings()
        LoadKeywords()
        LoadProfanities()
        Dim app As New System.Windows.Application
        If My.Settings.isLoggedIn Then
            app.Run(New MainWindow)
        Else
            app.Run(New LogInWindow)
        End If
    End Sub

    Public Sub LoadSettings()
        Debug.Print("Loaded Setting")
        Debug.Print("Device port: {0}", My.Settings.smsDevicePort)
    End Sub

    Private Sub LoadKeywords()
        keywords = New List(Of String)
        keywords.Add("ICT")
        keywords.Add("SBM")
        keywords.Add("SOE")
        keywords.Add("IT")

        keywords.Add("Guidance")
        keywords.Add("Dean")
        keywords.Add("OSA")
        keywords.Add("Alumni")

        keywords.Add("Registrar")
        keywords.Add("Admin")
        keywords.Add("Records")
        keywords.Add("Others")
    End Sub

    Private Sub LoadProfanities()
        profanities = New List(Of String)
        Dim profTable = SelectQuery("SELECT profane_word FROM tbl_profanity")
        For Each word In profTable
            profanities.Add(word("profane_word"))
        Next
    End Sub

    Public Sub CheckDB()
        If Not System.IO.Directory.Exists(smsSystemFolder) Then
            System.IO.Directory.CreateDirectory(smsSystemFolder)
        End If

        If Not System.IO.Directory.Exists(smsSystemImages) Then
            System.IO.Directory.CreateDirectory(smsSystemImages)
        End If

        If Not System.IO.File.Exists(smsSystemDB) Then
            SQLiteConnection.CreateFile(smsSystemDB)
        End If
        CreateAdminAccount()
        CreateInboxTable()
        CreateRawInboxTable()
        CreateProfanityTable()
        CreateOutboxTable()
        'CreateSentTable()
    End Sub

    Public Function AllTrim(ByVal text As String) As String
        Return text.Trim
    End Function

    Public Function SQLInject(ByVal value) As String
        If TypeOf value Is String Then
            Debug.Print("Value '{0}' is a string", value)
            Return String.Format("'{0}'", AllTrim(value).Replace("'", "''"))
            'ElseIf TypeOf value Is Integer Then
        Else
            Debug.Print("Value {0} is an integer, probably", value)
            Return String.Format("{0}", value)
        End If


    End Function

    Public Function pList(enumerable As IEnumerable(Of String))
        Dim list As New List(Of String)(enumerable)

        Return String.Join(",", list.ToArray)
    End Function

    Public Function yesNoMsgBox(message As String) As Integer
        Return MsgBox(message, vbYesNo + vbQuestion)
    End Function
End Module
