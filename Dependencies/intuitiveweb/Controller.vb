Imports System.IO
Imports System.Xml
Imports System.Text
Imports Intuitive.Functions
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports System.Web.UI
Imports Intuitive
Imports Intuitive.Web.Support
Imports Intuitive.Web.Tracking
Imports Intuitive.Web.Widgets

Public Class Controller

	Public Shared Sub SetupPage(ByVal Request As HttpRequest, ByVal Page As Master, ByVal PageServer As PageServer)

		Logging.Current.WidgetSpeeds.Clear()

		'1. get the page definition
		'call process widgets for backward compatible widget definition
		Dim oPageDef As PageDefinition = PageServer.GetPageDefinition(Request)
		oPageDef.ProcessWidgets()


		'1a set up the theme
		Dim sTheme As String = ""
		If Not BookingBase.Params Is Nothing Then
			sTheme = BookingBase.Params.Theme
		End If


		'2a. header
		Dim oHead As Head = Page.GetPageHead
		oHead.PageName = oPageDef.PageName

		'2b. save meta values
		SaveMetaValues(oHead, Page, oPageDef)

		'2c. Set Open-Graph Data
		SetOpenGraphData(oHead, Page, oPageDef)

		'3. site-wide css and js
		Dim sMasterPageName As String = Page.GetType.BaseType.Name.ToLower


		Dim sMasterCSS As String = "~/style/" & sMasterPageName & ".css"
		sMasterCSS = GetThemeURL(sMasterCSS, sTheme)
		oHead.AddCSS(sMasterCSS)


		Dim sMasterJS As String = "~/script/" & sMasterPageName & ".js"
		sMasterJS = GetThemeURL(sMasterJS, sTheme)
		oHead.AddJavaScript(sMasterJS)


		'3b. add any content picked up on the page definition
		Page.Content = oPageDef.Content

		If Not Page.Overbranding Is Nothing Then
			oPageDef.Overbranding = Page.Overbranding
		End If


		'setup tracking
		If BookingBase.Params.UseAdvancedTracking Then
			Dim oTopBody As TrackingControl = Page.GetTrackingControl("TopBody")
			Dim oBottomBody As TrackingControl = Page.GetTrackingControl("BottomBody")
			Dim tracking As New WebsiteTracking
			tracking.Setup(oPageDef, Request.RawUrl.ToLower)
			tracking.Render(oHead, oTopBody, oBottomBody)
		End If


		'4. for each section, add widgets
		For Each oSection As PageDefinition.Section In oPageDef.Sections

			Dim oContainer As Container = Page.GetContainer(oSection.Container)

			oContainer.OverrideOuterHTML = oSection.OverrideOuterHTML

			For Each oWidgetDef As PageDefinition.Widget In oSection.Widgets

				'4a work out local and url paths
				Dim sURLPath As String
				If oWidgetDef.FromLibrary Then
					sURLPath = "~/widgetslibrary/" & oWidgetDef.Name & "/"
				ElseIf oWidgetDef.ThemeSpecific Then
					sURLPath = "~/Themes/" & sTheme & "/widgets/" & oWidgetDef.Name & "/"
				Else
					sURLPath = "~/widgets/" & oWidgetDef.Name & "/"
				End If
				Dim sLocalPath As String = HttpContext.Current.Server.MapPath(sURLPath)

				'4b ensure this is a widget we want for this device
				If oWidgetDef.Devices <> Nothing AndAlso CheckDevices(oWidgetDef) Then Continue For

				'4c if booking journeys is set, only render if we're in one of the valid journeys
				If oWidgetDef.BookingJourneys <> Nothing AndAlso Not CheckBookingJourneys(oWidgetDef.BookingJourneys) Then Continue For

				'4d instantiate widget, set properties and add to container
				Dim oWidget As WidgetBase

				Select Case oWidgetDef.Type
					Case "Custom"
						If oWidgetDef.FromLibrary Then
							oWidget = PageServer.GetWidgetReference("iVectorWidgets." & oWidgetDef.Name)
						Else
							oWidget = PageServer.GetWidgetReference(oWidgetDef.Name)
						End If
					Case "HTML"
						oWidget = New HTML
					Case "RawHTML"
						oWidget = New RawHTML
					Case "SimpleCMS"
						oWidget = New SimpleCMS
					Case "BasketSQL"
						oWidget = New BasketSQL
					Case Else
						Throw New Exception("Invalid Widget Type & " & oWidgetDef.Type)
				End Select

				With oWidget
					.PageDefinition = oPageDef
					.WidgetName = oWidgetDef.Name
					.FromLibrary = oWidgetDef.FromLibrary
					.Settings = oWidgetDef.Settings
					.RawHTML = oWidgetDef.RawHTML
					.ObjectType = oWidgetDef.ObjectType
					.ObjectID = oWidgetDef.ObjectID
					.CacheMinutes = oWidgetDef.CacheMinutes
					.URLPath = sURLPath
					.LocalPath = sLocalPath
					.LoginStatus = SafeEnum(Of WidgetBase.LoginState)(oWidgetDef.LoginStatus.ToString)
				End With


				'4e Check the login status of the widget matches(or is not set before we render hte widget)
				If CheckLoginStatus(oWidget) Then
					oContainer.Controls.Add(oWidget)
				End If


				'4f Add the CSS
				If Not BookingBase.Params.ExcludeWidgetCSS Then
					AddCSS(sURLPath, oWidgetDef, oHead, sTheme)
				End If


				'4g Add the JS
				AddJavaScript(sURLPath, oWidgetDef, oHead, sTheme)

			Next
		Next


		'5. hide containers that are not needed
		For Each oSection As PageDefinition.Section In oPageDef.ExcludeSections
			Dim oContainer As Container = Page.GetContainer(oSection.Container)
			oContainer.Visible = False
		Next

        '6. Add Extensions
		Dim oExtensions As New Extensions(Page.Request.Url)
		Dim sExtensions As String = oExtensions.RetrieveExtensions()

	    If not String.IsNullOrEmpty(sExtensions) AndAlso Page.ContainerExists("Footer") Then
	       	Dim oFooterContainer As Container = Page.GetContainer("Footer")
		    Dim oControl As New LiteralControl(sExtensions)
		    oFooterContainer.Controls.Add(oControl) 
	    End If

	End Sub


#Region "getpagedetails"


	Private Shared Function GetPageDetails(ByVal Request As HttpRequest) As PageDefinition

		'load up xml
		Dim oXML As New XmlDocument
		oXML.Load(HttpContext.Current.Server.MapPath("/pagedefinition.xml"))


		'get url from request
		Dim sPath As String = Request.Url.AbsolutePath.ToLower
		sPath = sPath.Replace("default.aspx", "")


		'get page
		Dim oNode As XmlNode = oXML.SelectSingleNode(String.Format("/Pages/PageDefinition[URL='{0}']", sPath))
		If oNode Is Nothing Then
			Throw New Exception("Could not find page for path " & sPath)
		End If


		Dim oPage As PageDefinition
		oPage = Serializer.DeSerialize(Of PageDefinition)(oNode.OuterXml)


		Return oPage

	End Function


#End Region

#Region "Load CSS/JS"

	Public Shared Sub AddCSS(ByVal sURLPath As String, ByVal oWidgetDef As PageDefinition.Widget, ByVal oHead As Head, ByVal Theme As String)


		Try

			'1a set up the theme
			Dim sTheme As String = IIf(oWidgetDef.ThemeSpecific, Theme, "").ToString


			'1b Set sCss to its default value
			Dim sCSS As String

			Dim sWidgetsCSS As String = "~/widgets/" & oWidgetDef.Name & "/" & oWidgetDef.Name & ".css"

			'2. If this is not a library widget, and does not inherit one, load css from local folder 
			If oWidgetDef.FromLibrary OrElse Not String.IsNullOrEmpty(oWidgetDef.ParentWidget) Then


				'2a. If we are a library widget, or inheriting from one, we still might want to override the default CSS,
				'width one in the local project.
				Dim bUseWidgetCSS As Boolean
				bUseWidgetCSS = File.Exists(HttpContext.Current.Server.MapPath(GetThemeURL(sWidgetsCSS, sTheme)))

				If bUseWidgetCSS Then
					sCSS = sWidgetsCSS
				Else

					'2b. If we are an inherited widget, the parent widget might have a different name so use that to find the default css.
					If oWidgetDef.ParentWidget <> "" Then
						sCSS = "~/widgetslibrary/" & oWidgetDef.ParentWidget & "/" & oWidgetDef.ParentWidget & ".css"
					Else
						sCSS = "~/widgetslibrary/" & oWidgetDef.Name & "/" & oWidgetDef.Name & ".css"
					End If

				End If

			Else
				sCSS = sWidgetsCSS

			End If

			sCSS = GetThemeURL(sCSS, sTheme)

			oHead.AddCSS(sCSS)

		Catch ex As Exception

			Dim sCSSInfo As New StringBuilder
			sCSSInfo.AppendLine("URLPath: " & sURLPath)
			sCSSInfo.AppendLine(" ")
			sCSSInfo.AppendLine(Serializer.Serialize(oWidgetDef).InnerXml)

			Logging.LogError("Controller", "AddCSS", ex, sCSSInfo.ToString)

		End Try

	End Sub

	Public Shared Sub AddJavaScript(ByVal sURLPath As String, ByVal oWidgetDef As PageDefinition.Widget, ByVal oHead As Head, ByVal Theme As String)

		'1a set up the theme
		Dim sTheme As String = IIf(oWidgetDef.ThemeSpecific, Theme, "").ToString


		'2. If its a library widget or a parent - then load this JS first
		' child JS comes after so we can extend/override methods
		Dim sJavaScriptExtended As String
		Dim sJavaScript As String = sURLPath & oWidgetDef.Name & ".js"

		If oWidgetDef.FromLibrary OrElse oWidgetDef.ParentWidget <> "" Then

			'3b. If its a library widget, we want to check for an additional Javascript file (in case we're extending it)
			If oWidgetDef.FromLibrary Then
				If BookingBase.Params.UseTranspiledResources Then
					sJavaScriptExtended = "~/assets/script/" & oWidgetDef.Name & "/" & oWidgetDef.Name & ".js"
				Else
					sJavaScriptExtended = "~/widgets/" & oWidgetDef.Name & "/" & oWidgetDef.Name & ".js"
				End If
			Else
				'3c. If its a descendant of a library widget, we want to try and load the parents JS as well
				sJavaScriptExtended = "~/widgetslibrary/" & oWidgetDef.ParentWidget & "/" & oWidgetDef.ParentWidget & ".js"
			End If

			sJavaScriptExtended = GetThemeURL(sJavaScriptExtended, sTheme)

			If File.Exists(HttpContext.Current.Server.MapPath(sJavaScriptExtended)) Then
				oHead.AddJavaScript(sJavaScriptExtended)
			End If

			'3. Load the widgets default Javascript if it exists
			sJavaScript = GetThemeURL(sJavaScript, sTheme)

			If File.Exists(HttpContext.Current.Server.MapPath(sJavaScript)) Then
				oHead.AddJavaScript(sJavaScript)
			End If
		Else
			If BookingBase.Params.UseTranspiledResources Then
				sJavaScriptExtended = "~/assets/script/" & oWidgetDef.Name & "/" & oWidgetDef.Name & ".js"
				sJavaScriptExtended = GetThemeURL(sJavaScriptExtended, sTheme)

				If File.Exists(HttpContext.Current.Server.MapPath(sJavaScriptExtended)) Then
					oHead.AddJavaScript(sJavaScriptExtended)
				End If
			Else
				'3. Load the widgets default Javascript if it exists
				sJavaScript = GetThemeURL(sJavaScript, sTheme)

				If File.Exists(HttpContext.Current.Server.MapPath(sJavaScript)) Then
					oHead.AddJavaScript(sJavaScript)
				End If
			End If
		End If

	End Sub

#End Region


#Region "Check Devices"

	Public Shared Function CheckDevices(ByVal oWidgetDef As PageDefinition.Widget) As Boolean

		'A boolean to track whether or not the widget should be rendered
		Dim DoNotRender As Boolean = False

		Dim bIsMobileDevice As Boolean = BrowserSupport.IsMobile
		Dim bIsTabletDevice As Boolean = BrowserSupport.IsTablet

		'If we're a mobile device and the widget is not for mobile devices, dont render
		If bIsMobileDevice AndAlso Not oWidgetDef.Devices.ToLower.Contains("mobile") Then
			DoNotRender = True
		End If

		'If we're a tablet device and the widget is not for tablet devices, dont render
		If bIsTabletDevice AndAlso Not oWidgetDef.Devices.ToLower.Contains("tablet") Then
			DoNotRender = True
		End If

		'If we're on a desktop device and the widget is not for desktop devices, dont render
		If Not bIsMobileDevice AndAlso Not bIsTabletDevice AndAlso Not oWidgetDef.Devices.ToLower.Contains("desktop") Then
			DoNotRender = True
		End If

		Return DoNotRender

	End Function

#End Region

#Region "Check LoginStatus"
	Private Shared Function CheckLoginStatus(ByVal oWidget As WidgetBase) As Boolean


		'Check the login status property(used to control if a widget should or not show when logged in/out)
		Select Case oWidget.LoginStatus

			'If none set return true
			Case WidgetBase.LoginState.None
				Return True

			Case WidgetBase.LoginState.LoggedIn
				'If logged in, they must be logged in
				Return BookingBase.LoggedIn

			Case WidgetBase.LoginState.LoggedOut
				'If logged out they must not be logged in
				Return Not BookingBase.LoggedIn

		End Select

		Return True

	End Function

#End Region

#Region "Check Booking Journeys"

	Public Shared Function CheckBookingJourneys(ByVal sBookingJourneys As String) As Boolean

		Dim bValid As Boolean = sBookingJourneys.Contains(BookingBase.SearchDetails.SearchMode.ToString)

		If BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.ExtraOnly Then
			Dim sExtraTypesKeyword As String = "#ExtraTypes:"
			If bValid AndAlso sBookingJourneys.Contains(sExtraTypesKeyword) Then
				Dim iPosition As Integer = InStr(sBookingJourneys, "#ExtraTypes:") - 1
				Dim sExtraTypes As String = sBookingJourneys.Substring(iPosition + sExtraTypesKeyword.Length)
				Dim aExtraTypes() As String = sExtraTypes.Split(","c)

				bValid = CheckExtraTypes(aExtraTypes)

			End If
		End If

		Return bValid

	End Function

	Public Shared Function CheckExtraTypes(ByVal aExtraTypes As String()) As Boolean

		Dim bValidType As Boolean = False

		For Each sExtraType As String In aExtraTypes
			Dim iExtraTypeID As Integer = BookingBase.Lookups.GetKeyPairID(Lookups.LookupTypes.ExtraType, sExtraType)

			If BookingBase.SearchDetails.ExtraSearch.ExtraTypeIDs.Contains(iExtraTypeID) Then
				bValidType = True
				Exit For
			End If

			If BookingBase.SearchDetails.ExtraSearch.ExtraTypeIDs.Count = 0 AndAlso BookingBase.SearchDetails.ExtraSearch.ExtraID <> 0 Then
				For Each oExtra As BookingExtra.BasketExtra In BookingBase.Basket.BasketExtras
					If oExtra.BasketExtraOptions.Count <> 0 AndAlso
					 oExtra.BasketExtraOptions(0).ExtraID = BookingBase.SearchDetails.ExtraSearch.ExtraID AndAlso
					 oExtra.BasketExtraOptions(0).ExtraTypeID = iExtraTypeID Then

						bValidType = True
						Exit For

					End If
				Next
				For Each oExtra As BookingExtra.BasketExtra In BookingBase.SearchBasket.BasketExtras
					If oExtra.BasketExtraOptions.Count <> 0 AndAlso
					 oExtra.BasketExtraOptions(0).ExtraID = BookingBase.SearchDetails.ExtraSearch.ExtraID AndAlso
					 oExtra.BasketExtraOptions(0).ExtraTypeID = iExtraTypeID Then

						bValidType = True
						Exit For

					End If
				Next
			End If

		Next

		Return bValidType
	End Function

#End Region

#Region "support - getthemeurl, getwidgetreference"


	'get theme url
	Private Shared Function GetThemeURL(ByVal BaseURL As String, ByVal Theme As String) As String


		Dim sThemeFile As String

		'1. if we have a theme and the file exists, override the baseurl
		If Theme <> "" Then
			Dim sThemeURL As String = "~/Themes/" & Theme & BaseURL.Replace("~", "")
			sThemeFile = HttpContext.Current.Server.MapPath(sThemeURL)
			If File.Exists(sThemeFile) Then
				Return sThemeURL
			End If
		End If


		'2. if theme empty or no 'themed file', then go for default
		sThemeFile = HttpContext.Current.Server.MapPath(BaseURL)
		If File.Exists(sThemeFile) Then
			Return BaseURL
		End If


		'3. no file found, return empty string
		Return ""

	End Function



#End Region

#Region "Save Meta Values"

	Public Shared Sub SaveMetaValues(ByVal Head As Head, ByVal Page As Master, ByVal PageDef As PageDefinition)

		'get the XML once - if it needs it
		Dim oXML As New XmlDocument
		If PageDef.Head.FromXML Then
			If PageDef.Head.ObjectType <> "" Then
				oXML = Utility.BigCXML(PageDef.Head.ObjectType, 1, 60)
			ElseIf Not PageDef.Content Is Nothing Then
				oXML = PageDef.Content.XML
			End If
		End If

		Dim aProperties() As String = {"Title", "Description", "KeyWords", "CanonicalURL"}

		For Each sProperty As String In aProperties
			Dim sValue As String = ""
			Try
				'check XML if needed, else get from page def
				If PageDef.Head.FromXML Then
					sValue = XMLFunctions.SafeNodeValue(oXML, SafeString(CallByName(PageDef.Head, sProperty, CallType.Get)))
				Else
					sValue = SafeString(CallByName(PageDef.Head, sProperty, CallType.Get))
				End If
			Catch ex As Exception
				'do nothing, this is fine - we don't want to fall over because a meta property has failed
			End Try

			CallByName(Head, sProperty, CallType.Set, sValue)
		Next

	End Sub

#End Region

#Region "Set Open Graph Data"

	Public Shared Sub SetOpenGraphData(ByVal Head As Head, ByVal Page As Master, ByVal PageDef As PageDefinition)

		Dim oXML As New XmlDocument
		If PageDef.Head.OpenGraphXPath.ToSafeString <> "" Then
			Try

				oXML = PageDef.Content.XML

				Dim sOpenGraphNode As String = XMLFunctions.SafeOuterXML(oXML, PageDef.Head.OpenGraphXPath)
				If sOpenGraphNode <> "" Then
					Head.OpenGraph = Serializer.DeSerialize(Of OpenGraph)(sOpenGraphNode)
				End If

			Catch ex As Exception
				'do nothing, if this failed then the node probably doesn't exist in the page xml
			End Try
		End If

	End Sub

#End Region

End Class


