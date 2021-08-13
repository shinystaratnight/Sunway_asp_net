Imports System.Xml
Imports Intuitive


Public Class CMS


#Region "Lookup URL"

	Public Class URLContent
		Public Success As Boolean = False
		Public ObjectType As String
		Public ID As Integer
		Public URL As String
		Public XML As New System.Xml.XmlDocument
		Public URL301Redirect As String = ""
	End Class

	Public Shared Function LookupURL(ByVal URL As String) As URLContent

		'build url (in form http://../cms/url/[actual url])
		Dim sURL As String = BookingBase.Params.ServiceURL & "cms/url/" & URL.TrimStart("/"c)

		'handle 301 redirects
		Dim redirectXML As XmlDocument = Utility.BigCXML(sURL, 60)
		Dim sRedirectURL As String = ""
		If redirectXML.InnerXml.StartsWith("<URLContent>") Then

			'get redirect URL from xml
			sRedirectURL = XMLFunctions.SafeNodeValue(redirectXML, "URLContent/ContentXML/RedirectURLs/RedirectURL")

			'change the new big C url to request
			If sRedirectURL <> "" Then
				sURL = BookingBase.Params.ServiceURL & "cms/url/" & sRedirectURL.TrimStart("/"c)
			End If

		End If


		'get xml
		Dim oXML As XmlDocument = Utility.BigCXML(sURL, 60)


		'if we have content deserialize
		Dim oContent As URLContent = Nothing
		If oXML.InnerXml.StartsWith("<URLContent>") Then
			oContent = Intuitive.Serializer.DeSerialize(Of URLContent)(oXML.InnerXml)
			oContent.XML.LoadXml(oXML.SelectSingleNode("//ContentXML").InnerXml)
			oContent.Success = True

			If sRedirectURL <> "" Then
				oContent.URL301Redirect = sRedirectURL
			End If

		End If


		'if content not found, create new; success defaults to false
		If oContent Is Nothing Then
			oContent = New URLContent
		End If


		'return 
		Return oContent

	End Function


#End Region

End Class
