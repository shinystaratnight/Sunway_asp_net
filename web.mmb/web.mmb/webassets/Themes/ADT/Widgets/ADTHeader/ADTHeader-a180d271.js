var Header=new function(){this.Setup=function(){Header.AttachLanguageEvents()},this.ToggleMenu=function(e){var n=int.f.GetObject(e);int.f.Toggle(n)},this.ChangeCurrency=function(e){var n=e.value;int.ff.Call("Widgets.Header.ChangeCurrency",Header.ChangeCurrencyComplete,n)},this.ChangeCurrencyComplete=function(e){"True"==e&&window.location.reload()},this.AttachLanguageEvents=function(){if(int.f.GetObject("divLanguageSelect"))for(var e=int.f.GetObjectsByIDPrefix("liChangeLanguage_","li","divLanguageSelect"),n=int.f.GetObjectsByIDPrefix("liChangeLanguage_","li","divMobileLanguageSelect"),a=0;a<e.length;a++)int.f.AttachEvent(e[a],"click",function(){Header.ChangeLanguage(this)}),int.f.AttachEvent(n[a],"click",function(){Header.ChangeLanguage(this)})},this.ChangeLanguage=function(e){var n=e.id.split("liChangeLanguage_")[1];int.ff.Call("Widgets.ADTHeader.ChangeLanguage",Header.ChangeLanguageComplete,n)},this.ChangeLanguageComplete=function(e){"True"==e&&window.location.reload()}};$(document).ready(function(){Header.Setup()});