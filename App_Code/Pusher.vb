Imports Microsoft.VisualBasic
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

' =====================================================================================================================
'
'
' Copyright 2010, Jon Burrows. Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
'
'
' =====================================================================================================================

Public Class Pusher
    Private Const PusherURL As String = "api.pusherapp.com"
    Private Const PusherPort As String = "80"
    Private Const PusherTimeout As String = "30"
    Private _PusherAppID As String = String.Empty
    Private _PusherAppKey As String = String.Empty
    Private _PusherAppSecret As String = String.Empty
    Public Property PusherAppID() As String
        Get
            Return _PusherAppID
        End Get
        Set(ByVal value As String)
            _PusherAppID = value
        End Set
    End Property
    Public Property PusherAppKey() As String
        Get
            Return _PusherAppKey
        End Get
        Set(ByVal value As String)
            _PusherAppKey = value
        End Set
    End Property
    Public Property PusherAppSecret() As String
        Get
            Return _PusherAppSecret
        End Get
        Set(ByVal value As String)
            _PusherAppSecret = value
        End Set
    End Property


    Public Function PushIt(ByVal Data As String, ByVal Channel As String, Optional ByVal SocketID As String = "") As String
        Dim _Response As HttpWebResponse = Nothing
        Dim _Reader As StreamReader

        Dim _URIPath As String = BuildURIPath(Channel)
        Dim _Query As String = BuildQuery("Event", Data, SocketID)
        Dim _Signature As String = BuildAuthenticationSignature(_URIPath, _Query)
        Dim _URL As String = BuildURI(_URIPath, _Query, _Signature)
        Dim _Request As HttpWebRequest = DirectCast(WebRequest.Create(_URL), HttpWebRequest)
        _Request.Method = "POST"
        _Request.ContentType = "application/json"
        Dim _ByteData() As Byte
        _ByteData = UTF8Encoding.UTF8.GetBytes(Data)
        _Request.ContentLength = _ByteData.Length
        Try
            Dim _PostStream As Stream = _Request.GetRequestStream
            _PostStream.Write(_ByteData, 0, _ByteData.Length)
            _PostStream.Close()
        Catch Ex As Exception
            Throw New Exception(_URL)
        End Try
        Try
            _Response = DirectCast(_Request.GetResponse(), HttpWebResponse)
            _Reader = New StreamReader(_Response.GetResponseStream())
            Return _Reader.ReadToEnd()
        Finally
            If Not _Response Is Nothing Then _Response.Close()

        End Try
        Return "Something Went Wrong"
    End Function


    Private Function BuildURIPath(ByVal ChannelName As String) As String
        Dim _Buffer As StringBuilder = New StringBuilder
        _Buffer.Append("/apps/")
        _Buffer.Append(PusherAppID)
        _Buffer.Append("/channels/")
        _Buffer.Append(ChannelName)
        _Buffer.Append("/events")
        Return _Buffer.ToString
    End Function
    Private Function BuildQuery(ByVal EventName As String, ByVal jsonData As String, ByVal SocketID As String) As String
        Dim _Buffer As StringBuilder = New StringBuilder
        _Buffer.Append("auth_key=")
        _Buffer.Append(PusherAppKey)
        _Buffer.Append("&auth_timestamp=")
        _Buffer.Append(DateTime.Now.AddHours(-1).Subtract(New DateTime(1970, 1, 1)).TotalSeconds)
        _Buffer.Append("&auth_version=1.0")
        _Buffer.Append("&body_md5=")
        _Buffer.Append(MD5Hasher(jsonData))
        _Buffer.Append("&name=")
        _Buffer.Append(EventName)
        If SocketID.Length > 0 Then
            _Buffer.Append("&socket_id=")
            _Buffer.Append(SocketID)
        End If
        Return _Buffer.ToString
    End Function

    Private Function BuildAuthenticationSignature(ByVal URL As String, ByVal Query As String) As String
        Dim _Buffer As StringBuilder = New StringBuilder
        _Buffer.Append("POST" & Chr(10))
        _Buffer.Append(URL)
        _Buffer.Append(Chr(10))
        _Buffer.Append(Query)
        Return HMACSHA256Hasher(_Buffer.ToString)
    End Function

    Private Function BuildURI(ByVal URIPath As String, ByVal Query As String, ByVal Signature As String) As String
        Dim _Buffer As StringBuilder = New StringBuilder
        _Buffer.Append("http://")
        _Buffer.Append(PusherURL)
        _Buffer.Append(URIPath)
        _Buffer.Append("?")
        _Buffer.Append(Query)
        _Buffer.Append("&auth_signature=")
        _Buffer.Append(Signature)
        Return _Buffer.ToString
    End Function

    Private Function ByteToHex(ByVal buff As Byte()) As String
        Dim sbinary As String = ""
        For i As Integer = 0 To buff.Length - 1
            sbinary += buff(i).ToString("x2") ' HEX Format
        Next
        Return (sbinary)
    End Function

    Private Function MD5Hasher(ByVal Data As String) As String
        Dim myEncoder As New System.Text.UTF8Encoding
        Dim ByteSourceText() As Byte = myEncoder.GetBytes(Data)
        Dim MD5 As New MD5CryptoServiceProvider()
        Dim ByteHash() As Byte = MD5.ComputeHash(ByteSourceText)
        Return ByteToHex(ByteHash)
    End Function

    Private Function HMACSHA256Hasher(ByVal Data As String) As String
        Dim myEncoder As New System.Text.UTF8Encoding
        Dim Key() As Byte = myEncoder.GetBytes(PusherAppSecret)
        Dim XML() As Byte = myEncoder.GetBytes(Data)
        Dim myHMACSHA256 As New System.Security.Cryptography.HMACSHA256(Key)
        Dim HashCode As Byte() = myHMACSHA256.ComputeHash(XML)
        Return ByteToHex(HashCode).PadLeft(64, "0")
    End Function
End Class
