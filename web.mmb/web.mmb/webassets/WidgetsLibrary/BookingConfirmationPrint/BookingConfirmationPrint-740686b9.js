var BookingConfirmationPrint=new function(){this.ViewDocumentation=function(o,n){int.ff.Call("=iVectorWidgets.BookingConfirmationPrint.ShowDocumentation",function(o){BookingConfirmationPrint.ViewDocumentationDone(o)},o,n)},this.ViewDocumentationDone=function(sJSON){var oReturn=eval("("+sJSON+")");if(1==oReturn.OK&&oReturn.DocumentPaths.length>0)for(var i=0;i<oReturn.DocumentPaths.length;i++)window.open(oReturn.DocumentPaths[i],"documentation"+i,"toolbar=no,location=no,menubar=no,copyhistory=no,status=no,directories=no");else web.InfoBox.Show("Sorry, there was a problem generating your documentation.","warning")}};