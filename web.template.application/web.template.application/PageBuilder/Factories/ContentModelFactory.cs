namespace Web.Template.Application.PageBuilder.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Web.Template.Application.Interfaces.Models;
    using Web.Template.Application.Interfaces.PageBuilder.Factories;

    /// <summary>
    ///     A factory used to generate content models
    /// </summary>
    /// <seealso cref="IContentModelFactory" />
    public class ContentModelFactory : IContentModelFactory
    {
        /// <summary>
        /// The cache key
        /// </summary>
        private readonly string cacheKey = "contentModelFactoryTypes";

        /// <summary>
        ///     A List of all types that implement IContentModel
        /// </summary>
        private List<Type> contentModels;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentModelFactory" /> class.
        /// </summary>
        public ContentModelFactory()
        {
            this.LoadModels();
        }

        /// <summary>
        ///     Gets a concrete implementation of IContentModel based on a string of the models type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>A Concrete Implementation of IWidget</returns>
        /// <exception cref="System.NotImplementedException">Throws an exception if no widget with the specified name exists.</exception>
        public IContentModel CreateModelByType(string modelType)
        {
            IContentModel contentModel = this.CreateModel(modelType);

            if (contentModel == null)
            {
                throw new NotImplementedException();
            }

            return contentModel;
        }

        /// <summary>
        ///     Creates the page widget.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns> an Implementation of IContentModel</returns>
        private IContentModel CreateModel(string modelName)
        {
            IContentModel contentModel = null;

            foreach (Type modelType in this.contentModels)
            {
                contentModel = Activator.CreateInstance(modelType) as IContentModel;
                if (contentModel.GetType().ToString() == modelName)
                {
                    return contentModel;
                }

                contentModel = null;
            }

            return contentModel;
        }

        /// <summary>
        ///     Loads the models.
        /// </summary>
        private void LoadModels()
        {
            this.contentModels = new List<Type>();

            if (HttpContext.Current.Cache[this.cacheKey] != null)
            {
                this.contentModels = (List<Type>)HttpContext.Current.Cache[this.cacheKey];
            }
            else
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        this.contentModels.AddRange(assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IContentModel))).ToList());
                    }
                    catch (Exception ex)
                    {
                    }
                }

                HttpContext.Current.Cache.Insert(this.cacheKey, this.contentModels);
            }
        }
    }
}