Imports System.IO.File
Imports SMSCSFuncs

Public Class MonthlyReportWebWindow
    Public fromMonth As Date
    Public toMonth As Date
    Public allMonth As Boolean
    Public selectedDep As String
    Public repType As ReportType

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
        Debug.Print("Rep type: {0}", repType)
        If repType = ReportType.Feedbacks Then
            loadFeedbackReports()
        ElseIf repType = ReportType.Ratings Then
            loadRatingsReport()
        End If

    End Sub
    
    Private Sub loadFeedbackReports()
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
            htmlTable += String.Format("<td>{0}</td>", message("quality")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", message("timeliness")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", message("professionalism")) & vbCrLf
            Dim total As Double = Double.Parse(message("quality")) + Double.Parse(message("timeliness")) + Double.Parse(message("professionalism"))
            Dim average = Math.Round(total / 3, 2)
            htmlTable += String.Format("<td>{0}</td>", average) & vbCrLf
            If selectedDep = "ALL" Then
                htmlTable += String.Format("<td>{0}</td>", message("keyword")) & vbCrLf
            End If
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

    Private Sub loadRatingsReport()
        Debug.Print("Loading ratings")
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

        Dim ratingsArray = New List(Of Dictionary(Of String, Object))
        For i = 0 To 2
            'New Row, three times
            Dim row = New Dictionary(Of String, Object)
            If i = 0 Then
                row("header") = "Quality of Service"
            ElseIf i = 1 Then
                row("header") = "Timeliness of Service"
            ElseIf i = 2 Then
                row("header") = "Professionalism of Personel"
            End If

            Dim ratings = New Dictionary(Of String, Integer)
            ratings("0") = 0
            ratings("1") = 0
            ratings("2") = 0
            ratings("3") = 0
            ratings("4") = 0
            ratings("5") = 0
            row("ratings") = ratings

            ratingsArray.Add(row)
        Next

        Debug.Print("ratings count: {0}", ratingsArray.Count)
        For Each row In ratingsArray
            Debug.Print("Row {0} is", ratingsArray.IndexOf(row))
            Debug.Print("'{0}", row("header"))

            Debug.Print("Ratings are:")
            For Each k In DirectCast(row("ratings"), Dictionary(Of String, Integer)).Keys
                Debug.Print("{0} = {1}", k, row("ratings")(k))
            Next
        Next
        For Each message In filteredMessages
            'Ratings keys
            '0 = QoS
            '1 = ToS
            '2 = PoP
            Dim q = message("quality").ToString
            Dim t = message("timeliness").ToString
            Dim p = message("professionalism").ToString
            ratingsArray(0)("ratings")(q) += 1  'QoS rating
            ratingsArray(1)("ratings")(t) += 1 ' ToSD rating
            ratingsArray(2)("ratings")(p) += 1 'PoP rating
        Next

        For Each row In ratingsArray
            Debug.Print("Row {0} is", ratingsArray.IndexOf(row))
            Debug.Print("'{0}", row("header"))

            Debug.Print("Ratings are:")
            For Each k In DirectCast(row("ratings"), Dictionary(Of String, Integer)).Keys
                Debug.Print("{0} = {1}", k, row("ratings")(k))
            Next
        Next

        Dim htmlTable As String = ""
        For Each row In ratingsArray
            htmlTable += "<tr>" & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", row("header")) & vbCrLf
            Dim ratings As Dictionary(Of String, Integer) = row("ratings")
            htmlTable += String.Format("<td>{0}</td>", ratings("0")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", ratings("2")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", ratings("3")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", ratings("4")) & vbCrLf
            htmlTable += String.Format("<td>{0}</td>", ratings("5")) & vbCrLf
            htmlTable += "</tr>" & vbCrLf
        Next

        Dim base_dir = AppDomain.CurrentDomain.BaseDirectory
        Dim url = "\MonthlyReports\MonthlyRatingsPage.html"
        Debug.Print("Dep: {0}", selectedDep)
        If selectedDep <> "ALL" Then
            url = "\MonthlyReports\MonthlyRatingsPageDep.html"
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

Public Enum ReportType
    Feedbacks = 0
    Ratings = 1
End Enum