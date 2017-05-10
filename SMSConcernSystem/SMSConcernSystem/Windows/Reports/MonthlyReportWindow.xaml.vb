Imports System.IO.File
Public Class MonthlyReportWindow
    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        Dim base_dir = AppDomain.CurrentDomain.BaseDirectory
        Dim HTMLReportPage As String = ReadAllText(base_dir & "\MonthlyReports\MonthlyReportPage.html")

        Dim tryMeh = HTMLReportPage.Replace("{0}", "Gotta touch this =)")

    End Sub

    Private Sub btn_close_Click(sender As Object, e As RoutedEventArgs) Handles btn_close.Click
        Me.Close()
    End Sub

    Private Sub btn_print_Click(sender As Object, e As RoutedEventArgs) Handles btn_print.Click
        Dim webReportWindow As New MonthlyReportWebWindow
        webReportWindow.selectedMonth = TryCast(cmb_month.SelectedItem, ComboBoxItem).Content
        Debug.Print("Selected month is {0}", webReportWindow.selectedMonth)
        webReportWindow.ShowDialog()
    End Sub
End Class
