﻿Imports System.Data
Public Class StudentListWindow
    Public studentList As List(Of Dictionary(Of String, String))
    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim regWindow As New RegistrationWindow
        regWindow.isFromStudentList = True
        regWindow.parentForm = Me
        regWindow.ShowDialog()
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As RoutedEventArgs) Handles btnEdit.Click
        Dim regWindow As New RegistrationWindow
        Dim currentRowIndex = gridStudents.Items.IndexOf(gridStudents.SelectedItem)
        Debug.Print(currentRowIndex)
        If (currentRowIndex >= 0 Or currentRowIndex < gridStudents.Items.Count) Then
            regWindow.selectedStudent = studentList(currentRowIndex)
            regWindow.isFromStudentList = True
            regWindow.parentForm = Me
            regWindow.ShowDialog()
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        UpdateTableView()
    End Sub

    Public Sub UpdateTableView()
        Dim selectStudentsQuery As String = "SELECT student_id As `Student ID`," & _
                                             "(first_name || ' ' || last_name) AS `Name`," & _
                                             "course As `Course`," & _
                                             "year_section As `Year & Section`," & _
                                             "mobile_number As `Mobile No` " & _
                                            "FROM `tbl_contacts`"
        SelectData(selectStudentsQuery,
                   Sub(dataAdapter)
                       Dim dataSet As New DataSet()
                       dataAdapter.Fill(dataSet)
                       gridStudents.ItemsSource = dataSet.Tables(0).DefaultView
                   End Sub)

        Dim sql As String = String.Format("SELECT * FROM tbl_contacts")

        SelectQuery(sql, Sub(result)
                             studentList = result
                         End Sub)

        gridStudents.IsReadOnly = True
    End Sub
    Private Sub gridStudents_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles gridStudents.SelectionChanged
        If (gridStudents.SelectedIndex >= 0) Then
            Debug.Print(gridStudents.SelectedIndex)
            btnEdit.IsEnabled = True
            btnDelete.IsEnabled = True
        Else
            btnEdit.IsEnabled = False
            btnDelete.IsEnabled = False
        End If
    End Sub
End Class
