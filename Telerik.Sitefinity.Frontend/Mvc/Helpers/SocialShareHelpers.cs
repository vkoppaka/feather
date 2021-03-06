﻿using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    public static class SocialShareHelpers
    {
        /// <summary>
        /// Socials the share options. Redirect to the SocialShare control if exist else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        public static MvcHtmlString SocialShareOptions(this HtmlHelper helper)
        {
            MvcHtmlString result;
            try
            {
                result = helper.Action("Index", "SocialShare");
            }
            catch (HttpException e)
            {
                result = new MvcHtmlString("The SocialShare widget could not be found.");
            }
            return result;
        }
    }
}