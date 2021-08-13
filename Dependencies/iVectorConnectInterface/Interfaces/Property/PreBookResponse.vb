Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial
Imports iVectorConnectInterface.Interfaces

Namespace [Property]
	<XmlType("PropertyPreBookResponse")>
	<XmlRoot("PropertyPreBookResponse")>
	Public Class PreBookResponse

		Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse
		Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
		Public Property BookingToken As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property SupplierDetails As Support.SupplierDetails
		Public Property Cancellations As New CancellationsList(Of Cancellation)
		Public Property PaymentsDue As New List(Of Support.PaymentDue)
		Public Property Errata As New List(Of Support.Erratum)
		Public Property CommissionType As String
		Public Property CommissionValue As String

		<XmlIgnore()>
		Public Property hlpPropertyBookingID As Integer

		<XmlIgnore()>
		Public Property hlpCurrencyID As Integer

		<XmlArrayItem("Comment")>
		Public Property Comments As New Generic.List(Of String)

		'Terms and conditions
		Public Property TermsAndConditions As String
		Public Property TermsAndConditionsURL As String
		Public Property OptionRequired As Boolean

		Public Property PayLocalAvailable As Boolean
		Public Property PayLocalRequired As Boolean
		Public Property LocalPrice As Decimal
		Public Property LocalCurrencyID As Integer
        Public Property BondingID As Integer
        Public Property InstallmentPaymentOptions As New List(Of Support.InstalmentPaymentOption)

		<XmlIgnore>
		Public Property PriceChange As Boolean

    End Class

End Namespace
