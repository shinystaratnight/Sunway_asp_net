namespace Web.Template.Application.Interfaces.SocialMedia
{
    using System.Collections.Generic;

    using Web.Template.Domain.Entities.SocialMedia;

    /// <summary>
    /// Interface ITwitterService
    /// </summary>
    public interface ITwitterService
    {
        /// <summary>
        /// Gets the tweets by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>List of tweets.</returns>
        List<Tweet> GetTweetsByUser(string userName);
    }
}