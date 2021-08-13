Imports System.Text.RegularExpressions

''' <summary>
''' Class containing a regex translation pattern to match on, as well as an evaluator to perform a function on regex matches
''' </summary>
Public Class TranslationRegex

	''' <summary>
	''' The regex pattern
	''' </summary>
	Public Pattern As String

	''' <summary>
	''' The time taken for translation
	''' </summary>
	Public TimeTaken As Decimal

	''' <summary>
	''' The evaluator containing the function to perform on matches
	''' </summary>
	Public Evaluator As MatchEvaluator

End Class