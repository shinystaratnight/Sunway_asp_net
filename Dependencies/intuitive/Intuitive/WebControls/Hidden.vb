Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.ComponentModel
Imports Intuitive.Functions

Namespace WebControls
	Public Class Hidden
		Inherits WebControlBase

		Private sValue As String
		Private bEncodeValue As Boolean = False

        Dim bIsValid As Boolean = True

#Region "Properties"
        Public Overrides Property Value() As String
            Get
                Return sValue
            End Get
            Set(ByVal Value As String)
                sValue = System.Web.HttpUtility.HtmlEncode(Value)
            End Set
        End Property

		Public Property IntValue() As Integer
			Get
				Dim iValue As Integer = 0
				If Integer.TryParse(sValue, iValue) Then
					Return iValue
				Else
					Return -1
				End If
			End Get
			Set(ByVal Value As Integer)
				sValue = Value.ToString
			End Set
		End Property

		Public Shadows Property Enabled() As Boolean
            Get
                If Me.ViewState("Enabled") Is Nothing Then
                    Return True
                Else
                    Return CType(Me.ViewState("Enabled"), Boolean)
                End If
            End Get
            Set(ByVal Value As Boolean)
                Me.ViewState("Enabled") = Value
            End Set
        End Property

        Public Overrides Property IsValid() As Boolean
            Get
                Return bIsValid
            End Get
            Set(ByVal Value As Boolean)
                bIsValid = Value
            End Set
        End Property

        Public Property EncodeValue As Boolean
            Get
                Return bEncodeValue
            End Get
            Set(value As Boolean)
                bEncodeValue = value
            End Set
        End Property

#End Region

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim sTextBox As String

            'build the textbox + image template
            sTextBox = "<input type=""hidden"" name=""{0}"" id=""{0}"" " &
                       "value=""{1}"" /> "

            'output the hidden field
            Dim sValueToWriteOut As String = sValue
            If Me.EncodeValue Then sValueToWriteOut = System.Web.HttpContext.Current.Server.HtmlEncode(sValue)
            writer.Write(String.Format(sTextBox, Me.ID, sValueToWriteOut))

        End Sub

#Region "Postback Handling"
		Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

			If Me.Page.IsPostBack Then
				sValue = SafeString(Me.Page.Request(Me.ID))
			End If

		End Sub
#End Region

	End Class
End Namespace