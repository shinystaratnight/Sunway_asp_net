Imports Intuitive
Imports System.Text
Imports System.Web
Imports Intuitive.Functions

Namespace WebControls

	Public Class AutoComplete
		Inherits WebControlBase

		Public Event Selected(ByVal ID As Integer, ByVal DisplayValue As String)

#Region "properties 'n' that"

		Public Table As String
		Public Expression As String
		Public SecondExpression As String
		Public Order As String
		Public Filter As String
		Public Top As Integer = 0
		Public Left As Integer = 0
		Public AutoPostback As Boolean = False
		Public Script As String = ""
		Public BlurScript As String = ""
		Public FocusScript As String = ""
		Public ClearScript As String = ""
		Public KeyUpProxy As String = ""
		Public KeyDownProxy As String = ""
		Public OverrideConnectString As String = ""
        Public ShowEmptyResults As Boolean = True
        Public OverrideValue As String = ""

        'custom sql properties
        Public SourceStoredProcedure As String = ""
		Public SourceStoreProcedureParams As String = ""

		Public HiddenControls As String = ""

		Public URL As String = "~/Secure/WebServices/AutoComplete.aspx"
		Public CompleteURL As String

		Private bIsValid As Boolean = True
		Private bEnabled As Boolean = True
		Private sValue As String = ""
		Private sDisplayValue As String = ""

		Public Overrides Property IsValid() As Boolean
			Get
				Return bIsValid
			End Get
			Set(ByVal Value As Boolean)
				bIsValid = Value
			End Set
		End Property

		Public Overrides Property Value() As String
			Get
				Return sValue
			End Get
			Set(ByVal Value As String)
				sValue = Value

				If Me.Table <> "" AndAlso Me.Expression <> "" Then
					Dim sSql As String = "select {1} from {0} where {0}ID={2}"
					If Me.OverrideConnectString = "" Then
						sDisplayValue = SQL.GetValue(sSql, Me.Table, Me.Expression, SafeInt(Me.Value))
					Else
						sDisplayValue = SQL.GetValueWithConnectString(Me.OverrideConnectString, sSql, Me.Table, Me.Expression, SafeInt(Me.Value))
					End If
				Else
					sDisplayValue = ""
				End If

			End Set
		End Property

		Public Shadows Property Enabled() As Boolean
			Get
				Return bEnabled
			End Get
			Set(ByVal Value As Boolean)
				bEnabled = Value
			End Set
		End Property

		Public Property DisplayValue() As String
			Get
				Return sDisplayValue
			End Get
			Set(ByVal Value As String)
				sDisplayValue = Value
			End Set
		End Property

		Public ReadOnly Property IntValue() As Integer
			Get
				Return SafeInt(Me.Value)
			End Get
		End Property

#End Region

		Public Sub SetValueByDisplayValue(Optional DisplayValue As String = Nothing)

			If DisplayValue IsNot Nothing Then Me.DisplayValue = DisplayValue

			Dim iID As Integer = SQL.GetValue("SELECT {0} FROM {1} WHERE {2} = {3}",
				SQL.GetSqlValue(Table + "ID"),
				Table,
				SQL.GetSqlValue(Expression),
				SQL.GetSqlValue(Me.DisplayValue)).ToSafeInt

			Value = iID.ToSafeString

		End Sub

#Region "Viewstate Management"
		Protected Overrides Function SaveViewState() As Object
			Dim oState(2) As Object

			oState(0) = MyBase.SaveViewState
			oState(1) = Filter

			Return oState
		End Function

		Protected Overrides Sub LoadViewState(ByVal savedState As Object)

			If Not savedState Is Nothing Then
				Dim oState As Object() = CType(savedState, Object())

				MyBase.LoadViewState(oState(0))

				Filter = CType(oState(1), String)

			End If
		End Sub

#End Region

#Region "render"

		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

			Dim sb As New StringBuilder

			Dim sURL As String = IIf(Me.CompleteURL = "", Me.ResolveUrl(Me.URL), Me.CompleteURL)

			Dim sExtraClass As String = ""
			If Not bIsValid Then
				sExtraClass = " error"
			ElseIf Not bEnabled Then
				sExtraClass = " readonly"
			End If

			'control size
			If Me.ControlSize <> WebControlBase.ControlSizes.Normal Then
				sExtraClass += IIf(Me.ControlSize = WebControlBase.ControlSizes.Small, " small",
					IIf(Me.ControlSize = WebControlBase.ControlSizes.Large, " large", "")).ToString
			End If

			Dim sKeyUpFunction As String = "AutoSuggestKeyUp"
			If Me.KeyUpProxy <> "" Then sKeyUpFunction = Me.KeyUpProxy
			Dim sKeyDownFunction As String = "AutoSuggestKeyDown"
			If Me.KeyDownProxy <> "" Then sKeyDownFunction = Me.KeyDownProxy

			Dim sBlurScript As String = ""
			If Me.BlurScript <> "" Then
				sBlurScript = " onblur=""" & Me.BlurScript & """"
			Else
				sBlurScript = "onblur=""AutoSuggestOnBlur(this)"""
			End If

			Dim sFocusScript As String = ""
			If Me.FocusScript <> "" Then
				sFocusScript = " onfocus=""" & Me.FocusScript & """"
			End If

			sb.AppendFormat("<input type=""text"" id=""{0}"" name=""{0}"" class=""textbox autocomplete{3}"" value=""{2}"" {4} " &
			 "onkeyup=""{5}(event,this,'{1}');"" " &
			 "onkeydown=""{6}(event,this);""{7}{8} autocomplete=""off"" data-showemptyresults=""{9}"" />", Me.ID, sURL, sDisplayValue, sExtraClass,
			 IIf(bEnabled, "", "readonly=""readonly"""), sKeyUpFunction, sKeyDownFunction, sBlurScript, sFocusScript, Me.ShowEmptyResults.ToString.ToLower)

			sb.AppendFormat("<input type=""hidden"" id=""{0}Hidden"" value=""{1}"" name=""{0}Hidden""/>",
				Me.ID, SafeInt(Me.Value))

            sb.AppendFormat("<input type=""hidden"" id=""{0}Params"" value=""{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}"" name=""{0}Params""/>",
             Me.ID, Me.Table, HttpUtility.UrlEncode(Me.Expression), Me.Left, Me.Top,
             HttpUtility.UrlEncode(Me.SecondExpression), Me.Filter, Me.Order, Me.AutoPostback,
            Functions.Encrypt(Me.OverrideConnectString), Me.SourceStoredProcedure, Me.SourceStoreProcedureParams, Me.OverrideValue)

            If Me.HiddenControls <> "" Then
				sb.AppendFormat("<input type=""hidden"" id=""{0}HiddenControls"" name=""{0}HiddenControls"" value=""{1}""/>",
					Me.ID, Me.HiddenControls)
			End If

			If Me.Script <> "" Then
				sb.AppendFormat("<input type=""hidden"" id=""{0}_Script"" value=""{1}""/>", Me.ID, Me.Script)
			End If

			If Me.ClearScript <> "" Then
				sb.AppendFormat("<input type=""hidden"" id=""{0}_ClearScript"" value=""{1}""/>", Me.ID, Me.ClearScript)
			End If

			sb.AppendFormat("<div id=""{0}Container"" style=""display:none;"" class=""autocompletedrop""></div>", Me.ID)

			writer.Write(sb.ToString)

		End Sub

#End Region

		Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
			If Me.Page.IsPostBack Then
				Me.Value = Me.Page.Request.Form(Me.ID & "Hidden")
				Me.DisplayValue = Me.Page.Request.Form(Me.ID)
			End If
		End Sub

#Region "Restore values and check event"
		Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

			If Me.Command = Me.ID Then
				RaiseEvent Selected(SafeInt(Me.Value), Me.DisplayValue)
			End If
		End Sub
#End Region

	End Class

End Namespace