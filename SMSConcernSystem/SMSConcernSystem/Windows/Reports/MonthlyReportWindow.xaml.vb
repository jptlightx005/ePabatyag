Imports System.IO.File
Imports System.Collections.ObjectModel
Public Class MonthlyReportWindow
    Public Property department_list As New ObservableCollection(Of String)
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        cmb_dep.ItemsSource = department_list
        department_list.Add("ALL")
        cmb_dep.SelectedIndex = 0
        For Each kw In keywords
            department_list.Add(kw)
        Next
    End Sub
    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)


    End Sub

    Private Sub btn_close_Click(sender As Object, e As RoutedEventArgs) Handles btn_close.Click
        Me.Close()
    End Sub

    Private Sub btn_print_Click(sender As Object, e As RoutedEventArgs) Handles btn_print.Click
        Dim webReportWindow As New MonthlyReportWebWindow
        webReportWindow.selectedMonth = cmb_month.SelectedIndex
        webReportWindow.selectedDep = department_list(cmb_dep.SelectedIndex)
        Debug.Print("Selected month is {0}", webReportWindow.selectedMonth)
        webReportWindow.ShowDialog()
    End Sub
End Class
