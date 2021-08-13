Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel

Public Class Errata
	Inherits WidgetBase

	Private _xml As XmlDocument

	Public Property XML As XmlDocument
		Get
			If Not Me._xml Is Nothing Then
				Return Me._xml
			Else
				Return New XmlDocument
			End If
		End Get
		Set(value As XmlDocument)
			If Not value Is Nothing Then
				Me._xml = value
			End If
		End Set
	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

	    Dim oBasketType As eBasketType = Functions.SafeEnum(Of eBasketType)(GetSetting(eSetting.BasketType))

		'get basket xml
        dim oBasket As new BookingBasket
	    
	    if oBasketType = eBasketType.Main Then
	        oBasket = BookingBase.Basket
        Else 
            oBasket = BookingBase.SearchBasket
	    End If
	    
	    Dim oXml As XmlDocument  = oBasket.XML()


		Me.XML = Utility.BigCXML("CustomErrata", 1, 60)

		'Wrap BigCXML and BasketXML
		oXml = Intuitive.XMLFunctions.MergeXMLDocuments(oXml, Me.XML)

		'params
		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
        
        With oXSLParams
            .AddParam("Title", GetSetting(eSetting.Title).ToSafeString)
            .AddParam("CustomErrataType", GetSetting(eSetting.CustomErrataType).ToSafeString)
            .AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
            .AddParam("DateToday", Intuitive.DateFunctions.SQLDateFormat(Today.Date))
            .AddParam("DepartureDate", Intuitive.DateFunctions.SQLDateFormat(oBasket.FirstDepartureDate))
        End With

		'transform
		If Intuitive.ToSafeString(GetSetting(eSetting.TemplateOverride).ToSafeString) <> "" Then
			Me.XSLPathTransform(oXml, HttpContext.Current.Server.MapPath("~" & GetSetting(eSetting.TemplateOverride).ToSafeString), writer, oXSLParams)
		Else
			Me.XSLTransform(oXml, XSL.SetupTemplate(res.Errata, True, True), writer, oXSLParams)
		End If


	End Sub

#Region "Settings"

	Public Enum eSetting
		<Title("Title")>
		<Description("Title text used to override what is display as the title of the widget")>
		Title

        <Title("TemplateOverride")>
        <DeveloperOnly(True)>
        <Description("Value used to specifiy override XSL document")>
        TemplateOverride

        <Title("CustomErrataType")>
        <DeveloperOnly(True)>
        <Description("The component to show custom errata for")>
        CustomErrataType

	    <Title("BasketType")>
	    <DeveloperOnly(True)>
	    <Description("The basket we're using Search or Main")>
	    BasketType
	End Enum

	Public Enum eBasketType
	    Main
	    Search
	End Enum

#End Region

End Class
