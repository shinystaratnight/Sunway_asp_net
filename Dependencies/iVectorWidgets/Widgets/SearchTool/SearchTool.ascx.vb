Imports System.IO
Imports System.Xml
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web.Translation
Imports iw = Intuitive.Web
Imports System.Web.UI.HtmlControls
Imports Intuitive.Web


Public Class SearchToolUserControl
    Inherits UserControlBase


    Public Overrides Sub ApplySettings(ByVal Settings As iw.PageDefinition.WidgetSettings)

        'set our redirects
        If Not Me.hidSearchRedirects Is Nothing Then
            Dim sSearchRedirects As String = SafeString(Settings.GetValue("SearchRedirects"))
            Me.hidSearchRedirects.Value = sSearchRedirects
        End If


        'css class override
        Dim sCSSClassOverride As String = Settings.GetValue("CSSClassOverride")
        If sCSSClassOverride <> "" Then
            Me.divSearch.Attributes("class") = sCSSClassOverride
        End If

        'search again class override
        Dim sSearchAgainClassOverride As String = Settings.GetValue("SearchAgainClassOverride")
        If sSearchAgainClassOverride <> "" Then
            Me.divSearchAgain.Attributes("class") = sSearchAgainClassOverride
        End If

        'show search again if we're on basket page and there's nothing in the basket
        If Me.Request.RawUrl = "/basket" AndAlso Intuitive.Web.BookingBase.Basket.TotalComponents = 0 Then
            Me.divSearchAgain.Attributes("class") = Replace(Me.divSearchAgain.Attributes("class"), "hidden", "")
        End If

        'tabs class override
        Dim sTabsClassOverride As String = Settings.GetValue("TabsClassOverride")
        If sTabsClassOverride <> "" Then
            Me.divSearchMode.Attributes("class") = sTabsClassOverride
        End If

        'set search title (only show if set)
        Me.h2_SearchTitle.InnerText = Settings.GetValue("SearchTitle")
        Me.divSearchTitle.Visible = Me.h2_SearchTitle.InnerText <> ""


        'override labels
        Dim sLabelOverrides As String = Settings.GetValue("LabelOverrides")
        If sLabelOverrides <> "" Then
            Me.UpdateOverrides("Labels", sLabelOverrides)
        End If


        'override placeholders
        Dim sPlaceholderOverrides As String = Settings.GetValue("PlaceholderOverrides")
        If sPlaceholderOverrides <> "" Then
            Me.UpdateOverrides("Placeholders", sPlaceholderOverrides)
        End If


        'add suffixes to dropdowns if setting present
        Dim sDropdownSuffixes As String = Settings.GetValue("DropdownSuffixes")
        If sDropdownSuffixes <> "" Then
            Dim aDropdownSuffixes As String() = sDropdownSuffixes.Split("#"c)

            For Each sDropdownSuffix As String In aDropdownSuffixes
                Dim sDropdown As String = sDropdownSuffix.Split("|"c)(0)
                Dim sSuffix As String = sDropdownSuffix.Split("|"c)(1)
                Dim sSingularSuffix As String = sSuffix.Split(","c)(0)
                Dim sPluralSuffix As String = sSuffix.Split(","c)(1)

                Select Case sDropdown
                    Case "Duration"
                        Me.hidDurationSingularSuffix.Value = sSingularSuffix
                        Me.hidDurationPluralSuffix.Value = sPluralSuffix
                    Case "Rooms"
                        Me.hidRoomsSingularSuffix.Value = sSingularSuffix
                        Me.hidRoomsPluralSuffix.Value = sPluralSuffix
                    Case "Adults"
                        Me.hidAdultsSingularSuffix.Value = sSingularSuffix
                        Me.hidAdultsPluralSuffix.Value = sPluralSuffix
                    Case "Children"
                        Me.hidChildrenSingularSuffix.Value = sSingularSuffix
                        Me.hidChildrenPluralSuffix.Value = sPluralSuffix
                    Case "Infants"
                        Me.hidInfantsSingularSuffix.Value = sSingularSuffix
                        Me.hidInfantsPluralSuffix.Value = sPluralSuffix
                    Case "Ages"
                        Me.hidAgeSingularSuffix.Value = sSingularSuffix
                        Me.hidAgePluralSuffix.Value = sPluralSuffix
                End Select
            Next
        End If

        'change duration range
        Dim sDurationRange As String = Settings.GetValue("DurationRange")
        If sDurationRange <> "" Then
            Dim sDurationStart As String = sDurationRange.Split("|"c)(0)
            Dim sDurationEnd As String = sDurationRange.Split("|"c)(1)

            Me.ddlDuration.CSSClass = String.Format("range_{0}_{1}", sDurationStart, sDurationEnd)
        Else
            Me.ddlDuration.CSSClass = "range_1_17"
        End If

        Dim bReturnDatepicker As Boolean = Settings.GetValue("ReturnDatepicker").ToSafeBoolean
        Me.hidReturnDatepicker.Value = bReturnDatepicker.ToString
        If Not bReturnDatepicker Then
            Me.dlReturn.Visible = False
        End If

        'If we have it set pass in the minimum book ahead days to the search tool to ensure we are not allowing searches before it.
        If Not Me.hidDaysAhead Is Nothing Then
            Dim iDaysAhead As Integer = Intuitive.Web.BookingBase.Params.Search_BookAheadDays
            Me.hidDaysAhead.Value = iDaysAhead.ToString
        End If

        If Not hidReturnMinDaysAhead Is Nothing Then
            'Set the minimumn return day on the datepicker
            Dim sReturnMinDaysAhead As String = SafeString(Settings.GetValue("ReturnMinDaysAhead"))
            If sReturnMinDaysAhead <> "" Then
                Me.hidReturnMinDaysAhead.Value = sReturnMinDaysAhead
            Else
                Me.hidReturnMinDaysAhead.Visible = False
            End If
        End If

        Me.hidReturnDatepicker.Value = bReturnDatepicker.ToString

        Dim sCultureCode As String = Intuitive.Web.BookingBase.Lookups.GetKeyPairValue(Intuitive.Web.Lookups.LookupTypes.CultureCode, Intuitive.Web.BookingBase.DisplayLanguageID).ToLower()
        Dim sDatepickerLanguage As String = "/script/LanguageScripts/datepicker-" + sCultureCode + ".js"
        hidDatepickerCultureCode.Value = sDatepickerLanguage


        'change the total searchable pax
        'a. Adults
        Dim sAdultsRange As String = Settings.GetValue("AdultsRange")
        If sAdultsRange <> "" Then
            Dim sAdultsStart As String = sAdultsRange.Split("|"c)(0)
            Dim sAdultsEnd As String = sAdultsRange.Split("|"c)(1)

            Me.ddlAdults_1.CSSClass = String.Format("range_{0}_{1}", sAdultsStart, sAdultsEnd)
            Me.ddlAdults_2.CSSClass = String.Format("range_{0}_{1}", sAdultsStart, sAdultsEnd)
            Me.ddlAdults_3.CSSClass = String.Format("range_{0}_{1}", sAdultsStart, sAdultsEnd)
        Else
            Me.ddlAdults_1.CSSClass = "range_1_9"
            Me.ddlAdults_2.CSSClass = "range_1_9"
            Me.ddlAdults_3.CSSClass = "range_1_9"
        End If

        'b. Children
        Dim sChildrenRange As String = Settings.GetValue("ChildrenRange")
        If sChildrenRange <> "" Then
            Dim sChildrenStart As String = sChildrenRange.Split("|"c)(0)
            Dim sChildrenEnd As String = sChildrenRange.Split("|"c)(1)

            Me.ddlChildren_1.CSSClass = String.Format("range_{0}_{1}", sChildrenStart, sChildrenEnd)
            Me.ddlChildren_2.CSSClass = String.Format("range_{0}_{1}", sChildrenStart, sChildrenEnd)
            Me.ddlChildren_3.CSSClass = String.Format("range_{0}_{1}", sChildrenStart, sChildrenEnd)
        Else
            Me.ddlChildren_1.CSSClass = "range_0_4"
            Me.ddlChildren_2.CSSClass = "range_0_4"
            Me.ddlChildren_3.CSSClass = "range_0_4"
        End If

        'c. Infants
        Dim sInfantsRange As String = Settings.GetValue("InfantsRange")
        If sInfantsRange <> "" Then
            Dim sInfantsStart As String = sInfantsRange.Split("|"c)(0)
            Dim sInfantsEnd As String = sInfantsRange.Split("|"c)(1)

            Me.ddlInfants_1.CSSClass = String.Format("range_{0}_{1}", sInfantsStart, sInfantsEnd)
            Me.ddlInfants_2.CSSClass = String.Format("range_{0}_{1}", sInfantsStart, sInfantsEnd)
            Me.ddlInfants_3.CSSClass = String.Format("range_{0}_{1}", sInfantsStart, sInfantsEnd)
        Else
            Me.ddlInfants_1.CSSClass = "range_0_9"
            Me.ddlInfants_2.CSSClass = "range_0_9"
            Me.ddlInfants_3.CSSClass = "range_0_9"
        End If

        'set hidden input for datepicker
        Me.hidDatepickerMonths.Value = IIf(SafeInt(Settings.GetValue("DatepickerMonths")) = 0, "1", SafeString(Settings.GetValue("DatepickerMonths")))
        Me.hidDatepickerFirstDay.Value = IIf(SafeInt(Settings.GetValue("DatepickerFirstDay")) = 0, "1", SafeString(Settings.GetValue("DatepickerFirstDay")))
        Me.hidShowDatePickerDropDowns.Value = IIf(Settings.GetValue("ShowDatePickerDropDowns") = "", "False", Functions.SafeString(Settings.GetValue("ShowDatePickerDropDowns")))

        'departure mode
        If Functions.SafeString(Settings.GetValue("DepartureMode")) <> "" Then
            Dim oDepartureMode As SearchTool.LocationChooseMode = Functions.SafeEnum(Of SearchTool.LocationChooseMode)(Settings.GetValue("DepartureMode"))

            Me.divDepartingFromDropdown.Visible = oDepartureMode <> SearchTool.LocationChooseMode.AutoComplete
            Me.divDepartingFromAuto.Visible = oDepartureMode <> SearchTool.LocationChooseMode.Dropdown AndAlso oDepartureMode <> SearchTool.LocationChooseMode.DropdownGrouped _
              AndAlso oDepartureMode <> SearchTool.LocationChooseMode.DropdownGroupedWithCountry

            Me.divShowHideDepartingFromAutoComplete.Visible = oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdown _
              OrElse oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownGrouped _
              OrElse oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry


            If oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdown _
             OrElse oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownGrouped Then
                Me.divDepartingFromDropdown.Attributes.Add("style", "display:none;")
            End If

            If oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry Then
                Me.divDepartingFromAuto.Attributes.Add("style", "display:none;")
            End If


            'if using grouped dropdown override lookup and set to include parent
            If oDepartureMode = SearchTool.LocationChooseMode.DropdownGrouped OrElse oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownGrouped _
             OrElse oDepartureMode = SearchTool.LocationChooseMode.DropdownGroupedWithCountry Then
                Me.ddlDepartingFromID.Lookup = Intuitive.Web.Lookups.LookupTypes.CountryAirport
                Me.ddlDepartingFromID.IncludeParent = True
            End If

            Me.divDepartingFromCountryCountry.Visible = oDepartureMode = SearchTool.LocationChooseMode.DropdownGroupedWithCountry _
             OrElse oDepartureMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry
            Me.ddlDepartingFromCountryID.OverrideBlankText = GetCustomTranslation("Search Tool", "Country")

            'If we have set a flight only arriving at mode, and we want to have an autocomplete and a dropdown, set the visibility of form elements accordingly.
            Dim oFlightOnlyDestinationMode As SearchTool.LocationChooseMode = Functions.SafeEnum(Of SearchTool.LocationChooseMode)(Settings.GetValue("FlightOnlyDestinationMode"))
            Me.divShowHideAirportArrivingAtAutoComplete.Visible = oFlightOnlyDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry
            Me.divArrivingAtAirportAuto.Visible = oFlightOnlyDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry
            Me.divArrivingAtAirport.Visible = oFlightOnlyDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry

            If oFlightOnlyDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry Then
                Me.aShowDepartingFromDropdown.Attributes.Add("Style", "display:none;")
                Me.aShowDepartingFromAutoComplete.Attributes.Remove("Style")
            End If

        Else
            Me.divDepartingFromAuto.Visible = False
            Me.divShowHideDepartingFromAutoComplete.Visible = False
            Me.divDepartingFromCountryCountry.Visible = False

        End If

        'destination mode
        Dim oDestinationMode As SearchTool.LocationChooseMode = Functions.SafeEnum(Of SearchTool.LocationChooseMode)(Settings.GetValue("DestinationMode"))


        Me.divArrivingAtDropdown.Visible = oDestinationMode <> SearchTool.LocationChooseMode.AutoComplete
        Me.divArrivingAtAuto.Visible = oDestinationMode <> SearchTool.LocationChooseMode.Dropdown AndAlso oDestinationMode <> SearchTool.LocationChooseMode.DropdownGrouped _
         AndAlso oDestinationMode <> SearchTool.LocationChooseMode.DropdownGroupedWithCountry
        Me.divShowHideArrivingAtAutoComplete.Visible = oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdown _
         OrElse oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownGrouped OrElse oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry

        If oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdown _
         OrElse oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownGrouped _
         OrElse oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry Then
            Me.divArrivingAtDropdown.Attributes.Add("Style", "display:none;")
        End If

        If oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownGrouped OrElse oDestinationMode = SearchTool.LocationChooseMode.DropdownGrouped _
         AndAlso oDestinationMode <> SearchTool.LocationChooseMode.DropdownGroupedWithCountry Then
            Me.ddlArrivalAirportID.Lookup = Intuitive.Web.Lookups.LookupTypes.CountryArrivalAirport
            Me.ddlArrivalAirportID.IncludeParent = True
        End If

        Me.ddlCountryID.Visible = oDestinationMode = SearchTool.LocationChooseMode.AutoCompleteAndDropdownWithCountry
        Me.ddlCountryID.OverrideBlankText = GetCustomTranslation("Search Tool", "Country")

        'property option
        Dim bPriorityProperty As Boolean = Functions.SafeBoolean(Settings.GetValue("PriorityProperty"))
        Me.fldPriorityProperty.Visible = bPriorityProperty


        'show advanced options link
        Me.aShowAdvancedOptions.Visible = SafeBoolean(Settings.GetValue("AdvancedOptions"))
        Me.aHideAdvancedOptions.Visible = SafeBoolean(Settings.GetValue("AdvancedOptions"))

        'show clear button
        Me.btnClear.Visible = SafeBoolean(Settings.GetValue("ShowClearButton"))

        'show hide button
        Me.btnHide.Visible = SafeBoolean(Settings.GetValue("ShowHideButton"))

        'create search modes
        Dim sSearchModes As String = Settings.GetValue("SearchModes")
        If sSearchModes = "CMSControlled" Then
            Dim oSearchModesXML As XmlDocument = Intuitive.Web.Utility.BigCXML("SearchModes", 1, 60)
            Dim oSearchModes As SearchModes = Serializer.DeSerialize(Of SearchModes)(oSearchModesXML.InnerXml)

            Dim sSearchModesString As String = ""
            sSearchModesString += IIf(oSearchModes.HotelOnly, "Hotel#Hotel,", "")
            sSearchModesString += IIf(oSearchModes.FlightOnly, "Flight#Flight,", "")
            sSearchModesString += IIf(oSearchModes.FlightPlusHotel, "FlightPlusHotel#Flight & Hotel,", "")
            sSearchModesString += IIf(oSearchModes.Transfer, "Transfer#Transfer,", "")
            sSearchModesString = sSearchModesString.Chop(1)

            Me.CreateSearchModes(sSearchModesString)
        Else
            Me.CreateSearchModes(Settings.GetValue("SearchModes"))
        End If


        'set redirect urls
        Dim sFlightPlusHotelURL As String = Settings.GetValue("FlightPlusHotelURL")
        Me.hidFlightPlusHotelURL.Value = IIf(sFlightPlusHotelURL <> "", sFlightPlusHotelURL, "/search-results")

        Dim bUseFlightCarouselResults As Boolean = True
        If Settings.GetValue("UseFlightCarouselResults") <> "" Then bUseFlightCarouselResults = SafeBoolean(Settings.GetValue("UseFlightCarouselResults"))
        If Not hidUseFlightCarouselResults Is Nothing Then
            Me.hidUseFlightCarouselResults.Value = bUseFlightCarouselResults.ToString.ToLower
        End If


        'start mode
        If SafeString(Settings.GetValue("StartMode")) <> "" Then
            Me.hidStartMode.Value = SafeString(Settings.GetValue("StartMode"))

            'hide search again if start mode is hidden
            If SafeString(Settings.GetValue("StartMode")) = "Hidden" Then
                Me.btnSearchAgain.Attributes("style") = "display:none;"
            End If
        End If


        'expand action
        If Settings.GetValue("ExpandAction") <> "" Then
            Me.hidExpandAction.Value = Settings.GetValue("ExpandAction")
            If Me.hidStartMode.Value <> "Hidden" Then
                Me.hidStartMode.Value = "Collapsed"
            End If


            'hide search and show search again if action is popup
            If Settings.GetValue("ExpandAction") = "Popup" Then
                Me.divSearch.Attributes("style") = "display:none;"
                Me.divSearchAgain.Attributes("style") = "display:block;"
            End If
        End If

        If Settings.GetValue("UseShortDates") <> "" Then
            Me.hidUseShortDates.Value = Settings.GetValue("UseShortDates")
        End If


        'preSearchscript
        Dim sPreSearchScript As String = Settings.GetValue("PreSearchScript")
        If Not sPreSearchScript = "" Then
            Dim sScript As String = "<script type=""text/javascript"">"
            sScript += "SearchTool.PreSearchFunction = function() {"
            sScript += sPreSearchScript
            sScript += "}"
            sScript += "</script>"
            Me.plcPreSearchScript.Text = sScript
        End If

        'warning message insert after div
        Dim sWarningMessageAfter As String = SafeString(Settings.GetValue("InsertWarningAfter"))
        Me.hidInsertWarningAfter.Value = sWarningMessageAfter

        'add in the defaults search values json
        If Settings.GetValue("DefaultSearchValuesJSON") <> "" Then
            Me.hidDefaultSearchValues.Value = Settings.GetValue("DefaultSearchValuesJSON")
            Me.hidDefaultSearchValues.EncodeValue = False
        End If

        'add in the property avaerage score filter and set the label
        If iw.BookingBase.Params.HotelResults_PropertyMinimumScore >= 0 Then
            Me.lblHighRatedHotelFilter.Visible = True
            
            Dim oXML as XMLDocument = Utility.BigCXML("SearchToolSettings", 1, 60, BookingBase.DisplayLanguageID)

            If oXML IsNot Nothing Then
                Dim sTranslation As String = XMLFunctions.SafeNodeValue(oXML, "SearchToolSettings/MinimumScoreFilterName")
                If Not String.IsNullOrEmpty(sTranslation) Then
                    Me.spnHighRatedHotelFilter.InnerHtml = sTranslation
                End If
            End If
        Else
            Me.lblHighRatedHotelFilter.Visible = False
        End If

    End Sub


    Public Sub CreateSearchModes(ByVal SearchModes As String)

        Dim aSearchModes As String() = SearchModes.Split(","c)
        Dim sb As New StringBuilder

        For Each sSearchMode As String In aSearchModes

            Dim sMode As String = sSearchMode.ToLower.Split("#"c)(0)

            Dim sNameOverride As String = ""
            If sSearchMode.Contains("#") Then sNameOverride = sSearchMode.Split("#"c)(1)

            Select Case sMode
                Case "hotel"
                    sb.AppendLine("<li id=""li_SearchMode_HotelOnly"">")
                    sb.Append("<a id=""a_SearchMode_HotelOnly"" href=""javascript:void;"" ml=""Search Tool"" onclick=""SearchTool.Support.SetSearchMode('HotelOnly');return false;"">")
                    sb.Append(Functions.IIf(sNameOverride <> "", sNameOverride, "Hotel Only"))
                    sb.Append("</a></li>")
                Case "flight"
                    sb.AppendLine("<li id=""li_SearchMode_FlightOnly"">")
                    sb.Append("<a id=""a_SearchMode_FlightOnly"" href=""javascript:void;"" ml=""Search Tool"" onclick=""SearchTool.Support.SetSearchMode('FlightOnly');return false;"">")
                    sb.Append(Functions.IIf(sNameOverride <> "", sNameOverride, "Flight Only"))
                    sb.Append("</a></li>")
                Case "flightplushotel"
                    sb.AppendLine("<li id=""li_SearchMode_FlightPlusHotel"">")
                    sb.Append("<a id=""a_SearchMode_FlightPlusHotel"" href=""javascript:void;"" ml=""Search Tool"" onclick=""SearchTool.Support.SetSearchMode('FlightPlusHotel');return false;"">")
                    sb.Append(Functions.IIf(sNameOverride <> "", sNameOverride, "Holiday"))
                    sb.Append("</a></li>")
                Case "transfer"
                    sb.AppendLine("<li id=""li_SearchMode_TransferOnly"">")
                    sb.Append("<a id=""a_SearchMode_TransferOnly"" href=""javascript:void;"" ml=""Search Tool"" onclick=""SearchTool.Support.SetSearchMode('TransferOnly');return false;"">")
                    sb.Append(Functions.IIf(sNameOverride <> "", sNameOverride, "Transfer Only"))
                    sb.Append("</a></li>")
            End Select
        Next

        Me.ulSearchMode.Attributes("class") = "clear modes_" & aSearchModes.Length.ToString
        Me.ulSearchMode.InnerHtml = sb.ToString

    End Sub


    Public Sub UpdateOverrides(ByVal sType As String, ByVal sOverrides As String)

        'split overrides into array
        Dim aOverrides As String() = sOverrides.Split("#"c)

        'loop through each override and set value if control exists
        For Each sOverride As String In aOverrides

            Dim sID As String = sOverride.Split("|"c)(0)
            Dim sValue As String = sOverride.Split("|"c)(1)

            Dim oElement As Control = Me.FindControl(sID)

            Try

                If Not oElement Is Nothing Then

                    If sType = "Placeholders" Then
                        If TypeOf oElement Is HtmlInputText Then
                            CType(oElement, HtmlInputText).Attributes("placeholder") = sValue
                        ElseIf TypeOf oElement Is iw.Controls.Dropdown Then
                            CType(oElement, iw.Controls.Dropdown).OverrideBlankText = sValue
                        End If
                    Else
                        If TypeOf oElement Is HtmlAnchor Then
                            CType(oElement, HtmlAnchor).InnerText = sValue
                        ElseIf TypeOf oElement Is HtmlTableCell Then
                            CType(oElement, HtmlTableCell).InnerText = sValue
                        Else
                            CType(oElement, HtmlGenericControl).InnerHtml = sValue
                        End If
                    End If

                End If

            Catch ex As Exception

            End Try

        Next

    End Sub

    Public Class SearchModes
        Public HotelOnly As Boolean
        Public FlightOnly As Boolean
        Public FlightPlusHotel As Boolean
        Public Transfer As Boolean
    End Class

End Class