using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_BASEAPI.Json
{
    public static class Json2DataSetConverter
    {
        public static DataSet Convert(string json)
        {
            var token = (JToken)JsonConvert.DeserializeObject(json);
            var visitor = new DataSetBuildVisitor();
            FormatCheck(visitor, json);
            visitor.Visit(token);
            return visitor.DataSet;
        }

        public static DataSet Convert(string json, ConvertOptions options)
        {
            var token = (JToken)JsonConvert.DeserializeObject(json);
            var visitor = new DataSetBuildVisitor(options);
            FormatCheck(visitor, json);
            visitor.Visit(token);
            return visitor.DataSet;
        }

        private static void FormatCheck(DataSetBuildVisitor visitor, string json)
        {
            //string newjson = json.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
            visitor.IsArrayTable = true;
            //if (newjson.Contains("],["))
            //{
            //    visitor.IsArrayTable = true;
            //}
        }
    }
}
