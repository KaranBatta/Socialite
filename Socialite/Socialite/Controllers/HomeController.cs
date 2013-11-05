using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using DotNetOpenAuth.AspNet.Clients;
using FacebookClient = Facebook.FacebookClient;

namespace Socialite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "This is the test application to login to multiple services and manage them..";
            return View();
        }

        public ActionResult Facebook(object sender, EventArgs e)
        {
            var client = new FacebookClient();
            dynamic result = client.GetLoginUrl(new
            {
                client_id = "520025764710565",
                client_secret = "0d7ea70fb467f9f5a5c9444b63f2f8a6",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "read_stream"
            });
            return Redirect(result.AbsoluteUri);
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "520025764710565",
                client_secret = "0d7ea70fb467f9f5a5c9444b63f2f8a6",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;

            // Store the access token in the session
            Session["AccessToken"] = accessToken;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me/home?type=newsfeed");
            string email = me.email;

            // Set the auth cookie
            FormsAuthentication.SetAuthCookie(email, false);

            return RedirectToAction("Index", "Home");
        }
    }
}
