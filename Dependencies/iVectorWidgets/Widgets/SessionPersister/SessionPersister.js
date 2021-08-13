var SessionPersister = new function() {




	//#region Setup
	this.Persist = function () {

		int.ff.Call('=iVectorWidgets.SessionPersister.Persist', SessionPersister.PersistComplete);

	}

	this.PersistComplete = function () {

		setTimeout('SessionPersister.Persist();', 60000); 

	}



}