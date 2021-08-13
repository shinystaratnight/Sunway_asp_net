Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Namespace Xml.Serialization

	''' <summary>
	''' Generic list that can be serialized even as a list of interfaces.
	''' </summary>
	''' <typeparam name="T"></typeparam>
	''' <seealso cref="System.Collections.Generic.List(Of T)" />
	''' <seealso cref="System.Xml.Serialization.IXmlSerializable" />
	<Serializable>
	Public Class SerializableList(Of T)
		Inherits List(Of T)
		Implements IXmlSerializable

		''' <summary>
		''' The root attribute override.
		''' Potentially causes issues during deserializing sub types.
		''' </summary>
		Public RootAttributeOverride As XmlRootAttribute = Nothing

		''' <summary>
		''' Reads the XML.
		''' </summary>
		''' <param name="reader">The reader.</param>
		Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml

			Dim oAttributeOverrides As New XmlAttributeOverrides
			Dim oAttributes As New XmlAttributes

			For Each oType As Type In GetType(T).GetSubTypes(True)
				Dim oElementAttribute As New XmlElementAttribute(oType.Name, oType)

				oAttributes.XmlElements.Add(oElementAttribute)
			Next

			oAttributeOverrides.Add(GetType(T), GetType(T).Name, oAttributes)

			Dim oGenericListSerializer As New GenericListSerializer(Of T)(oAttributeOverrides)

			oGenericListSerializer.Deserialize(reader, Me)

		End Sub

		''' <summary>
		''' Writes the XML.
		''' </summary>
		''' <param name="writer">The writer.</param>
		Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml

			Dim oAttributeOverrides As New XmlAttributeOverrides
			Dim oAttributes As New XmlAttributes

			For Each oItem As T In Me
				Dim oElementAttribute As New XmlElementAttribute(oItem.GetType().Name, oItem.GetType())

				If Not oAttributes.XmlElements.Contains(oElementAttribute) Then oAttributes.XmlElements.Add(oElementAttribute)
			Next

			oAttributeOverrides.Add(GetType(T), oAttributes)

			Dim oGenericListSerializer As New GenericListSerializer(Of T)(oAttributeOverrides, RootAttributeOverride)

			oGenericListSerializer.Serialize(writer, Me)

		End Sub

		''' <summary>
		''' This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
		''' </summary>
		''' <returns>
		''' An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
		''' </returns>
		Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
			Return Nothing
		End Function

	End Class

End Namespace