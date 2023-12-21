using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Xml;
using BlogPost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

public class Program
{
    public static void Main()
    {
        var startup = new Startup();
        Console.WriteLine("Getting Latest Feed");
        WordpressPost wordpressPost = new WordpressPost(startup.DynamicParam);
        BloggerPost blogPost = null;
        if (startup.DynamicParam.IsPostBlog == "true")
            blogPost = new BloggerPost(startup.DynamicParam);
        SpinRewriter spinRewriter = new SpinRewriter();
        RSSFeed rss = new RSSFeed();

        var rssFeed = wordpressPost.GetPost();
        if (rssFeed == null || rssFeed.Count() <= 0)
            Console.WriteLine("No Latest Feed");
        else
            Console.WriteLine("Latest Feed Fetched - " + rssFeed.Count());

        foreach (var feed in rssFeed)
        {
            try
            {
                if (startup.DynamicParam.IsPostWordpress == "true")
                {
                    Console.WriteLine("Starting Wordpress Post");
                    #region Wordpress Post
                    rss.PostId = feed.PostId;
                    rss.Title = spinRewriter.SpinTitle(feed.Title);
                    if (rss.Title == "No")
                        break;
                    if (rss.Title == "Wait")
                    {
                        System.Threading.Thread.Sleep(7000);
                        rss.Title = spinRewriter.SpinTitle(feed.Title);
                    }
                    Console.WriteLine("Title Spin Successfully - " + rss.Title);
                    System.Threading.Thread.Sleep(7000);
                    rss.Content = spinRewriter.SpinContent(feed.Content);
                    if (rss.Content == "No")
                        break;
                    if (rss.Content == "Wait")
                    {
                        System.Threading.Thread.Sleep(7000);
                        rss.Content = spinRewriter.SpinContent(feed.Content);
                    }
                    Console.WriteLine("Content Spin Successfully");
                    System.Threading.Thread.Sleep(7000);
                    var response = wordpressPost.InsertPost(rss);
                    if (response == "Unauthorized")
                        break;
                    Console.WriteLine("Wordpress Post Updates Successfully - " + feed.PostId);
                    #endregion
                }
                try
                {
                    if (blogPost != null)
                    {
                        #region Blog Post
                        var blogs = blogPost.GetAllBlogs();
                        foreach (var blog in blogs.Items.ToList())
                        {
                            Console.WriteLine("Starting Blog Post - " + blog.Id);
                            rss.Title = feed.Title; //spinRewriter.SpinTitle(feed.Title);
                            if (rss.Title == "No")
                                return;
                            if (rss.Title == "Wait")
                            {
                                System.Threading.Thread.Sleep(7000);
                                rss.Title = spinRewriter.SpinTitle(feed.Title);
                            }
                            //Console.WriteLine("Title Spin Successfully - " + rss.Title);
                            //System.Threading.Thread.Sleep(7000);
                            rss.Content = feed.Content; // spinRewriter.SpinContent(feed.Content);
                            if (rss.Content == "No")
                                return;
                            if (rss.Content == "Wait")
                            {
                                System.Threading.Thread.Sleep(7000);
                                rss.Content = spinRewriter.SpinContent(feed.Content);
                            }
                            //Console.WriteLine("Content Spin Successfully");
                            //System.Threading.Thread.Sleep(7000);
                            blogPost.InsertPost(blog.Id, rss);
                            Console.WriteLine("Blog Post Inserted Successfully - " + blog.Id);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true);
    }
}
public class Startup
{
    public Startup()
    {
        var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        var builder = new ConfigurationBuilder()
                  .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                  .AddJsonFile("appsettings.json", optional: false);
        IConfiguration config = builder.Build();
        DynamicParam = config.GetSection("DynamicParam").Get<DynamicParam>();
    }

    public DynamicParam DynamicParam { get; private set; }
}
public class DynamicParam
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string CredPath { get; set; }
    public string ApplicationName { get; set; }
    public string FeedURL { get; set; }
    public string Authorization { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Categories { get; set; }
    public string IsPostWordpress { get; set; }
    public string IsPostBlog { get; set; }
    public string DateTimePath { get; set; }
}


