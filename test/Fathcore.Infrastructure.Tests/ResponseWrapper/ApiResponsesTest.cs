using System.Collections.Generic;
using Fathcore.Infrastructure.Pagination;
using Fathcore.Infrastructure.ResponseWrapper;
using Newtonsoft.Json;
using Xunit;

namespace Fathcore.Infrastructure.Tests.ResponseWrapper
{
    public class ApiResponsesTest
    {
        [Fact]
        public void Pagination_Result_Success()
        {
            var pagedList = new PagedList<string>(new List<string>() { "A", "B", "C", "D", "E" }, 0, 2);
            var response = ApiResponses.Success(pagedList, "Message");
            var result = JsonConvert.SerializeObject(response);
        }
    }
}
