var HotelResultsFooter=new function(){var t=this;this.Setup=function(){int.f.AttachEvent("btnBackToTop","click",function(){window.scrollTo(0,0)})},this.Hide=function(){int.f.Hide("divHotelResultsFooter")},this.Show=function(){int.f.Show("divHotelResultsFooter")},this.CurrentPage=1,this.PreviousPage=function(){t.SelectPage(t.CurrentPage-1)},this.NextPage=function(){t.SelectPage(t.CurrentPage+1)},this.SelectPage=function(e){t.CurrentPage=e,int.ff.Call("=iVectorWidgets.HotelResultsFooter.SelectPage",function(t){int.f.SetHTML("divPagingBottom",t),int.f.SetHTML("divPagingTop",t),int.e.ScrollIntoView("divPagingTop",10,2),HotelResults.UpdateResults()},e)},this.Update=function(){t.CurrentPage=1,int.ff.Call("=iVectorWidgets.HotelResultsFooter.SelectPage",HotelResultsFooter.UpdateComplete,t.CurrentPage)},this.UpdateComplete=function(t){int.f.ShowIf("divHotelResultsFooter",""!=t),int.f.SetHTML("divPagingBottom",t)}};