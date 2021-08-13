Imports System.Xml.Serialization

Namespace CarHire

    <XmlRoot("CarHireInsuranceQuoteResponse")>
    Public Class InsuranceQuoteResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

        Public Property BookingToken As String
        Public Property PremiumName As String
        Public Property FullPremiumName As String
        Public Property PremiumAmount As Decimal
        Public Property ProviderLogoURL As String
        Public Property InsuranceDescription As String
        Public Property InsuranceRegulationsText As String
        Public Property TermsAndConditions As String
        Public Property TermsAndConditionsURL As String
        Public Property ConfirmationText As String

        <XmlArrayItem("CoveredItem")>
        Public Property CoveredItems As New Generic.List(Of String)
    End Class

End Namespace