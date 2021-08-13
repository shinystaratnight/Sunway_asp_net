Imports System.Xml

Public Class XSL



	Public Shared Function SetupTemplate(ByVal Template As String, Optional ByVal IncludeFunctions As Boolean = False, _
	   Optional ByVal IncludeMarkdown As Boolean = False) As String

		'get template xml
		Dim oXSLTemplate As New XmlDocument
		oXSLTemplate.PreserveWhitespace = True 'ensures <xsl:text> </xsl:text> is not replaced with a closed node
		oXSLTemplate.LoadXml(Template)


		'setup xsl namespace
		Dim oNamespaces As New XmlNamespaceManager(oXSLTemplate.NameTable)
		oNamespaces.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform")


		'get root node
		Dim oRootNode As XmlNode = oXSLTemplate.SelectSingleNode("/xsl:stylesheet", oNamespaces)


		'include functions if set
		If IncludeFunctions Then
			Dim oFunctionsXSL As New XmlDocument
			oFunctionsXSL.LoadXml(res.Functions)
			Dim oNodeList As XmlNodeList = oFunctionsXSL.SelectNodes("//xsl:template", oNamespaces)
			For Each oNode As XmlNode In oNodeList
				oRootNode.AppendChild(oRootNode.OwnerDocument.ImportNode(oNode, True))
			Next
		End If


		'include markdown if set
		If IncludeMarkdown Then
			Dim oMarkdownXSL As New XmlDocument
			oMarkdownXSL.LoadXml(res.Markdown)
			Dim oNodeList As XmlNodeList = oMarkdownXSL.SelectNodes("//xsl:template", oNamespaces)
			For Each oNode As XmlNode In oNodeList
				oRootNode.AppendChild(oRootNode.OwnerDocument.ImportNode(oNode, True))
			Next

		End If


		'remove include nodes
		Dim oIncludeNodes As XmlNodeList = oXSLTemplate.SelectNodes("//xsl:include", oNamespaces)
		For Each oNode As XmlNode In oIncludeNodes
			oRootNode.RemoveChild(oNode)
		Next


		'return
		Return oXSLTemplate.InnerXml

	End Function


End Class
