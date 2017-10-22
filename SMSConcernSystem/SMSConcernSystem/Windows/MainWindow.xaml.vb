Imports GsmComm.PduConverter
Imports System.Data
Imports System.Data.SQLite
Imports System.ComponentModel
Imports System.Windows.Threading
Imports System.Collections.ObjectModel

Public Class MainWindow
    Dim messageList As List(Of Dictionary(Of String, String))
    Dim WithEvents deviceChecker As BackgroundWorker
    Dim WithEvents timerChecker As DispatcherTimer

    Dim closingObject As Object
    Public Sub New()
        InitializeComponent()
    End Sub
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        UpdateTable()
        timerChecker = New DispatcherTimer
        deviceChecker = New BackgroundWorker
        timerChecker.Interval = New TimeSpan(0, 0, 0, 1, 0)
        timerChecker.Start()

        'Uncomment for testing only
        'Dim sms_test_message As New SmsDeliverPdu
        'sms_test_message.UserDataText = "ICT Q5 P5 hello world!!"
        'sms_test_message.OriginatingAddress = "09098058053"
        'sms_test_message.SCTimestamp = New SmsTimestamp(Now, 8)
        'SaveRawMessage(sms_test_message)

    End Sub

    Private Sub menuItem_Click(sender As Object, e As RoutedEventArgs) Handles mnu_logout.Click, mnuSettings.Click, mnu_close.Click, mnu_report.Click
        Dim m As MenuItem = TryCast(sender, MenuItem)
        Debug.Print("Selected menu is: {0}", m.Header)
        If Equals(sender, mnu_logout) Then
            Select Case yesNoMsgBox("Are you sure you want to log-out?")
                Case vbYes
                    Dim loginWindow As New LogInWindow
                    My.Settings.isLoggedIn = False
                    My.Settings.Save()
                    loginWindow.Show()
                    closingObject = mnu_logout
                    Me.Close()
            End Select
        ElseIf Equals(sender, mnuSettings) Then
            'Dim settingsWindow As New SettingsWindow
            'settingsWindow.ShowDialog()
        ElseIf Equals(sender, mnu_close) Then
            closingObject = mnu_close
            Me.Close()
        ElseIf Equals(sender, mnu_report) Then
            Dim officeType As Type = Type.GetTypeFromProgID("Excel.Application")
            If officeType Is Nothing Then
                MsgBox("You must install Microsoft Excel on this computer to use the reporting system!", vbExclamation)
            Else
                Dim reportsWindow As New MonthlyReportWindow

                reportsWindow.ShowDialog()
            End If
        End If
    End Sub

    Private Sub timerChecker_Tick() Handles timerChecker.Tick
        If Not deviceChecker.IsBusy Then
            deviceChecker.RunWorkerAsync()
        End If
    End Sub
    Private Sub UpdateTable()
        Dim selectStudentsQuery As String = "SELECT * FROM tbl_inbox WHERE is_removed = 0 ORDER BY tbl_inbox.ID DESC "

        Dim dataSet As New DataSet()
        Dim data = SelectData(selectStudentsQuery)
        data.Fill(dataSet)
        gridInbox.ItemsSource = dataSet.Tables(0).DefaultView

        messageList = SelectQuery(selectStudentsQuery)

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

    Private Sub SaveRawMessage(message As SmsDeliverPdu)
        Debug.Print("Attempting to save raw message '{0}'", message.UserDataText)
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("sender", SQLInject(message.OriginatingAddress))
        parameters.Add("message", SQLInject(message.UserDataText))
        parameters.Add("date_sent", SQLInject(message.SCTimestamp.ToString))

        Dim sqlBuilder As New System.Text.StringBuilder
        sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_raw_inbox({0}) VALUES({1});",
            pList(parameters.Keys),
            pList(parameters.Values)))

        If ExecuteQuery(sqlBuilder.ToString) Then
            Debug.Print("Received a message and stored to raw inbox")
            CheckKeywordIfValid(message)
        Else
            Debug.Print("Received a message but failed to store to raw inbox")
        End If
    End Sub

    Private Sub CheckKeywordIfValid(message As SmsDeliverPdu)
        For Each key As String In keywords
            Debug.Print("{0} == {1}", message.Keyword, key)

            If message.Keyword = key Then
                Debug.Print("^ True ^")
                SaveFilteredMessage(message)
                Exit Sub
            End If
        Next
        Debug.Print("Message was not saved because keyword is not valid.")
    End Sub
    Private Sub SaveFilteredMessage(message As SmsDeliverPdu)
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("keyword", SQLInject(message.Keyword))
        parameters.Add("quality", SQLInject(message.Ratings.QualityOfService))
        parameters.Add("timeliness", SQLInject(message.Ratings.TimelinessOfService))
        parameters.Add("professionalism", SQLInject(message.Ratings.ProfesionalismOfPersonel))
        parameters.Add("message_content", SQLInject(message.FilteredMessage))
        parameters.Add("mobile_number", SQLInject(message.OriginatingAddress))
        parameters.Add("date_received", SQLInject(message.SCTimestamp.ToString(False)))
        parameters.Add("is_read", 0)
        Dim sqlBuilder As New System.Text.StringBuilder
        sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_inbox({0}) VALUES({1});",
            pList(parameters.Keys),
            pList(parameters.Values)))

        If ExecuteQuery(sqlBuilder.ToString) Then
            Debug.Print("Filtered a message and stored to inbox")
            Dispatcher.Invoke(Sub()
                                  MessageAlertTone()
                                  UpdateTable()
                              End Sub)
            ReplyIfMessageIsValid(message.OriginatingAddress)
        Else
            Debug.Print("Filtered a message but failed to store to inbox")
        End If
    End Sub

    Private Sub ReplyIfMessageIsValid(sender As String)
        If smsDeviceConnected Then
            Dim message = "Congratulations! Your message has been received. Thank you for your feedback! We will get in touch with you soon!"
            SendMessage(message, sender)
        End If
    End Sub

    Private Sub gridInbox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles gridInbox.SelectionChanged
        Debug.Print("Selected index {0}", gridInbox.SelectedIndex)
        btnRead.IsEnabled = gridInbox.SelectedIndex >= 0
        btnRemove.IsEnabled = gridInbox.SelectedIndex >= 0
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

    Private Sub btnRemove_Click(sender As Object, e As RoutedEventArgs) Handles btnRemove.Click
        If gridInbox.SelectedIndex >= 0 Then
            Dim selectedMessage = messageList(gridInbox.SelectedIndex)
            Dim sql = String.Format("UPDATE tbl_inbox SET is_removed = 1 WHERE ID = {0});", selectedMessage("ID"))

            If ExecuteQuery(sql) Then
                Debug.Print("Removed message")
                UpdateTable()
            Else
                Debug.Print("Filtered a message but failed to store to inbox")
            End If
        End If
    End Sub
End Class
