var MyBookings=MyBookings||{},me=MyBookings;MyBookings.BookingReference="",MyBookings.ShowCancellationPopup=function(e){me.BookingReference=e,int.ff.Call("=iVectorWidgets.MyBookings.PreCancel",function(e){MyBookings.RequestPreCancelComplete(e)},me.BookingReference),web.ModalPopup.Show("divCancellation",!0,"modalpopup container")},MyBookings.RequestPreCancelComplete=function(sJSON){var oPreCancelReturn=eval("("+sJSON+")");1==oPreCancelReturn.OK?(int.f.SetHTML("spnCancellationCost",oPreCancelReturn.CancellationCost),int.f.SetValue("hidCancellationToken",oPreCancelReturn.CancellationToken),web.ModalPopup.Show("divCancellation",!0,"modalpopup container")):web.InfoBox.Show("Sorry, there was a problem calculating the cancellation charges.","warning")},MyBookings.RequestCancellationValidate=function(){var e=int.f.GetHTML("spnCancellationCost"),t=int.f.GetValue("hidCancellationToken");int.f.SetClassIf("lblConfirmCancel","error",!int.cb.Checked("chkConfirmCancel")),int.cb.Checked("chkConfirmCancel")&&(int.f.Show("divCancellationWaitMessage"),int.f.Hide("divCancellationForm"),int.ff.Call("=iVectorWidgets.MyBookings.CancelBooking",function(e){MyBookings.CancelBookingComplete(e)},e,t,me.BookingReference))},MyBookings.CancelBookingComplete=function(sJSON){int.f.Hide("divCancellationWaitMessage");var oCancelReturn=eval("("+sJSON+")");if(1==oCancelReturn.OK){int.f.Show("divCancellationConfirmation"),int.f.Hide("divTotalPrice_"+me.BookingReference);var oNotification=document.createElement("div");oNotification.className="cancelledBooking",oNotification.innerHTML="This booking has been cancelled",int.f.GetObject("divBooking_"+me.BookingReference).appendChild(oNotification)}else int.f.Show("divCancellationFailed")},MyBookings.ShowAmendmentPopup=function(e){int.f.Show("divAmendmentForm"),int.f.Hide("divRequestSent");var t=new Date,n=new Date;int.d.AddDays(n,1);var o=int.f.GetValue("hidUseShortDates");int.f.SetValue("txtAmendmentDepartureDate",MyBookings.FormatDate(t)),web.DatePicker.Setup("#txtAmendmentDepartureDate",t,t,1,1,0,null,"checkin",!1,"","","","","",o),int.f.SetValue("txtAmendmentReturnDate",MyBookings.FormatDate(n)),web.DatePicker.Setup("#txtAmendmentReturnDate",n,n,1,1,0,null,"checkout",!1,"","","","","",o),me.BookingReference=e,web.ModalPopup.Show("divAmendment",!0,"modalpopup container booking-amendment")},MyBookings.FormatDate=function(e){var t=e.getMonth()+1;return 10>t&&(t="0"+t),e.getDate()+"/"+t+"/"+e.getFullYear()},MyBookings.RequestAmendmentValidate=function(){var e=int.f.GetValue("txtAmendmentDepartureDate"),t=int.f.GetValue("txtAmendmentReturnDate"),n=int.d.New(e.split("/")[0],e.split("/")[1],e.split("/")[2]),o=int.d.New(t.split("/")[0],t.split("/")[1],t.split("/")[2]),i=int.d.DateDiff(n,o);int.f.SetClassIf("txtAmendmentReturnDate","error",1>i),int.f.SetClassIf("txtAmendmentDepartureDate","error",!int.d.IsDate(n)||""==int.f.GetValue("txtAmendmentDepartureDate")),int.f.SetClassIf("txtAmendmentReturnDate","error",!int.d.IsDate(o)||""==int.f.GetValue("txtAmendmentReturnDate")||1>i),int.f.SetClassIf("ddlTotalRate","error",""==int.dd.GetText("ddlTotalRate")),int.f.SetClassIf("lblAuthorise","error",0==int.f.GetObject("chkAuthorise").checked);var a=int.f.GetElementsByClassName("*","error","divAmendment");0==a.length?(int.f.Hide("divAmendmentWarning"),int.f.Hide("divAmendmentForm"),int.f.Show("divAmendRequestWaitMessage"),int.ff.Call("=iVectorWidgets.MyBookings.RequestAmendment",function(e){MyBookings.RequestAmendmentComplete(e)},me.BookingReference,n,o,int.f.GetValue("txtDestination"),int.f.GetValue("txtHotelName"),int.dd.GetText("ddlTotalRate"),int.f.GetValue("txtAdditionalInformation"))):int.f.Show("divAmendmentWarning")},MyBookings.RequestAmendmentComplete=function(e){"Success"==e&&(int.f.Hide("divAmendRequestWaitMessage"),int.f.Hide("divAmendmentForm"),int.f.Show("divRequestSent"),int.f.SetHTML("h3divAmendment","Booking Amendment Request Sent"))},MyBookings.ViewDocumentation=function(e){web.ModalPopup.Show("divViewDocuments"),int.ff.Call("=iVectorWidgets.MyBookings.ShowDocumentation",function(e){MyBookings.ViewDocumentationDone(e)},e)},MyBookings.ViewDocumentationDone=function(e){web.ModalPopup.Hide();var t=JSON.parse(e);if(1==t.OK&&t.Keys.length>0)for(var n=0;n<t.Keys.length;n++)window.open("/services/view-documentation/"+t.Keys[n],"documentation"+n,"toolbar=no,location=no,menubar=no,copyhistory=no,status=no,directories=no");else web.InfoBox.Show("Sorry, there was a problem generating your documentation.","warning")},MyBookings.SendDocumentation=function(e){web.ModalPopup.Show("divEmailDocuments"),int.ff.Call("=iVectorWidgets.MyBookings.SendDocumentation",function(e){MyBookings.SendDocumentationDone(e)},e)},MyBookings.SendDocumentationDone=function(sJSON){var oReturn=eval("("+sJSON+")");1==oReturn.OK?(int.f.Hide("divEmailWaitMessage"),int.f.Show("divEmailSuccess"),setTimeout("web.ModalPopup.Hide()",2e3)):(web.InfoBox.Show("Sorry, there was a problem sending your documentation.","warning"),web.ModalPopup.Hide())},MyBookings.EditSeatReservations=function(e,t){int.f.Show("divSeatSelection_"+e),1==t&&me.ChangeSeatSelectionTab(e),int.f.GetObject("divSeatsConfirmed_"+e)&&int.f.Hide("divSeatsConfirmed_"+e)},MyBookings.ChangeSeatSelectionTab=function(e){for(var t=int.f.GetElementsByClassName("li","tab","divSeatSelection_"+e),n=0;n<t.length;n++)int.f.ToggleClass(t[n],"selected");for(var o=int.f.GetElementsByClassName("div","tabContent","divSeatSelection_"+e),n=0;n<o.length;n++)int.f.Toggle(o[n])},MyBookings.SelectSeat=function(e,t,n,o){web.InfoBox.Close();for(var i=int.f.GetObject("ddlSeats_"+e+"_"+t+"_"+n),a=int.dd.GetValue(i).split("_"),l=int.f.GetObjectsByIDPrefix("ddlSeats_","select","divSeatSelection_"+e),s=!1,r=0;r<l.length;r++)l[r]!=i&&int.dd.GetValue(l[r])==int.dd.GetValue(i)&&(s=!0);s&&0!=a[0]?(web.InfoBox.Show("Seat "+int.dd.GetText(i)+" has already been selected for another passenger, please reselect a different option.","warning"),i.selectedIndex=0,int.f.SetHTML("spnSeatPrice_"+e+"_"+t+"_"+n,0)):int.f.SetHTML("spnSeatPrice_"+e+"_"+t+"_"+n,a[1]);for(var d=0,c=0;c<l.length;c++)d+=int.n.SafeNumeric(int.dd.GetValue(l[c]).split("_")[1]);var f=d-o;0>f&&(f=0),int.f.SetHTML("spnTotalSeatPrice_"+e,d),int.f.SetHTML("spnAmountDue_"+e,f)},MyBookings.ConfirmSeatSelection=function(e,t,n){for(var o={BasketFlightExtras:[]},i=int.f.GetObjectsByIDPrefix("ddlSeats_","select","divSeatSelection_"+t),a=0;a<i.length;a++){var l=i[a].id.split("_"),s=int.dd.GetValue(i[a]).split("_");if(0!=s[0]){var r={ExtraBookingToken:s[0],QuantitySelected:1,ExtraType:"Seat",GuestID:l[2],Price:s[1]};o.BasketFlightExtras.push(r)}}int.ff.Call("=iVectorWidgets.MyBookings.AddSeatsToBasket",MyBookings.AddSeatsToBasketComplete,e,t,n,JSON.stringify(o))},MyBookings.AddSeatsToBasketComplete=function(sCompleteFunction){web.InfoBox.Close();try{var oCompleteFunction=eval("("+sCompleteFunction+")");oCompleteFunction&&oCompleteFunction()}catch(exception){web.InfoBox.Show("Sorry, there was a problem reserving your seats. Please try again or choose an alternative option.","warning")}},MyBookings.ReserveSeatsDone=function(e){""==e?int.ff.Call("=iVectorWidgets.MyBookings.UpdateMyBookingsHTML",function(e){int.f.SetHTML("divMyBookings",e)},"true"):web.InfoBox.Show(e,"warning")},MyBookings.LogOut=function(){var e="/booking-login";""!=int.f.GetValue("hidLogoutRedirectURL")&&(e=int.f.GetValue("hidLogoutRedirectURL")),int.ff.Call("=iVectorWidgets.MyBookings.LogOut",function(){web.Window.Redirect(e)})};