Imports System.Collections.Generic
Imports System.Linq
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive.Functions

Namespace Widgets
    Public Class APIS
        Inherits WidgetBase

#Region "Prpoerties"

        Public Shared Property APISEmailAddress As String
            Get
                If Not HttpContext.Current.Session("usersession_apisemailaddress") Is Nothing Then
                    Return CType(HttpContext.Current.Session("usersession_apisemailaddress"), String)
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                HttpContext.Current.Session("usersession_apisemailaddress") = value
            End Set
        End Property

#End Region

#Region "Page Lifecycle"

        Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

            'get booking reference
            Dim sBookingReference As String = iVectorWidgets.MyBookingsLogin.MyBookingsReference

            'get email
            APIS.APISEmailAddress = Intuitive.Functions.SafeString(Settings.GetValue("EmailAddress"))

            'get Flight details
            Dim oBookingDetails As BookingManagement.BookingDetailsReturn = BookingManagement.GetBookingDetails(sBookingReference)
            Dim oFlightDetails As Generic.List(Of iVectorConnectInterface.GetBookingDetailsResponse.Flight) = oBookingDetails.BookingDetails.Flights

            'put that into XML
            Dim oXML As New XmlDocument
            If oFlightDetails.Count > 0 Then
                oXML = Serializer.Serialize(oFlightDetails, True)
            End If

            Dim oPassportIssueGeo As XmlDocument = Utility.BigCXML("ManageMyBooking", 1, 60, CMSWebsiteID:=BookingBase.Params.CMSWebsiteID)

            'merge in nationalities	xml
            oXML = XMLFunctions.MergeXMLDocuments(oXML, Utility.BigCXML("Nationality", 1, 60), GetCountriesXML(oPassportIssueGeo))

            'xsl params
            Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
			With oXSLParams
				.AddParam("SellingGeographyLevel1ID", BookingBase.Params.SellingGeographyLevel1ID)
				.AddParam("Theme", BookingBase.Params.Theme)
				.AddParam("BookingReference", sBookingReference)
				.AddParam("RenderOnPage", Intuitive.Functions.SafeBoolean(Settings.GetValue("RenderOnPage")))
			End With

			'transform
			XSLTransform(oXML, writer, oXSLParams)
        End Sub

#End Region

#Region "Support Methods"

        Public Function GetCountriesXML(mmbXml As XmlDocument) As XmlDocument

            'Get the xml from cache
            Dim oXML As XmlDocument = CType(Intuitive.Functions.GetCache("APIS_Locations"), XmlDocument)

            'if its nothing
            If oXML Is Nothing Then

                'New Empty CountrieS object
                Dim oLocations As New IssueLocations

                Dim sGeographyLevel As String = "GeographyLevel1"

                Dim sPassportGeographyLevel As String = XMLFunctions.SafeNodeValue(mmbXml, "ManageMyBooking/PassportIssueGeographyLevel")

                If sPassportGeographyLevel <> "" Then
                    sGeographyLevel = sPassportGeographyLevel
                End If

                'for each Location node in lookupsxml
                For Each oNode As XmlNode In Lookups.LookupXML("Locations").SelectNodes("/Lookups/Locations/Location")

                    'make new country object and set the values
                    Dim oLocation As New IssueLocation
                    oLocation.LocationID = SafeInt(XMLFunctions.SafeNodeValue(oNode, sGeographyLevel & "ID"))
                    oLocation.LocationName = XMLFunctions.SafeNodeValue(oNode, sGeographyLevel & "Name")

                    'if this country is not already in oCountieS, add it
                    If oLocations.Where(Function(o) o.LocationID = oLocation.LocationID).FirstOrDefault Is Nothing Then
                        oLocations.Add(oLocation)
                    End If

                Next

                oLocations.Sort(Function(x, y) String.Compare(x.LocationName, y.LocationName, StringComparison.Ordinal))

                'serialize into xml
                oXML = Intuitive.Serializer.Serialize(oLocations, True)
                'add it to cache
                Intuitive.Functions.AddToCache("APIS_Locations", oXML, 600)
            End If

            Return oXML
        End Function

		Public Function SubmitApisInformation(ByVal sPassengersJSON As String, ByVal sBookingReferencesJSON As String) As Boolean
			Dim oAPISPassengers As ApisPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ApisPassengers)(sPassengersJSON)
			If Intuitive.Functions.SafeBoolean(Settings.GetValue("SendDetailsAsEmail")) Then
				Try
					Dim oBookingReferences As BookingReferences = Newtonsoft.Json.JsonConvert.DeserializeObject(Of BookingReferences)(sBookingReferencesJSON)

					Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
					With oXSLParams
						.AddParam("BookingReference", oBookingReferences.BookingReference)
						.AddParam("FlightBookingReference", oBookingReferences.FlightBookingReference)
						.AddParam("SupplierReference", oBookingReferences.SupplierReference)
					End With

					Dim oEmail As New XSLEmail
					With oEmail
						.SMTPHost = Booking.Params.SMTPHost
						.Subject = "APIS INFORMATION"
						.From = "Intuitive"
						.FromEmail = "admin@intuitivesystems.co.uk"
						.EmailTo = APIS.APISEmailAddress
						.XSLTemplate = HttpContext.Current.Server.MapPath("~" & "/Widgets/APIS/APISEmail.xsl")
						oEmail.XMLDocument = Serializer.Serialize(oAPISPassengers, True)
						.XSLParameters = oXSLParams
					End With

					oEmail.SendEmail(True)

				Catch ex As Exception
					Return False
				End Try

				Return True
			Else

				Dim req As New iVectorConnectInterface.UpdateFlightPassengerRequest
				req.LoginDetails = BookingBase.IVCLoginDetails
				For Each pas In oAPISPassengers
					Dim newpas = New iVectorConnectInterface.UpdateFlightPassengerRequest.FlightPassenger()
					With newpas
						.DateOfBirth = Convert.ToDateTime(pas.DateOfBirth)
						.FlightBookingPassengerID = pas.FlightBookingPassengerID
						.Gender = pas.Gender
						.MiddleName = pas.MiddleName
						.NationalityID = Lookups.GetNationalityIdByName(pas.Nationality)
						.PassportExpiryDate = Convert.ToDateTime(pas.PassportExpiryDate)
						.PassportIssueDate = Convert.ToDateTime(pas.PassportIssueDate)
						.PassportIssuingGeographyLevel1ID = pas.PassportIssuePlaceID
						.PassportNumber = pas.PassportNumber
					End With

					req.FlightPassengers.Add(newpas)
				Next

				Return Utility.iVectorConnect.SendRequest(Of iVectorConnectInterface.UpdateFlightPassengerResponse)(req).Success
			End If
		End Function

#End Region

#Region "Support Classes"

        Public Class BookingReferences
            Public BookingReference As String
            Public FlightBookingReference As String
            Public SupplierReference As String
        End Class

        Public Class ApisPassengers
            Inherits Generic.List(Of ApisPassenger)
        End Class

        Public Class ApisPassenger

            Private _Title As String
            Private _FirstName As String
            Private _MiddleName As String
            Private _LastName As String
            Private _DateOfBirth As String
            Private _Nationality As String
            Private _NationalityCode As String
            Private _PassportNumber As String
            Private _PassportIssueDate As String
            Private _PassportExpiryDate As String
            Private _PassportIssuePlaceID As Integer
            Private _PassportIssuePlaceName As String
            Private _FlightBookingPassengerID As Integer
            Private _Gender As String

            Public Property Title As String
                Get
                    Return _Title
                End Get
                Set(value As String)

                    _Title = value
                End Set
            End Property

            Public Property FirstName As String
                Get
                    Return _FirstName
                End Get
                Set(value As String)

                    _FirstName = value
                End Set
            End Property

            Public Property MiddleName As String
                Get
                    Return _MiddleName
                End Get
                Set(value As String)

                    _MiddleName = value
                End Set
            End Property

            Public Property LastName As String
                Get
                    Return _LastName
                End Get
                Set(value As String)

                    _LastName = value
                End Set
            End Property

            Public Property DateOfBirth As String
                Get
                    Return _DateOfBirth
                End Get
                Set(value As String)
                    _DateOfBirth = value
                End Set
            End Property

            Public Property Nationality As String
                Get
                    Return _Nationality
                End Get
                Set(value As String)
                    _Nationality = value
                End Set
            End Property

            Public Property NationalityCode As String
                Get
                    Return _NationalityCode
                End Get
                Set(value As String)
                    _NationalityCode = value
                End Set
            End Property

            Public Property PassportNumber As String
                Get
                    Return _PassportNumber
                End Get
                Set(value As String)
                    _PassportNumber = value
                End Set
            End Property

            Public Property PassportIssueDate As String
                Get
                    Return _PassportIssueDate
                End Get
                Set(value As String)
                    _PassportIssueDate = value
                End Set
            End Property

            Public Property PassportExpiryDate As String
                Get
                    Return _PassportExpiryDate
                End Get
                Set(value As String)
                    _PassportExpiryDate = value
                End Set
            End Property

            Public Property PassportIssuePlaceID As Integer
                Get
                    Return _PassportIssuePlaceID
                End Get
                Set(value As Integer)
                    _PassportIssuePlaceID = value
                End Set
            End Property

            Public Property PassportIssuePlaceName As String
                Get
                    Return _PassportIssuePlaceName
                End Get
                Set(value As String)
                    _PassportIssuePlaceName = value
                End Set
            End Property

            Public Property FlightBookingPassengerID As Integer
                Get
                    Return _FlightBookingPassengerID
                End Get
                Set(value As Integer)
                    _FlightBookingPassengerID = value
                End Set
            End Property

            Public Property Gender As String
                Get
                    Return _Gender
                End Get
                Set(value As String)
                    _Gender = value
                End Set
            End Property

        End Class

        Public Enum ApisPassengerType
            Adult
            Child
            Infant
        End Enum

        <XmlRoot("IssueLocations")>
        Public Class IssueLocations
            Inherits Generic.List(Of IssueLocation)

        End Class

        Public Class IssueLocation
            Public Property LocationID As Integer
            Public Property LocationName As String
        End Class

#End Region

    End Class

End Namespace