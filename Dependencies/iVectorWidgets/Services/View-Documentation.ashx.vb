Imports System.Web
Imports System.Web.Services
Imports System.Web.SessionState
Imports System.Web.Routing
Imports System.IO
Imports System.Net
Imports Intuitive.Web

Public Class ViewDocumentationHandler
	Implements System.Web.IHttpHandler, IReadOnlySessionState

	Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

		'check customer is logged
		If BookingBase.LoggedIn = False Then
			context.Response.Write("Permission denied")
		Else
			'get key from request
			Dim key As String = context.Request.RequestContext.RouteData.Values("key").ToString

			'get url from key, based on saved session list of documentation they must have already requested from mybookings
			'it won't exist otherwise
			Dim url As String = iVectorWidgets.MyBookings.GetDocumentationURL(key)

			If url = "" Then
				context.Response.Write("Documentation unavailable")
			Else
				Using wc As New WebClient()
					Using stream As IO.Stream = wc.OpenRead(url)
						Using memStream As New MemoryStream()
							stream.CopyTo(memStream)
							Dim byteArray() As Byte = memStream.ToArray()
							'Set the appropriate ContentType.
							context.Response.ContentType = "Application/pdf"
							'Write the file directly to the HTTP content output stream.
							context.Response.OutputStream.Write(byteArray, 0, byteArray.Length)
							context.Response.End()
						End Using
					End Using
				End Using
			End If

		End If



	End Sub


	ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
		Get
			Return False
		End Get
	End Property

End Class