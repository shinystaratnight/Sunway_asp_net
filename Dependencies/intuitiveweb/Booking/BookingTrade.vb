Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions

Public Class BookingTrade


#Region "Properties"

	Public Property TradeID As Integer = 0
	Public Property TradeContactID As Integer = 0
	Public Property CreditCardAgent As Boolean = False
	Public Property Commissionable As Boolean = False

	Public Property AgentReference As String = ""

	Public Property UserName As String = ""
	Public Property TradeName As String = ""
	Public Property Address1 As String = ""
	Public Property Address2 As String = ""
	Public Property TownCity As String = ""
	Public Property County As String = ""
	Public Property Postcode As String = ""
	Public Property Telephone As String = ""
	Public Property Email As String = ""
	Public Property BookingCountryID As Integer = 0
	Public Property RequiresContact As Boolean = False
	Public Property CanClaimIncentives As Boolean = False
    Public Property PDFVouchersEnabled As Boolean = False
    Public Property Logo As String = ""

	Public Property LanguageID As Integer = 0

	Public Property TradeContacts As New Generic.List(Of TradeContact)

	Public Shared ReadOnly Property CookieName As String
		Get
			Return "__TradeLoginDetails"
		End Get
	End Property

#End Region


#Region "Trade Login"

	Public Function Login(ByVal oTradeLoginDetails As TradeLoginDetails, Optional ByVal IVCLoginDetails As iVectorConnectInterface.LoginDetails = Nothing) As TradeLoginReturn

		Dim oTradeLoginReturn As New TradeLoginReturn

		'reset trade details incase they're already on the object
		Me.TradeID = 0
		Me.TradeContactID = 0
		Me.CreditCardAgent = False
		Me.TradeContacts.Clear()

		'set agent reference
		Me.AgentReference = oTradeLoginDetails.AgentReference


		Try

			'create login request
			Dim oTradeLoginRequest As New ivci.TradeLoginRequest
			With oTradeLoginRequest
				.LoginDetails = Functions.IIf(Not IVCLoginDetails Is Nothing, IVCLoginDetails, BookingBase.IVCLoginDetails)

				.UserName = oTradeLoginDetails.Username
				.Email = oTradeLoginDetails.EmailAddress
				.Password = oTradeLoginDetails.Password
				.WebsitePassword = oTradeLoginDetails.WebsitePassword


				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()
				If oWarnings.Count > 0 Then
					oTradeLoginReturn.OK = False
					oTradeLoginReturn.Warnings.AddRange(oWarnings)
				End If

			End With


			'If everything is ok then verify the login through connect
			If oTradeLoginReturn.OK Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.TradeLoginResponse)(oTradeLoginRequest, "", True)

				Dim oTradeLoginResponse As New ivci.TradeLoginResponse

				If oIVCReturn.Success Then

					oTradeLoginResponse = CType(oIVCReturn.ReturnObject, ivci.TradeLoginResponse)

					'Either we don't care if the Trade session is unique, or we need to ensure that it is unique
					If Not BookingBase.Params.UniqueTradeSessions OrElse Me.IsSessionUnique(oTradeLoginResponse.TradeID) Then

						'set trade details
						Me.TradeID = oTradeLoginResponse.TradeID
						Me.TradeContactID = oTradeLoginResponse.TradeContactID
						Me.CreditCardAgent = oTradeLoginResponse.CreditCardAgent
						Me.Commissionable = oTradeLoginResponse.Commissionable

						'Set Global Login Flag
						BookingBase.LoggedIn = True

						'get trade details from BigC
						Me.PopulateTradeDetails()

						'set language
						If BookingBase.Trade.LanguageID <> 0 Then
							BookingBase.DisplayLanguageID = BookingBase.Trade.LanguageID
						End If

						oTradeLoginReturn.OK = Me.TradeID > 0

						'if remember me, save cookie
						If oTradeLoginDetails.RememberMe Then
							BookingTrade.SetLoginDetailsToCookie(oTradeLoginDetails)
						Else 'to be on the safe side
							BookingTrade.DeleteLoginDetailsCookie()
						End If

					Else

						oTradeLoginReturn.Warnings.Add("Duplicate Login")
						oTradeLoginReturn.OK = False

					End If

				Else
					Me.AgentReference = ""
					oTradeLoginReturn.OK = False
					oTradeLoginReturn.Warnings.AddRange(oTradeLoginReturn.Warnings)
				End If

			End If

		Catch ex As Exception
			oTradeLoginReturn.OK = False
			oTradeLoginReturn.Warnings.Add(ex.ToString)
		End Try


		'return
		Return oTradeLoginReturn


	End Function

#End Region

	Public Function IsSessionUnique(ByVal TradeID As Integer) As Boolean

		'Get the user token
		Dim sUserToken As String = Me.GetUserToken()

		'Check if its unique
		Return CheckIfUniqueSession(TradeID, sUserToken)

	End Function

	Public Function GetUserToken() As String

		Dim sUserToken As String = ""

		Try

			'Combine IP with User ID
			sUserToken = (HttpContext.Current.Request.ServerVariables("LOCAL_ADDR"))
			sUserToken += "|"
			sUserToken += HttpContext.Current.Request.ServerVariables("AUTH_USER")

		Catch ex As Exception

		End Try

		Return sUserToken

	End Function

	Public Shared Sub AddUserToCollection(ByVal TradeID As Integer, ByVal UserToken As String)

		BookingBase.AgentSessions.Add(TradeID, UserToken)

	End Sub

	Public Shared Function CheckIfUniqueSession(ByVal TradeID As Integer, ByVal NewUserToken As String) As Boolean

		''If TradeID is in cache...
		'If BookingBase.AgentSessions.ContainsKey(TradeID) Then

		'	'Get the user token out of the cache and ensure its the same
		'	Dim sStoredUserToken As String = BookingBase.AgentSessions(TradeID)

		'	'If not return false
		'	Return sStoredUserToken = NewUserToken

		'End If

		If BookingBase.AgentSessions.ContainsKey(TradeID) Then
			Return False
		Else
			'If it's not in the cache it's unique, add it to cache and return true
			AddUserToCollection(TradeID, NewUserToken)

			Return True

		End If

	End Function


#Region "Logout"

	Public Shared Sub Logout()

		'If Trade session must be unique, remove it from the cache.
		If BookingBase.Params.UniqueTradeSessions Then
			BookingBase.AgentSessions.Remove(BookingBase.Trade.TradeID)
		End If

		're-set language id
		BookingBase.DisplayLanguageID = BookingBase.Params.LanguageID

		're-set trade details on session
		BookingBase.Trade = New BookingTrade

		'set autologin on tradecookie to false (so we dont just keep logging them back in again)
		Dim oTradeLoginDetails As TradeLoginDetails = BookingTrade.GetLoginDetailsFromCookie()
		oTradeLoginDetails.AutoLogin = False
		BookingTrade.SetLoginDetailsToCookie(oTradeLoginDetails)

		'Set the Bookingbase login variable to false
		BookingBase.LoggedIn = False

	End Sub

#End Region


#Region "Get Trade Contact"

	Public Shared Function GetTradeContact() As TradeContact

		Dim oTradeContact As TradeContact = BookingBase.Trade.TradeContacts.Where(Function(o) o.TradeContactID = BookingBase.Trade.TradeContactID).FirstOrDefault()

		If Not oTradeContact Is Nothing Then
			Return oTradeContact
		Else
			Return New TradeContact
		End If

	End Function

#End Region


#Region "login details cookie"

	Public Shared Function GetLoginDetailsFromCookie() As TradeLoginDetails
		Try
			Dim sCookie As String = Intuitive.CookieFunctions.Cookies.GetValue(BookingTrade.CookieName)
			Dim sDecryptedCookie As String = Intuitive.Functions.Decrypt(sCookie)
			Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of TradeLoginDetails)(sDecryptedCookie, New Newtonsoft.Json.Converters.StringEnumConverter)
		Catch ex As Exception
			Return New TradeLoginDetails
		End Try
	End Function

	Public Shared Sub SetLoginDetailsToCookie(ByVal TradeLoginDetails As TradeLoginDetails)
		Try
			Dim sCookie As String = Intuitive.Functions.Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(TradeLoginDetails, New Newtonsoft.Json.Converters.StringEnumConverter))
			Intuitive.CookieFunctions.Cookies.SetValue(BookingTrade.CookieName, sCookie, CookieFunctions.CookieExpiry.OneWeek)
		Catch ex As Exception

		End Try
	End Sub

	Public Shared Sub DeleteLoginDetailsCookie()
		Intuitive.CookieFunctions.Cookies.Remove(BookingTrade.CookieName)
	End Sub

#End Region


#Region "Get BigC agent info"

	Public Sub PopulateTradeDetails()

		'Get the trade details from BigC
		Dim oXML As XmlDocument
		oXML = Utility.BigCXML("Trade", BookingBase.Trade.TradeID, 60)

		Me.TradeName = XMLFunctions.SafeNodeValue(oXML, "Trade/TradeName")
		Me.Address1 = XMLFunctions.SafeNodeValue(oXML, "Trade/Address1")
		Me.Address2 = XMLFunctions.SafeNodeValue(oXML, "Trade/Address2")
		Me.TownCity = XMLFunctions.SafeNodeValue(oXML, "Trade/TownCity")
		Me.County = XMLFunctions.SafeNodeValue(oXML, "Trade/County")
		Me.Postcode = XMLFunctions.SafeNodeValue(oXML, "Trade/Postcode")
		Me.Telephone = XMLFunctions.SafeNodeValue(oXML, "Trade/Telephone")
		Me.Email = XMLFunctions.SafeNodeValue(oXML, "Trade/Email")
		Me.BookingCountryID = SafeInt(XMLFunctions.SafeNodeValue(oXML, "Trade/BookingCountryID"))
		Me.RequiresContact = SafeBoolean(XMLFunctions.SafeNodeValue(oXML, "Trade/RequiresContact"))
		Me.CanClaimIncentives = SafeBoolean(XMLFunctions.SafeNodeValue(oXML, "Trade/CanClaimIncentives"))
		Me.LanguageID = SafeInt(XMLFunctions.SafeNodeValue(oXML, "Trade/LanguageID"))
        Me.PDFVouchersEnabled = SafeBoolean(XMLFunctions.SafeNodeValue(oXML, "Trade/PDFVouchersEnabled"))
        Me.Logo = XMLFunctions.SafeNodeValue(oXML, "Trade/Logo")

        Dim iCurrencyId As Integer = XMLFunctions.SafeNodeValue(oXML, "Trade/CurrencyID").ToSafeInt()
        Dim iSellingCurrencyId As Integer = XMLFunctions.SafeNodeValue(oXML, "Trade/SellingCurrencyID").ToSafeInt()

	    If iCurrencyId <> 0 Then
	        BookingBase.CurrencyID = iCurrencyId
	    End If
	    If iSellingCurrencyId <> 0 Then
		    BookingBase.SellingCurrencyID = iSellingCurrencyId
	    End If
        
		'Populate Trade Contacts
		Me.TradeContacts = Intuitive.Web.Utility.XMLToGenericList(Of TradeContact)(oXML, "/Trade/TradeContacts/TradeContact")


	End Sub

#End Region


#Region "Class Definitions"

	Public Class TradeLoginDetails

		Public AgentReference As String = ""	'AbtaAtolNumber on Trade
		Public WebsitePassword As String = "" ' Trade Website Password

		Public Username As String 'Trade contact Username
		Public Password As String 'Trade contact password

		Public EmailAddress As String = "" 'Can be trade email or trade contact email

		Public LoginType As eLoginType = eLoginType.Trade 'Login mode (as ) 

		Public RememberMe As Boolean
		Public AutoLogin As Boolean = False

		Public Enum eLoginType
			Trade 'remember - this will be the default
			TradeContact
		End Enum

	End Class

	Public Enum eLoginMethod
		TradeDetails
		EmailAndPassword
	End Enum

	Public Class TradeLoginReturn
		Public OK As Boolean = True
		Public Warnings As New Generic.List(Of String)
	End Class


	Public Class TradeContact
		Public TradeContactID As Integer
		Public Title As String
		Public Forename As String
		Public Surname As String
		Public Email As String
		Public Address1 As String
		Public Address2 As String
		Public TownCity As String
		Public County As String
		Public Postcode As String
		Public Country As String
		Public Telephone As String
		Public Mobile As String
		Public Fax As String
	End Class

#End Region


End Class
