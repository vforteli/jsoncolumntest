using Dapper;
using jsontestapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace jsontestapp.Controllers
{
    [ApiController]
    public class JsontestController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=FL494;Initial Catalog=efcore;Integrated Security=True;";
        private readonly ILogger _logger;

        public JsontestController(ILogger<JsontestController> logger)
        {
            _logger = logger;
        }


        [HttpGet("api/json/{id}")]
        public async Task<IActionResult> Get([FromRoute]Guid id, [FromQuery]string orderBy = "ObjectId DESC", [FromQuery]string filter = "", [FromQuery]int top = 50, [FromQuery]int skip = 0)
        {
            var jsonPathParameter = "";
            if (orderBy.Contains("/"))
            {
                var parts = orderBy.Split("/");
                var column = parts[0];
                jsonPathParameter = "$." + string.Join(".", parts.Skip(1).Select(o => $@"""{o}"""));
                orderBy = $"JSON_VALUE({column}, @jsonPath) DESC";
            }
            else
            {
                var parts = orderBy.Split(" ");
                var properties = typeof(RowModel).GetProperties().Select(o => o.Name.ToUpperInvariant());
                if (!properties.Contains(parts[0].ToUpperInvariant()))
                {
                    return BadRequest($"Order by is not a valid column: {orderBy}");
                }
            }

            _logger.LogInformation($"Converted order by column: {orderBy}, parameter: {jsonPathParameter}");


            var query = $@"
                    SELECT
                        PartitionId
	                    ,ObjectId
                        ,DynamicData
	                    --,DynamicData1
                        --,DynamicData2
                        --,DynamicData3
                        --,DynamicData4
                        --,DynamicData5
                        ,testproperty
                    FROM JsonTest
                    --FROM JsonTestPartitioned
                    WHERE 
	                    PartitionId = @id
                    ORDER BY {orderBy}
                    OFFSET @skip ROWS
                    FETCH NEXT @top ROWS ONLY";

            var parameters = new
            {
                id = id,
                skip = skip,
                top = top,
                jsonPath = jsonPathParameter
            };



            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var stopwatch = Stopwatch.StartNew();

                var result = await connection.QueryAsync<RowModel>(query, parameters);

                _logger.LogInformation($"Execution and deserialization took {stopwatch.ElapsedMilliseconds} ms");

                return Ok(new
                {
                    orderBy = orderBy,
                    executionMilliseconds = stopwatch.ElapsedMilliseconds,
                    items = result,
                });
            }
        }


        /// <summary>
        /// Convert a path to either a column or proper json_value column and path
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ToPathOrColumn<T>(string path)
        {
            throw new NotImplementedException();
        }
    }
}
