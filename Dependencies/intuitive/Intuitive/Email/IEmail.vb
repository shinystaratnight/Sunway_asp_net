Imports System.IO
Imports System.Xml.Serialization

Public Interface IEmail

	''' <summary>
	''' The SMTP port.
	''' </summary>
	Property SMTPPort As Integer

	''' <summary>
	''' The SMTP Username
	''' </summary>
	Property Username As String

	''' <summary>
	''' The SMTP Password
	''' </summary>
	Property Password As String

	''' <summary>
	''' Gets or sets the SMTP host.
	''' </summary>
	Property SMTPHost() As String

	''' <summary>
	''' Gets or sets the from field.
	''' </summary>
	Property From() As String

	''' <summary>
	''' Gets or sets the from email.
	''' </summary>
	Property FromEmail() As String

	''' <summary>
	''' Gets or sets the email to field.
	''' </summary>
	Property EmailTo() As String

	''' <summary>
	''' Gets or sets the CC field
	''' </summary>
	Property CC() As String

	''' <summary>
	''' Gets or sets the BCC field
	''' </summary>
	Property BCC() As String

	''' <summary>
	''' Gets or sets the subject field.
	''' </summary>
	Property Subject() As String

	''' <summary>
	''' Gets or sets the email body.
	''' </summary>
	Property Body() As String

	''' <summary>
	''' Gets or sets the attachments.
	''' </summary>
	Property Attachments() As ArrayList

	''' <summary>
	''' Gets or sets the last error.
	''' </summary>
	Property LastError() As String

	''' <summary>
	''' Gets or sets the priority.
	''' </summary>
	Property Priority() As System.Net.Mail.MailPriority

	''' <summary>
	''' Gets or sets the stream of email attachments.
	''' </summary>
	<XmlIgnore()>
	Property StreamAttachments As Generic.Dictionary(Of String, MemoryStream)

	''' <summary>
	''' Sends the email.
	''' </summary>
	''' <param name="bHTMLFormat">if set to <c>true</c>, is HTML email.</param>
	''' <returns></returns>
	Function SendEmail(Optional ByVal bHTMLFormat As Boolean = False) As Boolean

	''' <summary>
	''' Adds specified address to the email, if an email address is already set, will append new email to the end using ;.
	''' </summary>
	''' <param name="EmailType">Type of address to add, EmailTo, CC or BCC.</param>
	''' <param name="EmailAddress">The email address to add.</param>
	Sub AddAddress(ByVal EmailType As Email.eEmailType, ByVal EmailAddress As String)

End Interface