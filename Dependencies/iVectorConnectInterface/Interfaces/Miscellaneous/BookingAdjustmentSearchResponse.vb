Imports System.Xml.Serialization


Public Class BookingAdjustmentSearchResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

#Region "Contsructor"

	Public Sub New()
		Me._BookingAdjustmentTypes = New Generic.List(Of BookingAdjustmentType)
	End Sub

#End Region

#Region "Properties"

	Private _ReturnStatus As New ReturnStatus
	Private _BookingAdjustmentTypes As Generic.List(Of BookingAdjustmentType) = Nothing

#End Region


#Region "Accessor methods"

	Public Property ReturnStatus As ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus
		Get
			Return Me._ReturnStatus
		End Get
		Set(value As ReturnStatus)
			Me._ReturnStatus = value
		End Set
	End Property

	Public Property BookingAdjustmentTypes As Generic.List(Of BookingAdjustmentType)
		Get
			Return Me._BookingAdjustmentTypes
		End Get
		Set(value As Generic.List(Of BookingAdjustmentType))
			Me._BookingAdjustmentTypes = value
		End Set
	End Property

#End Region


#Region "Helper Classes"

	Public Class BookingAdjustmentType

#Region "Properties"

		Private _AdjustmentType As String
		Private _AdjustmentAmount As Decimal

#End Region


#Region "Accessor methods"

		Public Property AdjustmentType As String
			Get
				Return Me._AdjustmentType
			End Get
			Set(value As String)
				Me._AdjustmentType = value
			End Set
		End Property

		Public Property AdjustmentAmount As Decimal
			Get
				Return Me._AdjustmentAmount
			End Get
			Set(value As Decimal)
				Me._AdjustmentAmount = value
			End Set
		End Property

#End Region

	End Class

#End Region

End Class
