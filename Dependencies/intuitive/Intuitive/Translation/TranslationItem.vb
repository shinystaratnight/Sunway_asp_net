''' <summary>
''' Class representing a single translation item
''' </summary>
Public Class TranslationItem

	''' <summary>
	''' The original text
	''' </summary>
	Public Original As String

	''' <summary>
	''' The translated text
	''' </summary>
	Public Translation As String

	''' <summary>
	''' The translation item type
	''' </summary>
	Public TranslationItemType As TranslationItemType

	''' <summary>
	''' How long the translation took
	''' </summary>
	Public Duration As Double

	''' <summary>
	''' Specifies whether this item attributes
	''' </summary>
	Public HasAttributes As Boolean

	''' <summary>
	''' Specifies whether the translation has errored
	''' </summary>
	Public Errored As Boolean = False

End Class