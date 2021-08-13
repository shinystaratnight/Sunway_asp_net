Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports ivci = iVectorConnectInterface

Public Class GuestDetails
	Inherits WidgetBase


	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("guestdetails_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("guestdetails_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("guestdetails_customsettings") = value
		End Set

	End Property


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'1. set up custom settings
		Dim oCustomSettings As New CustomSetting
		With oCustomSettings
			.BasketType = SafeEnum(Of eBasketType)(Settings.GetValue("BasketType"))
			.DOBSearchModes = SafeString(Settings.GetValue("DOBSearchModes"))
			.PopulateLeadGuest = SafeBoolean(Settings.GetValue("PopulateLeadGuest"))
			.RequiresInfantDOB = SafeBoolean(Settings.GetValue("RequiresInfantDOB"))
			.AddDefaultDOB = SafeBoolean(Settings.GetValue("AddDefaultDOB"))
			.TemplateOverride = SafeString(Settings.GetValue("TemplateOverride"))
			.Title = SafeString(Settings.GetValue("Title"))
			.ValidateAges = SafeBoolean(Settings.GetValue("ValidateAges"))
			.AdultAgeOverride = SafeInt(Settings.GetValue("AdultAgeOverride"))
			.MissingFieldsWarning = IIf(SafeString(Settings.GetValue("MissingFieldsWarning")) = "", "Please fill in all fields to continue", SafeString(Settings.GetValue("MissingFieldsWarning")))
			.DOBWarning = SafeString(Settings.GetValue("DOBWarning"))
			.Text = Settings.GetValue("Text")
		End With
		GuestDetails.CustomSettings = oCustomSettings

		Dim sHTML As String = RedrawGuestDetails(True)

		writer.Write(sHTML)
	End Sub



#Region "Add To Basket"

	Public Shared Function AddToBasket(ByVal sCurrentSearchGuestDetails As String) As String

		Dim oBasket As BookingBasket = Functions.IIf(GuestDetails.CustomSettings.BasketType = eBasketType.Main, BookingBase.Basket, BookingBase.SearchBasket)

		Dim bSuccess As Boolean = True
		Dim aWarnings As New Generic.List(Of String)
		Dim oCurrentSearchGuestDetails As New CurrentSearchGuestDetails

		Try
			Dim oStringEnumConverter As New Newtonsoft.Json.Converters.StringEnumConverter
			oCurrentSearchGuestDetails = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CurrentSearchGuestDetails)(sCurrentSearchGuestDetails, oStringEnumConverter)

			If Not oCurrentSearchGuestDetails.Rooms.Sum(Function(o) o.Guests.Where(Function(p) p.GuestType = CurrentSearchGuestDetails.eGuestType.Adult).Count) = BookingBase.SearchDetails.TotalAdults Then Throw New Exception("Number of adults must equal adults searched")
			If Not oCurrentSearchGuestDetails.Rooms.Sum(Function(o) o.Guests.Where(Function(p) p.GuestType = CurrentSearchGuestDetails.eGuestType.Child).Count) = BookingBase.SearchDetails.TotalChildren Then Throw New Exception("Number of children must equal children searched")
			If Not oCurrentSearchGuestDetails.Rooms.Sum(Function(o) o.Guests.Where(Function(p) p.GuestType = CurrentSearchGuestDetails.eGuestType.Infant).Count) = BookingBase.SearchDetails.TotalInfants Then Throw New Exception("Number of infants must equal infants searched")

		Catch ex As Exception
			bSuccess = False
			aWarnings.Add(ex.Message)
		End Try


		If bSuccess Then
			'Add passenger details
			Dim iPassengerNumber As Integer = 0
			Dim iRoom As Integer = 0
			Dim iChild As Integer = 0

			'clear ones already there (incase we go back/there are errors and we need to fill in again)
			If oBasket.BasketFlights.Count > 0 Then oBasket.BasketFlights.Last().GuestIDs.Clear()
			If oBasket.BasketTransfers.Count > 0 Then oBasket.BasketTransfers.Last().GuestIDs.Clear()
			If oBasket.BasketExtras.Count > 0 Then oBasket.BasketExtras.Last().GuestIDs.Clear()

			'Loop through each room (we need to assign the pax to the correct room for each property - transfers + flights get all pax)
			For i As Integer = 0 To oCurrentSearchGuestDetails.Rooms.Count - 1

				'clear room guests
				If oBasket.BasketProperties.Count > 0 Then oBasket.BasketProperties.Last().RoomOptions(i).GuestIDs.Clear()

				'set up guest details object from request
				For Each oGuest As CurrentSearchGuestDetails.Guest In oCurrentSearchGuestDetails.Rooms(i).Guests


					Dim oGuestDetail As ivci.Support.GuestDetail = oBasket.GuestDetails(iPassengerNumber)
					Dim iGuestID As Integer = oGuestDetail.GuestID

					With oGuestDetail
						.Title = oGuest.Title
						.FirstName = oGuest.FirstName
						.MiddleName = oGuest.MiddleName
						.LastName = oGuest.LastName
						.Type = oGuest.GuestType.ToString

						If oCurrentSearchGuestDetails.RequiresDOB OrElse (oCurrentSearchGuestDetails.RequiresInfantDOB AndAlso oGuest.GuestType = CurrentSearchGuestDetails.eGuestType.Infant) Then
							.DateOfBirth = oGuest.DateOfBirth
						ElseIf GuestDetails.CustomSettings.AddDefaultDOB Then
							Select Case oGuest.GuestType
								Case CurrentSearchGuestDetails.eGuestType.Adult
									'For adults, the departure date is their 21st birthday
									.DateOfBirth = oBasket.FirstDepartureDate.AddYears(-21)
								Case CurrentSearchGuestDetails.eGuestType.Child
									'For children, their birthday is jan 1st of January of the current year minus the child age that has 
									'been searched
									.DateOfBirth = New Date((Date.Now.Year - BookingBase.SearchDetails.AllChildAges(iChild)), 1, 1)
								Case CurrentSearchGuestDetails.eGuestType.Infant
									.DateOfBirth = oBasket.FirstDepartureDate.AddYears(-1)
							End Select
						End If

						If oGuest.GuestType = CurrentSearchGuestDetails.eGuestType.Child Then
							'add child ages from search
							.Age = BookingBase.SearchDetails.AllChildAges(iChild)
							iChild += 1
							'check to see if DOB matches age
							If GuestDetails.CustomSettings.ValidateAges AndAlso oCurrentSearchGuestDetails.RequiresDOB AndAlso Not DateFunctions.GetAgeAtTargetDate(.DateOfBirth, oBasket.FirstDepartureDate) = .Age Then
								bSuccess = False
								aWarnings.Add("The date of birth doesnt match the age entered on the search. Please go back and search again using correct age at date of booking")
							End If
						End If

					End With

					'Add Guest ID's to components
					'check guest ids do not already exist to avoid duplication if customers use the back button etc.
					If oBasket.BasketProperties.Count > 0 AndAlso Not oBasket.BasketProperties.Last().RoomOptions(i).GuestIDs.Contains(iGuestID) Then
						oBasket.BasketProperties.Last().RoomOptions(i).GuestIDs.Add(iGuestID)
					End If

					If oBasket.BasketFlights.Count > 0 AndAlso Not oBasket.BasketFlights.Last().GuestIDs.Contains(iGuestID) Then
						oBasket.BasketFlights.Last().GuestIDs.Add(iGuestID)
					End If

					If oBasket.BasketTransfers.Count > 0 AndAlso Not oBasket.BasketTransfers.Last().GuestIDs.Contains(iGuestID) Then
						oBasket.BasketTransfers.Last().GuestIDs.Add(iGuestID)
					End If

					For Each oExtra As BookingExtra.BasketExtra In oBasket.BasketExtras

						Dim iPaxOnExtra As Integer = oExtra.BasketExtraOptions.Sum(Function(o) o.Adults + o.Children + o.Infants)

						If oExtra.GuestIDs.Count < iPaxOnExtra Then
							oExtra.GuestIDs.Add(iGuestID)
						End If

					Next
					Dim oBasketCarHire As BookingCarHire.BasketCarHire = oBasket.BasketCarHires.LastOrDefault()
					If oBasketCarHire IsNot Nothing AndAlso Not oBasketCarHire.GuestIDs.Contains(iGuestID) Then
						oBasketCarHire.GuestIDs.Add(iGuestID)
						If oGuest.LeadDriver Then
							oBasketCarHire.DriverIDs.Add(iGuestID)
						End If
					End If

					iPassengerNumber += 1
				Next


			Next

			'if populating lead guest add lead guest details from first guest in first room
			If GuestDetails.CustomSettings.PopulateLeadGuest Then
				oBasket.LeadCustomer.CustomerTitle = oCurrentSearchGuestDetails.Rooms(0).Guests(0).Title
				oBasket.LeadCustomer.CustomerFirstName = oCurrentSearchGuestDetails.Rooms(0).Guests(0).FirstName
				oBasket.LeadCustomer.CustomerLastName = oCurrentSearchGuestDetails.Rooms(0).Guests(0).LastName
			End If

		End If

		Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.Success = bSuccess, .Warnings = aWarnings})

	End Function

#End Region

	Public Shared Function RedrawGuestDetails(ByVal FullRender As Boolean) As String

		Dim oXML As XmlDocument = Serializer.Serialize(BookingBase.SearchDetails)

		'Some flights do not require us to collect a date of birth, however even those require us to show one for infants
		Dim bFlightRequiresDOB As Boolean = False
		'We'll only have this information if we've prebooked
		If GuestDetails.CustomSettings.BasketType = eBasketType.Main Then
			bFlightRequiresDOB = BookingBase.Basket.PreBookResponse.FlightBookings.Count > 0 AndAlso BookingBase.Basket.PreBookResponse.FlightBookings(0).ShowDateOfBirth
		End If

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams

		'work out possible birth years for adults, children and Infants
		Dim sbA As New StringBuilder("#")
		Dim sbC As New StringBuilder("#")
		Dim sbI As New StringBuilder("#")
		Dim i As Integer

		If CustomSettings.AdultAgeOverride <> 0 Then
			For i = CustomSettings.AdultAgeOverride To 99 Step 1
				sbA.Append((Now.Year - i).ToSafeString).Append("#")
			Next
		Else
			For i = 17 To 99 Step 1
				sbA.Append((Now.Year - i).ToSafeString).Append("#")
			Next
		End If

		For i = 2 To 17 Step 1
			sbC.Append((Now.Year - i).ToSafeString).Append("#")
		Next
		For i = 0 To 2 Step 1
			sbI.Append((Now.Year - i).ToSafeString).Append("#")
		Next

		'work out if we require DOB based on the current search mode
		Dim aDOBSearchModes As String() = GuestDetails.CustomSettings.DOBSearchModes.ToLower.Split(","c)
		Dim bRequiresDOB As Boolean = aDOBSearchModes.Contains(BookingBase.SearchDetails.SearchMode.ToString.ToLower) OrElse bFlightRequiresDOB

		'set params
		With oXSLParams
			.AddParam("AdultYears", sbA.ToString)
			.AddParam("ChildYears", sbC.ToString)
			.AddParam("InfantYears", sbI.ToString)
			.AddParam("RequiresDOB", bRequiresDOB.ToString.ToLower)
			.AddParam("RequiresInfantDOB", (GuestDetails.CustomSettings.RequiresInfantDOB _
					  AndAlso (BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.FlightPlusHotel _
						OrElse BookingBase.SearchDetails.SearchMode = BookingSearch.SearchModes.Anywhere)).ToString.ToLower)
			.AddParam("Title", GuestDetails.CustomSettings.Title)
			.AddParam("FirstDepartureDate", SafeString(BookingBase.SearchDetails.DepartureDate))
			.AddParam("DOBWarning", Intuitive.Web.Translation.GetCustomTranslation("Guest Details", GuestDetails.CustomSettings.DOBWarning))
			.AddParam("MissingFieldsWarning", Intuitive.Web.Translation.GetCustomTranslation("Guest Details", GuestDetails.CustomSettings.MissingFieldsWarning))
			.AddParam("Text", GuestDetails.CustomSettings.Text)
			.AddParam("ShowLeadDriver", BookingBase.SearchBasket.BasketCarHires.Any())
			.AddParam("Full", FullRender)
		End With

		Dim sHTML As String
		If GuestDetails.CustomSettings.TemplateOverride <> "" Then
			sHTML = Intuitive.XMLFunctions.XMLTransformToString(oXML, HttpContext.Current.Server.MapPath("~" & GuestDetails.CustomSettings.TemplateOverride), oXSLParams)
		Else
            sHTML = Intuitive.XMLFunctions.XMLStringTransformToString(oXML, XSL.SetupTemplate(res.GuestDetails, True, False), oXSLParams)
        End If

		Return sHTML

	End Function

#Region "Support classes"

	Public Class CurrentSearchGuestDetails

		Public RequiresDOB As Boolean
		Public RequiresInfantDOB As Boolean
		Public RequiresLeadDriver As Boolean
		Public Rooms As New Generic.List(Of Room)

		Public Class Room
			Public RoomNumber As Integer
			Public Guests As New Generic.List(Of Guest)
		End Class

		Public Class Guest
			Public GuestType As eGuestType
			Public Title As String
			Public FirstName As String
			Public MiddleName As String
			Public LastName As String
			Public DateOfBirth As DateTime
			Public LeadDriver As Boolean
		End Class

		Public Enum eGuestType
			Adult
			Child
			Infant
		End Enum

	End Class


	Public Class CustomSetting
		Public BasketType As eBasketType
		Public DOBSearchModes As String
		Public PopulateLeadGuest As Boolean
		Public RequiresInfantDOB As Boolean
		Public AddDefaultDOB As Boolean
		Public TemplateOverride As String
		Public Title As String
		Public DOBWarning As String
		Public MissingFieldsWarning As String
		Public ValidateAges As Boolean = False
		Public AdultAgeOverride As Integer 'This setting allows us to override what the minimum Adult age is set as for generating DOB's as it's 12 on Bookabed
		Public Text As String
	End Class

	Public Enum eBasketType
		Search
		Main
	End Enum

#End Region




End Class
