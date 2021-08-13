Imports Intuitive.Functions

Namespace LargeData

	''' <summary>
	''' Class containing functions to perform on large sets of data
	''' </summary>
	''' <remarks>
	''' This includes functions and classes that are intended to be useful when dealing with large amounts of data
	''' I find myself writing more and more of these and wanting to use them all over the place, so I've put them in here for the benefit of all. -TK
	''' Some of these are really tricky to get to grips with, and some are easy to understand and use.
	''' </remarks>
	Public Class LargeDataFunctions

#Region "Merge Delete Algorithm"

		'This is a useful little algorithm for slimming down a list based on the items from another list.
		'It's very very fast because it scans through both lists at the same time, and it only scans through them once.
		'It is also able to delete consecutive chunks of the list, rather than one item at a time.
		'
		'It is CONSIDERABLY faster than nesting two for each loops or using one for each loop in conjustion with List.Contains()
		'I am talking thousands of times faster.
		'
		'It relies on the sort order of the two lists being the same.
		'If this is not the case, there is an option to sort either or both or the lists in advance.
		'But, be warned, that this will alter the order of the orginal lists.
		'
		'
		'A typical example of usage would be that you have a huge list of properties returned by a third party, and before doing
		'any calculations or sending them to the database, you want to check that the IDs are contained within a second list
		'that you have obtained with a database query.
		'
		'
		'Here is a real example of the function being used in such a circumstance:
		'
		'   MergeDelete(oHotelAvailabilities, oMasterIDs, Function(oHotelAvailability) oHotelAvailability.ServiceID)
		'
		'This is deleting any 'HotelAvailability' objects from the oHotelAvailabilities list where the 'ServiceID' is not
		'in the list of master IDs (oMasterIDs)
		'
		'
		'Have fun! :)

		''' <summary>
		''' Compares and merges 2 lists containing the same type of object
		''' </summary>
		''' <typeparam name="tComparisonObjectType">The type of the comparison object type.</typeparam>
		''' <param name="ListToDeleteFrom">The list to delete from.</param>
		''' <param name="ListToCompareWith">The list to compare with.</param>
		''' <param name="SortListToDeleteFrom">if set to <c>true</c> sort the list to delete from.</param>
		''' <param name="SortComparisonList">if set to <c>true</c> sort the comparison list.</param>
		''' <param name="Comparer">The comparer.</param>
		Public Shared Sub MergeDelete(Of tComparisonObjectType)(
			ByRef ListToDeleteFrom As Generic.List(Of tComparisonObjectType),
			ByRef ListToCompareWith As Generic.List(Of tComparisonObjectType),
			Optional ByVal SortListToDeleteFrom As Boolean = False,
			Optional ByVal SortComparisonList As Boolean = False,
			Optional ByVal Comparer As System.Comparison(Of tComparisonObjectType) = Nothing)

			MergeDelete(ListToDeleteFrom, ListToCompareWith, Function(oObject) oObject, Function(oObject) oObject,
				SortListToDeleteFrom, SortComparisonList, Comparer)

		End Sub

		''' <summary>
		''' Merges 2 lists with different object types
		''' When you have one list of the comparison object you want to delete from, but the second list is more complicated
		''' </summary>
		''' <typeparam name="tObject2Type">The type of the object2 type.</typeparam>
		''' <typeparam name="tComparisonObjectType">The type of the comparison object type.</typeparam>
		''' <param name="ListToDeleteFrom">The list to delete from.</param>
		''' <param name="ListToCompareWith">The list to compare with.</param>
		''' <param name="ComparisonListConvertor">The comparison list convertor.</param>
		''' <param name="SortListToDeleteFrom">if set to <c>true</c> sort the list to delete from.</param>
		''' <param name="SortComparisonList">if set to <c>true</c> sort the comparison list.</param>
		''' <param name="Comparer">The comparer.</param>
		Public Shared Sub MergeDelete(Of tObject2Type, tComparisonObjectType)(
			ByRef ListToDeleteFrom As Generic.List(Of tComparisonObjectType),
			ByRef ListToCompareWith As Generic.List(Of tObject2Type),
			ByVal ComparisonListConvertor As System.Converter(Of tObject2Type, tComparisonObjectType),
			Optional ByVal SortListToDeleteFrom As Boolean = False,
			Optional ByVal SortComparisonList As Boolean = True,
			Optional ByVal Comparer As System.Comparison(Of tComparisonObjectType) = Nothing)

			MergeDelete(ListToDeleteFrom, ListToCompareWith, Function(oObject) oObject, ComparisonListConvertor,
				SortListToDeleteFrom, SortComparisonList, Comparer)

		End Sub

		''' <summary>
		''' Merges 2 lists when you have a second list of the comparison object you want to compare with, but the first list is more complicated
		''' </summary>
		''' <typeparam name="tObject1Type">The type of the object1 type.</typeparam>
		''' <typeparam name="tComparisonObjectType">The type of the comparison object type.</typeparam>
		''' <param name="ListToDeleteFrom">The list to delete from.</param>
		''' <param name="ListToCompareWith">The list to compare with.</param>
		''' <param name="ListToDeleteFromConvertor">The list to delete from convertor.</param>
		''' <param name="SortListToDeleteFrom">if set to <c>true</c> [sort list to delete from].</param>
		''' <param name="SortComparisonList">if set to <c>true</c> [sort comparison list].</param>
		''' <param name="Comparer">The comparer.</param>
		Public Shared Sub MergeDelete(Of tObject1Type, tComparisonObjectType)(
			ByRef ListToDeleteFrom As Generic.List(Of tObject1Type),
			ByRef ListToCompareWith As Generic.List(Of tComparisonObjectType),
			ByVal ListToDeleteFromConvertor As System.Converter(Of tObject1Type, tComparisonObjectType),
			Optional ByVal SortListToDeleteFrom As Boolean = True,
			Optional ByVal SortComparisonList As Boolean = False,
			Optional ByVal Comparer As System.Comparison(Of tComparisonObjectType) = Nothing)

			MergeDelete(ListToDeleteFrom, ListToCompareWith, ListToDeleteFromConvertor, Function(oObject) oObject,
				SortListToDeleteFrom, SortComparisonList, Comparer)

		End Sub

		''' <summary>
		''' Merge 2 lists and delete where appropriate.
		''' Basically, it does the equivalent of a delete where is null with a left merge join.
		''' </summary>
		''' <typeparam name="tObject1Type">The type of the object1 type.</typeparam>
		''' <typeparam name="tObject2Type">The type of the object2 type.</typeparam>
		''' <typeparam name="tComparisonObjectType">The type of the comparison object type.</typeparam>
		''' <param name="ListToDeleteFrom">The list to delete from.</param>
		''' <param name="ListToCompareWith">The list to compare with.</param>
		''' <param name="ListToDeleteFromConvertor">The list to delete from convertor.</param>
		''' <param name="ComparisonListConvertor">The comparison list convertor.</param>
		''' <param name="SortListToDeleteFrom">if set to <c>true</c> [sort list to delete from].</param>
		''' <param name="SortComparisonList">if set to <c>true</c> [sort comparison list].</param>
		''' <param name="Comparer">The comparer.</param>
		''' <remarks>
		''' Merge delete algorithm, courtesy of Tom Knight
		''' Basically, it does the equivalent of a delete where is null with a left merge join - this is *extremely* efficient and lightning fast!! ;)
		'''
		''' delete
		'''		from list1
		'''			left merge join list2
		'''				on list1.comparisonobject = list2.comparisonobject
		'''		where list2.comparisonobject is null
		'''
		''' It uses delegate functions to retrieve the comparison objects from each list, and a delegate comparer too.
		''' I recommend using it with lambda functions. :)
		'''
		''' This is actually very simple, but it will make your brain ache trying to figure out what the code means...
		''' </remarks>
		Public Shared Sub MergeDelete(Of tObject1Type, tObject2Type, tComparisonObjectType)(
			ByRef ListToDeleteFrom As Generic.List(Of tObject1Type),
			ByRef ListToCompareWith As Generic.List(Of tObject2Type),
			ByVal ListToDeleteFromConvertor As System.Converter(Of tObject1Type, tComparisonObjectType),
			ByVal ComparisonListConvertor As System.Converter(Of tObject2Type, tComparisonObjectType),
			Optional ByVal SortListToDeleteFrom As Boolean = True,
			Optional ByVal SortComparisonList As Boolean = True,
			Optional ByVal Comparer As System.Comparison(Of tComparisonObjectType) = Nothing)

			'Make sure we have a comparer
			If Comparer Is Nothing Then Comparer = Function(x, y) Generic.Comparer(Of tComparisonObjectType).Default.Compare(x, y)

			'Sort the lists first if required
			If SortListToDeleteFrom Then ListToDeleteFrom.Sort(Function(x, y) Comparer(ListToDeleteFromConvertor(x), ListToDeleteFromConvertor(y)))
			If SortComparisonList Then ListToCompareWith.Sort(Function(x, y) Comparer(ComparisonListConvertor(x), ComparisonListConvertor(y)))

			'Now, this sounds complicated, but for each comparison object in the second list, scan through the next bit of the first list
			'and delete any that don't match
			Dim iCurrentIndex As Integer
			Dim iScanIndex As Integer

			For Each oComparisonObject As tObject2Type In ListToCompareWith
				If iCurrentIndex < ListToDeleteFrom.Count Then

					'make sure we never go back and scan objects in the list that we have already scanned
					iScanIndex = iCurrentIndex

					'scan through any objects in the list that are between the last comparison object and the current comparison object, and delete them
					While iScanIndex < ListToDeleteFrom.Count AndAlso Comparer(ListToDeleteFromConvertor(ListToDeleteFrom(iScanIndex)), ComparisonListConvertor(oComparisonObject)) < 0
						iScanIndex += 1
					End While
					ListToDeleteFrom.RemoveRange(iCurrentIndex, iScanIndex - iCurrentIndex)

					'now scan through any objects in the list that are equal to the current comparison object, but DON'T delete them
					While iCurrentIndex < ListToDeleteFrom.Count AndAlso Comparer(ListToDeleteFromConvertor(ListToDeleteFrom(iCurrentIndex)), ComparisonListConvertor(oComparisonObject)) = 0
						iCurrentIndex += 1
					End While
				End If
			Next

			'finally, delete any objects in the first list that are left (as these will be greater than any of the comparison objects)
			If iCurrentIndex < ListToDeleteFrom.Count Then
				ListToDeleteFrom.RemoveRange(iCurrentIndex, ListToDeleteFrom.Count - iCurrentIndex)
			End If

		End Sub

#End Region

#Region "Dedupe Algorithm"

		'Simlar to the merge delete, it compares a list with itself and only keeps the first item it finds in each group.
		'It's aimed at being fast and effecient.
		'
		'Like the merge delete, it relies on the sort order of the list being such that identical objects are next to each other.
		'If this is not the case, there is an option to sort the list in advance.
		'But, be warned, that this will alter the order of the orginal list.

		'Helper 1 - when the list only contains items of the type you want to compare
		''' <summary>
		''' Removes duplicates from a list
		''' </summary>
		''' <typeparam name="tComparisonObjectType">The type of the comparison object type.</typeparam>
		''' <param name="ListToDeleteFrom">The list to delete from.</param>
		''' <param name="SortListToDeleteFrom">if set to <c>true</c> [sort list to delete from].</param>
		''' <param name="Comparer">The comparer.</param>
		Public Shared Sub Dedupe(Of tComparisonObjectType)(
			ByRef ListToDeleteFrom As Generic.List(Of tComparisonObjectType),
			Optional ByVal SortListToDeleteFrom As Boolean = False,
			Optional ByVal Comparer As System.Comparison(Of tComparisonObjectType) = Nothing)

			Dedupe(ListToDeleteFrom, Function(oObject) oObject, SortListToDeleteFrom, Comparer)

		End Sub

		'Dedupe algorithm, courtesy of Tom Knight
		'Does what it says, an equivalent in sql would be:
		'
		'with rankeditems as (
		'	select id, row_number() over(partition by value1, value2, value3, etc... order by id) as ranking
		'		from list
		')
		'delete from list
		'	inner join rankeditems on list.id = rankeditems.id
		'	where rankeditems.ranking != 1
		'
		'It uses a delegate function to retrieve the comparison object from the list, and a delegate comparer too.
		'I recommend using it with lambda functions. :)
		''' <summary>
		''' Removes duplicates from a list
		''' </summary>
		''' <typeparam name="tObjectType">The type of the object type.</typeparam>
		''' <typeparam name="tComparisonObjectType">The type of the comparison object type.</typeparam>
		''' <param name="ListToDeleteFrom">The list to delete from.</param>
		''' <param name="ListToDeleteFromConvertor">The list to delete from convertor.</param>
		''' <param name="SortListToDeleteFrom">if set to <c>true</c> [sort list to delete from].</param>
		''' <param name="Comparer">The comparer.</param>
		Public Shared Sub Dedupe(Of tObjectType, tComparisonObjectType)(
			ByRef ListToDeleteFrom As Generic.List(Of tObjectType),
			ByVal ListToDeleteFromConvertor As System.Converter(Of tObjectType, tComparisonObjectType),
			Optional ByVal SortListToDeleteFrom As Boolean = True,
			Optional ByVal Comparer As System.Comparison(Of tComparisonObjectType) = Nothing)

			'Make sure we have a comparer
			If Comparer Is Nothing Then Comparer = Function(x, y) Generic.Comparer(Of tComparisonObjectType).Default.Compare(x, y)

			'Sort the list first if required
			If SortListToDeleteFrom Then ListToDeleteFrom.Sort(Function(x, y) Comparer(ListToDeleteFromConvertor(x), ListToDeleteFromConvertor(y)))

			'Now dedupe
			Dim iCurrentIndex As Integer
			Dim iScanIndex As Integer

			While iCurrentIndex < ListToDeleteFrom.Count - 1 'no point checking the last one

				'make sure we never go back and scan objects in the list that we have already scanned
				iScanIndex = iCurrentIndex

				'scan through any objects in the list that are equal to the current object
				While iScanIndex + 1 < ListToDeleteFrom.Count AndAlso Comparer(ListToDeleteFromConvertor(ListToDeleteFrom(iCurrentIndex)), ListToDeleteFromConvertor(ListToDeleteFrom(iScanIndex + 1))) = 0
					iScanIndex += 1
				End While

				'delete them
				If iScanIndex - iCurrentIndex <> 0 Then
					ListToDeleteFrom.RemoveRange(iCurrentIndex + 1, iScanIndex - iCurrentIndex)
				End If

				'keep going
				iCurrentIndex += 1

			End While

		End Sub

#End Region

#Region "CSV Split Line"

		''' <summary>
		''' Split a line from a csv file
		''' If you have simple requirements, then it will use String.Split() which is very fast.
		''' If you have complex requirements, then it will split the line itself using its own algorithm.
		''' </summary>
		''' <param name="sLine">The s line.</param>
		''' <param name="aDelimiters">a delimiters.</param>
		''' <param name="aQuoteCharacters">a quote characters.</param>
		''' <param name="aEscapeCharacters">a escape characters.</param>
		''' <param name="bSortArrays">if set to <c>true</c> [b sort arrays].</param>
		''' <returns></returns>
		Public Shared Function CSVSplitLine(ByVal sLine As String, ByVal aDelimiters() As Char, Optional ByVal aQuoteCharacters() As Char = Nothing,
			Optional ByVal aEscapeCharacters() As Char = Nothing, Optional ByVal bSortArrays As Boolean = True) As String()

			If aQuoteCharacters Is Nothing Then aQuoteCharacters = New Char() {}
			If aEscapeCharacters Is Nothing Then aEscapeCharacters = New Char() {}

			'check the line is not nothing or empty
			If sLine Is Nothing OrElse sLine = "" Then Return New String() {}

			'if all we have is a simple set of delimiters, then do a simple String.Split, otherwise, do the full on thing
			If aQuoteCharacters.Length = 0 AndAlso aEscapeCharacters.Length = 0 Then

				Return sLine.Split(aDelimiters, StringSplitOptions.None)
			Else

				'if we're doing the full on thing, we need to make sure the arrays are sorted to start with
				If bSortArrays Then
					If aDelimiters IsNot Nothing Then Array.Sort(aDelimiters)
					If aQuoteCharacters IsNot Nothing Then Array.Sort(aQuoteCharacters)
					If aEscapeCharacters IsNot Nothing Then Array.Sort(aEscapeCharacters)
				End If

				'now we loop through each char, keeping track of whether to escape or quote characters, etc
				'oh yeah, and we split on the delimiters - quite important this!! ;)
				Dim sCurrentQuoteChar As String = ""
				Dim bWasQuoted As Boolean = False
				Dim bEscapeNextCharacter As Boolean = False
				Dim iLastSplitPoint As Integer = 0

				Dim oSplits As New Generic.List(Of String)

				For i As Integer = 0 To sLine.Length - 1
					If Not bEscapeNextCharacter AndAlso Array.BinarySearch(Of Char)(aEscapeCharacters, sLine(i)) >= 0 _
						AndAlso (Array.BinarySearch(Of Char)(aQuoteCharacters, sLine(i)) < 0 _
							OrElse ((sCurrentQuoteChar <> "" OrElse iLastSplitPoint <> i) AndAlso (sCurrentQuoteChar = "" _
								OrElse (sLine.Length > i + 1 AndAlso Array.BinarySearch(Of Char)(aDelimiters, sLine(i + 1)) < 0)))) Then

						bEscapeNextCharacter = True

					ElseIf Not bEscapeNextCharacter AndAlso sCurrentQuoteChar = "" AndAlso iLastSplitPoint = i _
						AndAlso Array.BinarySearch(Of Char)(aQuoteCharacters, sLine(i)) >= 0 Then

						sCurrentQuoteChar = sLine(i)

					ElseIf Not bEscapeNextCharacter AndAlso sCurrentQuoteChar = sLine(i) _
						AndAlso (sLine.Length = i + 1 OrElse Array.BinarySearch(Of Char)(aDelimiters, sLine(i + 1)) >= 0) _
						AndAlso Array.BinarySearch(Of Char)(aQuoteCharacters, sLine(i)) >= 0 Then

						sCurrentQuoteChar = ""
						bWasQuoted = True

					ElseIf Not bEscapeNextCharacter AndAlso sCurrentQuoteChar = "" _
						AndAlso Array.BinarySearch(Of Char)(aDelimiters, sLine(i)) >= 0 Then

						Dim iQuoteOffset As Integer = IIf(bWasQuoted, 1, 0)
						Dim sSplit As String = sLine.Substring(iLastSplitPoint + iQuoteOffset, i - iLastSplitPoint - iQuoteOffset * 2)

						'replace escape characters
						'doing it in a while loop like this has the useful property of skipping over the characters that come straight after each one
						Dim j As Integer = 0
						While j < sSplit.Length
							If Array.BinarySearch(Of Char)(aEscapeCharacters, sSplit(j)) >= 0 Then
								sSplit = String.Concat(sSplit.Substring(0, j), sSplit.Substring(j + 1))
							End If
							j += 1
						End While

						'add the split
						oSplits.Add(sSplit)
						iLastSplitPoint = i + 1
						bWasQuoted = False

					ElseIf bEscapeNextCharacter Then

						bEscapeNextCharacter = False

					End If
				Next

				'add the last split
				Dim iFinalQuoteOffset As Integer = IIf(bWasQuoted, 1, 0)
				oSplits.Add(sLine.Substring(iLastSplitPoint + iFinalQuoteOffset, sLine.Length - iLastSplitPoint - iFinalQuoteOffset * 2))

				'return
				Return oSplits.ToArray

			End If

		End Function

#End Region

#Region "CSV Join Line"

		''' <summary>
		''' Creates a CSV from an array.
		''' </summary>
		''' <param name="oValues">The values.</param>
		''' <param name="sDelimiter">The delimiter.</param>
		''' <param name="sQuoteCharacter">The quote character.</param>
		''' <param name="sEscapeCharacter">The escape character.</param>
		''' <returns></returns>
		Public Shared Function CSVJoinLine(ByVal oValues() As Object, Optional ByVal sDelimiter As Char = ","c,
				Optional ByVal sQuoteCharacter As Char = """"c, Optional ByVal sEscapeCharacter As Char = """"c) As String

			'get the escape sequences
			Dim sEscapedEscape As String = Nothing
			Dim sEscapedQuote As String = Nothing

			If sEscapeCharacter <> "" Then
				sEscapedEscape = String.Concat(sEscapeCharacter, sEscapeCharacter)
				If sQuoteCharacter <> "" AndAlso sQuoteCharacter <> sEscapeCharacter Then sEscapedQuote = String.Concat(sEscapeCharacter, sQuoteCharacter)
			End If

			'loop through and escape everything
			Dim sValues(oValues.Length - 1) As String

			For i As Integer = 0 To oValues.Length - 1
				sValues(i) = oValues(i).ToString

				If sEscapeCharacter <> "" Then
					sValues(i) = sValues(i).Replace(sEscapeCharacter, sEscapedEscape)
					If sQuoteCharacter <> "" AndAlso sQuoteCharacter <> sEscapeCharacter Then sValues(i) = sValues(i).Replace(sQuoteCharacter, sEscapedQuote)
				End If

				If sQuoteCharacter <> "" AndAlso sValues(i).Contains(sDelimiter) Then sValues(i) = String.Concat(sQuoteCharacter, sValues(i), sQuoteCharacter)
			Next

			'return the concatenated line
			Return String.Join(sDelimiter, sValues)

		End Function

#End Region

	End Class

End Namespace