using System;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ElectionStatistics.WebSite
{
	public class AllowAnyOriginCorsRequestAttribute : ActionFilterAttribute
	{
		public const string OriginHeaderAttributeName = "Origin";
		private const string AllowMethodsHeaderName = "Access-Control-Allow-Methods";
		private const string AllowOriginHeaderName = "Access-Control-Allow-Origin";
		private const string AllowCredentialsHeaderName = "Access-Control-Allow-Credentials";

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var origin = filterContext.HttpContext.Request.Headers[OriginHeaderAttributeName].ToArray().SingleOrDefault();

			if (origin == null)
				return;

			filterContext.HttpContext.Response.Headers.Add(AllowOriginHeaderName, origin);
			filterContext.HttpContext.Response.Headers.Add(AllowCredentialsHeaderName, "true");

			if (filterContext.HttpContext.Request.Method.Equals(HttpMethod.Options.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				filterContext.HttpContext.Response.Headers.Add(AllowMethodsHeaderName, HttpMethod.Post.ToString());
				filterContext.Result = new EmptyResult();
			}
		}
	}
}