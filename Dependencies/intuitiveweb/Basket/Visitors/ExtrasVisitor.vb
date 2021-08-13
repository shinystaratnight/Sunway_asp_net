Imports Intuitive.Web.BookingExtra.BasketExtra

Namespace Basket.Visitors

	Public Class ExtrasVisitor
		Implements Basket.Interfaces.IBasketVisitor

		Private Identifier As String
		Private Quantity As Integer
        Private Key As String
        Private ClearExtrasOfTheSameType As Boolean
        Private IncludeOptionsAtBooking As Boolean
        Private ComponentID As Integer

        Public Sub New(Identifier As String, Quantity As Integer, Key As String, ClearExtrasOfTheSameType As Boolean,
                       IncludeOptionsAtBooking As Boolean, ComponentID As Integer)
            Me.Identifier = Identifier
            Me.Quantity = Quantity
            Me.Key = Key
            Me.ClearExtrasOfTheSameType = ClearExtrasOfTheSameType
            Me.IncludeOptionsAtBooking = IncludeOptionsAtBooking
            Me.ComponentID = ComponentID
        End Sub


        Public Function Visit(Visitable As Basket.Interfaces.IBasketVisitable) As Integer Implements Basket.Interfaces.IBasketVisitor.Visit

			Dim oBookingBasket As BookingBasket = CType(Visitable, BookingBasket)

			Dim sBookingToken As String = BookingBase.SearchDetails.ExtraResults.BookingTokens(Key)

            '1. Get the handler result
            Dim oExtra As ExtraResultHandler.Extra = BookingBase.SearchDetails.ExtraResults.GetSingleExtra(Me.Identifier, sBookingToken)

            '2. Use the index and booking token to ensure we have the right extra with the right options and hash token that baby up.
            Dim sHashtoken As String = BookingBase.SearchDetails.ExtraResults.ExtraHashToken(Me.Identifier, oExtra.Index, sBookingToken)

            '3. Get extra option from hashtoken
            Dim oExtraOption As BasketExtraOption = BasketExtraOption.DeHashToken(Of BasketExtraOption)(sHashtoken)

            '3. Add extra to basket

            'If ClearBasket Then
            '	If BasketType = eBasketType.Basket Then
            '		BookingBase.Basket.BasketExtras.Clear()
            '	Else
            '		BookingBase.SearchBasket.BasketExtras.Clear()
            '	End If
            'End If

            If Me.ClearExtrasOfTheSameType Then
				For i As Integer = BookingBase.SearchBasket.BasketExtras.Count To 1 Step -1
					If BookingBase.SearchBasket.BasketExtras(i - 1).ExtraTypeID = oExtra.ExtraTypeID Then
						BookingBase.SearchBasket.BasketExtras.RemoveAt(i - 1)

					End If
				Next
			End If

            Dim oBasketExtra As BookingExtra.BasketExtra

            ' If ComponentID = 0 Then

            'If we are using the options at booking, then we want multiple options on a single extra for more quantity.
            'Dim iOptionsQuanity As Integer = Intuitive.ToSafeInt(IIf(IncludeOptionsAtBooking, Me.Quantity, 1))

            'create basket extra
            oBasketExtra = New BookingExtra.BasketExtra
            With oBasketExtra
                .IncludeOptionsAtBooking = Me.IncludeOptionsAtBooking
                .BasketExtraOptions = New Generic.List(Of BasketExtraOption)
                .ComponentID = oBookingBasket.TotalComponents + 1
                .ExtraTypeID = oExtra.ExtraTypeID
                '.ConfirmationReference = ConfirmationReference
            End With

            'set content xml
            oExtraOption.ContentXML = Serializer.Serialize(oExtra, True)

            '  For i As Integer = 1 To iOptionsQuanity
            ' oBasketExtra.BasketExtraOptions.Add(oExtraOption)
            ' Next

            ' If GuestIDs IsNot Nothing Then oBasketExtra.GuestIDs = GuestIDs

            'If we are not using the options at booking, then we want multiple extras, not multiple options
            '  Quantity = Intuitive.ToSafeInt(IIf(Not IncludeOptionsAtBooking, Me.Quantity, 1))

            Dim oBasket As BookingBasket
            'If BasketType = eBasketType.Basket Then
            'oBasket = BookingBase.Basket
            'Else
            oBasket = BookingBase.SearchBasket
            'End If

            For i As Integer = 1 To Me.Quantity
                'Dim oAddtoBasketExtra As New BookingExtra.BasketExtra
                'oAddtoBasketExtra = BookingExtra.CloneExtra(oBasketExtra)
                'oBasket.BasketExtras.Add(oAddtoBasketExtra)
                oExtraOption.ContentXML = Serializer.Serialize(oExtra, True)

                oBasketExtra.BasketExtraOptions.Add(oExtraOption)
            Next

            If Me.Quantity > 0 Then
                oBasket.BasketExtras.Add(oBasketExtra)
            End If

            'already clearing down each time
            'If Me.Quantity = 0 Then
            '    BookingExtra.RemoveExtraFromBasket(sHashtoken)
            'End If

            Return oBasketExtra.ComponentID
            'Else
            ''add to existing extra
            'oBasketExtra = BookingBase.SearchBasket.BasketExtras.Where(Function(o) o.ComponentID = ComponentID).FirstOrDefault
            'If oBasketExtra Is Nothing Then Throw New Exception("No Extra Component matching ComponentID " & ComponentID.ToString & " in search basket")
            'For i As Integer = 1 To Me.Quantity

            '    'set content xml
            '    '  Dim oExtra As ExtraResultHandler.Extra = BookingBase.SearchDetails.ExtraResults.GetSingleExtra(Identifier, oExtraOption.BookingToken)

            '    oExtraOption.ContentXML = Serializer.Serialize(oExtra, True)

            '    oBasketExtra.BasketExtraOptions.Add(oExtraOption)
            'Next

            ' Return ComponentID

            ' End If



        End Function
	End Class


End Namespace