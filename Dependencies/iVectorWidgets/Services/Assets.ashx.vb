Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Text
Imports Intuitive

Public Class Assets
	Implements System.Web.IHttpHandler

	Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

		Dim key As String = context.Request.RequestContext.RouteData.Values("key").ToString
		'key.Replace(".css", "").Replace(".js", "")
		Dim pathToFile As String = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(key))

		Dim sCacheKey As String = "__assets_" & key

		Dim sData As String = ""

		If HttpContext.Current.Cache(sCacheKey) IsNot Nothing Then
			sData = Functions.SafeString(HttpContext.Current.Cache(sCacheKey))
		Else
			sData = File.ReadAllText(HttpContext.Current.Server.MapPath("~" & pathToFile))
		End If

		'bish it in the cache for an hour
		If Not Functions.IsDebugging Then Intuitive.Functions.AddToCache(sCacheKey, sData, 60)

		If pathToFile.EndsWith(".css") Then
			context.Response.ContentType = "text/css"
		ElseIf pathToFile.EndsWith(".js") Then
			context.Response.ContentType = "text/javascript"
		End If

		context.Response.Write(sData)
		context.Response.End()


	End Sub

	ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
		Get
			Return False
		End Get
	End Property

End Class