var TermsAndConditions=TermsAndConditions||{};TermsAndConditions.Validate=function(n){web.InfoBox.Close(),int.f.RemoveClass(int.f.GetObject("lblTermsAndConditions"),"error"),int.f.SetClassIf("lblTermsAndConditions","error",0==int.cb.Checked("cbTermsAndConditions"));for(var i=int.f.GetObjectsByIDPrefix("cbTermsAndConditions_","input","divTermsAndConditions"),e=0;e<i.length;e++){var o=int.cb.Checked(i[e].id),s=i[e].id.split("_")[2],t=i[e].id.split("_")[1];int.f.SetClassIf("lblTermsAndConditions_"+t+"_"+s,"error",0==o)}null!=n&&void 0!=n&&n()};