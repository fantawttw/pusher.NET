
' =====================================================================================================================
'
'
' Copyright 2010, Jon Burrows. Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
'
'
' =====================================================================================================================

Partial Class Sender
    Inherits System.Web.UI.Page

    Protected Sub lnkPushIt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkPushIt.Click
        Response.Write("Pushed")
        Dim _Pusher As Pusher = New Pusher
        _Pusher.PusherAppID = "1619" ' Your App ID.
        _Pusher.PusherAppKey = "a8b69347fd6146d0466a" ' Your Apps Key.
        _Pusher.PusherAppSecret = "983d93848d60f68d6fb1" ' Your Apps Secret.
        _Pusher.PushIt(DateTime.Now & " -> " & txtMessageToPush.Text, "dotNETDemo")
    End Sub
End Class
