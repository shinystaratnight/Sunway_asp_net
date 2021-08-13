Imports Intuitive.Functions

Namespace Tracking

    ''' <summary>
    ''' Class to represent data entity from iVC - not exclusively an affiliate, just an object for tracking/affiliate data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TrackingAffiliate

        Public TrackingAffiliateID As Integer
        Public Name As String
        Public BrandID As Integer
        Public Type As String
        Public QueryStringIdentifier As String
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
        Public BookingType As String
        Public BookingTypeIDs As String
        Public ValidForDays As Integer = 0


        Public Function Clone() As TrackingAffiliate

            Return Serializer.DeSerialize(Of TrackingAffiliate)(Serializer.Serialize(Me, True).InnerXml)

        End Function


        Public Class SelectedPage
            Public Page As String
        End Class

    End Class

End Namespace

