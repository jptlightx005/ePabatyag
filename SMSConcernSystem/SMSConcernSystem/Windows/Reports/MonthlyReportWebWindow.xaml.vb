Imports System.IO.File
Imports SMSCSFuncs

Public Class MonthlyReportWebWindow
    Public fromMonth As Date
    Public toMonth As Date
    Public allMonth As Boolean
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
            sql = String.Format("SELECT * FROM tbl_inbox WHERE is_removed = 0 AND keyword = '{0}'", selectedDep)
        End If

        Dim messages = SelectQuery(sql)
        Dim filteredMessages = New List(Of Dictionary(Of String, String))

        If allMonth Then
            filteredMessages = messages
        Else
            For Each message In messages
                Dim dateReceived As Date
                dateReceived = DateTime.Parse(message("date_received"))

                If dateReceived >= fromMonth And dateReceived <= toMonth Then
                    filteredMessages.Add(message)
                End If
            Next
        End If
        

        Dim htmlTable As String = ""
        For Each message In filteredMessages
            Dim dateReceived As Date
            dateReceived = DateTime.Parse(message("date_received"))

            htmlTable += "<tr>" & vbCrLf
            htmlTable += String.Format("<td>{0} {1}, {2}</td>", MonthName(dateReceived.Month, False), dateReceived.Day, dateReceived.Year) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", message("keyword")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", message("message_content")) & vbCrLf
            htmlTable += "</tr>" & vbCrLf
        Next

        Dim base_dir = AppDomain.CurrentDomain.BaseDirectory
        Dim url = "\MonthlyReports\MonthlyReportPage.html"
        Debug.Print("Dep: {0}", selectedDep)
        If selectedDep <> "ALL" Then
            url = "\MonthlyReports\MonthlyReportPageDep.html"
        End If

        Dim HTMLReportPage As String = ReadAllText(base_dir & url)
        Dim monthStr As String = "ALL"
        If Not allMonth Then
            monthStr = fromMonth.ToString("MMMM dd, yyyy") & " - " & toMonth.ToString("MMMM dd, yyyy")
        End If
        Dim finalHTML = HTMLReportPage.Replace("{0}", htmlTable).Replace("{1}", base_dir.Replace("\", "/") & "MonthlyReports/").Replace("{2}", monthStr)
        If selectedDep <> "ALL" Then
            finalHTML = finalHTML.Replace("{3}", selectedDep)
        End If
        Debug.Print("Test: {0}", finalHTML)
        wb_report.NavigateToString(finalHTML)
    End Sub
    

    Private Sub btnPrint_Click(sender As Object, e As RoutedEventArgs) Handles btnPrint.Click
        webbrowser_extension.PrintDocument(wb_report)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class
