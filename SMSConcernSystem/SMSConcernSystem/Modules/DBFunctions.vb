
Imports System.Data.SQLite
Imports System.Data.DataSet
Module DBFunctions

    Public Sub DBQuery(completionBlock As Action(Of SQLiteConnection))
        Dim cnstr As String = String.Format("DataSource={0};Version=3;New=False;Compress=True;", smsSystemDB)
        Using cn As New SQLiteConnection(cnstr)
            cn.Open()
            completionBlock(cn)
        End Using
    End Sub

    Public Sub ExecuteQuery(sql As String, completionBlock As Action(Of Boolean))
        DBQuery(Sub(cn)
                    Using cmd As New SQLiteCommand(sql, cn)
                        Try
                            Debug.Print("Execute SQL: " & sql)
                            Debug.Print("Result: {0}", cmd.ExecuteNonQuery())
                            If Not completionBlock Is Nothing Then
                                completionBlock(True)
                            End If

                        Catch ex As Exception
                            Debug.Print("Error: {0}", ex.Message)
                            If Not completionBlock Is Nothing Then
                                completionBlock(False)
                            End If
                        End Try
                    End Using
                End Sub)
    End Sub

    Public Sub SelectData(sql As String, completionBlock As Action(Of SQLiteDataAdapter))
        DBQuery(Sub(cn)
                    Debug.Print("Select SQL: " & sql)
                    Using da As New SQLiteDataAdapter(sql, cn)
                        completionBlock(da)
                    End Using
                End Sub)
    End Sub

    Public Sub SelectQuery(sql As String, completionBlock As Action(Of List(Of Dictionary(Of String, String))))
        DBQuery(Sub(cn)
                    Dim records As New List(Of Dictionary(Of String, String))
                    Debug.Print("Select SQL: " & sql)
                    Using cmd As New SQLiteCommand(sql, cn)
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

                    completionBlock(records)
                End Sub)
    End Sub
    Public Sub CreateAdminAccount()
        Dim tableQuery As String = "CREATE TABLE `tbl_admin` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`usrn`	TEXT NOT NULL," & _
                                    "`pssw`	TEXT NOT NULL," & _
                                    "`email`	TEXT NOT NULL" & _
                                    ");"
        Dim adminQuery As String = "INSERT INTO `tbl_admin` VALUES (NULL, 'admin', 'password', 'admin@password.com')"

        ExecuteQuery(tableQuery,
                     Sub(createdTable)
                         Debug.Print("Created : {0}", createdTable.ToString)
                         If (createdTable) Then
                             ExecuteQuery(adminQuery, Nothing)
                         Else
                             MsgBox("Failed to create Table", vbExclamation)
                         End If
                     End Sub)

    End Sub

    Public Sub CreateContactsTable()
        Dim tableQuery As String = "CREATE TABLE `tbl_contacts` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`student_id`	TEXT NOT NULL," & _
                                    "`mobile_number`	TEXT NOT NULL," & _
                                    "`first_name`	TEXT NOT NULL," & _
                                    "`last_name`	TEXT NOT NULL, " & _
                                    "`course`	TEXT NOT NULL, " & _
                                    "`year_section`	TEXT NOT NULL, " & _
                                    "`gender`	TEXT NOT NULL, " & _
                                    "`date_of_birth`	TEXT NOT NULL, " & _
                                    "`address`	TEXT NOT NULL, " & _
                                    "`email`	TEXT NOT NULL, " & _
                                    "`date_registered`	TEXT NOT NULL" & _
                                    ");"
        ExecuteQuery(tableQuery,
                     Sub(createdTable)
                         Debug.Print("Created : {0}", createdTable.ToString)
                     End Sub)
    End Sub

    Public Sub CreateInboxTable()
        Dim tableQuery As String = "CREATE TABLE `tbl_inbox` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`contact_id`	INTEGER NOT NULL," & _
                                    "`message_content`	TEXT NOT NULL," & _
                                    "`sender_number`	TEXT NOT NULL," & _
                                    "`date_received`	TEXT NOT NULL" & _
                                ");"
        ExecuteQuery(tableQuery,
                     Sub(createdTable)
                         Debug.Print("Created : {0}", createdTable.ToString)
                     End Sub)

    End Sub

    Public Sub CreateRawInboxTable()
        Dim tableQuery As String = "CREATE TABLE `tbl_raw_inbox` (" & _
                                    "`ID`	INTEGER PRIMARY KEY AUTOINCREMENT," & _
                                    "`sender`	TEXT NOT NULL," & _
                                    "`message`	TEXT NOT NULL," & _
                                    "`date_sent`	TEXT NOT NULL" & _
                                ");"
        ExecuteQuery(tableQuery,
                     Sub(createdTable)
                         Debug.Print("Created : {0}", createdTable.ToString)
                     End Sub)

    End Sub

    Public Sub Login(usrn As String, pssw As String, completionBlock As Action(Of List(Of Dictionary(Of String, String))))
        Dim loginQuery As String = String.Format("SELECT * FROM `tbl_admin` WHERE usrn = '{0}' And pssw = '{1}'", usrn, pssw)

        SelectQuery(loginQuery,
                    Sub(dictionaries)
                        completionBlock(dictionaries)
                    End Sub)
    End Sub

End Module
