
Imports System.Data.SQLite
Imports System.Data.DataSet
Imports System.Text
Module DBFunctions

    Public Function DBQuery() As SQLiteConnection
        Dim cnstr As String = String.Format("DataSource={0};Version=3;New=False;Compress=True;", smsSystemDB)
        Dim cn = New SQLiteConnection(cnstr)
        cn.Open()
        Return cn
    End Function

    Public Function ExecuteQuery(sql As String) As Boolean
        Using cmd As New SQLiteCommand(sql, DBQuery())
            Try
                Debug.Print("Execute SQL: " & sql)
                Debug.Print("Result: {0}", cmd.ExecuteNonQuery())
                Return True

            Catch ex As Exception
                Debug.Print("Error: {0}", ex.Message)
                Return False
            End Try
        End Using
    End Function

    Public Function SelectData(sql As String) As SQLiteDataAdapter
        Debug.Print("Select SQL: " & sql)
        Dim da = New SQLiteDataAdapter(sql, DBQuery.ConnectionString)
        'Using da As New SQLiteDataAdapter(sql, DBQuery.ConnectionString)
        Debug.Print("Should be here: {0}", DBQuery.ConnectionString)
        Return da
        'End Using
    End Function

    Public Function SelectQuery(sql As String) As List(Of Dictionary(Of String, String))
        Dim records As New List(Of Dictionary(Of String, String))
        Debug.Print("Select SQL: " & sql)
        Using cmd As New SQLiteCommand(sql, DBQuery)
            Using rs As SQLiteDataReader = cmd.ExecuteReader()
                While rs.Read
                    Dim record As New Dictionary(Of String, String)
                    For i As Integer = 0 To rs.FieldCount - 1
                        record.Add(rs.GetName(i), rs.GetValue(i))
                    Next
                    records.Add(record)
                End While
            End Using
        End Using

        Return records
    End Function

    Public Function TableExists(tablename As String) As Boolean
        Dim query = String.Format("SELECT name FROM sqlite_master WHERE type='table' AND name = '{0}'", tablename)
        Dim result = SelectQuery(query)
        Return result.Count > 0
    End Function
    Public Sub CreateAdminAccount()
        Debug.Print("Attempting to create table tbl_raw_inbox")
        If TableExists("tbl_admin") Then
            Debug.Print("Table exists!")
            Return
        End If
        Dim tableQuery As String = "CREATE TABLE `tbl_admin` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`usrn`	TEXT NOT NULL," & _
                                    "`pssw`	TEXT NOT NULL," & _
                                    "`email`	TEXT NOT NULL" & _
                                    ");"
        Dim adminQuery As String = "INSERT INTO `tbl_admin` VALUES (NULL, 'admin', 'password', 'admin@password.com')"

        If (ExecuteQuery(tableQuery)) Then
            Debug.Print("Admin table created!")
            Debug.Print("Added Admin: {0}", ExecuteQuery(adminQuery).ToString)
        Else
            MsgBox("Failed to create Table", vbExclamation)
        End If
    End Sub

    Public Sub CreateInboxTable()
        Debug.Print("Attempting to create table tbl_raw_inbox")
        If TableExists("tbl_inbox") Then
            Debug.Print("Table exists!")
            Return
        End If
        Dim tableQuery As String = "CREATE TABLE `tbl_inbox` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`keyword`	TEXT NOT NULL," & _
                                    "`quality`	TEXT NOT NULL," & _
                                    "`timeliness`	TEXT NOT NULL," & _
                                    "`professionalism`	TEXT NOT NULL," & _
                                    "`message_content`	TEXT NOT NULL," & _
                                    "`mobile_number`	TEXT NOT NULL," & _
                                    "`date_received`	TEXT NOT NULL," & _
                                    "`is_removed`	INTEGER NOT NULL DEFAULT 0," & _
                                    "`is_read`	INTEGER NOT NULL DEFAULT 0" & _
                                ");"

        Debug.Print("Created Inbox: {0}", ExecuteQuery(tableQuery).ToString)

    End Sub

    Public Sub CreateRawInboxTable()
        Debug.Print("Attempting to create table tbl_raw_inbox")
        If TableExists("tbl_raw_inbox") Then
            Debug.Print("Table exists!")
            Return
        End If
        Dim tableQuery As String = "CREATE TABLE `tbl_raw_inbox` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`sender`	TEXT NOT NULL," & _
                                    "`message`	TEXT NOT NULL," & _
                                    "`date_sent`	TEXT NOT NULL" & _
                                ");"

        Debug.Print("Created Raw Inbox: {0}", ExecuteQuery(tableQuery).ToString)
    End Sub

    Public Sub CreateProfanityTable()
        Debug.Print("Attempting to create table tbl_profanity")
        If TableExists("tbl_profanity") Then
            Debug.Print("Table exists!")
            Return
        End If
        Dim tableQuery As String = "CREATE TABLE `tbl_profanity` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`profane_word`	TEXT NOT NULL," & _
                                    "`date_added`	TEXT NOT NULL" & _
                                ");"


        If (ExecuteQuery(tableQuery)) Then
            Debug.Print("Profanity table created!")
            Dim profanitiesQuery As New StringBuilder
            profanitiesQuery.Append("INSERT INTO tbl_profanity VALUES ")
            Dim newInsertString = Function(profaneWord As String) As String
                                      Dim values = String.Format("(NULL, '{0}', '{1}')", profaneWord, Now.ToString)
                                      Return values
                                  End Function
            profanitiesQuery.Append(newInsertString("fuck") & ", ")
            profanitiesQuery.Append(newInsertString("shit") & ", ")
            profanitiesQuery.Append(newInsertString("bitch") & ", ")
            profanitiesQuery.Append(newInsertString("gago") & ", ")
            profanitiesQuery.Append(newInsertString("gaga") & ", ")
            profanitiesQuery.Append(newInsertString("linti") & ", ")
            profanitiesQuery.Append(newInsertString("linte") & ", ")
            profanitiesQuery.Append(newInsertString("lente") & ", ")
            profanitiesQuery.Append(newInsertString("mango"))

            Debug.Print("Added profanities: {0}", ExecuteQuery(profanitiesQuery.ToString).ToString)
        Else
            MsgBox("Failed to create Table", vbExclamation)
        End If
    End Sub

    Public Function Login(usrn As String, pssw As String) As List(Of Dictionary(Of String, String))
        Dim loginQuery As String = String.Format("SELECT * FROM `tbl_admin` WHERE usrn = '{0}' And pssw = '{1}'", usrn, pssw)
        Return SelectQuery(loginQuery)
    End Function

End Module
