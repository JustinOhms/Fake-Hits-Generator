﻿'
' Created by SharpDevelop.
' User: IP-Man
' Date: 10/14/2016
' Time: 8:21 PM
' 
Imports System.IO
Public Partial Class MainForm
	Dim Timer_Value As Integer 
	Dim Speed As Integer 
	Dim Hits As Integer 
	Dim SwitchHits As Integer
	Dim Proxy As String 
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
		'
	End Sub
	
	Sub Load_site_btnClick(sender As Object, e As EventArgs)	
		
		url_box.Text = url_box.Text.Trim
		hit_speed_val_box.Text = hit_speed_val_box.Text.Trim 
		start_btn.Enabled = True
		
		'Check inputs are null or empty
		If String.IsNullOrEmpty(url_box.Text) Or String.IsNullOrEmpty(hit_speed_val_box.Text) Then
			MsgBox("Input details are missing!",vbExclamation,"Error")	
		Else
			
			If ProxyListUse.Checked = True Then 'Check if proxy is selected
				LogAdd("Proxy Selected : " & proxy_list.SelectedItem)
				Call NavigateWithProxy()
				
			Else If ProxyListUse.Checked = False Then
				Call NavigateWithoutProxy()
				LogAdd("Proxy Disabled.")
			End If	
			
			load_site_btn.Enabled = False	'Disable buttons to start process
			proxy_list_grp.Enabled = False 
		End If
		
		LogAdd("Site Loaded : " & url_box.Text)
		
	End Sub
	
	Sub Start_btnClick(sender As Object, e As EventArgs)
		
		hit_timer.Enabled = True	'Starting the timer
		Speed = hit_speed_val_box.Text 'Setting the Hitting Speed
		Hits = 1	'Reset Hits
		
		start_btn.Enabled = False 
		stop_btn.Enabled = True 
		
		SwitchHits = switch_hits_txt.Text 'Set Proxy switch value
		
		proxy_list_grp.Enabled = True
		
		LogAdd("Process started. Hit Speed is " & hit_speed_val_box.Text)
		
	End Sub
	
	Sub Hit_timerTick(sender As Object, e As EventArgs)		
		
		Timer_Value += 1
		
		hit_cntdown.Text = "Waiting : " & Timer_Value.ToString
		
		If Timer_Value = Speed Then 
			
			webBrowser.Refresh
			
		If ProxyListUse.Checked = True Then Call UseProxyList() Else NavigateWithoutProxy()		
			hit_count.Text = "Hits : " & Hits.ToString
			Timer_Value = 0	
			Hits += 1	
		End If 
	End Sub
	
	Sub Stop_btnClick(sender As Object, e As EventArgs)
		hit_timer.Enabled = False 
		stop_btn.Enabled = False
		load_site_btn.Enabled = True
		hit_cntdown.Text = "Waiting : Stopped"
		SwitchHits = 0
		LogAdd("Process stopped!. " & "Total Hits : " & Hits)
	End Sub
	
	Sub Url_boxTextChanged(sender As Object, e As EventArgs)
		load_site_btn.Enabled = True
		start_btn.Enabled = False 
		stop_btn.Enabled = False 
	End Sub
	
	Sub Load_proxylistClick(sender As Object, e As EventArgs)
		Call OpenProxyList()
	End Sub
	
	Private Sub UseProxyList()
		
		If autoSwitch.Checked = True Then
			Call NavigateWithProxy()
			
			If switch_hits_txt.Text = Hits Then 
				
				If proxy_list.SelectedIndex = proxy_list.Items.Count - 1 Then proxy_list.SelectedIndex = -1 
				
				switch_hits_txt.Text += SwitchHits
				webBrowser.Stop
				proxy_list.SelectedIndex += 1
				
			End If
			
		Else 
			Call NavigateWithProxy()
			
		End If
		
	End Sub
	
	Private Sub NavigateWithProxy()
		Use_Proxy.UseProxy(Proxy) 
		webBrowser.Navigate(url_box.Text)
	End Sub
	
	Private Sub NavigateWithoutProxy()
		webBrowser.Navigate(url_box.Text)
	End Sub
	
	Private Sub OpenProxyList() 'Load proxy list from a text file
		
		Dim openfile = New OpenFileDialog()
		openfile.Filter = "Text (*.txt)|*.txt"
		If (openfile.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then
			Dim proxyfile As String = openfile.FileName
			Dim proxies As String() = File.ReadAllLines(proxyfile)
			
			For Each line As String In proxies
				proxy_list.Items.Add(line)
			Next
			proxy_list.SelectedIndex = 0
			LogAdd("Proxy list added with " & proxy_list.Items.Count & " proxies.")
		End If
		
	End Sub
	
	Sub Switch_hits_txtTextChanged(sender As Object, e As EventArgs)
		LogAdd("Switching proxy @ " & switch_hits_txt.Text & " Hits.")
	End Sub	
	
	Sub Proxy_listSelectedIndexChanged(sender As Object, e As EventArgs)
		Proxy = proxy_list.SelectedItem 'Changing proxy
		LogAdd("Proxy selected : " & Proxy)
	End Sub
	
	Sub LogAdd(log As String) 'Log box update
		logBox.Items.Add(log)
	End Sub
	
	Sub ProxyListUseCheckedChanged(sender As Object, e As EventArgs)
	If ProxyListUse.Checked = True Then LogAdd("Proxy list is enabled.") Else LogAdd("Proxy list is disabled.")
	End Sub
	
	Sub AutoSwitchCheckedChanged(sender As Object, e As EventArgs)	
	If autoSwitch.Checked = True Then LogAdd("Proxy auto switch is enabled.") Else LogAdd("Proxy auto switch is disabled.") 	
	End Sub
	
End Class
