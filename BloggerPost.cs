using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;

namespace BlogPost
{
	public class BloggerPost
	{
        public BloggerService Service { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CredPath { get; set; }
        public string ApplicationName { get; set; }

        public BloggerPost(DynamicParam param)
		{
            ClientId = param.ClientId;
            ClientSecret = param.ClientSecret;
            CredPath = param.CredPath;
            ApplicationName = param.ApplicationName;
            Service = GetService();
        }
		public BlogList GetAllBlogs()
		{
            var blogs = Service.Blogs.ListByUser("self").Execute();
            return blogs;
        }
        public void InsertPost(string blogid, RSSFeed feed)
        {
            Post post = new Post()
            {
                Title = feed.Title,
                Content = feed.Content,
                Labels = new List<string> { "News" }
            };
            Service.Posts.Insert(post, blogid).Execute();
        }
        public BloggerService GetService()
        {
            string clientId = ClientId;
            string clientSecret = ClientSecret;
            string[] scopes = new string[] { BloggerService.Scope.BloggerReadonly,BloggerService.Scope.Blogger}; 
            var credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            credPath = System.IO.Path.Combine(credPath, "credentials/" + CredPath);
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                                                                                         , scopes
                                                                                         , "self"
                                                                                         , CancellationToken.None
                                                                                         , new FileDataStore(credPath, true)).Result;
            return new BloggerService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
	}
}

