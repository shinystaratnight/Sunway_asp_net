namespace Web.Template.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Results.ResultModels;

    /// <summary>
    /// Results Service used to save and retrieve results from a store, using the cache.
    /// </summary>
    /// <seealso cref="Web.Template.Application.Interfaces.Services.IResultService" />
    public class CachedResultService : IResultService
    {
        /// <summary>
        /// Retrieves a result that matches specified token.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <param name="componentToken">The component token.</param>
        /// <returns>
        /// A results model that matches the specified token
        /// </returns>
        public IResult RetrieveResult(string searchToken, int componentToken)
        {
            IResultsModel results = this.RetrieveResults(searchToken);
            IResult selectedResult = null;

            if (results != null)
            {
                foreach (IResult result in results.ResultsCollection)
                {
                    if (result.ComponentToken == componentToken)
                    {
                        selectedResult = result;
                    }
                }
            }

            return selectedResult;
        }

        /// <summary>
        /// Retries a model from the cache for the matching token
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>
        /// A results model that matches the specified token
        /// </returns>
        public IResultsModel RetrieveResults(string searchToken)
        {
            IResultsModel resultsModel;

            try
            {
                byte[] data = Convert.FromBase64String(searchToken);
                DateTime tokenTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                if (tokenTime > DateTime.UtcNow.AddMinutes(-30))
                {
                    resultsModel = (IResultsModel)HttpContext.Current.Cache[searchToken];
                }
                else
                {
                    resultsModel = new Results();
                }
            }
            catch (Exception ex)
            {
                Intuitive.FileFunctions.AddLogEntry("ResultsService", "Retrieve Result Exception", ex.ToString());
                resultsModel = new Results();
            }

            return resultsModel;
        }

        /// <summary>
        /// Retrieves the search model.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>
        /// The search model used to carry out the search
        /// </returns>
        public ISearchModel RetrieveSearchModel(string searchToken)
        {
            ISearchModel searchModel;

            byte[] data = Convert.FromBase64String(searchToken);
            DateTime tokenTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            if (tokenTime > DateTime.UtcNow.AddMinutes(-30))
            {
                IResultsModel resultsModel = (IResultsModel)HttpContext.Current.Cache[searchToken];
                searchModel = resultsModel.SearchModel;
            }
            else
            {
                searchModel = default(ISearchModel);
            }

            return searchModel;
        }

        /// <summary>
        /// Retrieves the extra search model.
        /// </summary>
        /// <param name="searchToken">The search token.</param>
        /// <returns>The extra search model.</returns>
        public IExtraSearchModel RetrieveExtraSearchModel(string searchToken)
        {
            IExtraSearchModel extraSearchModel;

            byte[] data = Convert.FromBase64String(searchToken);
            DateTime tokenTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            if (tokenTime > DateTime.UtcNow.AddMinutes(-30))
            {
                IResultsModel resultsModel = (IResultsModel)HttpContext.Current.Cache[searchToken];
                extraSearchModel = resultsModel.ExtraSearchModel;
            }
            else
            {
                extraSearchModel = default(IExtraSearchModel);
            }

            return extraSearchModel;
        }

        /// <summary>
        /// Saves the specified results models in the cache
        /// </summary>
        /// <param name="resultsModels">The results models.</param>
        public void SaveResults(List<IResultsModel> resultsModels)
        {
            foreach (IResultsModel resultsModel in resultsModels)
            {
                HttpContext.Current.Cache.Insert(resultsModel.ResultToken, resultsModel, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero);
            }
        }
    }
}