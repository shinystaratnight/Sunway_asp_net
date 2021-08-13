Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports Intuitive.Functions

''' <summary>
''' Various date functions for comparing dates, validating dates and retrieving dates from strings
''' </summary>
Public Class DateFunctions

	''' <summary>
	''' Checks whether two date ranges overlap.
	''' </summary>
	''' <param name="dCheckStart">Start of the date range to check.</param>
	''' <param name="dCheckEnd">End of the date range to check.</param>
	''' <param name="dStart">Start of the range to check against.</param>
	''' <param name="dEnd">End of the range to check against.</param>
	Public Shared Function DateRangeOverlaps(ByVal dCheckStart As Date, ByVal dCheckEnd As Date,
				ByVal dStart As Date, ByVal dEnd As Date) As Boolean

		Return (dStart >= dCheckStart AndAlso dStart < dCheckEnd) OrElse
			(dEnd > dCheckStart AndAlso dEnd <= dCheckEnd) OrElse
			(dStart <= dCheckStart AndAlso dEnd >= dCheckEnd)

	End Function


	''' <summary>
	''' Checks whether date is within a range of two dates.
	''' </summary>
	''' <param name="dDateCheck">The date check to check.</param>
	''' <param name="dDateStart">The start date of the range.</param>
	''' <param name="dDateEnd">The end date of the range.</param>
	Public Shared Function DateBetween(ByVal dDateCheck As Date, ByVal dDateStart As Date, ByVal dDateEnd As Date) As Boolean
		Return dDateCheck >= dDateStart AndAlso dDateCheck <= dDateEnd
	End Function

	''' <summary>
	''' Checks whether a date range is within another date range.
	''' </summary>
	''' <param name="dCheckStart">Start of the date range to check.</param>
	''' <param name="dCheckEnd">End of the date range to check.</param>
	''' <param name="dStart">Start of the range to check against.</param>
	''' <param name="dEnd">End of the range to check against.</param>
	Public Shared Function DateRangeBetween(ByVal dCheckStart As Date, ByVal dCheckEnd As Date,
			   ByVal dStart As Date, ByVal dEnd As Date) As Boolean

		Return DateBetween(dCheckStart, dStart, dEnd) AndAlso DateBetween(dCheckEnd, dStart, dEnd)
	End Function

	''' <summary>
	''' Checks a provided date is valid then returns it in the dd MMM yyyy format.
	''' </summary>
	''' <param name="dDate">The date to represent.</param>
	Public Shared Function DisplayDate(ByVal dDate As Date) As String
		If Not IsEmptyDate(dDate) Then
			Return dDate.ToString("dd MMM yyyy")
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Checks a provided date is valid then returns it in the dd/MM/yyyy format.
	''' </summary>
	''' <param name="dDate">The date to represent.</param>
	Public Shared Function ShortDate(ByVal dDate As Date) As String
		If Not IsEmptyDate(dDate) Then
			Return dDate.ToString("dd/MM/yyyy")
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Checks a provided date is valid then returns it in the dd MMM 'yy format.
	''' </summary>
	''' <param name="dDate">The date to represent.</param>
	Public Shared Function DisplayDateShort(ByVal dDate As Date) As String
		If Not IsEmptyDate(dDate) Then
			Return dDate.ToString("dd MMM \'yy")
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Converts provided object to a date, checks the date is valid then returns it in the dd MMM yyyy HH:mm:ss format.
	''' </summary>
	''' <param name="oDate">The object to be converted to a date and displayed.</param>
	''' <param name="bIncludeSeconds">if set to <c>true</c> will display seconds for the date.</param>
	Public Shared Function DisplayDateTime(ByVal oDate As Object, Optional ByVal bIncludeSeconds As Boolean = False) As String
		Dim dDate As Date = SafeDate(oDate)
		If Not IsEmptyDate(dDate) Then
			Return dDate.ToString("dd MMM yyyy HH:mm") & IIf(bIncludeSeconds, dDate.ToString(":ss"), "")
		Else
			Return ""
		End If
	End Function

	''' <summary>
	''' Converts object to date, checks the date is valid then returns it in the dd MMM yyyy format.
	''' </summary>
	''' <param name="oDate">The object to be converted and dislplayed.</param>
	Public Shared Function DisplayDate(ByVal oDate As Object) As String

		If IsDate(oDate.ToString) AndAlso Not IsEmptyDate(CType(oDate, Date)) Then
			Return CType(oDate, Date).ToString("dd MMM yyyy")
		Else
			Return ""
		End If
	End Function



	''' <summary>
	''' Tries to convert the object to a date, if it's a valid date, returns the date, otherwise returns 01 01 1900
	''' </summary>
	''' <param name="oDate">The object to be converted to a date.</param>
	Public Shared Function SafeDate(ByVal oDate As Object) As Date

		'check for nothing object
		Dim sDate As String
		If oDate IsNot Nothing Then
			sDate = oDate.ToString
		Else
			sDate = ""
		End If

		'parse and return
		Dim dDate As Date
		Dim bOk As Boolean = Date.TryParse(sDate, dDate)
		If bOk Then
			Return dDate
		Else
			Return DateFunctions.EmptyDate
		End If

	End Function

	''' <summary>
	''' Converts a display date string to a Date
	''' </summary>
	''' <param name="sDate">The string to be converted to a date.</param>
	Public Shared Function DisplayDateToDate(ByVal sDate As String) As Date
		Dim aDateParts() As String
		Dim iDay As Integer
		Dim iMonth As Integer
		Dim iYear As Integer

		sDate = sDate.Trim
		If Validators.IsDisplayDate(sDate) Then
			aDateParts = sDate.Split(" ".ToCharArray)

			iDay = CType(aDateParts(0), Integer)
			iMonth = GetMonthFromDisplayMonth(aDateParts(1))
			iYear = CType(aDateParts(2), Integer)

			If (iDay < 0 Or iDay > 31) Or iMonth < 0 Or iMonth > 12 Then
				Return EmptyDate()
			Else
				Return New Date(iYear, iMonth, iDay)
			End If
		Else
			Return EmptyDate()
		End If
	End Function

	''' <summary>
	''' Creates a new date for the 1st Jan 1990
	''' </summary>
	Public Shared Function EmptyDate() As Date
		Return New Date(1900, 1, 1, 0, 0, 0, 1)
	End Function

	''' <summary>
	''' Checks whether a date is equal to a date created with <see cref="DateFunctions.EmptyDate"/>.
	''' </summary>
	''' <param name="dDate">The date to check.</param>
	Public Shared Function IsEmptyDate(ByVal dDate As Date) As Boolean
		Return dDate.Date <= EmptyDate().Date
	End Function

	''' <summary>
	''' Checks the string month in the format mmm and returns a matching integer value
	''' </summary>
	''' <param name="sMonth">The month to check.</param>
	''' <exception cref="System.Exception">Can throw a month not recognised execption</exception>
	Public Shared Function GetMonthFromDisplayMonth(ByVal sMonth As String) As Integer

		Select Case sMonth.ToLower
			Case "jan"
				Return 1
			Case "feb"
				Return 2
			Case "mar"
				Return 3
			Case "apr"
				Return 4
			Case "may"
				Return 5
			Case "jun"
				Return 6
			Case "jul"
				Return 7
			Case "aug"
				Return 8
			Case "sep"
				Return 9
			Case "oct"
				Return 10
			Case "nov"
				Return 11
			Case "dec"
				Return 12
			Case Else
				Return 0
		End Select

	End Function

	''' <summary>
	''' Checks the integer month passed in and returns a matching mmm month string.
	''' </summary>
	''' <param name="iMonth">The integer month to convert.</param>
	''' <exception cref="System.Exception">Can throw an exeption if an invalid month integer is passed in</exception>
	<Obsolete("GetDisplayMonthFromNumber is obsolete, use DateAndTime.MonthName instead")>
	Public Shared Function GetDisplayMonthFromNumber(ByVal iMonth As Integer) As String
		Dim sMonth As String = ""
		If iMonth <= 12 AndAlso iMonth >= 1 Then
			Select Case iMonth
				Case 1
					sMonth = "Jan"
				Case 2
					sMonth = "Feb"
				Case 3
					sMonth = "Mar"
				Case 4
					sMonth = "Apr"
				Case 5
					sMonth = "May"
				Case 6
					sMonth = "Jun"
				Case 7
					sMonth = "Jul"
				Case 8
					sMonth = "Aug"
				Case 9
					sMonth = "Sep"
				Case 10
					sMonth = "Oct"
				Case 11
					sMonth = "Nov"
				Case 12
					sMonth = "Dec"
			End Select
		Else
			Throw New Exception("There are twelve months in the year, not " & iMonth.ToString)
		End If

		Return sMonth
	End Function

	''' <summary>
	''' Checks whether a given string is a display date
	''' </summary>
	''' <param name="sDate">The string to check.</param>
	Public Shared Function IsDisplayDate(ByVal sDate As String) As Boolean
		Try
			Dim dDate As Date = CType(sDate, Date)
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	''' <summary>
	''' Checks whether a given string is a valid date
	''' </summary>
	''' <param name="sDate">The string to check.</param>
	Public Shared Function IsDate(ByVal sDate As String) As Boolean
		If sDate = "" Then Return False
		Try
			Dim ukCulture As CultureInfo = New CultureInfo("en-GB")
			DateTime.Parse(sDate, ukCulture.DateTimeFormat)
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	''' <summary>
	''' Checks that the dates are valid, and that the start date if before the end date
	''' </summary>
	''' <param name="dStartDate">The start date.</param>
	''' <param name="dEndDate">The end date.</param>
	Public Shared Function ValidateDates(ByVal dStartDate As Date, ByVal dEndDate As Date) As FunctionReturn
		Dim oReturn As New FunctionReturn

		If DateFunctions.IsEmptyDate(dStartDate) Then
			oReturn.AddWarning("The Start Date must be a valid date")
		End If
		If DateFunctions.IsEmptyDate(dEndDate) Then
			oReturn.AddWarning("The End Date must be a valid date")
		End If

		If oReturn.Success Then
			If dEndDate < dStartDate Then
				oReturn.AddWarning("The End Date must be after the Start Date")
			End If
		End If

		Return oReturn

	End Function

	''' <summary>
	''' Checks that the dates are valid, that the start date is before the end date, and the dates are between the earliest start date and latest end date
	''' </summary>
	''' <param name="dStartDate">The start date.</param>
	''' <param name="dEndDate">The end date.</param>
	''' <param name="dEarliestStartDate">The earliest start date.</param>
	''' <param name="dLatestEndDate">The latest end date.</param>
	Public Shared Function ValidateDates(ByVal dStartDate As Date, ByVal dEndDate As Date,
		ByVal dEarliestStartDate As Date, ByVal dLatestEndDate As Date) As FunctionReturn
		Dim oReturn As New FunctionReturn

		If DateFunctions.IsEmptyDate(dStartDate) Then
			oReturn.AddWarning("The Start Date must be a valid date")
		End If
		If DateFunctions.IsEmptyDate(dEndDate) Then
			oReturn.AddWarning("The End Date must be a valid date")
		End If

		If oReturn.Success Then
			If dEndDate < dStartDate Then
				oReturn.AddWarning("The End Date must be after the Start Date")
			End If

			If Not (dStartDate >= dEarliestStartDate AndAlso dStartDate <= dLatestEndDate) Then
				oReturn.AddWarning("The Start Date must be between " &
					DateFunctions.DisplayDate(dEarliestStartDate) & " and " &
						DateFunctions.DisplayDate(dLatestEndDate))
			End If

			If Not (dEndDate >= dEarliestStartDate AndAlso dEndDate <= dLatestEndDate) Then
				oReturn.AddWarning("The End Date must be between " &
					DateFunctions.DisplayDate(dEarliestStartDate) & " and " &
						DateFunctions.DisplayDate(dLatestEndDate))
			End If
		End If

		Return oReturn

	End Function

	''' <summary>
	''' Validates date range and checks whether the supplied dates overlap with any existing entries in the table.
	''' The Table must have a StartDate Field and an EndDate Field.
	''' </summary>
	''' <param name="dStartDate">The start date.</param>
	''' <param name="dEndDate">The end date.</param>
	''' <param name="sParentTable">The parent table name.</param>
	''' <param name="iParentTableID">The parent table id.</param>
	''' <param name="sTable">The table name.</param>
	''' <param name="iTableID">The table id.</param>
	Public Shared Function ValidateDates(ByVal dStartDate As Date, ByVal dEndDate As Date,
		ByVal sParentTable As String, ByVal iParentTableID As Integer,
		ByVal sTable As String, ByVal iTableID As Integer) As FunctionReturn

		Dim oReturn As New FunctionReturn

		If DateFunctions.IsEmptyDate(dStartDate) Then
			oReturn.AddWarning("The Start Date must be a valid date")
		End If
		If DateFunctions.IsEmptyDate(dEndDate) Then
			oReturn.AddWarning("The End Date must be a valid date")
		End If

		If oReturn.Success Then

			If dEndDate < dStartDate Then
				oReturn.AddWarning("The End Date must be after the Start Date")
			End If

			Dim sb As New StringBuilder
			With sb
				.Append("Select Count(*) from ").Append(sTable)
				.Append(" where ").Append(sParentTable).Append("ID=").Append(iParentTableID)
				.Append(" and (").Append(SQL.GetSqlValue(dStartDate, SQL.SqlValueType.Date))
				.Append(" between StartDate and EndDate ")
				.Append(" or ").Append(SQL.GetSqlValue(dEndDate, SQL.SqlValueType.Date))
				.Append(" between StartDate and EndDate)")
				.Append(" and ").Append(sTable).Append("ID<>").Append(iTableID)
			End With

			Dim iOverlapCount As Integer = SQL.ExecuteSingleValue(sb.ToString)
			If iOverlapCount > 0 Then
				oReturn.AddWarning("The Dates you have input overlap with existing dates")
			End If

		End If

		Return oReturn

	End Function

	''' <summary>
	''' Checks that the start date and end date are within the Earliest Start Date and Latest end date,
	''' then checks whether the supplied dates overlap with any existing entries in the table.
	''' </summary>
	''' <param name="dStartDate">The start date.</param>
	''' <param name="dEndDate">The end date.</param>
	''' <param name="dEarliestStartDate">The earliest start date.</param>
	''' <param name="dLatestEndDate">The latest end date.</param>
	''' <param name="sParentTable">The parent table.</param>
	''' <param name="iParentTableID">The parent table identifier.</param>
	''' <param name="sTable">The table, must have a StartDate and an EndDate field.</param>
	''' <param name="iTableID">The table identifier.</param>
	Public Shared Function ValidateDates(ByVal dStartDate As Date, ByVal dEndDate As Date,
		ByVal dEarliestStartDate As Date, ByVal dLatestEndDate As Date,
		ByVal sParentTable As String, ByVal iParentTableID As Integer,
		ByVal sTable As String, ByVal iTableID As Integer) As FunctionReturn

		'do bounds check
		Dim oReturn As FunctionReturn = ValidateDates(dStartDate, dEndDate, dEarliestStartDate, dLatestEndDate)

		'if all well, the do overslap check
		If oReturn.Success Then
			oReturn = ValidateDates(dStartDate, dEndDate, sParentTable, iParentTableID,
				sTable, iTableID)
		End If

		Return oReturn

	End Function

	''' <summary>
	''' Gets the ordinal of the provided number, e.g. st, nd, rd
	''' </summary>
	''' <param name="iNumber">The number.</param>
	Public Shared Function GetOrdinal(ByVal iNumber As Integer) As String

		Dim sOrdinal As String = ""

		If CType(iNumber, String).Length > 2 Then
			Dim iEndNum As Integer = CType(CType(iNumber, String).
				Substring(CType(iNumber, String).Length - 2, 2), Integer)
			If iEndNum >= 11 And iEndNum <= 13 Then
				Select Case iEndNum
					Case 11, 12, 13
						sOrdinal = "th"
				End Select
			End If
		End If

		If iNumber >= 21 Then
			' Handles 21st, 22nd, 23rd, et al
			Select Case CType(iNumber.ToString.Substring(
				iNumber.ToString.Length - 1, 1), Integer)
				Case 1
					sOrdinal = "st"
				Case 2
					sOrdinal = "nd"
				Case 3
					sOrdinal = "rd"
				Case 0, 4 To 9
					sOrdinal = "th"
			End Select
		Else
			' Handles 1st to 20th
			Select Case iNumber
				Case 1
					sOrdinal = "st"
				Case 2
					sOrdinal = "nd"
				Case 3
					sOrdinal = "rd"
				Case 4 To 20
					sOrdinal = "th"
			End Select
		End If

		Return sOrdinal
	End Function


	''' <summary>
	''' Converts the provided date to a SQL date format, with or without the time.
	''' </summary>
	''' <param name="dDate">The d date.</param>
	''' <param name="bDateOnly">if set to <c>true</c>, will only return the date.</param>
	Public Shared Function SQLDateFormat(ByVal dDate As DateTime, Optional ByVal bDateOnly As Boolean = True) As String

		If bDateOnly Then
			Return String.Format("{0}-{1}-{2}T00:00:00.0000000-00:00", dDate.Year, dDate.ToString("MM"),
				dDate.ToString("dd"))
		Else
			Return String.Format("{0}-{1}-{2}T{3}:{4}:{5}.0000000-00:00", dDate.Year, dDate.ToString("MM"),
				dDate.ToString("dd"), dDate.ToString("HH"), dDate.ToString("mm"),
				dDate.ToString("ss"))
		End If

	End Function

	''' <summary>
	''' Returns the next date for the specified day of week.
	''' </summary>
	''' <param name="dDate">The date.</param>
	''' <param name="iDayOfWeek">The day of week.</param>
	Public Shared Function NextDayOfWeek(ByVal dDate As Date, ByVal iDayOfWeek As System.DayOfWeek) As Date

		'get current day
		Dim iDay As Integer = dDate.DayOfWeek
		If iDay <= iDayOfWeek Then
			Return dDate.AddDays(iDayOfWeek - iDay)
		Else
			Return dDate.AddDays(7 - (iDay - iDayOfWeek))
		End If

	End Function

	''' <summary>
	''' Checks that the month and year from a string in the format mm/yy are valid.
	''' </summary>
	''' <param name="sMonthYear">The month year string, format of mm/yy.</param>
	Public Shared Function IsValidMonthYear(ByVal sMonthYear As String) As Boolean

		'split the values and check we've got em
		Dim sValues() As String = sMonthYear.Split("/"c)
		If sValues.Length <> 2 Then
			Return False
		End If

		'get the month and year out and check em
		Dim iMonth As Integer = SafeInt(sValues(0))
		Dim iYear As Integer = SafeInt(sValues(1))
		If iMonth < 1 OrElse iMonth > 12 Then
			Return False
		End If

		If iYear < 1 OrElse iYear > 99 Then
			Return False
		End If

		Return True

	End Function

	''' <summary>
	''' Returns the hours from a time string
	''' </summary>
	''' <param name="sTime">The time string, splits on :, hours is first segment, e.g. hh:mm:ss.</param>
	Public Shared Function GetHoursFromTimeString(ByVal sTime As String) As Integer

		If sTime = "" Then
			Return 0
		Else
			Return SafeInt(sTime.Split(":"c)(0))
		End If
	End Function

	''' <summary>
	''' Returns the minutes from a time string
	''' </summary>
	''' <param name="sTime">The time string, splits on :, minutes is second segment, e.g. hh:mm:ss.</param>
	Public Shared Function GetMinutesFromTimeString(ByVal sTime As String) As Integer

		If sTime = "" Then
			Return 0
		Else
			Return SafeInt(sTime.Split(":"c)(1))
		End If
	End Function

	Public Shared Function GetSecondsFromTimeString(ByVal sTime As String) As Integer

		If sTime = "" OrElse sTime.Split(":"c).Length < 3 Then
			Return 0
		Else
			Return SafeInt(sTime.Split(":"c)(2))
		End If
	End Function

	''' <summary>
	''' Returns the minimum date of 2 provided dates.
	''' </summary>
	''' <param name="Date1">First date.</param>
	''' <param name="Date2">Second date</param>
	Public Shared Function MinDate(ByVal Date1 As Date, ByVal Date2 As Date) As Date
		Return IIf(Date1 <= Date2, Date1, Date2)
	End Function

	''' <summary>
	''' Returns the max date of 2 provided dates
	''' </summary>
	''' <param name="Date1">First Date</param>
	''' <param name="Date2">Second Date</param>
	Public Shared Function MaxDate(ByVal Date1 As Date, ByVal Date2 As Date) As Date
		Return IIf(Date1 >= Date2, Date1, Date2)
	End Function

	''' <summary>
	''' Gets age in years from provided date of birth.
	''' </summary>
	''' <param name="dDateOfBirth">The date of birth.</param>
	Public Shared Function GetAgeFromDateOfBirth(ByVal dDateOfBirth As Date) As Integer
		Return DateFunctions.GetAgeAtTargetDate(dDateOfBirth, Date.Now)
	End Function

	''' <summary>
	''' Gets the age in years at target date.
	''' </summary>
	''' <param name="dDateOfBirth">The date of birth.</param>
	''' <param name="dTargetDate">The target date.</param>
	Public Shared Function GetAgeAtTargetDate(ByVal dDateOfBirth As Date, ByVal dTargetDate As Date) As Integer

		If DateFunctions.IsEmptyDate(dDateOfBirth) Then Return 0
		If DateFunctions.IsEmptyDate(dTargetDate) Then Return 0

		Dim iYearDiff As Integer = dTargetDate.Year - dDateOfBirth.Year
		If dDateOfBirth.AddYears(iYearDiff) > dTargetDate Then
			Return iYearDiff - 1
		Else
			Return iYearDiff
		End If

	End Function

	''' <summary>
	''' Checks that a dates day of the week matches the specified days represented with booleans.
	''' </summary>
	''' <param name="CheckDate">The check date.</param>
	''' <param name="Sun">if set to <c>true</c> will return true if the day of week is Sunday.</param>
	''' <param name="Mon">if set to <c>true</c> will return true if the day of week is Monday.</param>
	''' <param name="Tue">if set to <c>true</c> will return true if the day of week is Tuesday.</param>
	''' <param name="Wed">if set to <c>true</c> will return true if the day of week is Wednesday.</param>
	''' <param name="Thu">if set to <c>true</c> will return true if the day of week is Thursday.</param>
	''' <param name="Fri">if set to <c>true</c> will return true if the day of week is Friday.</param>
	''' <param name="Sat">if set to <c>true</c> will return true if the day of week is Saturday.</param>
	Public Shared Function DayMatch(CheckDate As DateTime, Sun As Boolean, Mon As Boolean, Tue As Boolean,
	 Wed As Boolean, Thu As Boolean, Fri As Boolean, Sat As Boolean) As Boolean

		Dim eDatePart As DayOfWeek = CheckDate.DayOfWeek
		Return (eDatePart = DayOfWeek.Sunday AndAlso Sun) OrElse
		 (eDatePart = DayOfWeek.Monday AndAlso Mon) OrElse
		 (eDatePart = DayOfWeek.Tuesday AndAlso Tue) OrElse
		 (eDatePart = DayOfWeek.Wednesday AndAlso Wed) OrElse
		 (eDatePart = DayOfWeek.Thursday AndAlso Thu) OrElse
		 (eDatePart = DayOfWeek.Friday AndAlso Fri) OrElse
		 (eDatePart = DayOfWeek.Saturday AndAlso Sat)

	End Function

	''' <summary>
	''' Checks the days of week of a range of dates and returns true if one of them matches the specified days represented with booleans.
	''' </summary>
	''' <param name="StartDate">The start date.</param>
	''' <param name="EndDate">The end date.</param>
	''' <param name="Sun">if set to <c>true</c> [sun].</param>
	''' <param name="Mon">if set to <c>true</c> [mon].</param>
	''' <param name="Tue">if set to <c>true</c> [tue].</param>
	''' <param name="Wed">if set to <c>true</c> [wed].</param>
	''' <param name="Thu">if set to <c>true</c> [thu].</param>
	''' <param name="Fri">if set to <c>true</c> [fri].</param>
	''' <param name="Sat">if set to <c>true</c> [sat].</param>
	Public Shared Function DayMatchRange(StartDate As DateTime, EndDate As DateTime, Sun As Boolean, Mon As Boolean,
	 Tue As Boolean, Wed As Boolean, Thu As Boolean, Fri As Boolean, Sat As Boolean) As Boolean

		Dim bReturn As Boolean = False
		Dim dDate As DateTime = StartDate

		While dDate <= EndDate

			Dim eDatePart As DayOfWeek = dDate.DayOfWeek

			bReturn = bReturn OrElse (eDatePart = DayOfWeek.Sunday AndAlso Sun) OrElse
			 (eDatePart = DayOfWeek.Monday AndAlso Mon) OrElse
			 (eDatePart = DayOfWeek.Tuesday AndAlso Tue) OrElse
			 (eDatePart = DayOfWeek.Wednesday AndAlso Wed) OrElse
			 (eDatePart = DayOfWeek.Thursday AndAlso Thu) OrElse
			 (eDatePart = DayOfWeek.Friday AndAlso Fri) OrElse
			 (eDatePart = DayOfWeek.Saturday AndAlso Sat)

			dDate = dDate.AddDays(1)

		End While

		Return bReturn

	End Function

	Public Shared Function CalculateDateOverlap(startDate1 As DateTime, endDate1 As DateTime, startDate2 As DateTime, endDate2 As DateTime) As Integer

		Dim rangeStart As DateTime = DateFunctions.MaxDate(startDate1, startDate2)
		Dim rangeEnd As DateTime = DateFunctions.MinDate(endDate1, endDate2)

		'if any date bands are empty Or they don't overlap return 0
		If startDate1 > endDate1 OrElse startDate2 > endDate2 OrElse startDate1 > endDate2 OrElse startDate2 > endDate1 Then
			Return 0
		End If

		Return rangeEnd.Subtract(rangeStart).Days + 1

	End Function

#Region "End and start dates"

	''' <summary>
	''' Returns a date representing the first day of the month
	''' </summary>
	''' <param name="dDate">The date.</param>
	Public Shared Function MonthStart(ByVal dDate As Date) As Date

		Dim sDate As String = 1 & "/" & dDate.Month & "/" & dDate.Year

		'Parse the date as UK format
		Dim ukCulture As CultureInfo = New CultureInfo("en-GB")
		Return DateTime.Parse(sDate, ukCulture.DateTimeFormat)
	End Function

	''' <summary>
	''' Returns a date representing the last day of the month
	''' </summary>
	''' <param name="dDate">The d date.</param>
	Public Shared Function MonthEnd(ByVal dDate As Date) As Date
		Dim iDay As Integer = Date.DaysInMonth(dDate.Year, dDate.Month)
		Dim sDate As String = iDay & "/" & dDate.Month & "/" & dDate.Year

		'Parse the date as UK format
		Dim ukCulture As CultureInfo = New CultureInfo("en-GB")
		Return DateTime.Parse(sDate, ukCulture.DateTimeFormat)

	End Function

#End Region

#Region "Integer Date"

	''' <summary>
	''' Converts a number date string to a date object.
	''' </summary>
	''' <param name="sDate">The date string, format ddmmyy.</param>
	''' <exception cref="System.Exception">The date you supplied is the wrong length. It needs to be in the format 010101</exception>
	Public Shared Function NumberDateToDate(ByVal sDate As String) As Date

		If Not sDate.Length = 6 Then
			Throw New Exception("The date you supplied is the wrong length. It needs to be in the format 010101")
		End If

		Dim iDay As Integer = Functions.SafeInt(sDate.Substring(0, 2))
		Dim iMonth As Integer = Functions.SafeInt(sDate.Substring(2, 2))
		Dim iYear As Integer = Functions.SafeInt("20" + sDate.Substring(4, 2))

		Dim dReturnDate As New Date(iYear, iMonth, iDay)
		Return dReturnDate

	End Function

#End Region

#Region "validate datebands"

	''' <summary>
	''' Validates datebands for a grid of dates
	''' </summary>
	''' <param name="aGridData">The data grid.</param>
	''' <param name="bContiguousDates">if set to <c>true</c>, checks that there are no gaps between the date bands.</param>
	''' <param name="dLowerDateBound">The lower date bound to check against.</param>
	''' <param name="dUpperDateBound">The upper date bound to check against.</param>
	''' <param name="bDateRangeCovered">if set to <c>true</c> checks that the range between the LowerDateBound and UpperDateBound is covered.</param>
	''' <param name="bCheckForOverlap">if set to <c>true</c>, checks that the date's don't overlap.</param>
	''' <param name="sStartDateColumnName">Name of the start date column.</param>
	''' <param name="sEndDateColumnName">Name of the end date column.</param>
	Public Shared Function ValidateDatebands(ByVal aGridData As ArrayList,
	 Optional ByVal bContiguousDates As Boolean = True,
	 Optional ByVal dLowerDateBound As Date = #1/1/1900#,
	 Optional ByVal dUpperDateBound As Date = #1/1/1900#,
	 Optional ByVal bDateRangeCovered As Boolean = False,
	 Optional ByVal bCheckForOverlap As Boolean = False,
	 Optional ByVal sStartDateColumnName As String = "Start",
	 Optional ByVal sEndDateColumnName As String = "End") As ArrayList

		Dim aWarnings As New ArrayList

		'validate dates
		Dim iRow As Integer
		Dim dLastEndDate As Date = Nothing
		Dim dEarliestStartDate As Date = New Date(1900, 1, 1)
		Dim dLatestEndDate As Date = New Date(1900, 1, 1)

		For Each oGridRow As WebControls.Grid.GridRow In aGridData

			Dim oStartDate As Object = oGridRow(sStartDateColumnName)
			Dim oEndDate As Object = oGridRow(sEndDateColumnName)
			Dim bValidDates As Boolean = True

			'check valid dates
			If Not DateFunctions.IsDisplayDate(oStartDate.ToString) Then
				aWarnings.Add(String.Format("The Start Date on Row {0} is not a valid date", iRow + 1))
				bValidDates = False
			End If
			If Not DateFunctions.IsDisplayDate(oEndDate.ToString) Then
				aWarnings.Add(String.Format("The End Date on Row {0} is not a valid date", iRow + 1))
				bValidDates = False
			End If

			'if dates valid then check enddate>startdate
			If bValidDates Then
				If DateFunctions.DisplayDateToDate(oStartDate.ToString) >
				 DateFunctions.DisplayDateToDate(oEndDate.ToString) Then
					aWarnings.Add("The Start Date must be before the End Date on Row " & (iRow + 1))
				End If
			End If

			'if dates valid and lower and upper date boundaries have been specified then
			'check 'em
			If bValidDates AndAlso dLowerDateBound.Year > 1990 Then
				If DateFunctions.DisplayDateToDate(oStartDate.ToString) < dLowerDateBound Then
					aWarnings.Add("The Start Date cannot be before " & DateFunctions.DisplayDate(dLowerDateBound) &
					 " on Row " & (iRow + 1))
				End If
			End If

			If bValidDates AndAlso dUpperDateBound.Year > 1990 Then
				If DateFunctions.DisplayDateToDate(oEndDate.ToString) > dUpperDateBound Then
					aWarnings.Add("The End Date cannot be later than " & DateFunctions.DisplayDate(dUpperDateBound) &
					 " on Row " & (iRow + 1))
				End If
			End If

			'if contiguous dates then make sure that the startdate=last enddate+1
			If bContiguousDates AndAlso bValidDates AndAlso Not dLastEndDate = Nothing Then

				Dim oSpan As TimeSpan
				oSpan = DateFunctions.DisplayDateToDate(oStartDate.ToString).Subtract(dLastEndDate)
				If oSpan.Days <> 1 Then
					aWarnings.Add(String.Format(
					 "The Start Date on Row {0} must be the date after the End Date on Row {1}",
					 iRow + 1, iRow))
				End If
			End If

			'store the earliest start date if irow=0 and bdaterangecovered=true
			If bValidDates AndAlso iRow = 0 AndAlso bDateRangeCovered Then
				dEarliestStartDate = DateFunctions.DisplayDateToDate(oStartDate.ToString)
			End If

			'store the latest end date if iRow=aPostValues.GetUpperBound(0)
			If bValidDates AndAlso iRow = aGridData.Count - 1 AndAlso bDateRangeCovered Then
				dLatestEndDate = DateFunctions.DisplayDateToDate(oEndDate.ToString)
			End If

			'store the last end date if the dates are ok
			If bValidDates Then
				dLastEndDate = DateFunctions.DisplayDateToDate(oEndDate.ToString)
			End If
		Next

		'do date range covered
		If bDateRangeCovered AndAlso aGridData.Count > 0 Then
			If dEarliestStartDate <> dLowerDateBound OrElse dLatestEndDate <> dUpperDateBound Then
				aWarnings.Add(String.Format("The dates must cover the period from {0} to {1}",
				 DateFunctions.DisplayDate(dLowerDateBound), DateFunctions.DisplayDate(dUpperDateBound)))
			End If
		End If

		'dates overlap - only if other checks have passed
		'otherwise we have all sorts of trouble
		If bCheckForOverlap AndAlso aWarnings.Count = 0 AndAlso aGridData.Count > 0 Then
			Dim iCheckRow As Integer
			Dim dStartDate As Date
			Dim dEndDate As Date
			Dim dCheckStartDate As Date
			Dim dCheckEndDate As Date

			For iRow = 0 To aGridData.Count - 1
				dStartDate = DateFunctions.SafeDate(CType(aGridData(iRow), WebControls.Grid.GridRow)(sStartDateColumnName))
				dEndDate = DateFunctions.SafeDate(CType(aGridData(iRow), WebControls.Grid.GridRow)(sEndDateColumnName))

				For iCheckRow = iRow + 1 To aGridData.Count - 1
					dCheckStartDate = DateFunctions.SafeDate(CType(aGridData(iCheckRow), WebControls.Grid.GridRow)(sStartDateColumnName))
					dCheckEndDate = DateFunctions.SafeDate(CType(aGridData(iCheckRow), WebControls.Grid.GridRow)(sEndDateColumnName))

					If DatesOverlap(dStartDate, dEndDate, dCheckStartDate, dCheckEndDate) Then
						aWarnings.Add(String.Format("The Dates on Row {0} overlap with those on Row {1}",
						 iCheckRow + 1, iRow + 1))
					End If
				Next
			Next
		End If
		Return aWarnings
	End Function

	'datesoverlap
	''' <summary>
	''' Checks whether 2 date ranges overlap
	''' </summary>
	''' <param name="dStartDate1">The start date of the first range.</param>
	''' <param name="dEndDate1">The end date of the first range.</param>
	''' <param name="dStartDate2">The start date of the second range.</param>
	''' <param name="dEndDate2">The end date of the second range.</param>
	Public Shared Function DatesOverlap(ByVal dStartDate1 As Date,
	 ByVal dEndDate1 As Date, ByVal dStartDate2 As Date, ByVal dEndDate2 As Date) As Boolean

		Return (dStartDate2 >= dStartDate1 AndAlso dStartDate2 <= dEndDate1) _
		 OrElse (dEndDate2 >= dStartDate1 AndAlso dEndDate2 <= dEndDate1) _
		 OrElse (dStartDate2 <= dStartDate1 AndAlso dEndDate2 >= dEndDate1)
	End Function

#End Region

#Region "Date Age"

	''' <summary>
	''' Compares the BaseDate to the ComparedTo date and returns the difference between them as a string
	''' </summary>
	''' <param name="BaseDate">The base date.</param>
	''' <param name="ComparedTo">The compared to, defaults to current date</param>
	''' <exception cref="System.Exception">Base date is in the future</exception>
	Public Shared Function DateAge(ByVal BaseDate As Date, Optional ByVal ComparedTo As Date = Nothing) As String

		'default
		If ComparedTo = Nothing Then ComparedTo = Date.Now

		Dim oTimespan As System.TimeSpan = ComparedTo.Subtract(BaseDate)
		Dim sPlural1 As String = "s"
		Dim sPlural2 As String = "s"

		If oTimespan.TotalMinutes < 0 Then
			Throw New Exception("Base date is in the future")
		ElseIf oTimespan.TotalMinutes < 60 Then

			If oTimespan.Minutes = 1 Then sPlural1 = ""
			Return String.Format("{0} minute{1} ago", oTimespan.Minutes.ToString, sPlural1)

		ElseIf oTimespan.TotalHours < 24 Then

			If oTimespan.Hours = 1 Then sPlural1 = ""
			Return String.Format("{0} hour{1} ago", oTimespan.Hours.ToString, sPlural1)

		ElseIf oTimespan.Days < 7 Then

			If oTimespan.Days = 1 Then sPlural1 = ""
			If oTimespan.Hours = 1 Then sPlural2 = ""
			Return String.Format("{0} day{1} and {2} hour{3} ago", oTimespan.Days, sPlural1, oTimespan.Hours, sPlural2)

		ElseIf oTimespan.Days < 30 Then

			If Math.Truncate(oTimespan.TotalDays / 7) = 1 Then sPlural1 = ""
			If Math.Round(oTimespan.TotalDays - Math.Truncate(oTimespan.TotalDays / 7) * 7, 0) = 1 Then sPlural2 = ""
			Return String.Format("{0} week{1} and {2} day{3} ago",
				  Math.Truncate(oTimespan.TotalDays / 7), sPlural1,
				  Math.Round(oTimespan.TotalDays - Math.Truncate(oTimespan.TotalDays / 7) * 7, 0), sPlural2)

		Else
			If Math.Round(oTimespan.TotalDays / 7, 0) = 1 Then sPlural1 = ""
			Return String.Format("{0} week{1} ago", Math.Round(oTimespan.TotalDays / 7, 0), sPlural1)
		End If

	End Function

#End Region

#Region "Time Functions"

	'takes a time and date and appends them
	''' <summary>
	''' Adds hours and minutes from specified time to a date
	''' </summary>
	''' <param name="dDate">The date.</param>
	''' <param name="sTime">The time, format of hh:mm.</param>
	Public Shared Function AddTimeToDate(ByVal dDate As Date, ByVal sTime As String) As Date

		dDate = dDate.AddHours(DateFunctions.GetHoursFromTimeString(sTime))
		dDate = dDate.AddMinutes(DateFunctions.GetMinutesFromTimeString(sTime))
		dDate = dDate.AddSeconds(DateFunctions.GetSecondsFromTimeString(sTime))

		Return dDate

	End Function


	''' <summary>
	''' Takes a time of day and then adds the duration of the subsequent time. so 12:00 + 01:30 = 13:30
	''' </summary>
	''' <param name="sStartTime">The start time.</param>
	''' <param name="aDurations">List of durations to be added.</param>
	Public Shared Function AddTimeToTime(ByVal sStartTime As String, ByVal ParamArray aDurations() As String) As String

		Dim dDate As Date = AddTimeToDate(Date.Now.Date, sStartTime)
		For Each sDuration As String In aDurations
			dDate = AddTimeToDate(dDate, sDuration)
		Next

		Return TimeFromDate(dDate)
	End Function

	'takes a time and date and appends them
	''' <summary>
	''' Subtracts hours and minutes from specified time to a date
	''' </summary>
	''' <param name="dDate">The date.</param>
	''' <param name="sTime">The time, format hh:mm.</param>
	Public Shared Function SubtractTimeFromDate(ByVal dDate As Date, ByVal sTime As String) As Date

		dDate = dDate.AddHours(-1 * DateFunctions.GetHoursFromTimeString(sTime))
		dDate = dDate.AddMinutes(-1 * DateFunctions.GetMinutesFromTimeString(sTime))
		dDate = dDate.AddSeconds(-1 * DateFunctions.GetSecondsFromTimeString(sTime))

		Return dDate

	End Function

	''' <summary>
	''' Takes a time of day and then adds the duration of the subsequent time. so 12:00 + 01:30 = 13:30
	''' </summary>
	''' <param name="sStartTime">The start time.</param>
	''' <param name="aDurations">List of durations to be subtracted.</param>
	Public Shared Function SubtractTimeFromTime(ByVal sStartTime As String, ByVal ParamArray aDurations() As String) As String

		Dim dDate As Date = AddTimeToDate(Date.Now.Date, sStartTime)
		For Each sDuration As String In aDurations
			dDate = SubtractTimeFromDate(dDate, sDuration)
		Next

		Return TimeFromDate(dDate)
	End Function

	'time from date
	''' <summary>
	''' Returns the time from a date
	''' </summary>
	''' <param name="dDate">The date to get the time from.</param>
	Public Shared Function TimeFromDate(ByVal dDate As Date) As String

		Return dDate.ToString("HH:mm")

	End Function

	''' <summary>
	''' Checks whether a time is between two other times.
	''' </summary>
	''' <param name="Time">The time to check.</param>
	''' <param name="StartTime">The start time.</param>
	''' <param name="EndTime">The end time.</param>
	Public Shared Function InTimeRange(ByVal Time As String, ByVal StartTime As String, ByVal EndTime As String) As Boolean

		Dim dDate As Date = DateFunctions.AddTimeToDate(Date.Now.Date, Time)
		Dim dStartDate As Date = DateFunctions.AddTimeToDate(Date.Now.Date, StartTime)
		Dim dEndDate As Date = DateFunctions.AddTimeToDate(Date.Now.Date, EndTime)

		Return dDate >= dStartDate AndAlso dDate <= dEndDate

	End Function

#End Region

End Class