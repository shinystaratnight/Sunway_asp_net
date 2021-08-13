Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Serialization

Namespace Xml.Serialization

	''' <summary>
	''' Generic list serializer.
	''' </summary>
	''' <typeparam name="T"></typeparam>
	''' <seealso cref="System.Xml.Serialization.XmlSerializer" />
	Class GenericListSerializer(Of T)
		Inherits XmlSerializer

		''' <summary>
		''' The generic list.
		''' To retain the serializer list.
		''' </summary>
		''' <remarks>
		''' Not sure if we need this - CT
		''' </remarks>
		Private oGenericList As SerializableList(Of T)

		''' <summary>
		''' The attribute overrides.
		''' </summary>
		Private oAttributeOverrides As XmlAttributeOverrides

		''' <summary>
		''' The root attribute.
		''' </summary>
		Private oRootAttribute As XmlRootAttribute

		''' <summary>
		''' The serializers stored with type name.
		''' </summary>
		Private oSerializers As New Dictionary(Of String, XmlSerializer)

		''' <summary>
		''' Initializes a new instance of the <see cref="T:GenericListSerializer{T}" /> class.
		''' </summary>
		''' <param name="attributeOverrides">The attribute overrides.</param>
		''' <param name="rootAttribute">The root attribute.</param>
		Public Sub New(Optional attributeOverrides As XmlAttributeOverrides = Nothing, Optional rootAttribute As XmlRootAttribute = Nothing)
			oAttributeOverrides = attributeOverrides
			oRootAttribute = rootAttribute
		End Sub

		''' <summary>
		''' Serializes the serializable list.
		''' </summary>
		''' <param name="writer">The output stream to write the serialized data.</param>
		''' <param name="list">The serializable list.</param>
		Public Shadows Sub Serialize(writer As XmlWriter, list As SerializableList(Of T))

			oGenericList = list

			For Each oItem As T In oGenericList
				GetSerializerByQualifiedName(oItem.GetType().GenericQualifiedName).Serialize(writer, oItem)
			Next

		End Sub

		''' <summary>
		''' Deserializes the serializable list.
		''' </summary>
		''' <param name="reader">The input stream to read the serialized data.</param>
		''' <param name="list">The serializable list.</param>
		Public Shadows Sub Deserialize(reader As XmlReader, list As SerializableList(Of T))

			Dim sName As String = reader.Name

			reader.Read()

			While sName IsNot reader.Name

				Dim oSerializer As XmlSerializer = GetSerializerByQualifiedName(String.Format("{0}.{1}, {2}", GetType(T).Namespace, reader.Name, list.GetType().Assembly.Name))

				list.Add(CType(oSerializer.Deserialize(reader), T))

			End While

		End Sub

		''' <summary>
		''' Gets the serializer by the qualified type name from internal XML serializers list.
		''' If specific serializers don't exist, adds it and returns it.
		''' </summary>
		''' <param name="qualifiedName">Name of the qualified.</param>
		''' <returns>
		''' XML serializer.
		''' </returns>
		Private Function GetSerializerByQualifiedName(qualifiedName As String) As XmlSerializer

			Dim oSerializer As XmlSerializer = Nothing

			If oSerializers.ContainsKey(qualifiedName) Then oSerializer = oSerializers.Item(qualifiedName)

			If oSerializer Is Nothing Then
				If oRootAttribute Is Nothing Then
					oSerializer = New XmlSerializer(Type.GetType(qualifiedName), GetType(T).GetSubTypes(True).ToArray)
				Else
					oSerializer = New XmlSerializer(Type.GetType(qualifiedName), oAttributeOverrides, GetType(T).GetSubTypes(True).ToArray, oRootAttribute, Nothing)
				End If

				oSerializers.Add(qualifiedName, oSerializer)
			End If

			Return oSerializer

		End Function

	End Class

End Namespace