Imports System.Xml
Imports System.Xml.Xsl
Imports System.IO
Imports System.Web
Imports System.Text
Imports Intuitive.WebControls.XSL
Imports System.Text.RegularExpressions
Imports Intuitive.Functions
Imports System.Linq
Imports System.Xml.Linq
Imports System.Security.Cryptography
Imports System.Security.Cryptography.Xml
Imports System.Security.Cryptography.X509Certificates

''' <summary>
''' Class containing numerous functions to perform on XML documents
''' </summary>
Public Class XMLFunctions

	''' <summary>
	''' Gets xml from source sql, transforms it using the specified xsl template and saves it to the specified file
	''' </summary>
	''' <param name="sSourceSQL">The source SQL.</param>
	''' <param name="sXSLTemplate">The XSL template.</param>
	''' <param name="sFileName">Name of the file.</param>
	Public Shared Sub XMLToFile(sSourceSQL As String, sXSLTemplate As String, sFileName As String)

		'get the xml from the database
		Dim oXMLDoc As XmlDocument
		oXMLDoc = SQL.GetXMLDoc(sSourceSQL)

		'create the stringwriter and transform.  do the transform.
		If Not oXMLDoc Is Nothing Then

			Dim oFileStream As New StreamWriter(sFileName)
			Dim oXMLTransform As XslCompiledTransform = Intuitive.XMLFunctions.CreateXSLCompiledTransform(sXSLTemplate)

			Try
				With oXMLTransform
					.Transform(oXMLDoc, Nothing, oFileStream)
				End With

			Finally

			End Try

		End If

	End Sub

	''' <summary>
	''' Transforms xml document using specified xsl template and saves it to the specified file
	''' </summary>
	''' <param name="oXMLDoc">The XML document.</param>
	''' <param name="sXSLTemplate">The XSL template.</param>
	''' <param name="sFileName">Name of the file.</param>
	Public Shared Sub XMLToFileFromDoc(oXMLDoc As XmlDocument, sXSLTemplate As String, sFileName As String)

		'create the stringwriter and transform.  do the transform.
		If Not oXMLDoc Is Nothing Then

			Dim oFileStream As New StreamWriter(sFileName)
			Dim oXMLTransform As XslCompiledTransform = Intuitive.XMLFunctions.CreateXSLCompiledTransform(sXSLTemplate)

			Try
				With oXMLTransform
					.Transform(oXMLDoc, Nothing, oFileStream)
				End With

			Finally
				oFileStream.Close()
			End Try
		End If
	End Sub

	''' <summary>
	''' Merges all XML documents in the array of documents into one xml document with the specified root node.
	''' </summary>
	''' <param name="NodeName">Name of the root node.</param>
	''' <param name="XMLDocuments">The XML documents.</param>
	Public Shared Function MergeXMLDocuments(NodeName As String, ByVal ParamArray XMLDocuments() As XmlDocument) As XmlDocument

		Dim sb As New StringBuilder

		sb.Append("<").Append(NodeName).Append(">")
		For Each oXMLDocument As XmlDocument In XMLDocuments
			If Not oXMLDocument Is Nothing Then
				sb.Append(oXMLDocument.InnerXml.Replace("<?xml version=""1.0"" encoding=""UTF-8""?>", "").Replace("<?xml version=""1.0""?>", "").Replace("<?xml version=""1.0"" encoding=""utf-8""?>", ""))

			End If
		Next
		sb.Append("</").Append(NodeName).Append(">")

		Dim oXmlResult As New XmlDocument
		oXmlResult.LoadXml(sb.ToString)

		Return oXmlResult
	End Function

	''' <summary>
	''' Merges param array of XML documents, the first xml document will be the root, the other documents will be children of the first one.
	''' </summary>
	''' <param name="XMLDocuments">The XML documents.</param>
	Public Shared Function MergeXMLDocuments(ByVal ParamArray XMLDocuments() As XmlDocument) As XmlDocument

		'we need to take the first xml document and then bish the rest into it's root node
		If XMLDocuments.Length > 0 Then
			Dim oParentDoc As New XmlDocument

			' we have to reload the entire document otherwise we are actually changing the original...
			If XMLDocuments(0).InnerXml <> "" Then
				oParentDoc.LoadXml(XMLDocuments(0).InnerXml)
			End If

			Dim oInsertNode As XmlNode = oParentDoc.FirstChild

			'need to test whether this is the xml root node, if so use the next node.
			If oInsertNode IsNot Nothing AndAlso oInsertNode.NodeType = XmlNodeType.XmlDeclaration AndAlso oParentDoc.ChildNodes.Count > 1 Then
				oInsertNode = oParentDoc.ChildNodes(1)
			End If

			If oInsertNode IsNot Nothing Then

				For iXMLDoc As Integer = 1 To XMLDocuments.Length - 1
					Dim oExtraXMLDocument As XmlDocument = XMLDocuments(iXMLDoc)

					Dim oNode As XmlNode = oExtraXMLDocument.FirstChild

					'if the document is empty, skip it
					If oNode Is Nothing Then
						Continue For
					End If

					'need to test whether this is the xml root node, if so use the next node.
					If oNode.NodeType = XmlNodeType.XmlDeclaration Then
						oNode = oExtraXMLDocument.ChildNodes(1)
					End If

					If oNode.InnerXml <> "" Then
						oInsertNode.AppendChild(oParentDoc.ImportNode(oNode, True))
					End If
				Next
			End If

			Return oParentDoc
		Else
			Return Nothing
		End If

	End Function

	''' <summary>
	''' Merges a list of XML documents into one document with the specified nodename as the root.
	''' </summary>
	''' <param name="NodeName">Name of the root node.</param>
	''' <param name="XMLDocuments">List of XML documents to merge.</param>
	Public Shared Function MergeXMLDocuments(NodeName As String, XMLDocuments As Generic.List(Of XmlDocument)) As XmlDocument

		Dim aXMLDocuments(XMLDocuments.Count - 1) As XmlDocument

		Dim iIndex As Integer = 0
		For Each oXML As XmlDocument In XMLDocuments
			aXMLDocuments(iIndex) = oXML
			iIndex += 1
		Next

		Return MergeXMLDocuments(NodeName, aXMLDocuments)
	End Function

	'append xml documents
	''' <summary>
	''' Appends the list of xml documents to the base xml document as child nodes
	''' </summary>
	''' <param name="BaseXMLDocument">The base XML document.</param>
	''' <param name="XMLDocumentsToAppend">The XML documents to append.</param>
	Public Shared Function AppendXMLDocuments(BaseXMLDocument As XmlDocument, ByVal ParamArray XMLDocumentsToAppend() As XmlDocument) As XmlDocument

		For Each oXMLDocument As XmlDocument In XMLDocumentsToAppend

			If Not oXMLDocument.SelectSingleNode("/*") Is Nothing Then
				BaseXMLDocument.LastChild.AppendChild(BaseXMLDocument.ImportNode(oXMLDocument.SelectSingleNode("/*"), True))
			End If
		Next

		Return BaseXMLDocument

	End Function

	''' <summary>
	''' Adds a new XML node to the XML document
	''' </summary>
	''' <param name="oXMLDocument">The XML document to add the node to.</param>
	''' <param name="sParentNode">The parent node to append the node on.</param>
	''' <param name="sNewNode">The new node.</param>
	''' <param name="sNewNodeValue">The new node value.</param>
	''' <param name="sPositionRefNode">The node path of a node you want to insert next to.</param>
	''' <param name="sBeforeAfterPositionRefNode">Whether to insert the node before or after the PositionRefNode.</param>
	Public Shared Sub AddXMLNode(oXMLDocument As XmlDocument, sParentNode As String, sNewNode As String,
								 sNewNodeValue As String, Optional ByVal sPositionRefNode As String = "",
								 Optional ByVal sBeforeAfterPositionRefNode As String = "")

		'Check XML DOcument
		If Not oXMLDocument Is Nothing Then
			Dim XMLRootNode As System.Xml.XmlNode = oXMLDocument.SelectSingleNode("/" + sParentNode)

			'Check For Root Node
			If Not XMLRootNode Is Nothing Then
				Dim XMLNewNode As XmlNode = oXMLDocument.CreateNode(XmlNodeType.Element, sNewNode, "")
				XMLNewNode.InnerText = sNewNodeValue

				'Check For Reference Node- If targeted Insert
				If sPositionRefNode <> "" Then
					Dim XMLPositionRefNode As System.Xml.XmlNode = XMLRootNode.SelectSingleNode(sPositionRefNode)
					'Appends At Target Node of XMLDocument
					If sBeforeAfterPositionRefNode = "After" Then XMLRootNode.InsertAfter(XMLNewNode, XMLPositionRefNode)
					If sBeforeAfterPositionRefNode = "Before" Then XMLRootNode.InsertBefore(XMLNewNode, XMLPositionRefNode)
				Else
					'Appends At Bottom of XMLDocument
					XMLRootNode.AppendChild(XMLNewNode)
				End If
			End If
		End If
	End Sub

	''' <summary>
	''' Formats specified XML using indents.
	''' </summary>
	''' <param name="sXML">The XML to format.</param>
	Public Shared Function FormatXML(sXML As String) As String

		'load the string into an xml doc
		Dim oXML As New XmlDocument
		oXML.LoadXml(sXML)

		'create memoer and writer objects, set formatting options
		Dim oMemoryStream As New System.IO.MemoryStream
		Dim oWriter As New XmlTextWriter(oMemoryStream, Encoding.Unicode)
		oWriter.Formatting = Formatting.Indented
		oWriter.Indentation = 4

		'flush content out and return formatted xml
		oXML.WriteContentTo(oWriter)
		oWriter.Flush()
		oMemoryStream.Flush()
		oMemoryStream.Position = 0
		Dim oReader As New StreamReader(oMemoryStream)
		Return oReader.ReadToEnd

	End Function

	''' <summary>
	''' Checks theat the node specified in the xpath exists then returns its value as a string.
	''' </summary>
	''' <param name="oNode">The node to retrieve the subnode value from.</param>
	''' <param name="sXPath">The xpath of the node to retrieve the value of.</param>
	Public Shared Function SafeNodeValue(oNode As XmlNode, sXPath As String) As String
		If Not oNode.SelectSingleNode(sXPath) Is Nothing Then
			Return oNode.SelectSingleNode(sXPath).InnerText
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Checks that the node at the specified xpath exists and returns the nodes outer xml.
	''' </summary>
	''' <param name="oXML">The XML document to get the outer xml from.</param>
	''' <param name="sXPath">The xpath of the node to get the outer xml of.</param>
	Public Shared Function SafeOuterXML(oXML As XmlDocument, sXPath As String) As String
		If Not oXML.SelectSingleNode(sXPath) Is Nothing Then
			Return oXML.SelectSingleNode(sXPath).OuterXml
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Checks that the node specified in the xpath exists and removes it from the xml document.
	''' </summary>
	''' <param name="oXML">The XML document to remove the node from.</param>
	''' <param name="sXPath">The xpath of the node to remove.</param>
	Public Shared Sub SafeRemoveNode(ByRef oXML As XmlDocument, sXPath As String)
		Dim oNode As System.Xml.XmlNode = oXML.SelectSingleNode(sXPath)

		If Not oNode Is Nothing Then oNode.ParentNode.RemoveChild(oNode)
	End Sub

	''' <summary>
	''' Cleans SOAP XML by stripping out the body tags and removing the xml namespaces.
	''' </summary>
	''' <param name="sSOAPXML">The SOAP xml to clean.</param>
	Public Shared Function CleanSOAPDocument(sSOAPXML As String) As String

		'soap 1.1?
		If sSOAPXML.IndexOf("<soap:Body>") > -1 Then
			sSOAPXML = Functions.RetrieveText(sSOAPXML, "<soap:Body>", "</soap:Body>").Replace("<soap:Body>", "").Replace("</soap:Body>", "")
		End If

		'soap 1.2?
		If sSOAPXML.IndexOf("<soap12:Body>") > -1 Then
			sSOAPXML = Functions.RetrieveText(sSOAPXML, "<soap12:Body>", "</soap12:Body>").Replace("<soap12:Body>", "").Replace("</soap12:Body>", "")
		End If

		'xmlns
		sSOAPXML = CleanXMLNamespaces(sSOAPXML).InnerXml

		Return sSOAPXML

	End Function

	''' <summary>
	''' Strips out XML namespaces.
	''' </summary>
	''' <param name="sXML">The XML.</param>
	<Obsolete("Use CleanXMLNamespaces(sXML).InnerXml instead")>
	Public Shared Function StripXMLNamespaces(sXML As String) As String
		Return CleanXMLNamespaces(sXML).InnerXml
	End Function

	''' <summary>
	''' Cleans namespaces from SOAP xml and returns the soap body.
	''' </summary>
	''' <param name="oXML">The XML.</param>
	Public Shared Function SafeSOAPToXML(oXML As XmlDocument) As XmlDocument
		Return SafeSOAPToXML(oXML.OuterXml)
	End Function

	''' <summary>
	''' Cleans namespaces from SOAP xml and returns the soap body.
	''' </summary>
	''' <param name="sXML">The XML.</param>
	Public Shared Function SafeSOAPToXML(sXML As String) As XmlDocument

		Dim oXML As New XmlDocument

		Try

			oXML.LoadXml(sXML)
			oXML = CleanXMLNamespaces(oXML)

			If oXML.SelectSingleNode("Envelope/Body") IsNot Nothing Then
				oXML.LoadXml(oXML.SelectSingleNode("Envelope/Body").InnerXml)
			End If

			If oXML.SelectSingleNode("envelope/Body") IsNot Nothing Then
				oXML.LoadXml(oXML.SelectSingleNode("envelope/Body").InnerXml)
			End If

			If oXML.SelectSingleNode("Envelope/body") IsNot Nothing Then
				oXML.LoadXml(oXML.SelectSingleNode("Envelope/body").InnerXml)
			End If

			If oXML.SelectSingleNode("envelope/body") IsNot Nothing Then
				oXML.LoadXml(oXML.SelectSingleNode("envelope/body").InnerXml)
			End If

		Catch ex As Exception

		End Try

		Return oXML

	End Function

	''' <summary>
	''' Takes a SOAP xml string and removes namepaces, body, envelope and header tags.
	''' </summary>
	''' <param name="sXML">The SOAP XML string to convert.</param>
	Public Shared Function SOAPToXML(sXML As String) As XmlDocument
		Dim oXML As New XmlDocument
		Try
			oXML.LoadXml(sXML)
		Catch ex As Exception
			Return Nothing
		End Try

		Return SafeSOAPToXML(oXML)
	End Function

	''' <summary>
	''' Takes a SOAP xml document and removes namepaces, body, envelope and header tags.
	''' </summary>
	''' <param name="oXML">The SOAP XML document to convert.</param>
	<Obsolete("Use SafeSOAPToXML instead")>
	Public Shared Function SOAPToXML(oXML As XmlDocument) As XmlDocument
		Return SafeSOAPToXML(oXML)
	End Function

	''' <summary>
	''' Creates SOAP XML document from specified xml document, places the content of the xml document in the body of the SOAP document
	''' </summary>
	''' <param name="oXml">The XML to convert.</param>
	''' <param name="soapEnvelopeNamespace">The SOAP envelope namespace.</param>
	''' <returns>SOAP envelope document.</returns>
	Public Shared Function XMLToSOAP(oXml As XmlDocument, Optional soapEnvelopeNamespace As String = SOAPEnvelopeNamespace.W3Org200112) As XmlDocument

		Dim sb As New StringBuilder

		sb.AppendFormat("<?xml version=""1.0""?><soap:Envelope xmlns:soap=""{0}"" soap:encodingStyle=""{0}""><soap:Body>", soapEnvelopeNamespace)
		sb.Append(RemoveProlog(oXml).OuterXml.ToString)
		sb.Append("</soap:Body></soap:Envelope>")

		oXml.LoadXml(sb.ToString)
		Return oXml
	End Function

	''' <summary>
	''' Removes the prolog.
	''' </summary>
	''' <param name="sXML">The XML.</param>
	Public Shared Function RemoveProlog(sXML As String) As XmlDocument
		Dim oXML As New XmlDocument
		Try
			oXML.LoadXml(sXML)
		Catch ex As Exception
			Return Nothing
		End Try

		Return RemoveProlog(oXML)
	End Function

	''' <summary>
	''' Removes the prolog from the XML document using regex.
	''' </summary>
	''' <param name="oXML">The XML document.</param>
	Public Shared Function RemoveProlog(oXML As XmlDocument) As XmlDocument

		Dim oRegex As New Generic.List(Of String)
		oRegex.Add("<[?]xml[\s]*([a-zA-Z0-9_]+=""[a-zA-Z0-9_\\.-]*""[\s]*)*[?]>")

		Dim sXML As String = oXML.InnerXml
		For Each sRegex As String In oRegex
			sXML = Intuitive.Functions.RegExp.Replace(sXML, sRegex)
		Next

		oXML.LoadXml(sXML)
		Return oXML

	End Function

	''' <summary>
	''' Converts a datatable to an XML document, created a node for each datarow in the datatable.
	''' </summary>
	''' <param name="dt">The datatable.</param>
	''' <param name="sPath">The desired node path to create for each datarow.</param>
	''' <param name="sRoot">The desired root node.</param>
	''' <param name="iColumnsToUse">The number of columns to use.</param>
	Public Shared Function DatatableToXML(dt As DataTable, sPath As String, Optional ByVal sRoot As String = "",
			Optional ByVal iColumnsToUse As Integer = 0) As XmlDocument

		Dim xml As New Xml.XMLBuilder()
		If iColumnsToUse = 0 Then iColumnsToUse = dt.Columns.Count

		'build the xml
		If sRoot <> "" Then xml.StartNode(sRoot)

		For Each dr As DataRow In dt.Rows
			xml.StartNode(sPath)
			For iLoop As Integer = 0 To iColumnsToUse - 1
				xml.AddElement(dt.Columns(iLoop).ColumnName, dr(iLoop))
			Next
			xml.EndNode(sPath)
		Next

		If sRoot <> "" Then xml.EndNode(sRoot)

		'return as an xml document
		Dim oXML As New XmlDocument
		oXML.LoadXml(xml.ToString)

		Return oXML

	End Function

#Region "xmltransform"

	''' <summary>
	''' Transforms an xml document using the specified xsl template and parameters.
	''' </summary>
	''' <param name="oXML">The XML to transform.</param>
	''' <param name="XSLTemplate">The XSL template.</param>
	''' <param name="oXSLParams">The XSL parameters.</param>
	''' <param name="oExchangeRateDef">The exchange rate definition.</param>
	Public Shared Function XMLTransform(oXML As XmlDocument, XSLTemplate As String,
	  Optional ByVal oXSLParams As XSLParams = Nothing, Optional ByVal oExchangeRateDef As ExchangeRateTransformDef = Nothing) As XmlDocument

		If Not oXML Is Nothing Then
			oXML.LoadXml(XMLTransformToString(oXML, XSLTemplate, oXSLParams, True, oExchangeRateDef))
		End If

		Return oXML

	End Function

	''' <summary>
	''' Transforms XML document using specified XSL template and parameters. 
	''' Returns it as a string.
	''' </summary>
	''' <param name="oXML">The XML to transform.</param>
	''' <param name="XSLTemplate">The XSL template.</param>
	''' <param name="oXSLParams">The XSL parameters.</param>
	''' <param name="ForceRecompile">if set to <c>true</c>, won't try to retrieve the transformation from the cache, will always perform the translation.</param>
	''' <param name="oExchangeRateDef">The exchange rate definition.</param>
	Public Shared Function XMLTransformToString(oXML As XmlDocument, XSLTemplate As String,
	  Optional ByVal oXSLParams As XSLParams = Nothing, Optional ByVal ForceRecompile As Boolean = False,
	  Optional ByVal oExchangeRateDef As ExchangeRateTransformDef = Nothing) As String

		If oExchangeRateDef Is Nothing Then oExchangeRateDef = New ExchangeRateTransformDef

		'create the stringwriter and transform.  do the transform.
		If Not oXML Is Nothing Then
			Dim oWriter As New StringWriter

			oXML = oExchangeRateDef.CalculateCurrency(oXML)

			Dim oXMLTransform As XslCompiledTransform = Intuitive.XMLFunctions.CreateXSLCompiledTransform(XSLTemplate, ForceRecompile)

			With oXMLTransform

				If oXSLParams Is Nothing Then
					.Transform(oXML, Nothing, oWriter)
				Else

					Dim oArgs As XsltArgumentList = New XsltArgumentList
					For Each oParam As WebControls.XSL.XSLParam In oXSLParams
						oArgs.AddParam(oParam.Name, "", oParam.Value)
					Next
					.Transform(oXML, oArgs, oWriter)
				End If
			End With

			Return oWriter.ToString
		End If

		Return ""
	End Function

	''' <summary>
	''' Transforms XML document using an xsl transformation created from the specified string.
	''' </summary>
	''' <param name="oXMLDocument">The XML document.</param>
	''' <param name="sXSL">The XSL string that will be used to create the XSL transformation.</param>
	''' <param name="oXSLParams">The XSL parameters.</param>
	''' <param name="oExchangeRateDef">The exchange rate definition.</param>
	Public Shared Function XMLStringTransform(oXMLDocument As XmlDocument, sXSL As String,
	 Optional ByVal oXSLParams As XSLParams = Nothing, Optional ByVal oExchangeRateDef As ExchangeRateTransformDef = Nothing) As XmlDocument

		Dim oXML As New XmlDocument
		oXML.LoadXml(XMLStringTransformToString(oXMLDocument, sXSL, oXSLParams, oExchangeRateDef))

		Return oXML

	End Function

	''' <summary>
	''' Transforms XML document using XSL transform generated from the specified string. 
	''' Returns the XML as a string
	''' </summary>
	''' <param name="oXMLDocument">The XML document to transform.</param>
	''' <param name="sXSL">The XSL string that will be used to create the XSL transform.</param>
	''' <param name="oXSLParams">The XSL parameters.</param>
	''' <param name="oExchangeRateDef">The exchange rate definition.</param>
	Public Shared Function XMLStringTransformToString(oXMLDocument As XmlDocument, sXSL As String,
	 Optional ByVal oXSLParams As XSLParams = Nothing, Optional ByVal oExchangeRateDef As ExchangeRateTransformDef = Nothing) As String

		If oExchangeRateDef Is Nothing Then oExchangeRateDef = New ExchangeRateTransformDef

		Dim oWriter As New StringWriter
		Dim oReader As New StringReader(sXSL)
		Dim oXMLReader As XmlReader = XmlReader.Create(oReader)

		Dim oXMLTransform As XslCompiledTransform = XMLFunctions.CreateXSLTransformFromString(sXSL)

		oXMLDocument = oExchangeRateDef.CalculateCurrency(oXMLDocument)

		If oXSLParams Is Nothing Then
			oXMLTransform.Transform(oXMLDocument, Nothing, oWriter)
		Else

			Dim oArgs As XsltArgumentList = New XsltArgumentList
			For Each oParam As XSLParam In oXSLParams
				oArgs.AddParam(oParam.Name, "", oParam.Value)
			Next
			oXMLTransform.Transform(oXMLDocument, oArgs, oWriter)
		End If

		Return oWriter.ToString

	End Function

#End Region

#Region "xmldocument to formatted string"

	''' <summary>
	''' Converts an XML document to a string
	''' </summary>
	''' <param name="XMLDocument">The XML document.</param>
	Public Shared Function XMLDocumentToFormattedString(XMLDocument As XmlDocument) As String

		Dim sReturn As String = ""

		If Not XMLDocument Is Nothing Then

			'get xml doc into stream
			Dim oStream As New System.IO.MemoryStream
			XMLDocument.Save(oStream)
			oStream.Position = 0
			oStream.Flush()

			'return into return variable
			Dim oReader As New System.IO.StreamReader(oStream)
			sReturn = oReader.ReadToEnd()

			'tidy up
			oReader.Close()
			oStream.Close()
		End If

		Return sReturn

	End Function

#End Region

#Region "xsltransform stuff"

	''' <summary>
	''' Creates an <see cref="XslCompiledTransform"/> stores it in the cache
	''' </summary>
	''' <param name="XSLTemplate">The template to create the XslCompiledTransform from..</param>
	''' <param name="ForceRecompile">if set to <c>true</c>, will always recreate the XslCompiledTransform.</param>
	''' <param name="PageName">Name of the page.</param>
	Public Shared Function CreateXSLCompiledTransform(XSLTemplate As String,
		Optional ByVal ForceRecompile As Boolean = False, Optional ByVal PageName As String = "") As XslCompiledTransform

		'if no cache then create new and return
		If (HttpContext.Current Is Nothing) OrElse (HttpContext.Current.Session Is Nothing) Then
			Return CreateXSLTransform(XSLTemplate)
		End If

		Dim sCacheName As String = GetXSLTemplateCacheName(XSLTemplate, PageName)
		Dim oXMLTransform As XslCompiledTransform = GetCache(Of XslCompiledTransform)(sCacheName)

		If oXMLTransform Is Nothing OrElse ForceRecompile Then
			oXMLTransform = CreateXSLTransform(XSLTemplate)
			AddToCache(sCacheName, oXMLTransform)
		End If

		Return oXMLTransform

	End Function

	''' <summary>
	''' Creates an XSL transform from an XSL template string, saves it on the cache if possible.
	''' </summary>
	''' <param name="XSLTemplate">The XSL template.</param>
	Public Shared Function CreateXSLTransform(XSLTemplate As String) As XslCompiledTransform

		Dim oXMLTransform As New XslCompiledTransform(Functions.IsDebugging)

		If XSLTemplate.IndexOf(":\") > -1 OrElse XSLTemplate.Substring(0, 2) = "\\" Then
			oXMLTransform.Load(XSLTemplate)
		Else
			If XSLTemplate.StartsWith("./") Then
				XSLTemplate = XSLTemplate.Substring(1)
			End If

			Dim sXSLPath As String
			If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Server IsNot Nothing Then
				sXSLPath = HttpContext.Current.Server.MapPath(XSLTemplate)
			Else
				sXSLPath = System.Web.Hosting.HostingEnvironment.MapPath(XSLTemplate)
			End If
			oXMLTransform.Load(sXSLPath)
		End If

		Return oXMLTransform
	End Function

	''' <summary>
	''' Creates an XSL transform from a string, adds it to the cache if possible.
	''' </summary>
	''' <param name="XSL">The XSL string to create the Compiled Transform with.</param>
	Public Shared Function CreateXSLTransformFromString(XSL As String) As XslCompiledTransform

		Dim oXMLTransform As XslCompiledTransform = Nothing

		'if no cache then create new and return
		If HttpRuntime.Cache Is Nothing OrElse Functions.IsDebugging Then

			Dim oReader As New StringReader(XSL)
			Dim oXMLReader As XmlReader = XmlReader.Create(oReader)

			oXMLTransform = New XslCompiledTransform(Functions.IsDebugging)

			oXMLTransform.Load(oXMLReader)

		Else

			Dim sCacheName As String = GetXSLTemplateCacheName(XSL, "", False)
			oXMLTransform = GetCache(Of XslCompiledTransform)(sCacheName)

			If oXMLTransform Is Nothing Then
				Dim oReader As New StringReader(XSL)
				Dim oXMLReader As XmlReader = XmlReader.Create(oReader)
				oXMLTransform = New XslCompiledTransform
				oXMLTransform.Load(oXMLReader)
				AddToCache(sCacheName, oXMLTransform)
			End If

		End If

		Return oXMLTransform

	End Function

	''' <summary>
	''' Gets the name of the XSL template cache.
	''' </summary>
	''' <param name="XSLTemplate">The XSL template.</param>
	''' <param name="PageName">Name of the page.</param>
	''' <param name="IsXSLFile">Specifies whether the XSL template is a file.</param>
	Private Shared Function GetXSLTemplateCacheName(XSLTemplate As String, PageName As String, Optional ByVal IsXSLFile As Boolean = True) As String

		'append hashcode for template and pagename
		Dim sCacheName As String = "xsltransformcache_" & XSLTemplate.GetHashCode.ToString
		If PageName <> "" Then
			sCacheName += "_" & PageName.GetHashCode.ToString
		End If

		'last off stick a timestamp on, should help us preventing having to clear the cache manually, although clearly won't work if we change a sub file
		If IsXSLFile Then

			Dim oFile As FileInfo
			If XSLTemplate.IndexOf(":\") > -1 OrElse XSLTemplate.Substring(0, 2) = "\\" Then
				oFile = New FileInfo(XSLTemplate)
			Else
				oFile = New FileInfo(HttpContext.Current.Request.MapPath(XSLTemplate))
			End If

			sCacheName += "_" & oFile.LastWriteTime.ToString("MMddHHmmss")

		End If

		Return sCacheName

	End Function

	''' <summary>
	''' Converts string to HTML Encoded string
	''' </summary>
	''' <param name="sString">The s string.</param>
	Public Shared Function CleanStringForXML(sString As String) As String
		Return HttpUtility.HtmlEncode(sString)
	End Function

	''' <summary>
	''' Takes a filepatch to an xsl file and returns the absolute path for it.
	''' </summary>
	''' <param name="XSLFilePath">The XSL file path.</param>
	''' <param name="XSL">The XSL.</param>
	Public Shared Function XSLStringRelativeToAbsolutePaths(XSLFilePath As String, Optional ByVal XSL As String = "") As String

		'This function takes the fully qualified path to an xsl file & the XSL itself and returns a string of the XSL
		'All file import paths will be replaced with absolute file paths
		'If the optional XSL parameter is passed in we will use that rather than loading the file
		Dim sXSL As String = IIf(XSL = "", FileFunctions.FileToText(XSLFilePath), XSL)
		Dim sXSLFileDirectory As String = XSLFilePath.Replace(New FileInfo(XSLFilePath).Name, "")
		Dim aFolders As String() = Chop(sXSLFileDirectory).Split("\"c)
		For Each oMatch As Match In Regex.Matches(sXSL, "\<xsl:import\shref=""(?<Path>[^""]+)""")

			'count back (occurences of ../)
			Dim sRelativePath As String = oMatch.Groups(1).Value
			Dim sAbsolutePath As String = ""
			Dim iCountBack As Integer = sRelativePath.CountOccurances("../")

			'build up absolute path
			For i As Integer = 0 To aFolders.Length - (iCountBack + 1)
				sAbsolutePath += aFolders(i) & "\"
			Next

			sAbsolutePath += sRelativePath.Replace("../", "").Replace("/", "\")
			sXSL = sXSL.Replace(oMatch.Groups(1).Value, sAbsolutePath)
		Next

		Return sXSL

	End Function

#End Region

#Region "Get Tag Value"

	''' <summary>
	''' Returns the first instance of an xml tag within a string
	''' </summary>
	''' <param name="XML">The XML string.</param>
	''' <param name="TagName">Name of the tag to get the value of.</param>
	Public Shared Function GetTagValue(XML As String, TagName As String) As String

		'returns the first instance of an xml tag within a string
		Dim iTagStartIndex As Integer = XML.IndexOf("<" & TagName & ">") + ("<" & TagName & ">").Length
		Dim iTagEndIndex As Integer = XML.IndexOf("</" & TagName & ">")

		Return XML.Substring(iTagStartIndex, iTagEndIndex - iTagStartIndex)

	End Function

#End Region

#Region "RemoveSOAPHeader"

	''' <summary>
	''' Removes the SOAP header from a SOAP XML Document.
	''' </summary>
	''' <param name="oXML">The XML to remove the SOAP header from.</param>
	Public Shared Function RemoveSOAPHeader(oXML As XmlDocument) As XmlDocument

		Dim oRegex As New Generic.List(Of String)

		'namespace definition
		' xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
		' xmlns:SOAP-ENC="http://schemas.xmlsoap.org/soap/encoding/"
		oRegex.Add("\sxmlns(?:.*?)?=\"".*?\""")

		'xsi:type="ResponseMacroRegion"
		'oRegex.Add("\s[a-zA-Z0-9-]*(?:.*?)?=\"".*?\""")
		oRegex.Add("\s\w+(:type?)=\""[^\""]*?\""")

		'namespace
		'ns2:
		oRegex.Add("(?<=<[/]?)([a-zA-Z0-9-]*:)")

		'envelope tag
		'<soap:Envelope>
		oRegex.Add("<[/]?Envelope[\s]*([a-zA-Z0-9_]+=""[a-zA-Z0-9_]*"")?>")

		'body tag
		'<soap:Body>
		'<SOAP-ENV:Body id="_0">
		oRegex.Add("<[/]?Body[\s]*([a-zA-Z0-9_]+=""[a-zA-Z0-9_]*"")?>")

		'envelope tag
		'<soap:Envelope>
		oRegex.Add("<Header>.*</Header>")
		'type tag

		Dim sXML As String = oXML.InnerXml
		For Each sRegex As String In oRegex
			sXML = Intuitive.Functions.RegExp.Replace(sXML, sRegex)
		Next

		oXML.LoadXml(sXML)
		Return oXML

	End Function

#End Region

#Region "Safe XML Text"

	''' <summary>
	''' Removes characters that aren't allowed in XML
	''' </summary>
	''' <param name="XMLText">The XML text.</param>
	Public Shared Function SafeXMLText(XMLText As String) As String

		'strips out chars not allowed in xml and encodes
		'will break if this exercise has already been done
		Return XMLText.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")

	End Function

#End Region

#Region "XMlDocument Wrapper"

	''' <summary>
	''' Class containing functions that can be performed on an XML Document
	''' </summary>
	Public Class XMLDocumentWrapper
		''' <summary>
		''' The XML Document to perform processes on
		''' </summary>
		Public Property XmlDocument As New XmlDocument
		Public Property NameSpaceManager As XmlNamespaceManager
		Public Property DefaultPrefix As String = ""

		Public Sub New(oXMl As XmlDocument)
			Me.XmlDocument = oXMl
			Me.NameSpaceManager = GenerateNameSpaceManager(Me.XmlDocument)
		End Sub

		''' <summary>
		''' Generates a namespace manager based on the XML document.
		''' </summary>
		''' <param name="oXML">The XML.</param>
		Public Function GenerateNameSpaceManager(oXML As XmlDocument) As XmlNamespaceManager

			Try
				Dim NameSpaceManager As New XmlNamespaceManager(oXML.NameTable)

				Dim RegX As New System.Text.RegularExpressions.Regex("xmlns(.*?)=("".*?"")")
				Dim Matches As System.Text.RegularExpressions.MatchCollection
				Matches = RegX.Matches(oXML.InnerXml)

				For Each Match As System.Text.RegularExpressions.Match In Matches

					Dim sNamspace As String = Match.ToString.Replace("xmlns:", "")
					Dim sPreFix As String = sNamspace.Split("="c)(0).Replace("xmlns", "")
					Dim sURI As String = sNamspace.Split("="c)(1)
					sURI = sURI.Replace("""", "")

					If sPreFix = "" Then
						If Me.DefaultPrefix <> "" Then
							sPreFix = Me.DefaultPrefix
						Else
							sPreFix = "DefaultPrefix"
							Me.DefaultPrefix = "DefaultPrefix"
						End If
					End If

					NameSpaceManager.AddNamespace(sPreFix, sURI)

				Next

				Return NameSpaceManager

			Catch ex As Exception
				Return Nothing
			End Try

		End Function

		''' <summary>
		''' Returns the xml node at the specified xpath.
		''' </summary>
		''' <param name="XPath">The xpath.</param>
		Public Function SelectSingleNode(XPath As String) As XmlNode
			Try

				If Me.DefaultPrefix <> "" Then
					XPath = AddDefaultPrefixToXPath(XPath)
				End If

				Return Me.XmlDocument.SelectSingleNode(XPath, Me.NameSpaceManager)
			Catch ex As Exception
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Selects the nodes at the specified xpath.
		''' </summary>
		''' <param name="XPath">The xpath.</param>
		Public Function SelectNodes(XPath As String) As XmlNodeList

			Try

				If Me.DefaultPrefix <> "" Then
					XPath = AddDefaultPrefixToXPath(XPath)
				End If

				Return Me.XmlDocument.SelectNodes(XPath, Me.NameSpaceManager)

			Catch ex As Exception
				Return Nothing
			End Try

		End Function

		''' <summary>
		''' Adds the default prefix to the specified xpath.
		''' </summary>
		''' <param name="XPath">The xpath.</param>
		Private Function AddDefaultPrefixToXPath(XPath As String) As String

			Dim sTransformedXPath As String = "/"
			Try

				Dim sNodes As String() = XPath.Split("/"c)

				Dim bNextNameSpaceReached As Boolean = False
				For Each sNode As String In sNodes

					If Not sNode = "" Then
						If Not bNextNameSpaceReached AndAlso Not sNode.Contains(":") Then
							sNode = Me.DefaultPrefix & ":" & sNode
						ElseIf Not bNextNameSpaceReached AndAlso sNode.Contains(":") Then
							bNextNameSpaceReached = True
						End If

						sTransformedXPath = sTransformedXPath & sNode & "/"
					End If
				Next

				sTransformedXPath = sTransformedXPath.Substring(0, sTransformedXPath.Length - 1)

			Catch ex As Exception
				Return XPath
			End Try

			Return sTransformedXPath

		End Function

	End Class

#End Region

#Region "LINQ to XML"

	''' <summary>
	''' Creates a new <see cref="XAttribute"/> with the specified name and value
	''' </summary>
	''' <param name="Name">The name.</param>
	''' <param name="Value">The value.</param>
	Public Shared Function NewOptionalXAttribute(Name As XName, Value As Object) As XAttribute
		If Value IsNot Nothing Then
			Return New XAttribute(Name, Value)
		Else
			Return Nothing
		End If
	End Function

	''' <summary>
	''' Creates a new <see cref="XAttribute"/> with the specified name and value if the specified condition evaluates to True.
	''' </summary>
	''' <param name="Name">The name.</param>
	''' <param name="Condition">The condition.</param>
	''' <param name="Value">The value.</param>
	Public Shared Function NewOptionalXAttribute(Name As XName, Condition As Func(Of Boolean), Value As Object) As XAttribute
		If Condition() Then
			Return New XAttribute(Name, Value)
		Else
			Return Nothing
		End If
	End Function

	''' <summary>
	''' Creates a new <see cref="XElement"/> the specified name and value
	''' </summary>
	''' <param name="Name">The name.</param>
	''' <param name="Value">The value.</param>
	Public Shared Function NewOptionalXElement(Name As XName, Value As Object) As XElement
		If Value IsNot Nothing Then
			Return New XElement(Name, Value)
		Else
			Return Nothing
		End If
	End Function

	''' <summary>
	''' Creates a new <see cref="XElement"/> with the specified name and value if the specified condition evaluates to True.
	''' </summary>
	''' <param name="Name">The name.</param>
	''' <param name="Condition">The condition.</param>
	''' <param name="Value">The value.</param>
	Public Shared Function NewOptionalXElement(Name As XName, Condition As Func(Of Boolean), ByVal ParamArray Value As Object()) As XElement
		If Condition() Then
			Return New XElement(Name, Value)
		Else
			Return Nothing
		End If
	End Function

#End Region

#Region "Remove Namespaces"

	''' <summary>
	''' Removes all namespaces from the XML document.
	''' </summary>
	''' <param name="XML">The XML.</param>
	Public Shared Function CleanXMLNamespaces(XML As XmlDocument) As XmlDocument
		Return CleanXMLNamespaces(XML.OuterXml)
	End Function

	''' <summary>
	''' Removes all namespaces from the XML document.
	''' </summary>
	''' <param name="OuterXML">The XML string.</param>
	Public Shared Function CleanXMLNamespaces(OuterXML As String) As XmlDocument

		Dim xmlReturn As New XmlDocument

		Dim xmlDocumentNoNS As XElement = RemoveAllNamespaces(XElement.Parse(OuterXML))
		xmlReturn.LoadXml(xmlDocumentNoNS.ToString)

		Return xmlReturn

	End Function

	''' <summary>
	''' Removes all namespaces from the xml element.
	''' </summary>
	''' <param name="xmlDocument">The XML element.</param>
	Public Shared Function RemoveAllNamespaces(xmlDocument As XElement) As XElement

		Dim xAttributes As New Generic.List(Of XAttribute)
		For Each attribute As XAttribute In xmlDocument.Attributes()

			If Not attribute.IsNamespaceDeclaration Then
				Dim xAttribute As New XAttribute(attribute.Name.LocalName, attribute.Value)
				xAttributes.Add(xAttribute)
			End If
		Next

		If xmlDocument.HasElements Then
			Return New XElement(xmlDocument.Name.LocalName, xAttributes, xmlDocument.Elements.Select(Function(el) RemoveAllNamespaces(el)))
		Else
			Return New XElement(xmlDocument.Name.LocalName, xAttributes, xmlDocument.Value)
		End If

	End Function

#End Region

#Region "Certificate Functions"

#Region "Sign"

	''' <summary>
	''' Adds a signed xml node to the xml document
	''' </summary>
	''' <param name="XML">The XML.</param>
	''' <param name="CertificateFriendlyName">Friendly name of the certificate.</param>
	''' <param name="XPathForXMLToSign">The xpath for the node to add the signature to.</param>
	''' <param name="CanonicalizationMethod">The canonicalization method.</param>
	''' <param name="ReferenceURI">The reference URI.</param>
	''' <exception cref="System.Exception"></exception>
	Public Shared Function SignXMLWithCertificate(ByRef XML As XmlDocument, CertificateFriendlyName As String,
  XPathForXMLToSign As String, Optional ByVal CanonicalizationMethod As String = "",
   Optional ByVal ReferenceURI As String = "") As XmlDocument

		'Get the certificate that will be used for signing
		Dim oCertificate As X509Certificate2 = Security.Functions.GetCertificate(CertificateFriendlyName)

		'Check we got something back
		If oCertificate Is Nothing Then Throw New Exception(String.Format("Certificate '{0}' not found", CertificateFriendlyName))

		Return SignXMLWithCertificate(XML, oCertificate, XPathForXMLToSign, CanonicalizationMethod, ReferenceURI)

	End Function

	''' <summary>
	''' Adds a signed xml node to the xml document
	''' </summary>
	''' <param name="XML">The XML.</param>
	''' <param name="Certificate">The certificate.</param>
	''' <param name="XPathForSignature">The xpath for the node to add the signature to.</param>
	''' <param name="CanonicalizationMethod">The canonicalization method.</param>
	''' <param name="ReferenceURI">The reference URI.</param>
	''' <exception cref="System.Exception">No node found to add signature to. Check XPath is valid</exception>
	Public Shared Function SignXMLWithCertificate(ByRef XML As XmlDocument, Certificate As X509Certificate2,
	  XPathForSignature As String, Optional ByVal CanonicalizationMethod As String = "",
	  Optional ByVal ReferenceURI As String = "") As XmlDocument

		'Check that the node to sign is present before starting to sign
		Dim oNodeForSignature As XmlNode = XML.SelectSingleNode(XPathForSignature)
		If oNodeForSignature Is Nothing Then Throw New Exception("No node found to add signature to. Check XPath is valid")

		'Create a signed XML object
		Dim oSignedXML As New SignedXml(XML)

		'Set up the RSA key using the certificate private key
		Dim oRSAKey As RSACryptoServiceProvider = DirectCast(Certificate.PrivateKey, RSACryptoServiceProvider)
		oSignedXML.SigningKey = oRSAKey

		'Load the certificate into a key info data object and add the certificate subject - this provides information on the certificate used in signing
		Dim oKeyInfoData As New KeyInfoX509Data(Certificate)
		oKeyInfoData.AddSubjectName(Certificate.Subject)

		'Load the key info data into the key info and add this to the signed xml document
		Dim oKeyInfo As New KeyInfo()
		oKeyInfo.AddClause(oKeyInfoData)
		oSignedXML.KeyInfo = oKeyInfo

		'Set up the canonicalization method if provided
		If CanonicalizationMethod <> "" Then
			oSignedXML.SignedInfo.CanonicalizationMethod = CanonicalizationMethod
		End If

		'Add a reference - this describes what is being signed
		Dim oReference As New Reference()
		oReference.Uri = ReferenceURI ' Empty string is for signing the entire document

		'Add an enveloped transformation to the reference - defines to the reciever how it was signed
		Dim oTransform As New XmlDsigEnvelopedSignatureTransform()
		oReference.AddTransform(oTransform)

		' Add the reference to the SignedXml object.
		oSignedXML.AddReference(oReference)

		'Create the signature and get the XML
		oSignedXML.ComputeSignature()
		Dim oXmlElement As XmlElement = oSignedXML.GetXml()

		'Add the signature to the original XML document
		oNodeForSignature.AppendChild(oXmlElement)

		Return XML

	End Function

#End Region

#Region "Validate"

	''' <summary>
	''' Checks the signature of the signed xml at the specified path on the xml document is valid for the certificate with the specified friendly certificate name.
	''' </summary>
	''' <param name="XML">The XML.</param>
	''' <param name="CertificateFriendlyName">Friendly name of the certificate to check against.</param>
	''' <param name="XPathForSignature">The xpath for signature.</param>
	''' <exception cref="System.Exception"></exception>
	Public Shared Function ValidateSignedXML(XML As XmlDocument, CertificateFriendlyName As String, XPathForSignature As String) As Boolean

		'Get the certificate that will be used for signing
		Dim oCertificate As X509Certificate2 = Security.Functions.GetCertificate(CertificateFriendlyName)

		'Check we got something back
		If oCertificate Is Nothing Then Throw New Exception(String.Format("Certificate '{0}' not found", CertificateFriendlyName))

		Return ValidateSignedXML(XML, oCertificate, XPathForSignature)

	End Function

	''' <summary>
	''' Checks the signature of the signed xml at the specified path on the xml document is valid for the specified certificate.
	''' </summary>
	''' <param name="XML">The XML.</param>
	''' <param name="Certificate">The certificate.</param>
	''' <param name="XPathForSignature">The xpath for signature.</param>
	''' <exception cref="System.Exception">Node for signature not found. Check the XPath</exception>
	Public Shared Function ValidateSignedXML(XML As XmlDocument, Certificate As X509Certificate2, XPathForSignature As String) As Boolean

		'Set up the RSA key using the certificate public key
		Dim oRSAKey As RSACryptoServiceProvider = DirectCast(Certificate.PublicKey.Key, RSACryptoServiceProvider)

		Dim oSignedXML As New SignedXml(XML)

		Dim oXMLNode As XmlNode = XML.SelectSingleNode(XPathForSignature)

		'Get the signature node. Can't do this directly through XPath as it has a namespace that can vary
		For Each oNode As XmlNode In oXMLNode.ChildNodes
			If oNode.LocalName = "Signature" Then
				oXMLNode = oNode
				Exit For
			End If
		Next

		If oXMLNode Is Nothing Then Throw New Exception("Node for signature not found. Check the XPath")

		oSignedXML.LoadXml(CType(oXMLNode, XmlElement))

		Return oSignedXML.CheckSignature(oRSAKey)

	End Function

#End Region

#End Region

End Class