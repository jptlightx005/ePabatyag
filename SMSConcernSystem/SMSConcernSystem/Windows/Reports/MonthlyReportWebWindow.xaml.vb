Imports System.IO.File
Imports SMSCSFuncs
Public Class MonthlyReportWebWindow
    Public selectedMonth As Integer
    Public selectedDep As String
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler wb_report.Navigated, AddressOf wb_report_navigated

    End Sub

    Sub wb_report_navigated(ByVal sender As Object, e As NavigationEventArgs)
        webbrowser_extension.SetSilent(wb_report, True)

    End Sub
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim sql As String
        If selectedDep = "ALL" Then
            sql = String.Format("SELECT * FROM tbl_inbox WHERE is_removed = 0")
        Else
            sql = String.Format("SELECT * FROM tbl_inbox WHERE is_removed = 0 AND keyword = {0}", selectedDep)
        End If

        Dim messages = SelectQuery(sql)
        Dim filteredMessages = New List(Of Dictionary(Of String, String))

        If selectedMonth = 0 Then
            filteredMessages = messages
        Else
            For Each message In messages
                Dim dateReceived As Date
                dateReceived = DateTime.Parse(message("date_received"))

                If dateReceived.Month = selectedMonth Then
                    filteredMessages.Add(message)
                End If
            Next
        End If
        

        Dim htmlTable As String = ""
        For Each message In filteredMessages
            Dim dateReceived As Date
            dateReceived = DateTime.Parse(message("date_received"))

            htmlTable += "<tr>"
            htmlTable += String.Format("<td>{0} {1}, {2}</td>", MonthName(dateReceived.Month, False), dateReceived.Day, dateReceived.Year)
            htmlTable += String.Format("<td>{0}</td>", message("keyword"))
            htmlTable += String.Format("<td>{0}</td>", message("message_content"))

            htmlTable += "</tr>"
        Next

        Dim base_dir = AppDomain.CurrentDomain.BaseDirectory
        Dim HTMLReportPage As String = ReadAllText(base_dir & "\MonthlyReports\MonthlyReportPage.html")

        Dim finalHTML = HTMLReportPage.Replace("{0}", htmlTable).Replace("{1}", base_dir.Replace("\", "/") & "MonthlyReports/")
        Debug.Print("Test: {0}", finalHTML)
        wb_report.NavigateToString(finalHTML)
    End Sub
    

End Class
