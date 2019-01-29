using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fathcore.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Asynchronously clone http request message.
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <returns></returns>
        public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = await request.Content.CloneAsync().ConfigureAwait(false),
                Version = request.Version
            };
            foreach (KeyValuePair<string, object> prop in request.Properties)
            {
                clone.Properties.Add(prop);
            }
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        /// <summary>
        /// Asynchronously clone http content.
        /// </summary>
        /// <param name="content">HttpContent</param>
        /// <returns></returns>
        public static async Task<HttpContent> CloneAsync(this HttpContent content)
        {
            if (content == null) return null;

            MemoryStream memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream).ConfigureAwait(false);
            memoryStream.Position = 0;

            StreamContent clone = new StreamContent(memoryStream);
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }
            
            return clone;
        }
    }
}
