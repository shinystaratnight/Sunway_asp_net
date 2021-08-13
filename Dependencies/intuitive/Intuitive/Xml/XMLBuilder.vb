Imports System.IO
Imports System.Text

Namespace Xml

	''' <summary>
	''' Class with functions for building an xml document, adding nodes and elements.
	''' </summary>
	Public Class XMLBuilder

		''' <summary>
		''' Stringbuilder used to build the xml document string
		''' </summary>
		Private sb As New StringBuilder
		''' <summary>
		''' The i indent level
		''' </summary>
		Private iIndentLevel As Integer = 0
		''' <summary>
		''' The xml document declaration
		''' </summary>
		Public Prolog As String = "<?xml version=""1.0""?>"

		''' <summary>
		''' Initializes a new instance of the <see cref="XMLBuilder"/> class.
		''' </summary>
		''' <param name="Prolog">The prolog.</param>
		Public Sub New(Optional ByVal Prolog As String = "")

			If Prolog <> "" Then
				Me.Prolog = Prolog
			End If

			sb.Append(Me.Prolog).Append(Environment.NewLine)

		End Sub

		''' <summary>
		''' Adds start node with specified name to xml document
		''' </summary>
		''' <param name="NodeName">Name of the node.</param>
		Public Sub StartNode(ByVal NodeName As String)
			sb.Append("	"c, iIndentLevel)
			sb.Append("<").Append(NodeName).Append(">").Append(Environment.NewLine)
			iIndentLevel += 1
		End Sub

		''' <summary>
		''' Adds end node with specified name to xml document
		''' </summary>
		''' <param name="NodeName">Name of the node.</param>
		Public Sub EndNode(ByVal NodeName As String)
			iIndentLevel -= 1
			sb.Append("	"c, iIndentLevel)
			sb.Append("</").Append(NodeName).Append(">").Append(Environment.NewLine)
		End Sub

		''' <summary>
		''' Adds an element to the xml document with the specified name and value.
		''' </summary>
		''' <param name="ElementName">Name of the element.</param>
		''' <param name="Value">The value.</param>
		''' <param name="DataType">The <see cref="ElementDataType"/>, can be String, Date or Decimal.</param>
		Public Sub AddElement(ByVal ElementName As String, ByVal Value As Object,
			Optional ByVal DataType As ElementDataType = ElementDataType.String)

			sb.Append("	"c, iIndentLevel)
			sb.Append("<").Append(ElementName).Append(">")

			If DataType = ElementDataType.Date Then
				Dim dDate As Date = Intuitive.DateFunctions.SafeDate(Value)
				sb.Append(dDate.ToString("yyyy-MM-dd")).Append("T").Append(dDate.ToString("HH:mm:ss"))
			ElseIf DataType = ElementDataType.Decimal Then
				Dim nValue As Double = Intuitive.Functions.SafeNumeric(Value)
				sb.Append(nValue.ToString("########0.00"))
			Else
				sb.Append(Value.ToString.Trim.Replace("&", "&amp;"))
			End If

			sb.Append("</").Append(ElementName).Append(">").Append(Environment.NewLine)

			'2006-01-19T16:07:16
		End Sub

		''' <summary>
		''' Saves the xml string to a file with the specified filename
		''' </summary>
		''' <param name="Filename">The filename.</param>
		Public Sub Save(ByVal Filename As String)
			File.WriteAllText(Filename, sb.ToString)
		End Sub

		''' <summary>
		''' Returns the xml document string
		''' </summary>
		Public Overrides Function ToString() As String
			Return sb.ToString
		End Function

		''' <summary>
		''' Enum of possible data types for the xml elements
		''' </summary>
		Public Enum ElementDataType
			[Date]
			[String]
			[Decimal]
		End Enum

	End Class

End Namespace