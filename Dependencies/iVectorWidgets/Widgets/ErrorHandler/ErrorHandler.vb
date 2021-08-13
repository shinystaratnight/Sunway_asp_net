Imports Intuitive.Web.Widgets
Imports Intuitive
Imports Intuitive.Functions
Imports System.Text
Imports System.Net
Imports System.Configuration.ConfigurationManager
Imports Intuitive.Web

Public Class ErrorHandler
	Inherits WidgetBase

	Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

		'email error
		ErrorHandler.EmailError()

		'xml
		Dim oXML As New System.Xml.XmlDocument

		'check for override
		Dim sTemplateOverride As String = Settings.GetValue("TemplateOverride")

		'transform
		If sTemplateOverride <> "" Then
			Me.XSLPathTransform(oXML, HttpContext.Current.Server.MapPath("~" & sTemplateOverride), writer)
		Else
			Me.XSLTransform(oXML, res.ErrorHandler, writer)
		End If

	End Sub

	Public Shared Sub EmailError()

		'dont fall over in here just trying to email
		Try

			Dim sb As New StringBuilder
			With sb

				'user and date info
				.Append("Date and Time : ").Append(Now.ToString("dd MMM yy HH:mm")).Append("\n")

				'do we have an exception
				Dim oException As Exception = CType(HttpContext.Current.Cache("lasterror"), Exception)
				Dim sUrl As String = SafeString(HttpContext.Current.Cache("errorurl"))

				If Not oException Is Nothing Then

					'gather as much error info as possible about the server, pass that back along with the whole exception
					.Append("Server Name    ").Append(Dns.GetHostName).Append("\n")

					'ip address
					Dim oHost As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName)
					For Each oIPAddress As IPAddress In oHost.AddressList
						.Append("IP Address     ").Append(oIPAddress.ToString).Append("\n")
					Next

					'client info
					.Append("Client IP         ").Append(HttpContext.Current.Request.UserHostAddress).Append("\n")
					.Append("Client Host Name  ").Append(HttpContext.Current.Request.UserHostName).Append("\n")

					'exception
					.Append("\n\n").Append("Details").Append("\n")
					.Append(oException.ToString)

					'basket
					If Not BookingBase.Basket Is Nothing Then
						Try
							.Append("\n\nBasket XML      \n").Append(XMLFunctions.FormatXML(BookingBase.Basket.XML.InnerXml)).Append("\n")
						Catch ex As Exception
						End Try
					End If

					'search basket
					If Not BookingBase.SearchBasket Is Nothing Then
						Try
							.Append("\n\nSearch Basket XML      \n").Append(XMLFunctions.FormatXML(BookingBase.SearchBasket.XML.InnerXml)).Append("\n")
						Catch ex As Exception
						End Try
					End If

					'remove from page cache
					HttpContext.Current.Cache.Remove("lasterror")
					HttpContext.Current.Cache.Remove("errorurl")

					'send the email
					Dim sEmail As String = SafeString(AppSettings("SystemEmail"))
					If sEmail <> "" Then

						Dim oEmail As New Email
						With oEmail
							.SMTPHost = AppSettings("SMTPHost")
							.EmailTo = sEmail
							.From = "intuitive error"
							.FromEmail = "errors@intuitivesystems.co.uk"

							.Subject = "Error in " & sUrl

							.Body = sb.ToString.Replace("\n", ControlChars.NewLine)

							.SendEmail()
						End With

					End If

				End If

			End With


		Catch ex As Exception

		End Try

	End Sub

End Class