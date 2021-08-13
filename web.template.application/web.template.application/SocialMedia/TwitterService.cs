namespace Web.Template.Application.SocialMedia
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Tweetinvi;
    using Tweetinvi.Models;
    using Tweetinvi.Parameters;

    using Web.Template.Application.Interfaces.Configuration;
    using Web.Template.Application.Interfaces.Services;
    using Web.Template.Application.Interfaces.SocialMedia;

    using Tweet = Web.Template.Domain.Entities.SocialMedia.Tweet;

    /// <summary>
    /// Class TwitterService.
    /// </summary>
    public class TwitterService : ITwitterService
    {
        /// <summary>
        /// The cache lock object
        /// </summary>
        private static readonly object CacheLockObject = new object();

        /// <summary>
        /// The site configuration
        /// </summary>
        private readonly ISiteService siteService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterService" /> class.
        /// </summary>
        /// <param name="siteService">The site service.</param>
        public TwitterService(ISiteService siteService)
        {
            this.siteService = siteService;
        }

        /// <summary>
        /// Gets the tweets.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>List of tweets.</returns>
        public List<Tweet> GetTweetsByUser(string userName)
        {
            var tweets = new List<Tweet>();

            var cacheKey = $"__twitter_user_tweets_{userName}";
            if (HttpContext.Current.Cache[cacheKey] != null)
            {
                tweets = (List<Tweet>)HttpContext.Current.Cache[cacheKey];
            }
            else
            {
                lock (CacheLockObject)
                {
                    this.SetUserCredentials();

                    var userTimelineParameters = new UserTimelineParameters { ExcludeReplies = true, IncludeRTS = false };

                    var timeline = Timeline.GetUserTimeline(userName, userTimelineParameters);
                    if (timeline != null)
                    {
                        foreach (ITweet tweetinvi in timeline)
                        {
                            var tweet = new Domain.Entities.SocialMedia.Tweet { Text = tweetinvi.Text, CreatedDate = tweetinvi.CreatedAt, Images = new List<string>(), Url = tweetinvi.Url};

                            foreach (var mediaEntity in tweetinvi.Media.Where(entity => entity.MediaType == "photo"))
                            {
                                tweet.Images.Add(mediaEntity.MediaURLHttps);
                            }

                            tweets.Add(tweet);
                        }

                        HttpContext.Current.Cache.Insert(cacheKey, tweets);
                    }
                }
            }

            return tweets;
        }

        /// <summary>
        /// Sets the user credentials.
        /// </summary>
        private void SetUserCredentials()
        {
            ISite site = this.siteService.GetSite(HttpContext.Current);
            string consumerKey = site.SiteConfiguration.TwitterConfiguration.ConsumerKey;
            string consumerSecret = site.SiteConfiguration.TwitterConfiguration.ConsumerSecret;
            string accessToken = site.SiteConfiguration.TwitterConfiguration.AccessToken;
            string accessTokenSecret = site.SiteConfiguration.TwitterConfiguration.AccessTokenSecret;

            Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }
    }
}