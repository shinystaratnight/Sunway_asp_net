Imports System.Xml
Imports Intuitive.WebControls

Public Interface IXSLEmail
	Inherits IEmail

	''' <summary>
	''' Gets or sets the XSL template.
	''' </summary>
	Property XSLTemplate As String

	''' <summary>
	''' Gets or sets the XML stored procedure.
	''' </summary>
	Property XMLStoredProcedure As String

	''' <summary>
	''' Gets or sets the XSL parameters.
	''' </summary>
	Property XSLParameters As XSL.XSLParams

	''' <summary>
	''' Gets or sets the XML document.
	''' </summary>
	Property XMLDocument As XmlDocument

	''' <summary>
	''' Sends the email.
	''' </summary>
	''' <param name="bHTMLFormat">if set to <c>true</c>, is HTML email.</param>
	''' <returns></returns>
	Overloads Function SendEmail(Optional ByVal bHTMLFormat As Boolean = False) As Boolean

	''' <summary>
	''' Sends the email.
	''' </summary>
	''' <param name="LanguageID">The language id used in translation.</param>
	''' <param name="CultureCode">The culture code used for translation.</param>
	''' <param name="IsDefaultLanguage">if set to <c>true</c> [is default language].</param>
	''' <param name="bHTMLFormat">if set to <c>true</c>, is HTML email.</param>
	''' <param name="TranslateDefaultLanguage">if set to <c>true</c>, translates to default language.</param>
	''' <returns></returns>
	Overloads Function SendEmail(LanguageID As Integer, CultureCode As String, IsDefaultLanguage As Boolean, Optional ByVal bHTMLFormat As Boolean = False, Optional ByVal TranslateDefaultLanguage As Boolean = False) As Boolean

End Interface