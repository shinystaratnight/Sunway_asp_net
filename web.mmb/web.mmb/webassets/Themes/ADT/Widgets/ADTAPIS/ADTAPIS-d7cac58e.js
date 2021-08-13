var ADTAPIS=ADTAPIS||{};ADTAPIS.Setup=function(){ADTAPIS.SetupDatePickers(),ADTAPIS.AttachValidate(),ADTAPIS.AttachCollapsePassenger()},ADTAPIS.SetupDatePickers=function(){var e=new Date,t=int.d.DatepickerDate(e),a=(new Date(2e3,1,1),int.f.GetElementsByClassName("INPUT","calendar","divADTAPIS"));for(i=0;i<a.length;i++)a[i].value=t,web.DatePicker.Setup("#"+a[i].id,"","","","","","",a[i].id,"True")},ADTAPIS.AttachValidate=function(){var e=int.f.GetObjectsByIDPrefix("btnUpdateApis_","a","divADTAPIS");for(i=0;i<e.length;i++)int.f.AttachEvent(e[i],"click",ADTAPIS.Validate.bind(null,e[i].id.replace("btnUpdateApis_","")))},ADTAPIS.AttachCollapsePassenger=function(){var e=ADTAPIS.GetObjectsByIDPrefixWithoutClass("APIS_","div","divADTAPIS","closed");for(i=0;i<e.length;i++){var t=int.f.GetObjectsByIDPrefix(e[i].id,"h2",e[i].id);for(j=0;j<t.length;j++)int.f.AttachEvent(t[j],"click",ADTAPIS.TogglePassengerDivCollapse.bind(null,t[j].id)),int.s.EndsWith(t[j].id,"_1")&&ADTAPIS.TogglePassengerDivCollapse(t[j].id)}},ADTAPIS.TogglePassengerDivCollapse=function(e){var t=document.getElementById(e+"_Collapsable");int.f.HasClass(t,"closed")?(int.f.RemoveClass(e,"closed"),int.f.RemoveClass(t,"closed")):(int.f.AddClass(e,"closed"),int.f.AddClass(t,"closed"))},ADTAPIS.GetObjectsByIDPrefixWithoutClass=function(e,t,i,a){i=void 0==i?document:int.f.SafeObject(i),void 0==t&&(t="input");for(var s=new Array,n=i.getElementsByTagName(t),r=0;r<n.length;r++)int.s.StartsWith(n[r].id,e)&&!int.f.HasClass(n[r].id,a)&&s.push(n[r]);return s},ADTAPIS.Validate=function(e){var t="APIS_"+e,a=(document.getElementById(t),int.f.GetObjectsByIDPrefix(t+"_","div",t));for(i=0;i<a.length;i++)ADTAPIS.ValidatePassenger(a[i].id);var s=int.f.GetElementsByClassName("*","error","divADTAPIS");if(0==s.length){var n=ADTAPIS.BuildJSON(a),r={BookingReference:int.f.GetValue("hidAPISBookingReference"),FlightBookingReference:int.f.GetValue("hidAPISFlightBookingReference"),SupplierReference:int.f.GetValue("hidAPISSupplierReference")},l=JSON.stringify(r);int.f.Hide("divAPISWarning"),int.f.Hide("divADTAPIS"),int.f.Show("divAPISWaitMessage"),int.ff.Call("Widgets.ADTAPIS.SubmitApisInformation",function(e){ADTAPIS.SubmitApisInformationComplete(e)},n,l)}else int.f.Show("divAPISWarning")},ADTAPIS.BuildJSON=function(e){function t(){this.Title,this.FirstName,this.MiddleName,this.LastName,this.DateOfBirth,this.Nationality,this.NationalityCode,this.PassportNumber,this.PassportIssueDate,this.PassportExpiryDate,this.PassportIssuePlaceID,this.PassportIssuePlaceName,this.FlightBookingPassengerID,this.Gender}var a=[];for(i=0;i<e.length;i++){var s=new t;s.FlightBookingPassengerID=int.n.SafeInt(int.f.GetValue(int.f.GetObjectsByIDPrefix("APISPassengerID","INPUT",e[i].id)[0])),s.Title=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISTitle","INPUT",e[i].id)[0]),s.FirstName=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISFirstName","INPUT",e[i].id)[0]),s.MiddleName=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISMiddleName","INPUT",e[i].id)[0]),s.LastName=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISLastName","INPUT",e[i].id)[0]),s.DateOfBirth=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISDateOfBirth","INPUT",e[i].id)[0]),s.Gender=int.f.GetRadioButtonValue(int.f.GetObjectsByIDPrefix("APISGender","INPUT",e[i].id)[0].name),s.PassportNumber=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISPassportNumber","INPUT",e[i].id)[0]),s.PassportIssueDate=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISPassportIssueDate","INPUT",e[i].id)[0]),s.PassportExpiryDate=int.f.GetValue(int.f.GetObjectsByIDPrefix("APISPassportExpiryDate","INPUT",e[i].id)[0]);var n=int.f.GetObjectsByIDPrefix("APISNationality","select",e[i].id)[0];s.Nationality=n.options[n.selectedIndex].text,s.NationalityCode=n.options[n.selectedIndex].value;var r=int.f.GetObjectsByIDPrefix("APISPassportIssuePlace","select",e[i].id)[0];s.PassportIssuePlaceID=int.n.SafeInt(r.options[r.selectedIndex].value),s.PassportIssuePlaceName=r.options[r.selectedIndex].text,a.push(s)}return JSON.stringify(a)},ADTAPIS.SubmitApisInformationComplete=function(e){"True"==e?(int.f.Hide("divAPISWaitMessage"),int.f.Show("divAPISRequestSent")):(int.f.Hide("divAPISWaitMessage"),int.f.Show("divAPISRequestNotSent"))},ADTAPIS.ValidatePassenger=function(e){var t=int.f.GetElementsByClassName("*","error",e);t.forEach(function(e){int.f.RemoveClass(e,"error")});var i=int.f.GetElementsByClassName("","validate",e);i.forEach(function(e){ADTAPIS.ValidateInputs(e)})},ADTAPIS.ValidateInputs=function(e){switch(e.type){case"select-one":int.f.SetClassIf(e,"error",""==e.value);break;case"radio":int.f.SetClassIf(e,"error",""==int.f.GetRadioButtonValue(e.name));break;case"text":if("APISMiddleName"==e.id)int.f.SetClassIf(e,"error",!int.v.IsAlpha(e.value));else if("APISPassportNumber"==e.id){var t=new RegExp(/^[a-zA-Z0-9]+$/);int.f.SetClassIf(e,"error",!t.test(e.value))}}},$(document).ready(function(){ADTAPIS.Setup()});