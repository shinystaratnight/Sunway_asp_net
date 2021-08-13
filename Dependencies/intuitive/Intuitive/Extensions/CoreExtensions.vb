Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Xml
Imports System.Drawing

''' <summary>
''' Provides functional extentions to objects
''' </summary>
Public Module Extensions

#Region "ToSafe..."

	''' <summary>
	''' Converts object to an integer value
	''' </summary>
	''' <param name="oInteger">The object to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToSafeInt(oInteger As Object) As Integer
		Return Functions.SafeInt(oInteger)
	End Function

	''' <summary>
	''' Converts object to a double
	''' </summary>
	''' <param name="oDouble">The object to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToSafeNumeric(oDouble As Object) As Double
		Return Functions.SafeNumeric(oDouble)
	End Function

	''' <summary>
	''' Converts an object to a decimal
	''' </summary>
	''' <param name="oDecimal">The object to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToSafeDecimal(oDecimal As Object) As Decimal
		Return Functions.SafeDecimal(oDecimal)
	End Function

	''' <summary>
	''' Converts an object to a date
	''' </summary>
	''' <param name="oDate">The object to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToSafeDate(oDate As Object) As Date
		Return DateFunctions.SafeDate(oDate)
	End Function

	''' <summary>
	''' Converts an object to a boolean
	''' </summary>
	''' <param name="oBoolean">The object to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToSafeBoolean(oBoolean As Object) As Boolean
		Return Functions.SafeBoolean(oBoolean)
	End Function

	''' <summary>
	''' Converts an object to a string
	''' </summary>
	''' <param name="oString">The object to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToSafeString(oString As Object) As String
		Return Functions.SafeString(oString)
	End Function

#End Region

#Region "String"

	''' <summary>
	''' Returns a string with the specified number of characters removed from the end.
	''' </summary>
	''' <param name="String">The string.</param>
	''' <param name="CharsToChop">The number of chars to chop.</param>
	''' <returns></returns>
	<Extension()>
	Public Function Chop([String] As String, Optional ByVal CharsToChop As Integer = 1) As String
		Return Functions.Chop([String], CharsToChop)
	End Function

	''' <summary>
	''' Converts date to string with format dd MMM yyyy.
	''' </summary>
	''' <param name="Date">The date.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToDisplayDate([Date] As Date) As String
		Return Intuitive.DateFunctions.DisplayDate([Date])
	End Function

	''' <summary>
	''' Returns a string with each word capitalised.
	''' </summary>
	''' <param name="value">The string to capitalise.</param>
	''' <returns></returns>
	<Extension()>
	Public Function Capitalise(value As String) As String
		Return Functions.Capitalise(value)
	End Function

	''' <summary>
	''' Returns a string with 'ID' strings removed and spaces inserted before capital letters
	''' </summary>
	''' <param name="value">The string to alter.</param>
	''' <returns></returns>
	<Extension()>
	Public Function GetNiceName(value As String) As String
		Return Functions.GetNiceName(value)
	End Function

	''' <summary>
	''' Capitalises the first letter of each word in a string, capitalises the first letter after a Mc, e.g. McDonald
	''' </summary>
	''' <param name="value">The string to capitalise.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ProperCase(value As String) As String
		Return Functions.ProperCase(value)
	End Function

	''' <summary>
	''' Counts the occurances of a substring within a string .
	''' </summary>
	''' <param name="value">The string we're searching.</param>
	''' <param name="SearchText">The string to search for.</param>
	''' <returns></returns>
	<Extension()>
	Public Function CountOccurances(value As String, SearchText As String) As Integer

		Dim iIndex As Integer = 0
		Dim iFound As Integer = 0
		Do
			iIndex = value.IndexOf(SearchText, iIndex)
			If iIndex = -1 Then Exit Do
			iIndex += 1
			iFound += 1
		Loop

		Return iFound

	End Function

#End Region

#Region "StringBuilder"

	''' <summary>
	''' Appends a formatted string to the stringbuilder then appends a newline character
	''' </summary>
	''' <param name="sb">The stringbuilder.</param>
	''' <param name="value">The string to append.</param>
	''' <param name="args">The string format values.</param>
	<Extension()>
	Public Sub AppendFormatLine(ByRef sb As StringBuilder, value As String, ByVal ParamArray args() As Object)
		sb.AppendFormat(value, args).AppendLine()
	End Sub

	''' <summary>
	''' Converts the stringbuilder to a string, chops off the number of end characters specified, then converts result back to stringbuilder
	''' </summary>
	''' <param name="sb">The stringbuilder to chop.</param>
	''' <param name="CharactersToChop">The number of end characters to chop.</param>
	<Extension()>
	Public Sub Chop(ByRef sb As StringBuilder, Optional ByVal CharactersToChop As Integer = 1)
		If sb.Length >= CharactersToChop Then
			sb.Length = sb.Length - CharactersToChop
		End If
	End Sub

	''' <summary>
	''' Appends the specified number of lines to the stringbuilder
	''' </summary>
	''' <param name="sb">The stringbuilder.</param>
	''' <param name="LineCount">The number of lines to append.</param>
	<Extension()>
	Public Sub AppendLines(ByRef sb As StringBuilder, LineCount As Integer)
		For i As Integer = 1 To LineCount
			sb.AppendLine("")
		Next
	End Sub

	''' <summary>
	''' Pads the right of the string with spaces to the speficied length and appends it to the stringbuilder
	''' </summary>
	''' <param name="sb">The stringbuilder.</param>
	''' <param name="value">The string to pad.</param>
	''' <param name="PadLength">Desired length of the string with padding.</param>
	<Extension()>
	Public Function AppendPad(ByRef sb As StringBuilder, value As String, PadLength As Integer) As StringBuilder
		sb.Append(value.PadRight(PadLength))
		Return sb
	End Function

	''' <summary>
	''' Pads the left of the string with spaces to the speficied length and appends it to the stringbuilder
	''' </summary>
	''' <param name="sb">The stringbuilder.</param>
	''' <param name="value">The string to pad.</param>
	''' <param name="PadLength">Desired length of the string with padding.</param>
	''' <returns></returns>
	<Extension()>
	Public Function AppendPadLeft(ByRef sb As StringBuilder, value As String, PadLength As Integer) As StringBuilder
		sb.Append(value.PadLeft(PadLength))
		Return sb
	End Function

	''' <summary>
	''' HTML encodes XMLValue, formats the string and appends to stringbuilder
	''' </summary>
	''' <param name="sb">The stringbuilder.</param>
	''' <param name="value">The value.</param>
	''' <param name="XMLValue">The XML value to HTML encode.</param>
	<Extension()>
	Public Sub AppendEnc(ByRef sb As StringBuilder, value As String, XMLValue As String)
		sb.AppendFormat(value, System.Web.HttpUtility.HtmlEncode(XMLValue))
	End Sub

#End Region

#Region "List"

    ''' <summary>
    ''' Converts a csv to a list of integers, splitting on the provided Delimiter
    ''' </summary>
    ''' <param name="sCSV">The CSV to convert to a list.</param>
    ''' <param name="Delimiter">The delimiter to split the csv.</param>
    ''' <returns></returns>
    <Extension()>
	Public Function ToList(sCSV As String, Optional ByVal Delimiter As Char = ","c) As List(Of Integer)
        Return sCSV.Split(Delimiter).Where(Function(o) Not String.IsNullOrWhiteSpace(o)).Select(Function(o) Integer.Parse(o)).ToList()
    End Function

#End Region

#Region "IEnumerable"

	''' <summary>
	''' Returns all combinations of a sequence of sequences of any type T.
	''' For example, given ( (a,b), (c,d) )
	''' will return: ( (a,c), (a,d), (b,c), (b,d) ).
	''' Uses deferred execution same as Linq methods.
	''' </summary>
	''' <typeparam name="T">The type of the sequences</typeparam>
	''' <param name="sequences">The sequence of sequences that we want all permutations of.</param>
	''' <returns>A sequence of sequences, </returns>
	<Extension()>
	Public Function CartesianProduct(Of T)(sequences As IEnumerable(Of IEnumerable(Of T))) As IEnumerable(Of IEnumerable(Of T))
		Dim emptyProduct As IEnumerable(Of IEnumerable(Of T)) = {Enumerable.Empty(Of T)()}
		Return sequences.Aggregate(
			emptyProduct,
			Function(accumulator, sequence)
				Return From accseq In accumulator
			           From item In sequence
			           Select accseq.Concat({item})
			End Function)
	End Function
#End Region

#Region "Maths"

	''' <summary>
	''' Rounds a number, only works properly with 3 decimal places
	''' </summary>
	''' <param name="nNumber">The n number.</param>
	<Obsolete("DoRound is obsolete, use Math.Round instead")>
	<Extension()>
	Public Function DoRound(nNumber As Double) As Double
		Return Functions.Round(nNumber)
	End Function

	''' <summary>
	''' Calculates the modulus of 2 numbers
	''' </summary>
	''' <param name="iBigNumber">The big number.</param>
	''' <param name="iSmallNumber">The small number.</param>
	<Obsolete("DoModulus is obsolete, use mod instead, e.g. int1 mod int2")>
	<Extension()>
	Public Function DoModulus(iBigNumber As Integer, iSmallNumber As Integer) As Integer
		Return Functions.Modulus(iBigNumber, iSmallNumber)
	End Function

	''' <summary>
	''' Rounds a number up to the nearest specified decimal place.
	''' </summary>
	''' <param name="nNumber">The number.</param>
	''' <param name="iDecimalPlaces">The desired number of decimal places.</param>
	''' <returns></returns>
	<Extension()>
	Public Function DoRoundUp(nNumber As Decimal, Optional ByVal iDecimalPlaces As Integer = 2) As Decimal
		Return Functions.RoundUp(nNumber, iDecimalPlaces)
	End Function

	''' <summary>
	''' Rounds a number down to the nearest specified decimal place
	''' </summary>
	''' <param name="nNumber">The number.</param>
	''' <param name="iDecimalPlaces">The desired number of decimal places.</param>
	''' <returns></returns>
	<Extension()>
	Public Function DoRoundDown(nNumber As Decimal, Optional ByVal iDecimalPlaces As Integer = 2) As Decimal
		Return Functions.RoundDown(nNumber, iDecimalPlaces)
	End Function

#End Region

#Region "DataTable / DataRow"

	''' <summary>
	''' Converts a datarow to an xml document, with a node for each column in the datarow
	''' </summary>
	''' <param name="value">The datarow to be converted.</param>
	''' <param name="ParentNode">The desired parent node of the xml document.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToGenericXML(value As DataRow, ParentNode As String) As XmlDocument

		Dim oSB As New StringBuilder

		oSB.AppendFormatLine("<{0}>", ParentNode)
		For Each oColumn As DataColumn In value.Table.Columns
			oSB.AppendFormatLine("<{0}>{1}</{0}>", oColumn.ColumnName, value(oColumn.ColumnName).ToString)
		Next
		oSB.AppendFormatLine("</{0}>", ParentNode)

		Dim oXML As New XmlDocument
		oXML.Load(oSB.ToString)
		Return oXML

	End Function

#End Region

#Region "XmlDocument"

	''' <summary>
	''' Sums the values of nodes specified in the XPath
	''' </summary>
	''' <param name="XMLDocument">The XML document to retrieve the value from.</param>
	''' <param name="XPath">The x path to where the nodes to be summed are.</param>
	<Extension()>
	Public Function SumNodes(XMLDocument As XmlDocument, XPath As String) As Decimal

		Dim nReturn As Decimal = 0

		Dim oNodes As XmlNodeList = XMLDocument.SelectNodes(XPath)
		For Each oNode As XmlNode In oNodes
			nReturn += Functions.SafeDecimal(oNode.InnerText)
		Next

		Return nReturn

	End Function

	''' <summary>
	''' Converts XMLDcument to XDocument
	''' </summary>
	''' <param name="XMLDocument">The XML document to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToXDocument(XMLDocument As XmlDocument) As Linq.XDocument
		Return XMLDocument.ToXDocument(Linq.LoadOptions.None)
	End Function

	''' <summary>
	''' Converts XMLDocument to XDocument with specified LoadOptions
	''' </summary>
	''' <param name="XMLDocument">The XML document to convert.</param>
	''' <param name="Options">System.Xml.Linq.LoadOptions.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToXDocument(XMLDocument As XmlDocument, Options As Linq.LoadOptions) As Linq.XDocument
		Using oXmlNodeReader As New XmlNodeReader(XMLDocument)
			Return Linq.XDocument.Load(oXmlNodeReader, Options)
		End Using
	End Function

#End Region

#Region "XDocument"

	''' <summary>
	''' Converts XDocument to XMLDocument
	''' </summary>
	''' <param name="XDocument">The XDocument to convert.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToXmlDocument(XDocument As Linq.XDocument) As XmlDocument
		Return XDocument.ToXmlDocument(Linq.ReaderOptions.None)
	End Function

	''' <summary>
	''' Converts XDocument to XMLDocument with specified ReaderOptions
	''' </summary>
	''' <param name="XDocument">The XDocument to convert to XMLDocument.</param>
	''' <param name="Options">The System.Xml.Linq.ReaderOptions to be used in the conversion.</param>
	''' <returns></returns>
	<Extension()>
	Public Function ToXmlDocument(XDocument As Linq.XDocument, Options As Linq.ReaderOptions) As XmlDocument
		Using oXmlNodeReader As XmlReader = XDocument.CreateReader(Options)
			Dim oXmlDocument As New XmlDocument()
			oXmlDocument.Load(oXmlNodeReader)
			Return oXmlDocument
		End Using
	End Function

#End Region

#Region "Image"

    <Extension()>
    Public Function GetMimeType(image As Image) As String
        Return image.RawFormat.GetMimeType()
    End Function

    <Extension()>
    Public Function GetMimeType(imageFormat As Imaging.ImageFormat) As String
        Dim codecs As Imaging.ImageCodecInfo() = Imaging.ImageCodecInfo.GetImageEncoders()
        Return codecs.First(Function(codec) codec.FormatID = imageFormat.Guid).MimeType
    End Function

#End Region

End Module