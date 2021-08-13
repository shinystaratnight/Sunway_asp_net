Imports ivci = iVectorConnectInterface
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive
Imports System.Linq
Imports System.Xml
Imports System.ComponentModel

Public Class Map
	Inherits WidgetBase

#Region "Properties"

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("map_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("map_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("map_customsettings") = value
		End Set

	End Property

#End Region

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. save settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.PopupX = Functions.IIf(GetSetting(eSetting.PopupX) = "", -150, Functions.SafeInt(GetSetting(eSetting.PopupX)))
			.PopupY = Functions.IIf(GetSetting(eSetting.PopupY) = "", -30, Functions.SafeInt(GetSetting(eSetting.PopupY)))
			.ClusterPopupX = Functions.IIf(GetSetting(eSetting.ClusterPopupX) = "", -110, Functions.SafeInt(GetSetting(eSetting.ClusterPopupX)))
			.ClusterPopupY = Functions.IIf(GetSetting(eSetting.ClusterPopupY) = "", -30, Functions.SafeInt(GetSetting(eSetting.ClusterPopupY)))
			.TitleOverride = Functions.IIf(GetSetting(eSetting.TitleOverride) = "", "Map", Functions.SafeString(GetSetting(eSetting.TitleOverride)))
		End With

		Map.CustomSettings = oCustomSettings

		'2.build control
		If Functions.SafeBoolean(Me.Setting("LoadMapControl")) Then

			Dim sControlPath As String = Me.URLPath & "/map.ascx"
			Dim oControl As Control = Me.Page.LoadControl(sControlPath)

			If Functions.SafeBoolean(Me.Setting("CreateMapFromPageXML")) Then
				CType(oControl, iVectorWidgets.MapControl).MapPointsJSON = Map.CreateMapFromXML(Me.PageDefinition.Content.XML)
			End If

			CType(oControl, iVectorWidgets.MapControl).IconsJSON = Map.GetMapIcons()

			CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)
			Dim sControlHTML As String = Intuitive.Functions.RenderControlToString(oControl)

			writer.Write(sControlHTML)

		Else
			Dim oXML As New XmlDocument
			Me.XSLTransform(oXML, writer)
		End If

	End Sub


#Region "Create Map From Filtered Hotels"

	Public Shared Function CreateMapFromFilteredHotels() As String

		Dim oMapReturn As New MapReturn

		For Each oWorkTableItem As PropertyResultHandler.WorkTableItem In BookingBase.SearchDetails.PropertyResults.WorkTable

			If oWorkTableItem.Display Then

				Dim oIVCResult As ivci.Property.SearchResponse.PropertyResult = BookingBase.SearchDetails.PropertyResults.iVectorConnectResults(oWorkTableItem.Index)

				Dim oMapPoint As New iVectorWidgets.Map.MapPoint
				With oMapPoint
					.ID = oWorkTableItem.PropertyReferenceID
					.Type = Math.Floor(oWorkTableItem.Rating).ToString & "star"
					.URL = XMLFunctions.SafeNodeValue(oIVCResult.SearchResponseXML, "Property/URL")
					.Latitude = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oIVCResult.SearchResponseXML, "Property/Latitude"))
					.Longitude = Functions.SafeDecimal(XMLFunctions.SafeNodeValue(oIVCResult.SearchResponseXML, "Property/Longitude"))
				End With

				If oMapPoint.Latitude <> 0 AndAlso oMapPoint.Longitude <> 0 Then
					oMapReturn.Markers.Add(oMapPoint)
				End If

			End If

		Next

		With oMapReturn
			.PopupX = Map.CustomSettings.PopupX
			.PopupY = Map.CustomSettings.PopupY
			.ClusterPopupX = Map.CustomSettings.ClusterPopupX
			.ClusterPopupY = Map.CustomSettings.ClusterPopupY
		End With

		Dim oMapIcons As Generic.List(Of MapIcon) = Utility.XMLToGenericList(Of MapIcon)(Utility.BigCXML("MapIcons", 1, 60))
		oMapReturn.Icons = oMapIcons

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oMapReturn)

	End Function

#End Region

#Region "Get Map Icons JSON"

	Public Shared Function GetMapIcons() As String

		Dim oMapIcons As Generic.List(Of MapIcon) = Utility.XMLToGenericList(Of MapIcon)(Utility.BigCXML("MapIcons", 1, 60))
		Return Newtonsoft.Json.JsonConvert.SerializeObject(oMapIcons)

	End Function

#End Region


#Region "Create Map From XML"

	Public Shared Function CreateMapFromXML(oXML As System.Xml.XmlDocument) As String

		Dim oMapReturn As New MapReturn
		Dim oMapPoints As New Generic.List(Of iVectorWidgets.Map.MapPoint)

		Dim oNodes As System.Xml.XmlNodeList = oXML.SelectNodes("//MapPoint")

		'iterate over the nodes
		For Each oNode As System.Xml.XmlNode In oNodes

			Dim dLatitude As Decimal = Intuitive.ToSafeDecimal(Intuitive.XMLFunctions.SafeNodeValue(oNode, "Latitude"))
			Dim dLongitude As Decimal = Intuitive.ToSafeDecimal(Intuitive.XMLFunctions.SafeNodeValue(oNode, "Longitude"))

			If dLatitude <> 0 OrElse dLongitude <> 0 Then
				Dim oMapPoint As New iVectorWidgets.Map.MapPoint
				With oMapPoint
					.ID = Intuitive.ToSafeInt(Intuitive.XMLFunctions.SafeNodeValue(oNode, "ID"))
					.Type = Intuitive.XMLFunctions.SafeNodeValue(oNode, "Type")
					.URL = Intuitive.XMLFunctions.SafeNodeValue(oNode, "URL")
					.Latitude = dLatitude
					.Longitude = dLongitude
					.Name = Intuitive.XMLFunctions.SafeNodeValue(oNode, "Name")
				End With
				oMapPoints.Add(oMapPoint)
			End If
		Next

		For Each oMapPoint As MapPoint In oMapPoints
			oMapReturn.Markers.Add(oMapPoint)
		Next

		Dim oMapIcons As Generic.List(Of MapIcon) = Utility.XMLToGenericList(Of MapIcon)(Utility.BigCXML("MapIcons", 1, 60))
		oMapReturn.Icons = oMapIcons

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oMapReturn)

	End Function

#End Region


#Region "Support Classes - MapReturn, MapPoint, MapIcon"

	Public Class MapReturn
		Public Property Markers As New Generic.List(Of MapPoint)
		Public Property Icons As New Generic.List(Of MapIcon)
		Public Property PopupX As Integer
		Public Property PopupY As Integer
		Public Property ClusterPopupX As Integer
		Public Property ClusterPopupY As Integer
	End Class

	Public Class MapPoint
		Public Property Longitude As Decimal
		Public Property Latitude As Decimal
		Public Property ID As Integer
		Public Property Type As String
		Public Property URL As String
		Public Property Name As String
	End Class

	Public Class MapIcon
		Public Property Name As String
		Public Property URL As String
		Public Property Width As Integer
		Public Property Height As Integer
		Public Property AnchorX As Integer
		Public Property AnchorY As Integer
		Public Property OriginX As Integer
		Public Property OriginY As Integer
	End Class

	Public Class CustomSetting
		Public Property PopupX As Integer
		Public Property PopupY As Integer
		Public Property ClusterPopupX As Integer
		Public Property ClusterPopupY As Integer
		Public Property TitleOverride As String
	End Class

	Public Enum eSetting

		<Title("Title")>
		<Description("Title to display in widget header")>
		TitleOverride

		<Title("Popup X")>
		<Description("Map popup X position")>
		<DeveloperOnly(True)>
		PopupX

		<Title("Popup Y")>
		<Description("Map popup Y position")>
		<DeveloperOnly(True)>
		PopupY

		<Title("Custer Popup X")>
		<Description("Map cluster popup X position")>
		<DeveloperOnly(True)>
		ClusterPopupX

		<Title("Cluster Popup Y")>
		<Description("Map cluser popup Y position")>
		<DeveloperOnly(True)>
		ClusterPopupY

	End Enum

#End Region

End Class
