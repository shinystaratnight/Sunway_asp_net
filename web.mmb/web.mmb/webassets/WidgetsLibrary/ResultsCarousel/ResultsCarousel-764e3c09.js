var ResultsCarousel=ResultsCarousel||{},ResultsCarousel=ResultsCarousel;ResultsCarousel.Rotator,ResultsCarousel.Mode,ResultsCarousel.StartPage,ResultsCarousel.CurrentPage,ResultsCarousel.TotalPages,ResultsCarousel.PageWidth,ResultsCarousel.StartTime,ResultsCarousel.Seconds,ResultsCarousel.CurrentLeft=0,ResultsCarousel.TargetLeft,ResultsCarousel.UpdateFunction,ResultsCarousel.Holder,ResultsCarousel.SearchAgain=!1,ResultsCarousel.Setup=function(e,s,t,l,u){ResultsCarousel.Mode=e,ResultsCarousel.StartPage=s,ResultsCarousel.CurrentPage=s,ResultsCarousel.TotalPages=t,ResultsCarousel.PageWidth=l,ResultsCarousel.Holder=int.f.GetObject("divResultsCarouselItems"),ResultsCarousel.CurrentLeft=-(s-1)*l,ResultsCarousel.Holder.style.left=ResultsCarousel.CurrentLeft+"px",ResultsCarousel.SearchAgain=int.f.GetValue("hidCarouselSearchAgain"),ResultsCarousel.UpdateFunction=void 0!=u?u:FlightResults.UpdateResults,ResultsCarousel.ScrollCenter()},ResultsCarousel.ScrollCenter=function(){var e=(document.getElementById("divResultsCarouselItems").clientWidth-document.getElementById("divResultsCarouselContent").clientWidth)/2;$("#divResultsCarouselContent").scrollLeft(e)},ResultsCarousel.Filter=function(e){int.ff.Call("=iVectorWidgets.ResultsCarousel.Filter",ResultsCarousel.FilterComplete,e)},ResultsCarousel.FilterComplete=function(e){ResultsCarousel.UpdateFunction(),int.f.SetHTML("divResultsCarouselItems",e),ResultsCarousel.RedrawFilters()},ResultsCarousel.RedrawFilters=function(){"undefined"!=typeof FlightResultsFilter&&FlightResultsFilter.RedrawFilters()},ResultsCarousel.ReSearch=function(e){WaitMessage.Show("Search"),int.ff.Call("=iVectorWidgets.ResultsCarousel.ReSearch",ResultsCarousel.ReSearchComplete,e)},ResultsCarousel.ReSearchComplete=function(sJSON){WaitMessage.Hide(),WaitMessage.Suppress=!0;var oReturn=eval("("+sJSON+")"),sRedirectURL="";"FlightPlusHotel"==oReturn.SearchMode&&oReturn.PropertyCount>0&&oReturn.FlightCount>0?(sRedirectURL=int.f.GetValue("hidFlightPlusHotelRedirectURL"),web.Window.Redirect(""!=sRedirectURL?sRedirectURL:"/search-results")):"HotelOnly"==oReturn.SearchMode&&oReturn.PropertyCount>0?web.Window.Redirect("/search-results"):"FlightOnly"==oReturn.SearchMode&&oReturn.FlightCount>0?(sRedirectURL=int.f.GetValue("hidFlightOnlyRedirectURL"),web.Window.Redirect(""!=sRedirectURL?sRedirectURL:"/flight-results")):("FlightPlusHotel"==oReturn.SearchMode||"FlightOnly"==sSearchMode)&&oReturn.FlightCarouselCount>0?web.Window.Redirect("/search-results"):web.InfoBox.Show(int.f.GetValue("hidWarning_NoSearchResults"),"warning")},ResultsCarousel.Forward=function(e){if("True"==ResultsCarousel.SearchAgain){var s=int.f.GetValue("hidCarouselDaysPerPage");ResultsCarousel.Search(s)}else{if(ResultsCarousel.Sliding)return;ResultsCarousel.TargetLeft=ResultsCarousel.CurrentLeft-ResultsCarousel.PageWidth,ResultsCarousel.CurrentPage+=1,int.f.ShowIf("aResultsCarouselLeft",ResultsCarousel.CurrentPage>1),int.f.ShowIf("aResultsCarouselRight",ResultsCarousel.CurrentPage<ResultsCarousel.TotalPages),ResultsCarousel.StartTime=new Date,ResultsCarousel.Slide()}},ResultsCarousel.Backward=function(e){if("True"==ResultsCarousel.SearchAgain){var s=int.f.GetValue("hidCarouselDaysPerPage");ResultsCarousel.Search(-s)}else{if(ResultsCarousel.Sliding)return;ResultsCarousel.TargetLeft=ResultsCarousel.CurrentLeft+ResultsCarousel.PageWidth,ResultsCarousel.CurrentPage-=1,int.f.ShowIf("aResultsCarouselLeft",ResultsCarousel.CurrentPage>1),int.f.ShowIf("aResultsCarouselRight",ResultsCarousel.CurrentPage<ResultsCarousel.TotalPages),ResultsCarousel.StartTime=new Date,ResultsCarousel.Slide()}},ResultsCarousel.Slide=function(){ResultsCarousel.Sliding=!0;var e=Math.abs((new Date-ResultsCarousel.StartTime)/ResultsCarousel.Seconds/1e3);1>e?ResultsCarousel.Holder.style.left=ResultsCarousel.CurrentLeft+(ResultsCarousel.TargetLeft-ResultsCarousel.CurrentLeft)*Math.sin(Math.PI/2*e)+"px":(ResultsCarousel.Holder.style.left=ResultsCarousel.TargetLeft+"px",ResultsCarousel.CurrentLeft=ResultsCarousel.TargetLeft,ResultsCarousel.Sliding=!1)},ResultsCarousel.Search=function(e){WaitMessage.Show("Search",function(){int.ff.Call("=iVectorWidgets.ResultsCarousel.SearchAgain",ResultsCarousel.SearchComplete,e)})},ResultsCarousel.SearchComplete=function(e){var s=JSON.parse(e),t=int.f.GetValue("hidSearchMode");if("FlightPlusHotel"==t&&s.PropertyCount>0&&s.FlightCount>0){var l=int.f.GetValue("hidFlightPlusHotelRedirectURL");web.Window.Redirect(""!=l?l:int.f.GetValue("hidFlightPlusHotelURL"))}else"HotelOnly"==t&&s.PropertyCount>0?web.Window.Redirect("/search-results"):"FlightOnly"==t&&s.FlightCount>0?web.Window.Redirect("/flight-results"):"TransferOnly"==t&&s.TransferCount>0?web.Window.Redirect("/booking-summary"):("FlightPlusHotel"==t||"FlightOnly"==t)&&s.FlightCarouselCount>0?web.Window.Redirect(int.f.GetValue("hidFlightPlusHotelURL")):(WaitMessage.Hide(),WaitMessage.Suppress=!0,web.InfoBox.Show(int.f.GetValue("hidWarning_NoSearchResults"),"warning"))};