Imports System.Web.UI
Imports Intuitive.Functions
Namespace WebControls
    Public Class DropDown
        Inherits WebControlBase
        Implements IPostBackDataHandler

        Private sOptions As String = ""
        Private bAddBlank As Boolean = True
        Private bAddAll As Boolean = False
        Private sValue As String = ""
        Private bIsValid As Boolean = True
        Private bAutoPostBack As Boolean = False
        Private sCssClass As String = ""
        Private sScript As String = ""
        Private bAutoFilter As Boolean = True
        Private bAlwaysUseOnChange As Boolean = False
        Private sMultilingualTag As String = ""
        Public AutoGroupType As String = "Class"
        Public Property OverrideBlankText As String = ""

#Region "Properties"

        Public Property Options() As String
            Get
                Return sOptions
            End Get
            Set(ByVal Value As String)
                sOptions = Value
            End Set
        End Property

        Public Property AddBlank() As Boolean
            Get
                Return bAddBlank
            End Get
            Set(ByVal Value As Boolean)
                bAddBlank = Value
            End Set
        End Property

        Public Property AddAll() As Boolean
            Get
                Return bAddAll
            End Get
            Set(ByVal Value As Boolean)
                bAddAll = Value
            End Set
        End Property

        Public Overrides Property Value() As String
            Get
                Return sValue
            End Get
            Set(ByVal Value As String)
                sValue = Value
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

        Public Property AutoPostBack() As Boolean
            Get
                Return bAutoPostBack
            End Get
            Set(ByVal Value As Boolean)
                bAutoPostBack = Value
            End Set
        End Property

        Public ReadOnly Property HasValue() As Boolean
            Get
                Return sValue <> ""
            End Get
        End Property

		Public Overloads Property CSSClass() As String
			Get
				Return sCssClass
			End Get
			Set(ByVal Value As String)
				sCssClass += " " & Value
			End Set
		End Property
		Public Property Script() As String
            Get
                Return sScript
            End Get
            Set(ByVal Value As String)
                sScript = Value
            End Set
        End Property
        Public Property AutoFilter() As Boolean
            Get
                Return bAutoFilter
            End Get
            Set(ByVal Value As Boolean)
                bAutoFilter = Value
            End Set
        End Property
        Public Property AlwaysUseOnChange() As Boolean
            Get
                Return bAlwaysUseOnChange
            End Get
            Set(ByVal Value As Boolean)
                bAlwaysUseOnChange = Value
            End Set
        End Property

        Public Property MultilingualTag() As String
            Get
                Return sMultilingualTag
            End Get
            Set(ByVal Value As String)
                sMultilingualTag = Value
            End Set
        End Property
#End Region

#Region "Viewstate Management"
        Protected Overrides Function SaveViewState() As Object
            Dim oState(3) As Object

            oState(0) = MyBase.SaveViewState
            oState(1) = sOptions
            oState(2) = bAutoPostBack

            Return oState
        End Function

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)

            If Not savedState Is Nothing Then
                Dim oState As Object() = CType(savedState, Object())

                MyBase.LoadViewState(oState(0))

                sOptions = CType(oState(1), String)
                bAutoPostBack = CType(oState(2), Boolean)

            End If
        End Sub

#End Region

#Region "Add Numberic options"
        Public Sub AddNumbericOptions(ByVal iMax As Integer, Optional ByVal iMin As Integer = 0)
            Dim sOptions As String = ""
            For i As Integer = iMin To iMax
                sOptions += i & "#"
            Next
            If sOptions.Length > 1 Then
                sOptions = sOptions.Substring(0, sOptions.Length - 1)
            End If
            Me.Options = sOptions
        End Sub
#End Region

#Region "Contains Value"

        Public Function ContainsValue(ByVal sValue As String) As Boolean

            For Each sOption As String In Me.Options.Split("#"c)

                Dim aOption() As String = sOption.Split("|"c)
                If aOption.Length > 1 AndAlso sValue = aOption(1) Then
                    Return True
                ElseIf sOption = sValue Then
                    Return True
                End If

            Next

            Return False

        End Function

#End Region

        Public Event ItemSelected(ByVal Value As String)

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

			'convention if you want to display a # in the dropdown use the unlikely combo ~Hash~
			Dim aOptions() As String = sOptions.Split("#"c)
			Dim sClass As String
            Dim sOption As String
            Dim aOption() As String
            Dim sAutoPostbackJS As String = ""
            Dim sTabIndex As String = ""
            Dim sML As String = ""

            'Sort out script property if we have one
            Dim sOnChange As String = ""
            If sScript <> "" Then sOnChange = "onchange=""" & sScript & """"
            If sMultilingualTag <> "" Then sML = " ml=""" & sMultilingualTag & """"

            'if not enabled then just write out the thingy with the value (and the onchange if required)
            If Not Me.Enabled Then
                sClass = "dropdown readonly"
                sClass += IIf(Me.ControlSize = WebControlBase.ControlSizes.Small, " small",
                    IIf(Me.ControlSize = WebControlBase.ControlSizes.Large, " large", "")).ToString

                writer.Write(String.Format("<select id=""{0}"" name=""{0}"" {3} class=""{1}""{4} {2}>",
                    Me.ID, sClass, sTabIndex, IIf(bAlwaysUseOnChange, sOnChange, ""), sML))

                Dim sDisplayValue As String = Me.GetDisplayValue
                writer.Write(String.Format("<option value=""{0}"">{1}</option>", sValue, sDisplayValue.Replace("""", "&#34;")))

                writer.Write("</select>")
                Return
            End If

            'postback
            If bAutoPostBack Then
                sAutoPostbackJS = String.Format("onchange=""javascript:Postback('{0}','')""", Me.ID)
            End If

            'tab index
            If Me.TabIndex > 0 Then
                sTabIndex = String.Format("tabindex=""{0}""", Me.TabIndex)
            End If

            'Sort out class
            sClass = "dropdown" & sCssClass
            sClass += IIf(bIsValid, "", " error").ToString
            sClass += IIf(Me.ControlSize = WebControlBase.ControlSizes.Small, " small",
                IIf(Me.ControlSize = WebControlBase.ControlSizes.Large, " large", "")).ToString

            'If we have autofilter set to true then set up the onkeypress attribute
            'and add a hidden control to aid in the searching
            Dim sAutoFilter As String = ""
            If bAutoFilter Then
                sAutoFilter = String.Format(" onkeypress=""javascript:DropDownSelect(this, event, '{0}');" &
                    "return false;"" onfocus=""ClearSelection(this);""", bAutoPostBack.ToString.ToLower)
                writer.Write(String.Format("<input type=""hidden"" id=""hid{0}"" name=""hid{0}""/>", Me.ID))
            End If

            'select list open tag
            writer.Write(String.Format("<select id=""{0}"" name=""{0}"" class=""{1}"" {2} {3} {4}{5}{6}>",
                Me.ID, sClass, sOnChange, sAutoPostbackJS, sTabIndex, sAutoFilter, sML))

            'add blank item
            If bAddBlank Then
                writer.Write(String.Format("<option>{0}</option>", Me.OverrideBlankText))
            End If

            'add 'All' item
            If bAddAll Then
                writer.Write("<option>All</option>")
            End If

            If aOptions.Length > 0 Then

                'add the rest of the items
                'if we have values specified then do something a bit different
                Dim sVal As String
                Dim sInsertVal As String
                Dim sDisplay As String
                Dim sSelected As String
                Dim sGroup As String = ""
                Dim sLastGroup As String = ""

                For i As Integer = 0 To aOptions.Length - 1

                    sOption = aOptions(i)

					'see if we have just an option or an option/value pair
					aOption = sOption.Split("|"c)
					If aOption.Length = 1 Then
                        sDisplay = sOption.Replace("~Hash~", "#")
                        sVal = ""
                        sInsertVal = " value=""" & sDisplay & """"
                    Else
                        sDisplay = aOption(0).Replace("~Hash~", "#")
                        sVal = aOption(1)
                        sInsertVal = " value=""" & sVal & """"
                    End If

                    If sDisplay.IndexOf("~") > 0 Then
                        sGroup = sDisplay.Split("~".ToCharArray)(0)
                        If sGroup <> sLastGroup Then

                            If Me.AutoGroupType.ToLower = "class" Then
                                writer.Write(String.Format("<option class=""dropdowngroup"" value=""0"">{0}</option>", sGroup.Replace("""", "&#34;")))
                            ElseIf Me.AutoGroupType.ToLower = "standard" Then

                                'close off last group
                                If sLastGroup <> "cockpisspartridge" Then
                                    writer.Write("</optgroup>")
                                End If

                                writer.Write(String.Format("<optgroup label=""{0}"">", sGroup.Replace("""", "&#34;")))
                            End If

                        End If
                        sDisplay = sDisplay.Split("~".ToCharArray)(1)
                    End If

                    'set selected if necessary
                    If sDisplay = sValue OrElse (sVal <> "" AndAlso sVal = sValue) Then
                        sSelected = " selected=""selected"""
                    Else
                        sSelected = ""
                    End If

                    writer.Write(String.Format("<option{0}{1}>{2}</option>",
                        sInsertVal, sSelected, sDisplay.Replace("""", "&#34;")))

                    'close off the optgroup if last row?
                    If sDisplay.IndexOf("~") > 0 AndAlso Me.AutoGroupType.ToLower = "standard" AndAlso sLastGroup <> "cockpisspartridge" AndAlso i = aOptions.Length - 1 _
                                AndAlso sDisplay.IndexOf("~") > -1 Then

                        writer.Write("</optgroup>")
                    End If

                    sLastGroup = sGroup
                Next
            End If

            writer.Write("</select>")
        End Sub

#Region "Postback Handling"
        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData

            'set the value
            sValue = postCollection(postDataKey)

            'return true if this control has posted back
            Return Me.Command = Me.ID
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
            RaiseEvent ItemSelected(sValue)
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            If Me.Page.Request(Me.ID) IsNot Nothing Then
                sValue = SafeString(Me.Page.Request(Me.ID))
            End If
        End Sub

#End Region

#Region "Helper Functions"

		Public Function GetDisplayValue() As String

			For Each sOption As String In Me.Options.Split("#"c)

				Dim aOption() As String = sOption.Split("|"c)
				If aOption.Length > 1 AndAlso sValue = aOption(1) Then
					Return aOption(0)
				ElseIf sOption = sValue Then
					Return sOption
				End If

			Next

			Return ""
		End Function

#End Region

	End Class

End Namespace