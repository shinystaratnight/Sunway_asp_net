Imports ivci = iVectorConnectInterface
Imports Intuitive.Functions
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Text

Namespace Tracking

    Public Class Token

#Region "public members"

        Public TotalPrice As Decimal = 0
        Public PropertyID As Integer = 0
        Public TrackingPropertyID As Integer = 0
        Public BookingReference As String = ""
        Public Resort As String = ""
        Public Region As String = ""
        Public BookingType As String = ""
        Public BookingTypeID As String = ""
        Public PropertyName As String = ""
        Public PropertyPrice As Decimal = 0
        Public PropertyRatesType As String = ""
        Public DepartureAirportName As String = ""
        Public ArrivalAirportName As String = ""
        Public PromoCode As String = ""
        Public DepartureDate As DateTime = DateFunctions.EmptyDate
        Public ArrivalDate As DateTime = DateFunctions.EmptyDate
        Public Transfer As String = ""
        Public TransferValue As Decimal = 0
        Public FirstDepartureDateTimestamp As Integer
        Public LastReturnDateTimestamp As Integer
        Public FlightAmount As Decimal = 0
        Public IsPreferred As String = ""
        Public TotalExtras As Decimal = 0


#End Region

#Region "public methods"

        Public Sub Generate(ByVal BookingTypeIDs As String)


            If BookingBase.Basket.BasketProperties.Count > 0 AndAlso BookingBase.Basket.BasketFlights.Count > 0 Then
                Me.BookingType = "FlightAndAccom"
            ElseIf BookingBase.Basket.BasketProperties.Count > 0 AndAlso BookingBase.Basket.BasketFlights.Count = 0 Then
                Me.BookingType = "Accom"
            ElseIf BookingBase.Basket.BasketProperties.Count = 0 AndAlso BookingBase.Basket.BasketFlights.Count > 0 Then
                Me.BookingType = "Flight"
            End If

            Me.BookingTypeID = Me.GetTrackingAffiliateBookingTypeID(Me.BookingType, BookingTypeIDs)

            If BookingBase.Basket.BookingReference IsNot Nothing Then
                Me.BookingReference = BookingBase.Basket.BookingReference
                Me.TotalPrice = BookingBase.Basket.TotalPrice
                Me.PromoCode = BookingBase.Basket.PromoCode
            End If

            If BookingBase.Basket.BasketProperties.Count > 0 AndAlso
                    BookingBase.Basket.BasketProperties(0).RoomOptions.Count > 0 Then

                ' this will no longer be property id
                Me.PropertyID = BookingBase.Basket.BasketProperties(0).RoomOptions(0).PropertyReferenceID
                Me.PropertyName = BookingBase.Basket.BasketProperties(0).RoomOptions(0).PropertyName
                Me.ArrivalDate = BookingBase.Basket.BasketProperties(0).RoomOptions(0).ArrivalDate
                Me.DepartureDate = BookingBase.Basket.BasketProperties(0).RoomOptions(0).DepartureDate
                Me.Resort = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Resort, BookingBase.Basket.BasketProperties(0).RoomOptions(0).GeographyLevel3ID)
                Dim oLocation As Lookups.Location = BookingBase.Lookups.Locations.Where(Function(o) o.GeographyLevel3ID = BookingBase.Basket.BasketProperties(0).RoomOptions(0).GeographyLevel3ID).FirstOrDefault
                If oLocation IsNot Nothing Then
                    Me.Region = oLocation.GeographyLevel2Name
                End If
                Me.PropertyPrice = BookingBase.Basket.BasketProperties(0).RoomOptions.Sum(Function(oRoom) oRoom.TotalPrice)
                Me.PropertyRatesType = IIf(BookingBase.Basket.BasketProperties(0).RoomOptions(0).hlpAffiliatePreferredRates, "PreferredRates", "NormalHotel")
                Me.IsPreferred = IIf(BookingBase.Basket.BasketProperties(0).RoomOptions(0).hlpAffiliatePreferredRates, "Preferred", "")
            End If

            If BookingBase.Basket.BasketFlights.Count > 0 Then
                Me.DepartureAirportName = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, BookingBase.Basket.BasketFlights(0).Flight.DepartureAirportID)
                Me.ArrivalAirportName = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.Airport, BookingBase.Basket.BasketFlights(0).Flight.ArrivalAirportID)
                Me.DepartureDate = BookingBase.Basket.BasketFlights(0).Flight.OutboundDepartureDate
                Me.ArrivalDate = BookingBase.Basket.BasketFlights(0).Flight.ReturnArrivalDate
                Me.FlightAmount = BookingBase.Basket.BasketFlights(0).Flight.Price
            End If

            Me.Transfer = IIf(BookingBase.Basket.BasketTransfers.Count > 0, "Transfer", "NoTransfer")
            Dim dTransferPrice As Decimal = 0
            If BookingBase.Basket.BasketTransfers.Count > 0 Then
                dTransferPrice = BookingBase.Basket.BasketTransfers(0).Transfer.Price
            End If
            Me.TransferValue = dTransferPrice
            Me.TotalExtras = BookingBase.Basket.BasketExtras.Sum(Function(oExtra) oExtra.BasketExtraOptions.Sum(Function(oOption) oOption.TotalPrice))
            Me.TotalExtras += Me.TransferValue
            ' + car hire, + adjustments

            If BookingBase.Basket.BasketFlights.Count > 0 Then
                Me.FirstDepartureDateTimestamp = ConvertToTimestampFormat(BookingBase.Basket.BasketFlights(0).Flight.OutboundDepartureDate)
                Me.LastReturnDateTimestamp = ConvertToTimestampFormat(BookingBase.Basket.BasketFlights(0).Flight.ReturnArrivalDate)
            ElseIf BookingBase.Basket.BasketProperties.Count > 0 Then
                Me.FirstDepartureDateTimestamp = ConvertToTimestampFormat(BookingBase.Basket.BasketProperties(0).RoomOptions(0).ArrivalDate)
                Me.LastReturnDateTimestamp = ConvertToTimestampFormat(BookingBase.Basket.BasketProperties(0).RoomOptions(0).DepartureDate)
            End If



        End Sub


        Public Function Detokenize(ByVal oPageTrackingAffiliates As IEnumerable(Of Tracking.TrackingAffiliate), ByVal ScriptType As String,
             oTokenDetails As Tracking.Token) As IEnumerable(Of Tracking.TrackingAffiliate)

            If ScriptType.ToLower = "confirmation" Then
				For Each oTrackingAffiliate As TrackingAffiliate In oPageTrackingAffiliates.Where(Function(oTA) oTA.ConfirmationScript <> "")
					oTrackingAffiliate.ConfirmationScript = oTokenDetails.ReplaceScriptKeys(oTrackingAffiliate, oTrackingAffiliate.ConfirmationScript,
																							oTokenDetails)
				Next
			ElseIf ScriptType.ToLower = "landingpage" Then
				For Each oTrackingAffiliate As TrackingAffiliate In oPageTrackingAffiliates.Where(Function(oTA) oTA.LandingPageScript <> "")
					oTrackingAffiliate.LandingPageScript = oTokenDetails.ReplaceScriptKeys(oTrackingAffiliate, oTrackingAffiliate.LandingPageScript,
																						   oTokenDetails)
				Next
			End If

            ' general scripts

            'secure
            For Each oTrackingAffiliate As TrackingAffiliate In oPageTrackingAffiliates.Where(Function(oTA) oTA.Secure = 1 AndAlso oTA.SecureScript <> "")
				oTrackingAffiliate.SecureScript = oTokenDetails.ReplaceScriptKeys(oTrackingAffiliate, oTrackingAffiliate.SecureScript, oTokenDetails)
			Next

            'not secure
            For Each oTrackingAffiliate As TrackingAffiliate In oPageTrackingAffiliates.Where(Function(oTA) oTA.Secure = 0 AndAlso oTA.Script <> "")
				oTrackingAffiliate.Script = oTokenDetails.ReplaceScriptKeys(oTrackingAffiliate, oTrackingAffiliate.Script, oTokenDetails)
			Next

			Return oPageTrackingAffiliates

        End Function



#End Region


#Region "Delegate Tokens"

        Public Class DelegateDateTokens

            Public oTokenDetails As Token

            Public Sub New(ByVal o As Token)
                Me.oTokenDetails = o
            End Sub

            Public Function CaptureAndReplaceDateTokens(ByVal sScript As String) As String

                Dim RegX As Regex = New RegularExpressions.Regex("(?<TagName>\#\#DepartureDate|\#\#ArrivalDate)(?:~(?<DateFormat>[^\#]+))*\#\#")
                Dim oEval As MatchEvaluator = New MatchEvaluator(AddressOf ReplaceDateToken)
                If sScript <> "" Then sScript = Regex.Replace(sScript, RegX.ToString, oEval, RegexOptions.Compiled)
                Return sScript

            End Function

            Private Function ReplaceDateToken(ByVal match As Match) As String
                Dim TokenDate As DateTime
                Dim TagName As String = ""
                Dim DateFormat As String = ""

                TagName = match.Groups("TagName").ToString.Replace("#", "")
                DateFormat = match.Groups("DateFormat").ToString.Replace("m", "M")

                If TagName = "ArrivalDate" Then
                    TokenDate = Me.oTokenDetails.ArrivalDate

                ElseIf TagName = "DepartureDate" Then
                    TokenDate = Me.oTokenDetails.DepartureDate

                Else
                    TokenDate = DateFunctions.EmptyDate
                End If

                If DateFormat <> "" Then
                    If DateFormat = "unix" Then
                        Dim DateUnix As TimeSpan = TokenDate - New System.DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()
                        Return DateUnix.TotalSeconds.ToString
                    End If

                    Dim aDateFormat() As String = DateFormat.Replace("m", "M").Split("/"c)

                    If aDateFormat.Length = 3 Then
                        Return TokenDate.ToString(aDateFormat(0).ToString & "/" & aDateFormat(1).ToString & "/" & aDateFormat(2).ToString)
                    Else
                        Return TokenDate.ToString("yyyy/MM/dd")
                    End If

                Else
                    Return TokenDate.ToString("yyyy/MM/dd")
                End If

            End Function
        End Class


        Public Class DelegatePercentageTokens

            Public oTokenDetails As Token

            Public Sub New(ByVal o As Token)
                Me.oTokenDetails = o
            End Sub

            Public Function CaptureAndReplacePercentageTokens(ByVal sScript As String) As String

                'currently only set up for totalprice but could be used for other numeric tags. NB the filter corresponds to the booking type
                Dim RegX As Regex = New RegularExpressions.Regex("(?<TagName>\#\#TotalPrice)(?:|(?<Filter>[^~]+))(?:~(?<PercentAmount>[^\#]+))*\#\#")
                Dim oEval As MatchEvaluator = New MatchEvaluator(AddressOf ReplacePercentageToken)
                If sScript <> "" Then sScript = Regex.Replace(sScript, RegX.ToString, oEval, RegexOptions.Compiled)
                Return sScript

            End Function

            Public Function ReplacePercentageToken(ByVal match As Match) As String
                Dim TagName As String = ""
                Dim Filter As String = ""
                Dim PercentAmount As Decimal = 0

                TagName = match.Groups("TagName").ToString.Replace("#", "")
                Filter = match.Groups("Filter").ToString.Replace("|", "")
                PercentAmount = SafeDecimal(match.Groups("PercentAmount").ToString.Replace("%", ""))

                If Filter <> "" Then
                    If Not Filter = oTokenDetails.BookingTypeID.ToString Then
                        Return SafeString(0)
                    End If
                End If

                If TagName = "TotalPrice" Then
                    Return SafeString(Math.Round(oTokenDetails.TotalPrice * (PercentAmount / 100), 2))
                Else
                    Return SafeString(0)
                End If
            End Function

        End Class


#End Region



#Region "private methods"

        Private Function ConvertToTimestampFormat(oDate As DateTime) As Integer

            Dim oSpan As TimeSpan = (oDate - New DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime())
            Return SafeInt(oSpan.TotalSeconds)

        End Function


        Private Function GetTrackingAffiliateBookingTypeID(ByVal BookingType As String, ByVal BookingTypeIDs As String) As String

            If BookingTypeIDs = String.Empty Then Return BookingTypeIDs

            Try
                Dim aSettings As String() = BookingTypeIDs.Split("|"c)
                For Each sTypeIDPair As String In aSettings
                    Dim aTypeIDPair As String() = sTypeIDPair.Split("="c)
                    If aTypeIDPair(0) = BookingType Then Return aTypeIDPair(1)
                Next

            Catch ex As Exception
                Return String.Empty
            End Try

            Return String.Empty

        End Function


        Private Function ReplaceScriptKeys(ByVal oTrackingAffiliate As Tracking.TrackingAffiliate, ByVal sScript As String,
         ByVal oTokenDetails As Tracking.Token) As String

            If oTrackingAffiliate IsNot Nothing Then
                If oTokenDetails.BookingType = "FlightAndAccom" And Not oTrackingAffiliate.FlightAndAccomTokenOverride = "" Then
                    oTokenDetails.BookingType = oTrackingAffiliate.FlightAndAccomTokenOverride
                ElseIf oTokenDetails.BookingType = "Accom" And Not oTrackingAffiliate.AccomTokenOverride = "" Then
                    oTokenDetails.BookingType = oTrackingAffiliate.AccomTokenOverride
                ElseIf oTokenDetails.BookingType = "Flight" And Not oTrackingAffiliate.FlightTokenOverride = "" Then
                    oTokenDetails.BookingType = oTrackingAffiliate.FlightTokenOverride
                End If
            End If

            If oTokenDetails.BookingReference <> "" Then
                sScript = Replace(sScript, "##BookingReference##", oTokenDetails.BookingReference)
                sScript = Replace(sScript, "##TotalPrice##", oTokenDetails.TotalPrice.ToString("######0.00"))
                sScript = Replace(sScript, "##PromoCode##", oTokenDetails.PromoCode.Replace("'", "\'"))
            End If

            If BookingBase.Basket.BasketProperties.Count > 0 Then
                sScript = Replace(sScript, "##PropertyID##", oTokenDetails.PropertyID.ToString)
                sScript = Replace(sScript, "##AccomName##", oTokenDetails.PropertyName.Replace("'", "\'"))
                sScript = Replace(sScript, "##Resort##", oTokenDetails.Resort.Replace("'", "\'"))
                sScript = Replace(sScript, "##Region##", oTokenDetails.Region.Replace("'", "\'"))
                sScript = Replace(sScript, "##PropertyPrice##", oTokenDetails.PropertyPrice.ToString)
                sScript = Replace(sScript, "##TrackingPropertyID##", oTokenDetails.TrackingPropertyID.ToString)
                sScript = Replace(sScript, "##PropertyRatesType##", oTokenDetails.PropertyRatesType)
                sScript = Replace(sScript, "##IsPreferred##", oTokenDetails.IsPreferred)
            End If

            If BookingBase.Basket.BasketFlights.Count > 0 Then
                sScript = Replace(sScript, "##DepartureAirport##", oTokenDetails.DepartureAirportName.Replace("'", "\'"))
                sScript = Replace(sScript, "##ArrivalAirport##", oTokenDetails.ArrivalAirportName.Replace("'", "\'"))
                sScript = Replace(sScript, "##FlightAmount##", oTokenDetails.FlightAmount.ToString("######0.00"))
            Else
                sScript = Replace(sScript, "##DepartureAirport##", "")
                sScript = Replace(sScript, "##ArrivalAirport##", "")
                sScript = Replace(sScript, "##FlightAmount##", "")
            End If

            sScript = Replace(sScript, "##DepartureDate~dd/mm/yy##", oTokenDetails.DepartureDate.ToString("dd/MM/yy"))
            sScript = Replace(sScript, "##DepartureDate##", oTokenDetails.DepartureDate.ToString("yyyy/MM/dd"))
            sScript = Replace(sScript, "##ArrivalDate~dd/mm/yy##", oTokenDetails.ArrivalDate.ToString("dd/MM/yy"))
            sScript = Replace(sScript, "##ArrivalDate##", oTokenDetails.ArrivalDate.ToString("yyyy/MM/dd"))

            sScript = Replace(sScript, "##Type##", oTokenDetails.BookingType)
            sScript = Replace(sScript, "##TypeID##", oTokenDetails.BookingTypeID.ToString)

            sScript = Replace(sScript, "##Transfer##", oTokenDetails.Transfer.Replace("'", "\'"))
            sScript = Replace(sScript, "##TransferValue##", oTokenDetails.TransferValue.ToString("######0.00"))
            sScript = Replace(sScript, "##TotalExtras##", oTokenDetails.TotalExtras.ToString("######0.00"))

            sScript = Replace(sScript, "##FirstDepartureDateTimestamp##", oTokenDetails.FirstDepartureDateTimestamp.ToString)
            sScript = Replace(sScript, "##LastReturnDateTimestamp##", oTokenDetails.LastReturnDateTimestamp.ToString)

            Dim oDateDeleage As New Tracking.Token.DelegateDateTokens(oTokenDetails)
            sScript = oDateDeleage.CaptureAndReplaceDateTokens(sScript)

            Dim oPercentDelegate As New Tracking.Token.DelegatePercentageTokens(oTokenDetails)
            sScript = oPercentDelegate.CaptureAndReplacePercentageTokens(sScript)

            'If sScript <> "" AndAlso sScript.Contains("##additionalcode##") Then sScript = sScript.Replace("##additionalcode##", sAdditionalTrackingCode)
            If sScript <> "" AndAlso sScript.Contains("##Rand##") Then sScript = sScript.Replace("##Rand##", RandomInteger(0, 10000).ToString)
            Return sScript

        End Function

#End Region



    End Class


End Namespace
