Public Class ThreeDSecure
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        me.plcContent.Text = BookingBase.Basket.ThreeDSecureHTML

    End Sub

End Class