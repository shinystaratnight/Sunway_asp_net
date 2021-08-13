Imports System.Web
Imports System.Web.Compilation
Imports System.Web.Routing

Public Class HttpHandlerRouteHandler(Of T As {IHttpHandler, New})
	Implements IRouteHandler

	Public Sub New()
	End Sub

	Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler
		Return New T()
	End Function
End Class

Public Class HttpHandlerRouteHandler
	Implements IRouteHandler

	Private _VirtualPath As String

	'can map to specific handler if needed
	Public Sub New(handler As String)
		Me._VirtualPath = String.Format("~/Services/{0}.ashx", handler)

	End Sub

	Public Sub New()
	End Sub

	Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler

		If Me._VirtualPath = "" Then
			Me._VirtualPath = String.Format("~/Services/{0}.ashx", requestContext.RouteData.Values("handler").ToString)
		End If

		Return DirectCast(BuildManager.CreateInstanceFromVirtualPath(Me._VirtualPath, GetType(IHttpHandler)), IHttpHandler)
	End Function

End Class