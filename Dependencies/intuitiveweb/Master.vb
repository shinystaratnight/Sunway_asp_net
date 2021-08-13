Imports System.Web.UI
Imports System.Web

Public MustInherit Class Master
	Inherits System.Web.UI.MasterPage

	Public Property Content As CMS.URLContent
	Public Property Overbranding As Overbranding
	Public Property CSSOverrides As CSSOverrides

	MustOverride ReadOnly Property GetPageServer() As PageServer

	Protected Overrides Sub OnLoad(e As System.EventArgs)

		If HttpContext.Current.Request.QueryString.ToString.ToLower.Contains("searchimage") Then
			Dim sEmailTo As String = HttpContext.Current.Request.QueryString("searchimage")
			If sEmailTo.Contains("?") Then sEmailTo = sEmailTo.Split("?"c)(0)
			BookingBase.EmailSearchImage = sEmailTo
		End If


		'clear out the old ivc logs - currently we only show the current page logs
		'I would like to extend this to build up the whole booking journey
		Logging.Current.ClearIVCLogs()

		'get current path from request
		Dim sAppRootPath As String = BookingBase.Params.RootPath
		Dim bNeedsRedirect As Boolean = False
		Dim request As HttpRequest = HttpContext.Current.Request

		Dim sPath As String = request.Url.AbsolutePath.ToLower

		'only redirect if we have tried to access the root of the app when we have a "root path" config setting
		'compare raw URL against the root path because of the IIS rewrite, otherwise we get stuck in infinite loop
		If Not sAppRootPath = "" AndAlso Not sPath.StartsWith(sAppRootPath) _
		  AndAlso Not request.RawUrl.ToLower.StartsWith(sAppRootPath.ToLower) Then
			bNeedsRedirect = True
			If sPath = "/default.aspx" Then
				sPath = sAppRootPath
			Else
				sPath = sAppRootPath & request.Url.AbsolutePath.ToLower
			End If
		End If


		'if the path is not the root and is trailing then chop it off
		'OR if the raw URL is the same as the "root path" config setting but with a trailing slash then also chop off 
		If Not sPath = "/" AndAlso sPath.EndsWith("/") Then

			bNeedsRedirect = True
			sPath = sPath.Chop(1)
		End If

		If request.RawUrl.ToLower = sAppRootPath.ToLower & "/" AndAlso sAppRootPath.ToLower <> "" Then
			bNeedsRedirect = True
			sPath = request.RawUrl.Chop(1)
		End If


		'if there was a query string add it on
		'only if need to redirect though
		If request.QueryString.ToString <> "" AndAlso bNeedsRedirect Then
			sPath = sPath & "?" & request.QueryString.ToString
		End If


		'if needed the redirect it
		If bNeedsRedirect Then
			'clear these down so they don't get logged incorrectly
			Logging.Current.PageEvents.CurrentEvents.Clear()
			Logging.Current.PageEvents.PreviousEvents.Clear()

			'make sure its tagged as a permanent redirect for SEO
			Response.Redirect(sPath, False)
			Response.StatusCode = 301
			Response.End()
		End If


		'get the valid virtual path
		Dim sVirtualDefaultPath As String = String.Format("{0}/default.aspx", HttpContext.Current.Request.ApplicationPath).Replace("//", "/")


		'only setup page if request has no extensions as we do not want .jpg, .gif, .ico etc
		'allow for .htm / .html files so this can be picked up by the URL 301 redirect functionality
		If Not sPath.Contains(".") OrElse sPath.Contains(".htm") OrElse sPath = sVirtualDefaultPath Then
			Controller.SetupPage(HttpContext.Current.Request, Me, Me.GetPageServer)
		End If


	End Sub


	Public Function GetPageHead() As Head
		For Each oControl As Control In Me.Controls
			If oControl.GetType.Name = "Head" Then
				Return CType(oControl, Head)
			End If
		Next
		Throw New Exception("Could not find head control")
	End Function

	Public Function GetTrackingControl(ByVal sSection As String) As TrackingControl
		For Each oControl As Control In Me.Controls
            If oControl.GetType.Name = "TrackingControl" AndAlso oControl.ID = sSection Then
                Return CType(oControl, TrackingControl)
            End If
            If oControl.ID = "frm" Then
                for Each oSubControl as Control in oControl.Controls
                    If oSubControl.GetType.Name = "TrackingControl" AndAlso oSubControl.ID = sSection Then
                        Return CType(oSubControl, TrackingControl)
                    End If
                Next
            End If
        Next
		Return Nothing
	End Function


	Public Function GetContainer(ByVal ContainerName As String) As Container
		Dim oContainer As Container = CType(Me.FindControl("div" & ContainerName), Container)
		If oContainer Is Nothing Then Throw New Exception("Could not find container " & ContainerName)
		Return oContainer
	End Function


    Public Function ContainerExists(ByVal ContainerName As String) As Boolean
        Dim oContainer As Container = CType(Me.FindControl("div" & ContainerName), Container)
        return oContainer IsNot Nothing
    End Function

End Class


