using Fathcore.Infrastructure.Enum;
using Fathcore.Infrastructure.ResponseWrapper;
using Newtonsoft.Json;
using Xunit;

namespace Fathcore.Infrastructure.Tests.ResponseWrapper
{
    public class ResponseExceptionTest
    {
        [Fact]
        public void Convert_To_Json()
        {
            var responseException = new ResponseException("Error Message.", ErrorType.InternalServerError, "Stack Trace.");

            var result = JsonConvert.SerializeObject(responseException);

            Assert.Equal("{\"ErrorType\":\"INTERNAL_SERVER_ERROR\",\"ErrorMessage\":\"Error Message.\",\"Details\":\"Stack Trace.\"}", result);
        }
    }
}
