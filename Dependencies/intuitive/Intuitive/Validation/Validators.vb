Imports System.Text.RegularExpressions

''' <summary>
''' Class containing various validators that validate strings using regex
''' </summary>
Public Class Validators

	''' <summary>
	''' Enum of possible card types
	''' </summary>
	Public Enum CardType
		Any
		Amex
		MasterCard
		Visa
		Diners
		EnRoute
		Discover
		JCB
		Unknown
	End Enum

	''' <summary>
	''' Uses regex to determine whether the specified URL is a valid URL.
	''' </summary>
	''' <param name="sURL">The URL to check.</param>
	Public Shared Function IsURL(ByVal sURL As String) As Boolean
		Return Regex.IsMatch(sURL, "^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*[^\.\,\)\(\s]$")
	End Function

	''' <summary>
	''' Uses regex to determine whether the specified postcode is valid.
	''' </summary>
	''' <param name="sPostcode">The postcode to check.</param>
	Public Shared Function IsPostcode(ByVal sPostcode As String) As Boolean
		Return Regex.IsMatch(sPostcode, "^([A-PR-UWYZ0-9][A-HK-Y0-9][AEHMNPRTVXY0-9]?[ABEHMNPRVWXY0-9]? {1,2}[0-9][ABD-HJLN-UW-Z]{2}|GIR 0AA)$ ")
	End Function

	''' <summary>
	''' Uses regex to determine whether specified postcode is a valid UK postcode.
	''' </summary>
	''' <param name="sPostcode">The postcode to check.</param>
	Public Shared Function IsUKPostcode(ByVal sPostcode As String) As Boolean
		'this isn't perfect, but will do for now and is reasonably future proof
		Return Regex.IsMatch(sPostcode, "^[A-Z]{1,2}[0-9]([0-9]|[A-Z])? {1,2}[0-9][ABDEFGHJLNPQRSTUWXYZ]{2}$")
	End Function

	''' <summary>
	''' Uses regex to determine whether the specified email is a valid email address.
	''' </summary>
	''' <param name="sEmail">The email address to validate.</param>
	Public Shared Function IsEmail(ByVal sEmail As String) As Boolean
		Return Regex.IsMatch(sEmail, "^([^@]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z0-9]+)$")
	End Function

	''' <summary>
	''' Uses regex to determine whether the specified emails are valid
	''' </summary>
	''' <param name="sEmail">The email addresses to validate, separated by ';'.</param>
	Public Shared Function IsMultipleEmail(ByVal sEmail As String) As Boolean

		Dim aEmailAddresses As String() = sEmail.Split(";"c)
		For Each sEmailAddress As String In aEmailAddresses
			If sEmailAddress <> "" AndAlso Not Validators.IsEmail(sEmailAddress) Then Return False
		Next

		Return True

	End Function

	''' <summary>
	''' Determines whether the specified string is a valid display date, in the format dd mmm yyyy
	''' </summary>
	''' <param name="sDate">The date string to validate.</param>
	Public Shared Function IsDisplayDate(ByVal sDate As String) As Boolean
		Return Regex.IsMatch(sDate, "^[0-3][0-9]\s" &
			"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s(19|20)\d\d$", RegexOptions.IgnoreCase)
	End Function

	''' <summary>
	''' Determines whether the specified time string is a valid time
	''' </summary>
	''' <param name="sTime">The time string to check.</param>
	Public Shared Function IsTime(ByVal sTime As String) As Boolean
		Return Regex.IsMatch(sTime, "^([0-1][0-9]|2[0-3]):[0-5][0-9]$")
	End Function

	''' <summary>
	''' Determines whether the specified string is a valid VAT number
	''' </summary>
	''' <param name="VATNumber">The vat number to check.</param>
	Public Shared Function IsVATNumber(ByVal VATNumber As String) As Boolean
		Return Regex.IsMatch(VATNumber, "^\d{3}\s\d{4}\s\d{2}$")
	End Function

	''' <summary>
	''' Determines whether the specified string is a valid IPV4 address
	''' </summary>
	''' <param name="IPAddress">The ip address.</param>
	''' <returns></returns>
	Public Shared Function IsIPAddress(ByVal IPAddress As String) As Boolean
		Return Regex.IsMatch(IPAddress, "^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$")
	End Function

	''' <summary>
	''' Uses regex to determine whether the specified card number is valid
	''' </summary>
	''' <param name="sNumber">The card number to validate.</param>
	Public Shared Function IsCardValid(ByVal sNumber As String) As Boolean

		'return false straight away if it isn't a number or is < 13 digits
		If sNumber IsNot Nothing AndAlso sNumber.Length < 13 Then Return False

		'strip the cardnumber so its just numbers
		If sNumber.Length <> Functions.DigitsOnly(sNumber).Length Then Return False

		'validate the card number using Lunh's formula
		'If isCardValidType(sNumber) = True Then
		Dim aCheckNums1 As New ArrayList
		Dim i As Integer
		Dim iTotal1 As Integer
		Dim iTotal2 As Integer

		'step 1 - Double the value of alternating digits - starting with the 2nd digit from the right
		For i = sNumber.Length - 2 To 0 Step -2
			aCheckNums1.Add((Int32.Parse(sNumber.Substring(i, 1)) * 2))
		Next

		'step 2 - Add the separate digits of all the products of step 1
		For i = 0 To aCheckNums1.Count - 1
			If aCheckNums1(i).ToString.Length = 1 Then
				iTotal1 += Int32.Parse(aCheckNums1(i).ToString)
			Else
				iTotal1 += (Int32.Parse(aCheckNums1(i).ToString.Substring(0, 1)) +
							Int32.Parse(aCheckNums1(i).ToString.Substring(1, 1)))
			End If
		Next

		'step 3 - Add the unaffected digits - starting the rightmost digit
		For i = sNumber.Length - 1 To 0 Step -2
			iTotal2 += Int32.Parse(sNumber.Substring(i, 1))
		Next

		'step 4 - sum results of step 2 and 3 and mod by 10 if its 0 then its valid
		Return ((iTotal1 + iTotal2) Mod 10 = 0)

	End Function

#Region "old card rubbish"

	''' <summary>
	''' Determines whether the specified card number is valid for the specified <see cref="CardType"/>.
	''' </summary>
	''' <param name="sNumber">The s number.</param>
	''' <param name="CardType">Type of the card.</param>
	Public Shared Function IsCardValidType(ByVal sNumber As String,
			Optional ByVal CardType As CardType = CardType.Any) As Boolean

		Dim bValidType As Boolean = False

		'validate card type is the correct length for its type

		If Regex.IsMatch(sNumber, "^(34|37)") Then
			'amex (34,37) 15 length
			bValidType = (sNumber.Length = 15 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.Amex))
		ElseIf Regex.IsMatch(sNumber, "^(51|52|53|54|55)") Then
			'mastercard (51-55) 16 length
			bValidType = (sNumber.Length = 16 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.MasterCard))
		ElseIf Regex.IsMatch(sNumber, "^(4)") Then
			'visa (4) 13 or 16 length
			bValidType = (sNumber.Length = 13 Or sNumber.Length = 16 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.Visa))
		ElseIf Regex.IsMatch(sNumber, "^(300|301|302|303|304|305|36|38)") Then
			'diners (300-305,36,38) 14 length
			bValidType = (sNumber.Length = 14 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.Diners))
		ElseIf Regex.IsMatch(sNumber, "^(2014|2149)") Then
			'enRoute (2014,2149) 15 length
			bValidType = (sNumber.Length = 15 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.EnRoute))
		ElseIf Regex.IsMatch(sNumber, "^(6011)") Then
			'discover (6011) 16 length
			bValidType = (sNumber.Length = 16 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.Discover))
		ElseIf Regex.IsMatch(sNumber, "^(3)") Then
			'jcb (3) 16 length
			bValidType = (sNumber.Length = 16 AndAlso
				(CardType = CardType.Any OrElse CardType = CardType.JCB))
		ElseIf Regex.IsMatch(sNumber, "^(2131|1800)") Then
			'more jcb (2131,1800) 15 length
			bValidType = (sNumber.Length = 15 AndAlso
			(CardType = CardType.Any OrElse CardType = CardType.JCB))
		Else
			'type unknown
			bValidType = False
		End If
		Return bValidType
	End Function

#End Region

	''' <summary>
	''' Determines whether the specified string is a valid credit card month.
	''' </summary>
	''' <param name="CCMonth">The month string to validate.</param>
	Public Shared Function IsCCMonth(ByVal CCMonth As String) As Boolean
		Return Regex.IsMatch(CCMonth, "(01|02|03|04|05|06|07|08|09|10|11|12)")
	End Function

	''' <summary>
	''' Determines whether the specified string is a valid credit card year.
	''' </summary>
	''' <param name="CCYear">The year string to validate.</param>
	Public Shared Function IsCCYear(ByVal CCYear As String) As Boolean
		Return Regex.IsMatch(CCYear, "^[0-2][0-9]$")
	End Function

	''' <summary>
	''' Determines whether the specified string is a valid credit card security number i.e. 3 digits.
	''' </summary>
	''' <param name="CCSecurityNumber">The security number to validate.</param>
	Public Shared Function IsCCSecurityNumber(ByVal CCSecurityNumber As String) As Boolean
		Return Regex.IsMatch(CCSecurityNumber, "^[\d][\d][\d]$")
	End Function

End Class