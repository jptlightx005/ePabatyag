﻿Imports GsmComm.PduConverter
Imports System.Data
Imports System.Data.SQLite
Imports System.ComponentModel
Imports System.Windows.Threading
Class MainWindow
    Dim messageList As List(Of Dictionary(Of String, String))
    Dim WithEvents deviceChecker As BackgroundWorker
    Dim WithEvents timerChecker As DispatcherTimer
    Dim closingObject As Object
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        UpdateTable()
        timerChecker = New DispatcherTimer
        deviceChecker = New BackgroundWorker
        timerChecker.Interval = New TimeSpan(0, 0, 0, 1, 0)
        timerChecker.Start()
    End Sub
    Private Sub timerChecker_Tick() Handles timerChecker.Tick
        If Not deviceChecker.IsBusy Then
            deviceChecker.RunWorkerAsync()
        End If
    End Sub
    Private Sub UpdateTable()
        Dim selectStudentsQuery As String = "SELECT keyword As `Keyword`," & _
                                            "message_content AS `Message`," & _
                                            "mobile_number As `Mobile No` " & _
                                            "FROM tbl_inbox ORDER BY tbl_inbox.ID DESC"

        SelectData(selectStudentsQuery,
                   Sub(dataAdapter)
                       Dim dataSet As New DataSet()
                       dataAdapter.Fill(dataSet)
                       gridInbox.ItemsSource = dataSet.Tables(0).DefaultView
                   End Sub)

        Dim sql As String = String.Format("SELECT * FROM tbl_inbox ORDER BY tbl_inbox.ID DESC")

        SelectQuery(sql, Sub(result)
                             messageList = result
                         End Sub)

        gridInbox.IsReadOnly = True
    End Sub

    Private Sub gridInboxRow_DoubleClick(sender As Object, e As MouseButtonEventArgs)
        Debug.Print(String.Format("Did select row number: {0}", gridInbox.SelectedIndex))
        Dim messageWindow As New ViewMessageWindow
        messageWindow.selectedMessage = messageList(gridInbox.SelectedIndex)
        messageWindow.ShowDialog()
    End Sub

    Private Sub btnRead_Click(sender As Object, e As RoutedEventArgs) Handles btnRead.Click
        If gridInbox.SelectedIndex >= 0 Then
            Dim messageWindow As New ViewMessageWindow
            messageWindow.selectedMessage = messageList(gridInbox.SelectedIndex)
            messageWindow.ShowDialog()
        End If
    End Sub

    Private Sub mnu_logout_Click(sender As Object, e As RoutedEventArgs) Handles mnu_logout.Click
        Select Case yesNoMsgBox("Are you sure you want to log-out?")
            Case vbYes
                Dim loginWindow As New LogInWindow
                My.Settings.isLoggedIn = False
                My.Settings.Save()
                loginWindow.Show()
                closingObject = mnu_logout
                Me.Close()
        End Select
    End Sub

    Private Sub mnuSettings_Click(sender As Object, e As RoutedEventArgs) Handles mnuSettings.Click
        'Dim settingsWindow As New SettingsWindow
        'settingsWindow.ShowDialog()
    End Sub

    Private Sub SaveRawMessage(message As SmsDeliverPdu)
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("sender", SQLInject(message.OriginatingAddress))
        parameters.Add("message", SQLInject(message.UserDataText))
        parameters.Add("date_sent", SQLInject(message.SCTimestamp.ToString))

        Dim sqlBuilder As New System.Text.StringBuilder
        sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_raw_inbox({0}) VALUES({1});",
            pList(parameters.Keys),
            pList(parameters.Values)))

        ExecuteQuery(sqlBuilder.ToString, Sub(result)
                                              If result Then
                                                  Debug.Print("Received a message and stored to raw inbox")
                                                  CheckKeywordIfValid(message)
                                              Else
                                                  Debug.Print("Received a message but failed to store to raw inbox")
                                              End If
                                          End Sub)
    End Sub

    Private Sub CheckKeywordIfValid(message As SmsDeliverPdu)
        For Each key In keywords
            Debug.Print("{0} == {1}", message.Keyword, key)

            If message.Keyword = key Then
                Debug.Print("^ True ^")
                SaveFilteredMessage(message)
                Exit Sub
            End If
        Next
        Debug.Print("Message was not saved because sender is not registered.")
    End Sub
    Private Sub SaveFilteredMessage(message As SmsDeliverPdu)
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("keyword", SQLInject(message.Keyword))
        parameters.Add("message_content", SQLInject(message.UserDataText))
        parameters.Add("sender_number", SQLInject(message.OriginatingAddress))
        parameters.Add("date_received", SQLInject(message.SCTimestamp.ToString))
        parameters.Add("is_read", 0)
        Dim sqlBuilder As New System.Text.StringBuilder
        sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_inbox({0}) VALUES({1});",
            pList(parameters.Keys),
            pList(parameters.Values)))

        ExecuteQuery(sqlBuilder.ToString, Sub(result)
                                              If result Then
                                                  Debug.Print("Filtered a message and stored to inbox")
                                                  Dispatcher.Invoke(Sub()
                                                                        MessageAlertTone()
                                                                        UpdateTable()
                                                                    End Sub)
                                              Else
                                                  Debug.Print("Filtered a message but failed to store to inbox")
                                              End If
                                          End Sub)
    End Sub

    Private Sub gridInbox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles gridInbox.SelectionChanged
        Debug.Print("Selected index {0}", gridInbox.SelectedIndex)
        btnRead.IsEnabled = gridInbox.SelectedIndex >= 0
    End Sub

    Private Sub deviceChecker_DoWork() Handles deviceChecker.DoWork
        If Not smsDeviceConnected Then
            Dim savedPort = My.Settings.smsDevicePort
            If savedPort = String.Empty Then
                Dim ports = GetPorts()
                Dim portIndex = -1
                For Each port In ports
                    smsDeviceConnected = PortHasDevice(port)
                    If smsDeviceConnected Then
                        My.Settings.smsDevicePort = port
                        My.Settings.Save()
                        Dispatcher.Invoke(Sub()
                                              imgConnected.Visibility = Windows.Visibility.Visible
                                              lblConnected.Content = "Connected"
                                          End Sub)
                        Exit Sub
                    Else
                        Dispatcher.Invoke(Sub()
                                              imgConnected.Visibility = Windows.Visibility.Hidden
                                              lblConnected.Content = "Not Connected"
                                          End Sub)
                    End If
                Next
            Else
                smsDeviceConnected = PortHasDevice(savedPort)
                If smsDeviceConnected Then
                    Dispatcher.Invoke(Sub()
                                          imgConnected.Visibility = Windows.Visibility.Visible
                                          lblConnected.Content = "Connected"
                                      End Sub)
                    Exit Sub
                Else
                    My.Settings.smsDevicePort = ""
                    My.Settings.Save()
                    Dispatcher.Invoke(Sub()
                                          imgConnected.Visibility = Windows.Visibility.Hidden
                                          lblConnected.Content = "Not Connected"
                                      End Sub)
                End If
            End If
            
        Else
            Debug.Print("Retrieving Messages...")
            Dim unreadMessages = GetAllUnreadMessages()
            If unreadMessages.Count > 0 Then
                For Each message In unreadMessages
                    SaveRawMessage(message)
                Next
            End If
        End If
    End Sub

    Private Sub MessageAlertTone()
        My.Computer.Audio.Play("Resources\sos_ringtone.wav", AudioPlayMode.Background)
    End Sub

    Private Sub lblConnected_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles lblConnected.MouseUp
        'use this to test codes tee hee
        'MessageAlertTone()
    End Sub

    Private Sub Window_LostFocus(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Window_GotFocus(sender As Object, e As RoutedEventArgs)
        timerChecker.Start()
    End Sub

    Private Sub Window_Closing(sender As Object, e As CancelEventArgs)
        If Not closingObject Is mnu_logout Then
            Select Case yesNoMsgBox("Are you sure you want to close the program?")
                Case vbYes
                    End
                Case vbNo
                    e.Cancel = True
            End Select
        Else
            smsDeviceConnected = False
            If (smsComm.IsOpen) Then
                smsComm.Close()
            End If
            timerChecker.Stop()
            If deviceChecker.IsBusy Then
                deviceChecker.CancelAsync()
            End If
        End If
    End Sub

    Private Sub mnu_close_Click(sender As Object, e As RoutedEventArgs) Handles mnu_close.Click
        closingObject = mnu_close
        Me.Close()
    End Sub
End Class
