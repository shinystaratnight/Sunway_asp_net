var MyBookingsLogin=MyBookingsLogin||{};MyBookingsLogin.RedirectURL="",MyBookingsLogin.InsertWarningAfter="",MyBookingsLogin.Setup=function(e){var n=new Date,t=MyBookingsLogin.FormatDate(n);MyBookingsLogin.RedirectURL=e,MyBookingsLogin.InsertWarningAfter=int.f.GetValue("hidInsertWarningAfter");var i=int.f.GetValue("hidDatepickerLanguageCodeMyBookings");int.f.SetValue("txtDepartureDate",t);var o={ID:"#txtDepartureDate",MinDate:n,DefaultDate:n,UseShortDates:!0,LanguageCode:i};web.DatePicker.SetupFromObject(o)},MyBookingsLogin.FormatDate=function(e){var n=e.getMonth()+1;return 10>n&&(n="0"+n),e.getDate()+"/"+n+"/"+e.getFullYear()},MyBookingsLogin.Validate=function(e){"BookingDetails"===e?(int.f.SetClassIf("txtBookingRef","error",""===int.f.GetValue("txtBookingRef")),int.f.SetClassIf("txtLastName","error",""===int.f.GetValue("txtLastName")),int.f.SetClassIf("txtDepartureDate","error",""===int.f.GetValue("txtDepartureDate"))):"EmailAndReference"===e?(int.f.SetClassIf("txtEmailAddress","error",""===int.f.GetValue("txtEmailAddress")),int.f.SetClassIf("txtBookingRef","error",""===int.f.GetValue("txtBookingRef"))):"EmailAndPassword"===e&&(int.f.SetClassIf("txtEmailAddress","error",""===int.f.GetValue("txtEmailAddress")),int.f.SetClassIf("txtPassword","error",""===int.f.GetValue("txtPassword")));var n=int.f.GetElementsByClassName("*","error","divBookingLogin");if(0===n.length){var t={};t.BookingReference=int.f.GetValue("txtBookingRef"),"BookingDetails"===e?(t.LastName=int.f.GetValue("txtLastName"),t.DepartureDate=int.f.GetValue("txtDepartureDate")):"EmailAndReference"===e?t.EmailAddress=int.f.GetValue("txtEmailAddress"):"EmailAndPassword"===e&&(t.EmailAddress=int.f.GetValue("txtEmailAddress"),t.Password=int.f.GetValue("txtPassword")),t.LoginType=e;var i=JSON.stringify(t);int.ff.Call("=iVectorWidgets.MyBookingsLogin.Login",function(e){MyBookingsLogin.LoginComplete(e)},i)}else web.InfoBox.Show("Please make sure all fields are entered correctly. Incorrect fields have been highlighted.","warning",null,MyBookingsLogin.InsertWarningAfter)},MyBookingsLogin.LoginComplete=function(sJSON){var oCustomerLoginReturn=eval("("+sJSON+")");oCustomerLoginReturn.OK===!0?web.Window.Redirect(""!==MyBookingsLogin.RedirectURL?MyBookingsLogin.RedirectURL:"/my-bookings"):web.InfoBox.Show("Sorry, your login details were incorrect. Please check and try again.","warning",null,MyBookingsLogin.InsertWarningAfter)},MyBookingsLogin.SendReminder=function(){MyBookingsLogin.ShowWait("aReminder");var e=int.f.GetValue("txtEmailAddress");int.f.SetClassIf("txtEmailAddress","error",!MyBookingsLogin.IsEmail(e)),int.f.RemoveClass("txtBookingRef","error");var n=int.f.GetElementsByClassName("*","error","divBookingLogin");0===n.length?int.ff.Call("=iVectorWidgets.MyBookingsLogin.SendBookingReferencesEmail",function(e){MyBookingsLogin.SendReminder_Done(e)},e):(MyBookingsLogin.RemoveWait("aReminder"),web.InfoBox.Show("Please complete the highlighted fields.","warning",null,MyBookingsLogin.InsertWarningAfter))},MyBookingsLogin.SendReminder_Done=function(e){"failed"===e?(web.InfoBox.Show("There are no bookings associated with the specified email.","warning",null,MyBookingsLogin.InsertWarningAfter),MyBookingsLogin.RemoveWait("aReminder")):(int.f.SetClass("aReminder","done"),int.f.GetObject("aReminder").setAttribute("onclick",""),int.f.SetHTML("aReminder","A reminder has been sent to the email address provided"))},MyBookingsLogin.IsEmail=function(e){var n=/^[a-zA-Z0-9._-]+@([a-zA-Z0-9.-]+\.)+[a-zA-Z0-9.-]{2,4}$/,t=new RegExp(n);return t.test(e)},MyBookingsLogin.ShowWait=function(e){int.f.AddClass(e,"working"),MyBookingsLogin.RemoveText(e)},MyBookingsLogin.RemoveWait=function(e){int.f.RemoveClass(e,"working"),MyBookingsLogin.SetText(e)},MyBookingsLogin.SetText=function(e){"aLogin"===e?int.f.SetHTML(e,"Login"):"aReminder"===e&&int.f.SetHTML(e,"> I do not remember or cannot find my Booking reference.")},MyBookingsLogin.RemoveText=function(e){int.f.SetHTML(e," ")};