Imports Intuitive.Functions
Imports Intuitive.Net
Imports Newtonsoft.Json.Linq
Imports System.Text

Public Class Extensions

	Private ReadOnly PageURL As Uri
	Private ReadOnly UseCache As Boolean
	Private Const CacheKeyPrefix As String = "iVectorExtensions_"
	Private Const ExtensionURL As String = "http://extensions.ivector.co.uk/?URL="

	Public Sub New(ByVal oURL As Uri)

		Me.PageURL = oURL
		Me.UseCache = Not oURL.Host.Contains("test")

	End Sub

	Public Function RetrieveExtensions() As String

		Dim sCacheKey As String = CacheKeyPrefix & PageURL.AbsolutePath
		Dim sExtensions As String = GetExtensionsFromCache(sCacheKey)

		If sExtensions Is Nothing Then

			Try

				Dim sRequest As String = ExtensionURL & PageURL.GetLeftPart(UriPartial.Path).ToLower()

				Dim oWebRequest As New WebRequests.Request
				With oWebRequest
					.EndPoint = sRequest
					.Method = WebRequests.eRequestMethod.GET
					.TimeoutInSeconds = 2
					.Send()
				End With

				sExtensions = ProcessExtensionsResponse(oWebRequest.ResponseString, oWebRequest.RequestError, sCacheKey)

			Catch ex As Exception

				sExtensions = BuildExtensionsRequestFailureComment(ex.Message)
				FileFunctions.AddLogEntry("iVectorExtensions", "RetrieveExtensionsError", ex.ToString)

			End Try

		End If

		Return sExtensions

	End Function

	Public Shared Sub ClearCache()

		Dim oItemsToRemove As New Generic.List(Of String)

		Dim oCacheEnumerator As IDictionaryEnumerator = HttpRuntime.Cache.GetEnumerator()
		While (oCacheEnumerator.MoveNext())
			Dim sKey As String = oCacheEnumerator.Key.ToString()
			If sKey.StartsWith(CacheKeyPrefix) Then
				oItemsToRemove.Add(sKey)
			End If
		End While

		For Each sItem As String In oItemsToRemove
			HttpRuntime.Cache.Remove(sItem)
		Next

	End Sub

	Private Function GetExtensionsFromCache(ByVal CacheKey As String) As String

		If UseCache Then
			Return GetCache(Of String)(CacheKey)
		End If

		Return Nothing

	End Function

	Private Function ProcessExtensionsResponse(ByVal ResponseString As String, ByVal RequestError As WebRequests.RequestError, ByVal CacheKey As String) As String

		If ResponseString <> "" Then
			Dim sExtensions As String = JToken.Parse(ResponseString).ToString()
			If UseCache Then
				AddToCache(CacheKey, sExtensions, 5)
			End If
			Return sExtensions
		End If

		Dim sError As String = RequestError.Status
		If sError = "" Then sError = "Empty response"

		FileFunctions.AddLogEntry("iVectorExtensions", "RetrieveExtensionsError", RequestError.Text)

		Return BuildExtensionsRequestFailureComment(sError)

	End Function

	Private Function BuildExtensionsRequestFailureComment(ByVal [Error] As String) As String

		Dim sbFailureComment As New StringBuilder

		With sbFailureComment
			.Append("<!--").AppendLine()
			.Append("    iVector Extensions").AppendLine()
			.Append("    Error = ").Append([Error])
			.AppendLine().Append("-->")
		End With

		Return sbFailureComment.ToString()

	End Function

End Class
