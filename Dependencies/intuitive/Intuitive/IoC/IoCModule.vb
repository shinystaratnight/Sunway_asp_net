Imports Autofac
Imports Intuitive.Configuration
Imports Intuitive.IOUtility
Imports Intuitive.Net
Imports Intuitive.SqlInstance

Namespace IoC

	''' <summary>
	''' The class for registering components with the IoC container
	''' </summary>
	Public Class IoCModule
		Inherits [Module]

		''' <summary>
		''' Add registrations to the container here
		''' </summary>
		''' <param name="builder">The builder through which components can be registered</param>
		''' <remarks>In order to use this project, this module must be registered by the client on startup</remarks>
		Protected Overrides Sub Load(builder As ContainerBuilder)

			builder.RegisterType(Of Email)().As(Of IEmail)()
			builder.RegisterType(Of XSLEmail)().As(Of IXSLEmail)()
			builder.RegisterType(Of WebRequests.Request)().As(Of IRequest)()

			MyBase.Load(builder)

		End Sub

	End Class


End Namespace