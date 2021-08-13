Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.ComponentModel

Public Class Breadcrumbs
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		Try
			Dim oPageXML As XmlDocument
			Dim sSearchMode As String = BookingBase.SearchDetails.SearchMode.ToString

			'1. Get XML
			If GetSetting(eSetting.BookingPathway).ToSafeBoolean Then
				'booking journey - from BigC

				'do we want different XML for each journey?
				If GetSetting(eSetting.SeparateXML).ToSafeBoolean Then
					oPageXML = Utility.BigCXML("BookingPathwayBreadcrumbs_" & sSearchMode, 1, 60)
				Else
					oPageXML = Utility.BigCXML("BookingPathwayBreadcrumbs", 1, 60)
				End If
			Else
				'get page xml from static/seo pages
				oPageXML = Me.PageDefinition.Content.XML
			End If


			'2. breadcrumbs xml - find the right nodes
			Dim oBreadcrumbsXML As New XmlDocument
			oBreadcrumbsXML.LoadXml(oPageXML.SelectSingleNode("//Breadcrumbs").OuterXml)


			'3. set up parameters
			Dim sCurrentPage As String = Me.PageURL.Replace("/Default.aspx", "/")
			If Not BookingBase.Params.RootPath = "" AndAlso sCurrentPage.StartsWith(BookingBase.Params.RootPath) Then
				sCurrentPage = sCurrentPage.Substring(BookingBase.Params.RootPath.Length)
			End If

			Dim sObjectType As String = ""
			If Not Me.PageDefinition.Content Is Nothing Then
				sObjectType = Me.PageDefinition.Content.ObjectType.ToSafeString
			End If

			Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("BaseURL", Functions.GetBaseURL.Replace("https", "http").Replace(":81", "").Chop(1))
				.AddParam("BookingPathway", GetSetting(eSetting.BookingPathway).ToSafeBoolean.ToString.ToLower)
				.AddParam("CurrentPage", sCurrentPage)
				.AddParam("SearchMode", sSearchMode)

				'add in extra types if extra only search and extra types set
				If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.ExtraOnly _
					AndAlso BookingBase.SearchDetails.ExtraSearch.ExtraTypeIDs.Count > 0 Then

					Dim sExtraTypes As String = ""
					For Each iExtraTypeID As Integer In BookingBase.SearchDetails.ExtraSearch.ExtraTypeIDs
						Dim sExtraType As String = BookingBase.Lookups.NameLookup(Intuitive.Web.Lookups.LookupTypes.ExtraType, _
																	  "ExtraType", "ExtraTypeID", iExtraTypeID)
						sExtraTypes = sExtraTypes & sExtraType & "|"
					Next
					sExtraTypes = sExtraTypes.Chop()

					.AddParam("ExtraTypes", sExtraTypes)

				End If

				.AddParam("UseAccessibleURLs", GetSetting(eSetting.UseAccessibleURLs).ToString.ToLower)
				.AddParam("SeperateXML", GetSetting(eSetting.SeparateXML).ToSafeBoolean.ToString.ToLower)
				.AddParam("UseNumbers", GetSetting(eSetting.UseNumbers).ToSafeBoolean.ToString.ToLower)
				.AddParam("ObjectType", sObjectType)
			End With


			Dim oBasketXML As XmlDocument = BookingBase.SearchBasket.XML

            Dim oSearchDetailsXML as XmlDocument = Serializer.Serialize(BookingBase.SearchDetails)

		    oBreadcrumbsXML = XMLFunctions.MergeXMLDocuments(oBreadcrumbsXML, oBasketXML, oSearchDetailsXML)

			'4. and transform
			If GetSetting(eSetting.TemplateOverride) <> "" Then
				Me.XSLPathTransform(oBreadcrumbsXML, HttpContext.Current.Server.MapPath("~" & GetSetting(eSetting.TemplateOverride)), writer, oXSLParams)
			Else
				Me.XSLTransform(oBreadcrumbsXML, res.Breadcrumbs, writer, oXSLParams)
			End If

		Catch ex As Exception
		End Try

	End Sub

#Region "Settings"

	Public Enum eSetting
		<Title("Booking Pathway Boolean")>
		<Description("Boolean to specify if this is a booking pathway breadcrumb")> _
		<DeveloperOnly(True)>
		BookingPathway

		<Title("Use Accessible URLs Boolean")>
		<Description("Boolean to specify whether or not to use accessible urls")>
		UseAccessibleURLs

		<Title("Separate XML Boolean")>
		<Description("Boolean to specify whether or not to use a separate xml for different search mode booking journeys")>
		<DeveloperOnly(True)>
		SeparateXML

		<Title("Use Numbers Boolean")>
		<Description("Boolean to specify whether or not to add a number to each crumb")>
		UseNumbers

		<Title("XSL Template Override")>
		<Description("File path to an overide XSL (name it breadcrumbs.xsl)")>
		<DeveloperOnly(True)>
		TemplateOverride
	End Enum

#End Region


End Class