Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.DateFunctions
Imports System.Text.RegularExpressions

''' <summary>
''' Class with functions for serialising and deserialising objects and xml documents
''' </summary>
Public Class Serializer

	Private Const sXmlHeader As String = "<?xml version=""1.0""?>"

#Region "Serialize"

	''' <summary>
	''' Serialises the specified object into an xml document.
	''' </summary>
	''' <param name="oObject">The object to serialise.</param>
	''' <param name="bAutoClean">if set to <c>true</c>, will remove document declarations from the xml before returning it.</param>
	Public Shared Function Serialize(ByVal oObject As Object, Optional ByVal bAutoClean As Boolean = False) As XmlDocument

		Dim oXMLSerializer As New XmlSerializer(oObject.GetType)

		Using oStream As New MemoryStream

			oXMLSerializer.Serialize(oStream, oObject)

			oStream.Position = 0

			Dim oXmlDocument As New XmlDocument
			oXmlDocument.Load(oStream)

			oStream.Close()
			oStream.Dispose()

			oXMLSerializer = Nothing

			If Not bAutoClean Then
				Return oXmlDocument
			Else
				Return Serializer.CleanXml(oXmlDocument, True)
			End If

		End Using

	End Function

#End Region

#Region "Deserialize"

	''' <summary>
	''' Deserialises the specified xml string to an object of the specified type.
	''' </summary>
	''' <typeparam name="T">The type to convert the object to.</typeparam>
	''' <param name="xmlDocument">The XML document to deserialise to a class.</param>
	''' <param name="appendHeader">If set to <c>true</c>, will append an xml header to the beginning of the string.</param>
	''' <returns>
	''' Deserialized object.
	''' </returns>
	Public Shared Function DeSerialize(Of T As Class)(xmlDocument As XmlDocument, Optional appendHeader As Boolean = True) As T
		Return DeSerialize(Of T)(xmlDocument.OuterXml, appendHeader)
	End Function

	'deserialize helper (use this one because it's strongly typed)
	''' <summary>
	''' Deserialises the specified xml string to an object of the specified type.
	''' </summary>
	''' <typeparam name="T">Type of object to deserialise to.</typeparam>
	''' <param name="sString">The string to deserialise into an object.</param>
	''' <param name="bAppendHeader">if set to <c>true</c>, will append an xml header to the beginning of the string.</param>
	Public Shared Function DeSerialize(Of T As Class)(ByVal sString As String, Optional ByVal bAppendHeader As Boolean = True) As T
		Return CType(Serializer.DeSerialize(GetType(T), sString, bAppendHeader), T)
	End Function

	'deserialize
	''' <summary>
	''' Deserialises the specified xml string to an object of the specified type.
	''' </summary>
	''' <param name="oType">Type of object to deserialise to.</param>
	''' <param name="sString">The string to deserialise into an object.</param>
	''' <param name="bAppendheader">if set to <c>true</c>, will append an xml header to the beginning of the string.</param>
	Public Shared Function DeSerialize(ByVal oType As Type, ByVal sString As String, Optional ByVal bAppendheader As Boolean = True) As Object

		Dim oXMLSerializer As New XmlSerializer(oType)

		Using oStream As New MemoryStream
			Using oStreamwriter As New StreamWriter(oStream)

				oStreamwriter.Write(IIf(bAppendheader, sXmlHeader, "").ToString & sString.Trim)
				oStreamwriter.Flush()

				oStream.Position = 0

				Return oXMLSerializer.Deserialize(oStream)

			End Using
		End Using

	End Function

#End Region

#Region "Clone"

	''' <summary>
	''' Serialises the specified object to a new string and returns it deserialised as a new object of the same type.
	''' </summary>
	''' <typeparam name="T">The type of object to clone</typeparam>
	''' <param name="oObjectToClone">The object to clone.</param>
	''' <param name="CheckSerializability">if set to <c>true</c>, will check the objects type is serialisable before cloning.</param>
	Public Shared Function Clone(Of T)(ByVal oObjectToClone As Object, Optional ByVal CheckSerializability As Boolean = True) As T
		Return CType(Serializer.Clone(oObjectToClone, CheckSerializability), T)
	End Function

	''' <summary>
	''' Serialises the specified object to a new string and returns it deserialised as a new object of the same type.
	''' </summary>
	''' <param name="oObjectToClone">The object to clone.</param>
	''' <param name="CheckSerializability">if set to <c>true</c>, will check the objects type is serialisable before cloning.</param>
	''' <exception cref="System.Exception">You can only clone serializable objects</exception>
	Public Shared Function Clone(ByVal oObjectToClone As Object, Optional ByVal CheckSerializability As Boolean = True) As Object

		If CheckSerializability AndAlso Not oObjectToClone.GetType.IsSerializable Then
			Throw New Exception("You can only clone serializable objects")
		End If

		Dim sSerializedObject As String = CleanXml(Serializer.Serialize(oObjectToClone)).InnerXml

		Return Serializer.DeSerialize(oObjectToClone.GetType, sSerializedObject)

	End Function

#End Region

#Region "Clean XML"

	''' <summary>
	''' Cleans the specified XML, removing document definitions.
	''' </summary>
	''' <param name="oDocumentToClean">The xml document to clean.</param>
	''' <param name="bStripArrayOf">Not used</param>
	Public Shared Function CleanXml(ByVal oDocumentToClean As XmlDocument,
			Optional ByVal bStripArrayOf As Boolean = False) As XmlDocument

		'Clean XML. Else you end up with the doc definition in the XML
		Dim sCleanXml As String = oDocumentToClean.InnerXml
		sCleanXml = sCleanXml.Replace("<?xml version=""1.0""?>", "")
		sCleanXml = sCleanXml.Replace("<?xml version=""1.0"" encoding=""utf-16""?>", "")
		sCleanXml = sCleanXml.Replace("<?xml version=""1.0"" encoding=""utf-8""?>", "")

		sCleanXml = sCleanXml.Replace("xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""", "")
		sCleanXml = sCleanXml.Replace("xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "")
		sCleanXml = sCleanXml.Replace("<ArrayOf", "<")
		sCleanXml = sCleanXml.Replace("</ArrayOf", "</")
		sCleanXml = sCleanXml.Replace("xsi:type=""xsd:int""", "")
		sCleanXml = sCleanXml.Replace("xsi:nil=""true""", "")
		sCleanXml = sCleanXml.Replace("xsi:type=""xsd:string""", "")

		'Replace any dodgy charecters.Must eb a better way
		' idealy you'd tell SQl to expect UTF-16. Cant work out how though.
		'sCleanXml = sCleanXml.Replace("£", "")
		'sCleanXml = sCleanXml.Replace("€", "")
		'sCleanXml = sCleanXml.Replace("$", "")
		'sCleanXml = sCleanXml.Replace("@", "")
		'sCleanXml = sCleanXml.Replace("%", "")
		'sCleanXml = sCleanXml.Replace("½", "&#189;")

		Dim oCleaned As New XmlDocument
		oCleaned.LoadXml(sCleanXml)
		Return oCleaned
	End Function

#End Region

#Region "serializetojson"

	''' <summary>
	''' Serialises specified object to json, uses single quotes.
	''' </summary>
	''' <param name="o">The object to serialise.</param>
	<Obsolete("Use the NewtonSoft library for JSON serialization")>
	Public Shared Function SerializeToJSON(ByVal o As Object) As String

		'build up the name/value pairs
		Dim aPairs As New Generic.Dictionary(Of String, String)
		Dim sValue As String = ""
		Dim oValue As Object

		'for each field
		For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

			oValue = oField.GetValue(o)
			sValue = ""

			If Not oValue Is Nothing Then
				Select Case oField.FieldType.Name
					Case "String"
						sValue = oField.GetValue(o).ToString
					Case "Int32"
						sValue = oField.GetValue(o).ToString
					Case "DateTime"
						sValue = DisplayDate(oField.GetValue(o).ToString)
					Case "Double", "Decimal"
						sValue = oField.GetValue(o).ToString
					Case "Boolean"
						sValue = oField.GetValue(o).ToString
					Case Else
						sValue = "**Ignore"
				End Select
			End If

			If sValue <> "**Ignore" Then
				sValue = """" & sValue.Replace("\", "\\").Replace("""", "\""").Replace("'", "\'").Replace(Environment.NewLine, "") & """"
				aPairs.Add(oField.Name, sValue)
			End If

		Next

		'build up the json string
		Dim sb As New System.Text.StringBuilder
		sb.Append("{")

		For Each oPair As Generic.KeyValuePair(Of String, String) In aPairs
			sb.Append("'").Append(oPair.Key).Append("' : ").Append(oPair.Value).Append(",")
		Next

		sb.Chop()

		sb.Append("}")

		Return sb.ToString

	End Function

	''' <summary>
	''' Serialises specified object to json, uses double quotes.
	''' </summary>
	''' <param name="o">The object to serialise.</param>
	<Obsolete("Use the NewtonSoft library for JSON serialization")>
	Public Shared Function SerializeToJSON2(ByVal o As Object) As String

		'build up the name/value pairs
		Dim aPairs As New Generic.Dictionary(Of String, String)
		Dim sValue As String = ""
		Dim oValue As Object

		'for each field
		For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

			oValue = oField.GetValue(o)
			sValue = ""

			If Not oValue Is Nothing Then
				Select Case oField.FieldType.Name
					Case "String"
						sValue = oField.GetValue(o).ToString
					Case "Int32"
						sValue = oField.GetValue(o).ToString
					Case "DateTime"
						sValue = DisplayDate(oField.GetValue(o).ToString)
					Case "Double", "Decimal"
						sValue = oField.GetValue(o).ToString
					Case "Boolean"
						sValue = oField.GetValue(o).ToString
					Case Else
						sValue = "**Ignore"
				End Select
			End If

			If sValue <> "**Ignore" Then
				sValue = """" & sValue.Replace("\", "\\").Replace("""", "\""").Replace("'", "\'").Replace(Environment.NewLine, "") & """"
				aPairs.Add(oField.Name, sValue)
			End If

		Next

		'build up the json string
		Dim sb As New System.Text.StringBuilder
		sb.Append("{")

		For Each oPair As Generic.KeyValuePair(Of String, String) In aPairs
			sb.Append("""").Append(oPair.Key).Append(""" : ").Append(oPair.Value).Append(",")
		Next

		sb.Chop()

		sb.Append("}")

		Return sb.ToString

	End Function

#End Region

#Region "deserialize query string"

	''' <summary>
	''' Takes values from specified query string and sets them on matching fields on the specified object.
	''' </summary>
	''' <param name="o">The object to set the values on.</param>
	''' <param name="QueryString">The query string to get the values from.</param>
	''' <exception cref="System.Exception">
	''' no translation for field type
	''' </exception>
	Public Shared Sub DeserializeQueryString(ByVal o As Object, ByVal QueryString As String)

		'split query string into dictionary
		Dim aQueryString As Generic.Dictionary(Of String, String) = Intuitive.Functions.Web.ConvertQueryStringToDictionary(QueryString)

		'scan through each pair
		For Each oPair As Generic.KeyValuePair(Of String, String) In aQueryString

			'for each field
			For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

				If oPair.Key.ToLower = oField.Name.ToLower OrElse
				 Regex.IsMatch(oPair.Key.ToLower, String.Format("^\w{{3}}{0}$", oField.Name.ToLower)) Then

					Select Case oField.FieldType.Name
						Case "String"
							oField.SetValue(o, oPair.Value)
						Case "Int32"
							oField.SetValue(o, SafeInt(oPair.Value))
						Case "DateTime"
							oField.SetValue(o, DateFunctions.SafeDate(oPair.Value))
						Case "Double"
							oField.SetValue(o, SafeNumeric(oPair.Value))
						Case "Decimal"
							oField.SetValue(o, CType(SafeNumeric(oPair.Value), Decimal))
						Case "Boolean"
							oField.SetValue(o, SafeBoolean(oPair.Value))
						Case Else
							Throw New Exception("no translation for field type " & oField.FieldType.Name)
					End Select
				End If

			Next

			'for each property
			For Each oProperty As System.Reflection.PropertyInfo In o.GetType.GetProperties

				If oPair.Key.ToLower = oProperty.Name.ToLower OrElse
				 Regex.IsMatch(oPair.Key.ToLower, String.Format("^\w{{3}}{0}$", oProperty.Name.ToLower)) Then

					Select Case oProperty.PropertyType.Name
						Case "String"
							oProperty.SetValue(o, oPair.Value, Nothing)
						Case "Int32"
							oProperty.SetValue(o, SafeInt(oPair.Value), Nothing)
						Case "DateTime"
							oProperty.SetValue(o, DateFunctions.SafeDate(oPair.Value), Nothing)
						Case "Double"
							oProperty.SetValue(o, SafeNumeric(oPair.Value), Nothing)
						Case "Decimal"
							oProperty.SetValue(o, CType(SafeNumeric(oPair.Value), Decimal), Nothing)
						Case "Boolean"
							oProperty.SetValue(o, SafeBoolean(oPair.Value), Nothing)
						Case Else
							Throw New Exception("no translation for field type " & oProperty.PropertyType.Name)
					End Select
				End If
			Next

		Next

	End Sub

#End Region

#Region "deserialize data row"

	''' <summary>
	''' Deserialises the datatable to a list of objects of specified type, each row in the datatable is converted to an object in the list.
	''' </summary>
	''' <typeparam name="T">The type of object to convert the datatable to</typeparam>
	''' <param name="dt">The datatable.</param>
	''' <param name="IgnoreNonTranslatableFieldTypes">if set to <c>true</c>, .</param>
	Public Shared Function DeserializeDataTable(Of T As New)(ByVal dt As DataTable, Optional ByVal IgnoreNonTranslatableFieldTypes As Boolean = False) As Generic.List(Of T)

		Dim oReturn As New Generic.List(Of T)

		For Each dr As DataRow In dt.Rows
			oReturn.Add(Serializer.DeserializeDataRow(Of T)(dr, IgnoreNonTranslatableFieldTypes))
		Next

		Return oReturn

	End Function

	''' <summary>
	''' Takes values from a datarow and sets them on matching fields on a new object of specified type
	''' </summary>
	''' <typeparam name="T">The type of the object to return</typeparam>
	''' <param name="dr">The datarow.</param>
	''' <param name="IgnoreNonTranslatableFieldTypes">if set to <c>true</c>, will ignore fields that can't be translated.</param>
	Public Shared Function DeserializeDataRow(Of T As New)(ByVal dr As DataRow, Optional ByVal IgnoreNonTranslatableFieldTypes As Boolean = False) As T

		Dim oReturn As New T

		Serializer.DeserializeDataRow(oReturn, dr, IgnoreNonTranslatableFieldTypes)

		Return oReturn

	End Function

	''' <summary>
	''' Takes values from a datarow and sets them on matching fields on the specified object
	''' </summary>
	''' <param name="o">The object to set the values on.</param>
	''' <param name="dr">The datarow.</param>
	''' <param name="IgnoreNonTranslatableFieldTypes">if set to <c>true</c>, will ignore fields that can't be translated.</param>
	''' <exception cref="System.Exception">
	''' no translation for field type
	''' </exception>
	Public Shared Sub DeserializeDataRow(ByVal o As Object, ByVal dr As DataRow, Optional ByVal IgnoreNonTranslatableFieldTypes As Boolean = False)

		Dim sValue As String

		For Each oColumn As DataColumn In dr.Table.Columns

			For Each oField As System.Reflection.FieldInfo In o.GetType.GetFields

				If oColumn.ColumnName.ToLower = oField.Name.ToLower Then

					sValue = dr(oColumn.ColumnName).ToString

					Select Case oField.FieldType.Name
						Case "String"
							oField.SetValue(o, sValue)
						Case "Int32"
							oField.SetValue(o, SafeInt(sValue))
						Case "DateTime"
							oField.SetValue(o, DateFunctions.SafeDate(sValue))
						Case "Double"
							oField.SetValue(o, SafeNumeric(sValue))
						Case "Decimal"
							oField.SetValue(o, CType(SafeNumeric(sValue), Decimal))
						Case "Boolean"
							oField.SetValue(o, SafeBoolean(sValue))
						Case Else
							If oField.FieldType.BaseType.Name = "Enum" Then
								oField.SetValue(o, Intuitive.Functions.SafeEnum(oField.FieldType, sValue))
							ElseIf Not IgnoreNonTranslatableFieldTypes Then
								Throw New Exception("no translation for field type " & oField.FieldType.Name)
							End If
					End Select
				End If

			Next

			For Each oProperty As System.Reflection.PropertyInfo In o.GetType.GetProperties

				If oColumn.ColumnName.ToLower = oProperty.Name.ToLower Then
					sValue = dr(oColumn.ColumnName).ToString
					Select Case oProperty.PropertyType.Name
						Case "String"
							oProperty.SetValue(o, sValue, Nothing)
						Case "Int32"
							oProperty.SetValue(o, SafeInt(sValue), Nothing)
						Case "DateTime"
							oProperty.SetValue(o, DateFunctions.SafeDate(sValue), Nothing)
						Case "Double"
							oProperty.SetValue(o, SafeNumeric(sValue), Nothing)
						Case "Decimal"
							oProperty.SetValue(o, CType(SafeNumeric(sValue), Decimal), Nothing)
						Case "Boolean"
							oProperty.SetValue(o, SafeBoolean(sValue), Nothing)
						Case Else
							If oProperty.PropertyType.BaseType.Name = "Enum" Then
								oProperty.SetValue(o, Intuitive.Functions.SafeEnum(oProperty.PropertyType, sValue), Nothing)
							ElseIf Not IgnoreNonTranslatableFieldTypes Then
								Throw New Exception("no translation for field type " & oProperty.PropertyType.Name)
							End If
					End Select
				End If
			Next

		Next

	End Sub

#End Region

#Region "Merge Classes"

	''' <summary>
	''' Merges source object into target object. 
	''' Sets Property values on target object to the value of matching Properties from the source object. 
	''' Properties match if the Name and PropertyType are the same.
	''' </summary>
	''' <param name="oSource">The source object.</param>
	''' <param name="oTarget">The target object.</param>
	Public Shared Sub MergeClasses(oSource As Object, oTarget As Object)

		'loop through source properties
		For Each oSourceProperty As System.Reflection.PropertyInfo In oSource.GetType.GetProperties

			'loop through target properties
			For Each oTargetProperty As System.Reflection.PropertyInfo In oTarget.GetType.GetProperties

				'if the names and types match then copy over
				If oSourceProperty.Name = oTargetProperty.Name AndAlso oSourceProperty.PropertyType = oTargetProperty.PropertyType Then
					oTargetProperty.SetValue(oTarget, oSourceProperty.GetValue(oSource, Nothing), Nothing)
				End If

			Next

		Next

	End Sub

	''' <summary>
	''' Merges source object into target object. 
	''' Sets Property and Field values on target object to the value of matching Propeties and Fields from the source object. 
	''' Properties match if the Name and PropertyType are the same. 
	''' Fields match if the Name and FieldType are the same.
	''' </summary>
	''' <param name="oSource">The source object.</param>
	''' <param name="oTarget">The target object.</param>
	Public Shared Sub MergeClassesComplete(oSource As Object, oTarget As Object)

		'loop through source fields first
		For Each oSourceProperty As System.Reflection.FieldInfo In oSource.GetType.GetFields

			'loop through target fields
			For Each oTargetProperty As System.Reflection.FieldInfo In oTarget.GetType.GetFields

				'if the names and types match then copy over
				If oSourceProperty.Name = oTargetProperty.Name AndAlso oSourceProperty.FieldType = oTargetProperty.FieldType Then

					oTargetProperty.SetValue(oTarget, oSourceProperty.GetValue(oSource))
					Exit For

				End If

			Next

		Next

		'loop through source properties
		For Each oSourceProperty As System.Reflection.PropertyInfo In oSource.GetType.GetProperties

			'loop through target properties
			For Each oTargetProperty As System.Reflection.PropertyInfo In oTarget.GetType.GetProperties

				'if the names and types match
				If oSourceProperty.Name = oTargetProperty.Name AndAlso oSourceProperty.PropertyType = oTargetProperty.PropertyType Then

					'and the property isn't readonly then copy over
					If oTargetProperty.CanWrite Then
						oTargetProperty.SetValue(oTarget, oSourceProperty.GetValue(oSource, Nothing), Nothing)
					End If
					Exit For

				End If

			Next

		Next

	End Sub

#End Region

End Class