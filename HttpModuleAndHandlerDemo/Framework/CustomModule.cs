using System;
using System.Security.Principal;
using System.Web;

namespace HttpModuleAndHandlerDemo.Framework
{
    public class CustomModule : IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            //TODO: create a custom module to rewrite urls
            context.BeginRequest += Context_BeginRequest;
            context.AuthenticateRequest += Context_AuthenticateRequest;
            context.PostAuthenticateRequest += Context_PostAuthenticateRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            app.Context.Response.Write("It's fired from BeginRequest event!\n");
        }

        private void Context_AuthenticateRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            app.Context.User = new GenericPrincipal(new GenericIdentity("toto@yopmail.com"), null);
            app.Context.Response.Write("It's fired from AuthenticateRequest event!\n");
        }

        private void Context_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            app.Context.Response.Write(string.Format("User connected is {0} from PostAuthenticateRequest event!\n", app.Context.User.Identity.Name));
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            app.Context.Response.Write("It's fired from EndRequest event!");
        }
    }
}