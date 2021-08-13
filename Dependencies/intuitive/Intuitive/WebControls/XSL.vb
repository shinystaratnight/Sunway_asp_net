Imports System.Configuration.ConfigurationSettings
Imports System.Data.SqlClient
Imports System.Xml
Imports System.Xml.Xsl
Imports System.IO
Imports System.Xml.XPath
Imports System.Web.UI
Imports System.Web
Imports Intuitive.Functions

Namespace WebControls

	Public Class XSL
		Inherits Control

		Private sBaseXML As String
		Private oXMLDocument As XmlDocument
		Private bAutoCache As Boolean = False
		Private oXSLParams As New XSLParams
		Private bForceRecompile As Boolean = False
		Public RenderDuration As Double = 0

		Public oExchangeRateTransformDef As ExchangeRateTransformDef = New ExchangeRateTransformDef

#Region "properties"

		Public Property SourceSql() As String
			Get
				If Not Me.ViewState("SourceSql") Is Nothing Then
					Return Me.ViewState("SourceSql").ToString
				Else
					Return ""
				End If
			End Get
			Set(ByVal Value As String)
				Me.ViewState("SourceSql") = Value
				Me.Clear()
			End Set
		End Property

		Public Property ConnectString() As String
			Get
				Return SafeString(Me.ViewState("ConnectString"))
			End Get
			Set(ByVal Value As String)
				Me.ViewState("ConnectString") = Value
			End Set
		End Property

		Public Property XSLTemplate() As String
			Get
				Return SafeString(Me.ViewState("XSLTemplate"))
			End Get
			Set(ByVal Value As String)
				Me.ViewState("XSLTemplate") = Value
			End Set
		End Property

		Public ReadOnly Property HasCachedData() As Boolean
			Get
				Return Not CType(HttpContext.Current.Session(Me.ID & Page.ToString), XmlDocument) Is Nothing
			End Get
		End Property

		Public ReadOnly Property Command() As String
			Get
				Return Page.Request.Form("Command")
			End Get
		End Property

		Public ReadOnly Property Argument() As String
			Get
				Return Page.Request.Form("Argument")
			End Get
		End Property

		Public Property XMLDocument() As XmlDocument
			Get
				Return oXMLDocument
			End Get
			Set(ByVal Value As XmlDocument)

				If oXMLDocument Is Nothing Then oXMLDocument = New XmlDocument

				oXMLDocument = Value
				If bAutoCache Then
					Me.StoreInCache(Value)
				End If
			End Set
		End Property

		Public Property AutoCache() As Boolean
			Get
				Return bAutoCache
			End Get
			Set(ByVal Value As Boolean)
				bAutoCache = Value
			End Set
		End Property

		Public Property XSLParameters() As XSLParams
			Get
				Return oXSLParams
			End Get
			Set(ByVal Value As XSLParams)
				oXSLParams = Value
			End Set
		End Property

		Public Property ForceRecompile() As Boolean
			Get
				Return bForceRecompile
			End Get
			Set(ByVal value As Boolean)
				bForceRecompile = value
			End Set
		End Property

#End Region

#Region "get base xml"
		Public Function GetXMLDocument(Optional ByVal bDontCache As Boolean = False) As XmlDocument

			'if special one spec'd then use that
			If Not oXMLDocument Is Nothing Then
				Return oXMLDocument
			End If

			Dim oXMLDoc As XmlDocument = Nothing
			If Not bDontCache Then
				oXMLDoc = CType(HttpContext.Current.Session(Me.ID & Page.ToString), XmlDocument)
			End If

			If oXMLDoc Is Nothing AndAlso Me.SourceSql <> "" Then

				If Me.ConnectString = "" Then
					oXMLDoc = SQL.GetXMLDoc(Me.SourceSql)
				Else
					oXMLDoc = SQL.GetXMLDocWithConnectString(Me.ConnectString, Me.SourceSql)
				End If

				If Not bDontCache Then
					Me.StoreInCache(oXMLDoc)
				End If
			End If

			Return oXMLDoc

		End Function

#End Region

#Region "StoreInCache"

		Private Sub StoreInCache(ByVal oXMLDocument As XmlDocument)
			HttpContext.Current.Session(Me.ID & Page.ToString) = oXMLDocument
		End Sub

#End Region

#Region "clear"
		Public Sub Clear()

			oXMLDocument = Nothing

			If Not Page Is Nothing AndAlso Not Context.Session(Me.ID & Page.ToString) Is Nothing Then
				Context.Session.Remove(Me.ID & Page.ToString)
			End If

		End Sub
#End Region

#Region "render"
		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
			Dim oXMLDoc As XmlDocument = Me.GetXMLDocument

			Dim dStartTime As DateTime = Date.Now

			'create the stringwriter and transform.  do the transform.
			If Not oXMLDoc Is Nothing Then
				Dim oWriter As New StringWriter

				oXMLDoc = Me.oExchangeRateTransformDef.CalculateCurrency(oXMLDoc)

				Dim sPageName As String = ""
				If Not Me.Page Is Nothing Then
					sPageName = Me.Page.ToString
				Else
					sPageName = Me.ID
				End If

				Dim oXMLTransform As XslCompiledTransform = Intuitive.XMLFunctions.CreateXSLCompiledTransform(XSLTemplate, Me.ForceRecompile,
					sPageName)
				With oXMLTransform

					If oXSLParams Is Nothing Then
						.Transform(oXMLDoc, Nothing, oWriter)
					Else

						Dim oArgs As XsltArgumentList = New XsltArgumentList
						For Each oParam As WebControls.XSL.XSLParam In oXSLParams
							oArgs.AddParam(oParam.Name, "", oParam.Value)
						Next
						.Transform(oXMLDoc, oArgs, oWriter)
					End If

				End With

				writer.Write(oWriter.ToString)
			End If

			Me.RenderDuration = Date.Now.Subtract(dStartTime).TotalMilliseconds

		End Sub

#End Region

#Region "generate"

		Public Function Generate(Optional ByVal sSql As String = "",
			Optional ByVal GenerateWithNoXMLDocument As Boolean = False) As String

			If sSql <> "" Then
				Me.SourceSql = sSql
			End If

			Dim oXMLDoc As XmlDocument = Me.GetXMLDocument(True)

			If oXMLDoc Is Nothing AndAlso GenerateWithNoXMLDocument Then
				oXMLDoc = New XmlDocument
			End If

			'create the stringwriter and transform.  do the transform.
			If Not oXMLDoc Is Nothing Then
				Dim oWriter As New StringWriter
				Dim oXMLTransform As New XslCompiledTransform

				oXMLDoc = Me.oExchangeRateTransformDef.CalculateCurrency(oXMLDoc)

				With oXMLTransform

					If Me.XSLTemplate.IndexOf(":\") > -1 OrElse Me.XSLTemplate.Substring(0, 2) = "\\" Then
						.Load(Me.XSLTemplate)
					Else
						.Load(HttpContext.Current.Request.MapPath(Me.XSLTemplate))
					End If

					If oXSLParams Is Nothing Then
						.Transform(oXMLDoc, Nothing, oWriter)
					Else

						Dim oArgs As XsltArgumentList = New XsltArgumentList
						For Each oParam As WebControls.XSL.XSLParam In oXSLParams
							oArgs.AddParam(oParam.Name, "", oParam.Value)
						Next
						.Transform(oXMLDoc, oArgs, oWriter)
					End If

				End With

				Return oWriter.ToString
			End If

			Return ""
		End Function

#End Region

#Region "Set and Get Value"
		Public Sub SetValue(ByVal sQuery As String, ByVal sValue As String)
			'get the xml doc
			Dim oXMLDoc As XmlDocument = Me.GetXMLDocument

			If Not oXMLDoc Is Nothing Then

				Dim oNode As XmlNode

				'try to get our xml node with the xpath query
				oNode = oXMLDoc.SelectSingleNode(sQuery)

				If Not oNode Is Nothing Then
					'set the value
					oNode.InnerText = sValue

					'what about attribs
					'oNode.Attributes("AttributeName").Value = sValue
				End If
			End If

		End Sub

		Public Function GetValue(ByVal sNode As String) As String

			'get the xml doc
			Dim oXMLDoc As XmlDocument = Me.GetXMLDocument

			If Not oXMLDoc Is Nothing Then
				Dim oNode As XmlNode
				oNode = oXMLDoc.SelectSingleNode(sNode)
				If Not oNode Is Nothing Then
					Return oNode.InnerText
				End If
			End If

			Return ""
		End Function

#End Region

#Region "Set Source SQL"

		Public Sub SetSourceSQL(ByVal SQL As String, ByVal ParamArray aParams() As Object)
			Me.SourceSql = Intuitive.SQL.FormatStatement(SQL, aParams)
		End Sub

		Public Sub SetSourceSQLWithConnectString(ByVal sConnectString As String, ByVal SQL As String, ByVal ParamArray aParams() As Object)
			Me.ConnectString = sConnectString
			Me.SourceSql = Intuitive.SQL.FormatStatement(SQL, aParams)
		End Sub

#End Region

#Region "Set Node"
		Public Sub SetNode(ByVal sXPath As String, ByVal sValue As String, ByVal ParamArray aParams() As Object)

			Dim oXMLDoc As XmlDocument = Me.GetXMLDocument
			oXMLDoc.SelectSingleNode(String.Format(sValue, aParams)).InnerText = sValue
		End Sub
#End Region

#Region "xsl params"

		Public Class XSLParams
			Inherits Generic.List(Of XSLParam)

			Public Sub AddParam(ByVal sParameterName As String, ByVal sValue As String)
				'Me.Add(sParameterName & "|" & sValue)
				Me.Add(New XSLParam(sParameterName, sValue))
			End Sub
			Public Sub AddParam(ByVal sParameterName As String, ByVal oValue As Object)
				Me.AddParam(sParameterName, oValue.ToString)
			End Sub
		End Class

		Public Class XSLParam
			Public Name As String
			Public Value As String

			Public Sub New()
			End Sub

			Public Sub New(ByVal Name As String, ByVal Value As String)
				Me.Name = Name
				Me.Value = SafeString(Value)
			End Sub
		End Class

#End Region

#Region "Exchange Rate Transform Def"
		Public Class ExchangeRateTransformDef

			Public ExchangeRate As Decimal = 1
			Public SellingCurrencySymbol As String = ""
			Public CurrencySymbolPosition As String = "Prepend"
			Public NumberOnly As Boolean = False
			Public XPath As Generic.List(Of String) = New Generic.List(Of String)

			Public Sub New()

			End Sub

			Public Sub New(ByVal nExchangeRate As Decimal, ByVal bNumberOnly As Boolean, ByVal oXPath As Generic.List(Of String),
			   Optional ByVal sSellingSurrencySymbol As String = "", Optional ByVal sCurrencySymbolPosition As String = "Prepend")

				Me.ExchangeRate = nExchangeRate
				Me.SellingCurrencySymbol = sSellingSurrencySymbol
				Me.CurrencySymbolPosition = sCurrencySymbolPosition
				Me.NumberOnly = bNumberOnly
				Me.XPath = oXPath

			End Sub

#Region "Calculate Currency"

			Public Function CalculateCurrency(ByVal oXML As XmlDocument) As XmlDocument

				For Each sXPath As String In Me.XPath

					Dim oNodes As XmlNodeList = oXML.SelectNodes(sXPath)
					For Each oNode As XmlNode In oNodes
						If oNode IsNot Nothing Then
							Dim sValue As String
							If Me.NumberOnly Then
								sValue = SafeString(Math.Round(oNode.InnerText.ToSafeDecimal * Me.ExchangeRate, 2))
							Else
								sValue = FormatMoney(SafeNumeric(oNode.InnerText.ToSafeDecimal * Me.ExchangeRate), Me.SellingCurrencySymbol, Me.CurrencySymbolPosition)
							End If
							oNode.InnerText = sValue
						End If
					Next

				Next

				Return oXML
			End Function

#End Region
		End Class
#End Region

	End Class
End Namespace