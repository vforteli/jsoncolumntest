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
        public async Task<IActionResult> Get([FromRoute]Guid id, [FromQuery]string orderBy = "ObjectId", [FromQuery]string filter = "", [FromQuery]int top = 50, [FromQuery]int skip = 0)
        {
            var (column, jsonPathParameter) = ToPathOrColumn<RowModel>(orderBy);

            _logger.LogInformation($"Converted order by column: {column}, parameter: {jsonPathParameter}");


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
                    ORDER BY {column} DESC
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
                    orderBy = column,
                    executionMilliseconds = stopwatch.ElapsedMilliseconds,
                    items = result,
                });
            }
        }


        /// <summary>
        /// Convert a path to either a column or proper json_value column and path
        /// Validates the column against a model passed in as T
        /// </summary>
        /// <param name="input">column[/jsonpath]</param>
        /// <returns></returns>
        private (string column, string jsonPathParameter) ToPathOrColumn<T>(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Column cannot be null or empty");
            }

            var parts = input.Split("/");
            var column = parts[0];

            var properties = typeof(T).GetProperties().Select(o => o.Name.ToUpperInvariant());
            if (!properties.Contains(column.ToUpperInvariant()))
            {
                throw new ArgumentException($"Input does not contain a valid column name: {input}");
            }

            // if the input only contains one part it is a real column
            if (parts.Length == 1)
            {
                return (column, null);
            }

            // otherwise the first part is the column, and the rest is the json path
            var jsonPathParameter = "$." + string.Join(".", parts.Skip(1).Select(o => $@"""{o}"""));
            return ($"JSON_VALUE({column}, @jsonPath)", jsonPathParameter);
        }


        /// <summary>
        /// Convert a path to either a column or proper json_value column and path
        /// Validates the column against a model passed in as T
        /// LocalDb and previous versions of SQL server do not support json paths as parameters
        /// </summary>
        /// <param name="input">column[/jsonpath]</param>
        /// <returns></returns>
        private (string column, string jsonPathParameter) ToPathOrColumnPre2017<T>(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Column cannot be null or empty");
            }

            var parts = input.Split("/");
            var column = parts[0];

            var properties = typeof(T).GetProperties().Select(o => o.Name.ToUpperInvariant());
            if (!properties.Contains(column.ToUpperInvariant()))
            {
                throw new ArgumentException($"Input does not contain a valid column name: {input}");
            }

            // if the input only contains one part it is a real column
            if (parts.Length == 1)
            {
                return (column, null);
            }

            // otherwise the first part is the column, and the rest is the json path
            var jsonPathParameter = "$." + string.Join(".", parts.Skip(1).Select(o => $@"""{o}"""));
            return ($"JSON_VALUE({column}, '{jsonPathParameter}')", null);
        }
    }
}
