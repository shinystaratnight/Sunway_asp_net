Imports System.Xml
Imports Intuitive.Web

Namespace CacheControl

	Public Class Cache

		Public Shared ReadOnly Property CacheName(ByVal sCacheKey As String) As String
			Get
				Return "iVectorWidgets.CacheControl.Cache." & sCacheKey & "." & BookingBase.Params.CMSWebsiteID.ToString
			End Get
		End Property

		Public Shared Function GetCache(Of tObjectType As Class)(ByVal sCacheName As String) As tObjectType
			Return Intuitive.Functions.GetCache(Of tObjectType)(Cache.CacheName(sCacheName))
		End Function

		Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object)
			Cache.AddToCache(sCacheName, oObject, 10)
		End Sub

		Public Shared Sub AddToCache(ByVal sCacheName As String, ByVal oObject As Object, ByVal iMinutes As Integer)
			Intuitive.Functions.AddToCache(Cache.CacheName(sCacheName), oObject, iMinutes)
		End Sub

	End Class

End Namespace