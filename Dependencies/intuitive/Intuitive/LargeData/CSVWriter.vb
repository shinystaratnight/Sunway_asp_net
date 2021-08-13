Imports System.Text
Imports System.IO

Namespace LargeData

	''' <summary>
	''' Class that represents a CSV writer
	''' </summary>
	Public Class CSVWriter

		Private TextWriter As TextWriter
		Private Delimiter As Char
		Private QuoteCharacter As Char
		Private EscapeCharacter As Char
		Private iCurrentLine As Integer

#Region "Properties"

		''' <summary>
		''' Gets the current line number.
		''' </summary>
		Public ReadOnly Property CurrentLine As Integer
			Get
				Return Me.iCurrentLine
			End Get
		End Property

#End Region

#Region "Constructors"

		''' <summary>
		''' Initializes a new instance of the <see cref="CSVWriter"/> class using a <see cref="TextWriter"/>.
		''' </summary>
		''' <param name="oTextWriter">The text writer.</param>
		''' <param name="sDelimiter">The delimiter.</param>
		''' <param name="sQuoteCharacter">The quote character.</param>
		''' <param name="sEscapeCharacter">The escape character.</param>
		Public Sub New(ByVal oTextWriter As TextWriter, Optional ByVal sDelimiter As Char = ","c,
				Optional ByVal sQuoteCharacter As Char = """"c, Optional ByVal sEscapeCharacter As Char = """"c)

			Me.TextWriter = oTextWriter
			Me.Delimiter = sDelimiter
			Me.QuoteCharacter = sQuoteCharacter
			Me.EscapeCharacter = sEscapeCharacter
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="CSVWriter"/> class using a <see cref="Stream"/>.
		''' </summary>
		''' <param name="oStream">The stream.</param>
		''' <param name="sDelimiter">The delimiter.</param>
		''' <param name="sQuoteCharacter">The quote character.</param>
		''' <param name="sEscapeCharacter">The escape character.</param>
		Public Sub New(ByVal oStream As Stream, Optional ByVal sDelimiter As Char = ","c,
				Optional ByVal sQuoteCharacter As Char = """"c, Optional ByVal sEscapeCharacter As Char = """"c)

			Me.New(New StreamWriter(oStream), sDelimiter, sQuoteCharacter, sEscapeCharacter)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="CSVWriter"/> class using a <see cref="Text.StringBuilder"/>.
		''' </summary>
		''' <param name="oStringBuilder">The string builder.</param>
		''' <param name="sDelimiter">The delimiter.</param>
		''' <param name="sQuoteCharacter">The quote character.</param>
		''' <param name="sEscapeCharacter">The escape character.</param>
		Public Sub New(ByVal oStringBuilder As StringBuilder, Optional ByVal sDelimiter As Char = ","c,
				Optional ByVal sQuoteCharacter As Char = """"c, Optional ByVal sEscapeCharacter As Char = """"c)

			Me.New(New StringWriter(oStringBuilder), sDelimiter, sQuoteCharacter, sEscapeCharacter)
		End Sub

#End Region

#Region "Write Row"

		''' <summary>
		''' Writes a row with the specified values.
		''' </summary>
		''' <param name="aValues">The values to write.</param>
		Public Sub WriteRow(ByVal ParamArray aValues() As Object)
			Me.WriteRow(True, aValues)
		End Sub

		''' <summary>
		''' Writes a row with the specified values.
		''' </summary>
		''' <param name="oValues">The values to write.</param>
		Public Sub WriteRow(ByVal oValues As ICollection)
			Me.WriteRow(True, oValues)
		End Sub

		''' <summary>
		''' Writes a header row with the specified values.
		''' </summary>
		''' <param name="aValues">The values to write.</param>
		Public Sub WriteHeaderRow(ByVal ParamArray aValues() As Object)
			Me.WriteRow(False, aValues)
		End Sub

		''' <summary>
		''' Writes a header row with the specified values.
		''' </summary>
		''' <param name="oValues">The values to write.</param>
		Public Sub WriteHeaderRow(ByVal oValues As ICollection)
			Me.WriteRow(False, oValues)
		End Sub

		''' <summary>
		''' Writes a row with the specified values.
		''' </summary>
		''' <param name="bIncreaseRowCount">if set to <c>true</c> increase the row count.</param>
		''' <param name="aValues">The values to write.</param>
		Private Sub WriteRow(ByVal bIncreaseRowCount As Boolean, ByVal ParamArray aValues() As Object)
			Me.TextWriter.WriteLine(LargeDataFunctions.CSVJoinLine(aValues, Me.Delimiter, Me.QuoteCharacter, Me.EscapeCharacter))
			If bIncreaseRowCount Then Me.iCurrentLine += 1
		End Sub

		''' <summary>
		''' Writes a rowwith the specified values.
		''' </summary>
		''' <param name="bIncreaseRowCount">if set to <c>true</c>, increase the row count.</param>
		''' <param name="oValues">The values to write.</param>
		Private Sub WriteRow(ByVal bIncreaseRowCount As Boolean, ByVal oValues As ICollection)
			Dim aValues(oValues.Count - 1) As Object
			oValues.CopyTo(aValues, 0)
			Me.WriteRow(bIncreaseRowCount, aValues)
		End Sub

#End Region

#Region "Flush"

		''' <summary>
		''' Flushes this instance.
		''' </summary>
		Public Sub Flush()
			Me.TextWriter.Flush()
		End Sub

#End Region

	End Class

End Namespace