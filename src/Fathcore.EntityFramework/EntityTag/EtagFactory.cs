using System;
using System.Linq;
using System.Text;
using Fathcore.Infrastructure.Enum;
using Fathcore.Infrastructure.Localization.Resources;
using Fathcore.Infrastructure.ResponseWrapper;
using Fathcore.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Represents Etag factory.
    /// </summary>
    public class EtagFactory : IEtagFactory
    {
        private readonly HttpContext _httpContext;
        private readonly IHashFactory _hashFactory;
        private readonly IStringLocalizer<CommonMessage> _commonMessageLocalizer;
        private readonly IStringLocalizer<ValidationMessage> _validationMessageLocalizer;

        /// <summary>
        /// Gets current if match header value.
        /// </summary>
        public string CurrentIfMatchValue
        {
            get
            {
                return _httpContext.Request.Headers[HeaderNames.IfMatch]
                    .FirstOrDefault()
                    ?.Replace("\"", string.Empty)
                    ?? string.Empty;
            }
        }

        public EtagFactory(IHttpContextAccessor httpContextAccessor, IHashFactory hashFactory, IStringLocalizer<CommonMessage> commonMessageLocalizer, IStringLocalizer<ValidationMessage> validationMessageLocalizer)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _hashFactory = hashFactory;
            _commonMessageLocalizer = commonMessageLocalizer;
            _validationMessageLocalizer = validationMessageLocalizer;
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
        /// Generate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        public string GenerateAndAddEtagHeader(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return GenerateAndAddEtagHeader(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Generate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        public string GenerateAndAddEtagHeader(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return GenerateAndAddEtagHeader(Encoding.Unicode.GetBytes(data));
        }

        /// <summary>
        /// Generate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        public string GenerateAndAddEtagHeader(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var etag = Generate(data);

            if (_httpContext.Response.Headers.ContainsKey(HeaderNames.ETag))
            {
                _httpContext.Response.Headers.Remove(HeaderNames.ETag);
            }

            _httpContext.Response.Headers.Add(HeaderNames.ETag, etag);
            return etag;
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

            return CurrentIfMatchValue.Equals(Generate(data), StringComparison.Ordinal);
        }

        /// <summary>
        /// Validate ETag and add it to new response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public bool ValidateAndAddEtagHeader(object data, bool allowEmpty = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ValidateAndAddEtagHeader(JsonConvert.SerializeObject(data), allowEmpty);
        }

        /// <summary>
        /// Validate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public bool ValidateAndAddEtagHeader(string data, bool allowEmpty = false)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return ValidateAndAddEtagHeader(Encoding.Unicode.GetBytes(data), allowEmpty);
        }

        /// <summary>
        /// Validate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public bool ValidateAndAddEtagHeader(byte[] data, bool allowEmpty = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            GenerateAndAddEtagHeader(data);

            var validateResult = Validate(data, allowEmpty);

            switch (_httpContext.Request.Method.ToUpper())
            {
                case "GET":
                {
                    if (validateResult && CurrentIfMatchValue != "*")
                        _httpContext.Response.StatusCode = StatusCodes.Status304NotModified;
                }
                break;
                case "PUT":
                case "DELETE":
                {
                    if (!validateResult && CurrentIfMatchValue != "*" && string.IsNullOrWhiteSpace(CurrentIfMatchValue))
                    {
                        _httpContext.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                        throw new ResponseException(_validationMessageLocalizer[ValidationMessage.ArgumentNull, HeaderNames.IfMatch], ErrorType.PreconditionFailed);
                    }

                    if (!validateResult && CurrentIfMatchValue != "*" && !string.IsNullOrWhiteSpace(CurrentIfMatchValue))
                    {
                        _httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                        throw new ResponseException(_commonMessageLocalizer[CommonMessage.Conflict], ErrorType.Conflict);
                    }
                }
                break;
                default:
                    break;
            }

            return validateResult;
        }
    }
}
