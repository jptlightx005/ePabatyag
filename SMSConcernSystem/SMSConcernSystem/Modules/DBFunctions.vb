
Imports System.Data.SQLite

Module DBFunctions

    Public Sub DBQuery(completionBlock As Func(Of SQLiteConnection, Integer))
        Dim cnstr As String = String.Format("DataSource={0};Version=3;New=False;Compress=True;", smsSystemDB)
        Using cn As New SQLiteConnection(cnstr)
            cn.Open()
            completionBlock(cn)
        End Using
    End Sub

    Public Sub ExecuteQuery(sql As String, completionBlock As Func(Of Boolean, Integer))
        DBQuery(Function(cn)
                    Using cmd As New SQLiteCommand(sql, cn)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SQLiteCommand(sql, cn)
                        cmd.ExecuteNonQuery()
                    End Using
                    Return 0
                End Function)
    End Sub

    Public Sub SelectQuery(sql As String, completionBlock As Func(Of ArrayList, Integer))
        DBQuery(Function(cn)
                    Dim record As New Dictionary(Of String, String)
                    Dim records As New ArrayList
                    Debug.Print("SQL: " & sql)
                    Using cmd As New SQLiteCommand(sql, cn)
                        Using rs As SQLiteDataReader = cmd.ExecuteReader()
                            While rs.Read
                                For i As Integer = 0 To rs.FieldCount - 1
                                    record.Add(rs.GetName(i), rs.GetValue(i))
                                Next
                                records.Add(record)
                            End While
                        End Using
                    End Using

                    completionBlock(records)
                    Return 0
                End Function)
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

        DBQuery(Function(cn)
                    Using cmd As New SQLiteCommand(tableQuery, cn)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SQLiteCommand(adminQuery, cn)
                        cmd.ExecuteNonQuery()
                    End Using
                    Return 0
                End Function)

    End Sub

    Public Sub Login(usrn As String, pssw As String, completionBlock As Func(Of ArrayList, Integer))
        Dim loginQuery As String = String.Format("SELECT * FROM `tbl_admin` WHERE usrn = '{0}' And pssw = '{1}'", usrn, pssw)

        SelectQuery(loginQuery, Function(dictionaries)
                                    completionBlock(dictionaries)
                                    Return 0
                                End Function)
    End Sub

End Module
