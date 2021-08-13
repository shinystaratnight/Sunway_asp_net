Imports Intuitive.Functions
Imports Intuitive.Web.Widgets
Imports Intuitive.Web
Imports Intuitive.XMLFunctions
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json

Public Class CarHire
    Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("carhire_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("carhire_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("carhire_customsettings") = value
		End Set

	End Property

	Public Shared Property RenderWidget As Boolean = True

    Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

        '1. Set up custom settings
        Dim oCustomSettings As New CustomSetting
        With oCustomSettings
            .TextOverrides = SafeString(Settings.GetValue("TextOverrides"))
            .Title = SafeString(Settings.GetValue("Title"))
            .SubTitle = SafeString(Settings.GetValue("SubTitle"))
            .TemplateOverride = SafeString(Settings.GetValue("TemplateOverride"))
            .ResultsTemplateOverride = SafeString(Settings.GetValue("ResultsTemplateOverride"))
            .PopupTemplateOverride = SafeString(Settings.GetValue("PopupTemplateOverride"))
            .SelectedTemplateOverride = SafeString(Settings.GetValue("SelectedTemplateOverride"))
            .CSSClassOverride = SafeString(Settings.GetValue("CSSClassOverride"))
            .SearchMode = SafeString(Settings.GetValue("SearchMode"))
            .AutomaticSearchType = SafeString(Settings.GetValue("AutomaticSearchType"))
            .LeadDriverAge = SafeInt(Settings.GetValue("LeadDriverAge"))
            .LeadDriverBookingCountryID = SafeInt(Settings.GetValue("LeadDriverBookingCountryID"))
            .TransferOrCarHireOnly = SafeBoolean(Settings.GetValue("TransferOrCarHireOnly"))
            .TransferOrCarHireOnlyWarning = SafeString(Settings.GetValue("TransferOrCarHireOnlyWarning"))
            .SearchWarning = SafeString(Settings.GetValue("SearchWarning"))
            .ControlOverride = SafeString(Settings.GetValue("ControlOverride"))
            .ListClass = SafeString(Settings.GetValue("ListClass"))
            .ListItemClass = SafeString(Settings.GetValue("ListItemClass"))
            .UseTheme = SafeBoolean(Settings.GetValue("UseTheme"))
            .HiddenSearchTool = SafeBoolean(Settings.GetValue("HiddenSearchTool"))
            .PriceFormat = Intuitive.Functions.IIf(Settings.GetValue("PriceFormat") = "", "###,##0.00", Settings.GetValue("PriceFormat"))
            .UseBasketSelectedCarHire = Intuitive.ToSafeBoolean(Settings.GetValue("UseBasketSelectedCarHire"))
            .Redirect = Intuitive.ToSafeBoolean(Settings.GetValue("Redirect"))
            .RedirectUrl = Settings.GetValue("RedirectUrl")
            .StartExpanded = SafeBoolean(Settings.GetValue("StartExpanded"))
        End With

        CarHire.CustomSettings = oCustomSettings

        'If we have a Manual Search then load the Control, otherwise load the XSL
        If CarHire.CustomSettings.SearchMode = "Manual" Then
            If SafeString(Settings.GetValue("ControlOverride")) <> "" Then Me.URLPath = Settings.GetValue("ControlOverride")
            Dim sControlPath As String

            If Me.URLPath.EndsWith(".ascx") Then
                sControlPath = Me.URLPath
            Else
                sControlPath = Me.URLPath & "/" & "carhire.ascx"
            End If


            Dim oControl As New Control
            Try
                oControl = Me.Page.LoadControl(sControlPath)
            Catch ex As Exception
                oControl = Me.Page.LoadControl("/widgetslibrary/CarHire/CarHire.ascx")
            End Try

            CType(oControl, iVectorWidgets.UserControlBase).ApplySettings(Me.Settings)

            If RenderWidget then
                DrawControl(writer, oControl)
            End If
        Else
            Dim oXML As New System.Xml.XmlDocument

            Dim oXSLParams As Intuitive.WebControls.XSL.XSLParams = XSLParams()

            If CarHire.CustomSettings.TemplateOverride <> "" Then
                Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.TemplateOverride), writer, oXSLParams)
            Else
                Me.XSLTransform(oXML, XSL.SetupTemplate(res.CarHire, True, False), writer, oXSLParams)
            End If
        End If

    End Sub

    Public Shared Function XSLParams() As Intuitive.WebControls.XSL.XSLParams

        Dim sSelectedCarHire As String = ""

        If Not BookingBase.SearchBasket.BasketCarHires.Count = 0 AndAlso Not BookingBase.SearchBasket.BasketCarHires(0) Is Nothing Then
            sSelectedCarHire = BookingBase.SearchBasket.BasketCarHires(0).CarHire.BookingToken
        End If

        Dim iTotalPassengers As Integer = BookingBase.SearchBasket.TotalAdults + BookingBase.SearchBasket.TotalChildren + BookingBase.SearchBasket.TotalInfants

        Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
        With oXSLParams
            .AddParam("Title", CarHire.CustomSettings.Title)
            .AddParam("SubTitle", Intuitive.Web.Translation.GetCustomTranslation("Car Hire", CarHire.CustomSettings.SubTitle))
            .AddParam("Theme", BookingBase.Params.Theme)
            .AddParam("CurrencySymbol", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID))
            .AddParam("CurrencySymbolPosition", BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID))
            .AddParam("CSSClassOverride", CarHire.CustomSettings.CSSClassOverride)
            .AddParam("SearchMode", CarHire.CustomSettings.SearchMode)
            .AddParam("AutomaticSearchType", CarHire.CustomSettings.AutomaticSearchType)
            .AddParam("SearchWarning", Intuitive.Web.Translation.GetCustomTranslation("Car Hire", CarHire.CustomSettings.SearchWarning))
            .AddParam("ListClass", CarHire.CustomSettings.ListClass)
            .AddParam("ListItemClass", CarHire.CustomSettings.ListItemClass)
            .AddParam("PriceFormat", CarHire.CustomSettings.PriceFormat)
            .AddParam("UseBasketCarHire", CarHire.CustomSettings.UseBasketSelectedCarHire)
            .AddParam("SelectedCarHireToken", sSelectedCarHire)
            .AddParam("PaxCount", iTotalPassengers)
            .AddParam("StartExpanded", CarHire.CustomSettings.StartExpanded)
        End With

        Return oXSLParams
    End Function

    Public Shared Function GetCarHireHTML(Optional ByVal sSearchRequest As String = "") As String

        Dim oCarHireSearchReturn As New CarHireSearchReturn

        Try
            'do our search
            Dim oCarHireSearch As BookingSearch.SearchReturn = SearchCarHire(sSearchRequest)

            'check if we have results
            If BookingBase.SearchDetails.CarHireResults.TotalCarHires > 0 Then
                Dim oXML As XmlDocument = BookingBase.SearchDetails.CarHireResults.GetResultsXML
                If CarHire.CustomSettings.ResultsTemplateOverride <> "" Then
                    oCarHireSearchReturn.HTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.ResultsTemplateOverride), CarHire.XSLParams()))
                Else
                    oCarHireSearchReturn.HTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.CarHireResults, True, True), CarHire.XSLParams()))
                End If
            Else
                oCarHireSearchReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry there are no car hires available for your holiday combination.")
            End If

            'success if car hire returned
            oCarHireSearchReturn.Success = BookingBase.SearchDetails.CarHireResults.TotalCarHires > 0

        Catch ex As Exception
            oCarHireSearchReturn.Success = False
            oCarHireSearchReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry, there was an unexpected error during the search")
        End Try

        Return Newtonsoft.Json.JsonConvert.SerializeObject(oCarHireSearchReturn)

    End Function

    Public Shared Function GetCarHireHTMLWithoutSearch() As String

        Dim oCarHireSearchReturn As New CarHireSearchReturn

        Try

            'check if we have results
            If BookingBase.SearchDetails.CarHireResults.TotalCarHires > 0 Then
                Dim oXML As XmlDocument = BookingBase.SearchDetails.CarHireResults.GetResultsXML
                If CarHire.CustomSettings.ResultsTemplateOverride <> "" Then
                    oCarHireSearchReturn.HTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.ResultsTemplateOverride), CarHire.XSLParams()))
                Else
                    oCarHireSearchReturn.HTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.CarHireResults, True, True), CarHire.XSLParams()))
                End If
            Else
                oCarHireSearchReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry there are no car hires available for your holiday combination.")
            End If

            'success if car hire returned
            oCarHireSearchReturn.Success = BookingBase.SearchDetails.CarHireResults.TotalCarHires > 0

        Catch ex As Exception
            oCarHireSearchReturn.Success = False
            oCarHireSearchReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry, there was an unexpected error during the search")
        End Try

        Return Newtonsoft.Json.JsonConvert.SerializeObject(oCarHireSearchReturn)

    End Function

    Public Shared Function GetSelectedCarHireAndExtras() As String

        Dim sHTML As String = ""

        If BookingBase.SearchBasket.BasketCarHires.Count > 0 Then
            Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(BookingBase.SearchBasket.BasketCarHires(0))

            If CustomSettings.SelectedTemplateOverride <> "" Then
                sHTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.SelectedTemplateOverride), CarHire.XSLParams()))
            ElseIf CustomSettings.UseTheme Then
                Dim sTheme As String = BookingBase.Params.Theme
                Dim sXSLPath As String = HttpContext.Current.Server.MapPath("Themes/" & sTheme & "/widgets/" & "CarHire" & "/SelectedCarHire.xsl")
                sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, sXSLPath, CarHire.XSLParams())
            Else
                sHTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.SelectedCarHire, True, True), CarHire.XSLParams()))
            End If


        End If

        Return sHTML

    End Function

    Public Shared Function SearchCarHire(Optional ByVal sSearchRequest As String = "") As BookingSearch.SearchReturn

        Dim oSearchRequest As New BookingCarHire.CarHireSearch

        'check our search mode and adjust details accordingly or use supplied search details from json string
        'manual if we are using a car hire search tool
        If CarHire.CustomSettings.SearchMode = "Manual" And sSearchRequest <> "" Then
            oSearchRequest = JsonConvert.DeserializeObject(Of BookingCarHire.CarHireSearch)(sSearchRequest, New Converters.IsoDateTimeConverter() With { _
             .DateTimeFormat = "dd/MM/yyyy" _
              })

        Else
            'set some base values
            oSearchRequest.TotalPassengers = BookingBase.SearchBasket.TotalAdults + BookingBase.SearchBasket.TotalChildren + BookingBase.SearchBasket.TotalInfants
            oSearchRequest.LeadDriverBookingCountryID = IIf(CarHire.CustomSettings.LeadDriverBookingCountryID <> 0, CarHire.CustomSettings.LeadDriverBookingCountryID, 1)

            'set a lead driver
            Dim oDriverAges As New Generic.List(Of Integer)
            oDriverAges.Add(IIf(CarHire.CustomSettings.LeadDriverAge <> 0, CarHire.CustomSettings.LeadDriverAge, 26))

            oSearchRequest.DriverAges = oDriverAges

            'get the rest from the basket depending on the auto search type - currently only one default mode (from airport)
            If CarHire.CustomSettings.AutomaticSearchType = "AirportDepot" Then

                'get our depots
                Dim oDepots As Generic.List(Of Lookups.CarHireDepot) = BookingBase.Lookups.GetDepotsByAirportID(BookingBase.SearchBasket.BasketFlights(0).Flight.ArrivalAirportID)

                oSearchRequest.PickUpDepotID = oDepots.First.CarHireDepotID
                oSearchRequest.DropOffDepotID = oDepots.First.CarHireDepotID
                oSearchRequest.PickUpDate = BookingBase.SearchBasket.BasketFlights(0).Flight.OutboundDepartureDate
                oSearchRequest.PickUpTime = BookingBase.SearchBasket.BasketFlights(0).Flight.OutboundArrivalTime
                oSearchRequest.DropOffDate = BookingBase.SearchBasket.BasketFlights(0).Flight.ReturnDepartureDate
                oSearchRequest.DropOffTime = BookingBase.SearchBasket.BasketFlights(0).Flight.ReturnDepartureTime

            End If
        End If

        'set these after the deserialization or they get wiped out
        oSearchRequest.LoginDetails = BookingBase.IVCLoginDetails
        oSearchRequest.CustomerIP = HttpContext.Current.Request.UserHostAddress

        'do the search
        Dim oCarHireSearch As BookingSearch.SearchReturn = BookingCarHire.Search(oSearchRequest)

        'save the search request if it returns results
        If oCarHireSearch.CarHireResults.Count > 0 Then
            BookingBase.SearchDetails.CarHireSearch = oSearchRequest
        End If


        Return oCarHireSearch

    End Function

    Public Shared Function AddCarHireToBasket(ByVal HashToken As String) As String

        Dim oAddCarHireReturn As New AddCarHireReturn With {.Success = False}

        Try
            If CarHire.CustomSettings.TransferOrCarHireOnly AndAlso BookingBase.SearchBasket.BasketTransfers.Count > 0 Then
                'if the setting is on and we already have a transfer, dont allow car hire to be added
                oAddCarHireReturn.Success = False
                oAddCarHireReturn.Warnings.Add(IIf(CarHire.CustomSettings.TransferOrCarHireOnlyWarning <> "", _
                 CarHire.CustomSettings.TransferOrCarHireOnlyWarning, Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "You cannot add car hire as you already have a transfer")))
            Else
                'add car hire to basket
                BookingCarHire.AddCarHireToBasket(HashToken, True)

                'return
                oAddCarHireReturn.Success = True
            End If

        Catch ex As Exception
            oAddCarHireReturn.Warnings.Add(Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry, there was an unexpected error adding this car"))
        End Try

        With oAddCarHireReturn
            .TotalPriceUnformated = BookingBase.SearchBasket.TotalPrice
        End With
        oAddCarHireReturn.Redirect = CarHire.CustomSettings.Redirect
        oAddCarHireReturn.RedirectUrl = CarHire.CustomSettings.RedirectUrl


        Return Newtonsoft.Json.JsonConvert.SerializeObject(oAddCarHireReturn)

    End Function

    Public Function Filter(ByVal sFilterJSON As String) As String

        Dim oFilterReturn As New FilterReturn

        Try
            Dim oFilter As New BookingCarHire.CarHireResults.Filters
            oFilter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of BookingCarHire.CarHireResults.Filters)(sFilterJSON)

            Dim oFilteredResults As BookingCarHire.CarHireResults
            oFilteredResults = BookingCarHire.CarHireResults.Filter(oFilter)

            Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oFilteredResults, True)

            If oFilteredResults.TotalCarHires > 0 Then
                If CarHire.CustomSettings.ResultsTemplateOverride <> "" Then
                    oFilterReturn.HTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.ResultsTemplateOverride), CarHire.XSLParams()))
                Else
                    oFilterReturn.HTML = Intuitive.Web.Translation.TranslateHTML(Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.CarHireResults, True, True), CarHire.XSLParams()))
                End If
            Else
                oFilterReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry there are no car hires available matching your criteria.")
            End If
            
            oFilterReturn.Success = oFilteredResults.TotalCarHires > 0

        Catch ex As Exception
            oFilterReturn.Success = False
            oFilterReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry, there was an unexpected error during filtering")
        End Try

        Return Newtonsoft.Json.JsonConvert.SerializeObject(oFilterReturn)

    End Function

    Public Function AddExtrasToBasket(extrasJson As String) As String

        Dim Extras As New Generic.List(Of String)
        Extras = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(Of Generic.List(Of String))(extrasJson, Extras)

        BookingCarHire.CarHireResults.UpdateCarHireExtras(Extras)

        Dim sHTML As String = ""
        Try
            sHTML = GetSelectedCarHireAndExtras()
        Catch ex As Exception
            sHTML = ""
        End Try

        Return sHTML

    End Function

    Public Function AddCarHireAndExtrasToBasket(index As Integer, extrasJson As String) As String

        Dim oAddCarHireReturn As New AddCarHireReturn With {.Success = False}
        Dim oExtras as New Generic.List(Of String)
        Dim ExtraIndexes As New Generic.List(Of Integer)
        ExtraIndexes = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(Of Generic.List(Of Integer))(extrasJson, ExtraIndexes)

        Dim sCarHireToken as String
        sCarHireToken = BookingBase.SearchDetails.CarHireResults.CarHires(index).CarHireOptionHashToken

        Try
            BookingCarHire.AddCarHireToBasket(sCarHireToken, True)

            if BookingBase.SearchBasket.BasketCarHires.Count > 0 AndAlso ExtraIndexes.Count > 0 Then
                For Each i as Integer in ExtraIndexes
                    Dim sToken as String
                    sToken = BookingBase.SearchDetails.CarHireResults.CarHires(index).CarHireExtras(i).ExtraBookingToken

                    oExtras.Add(sToken & "###1")
                Next

                BookingCarHire.CarHireResults.UpdateCarHireExtras(oExtras)
            End If

            'return
            oAddCarHireReturn.Success = True

        Catch ex As Exception
            oAddCarHireReturn.Warnings.Add(Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Sorry, there was an unexpected error adding this car"))
        End Try

        Return Newtonsoft.Json.JsonConvert.SerializeObject(oAddCarHireReturn)

    End Function

    Public Shared Function RemoveCarHire() As String

        Dim oRemoveCarHireReturn As New RemoveCarHireReturn

        'we don't need to update the basket if there is no car hire in there
        If BookingBase.SearchBasket.BasketCarHires.Count > 0 Then
            oRemoveCarHireReturn.UpdateBasket = "True"
        Else
            oRemoveCarHireReturn.UpdateBasket = "False"
        End If

        'clear car hire
        BookingBase.SearchBasket.BasketCarHires.Clear()

        'return
        oRemoveCarHireReturn.Success = True
        Return Newtonsoft.Json.JsonConvert.SerializeObject(oRemoveCarHireReturn)

    End Function

    Public Shared Function InformationPrebook(ByVal sBookingToken As String) As String

        Dim oInfoPrebookReturn As New InfoPrebookReturn With {.Success = False}

        Try

            'set the prebook request up
            Dim oInfoPrebookRequest As New iVectorConnectInterface.CarHire.PreBookRequest
            oInfoPrebookRequest.LoginDetails = BookingBase.IVCLoginDetails
            oInfoPrebookRequest.BookingToken = sBookingToken

            'send it off
            Dim oPreBookReturn As BookingCarHire.InformationPreBookReturn
            oPreBookReturn = BookingCarHire.InformationPreBook(oInfoPrebookRequest)

            Dim oPreBookReturnXML As XmlDocument = Intuitive.Serializer.Serialize(oPreBookReturn, True)

            If CarHire.CustomSettings.PopupTemplateOverride <> "" Then
                oInfoPrebookReturn.HTML = Intuitive.XMLFunctions.XMLTransformToString(oPreBookReturnXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.PopupTemplateOverride), CarHire.XSLParams())
            Else
                oInfoPrebookReturn.HTML = Intuitive.XMLFunctions.XMLStringTransformToString(oPreBookReturnXML, XSL.SetupTemplate(res.CarHireResults, True, True), CarHire.XSLParams())
            End If

            If oPreBookReturn.OK Then
                oInfoPrebookReturn.Success = True
            End If

        Catch ex As Exception
            oInfoPrebookReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Failed to retrieve rental conditions")
        End Try


        Return Newtonsoft.Json.JsonConvert.SerializeObject(oInfoPrebookReturn)

    End Function

    Public Shared Function AdditionalInformation(ByVal Index As Integer) As String
        
        Dim oAdditionalInfoReturn As New AdditionalInfoReturn With {.Success = False}

        Try
            Dim oCarHireOption as BookingCarHire.CarHireResults.CarHire
            oCarHireOption = BookingBase.SearchDetails.CarHireResults.CarHires(index)

            dim oCarHireXML As XmlDocument
            oCarHireXML = Intuitive.Serializer.Serialize(oCarHireOption, True)

            If CarHire.CustomSettings.PopupTemplateOverride <> "" Then
                oAdditionalInfoReturn.HTML = Intuitive.XMLFunctions.XMLTransformToString(oCarHireXML, HttpContext.Current.Server.MapPath("~" & CarHire.CustomSettings.PopupTemplateOverride), CarHire.XSLParams())
            Else
                oAdditionalInfoReturn.HTML = Intuitive.XMLFunctions.XMLStringTransformToString(oCarHireXML, XSL.SetupTemplate(res.CarHireResults, True, True), CarHire.XSLParams())
            End If

            oAdditionalInfoReturn.Success = True

        Catch ex As Exception
            oAdditionalInfoReturn.Warning = Intuitive.Web.Translation.GetCustomTranslation("Car Hire", "Failed to retrieve rental information")
        End Try

        Return Newtonsoft.Json.JsonConvert.SerializeObject(oAdditionalInfoReturn)

    End Function

#Region "Car Hire Depot HTML"

    Public Shared Function GetCarHireDepotHTML(ByVal GeographyLevel1ID As Integer) As String


        Dim oCarHireDepots As Lookups.KeyValuePairs = BookingBase.Lookups.ListCarHireDepotsByCountryID(GeographyLevel1ID)

        Dim sbHTML As New StringBuilder
        sbHTML.AppendLine("<option>Select Depot...</option>")
        For Each Lookup As Generic.KeyValuePair(Of Integer, String) In oCarHireDepots.OrderBy(Function(o) o.Value)
            sbHTML.AppendLine(String.Format("<option value=""{0}"">{1}</option>", Lookup.Key, Lookup.Value))
        Next

        Return sbHTML.ToString

    End Function

#End Region

#Region "Validate Basket"

    Public Shared Function ValidateBasket() As Integer

        Return BookingBase.SearchBasket.BasketCarHires.Count

    End Function

#End Region

#Region "Helper Classes"

    Public Class CustomSetting
        Public TextOverrides As String
        Public Title As String
        Public SubTitle As String
        Public TemplateOverride As String
        Public ResultsTemplateOverride As String
        Public PopupTemplateOverride As String
        Public SelectedTemplateOverride As String
        Public CSSClassOverride As String
        Public SearchMode As String
        Public AutomaticSearchType As String
        Public LeadDriverAge As Integer
        Public LeadDriverBookingCountryID As Integer
        Public TransferOrCarHireOnly As Boolean
        Public TransferOrCarHireOnlyWarning As String
        Public SearchWarning As String
        Public ControlOverride As String
        Public ListClass As String
        Public ListItemClass As String
        Public UseTheme As Boolean
        Public HiddenSearchTool As Boolean
        Public PriceFormat As String
        Public UseBasketSelectedCarHire As Boolean
        Public Redirect As Boolean
        Public RedirectUrl As String
        Public StartExpanded As Boolean
    End Class

    Public Class CarHireSearchReturn
        Public Success As Boolean
        Public Warning As String
        Public HTML As String = ""
    End Class

    Public Class FilterReturn
        Public Success As Boolean
        Public Warning As String
        Public ResultCount As Integer
        Public HTML As String = ""
    End Class

    Public Class AddCarHireReturn
        Public Success As Boolean
        Public Warnings As New Generic.List(Of String)
        Public TotalPriceUnformated As Decimal = 0
        Public Redirect As Boolean = False
        Public RedirectUrl As String
    End Class

    Public Class RemoveCarHireReturn
        Public Success As Boolean
        Public Warnings As New Generic.List(Of String)
        Public UpdateBasket As String
    End Class

    Public Class InfoPrebookReturn
        Public Success As Boolean
        Public Warning As String
        Public HTML As String = ""
    End Class

    Public Class AdditionalInfoReturn
        Public Success As Boolean
        Public Warning As String
        Public HTML As String = ""
    End Class

#End Region

End Class