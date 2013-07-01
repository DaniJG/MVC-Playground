using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DJRM.Common.Infrastructure.HtmlHelpers
{
    public static class ValidationHelpers
    {
        private const string HiddenListItem = @"<li style=""display:none""></li>";

        /// <summary>
        /// Extends the standard Html.ValidationSummary, adding a jqueryUI alert icon and the ui-state-error to the whole div
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="excludePropertyErrors"></param>
        /// <param name="message"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ExtendedValidationSummary(this HtmlHelper htmlHelper, bool excludePropertyErrors = true, string message = null, IDictionary<string, object> htmlAttributes = null)
        {
            
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            FormContext formContext = htmlHelper.ViewContext.ClientValidationEnabled ? htmlHelper.ViewContext.FormContext : null; 
            if (htmlHelper.ViewData.ModelState.IsValid)
            {
                if (formContext == null)
                {
                    // No client side validation
                    return null;
                }
                // TODO: This isn't really about unobtrusive; can we fix up non-unobtrusive to get rid of this, too?
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled && excludePropertyErrors)
                {
                    // No client-side updates
                    return null;
                }
            }

            string messageSpan;
            if (!String.IsNullOrEmpty(message))
            {
                TagBuilder errorIconTag = new TagBuilder("span");
                errorIconTag.AddCssClass("ui-icon ui-icon-alert");
                TagBuilder messageTag = new TagBuilder("p");
                messageTag.InnerHtml = errorIconTag.ToString() + message;
                messageSpan = messageTag.ToString(TagRenderMode.Normal) + Environment.NewLine;
            }
            else
            {
                messageSpan = null;
            }

            StringBuilder htmlSummary = new StringBuilder();
            TagBuilder unorderedList = new TagBuilder("ul");

            IEnumerable<ModelState> modelStates = GetModelStateList(htmlHelper, excludePropertyErrors);

            foreach (ModelState modelState in modelStates)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    string errorText = GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, modelError, null /* modelState */);
                    if (!String.IsNullOrEmpty(errorText))
                    {
                        TagBuilder listItem = new TagBuilder("li");
                        listItem.SetInnerText(errorText);
                        htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
                    }
                }
            }

            if (htmlSummary.Length == 0)
            {
                htmlSummary.AppendLine(HiddenListItem);
            }

            unorderedList.InnerHtml = htmlSummary.ToString();

            TagBuilder divBuilder = new TagBuilder("div");
            divBuilder.MergeAttributes(htmlAttributes);
            divBuilder.AddCssClass((htmlHelper.ViewData.ModelState.IsValid) ? HtmlHelper.ValidationSummaryValidCssClassName : HtmlHelper.ValidationSummaryCssClassName);
            divBuilder.AddCssClass("ui-widget ui-state-error ui-corner-all");
            divBuilder.InnerHtml = messageSpan + unorderedList.ToString(TagRenderMode.Normal);

            if (formContext != null)
            {
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                {
                    if (!excludePropertyErrors)
                    {
                        // Only put errors in the validation summary if they're supposed to be included there
                        divBuilder.MergeAttribute("data-valmsg-summary", "true");
                    }
                }
                else
                {
                    // client val summaries need an ID
                    divBuilder.GenerateId("validationSummary");
                    formContext.ValidationSummaryId = divBuilder.Attributes["id"];
                    formContext.ReplaceValidationSummary = !excludePropertyErrors;
                }
            }
            return new MvcHtmlString(divBuilder.ToString(TagRenderMode.Normal));
        }

        private static string GetUserErrorMessageOrDefault(HttpContextBase httpContext, ModelError error, ModelState modelState)
        {
            if (!String.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }
            if (modelState == null)
            {
                return null;
            }

            string attemptedValue = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
            return String.Format(CultureInfo.CurrentCulture, GetInvalidPropertyValueResource(httpContext), attemptedValue);
        }

        private static string GetInvalidPropertyValueResource(HttpContextBase httpContext)
        {
            string resourceValue = null;
            if (!String.IsNullOrEmpty(ValidationExtensions.ResourceClassKey) && (httpContext != null))
            {
                // If the user specified a ResourceClassKey try to load the resource they specified.
                // If the class key is invalid, an exception will be thrown.
                // If the class key is valid but the resource is not found, it returns null, in which
                // case it will fall back to the MVC default error message.
                resourceValue = httpContext.GetGlobalResourceObject(ValidationExtensions.ResourceClassKey, "InvalidPropertyValue", CultureInfo.CurrentUICulture) as string;
            }
            return resourceValue ?? "The value '{0}' is invalid."; //In MVC source this is a resource!: MvcResources.Common_ValueNotValidForProperty;
        }

        // Returns non-null list of model states, which caller will render in order provided.
        private static IEnumerable<ModelState> GetModelStateList(HtmlHelper htmlHelper, bool excludePropertyErrors)
        {
            if (excludePropertyErrors)
            {
                ModelState ms;
                htmlHelper.ViewData.ModelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out ms);
                if (ms != null)
                {
                    return new ModelState[] { ms };
                }

                return new ModelState[0];
            }
            else
            {
                // Sort modelStates to respect the ordering in the metadata.                 
                // ModelState doesn't refer to ModelMetadata, but we can correlate via the property name.
                Dictionary<string, int> ordering = new Dictionary<string, int>();

                var metadata = htmlHelper.ViewData.ModelMetadata;
                if (metadata != null)
                {
                    foreach (ModelMetadata m in metadata.Properties)
                    {
                        ordering[m.PropertyName] = m.Order;
                    }
                }

                return from kv in htmlHelper.ViewData.ModelState
                       let name = kv.Key
                       orderby (ordering.ContainsKey(name) ? ordering[name] : ModelMetadata.DefaultOrder)
                       select kv.Value;
            }
        }
    }
}
