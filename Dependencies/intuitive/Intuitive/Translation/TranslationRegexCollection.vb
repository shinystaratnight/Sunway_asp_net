Imports System.Text.RegularExpressions

''' <summary>
''' Class containing a list of regular expressions used for translations
''' </summary>
''' <seealso cref="System.Collections.Generic.List" />
Public Class TranslationRegexCollection
	Inherits Generic.List(Of TranslationRegex)

	''' <summary>
	''' The time the translation was started
	''' </summary>
	Public StartTime As DateTime
	''' <summary>
	''' The time the translation was finished
	''' </summary>
	Public EndTime As DateTime

	''' <summary>
	''' Gets the translation time in milliseconds.
	''' </summary>
	Public ReadOnly Property Milliseconds() As Decimal
		Get
			Return Intuitive.Functions.SafeDecimal(EndTime.Subtract(StartTime).TotalMilliseconds)
		End Get
	End Property

	''' <summary>
	''' Adds a new <see cref="TranslationRegex"/> to the list
	''' </summary>
	''' <param name="sPattern">The regex pattern.</param>
	''' <param name="oEvaluator">The evaluator function to be called on a regex match.</param>
	Public Overloads Sub Add(ByVal sPattern As String, ByVal oEvaluator As MatchEvaluator)

		Dim oTranslationRegex As New TranslationRegex
		With oTranslationRegex
			.Pattern = sPattern
			.Evaluator = oEvaluator
		End With

		Me.Add(oTranslationRegex)

	End Sub

	''' <summary>
	''' Translates the page using this instances Translation Regex collection.
	''' </summary>
	''' <param name="sPageHTML">The page HTML to translate.</param>
	Public Function TranslatePage(ByVal sPageHTML As String) As String

		Me.StartTime = Date.Now

		For Each oTranslationRegex As TranslationRegex In Me
			Dim dStart As DateTime = Date.Now
			sPageHTML = Regex.Replace(sPageHTML, oTranslationRegex.Pattern, oTranslationRegex.Evaluator, RegexOptions.Compiled Or RegexOptions.Singleline)
			oTranslationRegex.TimeTaken = Intuitive.Functions.SafeDecimal(Date.Now.Subtract(dStart).TotalMilliseconds)
		Next

		sPageHTML = Regex.Replace(sPageHTML, "<(/?)trans[^>]*>", "", RegexOptions.Compiled)
		sPageHTML = Regex.Replace(sPageHTML, "<(/?)fo:trans[^>]*>", "", RegexOptions.Compiled)
		sPageHTML = Regex.Replace(sPageHTML, "\sml=\""[^""]+\""", "", RegexOptions.Compiled)

		Me.EndTime = Date.Now

		Return sPageHTML

	End Function

End Class