using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;

namespace DJRM.Common.Security.OAuthProviders
{
    public sealed class GoogleOAuthProvider : OpenIdClient
    {

        public GoogleOAuthProvider()
            : base("google", WellKnownProviders.Google)
        {
        }
        /// <summary>
        /// Gets the extra data obtained from the response message when authentication is successful.
        /// </summary>
        /// <param name="response">
        /// The response message. 
        /// </param>
        /// <returns>A dictionary of profile data; or null if no data is available.</returns>
        protected override Dictionary<string, string> GetExtraData(IAuthenticationResponse response)
        {
            FetchResponse extension = response.GetExtension<FetchResponse>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (extension != null)
            {
                dictionary.AddItemIfNotEmpty("email", extension.GetAttributeValue(WellKnownAttributes.Contact.Email));
                dictionary.AddItemIfNotEmpty("country", extension.GetAttributeValue(WellKnownAttributes.Contact.HomeAddress.Country));
                dictionary.AddItemIfNotEmpty("firstName", extension.GetAttributeValue(WellKnownAttributes.Name.First));
                dictionary.AddItemIfNotEmpty("lastName", extension.GetAttributeValue(WellKnownAttributes.Name.Last));

                string userId = extension.GetAttributeValue("http://schemas.openid.net/ax/api/user_id");
                if (!String.IsNullOrEmpty(userId))
                {
                    dictionary.Add("user_id", userId);
                    dictionary.Add("pictureUrl", String.Format("https://profiles.google.com/s2/photos/profile/{0}", userId));
                }
            }
            return dictionary;
        }


        /// <summary>
        /// Called just before the authentication request is sent to service provider.
        /// </summary>
        /// <param name="request">
        /// The request. 
        /// </param>
        protected override void OnBeforeSendingAuthenticationRequest(IAuthenticationRequest request)
        {            
            FetchRequest fetchRequest = new FetchRequest();

            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Contact.HomeAddress.Country);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.First);
            fetchRequest.Attributes.AddRequired(WellKnownAttributes.Name.Last);
            fetchRequest.Attributes.AddRequired("http://schemas.openid.net/ax/api/user_id");

            request.AddExtension(fetchRequest);
        }

    }

    public static class DictonaryExtensions
    {
        public static void AddItemIfNotEmpty(this Dictionary<string, string> dic, string key, string value)
        {
            if (String.IsNullOrEmpty(value))
                return;

            if (!dic.ContainsKey(key))
            {
                dic.Add(key, value);
            }
            else
            {
                dic[key] = value;
            }
        }
    }

}
