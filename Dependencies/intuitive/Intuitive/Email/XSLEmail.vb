Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports Intuitive.WebControls.XSL

''' <summary>
''' 
''' </summary>
Public Class XSLEmail
	Inherits Email
	Implements IXSLEmail

#Region "Fields"

	''' <summary>
	''' The XSL template used to generate the email structure
	''' </summary>
	Private sXSLTemplate As String

	''' <summary>
	''' The stored procedure to generate the xml for the email, used if oLocalXMLDocument isn't set
	''' </summary>
	Private sXMLStoredProcedure As String

	''' <summary>
	''' The XSL parameters to be passed into the template
	''' </summary>
	Private oXSLParams As New XSLParams

	''' <summary>
	''' The local XML document to be used for the template
	''' </summary>
	Private oLocalXMLDocument As XmlDocument

#End Region

#Region "Properties"

	''' <summary>
	''' Gets or sets the XSL template.
	''' </summary>
	Public Property XSLTemplate As String Implements IXSLEmail.XSLTemplate
		Get
			Return sXSLTemplate
		End Get
		Set
			sXSLTemplate = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the XML stored procedure.
	''' </summary>
	Public Property XMLStoredProcedure As String Implements IXSLEmail.XMLStoredProcedure
		Get
			Return sXMLStoredProcedure
		End Get
		Set
			sXMLStoredProcedure = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the XSL parameters.
	''' </summary>
	Public Property XSLParameters As XSLParams Implements IXSLEmail.XSLParameters
		Get
			Return oXSLParams
		End Get
		Set
			oXSLParams = Value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the XML document.
	''' </summary>
	Public Property XMLDocument As XmlDocument Implements IXSLEmail.XMLDocument
		Get
			Return oLocalXMLDocument
		End Get
		Set
			oLocalXMLDocument = Value
		End Set
	End Property

#End Region

#Region "Send"

	''' <summary>
	''' Sends the email.
	''' </summary>
	''' <param name="bHTMLFormat">if set to <c>true</c>, is HTML email.</param>
	''' <returns></returns>
	Public Overloads Function SendEmail(Optional ByVal bHTMLFormat As Boolean = False) As Boolean Implements IXSLEmail.SendEmail
		Return Me.SendEmail(0, "", True, bHTMLFormat)
	End Function

	''' <summary>
	''' Sends the email.
	''' </summary>
	''' <param name="LanguageID">The language id used in translation.</param>
	''' <param name="CultureCode">The culture code used for translation.</param>
	''' <param name="IsDefaultLanguage">if set to <c>true</c> [is default language].</param>
	''' <param name="bHTMLFormat">if set to <c>true</c>, is HTML email.</param>
	''' <param name="TranslateDefaultLanguage">if set to <c>true</c>, translates to default language.</param>
	''' <returns></returns>
	Public Overloads Function SendEmail(ByVal LanguageID As Integer, ByVal CultureCode As String, ByVal IsDefaultLanguage As Boolean, Optional ByVal bHTMLFormat As Boolean = False, Optional ByVal TranslateDefaultLanguage As Boolean = False) As Boolean Implements IXSLEmail.SendEmail

		'get the xml doc
		Dim oXMLDocument As XmlDocument
		If Me.XMLDocument Is Nothing Then
			oXMLDocument = SQL.GetXMLDoc(sXMLStoredProcedure)
		Else
			oXMLDocument = Me.XMLDocument
		End If

		'transform with the template
		Dim sbBody As New System.Text.StringBuilder
		Dim oWriter As New StringWriter(sbBody)

		Dim oXMLTransform As New XslCompiledTransform
		With oXMLTransform
			.Load(sXSLTemplate)

			If oXSLParams Is Nothing Then
				.Transform(oXMLDocument, Nothing, oWriter)
			Else

				Dim oArgs As XsltArgumentList = New XsltArgumentList
				For Each oParam As WebControls.XSL.XSLParam In oXSLParams
					oArgs.AddParam(oParam.Name, "", oParam.Value)
				Next
				.Transform(oXMLDocument, oArgs, oWriter)
			End If

		End With

		Me.Body = Multilingual.TranslatePage(sbBody.ToString, LanguageID, CultureCode, IsDefaultLanguage, TranslateDefaultLanguage, False).HTML

		Return MyBase.SendEmail(bHTMLFormat)

	End Function

#End Region

End Class