namespace Web.Template.API.Content
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Web.Template.Application.Interfaces.SocialMedia;
    using Web.Template.Domain.Entities.SocialMedia;

    /// <summary>
    /// The API to Twitter content
    /// </summary>
    public class TwitterController : ApiController
    {
        /// <summary>
        /// The twitter service
        /// </summary>
        private readonly ITwitterService twitterService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterController"/> class.
        /// </summary>
        /// <param name="twitterService">The twitter service.</param>
        public TwitterController(ITwitterService twitterService)
        {
            this.twitterService = twitterService;
        }

        /// <summary>
        /// Gets the tweets by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>List of Tweets</returns>
        [Route("api/twitter/tweets/{userName}")]
        public List<Tweet> GetTweetsByUser(string userName)
        {
            return this.twitterService.GetTweetsByUser(userName);
        }
    }
}