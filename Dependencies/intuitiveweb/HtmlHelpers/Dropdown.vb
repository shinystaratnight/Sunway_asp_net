Imports System.Collections

Namespace HtmlHelpers

    Public Class Dropdown

#Region "Fields + properties"

        Private Options As New List(Of DropdownOption)
		Private Tag As String = "<select id=""{0}"" class=""{1}"">{2}</select>"

        Public ID As String = ""
        Public ClassName As String = ""
		Public OverrideBlank As String = ""
		Public Script As String = ""

#End Region

#Region "List functions"

        Public Sub Add(dropdownOption As DropdownOption)
            Me.Options.Add(dropdownOption)
        End Sub

        Public Sub AddMultiple(dropdownOptions As List(Of DropdownOption))
            Me.Options.AddRange(dropdownOptions)
        End Sub

        Private Sub Sort()
            Me.Options = Me.Options.OrderBy(Function(o) o.Priority).ThenBy(Function(o) o.Parent).ThenBy(Function(o) o.Key).ToList
        End Sub

        Private Function BuildOptions() As String

            Dim options As String = ""
            Dim currentParent As String = ""

            For Each dropdownOption As DropdownOption In Me.Options

                If Not dropdownOption.Parent = "" AndAlso Not currentParent = "" AndAlso Not currentParent = dropdownOption.Parent Then
                    options += "</optgroup>"
                End If

                If Not dropdownOption.Parent = "" AndAlso Not dropdownOption.Parent = currentParent Then
                    options += String.Format("<optgroup label=""{0}"">", dropdownOption.Parent)
                End If

                If dropdownOption.Key = 0 Then
                    options += String.Format("<option>{1}</option>", dropdownOption.Key, dropdownOption.Value)
                Else
                    options += String.Format("<option value=""{0}"">{1}</option>", dropdownOption.Key, dropdownOption.Value)
                End If

                currentParent = dropdownOption.Parent

            Next

            If Not currentParent = "" Then
                options += "</optgroup>"
            End If

            Return options

        End Function

        Public Sub AddLookup(Lookup As Lookups.LookupTypes, Optional Priority As Integer = 9999)
            Dim oKeyValuePairs As Lookups.KeyValuePairs = BookingBase.Lookups.GenerateKeyValuePairs(Lookup)

            For Each pair As Generic.KeyValuePair(Of Integer, String) In oKeyValuePairs
                Me.Options.Add(New DropdownOption() With {.Key = pair.Key, .Value = pair.Value, .Priority = Priority})
            Next

        End Sub

        Public Sub AddLookupWithParent(Lookup As Lookups.LookupTypes, Optional Priority As Integer = 9999)
            Dim oKeyValuePairsWithParent As Lookups.KeyValuePairsWithParent = BookingBase.Lookups.GenerateKeyValuePairsWithParent(Lookup)

            Me.ProcessLookupWithParent(oKeyValuePairsWithParent, Priority)

        End Sub

        Public Sub ProcessLookupWithParent(pairs As Lookups.KeyValuePairsWithParent, Optional Priority As Integer = 9999)
            For Each pair As Lookups.KeyValuePairWithParent In pairs
                Me.Options.Add(New DropdownOption() With {.Key = pair.ID, .Value = pair.Value, .Parent = pair.Parent, .Priority = Priority})
            Next
        End Sub

#End Region

#Region "Render"

        Public Function Render() As String

            Me.Sort()
            Me.Options.Insert(0, New DropdownOption() With {.Key = 0, .Value = OverrideBlank})
			Return String.Format(Tag, Me.ID, Me.ClassName, Me.BuildOptions())

        End Function


#End Region

#Region "Options class"

        Public Class DropdownOption

            Public Key As Integer
            Public Value As String
            Public Parent As String
            Public Priority As Integer = 9999

        End Class

#End Region


    End Class


End Namespace
