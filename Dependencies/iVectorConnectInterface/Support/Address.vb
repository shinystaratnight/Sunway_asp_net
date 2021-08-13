Namespace Support

	Public Class Address

		'customer fields
		Public CustomerTitle As String = ""
		Public CustomerFirstName As String = ""
		Public CustomerLastName As String = ""
		Public CustomerID As Integer = 0
		Public Address1 As String = ""
		Public Address2 As String = ""
		Public TownCity As String
		Public County As String = ""
		Public Postcode As String
		Public Telephone As String = ""
		Public Mobile As String = ""
		Public Fax As String = ""
		Public Email As String = ""
		Public BirthDay As String = ""
		Public Sequence As Integer
		Public BookingCountryID As Integer = 1
		Public CountryISOCode As String
		Public Type As String = ""
		Public LanguageID As Integer = 0

		'postcode provider stuff
		Public BuildingID As String
		Public Description As String
	End Class

End Namespace