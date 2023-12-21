using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace BlogPost
{
	public class SpinRewriter
	{
		public SpinRewriter()
		{
		}
		public string SpinTitle(string title)
		{
            try
            {
                string json = String.Empty;
                NameValueCollection data = new NameValueCollection();
                data["email_address"] = "darieylmitchell@gmail.com";
                data["api_key"] = "49752ab#b57ac30_2a8e525?6093080";
                data["action"] = "unique_variation";
                data["text"] = title;
                var client = new WebClient();
                var response = client.UploadValues("http://www.spinrewriter.com/action/api", "POST", data);
                json = Encoding.UTF8.GetString(response);
                var result = JsonValue.Parse(json);
                if (result["response"].ToString().Contains("API quota exceeded"))
                    return "No";
                if (result["response"].ToString().Contains("7 seconds"))
                    return "Wait";
                return result["response"].ToString();
            }
            catch(Exception)
            {
                throw;
            }
        }
        public string SpinContent(string content)
        {
            try
            {
                string json = String.Empty;
                NameValueCollection data = new NameValueCollection();
                data["email_address"] = "darieylmitchell@gmail.com";
                data["api_key"] = "49752ab#b57ac30_2a8e525?6093080";
                data["action"] = "unique_variation";
                data["add_html_markup"] = "false";
                data["use_html_linebreaks"] = "false";
                data["text"] = content;
                var client = new WebClient();
                var response = client.UploadValues("http://www.spinrewriter.com/action/api", "POST", data);
                json = Encoding.UTF8.GetString(response);
                var result = JsonValue.Parse(json);
                if (result["response"].ToString().Contains("API quota exceeded"))
                    return "No";
                if (result["response"].ToString().Contains("7 seconds"))
                    return "Wait";
                return result["response"].ToString();
            }
            catch(Exception)
            {
                throw;
            }
}
    }
}

