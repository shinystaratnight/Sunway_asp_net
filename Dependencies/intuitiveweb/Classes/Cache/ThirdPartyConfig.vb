Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Web
Imports Intuitive.Functions

Namespace CacheControl

#Region "Class"

	'Class
	<Serializable()> _
	Public Class ThirdPartyConfigDef

		<XmlArrayItem("ThirdPartyConfig")>
		Public ThirdPartyConfigs As New Generic.List(Of ThirdPartyConfig)

	End Class

	Public Class ThirdPartyConfig

#Region "Properties"
		Public ThirdPartyID As Integer
		Public ThirdParty As String
		Public ThirdPartyType As String
		Public SalesChannelID As Integer
		Public BrandID As Integer
		Public Config As String
#End Region

#Region "Get"

		'Try not to use if it can be avoided. Shouldnt go and get thirdpartysettings from the database
		'Should be handled by connect but doesnt currently support this
		Public Shared ReadOnly Property GetThirdPartyConfig(ByVal ThirdParty As String, ByVal ThirdPartyType As String) As String
			Get

				Dim oThirdPartyConfig As ThirdPartyConfig

				Try

					'Check for ThirdParty by Brand
					Dim sKey As String = CacheControl.ThirdPartyConfig.Cache.GetConfiguration_key(ThirdParty, ThirdPartyType, 1, BookingBase.Params.BrandID)
					oThirdPartyConfig = CacheControl.ThirdPartyConfig.Cache.GetConfiguration(sKey)

				Catch ex As Exception

					Try

						'If not found check without Brand
						Dim sKey As String = CacheControl.ThirdPartyConfig.Cache.GetConfiguration_key(ThirdParty, ThirdPartyType, 1, 0)
						oThirdPartyConfig = CacheControl.ThirdPartyConfig.Cache.GetConfiguration(sKey)

					Catch exc As Exception

						Return "off"

					End Try

				End Try

				Return oThirdPartyConfig.Config.ToLower

			End Get
		End Property
#End Region

#Region "Cache"
		Public Class Cache

			Public Class ThirdPartyConfig_Dictionary
				Inherits Dictionary(Of String, ThirdPartyConfig)
			End Class


			Public Shared ReadOnly Property GetConfiguration As ThirdPartyConfig_Dictionary

				Get
					Dim oDictionary As ThirdPartyConfig_Dictionary = CacheControl.Cache.GetCache(Of ThirdPartyConfig_Dictionary)("ThirdPartyConfig_Dictionary")

					If oDictionary Is Nothing Then

						oDictionary = New ThirdPartyConfig_Dictionary

						Dim oThirdPartyConfigDef As ThirdPartyConfigDef = CacheControl.ThirdPartyConfig.Cache.GetThirdPartyConfigDef

						For Each o As ThirdPartyConfig In oThirdPartyConfigDef.ThirdPartyConfigs

							Dim sKey As String = CacheControl.ThirdPartyConfig.Cache.GetConfiguration_key(o.ThirdParty, o.ThirdPartyType, o.SalesChannelID, o.BrandID)

							oDictionary.Add(sKey, o)

						Next

						CacheControl.Cache.AddToCache("ThirdPartyConfig_Dictionary", oDictionary, 60)

					End If

					Return oDictionary

				End Get
			End Property

			Public Shared Function GetConfiguration_key(ByVal ThirdParty As String, ByVal ThirdPartyType As String, ByVal SalesChannelID As Integer, ByVal BrandID As Integer) As String

				Dim sKey As String = String.Format("{0}#{1}#{2}#{3}", ThirdParty, ThirdPartyType, SalesChannelID, BrandID)

				Return sKey

			End Function

			Public Shared Function GetThirdPartyConfigDef() As ThirdPartyConfigDef

				Dim oXML As System.Xml.XmlDocument = Utility.BigCXML("ThirdPartyConfig", 1, 60, 0, 0)

				Dim oThirdPartyConfigDef As New ThirdPartyConfigDef

				Try
					oThirdPartyConfigDef = Intuitive.Serializer.DeSerialize(Of ThirdPartyConfigDef)(oXML.InnerXml)
				Catch ex As Exception

#If DEBUG Then
					'Ensure the ThirdPartyBigC job has been set up on clients system
					'Job can be found in iVectorWidgets/Classes/Cache/BigC/ThirdPartyConfig.txt

					Dim oEmail As New Intuitive.Email
					With oEmail
						.EmailTo = "jon@intuitivesystems.co.uk"
						.FromEmail = "iVectorWidgets@intuitivesystems.co.uk"
						.From = "iVectorWidgets"
						.Subject = "ThirdPartyConfig BigC Job not created"
						.Body = String.Format("ThirdPartyConfig BigC Job not created, {1}, {0}", System.Environment.MachineName.ToLower, BookingBase.Params.Theme)
						.SMTPHost = BookingBase.Params.SMTPHost

						.SendEmail()
					End With


					Throw New Exception

#Else
						oThirdPartyConfigDef = New ThirdPartyConfigDef
#End If

				End Try

				Return oThirdPartyConfigDef

			End Function

		End Class
#End Region

	End Class

#End Region



End Namespace
