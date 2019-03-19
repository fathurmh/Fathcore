using System;
using System.Linq;
using System.Text;
using Fathcore.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Fathcore.EntityTag
{
    /// <summary>
    /// Represents ETag factory.
    /// </summary>
    public class EtagFactory : IEtagFactory
    {
        private string _currentIfMatchValue;
        private readonly HttpContext _httpContext;
        private readonly IHashFactory _hashFactory;

        /// <summary>
        /// Gets current if match header value.
        /// </summary>
        public string CurrentIfMatchValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_currentIfMatchValue))
                {
                    _currentIfMatchValue = _httpContext.Request.Headers[HeaderNames.IfMatch]
                        .FirstOrDefault()
                        ?.Replace("\"", string.Empty)
                        ?? string.Empty;
                }

                return _currentIfMatchValue;
            }
        }

        public EtagFactory(IHttpContextAccessor httpContextAccessor, IHashFactory hashFactory)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _hashFactory = hashFactory;
        }

        /// <summary>
        /// Generate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        public string Generate(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return Generate(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Generate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        public string Generate(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return Generate(Encoding.Unicode.GetBytes(data));
        }

        /// <summary>
        /// Generate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        public string Generate(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            return _hashFactory.Md5Hash(data);
        }

        /// <summary>
        /// Validate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public bool Validate(object data, bool allowEmpty = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return Validate(JsonConvert.SerializeObject(data), allowEmpty);
        }

        /// <summary>
        /// Validate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public bool Validate(string data, bool allowEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return Validate(Encoding.Unicode.GetBytes(data), allowEmpty);
        }

        /// <summary>
        /// Validate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public bool Validate(byte[] data, bool allowEmpty = false)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            if (CurrentIfMatchValue == "*")
                return true;

            if (string.IsNullOrWhiteSpace(CurrentIfMatchValue) && allowEmpty)
                return true;

            return CurrentIfMatchValue == Generate(data);
        }
    }
}
