Public Class GetTrackingAffiliatesResponse
		Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Properties"

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public TrackingAffiliates As Generic.List(Of TrackingAffiliate)
	Public TrackingAffiliateTypeIDs As String

#End Region

#Region "Inner Classes"

	Public Class TrackingAffiliate
		Public TrackingAffiliateID As Integer
		Public Name As String
		Public BrandID As Integer
		Public Type As String
		Public QueryStringIdentifier As String
		Public ValidForDays As Integer
		Public Script As String
		Public LandingPageScript As String
		Public ConfirmationScript As String
		Public Pages As String
		Public Position As String
		Public Secure As Integer
		Public SecureScript As String
		Public CMSWebsiteID As Integer
		Public FlightAndAccomTokenOverride As String
		Public AccomTokenOverride As String
		Public FlightTokenOverride As String
		Public SelectedPages As Generic.List(Of SelectedPage)
	End Class

	Public Class SelectedPage
		Public Page As String
	End Class

#End Region

End Class
