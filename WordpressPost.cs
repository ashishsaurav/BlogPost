using System;
using Newtonsoft.Json;
using System.Net;
using RestSharp;
using System.Text.RegularExpressions;

namespace BlogPost
{
    public class RSSFeed
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
    #region jsonclass
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Content
    {
        public string rendered { get; set; }
        public bool @protected { get; set; }
    }

    public class Root
    {
        public int id { get; set; }
        public DateTime modified { get; set; }
        public Title title { get; set; }
        public Content content { get; set; }
    }

    public class Title
    {
        public string rendered { get; set; }
    }
    #endregion

    public class WordpressPost
	{
        public string FeedURL { get; set; }
        public string Authorization { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Categories { get; set; }
        public static string DateTimePath { get; set; }
        public static string DatePath
        {
            get
            {
                var datepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                datepath = System.IO.Path.Combine(datepath, "credentials/datetime/" + DateTimePath);
                return datepath;
            }
        }
        public DateTime LastModifiedDate { get; set; }
        public WordpressPost(DynamicParam param)
		{
            FeedURL = param.FeedURL;
            Username = param.Username;
            Password = param.Password;
            Authorization = param.Authorization;
            Categories = param.Categories;
            DateTimePath = param.DateTimePath;
            LastModifiedDate = File.Exists(DatePath) ? Convert.ToDateTime(File.ReadAllText(DatePath)) : DateTime.Now;
        }
		public List<RSSFeed> GetPost()
		{
            List<RSSFeed> feed = new List<RSSFeed>();
            try
            {
                var client = new RestClient();
                var request = new RestRequest(FeedURL + "?_fields=id,content,title,modified&categories=" + Categories + "&per_page=100", Method.Get);
                request.AddHeader("Authorization", Authorization);
                RestResponse response = client.ExecuteAsync(request).Result;
                if (response.StatusCode.ToString() == "Unauthorized")
                    return feed;
                var posts = JsonConvert.DeserializeObject<List<Root>>(response.Content);

                var postToUpdate = posts.Where(x => x.modified > LastModifiedDate).ToList();
                foreach (var post in postToUpdate)
                {
                    feed.Add(new RSSFeed
                    {
                        PostId = post.id,
                        Title = post.title.rendered,
                        Content = ExtractTextFromHTML(post.content.rendered)
                    });
                    LastModifiedDate = LastModifiedDate < post.modified ? post.modified : LastModifiedDate;
                }
                File.WriteAllText(DatePath, LastModifiedDate.ToString());
                return feed;
            }
            catch (Exception)
            {
                return feed;
            }
        }
		public string InsertPost(RSSFeed rssFeed)
		{
            try
            {
                var client = new RestClient();
                var request = new RestRequest(FeedURL + "/" + rssFeed.PostId, Method.Post);
                request.AddHeader("Authorization", Authorization);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("title", rssFeed.Title);
                request.AddParameter("content", rssFeed.Content);
                RestResponse response = client.ExecuteAsync(request).Result;
                if (response.StatusCode.ToString().ToLower() != "ok")
                    return "Unauthorized";
                else
                    return "Ok";
            }
            catch(Exception)
            {
                throw;
            }
        }
        public string ExtractTextFromHTML(string html)
        {
            try
            {
                Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
                var text = rRemScript.Replace(html, "");
                /*const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
                const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
                const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
                var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
                var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

                //Decode html specific characters
                text = System.Net.WebUtility.HtmlDecode(text);
                //Remove tag whitespace/line breaks
                text = tagWhiteSpaceRegex.Replace(text, "><");
                //Replace <br /> with line breaks
                text = lineBreakRegex.Replace(text, Environment.NewLine);
                //Strip formatting
                text = stripFormattingRegex.Replace(text, string.Empty);*/

                return text;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return html;
            }
        }
    }
}

