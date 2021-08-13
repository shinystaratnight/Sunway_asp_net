Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Namespace Widgets
    Public Class Warning
        Inherits iVectorWidgets.Warning
        Public Overrides Sub Draw(ByVal writer As System.Web.UI.HtmlTextWriter)

            'get warning type
            Dim sWarningType As String = Me.Page.Request.QueryString("warn")

            'check if we have a specific div to insert the message after
            Dim sInsertWarningAfter As String = Functions.SafeString(Settings.GetValue("InsertWarningAfter"))

            If Not sWarningType = "" Then

                'get warning messages xml
                Dim oWarningMessagesXML As XmlDocument = Utility.BigCXML(Settings.GetValue("ObjectType"), 1, 60)

                Dim oNodes As XmlNodeList = oWarningMessagesXML.SelectNodes("WarningMessages/WarningMessage[Type='" & sWarningType & "']")

                Dim sb As New StringBuilder
                sb.Append("<script type=""text/javascript"">")
                For Each oNode As XmlNode In oNodes
                    Dim sMessage As String = XMLFunctions.SafeNodeValue(oNode, "Message")

                    'escape apostrophes
                    sMessage = sMessage.Replace("'", "/'")

                    'If this is a price change message then we need to make the user aggree to it.
                    If sWarningType = "pricechange" Then

                        'Get the amount from the query string.
                        Dim sPriceChangeAmount As String = Intuitive.ToSafeString(Format(Double.Parse(Me.Page.Request.QueryString("amount")), "0.00"))

                        Dim sCurrencySymbol As String = Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbol, BookingBase.CurrencyID)
                        Dim sCurrencySymbolPosition As String = Lookups.GetKeyPairValue(Lookups.LookupTypes.CurrencySymbolPosition, BookingBase.CurrencyID)

                        'Add on the currency sybol
                        If sCurrencySymbolPosition = "Prepend" Then
                            sPriceChangeAmount = sCurrencySymbol & sPriceChangeAmount
                        Else
                            sPriceChangeAmount = sPriceChangeAmount & sCurrencySymbol
                        End If

                        'inject the price into the message.
                        sMessage = sMessage.Replace("(0)", sPriceChangeAmount)

                        'inject the price message using the {0} standard. (0) line left in to prevent issues with old messages
                        sMessage = sMessage.Replace("{0}", sPriceChangeAmount)

                        sb.Append("int.ll.OnLoad.Run(function () {  ")
                        sb.AppendFormat("web.InfoBox.Show('{0}', 'warning', 'True'); ", sMessage) 'keep the end space here!

                    Else
                        sb.Append("int.ll.OnLoad.Run(function () {  ")

                        Dim sWarning As New StringBuilder()
                        sWarning.AppendFormat("<p class=""icon warning_sign"" id=""pWarning""> <i></i> {0} </p>", sMessage)

                        If sInsertWarningAfter <> "" Then
                            sb.AppendFormat("web.InfoBox.Show('{0}', 'warning', null, '{1}'); ", sMessage, sInsertWarningAfter)  'keep the end space here!
                        Else
                            sb.AppendFormat("web.ModalPopup.Show('{0}', true, 'modalpopup infobox searchtool warning', '200','500'); ", sWarning.ToString) 'keep the end space here!
                        End If

                    End If


                    sb.Append(" })")
                Next

                sb.Append("</script>")

                writer.Write(sb.ToString)

            End If

        End Sub

    End Class
End Namespace

