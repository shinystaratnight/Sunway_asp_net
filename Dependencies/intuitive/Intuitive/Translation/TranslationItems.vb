Imports System.Collections.Generic

''' <summary>
''' Class representing a list of <see cref="TranslationItem"/>s
''' </summary>
''' <seealso cref="System.Collections.Generic.List" />
Public Class TranslationItems
	Inherits List(Of TranslationItem)

	''' <summary>
	''' Adds a new translation item with the specified item type and duration
	''' </summary>
	''' <param name="TranslationType">Type of the translation item.</param>
	''' <param name="Duration">The duration.</param>
	Public Overloads Sub Add(ByVal TranslationType As TranslationItemType, ByVal Duration As Double)
		Me.Add(TranslationType, Duration, "", "", False)
	End Sub

	''' <summary>
	''' Adds a new translation item with the specified item type, duration and attribute status
	''' </summary>
	''' <param name="TranslationType">Type of the translation item.</param>
	''' <param name="Duration">The duration.</param>
	''' <param name="HasAttributes">Specifies whether the translation item has attributes</param>
	Public Overloads Sub Add(ByVal TranslationType As TranslationItemType, ByVal Duration As Double, ByVal HasAttributes As Boolean)
		Me.Add(TranslationType, Duration, "", "", False)
	End Sub

	''' <summary>
	''' Adds the specified translation type.
	''' </summary>
	''' <param name="TranslationType">Type of the translation item.</param>
	''' <param name="Duration">The duration.</param>
	''' <param name="Original">The original text.</param>
	''' <param name="Translation">The translated text.</param>
	Public Overloads Sub Add(ByVal TranslationType As TranslationItemType, ByVal Duration As Double, ByVal Original As String, ByVal Translation As String)
		Me.Add(TranslationType, Duration, Original, Translation, False)
	End Sub

	''' <summary>
	''' Adds the specified translation type.
	''' </summary>
	''' <param name="TranslationType">Type of the translation item.</param>
	''' <param name="Duration">The duration.</param>
	''' <param name="Original">The original text.</param>
	''' <param name="Translation">The translated text.</param>
	''' <param name="HasAttributes">Specifies whether the translation item has attributes</param>
	Public Overloads Sub Add(ByVal TranslationType As TranslationItemType, ByVal Duration As Double, ByVal Original As String, ByVal Translation As String, ByVal HasAttributes As Boolean)
		Me.Add(TranslationType, Duration, Original, Translation, HasAttributes, False)
	End Sub

	''' <summary>
	''' Adds the specified translation type.
	''' </summary>
	''' <param name="TranslationType">Type of the translation item.</param>
	''' <param name="Duration">The duration.</param>
	''' <param name="Original">The original text.</param>
	''' <param name="Translation">The translated text.</param>
	''' <param name="HasAttributes">Specifies whether the translation item has attributes</param>
	''' <param name="Errored">Specifies whether the translation has errored</param>
	Public Overloads Sub Add(ByVal TranslationType As TranslationItemType,
		ByVal Duration As Double,
		ByVal Original As String,
		ByVal Translation As String,
		ByVal HasAttributes As Boolean,
		ByVal Errored As Boolean)
		Dim oTranslationItem As New TranslationItem
		With oTranslationItem
			.TranslationItemType = TranslationType
			.Duration = Duration
			.HasAttributes = HasAttributes
			.Translation = Translation
			.Original = Original
			.Errored = Errored
		End With
		Me.Add(oTranslationItem)
	End Sub

End Class