Imports System.Net
Imports System.Net.Sockets
Imports System.Net.NetworkInformation
Imports Microsoft.Win32
Imports System.Windows.Forms


' App created by NGRT
' My website: https://ngrt.rf.gd | TikTok : https://tiktok.com/@n.grt372 | Github : https://github.com/ngrt372
' This app has for purposes Wake-on-LAN your PC inside your local network area
'   How to enable WoL?
'       - Enable first, this feature in your BIOS PC
'       - After, enable this on Windows (search on the web)
'            _______  _       
'  |\     /|(  ___  )( \      
'  | )   ( || (   ) || (      
'  | | _ | || |   | || |      
'  | |( )| || |   | || |      
'  | || || || |   | || |      
'  | () () || (___) || (____/\
'  (_______)(_______)(_______/
'                             

Public Class Main

    Private notifyIcon As New NotifyIcon()
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TbIP.Text = "" Then
            MsgBox("Invalid IP Adress.", vbCritical, "WoL App by NGRT")

        ElseIf TbMAC.Text = "" Then
            MsgBox("Invalid MAC Adress.", vbCritical, "WoL App by NGRT")
        End If

        Dim macAddress As String = TbMAC.Text

        If Not IsValidMACAddress(macAddress) Then
            MessageBox.Show("Invalid MAC Adress.", "WoL App by NGRT", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        WakeUpComputer(macAddress)
    End Sub
    Private Function IsValidMACAddress(macAddress As String) As Boolean
        Return System.Text.RegularExpressions.Regex.IsMatch(macAddress, "([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})")
    End Function
    Private Function GetMacBytes(macAddress As String) As Byte()
        Dim macBytes As New List(Of Byte)

        For Each hexPair In macAddress.Split(":"c, "-"c)
            If hexPair.Length <> 2 Then
                MessageBox.Show("Invalid MAC Adress.", "WoL App by NGRT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return New Byte() {}
            End If

            Try
                macBytes.Add(Convert.ToByte(hexPair, 16))
            Catch ex As Exception
                MessageBox.Show("Error during the MAC Adress conversion.", "WoL App by NGRT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return New Byte() {}
            End Try
        Next

        Return macBytes.ToArray()
    End Function
    Private Function BuildMagicPacket(macBytes As Byte()) As Byte()
        Dim packet As New List(Of Byte)

        For i As Integer = 0 To 5
            packet.Add(&HFF)
        Next

        For i As Integer = 0 To 15
            packet.AddRange(macBytes)
        Next

        Return packet.ToArray()
    End Function
    Private Sub WakeUpComputer(macAddress As String)
        Try
            Dim macBytes As Byte() = GetMacBytes(macAddress)
            Dim magicPacket As Byte() = BuildMagicPacket(macBytes)

            Using client As New UdpClient()
                client.Connect(IPAddress.Broadcast, 7)
                client.Send(magicPacket, magicPacket.Length)
            End Using

            MessageBox.Show("Magic packet sent with success!", "WoL App by NGRT", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error during the send of the magic packet." & ex.Message, "WoL App by NGRT", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TimerNetwork.Start()

        notifyIcon.Icon = Me.Icon
        notifyIcon.Text = "WoL App by NGRT"
        AddHandler notifyIcon.MouseClick, AddressOf NotifyIcon_Click

        Me.Visible = False
    End Sub
    Private Sub TimerNetwork_Tick(sender As Object, e As EventArgs) Handles TimerNetwork.Tick
        If OnlinePC() Then
            Label5.Text = "Online"
            Label5.ForeColor = Color.LimeGreen
        Else
            Label5.Text = "Offline"
            Label5.ForeColor = Color.Red
        End If
    End Sub
    Private Function OnlinePC() As Boolean
        Try
            Using pingSender As New Ping()
                Dim reply As PingReply = pingSender.Send("1.1.1.1")
                Return reply.Status = IPStatus.Success
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Options.Show()
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process1.Start()
    End Sub
    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        End
    End Sub
    Private Sub ContextMenuStrip1_DoubleClick(sender As Object, e As EventArgs) Handles ContextMenuStrip1.DoubleClick
        Me.Show()
    End Sub
    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.WindowState = FormWindowState.Minimized
            Me.Visible = False
        End If
    End Sub
    Private Sub AfficherLappToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AfficherLappToolStripMenuItem.Click
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub
    Private Sub OptionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OptionsToolStripMenuItem.Click
        Options.Show()
    End Sub
    Private Sub NotifyIcon_Click(sender As Object, e As MouseEventArgs)
        Me.WindowState = FormWindowState.Normal
        Me.Visible = True
    End Sub
End Class