var LeadGuestDetails=LeadGuestDetails||{};LeadGuestDetails.CompleteFunction=null,LeadGuestDetails.Validate=function(e){web.InfoBox.Close();var t=int.f.GetValue("hidLeadGuestDetails_ValidationExclude").split(",");if(LeadGuestDetails.CompleteFunction=e,!int.f.GetObject("divLeadGuestDetails"))return void LeadGuestDetails.AddDetailsToBasketComplete();int.f.SetClassIf("ddlLeadGuestDetails_Title","error",0==int.dd.GetIndex("ddlLeadGuestDetails_Title"));var s=int.f.GetValue("txtLeadGuestDetails_FirstName");int.f.SetClassIf("txtLeadGuestDetails_FirstName","error",""==s||!int.v.IsAlpha(s));var a=int.f.GetValue("txtLeadGuestDetails_LastName");int.f.SetClassIf("txtLeadGuestDetails_LastName","error",""==a||!int.v.IsAlpha(a)),int.f.SetClassIf("txtLeadGuestDetails_Email","error",0==int.v.IsEmail(int.f.GetValue("txtLeadGuestDetails_Email"))),int.f.SetClassIf("txtLeadGuestDetails_RepeatEmail","error",0==int.v.IsEmail(int.f.GetValue("txtLeadGuestDetails_RepeatEmail"))||int.f.GetValue("txtLeadGuestDetails_RepeatEmail")!=int.f.GetValue("txtLeadGuestDetails_Email")),int.f.SetClassIf("txtLeadGuestDetails_Address","error",""==int.f.GetValue("txtLeadGuestDetails_Address")),int.f.GetObject("ddlDOBDay")&&int.f.SetClassIf("ddlDOBDay","error",""==int.f.GetValue("ddlDOBDay")),int.f.GetObject("ddlDOBMonth")&&int.f.SetClassIf("ddlDOBMonth","error",""==int.f.GetValue("ddlDOBMonth")),int.f.GetObject("ddlDOBYear")&&int.f.SetClassIf("ddlDOBYear","error",""==int.f.GetValue("ddlDOBYear")),int.f.GetObject("txtLeadGuestDetails_Postcode")&&!int.a.ArrayContains(t,"Postcode")&&int.f.SetClassIf("txtLeadGuestDetails_Postcode","error",""==int.f.GetValue("txtLeadGuestDetails_Postcode")),int.f.GetObject("txtLeadGuestDetails_Passport")&&int.f.SetClassIf("txtLeadGuestDetails_Passport","error",""==int.f.GetValue("txtLeadGuestDetails_Passport")),int.f.SetClassIf("txtLeadGuestDetails_City","error",""==int.f.GetValue("txtLeadGuestDetails_City")),int.f.SetClassIf("ddlLeadGuestDetails_BookingCountry","error",0==int.dd.GetIntValue("ddlLeadGuestDetails_BookingCountry"));var d=int.f.GetValue("txtLeadGuestDetails_PhoneNumber");int.f.SetClassIf("txtLeadGuestDetails_PhoneNumber","error",""==d||!int.v.IsNumericPhoneNumber(d));var i=int.f.GetElementsByClassName("","error","divLeadGuestDetails").length;0==i?LeadGuestDetails.AddDetailsToBasket():LeadGuestDetails.AddDetailsToBasketComplete()},LeadGuestDetails.AddDetailsToBasket=function(){var e={CustomerTitle:int.dd.GetValue("ddlLeadGuestDetails_Title"),CustomerFirstName:int.f.GetValue("txtLeadGuestDetails_FirstName"),CustomerLastName:int.f.GetValue("txtLeadGuestDetails_LastName"),CustomerEmail:int.f.GetValue("txtLeadGuestDetails_Email"),CustomerAddress1:int.f.GetValue("txtLeadGuestDetails_Address"),CustomerAddress2:int.f.SafeObject("txtLeadGuestDetails_Address2")?int.f.GetValue("txtLeadGuestDetails_Address2"):"",CustomerTownCity:int.f.GetValue("txtLeadGuestDetails_City"),CustomerCounty:int.f.SafeObject("txtLeadGuestDetails_County")?int.f.GetValue("txtLeadGuestDetails_County"):"",CustomerPostcode:int.f.GetValue("txtLeadGuestDetails_Postcode"),CustomerBookingCountryID:int.dd.GetIntValue("ddlLeadGuestDetails_BookingCountry"),CustomerPhone:int.f.GetValue("txtLeadGuestDetails_PhoneNumber"),CustomerMobile:int.f.SafeObject("txtLeadGuestDetails_MobileNumber")?int.f.GetValue("txtLeadGuestDetails_MobileNumber"):"",CustomerPassportNumber:int.f.SafeObject("txtLeadGuestDetails_Passport")?int.f.GetValue("txtLeadGuestDetails_Passport"):"",DateOfBirth:int.f.SafeObject("ddlDOBDay")?int.d.New(int.f.GetValue("ddlDOBDay"),int.f.GetValue("ddlDOBMonth"),int.f.GetValue("ddlDOBYear")):int.d.New("1","1","1900")},t=JSON.stringify(e);int.ff.Call("=iVectorWidgets.LeadGuestDetails.AddDetailsToBasket",function(){LeadGuestDetails.AddDetailsToBasketComplete()},t)},LeadGuestDetails.AddDetailsToBasketComplete=function(){null!=LeadGuestDetails.CompleteFunction&&void 0!=LeadGuestDetails.CompleteFunction&&LeadGuestDetails.CompleteFunction()},LeadGuestDetails.CapitaliseFirstLetter=function(e){var t=e.value,s=int.s.Left(t,1),a=int.s.Substring(t,1,t.length);int.f.SetValue(e,s.toUpperCase()+a)},LeadGuestDetails.FindAddresses=function(){var e=int.f.GetValue("txtPostcodeLookup_Postcode");""!=e&&int.ff.Call("=iVectorWidgets.LeadGuestDetails.FindAddresses",LeadGuestDetails.FindAddressesReturn,e)},LeadGuestDetails.FindAddressesReturn=function(e){""!=e?(document.getElementById("ddlPostcodeLookup_Addresses").innerHTML=e,document.getElementById("trPostcodeLookup_Select").style.display="block",int.f.AttachEvent("ddlPostcodeLookup_Addresses","change",function(){LeadGuestDetails.SelectAddress()})):(web.ModalPopup.Show("<div><h3>No addresses found.</h3><p>Please check your postcode or enter your adddress manually.</p></div>",!0,"modalpopup PostcodeError"),document.getElementById("trPostcodeLookup_Select").style.display="none",int.f.AttachEvent("divOverlay","click",function(){web.ModalPopup.Hide()}))},LeadGuestDetails.SelectAddress=function(){var e=document.getElementById("ddlPostcodeLookup_Addresses"),t=e.options[e.selectedIndex].value;"0"!=t&&null!=t&&void 0!=t&&int.ff.Call("=iVectorWidgets.LeadGuestDetails.SelectAddress",LeadGuestDetails.SelectAddressReturn,t)},LeadGuestDetails.SelectAddressReturn=function(e){var t=JSON.parse(e);""!=t.Address1&&""!=t.Address2?document.getElementById("txtLeadGuestDetails_Address").value=t.Address1+", "+t.Address2:""!=t.Address1&&(document.getElementById("txtLeadGuestDetails_Address").value=t.Address1),""!=t.TownCity&&(document.getElementById("txtLeadGuestDetails_City").value=t.TownCity),""!=t.Postcode&&(document.getElementById("txtLeadGuestDetails_Postcode").value=t.Postcode),""!=t.BookingCountryID&&(document.getElementById("ddlLeadGuestDetails_BookingCountry").value=t.BookingCountryID)},LeadGuestDetails.Setup=function(){var e=document.getElementById("aPostcodeLookup_Find");e&&int.f.AttachEvent(e,"click",function(){LeadGuestDetails.FindAddresses()})},$(document).ready(function(){LeadGuestDetails.Setup()});