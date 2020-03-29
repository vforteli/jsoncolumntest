using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jsontestapp.Models
{
    public class RowModel
    {
        public Guid PartitionId { get; set; }
        public Guid ObjectId { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string DynamicData { get; set; }
        public dynamic DynamicData1 { get; set; }
        public dynamic DynamicData2 { get; set; }
        public dynamic DynamicData3 { get; set; }
        public dynamic DynamicData4 { get; set; }
        public dynamic DynamicData5 { get; set; }
        public string TestProperty { get; set; }
    }
}
