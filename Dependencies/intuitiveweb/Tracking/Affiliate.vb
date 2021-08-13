
Namespace Tracking

    Public Class Affiliate

#Region "private members"

        Private Const COOKIE_KEY As String = "__tracking_affiliate_id"
        Private _TrackingAffiliateID As Integer

#End Region

#Region "public members"

        Public ReadOnly Property TrackingAffiliateID As Integer
            Get
                Return Me._TrackingAffiliateID
            End Get
        End Property


#End Region

#Region "public methods"

        Public Sub New()

            'upon setup - check if the cookie exists, if it does then set the value
            Me._TrackingAffiliateID = Functions.SafeInt(CookieFunctions.Cookies.GetValue(COOKIE_KEY))

        End Sub


        Public Sub SetupFromQueryString(RequestUrl As String, TrackingAffiliates As List(Of Tracking.TrackingAffiliate))

            Dim oTrackingAffiliateList As IEnumerable(Of Tracking.TrackingAffiliate) = TrackingAffiliates.Where(Function(o) o.Type = "Affiliate" _
                                                                                                AndAlso RequestUrl.Contains(o.QueryStringIdentifier.ToLower))
            Dim oTrackingAffiliate As New Tracking.TrackingAffiliate


            If oTrackingAffiliateList.Count > 0 Then
                oTrackingAffiliate = oTrackingAffiliateList.First
                Me.SetCookie(oTrackingAffiliate.TrackingAffiliateID, oTrackingAffiliate.ValidForDays)
                Me._TrackingAffiliateID = oTrackingAffiliate.TrackingAffiliateID
            End If



        End Sub

#End Region

#Region "private methods"


        Private Function GetCookieValue() As Integer

            Try
                Return Functions.SafeInt(CookieFunctions.Cookies.GetValue(COOKIE_KEY))
            Catch ex As Exception
                Return 0
            End Try

        End Function


        Private Sub SetCookie(ByVal TrackingAffiliateID As Integer, ByVal ExpiryDays As Integer)

            Me._TrackingAffiliateID = TrackingAffiliateID
            ' Me.ExpiryDays = ExpiryDays

            Dim oCookie As HttpCookie = HttpContext.Current.Request.Cookies(COOKIE_KEY)

            If oCookie Is Nothing Then
                oCookie = New HttpCookie(COOKIE_KEY)
            End If

            oCookie.Value = TrackingAffiliateID.ToString
            oCookie.Expires = Now.AddDays(ExpiryDays)
            HttpContext.Current.Response.Cookies.Add(oCookie)

        End Sub

#End Region


    End Class

End Namespace

