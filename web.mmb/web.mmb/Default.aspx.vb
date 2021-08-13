Imports Intuitive

Public Class _default1
	Inherits Intuitive.Web.PageBase

	Public Shared ErrorCount As Integer = 0

	Protected Overrides Sub OnPreInit(e As System.EventArgs)

		Dim sThemedVirtualMasterPath As String = String.Format("~/themes/{0}/MasterPage/{0}.Master", Booking.Params.Theme)
		Dim sThemedPhysicalMasterPath As String = HttpContext.Current.Server.MapPath(sThemedVirtualMasterPath)
		If Intuitive.Functions.FileExists(sThemedPhysicalMasterPath) Then
			Me.MasterPageFile = sThemedVirtualMasterPath
		End If

		MyBase.OnPreInit(e)

	End Sub

	Protected Overrides Sub OnError(ByVal e As System.EventArgs)

		Try

			ErrorCount += 1

			If ErrorCount = 1 Then

				Intuitive.Web.Logging.LogError("Default", "OnError", Server.GetLastError)

			End If

			Cache("lasterror") = Server.GetLastError
			Cache("errorurl") = Me.Request.Url.OriginalString

		Catch ex As Exception

		End Try

		Response.Redirect(BookingBase.Params.RootPath & "/error")


	End Sub

End Class