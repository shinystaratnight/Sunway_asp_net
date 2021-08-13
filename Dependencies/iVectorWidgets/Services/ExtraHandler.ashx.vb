Imports System.Web
Imports System.Web.Services
Imports System.Reflection

Public Class ExtraHandler
	Implements System.Web.IHttpHandler, IRequiresSessionState

	Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

		Dim handlerFunction As String = context.Request.RequestContext.RouteData.Values("key").ToString
		Dim thisType As Type = Me.GetType()
		Dim method As MethodInfo = thisType.GetMethod(handlerFunction,
													  BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Instance)

		Dim params() As Object = New Object() {context}
		Dim result As Object = Nothing

        result = method.Invoke(Me, params)

        'do something with result and write out to the response
        context.Response.Write(result)

	End Sub

	Public Function Save(ByVal context As HttpContext) As String

		Dim sJson As String = ""
		context.Request.InputStream.Position = 0
		Using inputStream As New System.IO.StreamReader(context.Request.InputStream)
			sJson = inputStream.ReadToEnd()
		End Using

		Dim oExtraSelection As New Extras

		oExtraSelection = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Extras)(sJson)
        Dim iComponentID As Integer = 0
        Dim clearExtrasOfTheSameType As Boolean = True
		For Each ExtraSelection As ExtraSelection In oExtraSelection.Extras
			Dim oExtraVisitor As New Intuitive.Web.Basket.Visitors.ExtrasVisitor(oExtraSelection.Identifier,
																ExtraSelection.Quantity,
																ExtraSelection.Key,
																clearExtrasOfTheSameType,
																True,
																oExtraSelection.ComponentID)

			iComponentID = Intuitive.Web.BookingBase.SearchBasket.Accept(oExtraVisitor)
			clearExtrasOfTheSameType = False
		Next

		Return Newtonsoft.Json.JsonConvert.SerializeObject(New With {.ComponentID = iComponentID})

	End Function

	Public Function UpdateVehicleRegistration(ByVal context As HttpContext) As String

		Dim sJson As String = ""
		context.Request.InputStream.Position = 0
		Using inputStream As New System.IO.StreamReader(context.Request.InputStream)
			sJson = inputStream.ReadToEnd()
		End Using

		Dim oVehicleInformation As New VehicleInformation

		oVehicleInformation = Newtonsoft.Json.JsonConvert.DeserializeObject(Of VehicleInformation)(sJson)

		Dim oReturn As New RegisterVehicleReturn

		oReturn.Success = True

		Try
			Intuitive.Web.BookingBase.Basket.VehicleInfo.CarRegistration = oVehicleInformation.CarRegistration
            Intuitive.Web.BookingBase.Basket.VehicleInfo.CarMake = oVehicleInformation.CarMake
            Intuitive.Web.BookingBase.Basket.VehicleInfo.CarModel = oVehicleInformation.CarModel
            Intuitive.Web.BookingBase.Basket.VehicleInfo.CarColour = oVehicleInformation.CarColour
		Catch ex As Exception
			oReturn.Success = False
		End Try
		

		Return Newtonsoft.Json.JsonConvert.SerializeObject(oReturn)

	End Function

	Public Class Extras
		Public Extras As List(Of ExtraSelection)
        Public Identifier As String
        Public ComponentID As Integer
	End Class

	Public Class ExtraSelection
		Public Key As String
        Public Quantity As Integer
	End Class

	Public Class VehicleInformation
		Public CarRegistration As String
		Public CarMake As String
		Public CarModel As String
		Public CarColour As String
	End Class

	Public Class RegisterVehicleReturn
		Public Success As Boolean
	End Class

	ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
		Get
			Return False
		End Get
	End Property

End Class