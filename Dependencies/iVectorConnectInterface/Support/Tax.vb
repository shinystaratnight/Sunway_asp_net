Imports System.Xml.Serialization

Namespace Support

	Public Class Tax

		<XmlText>
		Public Property TaxAmount As Decimal

		<XmlAttribute("name")>
		Public Property TaxType As String

	End Class

End Namespace