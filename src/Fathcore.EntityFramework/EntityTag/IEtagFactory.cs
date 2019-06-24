namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface of Etag factory.
    /// </summary>
    public interface IEtagFactory
    {
        /// <summary>
        /// Gets current if match header value.
        /// </summary>
        string CurrentIfMatchValue { get; }

        /// <summary>
        /// Generate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        string Generate(object data);

        /// <summary>
        /// Generate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        string Generate(string data);

        /// <summary>
        /// Generate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        string Generate(byte[] data);

        /// <summary>
        /// Generate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        string GenerateAndAddEtagHeader(object data);

        /// <summary>
        /// Generate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        string GenerateAndAddEtagHeader(string data);

        /// <summary>
        /// Generate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <returns>Generated ETag.</returns>
        string GenerateAndAddEtagHeader(byte[] data);

        /// <summary>
        /// Validate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool Validate(object data, bool allowEmpty = false);

        /// <summary>
        /// Validate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool Validate(string data, bool allowEmpty = false);

        /// <summary>
        /// Validate ETag.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool Validate(byte[] data, bool allowEmpty = false);

        /// <summary>
        /// Validate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool ValidateAndAddEtagHeader(object data, bool allowEmpty = false);

        /// <summary>
        /// Validate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool ValidateAndAddEtagHeader(string data, bool allowEmpty = false);

        /// <summary>
        /// Validate ETag and add it to response headers.
        /// </summary>
        /// <param name="data">Unique data for etag.</param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        bool ValidateAndAddEtagHeader(byte[] data, bool allowEmpty = false);
    }
}
