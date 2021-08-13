Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace CarHire
	<XmlType("CarHirePreBookResponse")>
	<XmlRoot("CarHirePreBookResponse")>
	Public Class PreBookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property BookingToken As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property RentalConditions As String
		Public Property PaymentsDue As New List(Of Support.PaymentDue)
		Public Property Cancellations As New CancellationsList(Of Cancellation)
		Public Property HireConditions As New List(Of HireCondition)
		Public Property CommissionType As String
        Public Property CommissionValue As String
        Public Property PayLocalAvailable As Boolean
        Public Property LocalPrice As Decimal
        Public Property LocalCurrencyID As Integer
        Public Property Extras As New List(Of SearchResponse.CarHireExtra)
        Public Property BondingID As Integer

        <XmlIgnore()>
		Public Property hlpCarHireBookingID As Integer

		Public Class HireCondition
			Public Property Title As String
            Public Property Body As String
            Public Property IsLink As Boolean
                Get
                    Return Intuitive.Validators.IsURL(Me.Body)
                End Get
                Set(value As Boolean)

                End Set
            End Property
        End Class
	End Class
End Namespace


