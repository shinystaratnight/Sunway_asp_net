Imports Intuitive.Validators
Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces

Namespace Basket
	<XmlRoot("BasketQuoteRequest")>
	Public Class QuoteRequest
		Inherits BasketRequest
		Implements iVectorConnectRequest
	End Class

End Namespace