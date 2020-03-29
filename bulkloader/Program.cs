using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace bulkloader
{
    class Program
    {
        private static readonly Random _random = new Random();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting bulk insert");
            var connectionString = "Data Source=FL494;Initial Catalog=efcore;Integrated Security=True;";
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                var table = new DataTable();
                table.Columns.Add("PartitionId", typeof(Guid));
                table.Columns.Add("ObjectId", typeof(Guid));
                table.Columns.Add("DynamicData", typeof(String));


                var PartitionId = new Guid(i.ToString().PadLeft(32).Replace(' ', '0'));

                for (int items = 0; items < 100000; items++)
                {
                    var ObjectId = Guid.NewGuid();
                    var randomstuff = GenerateObject();
                    table.Rows.Add(PartitionId, ObjectId, randomstuff);
                }

                var bulkcopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "jsontest",
                    BatchSize = 10000,
                    BulkCopyTimeout = 300,
                };

                await bulkcopy.WriteToServerAsync(table);
                Console.WriteLine($"Wrote partition {i} after {stopwatch.ElapsedMilliseconds} ms");
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    var table = new DataTable();
            //    table.Columns.Add("PartitionId", typeof(Guid));
            //    table.Columns.Add("ObjectId", typeof(Guid));
            //    table.Columns.Add("DynamicData1", typeof(String));
            //    table.Columns.Add("DynamicData2", typeof(String));
            //    table.Columns.Add("DynamicData3", typeof(String));
            //    table.Columns.Add("DynamicData4", typeof(String));
            //    table.Columns.Add("DynamicData5", typeof(String));

            //    var PartitionId = new Guid(i.ToString().PadLeft(32).Replace(' ', '0'));

            //    for (int users = 0; users < 50000; users++)
            //    {
            //        var ObjectId = Guid.NewGuid();
            //        var randomstuff = GeneratePartitionedObject();
            //        table.Rows.Add(PartitionId, ObjectId, randomstuff[0], randomstuff[1], randomstuff[2], randomstuff[3], randomstuff[4]);
            //    }

            //    var bulkcopy = new SqlBulkCopy(connection)
            //    {
            //        DestinationTableName = "jsontestpartitioned",
            //        BatchSize = 10000,
            //        BulkCopyTimeout = 300,
            //    };


            //    await bulkcopy.WriteToServerAsync(table);
            //    Console.WriteLine($"Wrote partition {i} after {stopwatch.ElapsedMilliseconds} ms");
            //}

            Console.WriteLine($"Done after {stopwatch.ElapsedMilliseconds}ms");
        }


        public static string GenerateObject()
        {
            var foo = new
            {
                Property1 = _random.Next(0, 100),
                Property2 = _random.Next(10000, 100000),
                Property3 = _random.Next(10000, 100000),
                Property4 = "sameforall",
                Property5 = "somethingslightlylongerdsf sdkfjhs dfkjhdsfksjd hfkjkf jsdfkj ksdfhere lol wtf hurr derp?",
                Proandsincethesecanbereeeeeeeeeaallybathshitinsanelonghereissomerealldlongtextwtgfsfperty6 = "somethingslightlylongerdsf sdkfjhs dfkjhdsfksjd hfkjkf jsdfkj ksdfhere lol wtf hurr derp?",
                Proandsincethesfdfffecanbereeeeeeeeeaallybathshitinsanelonghereissomerealldlongtextwtgfsfperty6 = "somethinjsdfkj ksdfhere lol wtf hurr derp?",
                PropertyClasses = new Dictionary<String, Object>(),
                PropertyClassesNumbers = new Dictionary<String, Object>(),
                PropertyClassesNumbersNumber = new Dictionary<String, Object>(),
                PropertyClassesfoo = new Dictionary<String, Object>(),
                PropertyClassesbar = new Dictionary<String, Object>(),
            };

            Enumerable.Range(2015, 5).ToList().ForEach(o =>
            {
                foo.PropertyClasses.Add(o.ToString(), new
                {
                    x = _random.Next(10000, 20000),
                    y = _random.Next(5000, 10000),
                });
            });
            Enumerable.Range(0, 20).ToList().ForEach(o => { foo.PropertyClassesNumbers.Add($"subpropdynamic{o}", _random.Next(5000, 20000)); });
            Enumerable.Range(2000, 20).ToList().ForEach(o => { foo.PropertyClassesNumbersNumber.Add(o.ToString(), _random.Next(5000, 20000)); });
            Enumerable.Range(2010, 10).ToList().ForEach(o => { foo.PropertyClassesfoo.Add(o.ToString(), _random.Next(5000, 20000)); });
            Enumerable.Range(2000, 20).ToList().ForEach(o => { foo.PropertyClassesbar.Add(o.ToString(), _random.Next(5000, 20000)); });

            return JsonSerializer.Serialize(foo);
        }


        public static List<string> GeneratePartitionedObject()
        {
            var partition1 = new
            {
                Property1 = _random.Next(0, 100),
                Property2 = _random.Next(10000, 100000),
                Property3 = _random.Next(10000, 100000),
                Property4 = "sameforall",
                Property5 = "somethingslightlylongerdsf sdkfjhs dfkjhdsfksjd hfkjkf jsdfkj ksdfhere lol wtf hurr derp?",
                Proandsincethesecanbereeeeeeeeeaallybathshitinsanelonghereissomerealldlongtextwtgfsfperty6 = "somethingslightlylongerdsf sdkfjhs dfkjhdsfksjd hfkjkf jsdfkj ksdfhere lol wtf hurr derp?",
                Proandsincethesfdfffecanbereeeeeeeeeaallybathshitinsanelonghereissomerealldlongtextwtgfsfperty6 = "somethinjsdfkj ksdfhere lol wtf hurr derp?",
            };

            var partition2 = new { PropertyClasses = new Dictionary<String, String>() };
            for (int i = 0; i < _random.Next(10, 30); i++)
            {
                partition2.PropertyClasses.Add($"subprop{i}", "somevaluehere");
            }

            var partition3 = new { PropertyClassesNumbers = new Dictionary<String, int>() };
            for (int i = 0; i < _random.Next(10, 30); i++)
            {
                partition3.PropertyClassesNumbers.Add($"subpropdynamic{i}", _random.Next(5000, 20000));
            }

            var partition4 = new { PropertyClassesfoo = new Dictionary<String, int>() };
            for (int i = 0; i < _random.Next(20, 30); i++)
            {
                partition4.PropertyClassesfoo.Add($"subp fefe ef lekfwe fwefwef wefwefwefwef efewfropdynamic{i}", _random.Next(5000, 20000));
            }

            var partition5 = new { PropertyClassesbar = new Dictionary<String, int>() };
            for (int i = 0; i < _random.Next(20, 30); i++)
            {
                partition5.PropertyClassesbar.Add($"subp fefe eefefeff lekfwe fwefwef wefwefwefwef efewfropdynamic{i}", _random.Next(5000, 20000));
            }

            return new List<string>{
                JsonSerializer.Serialize(partition1),
                JsonSerializer.Serialize(partition2),
                JsonSerializer.Serialize(partition3),
                JsonSerializer.Serialize(partition4),
                JsonSerializer.Serialize(partition5),
            };
        }
    }
}

