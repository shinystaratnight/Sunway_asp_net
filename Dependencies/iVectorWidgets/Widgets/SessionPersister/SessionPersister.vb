Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.Drawing
Imports System.Net
Imports System.IO

Public Class SessionPersister
	Inherits WidgetBase


	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

		'2. Get xml from content path
		Dim oXML As New XmlDocument


		'4. Transform
		Me.XSLTransform(oXML, res.SessionPersister, writer)


	End Sub

	Public Shared Sub Persist()

		'We need to access something on the session to keep it alive,
		Intuitive.Web.BookingBase.Params.Theme = Intuitive.Web.BookingBase.Params.Theme

	End Sub

End Class

