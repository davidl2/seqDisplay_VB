Imports System
Imports System.IO
Imports System.Net.Http


Module Program

    Sub Main(args As String())
        Dim client As HttpClient = New HttpClient()

        Dim request = new HttpRequestMessage(HttpMethod.GET, "http://scatha-xfin.lmert.com:30780/next_seq.php")
        Dim response As HttpResponseMessage = client.Send(request)

        response.EnsureSuccessStatusCode()
        Dim stream = response.Content.ReadAsStream()
        Dim seqNo As Int32 = Convert.toInt32(new StreamReader(stream).ReadLine())

        Console.WriteLine("Got next number {0}", seqNo)

        Try 
            Dim regVersion = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                  "Software\lmert\misc", True)
            If regVersion Is Nothing Then
                ' Key doesn't exist; create it.
                regVersion = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                     "Software\lmert\misc")
            End If

            If regVersion IsNot Nothing Then
                dim lastSeqNo as Int32
                lastSeqNo = regVersion.GetValue("lastSeqNo", 0)
                If lastSeqNo > 0 Then
                  Console.WriteLine("  (prior value was {0})", lastSeqNo)  
                End If
                regVersion.SetValue("lastSeqNo", seqNo)
                regVersion.Close()
            End If
        Catch ex As System.TypeInitializationException
            ' i.e., on non-Windows platform (Linux)
            Dim homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            Dim dotfilePath As String = Path.Combine(homePath,".lmert-lastSeqNo.txt")
            Dim lastSeqNo as Int32 = 0
            Try
                Dim r = new StreamReader(File.OpenRead(dotfilePath))
                lastSeqNo = Convert.toInt32(r.readLine)
                r.Close
            Catch ex2 As Exception
                Console.WriteLine("  (error retrieving prior sequence number: {0})",ex2)
            End Try
            If lastSeqNo > 0 Then
                  Console.WriteLine("  (prior value was {0})", lastSeqNo)  
            End If
            Try
                Dim w = new StreamWriter(File.Create(dotfilePath))
                w.WriteLine(seqNo.ToString())
                w.Close
            Catch ex3 As Exception
                Console.WriteLine("  (error saving sequence number: {0})",ex3)
            End Try
        End Try



'        Dim lastSeqNo = My.Computer.Registry.GetValue(
'    "HKEY_CURRENT_USER\Software\lmert\misc", "lastSeqNo", Nothing)

'        If lastSeqNo IsNot Nothing Then
'           Console.WriteLine("  (prior value was {0})", lastSeqNo)
'        End If

'        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\lmert\misc", "lastSeqNo", lastSeqNo)

    End Sub
End Module
