//using Core.Framework.Utility.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_BASEAPI.Json
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class DataSetBuildVisitor : JsonVisitor
    {
        private const string BaseTableIdentifier = "#";
        private readonly Dictionary<string, DataTable> dataTables = new Dictionary<string, DataTable>();
        private readonly ConvertOptions options;
        private long rowNumber;
        private DataSet dataSet;
        
        //Build Data Relation
        private const string defaultID = "_id";
        private const string parentID = "_parent_id";
        private string GUID = string.Empty;
        private string parentGUID = string.Empty;
        private string parentTableName = string.Empty;

        internal DataSetBuildVisitor()
            : this(ConvertOptions.Default)
        { }

        internal DataSetBuildVisitor(ConvertOptions options)
        {
            this.options = options;
        }

        #region Private Static Methods
        private static string GetDictionaryKey(JToken token)
        {
            for (JToken c = token; c != null; c = c.Parent)
            {
                if (c is JProperty)
                {
                    return (c as JProperty).Name;
                }
            }
            return BaseTableIdentifier;
        }

        private static string GetParentTable(JToken token, string tableName)
        {
            for (JToken c = token; c != null; c = c.Parent)
            {
                if (c is JProperty)
                {
                    string propertyName = (c as JProperty).Name;
                    if (tableName != propertyName)
                    {
                        return propertyName;
                    }
                }
            }
            return "";
        }

        private static bool IsPrimitiveValue(JValue value)
        {
            return value.Type == JTokenType.Boolean ||
                value.Type == JTokenType.Date ||
                value.Type == JTokenType.Float ||
                value.Type == JTokenType.Guid ||
                value.Type == JTokenType.Integer ||
                value.Type == JTokenType.String ||
                value.Type == JTokenType.TimeSpan;
        }
        #endregion

        #region Private Methods
        private DataTable PrepareDataTable(JToken token)
        {
            var key = GetDictionaryKey(token);
            if (key == BaseTableIdentifier)
            {
                ++rowNumber;
            }
            DataTable dataTable = null;
            if (token.Type == JTokenType.Array)
            {
                if (token.Path.Length < PrePath.Length)
                {
                    DynamicTableIndex++;
                }
                key = key + DynamicTableIndex;
                if (this.dataTables.ContainsKey(key))
                {
                    dataTable = dataTables[key];
                }
                else
                {
                    string tableName = key == BaseTableIdentifier ? options.BaseTableName : key;
                    dataTable = new DataTable(tableName);
                    dataTable.Columns.Add(options.KeyColumnName);

                    this.dataTables.Add(key, dataTable);
                }
                PrePath = token.Path;
            }
            else
            {
                if (this.dataTables.ContainsKey(key))
                {
                    dataTable = dataTables[key];
                }
                else
                {
                    string tableName = key == BaseTableIdentifier ? options.BaseTableName : key;
                    dataTable = new DataTable(tableName);
                    dataTable.Columns.Add(options.KeyColumnName);

                    this.dataTables.Add(key, dataTable);
                }
            }
            return dataTable;
        }

        private string hasChild(JToken obj)
        {
            foreach (var child in obj.Children())
            {
                if (child.Type == JTokenType.Object)
                {
                    var table = PrepareDataTable(child);
                    if (table != null)
                    {
                        return table.TableName;
                    }
                }
                else
                {
                    string childName = hasChild(child);
                    if (!string.IsNullOrEmpty(childName))
                    {
                        return childName;
                    }
                }
            }

            return "";
        }
        #endregion

        #region Protected Methods
        protected override void VisitObject(JObject obj)
        {
            var dataTable = PrepareDataTable(obj);
            if (!dataTable.Columns.Contains(defaultID))
                dataTable.Columns.Add(defaultID);

            GUID = Guid.NewGuid().ToString("N"); //GuidHelper.NewGuid(GuidHelper.GuidTypeEnum.N);
            var row = dataTable.NewRow();

            string childName = hasChild(obj);
            if (!string.IsNullOrEmpty(childName))
            {
                parentGUID = GUID;
            }

            parentTableName = GetParentTable(obj, dataTable.TableName);
            if (!string.IsNullOrEmpty(parentTableName))
            {
                if (!dataTable.Columns.Contains(parentID))
                {
                    dataTable.Columns.Add(parentID);
                }
                row[parentID] = parentGUID;
            }

            row[options.KeyColumnName] = rowNumber;
            row[defaultID] = GUID;
            dataTable.Rows.Add(row);
        }

        protected override void VisitProperty(JProperty property)
        {
            var propertyValue = property.Value as JValue;
            if (property.Value is JValue && propertyValue != null)
            {
                if (propertyValue.Value != null)
                {
                    if (IsPrimitiveValue(propertyValue))
                    {
                        var key = GetDictionaryKey(property.Parent);

                        var dataTable = this.dataTables[key];

                        var row = dataTable.Rows[dataTable.Rows.Count - 1];

                        if (!dataTable.Columns.Contains(property.Name))
                        {
                            dataTable.Columns.Add(property.Name, propertyValue.Value.GetType());
                        }

                        row[property.Name] = propertyValue.Value;
                    }
                }
                else
                {
                    var key = GetDictionaryKey(property.Parent);

                    var dataTable = this.dataTables[key];

                    var row = dataTable.Rows[dataTable.Rows.Count - 1];
                    if (!dataTable.Columns.Contains(property.Name))
                    {
                        dataTable.Columns.Add(property.Name, typeof(string));
                    }

                    row[property.Name] = "";
                }
            }
            else
            {
                var key = GetDictionaryKey(property.Parent);

                var dataTable = this.dataTables[key];

                var row = dataTable.Rows[dataTable.Rows.Count - 1];
                if (!dataTable.Columns.Contains(property.Name))
                {
                    dataTable.Columns.Add(property.Name, typeof(string));
                }

                row[property.Name] = "";
            }
        }

        protected override void VisitArray(JArray array)
        {
            if (!string.IsNullOrEmpty(array.Path)) // TODO: Performance improvement
            {
                var table = this.PrepareDataTable(array);
                foreach (JValue val in array.Children<JValue>())
                {
                    if (val != null && val.Value != null && IsPrimitiveValue(val))
                    {
                        if (!table.Columns.Contains(options.ArrayValueColumnName))
                        {
                            table.Columns.Add(options.ArrayValueColumnName, val.Value.GetType());
                        }

                        var row = table.NewRow();
                        row[options.KeyColumnName] = rowNumber;
                        row[options.ArrayValueColumnName] = val.Value;
                        table.Rows.Add(row);
                    }
                }
            }
        }

        protected override void VisitArrayRow(JArray array)
        {
            if (!string.IsNullOrEmpty(array.Path)) // TODO: Performance improvement
            {
                var table = this.PrepareDataTable(array);
                int len = array.Children<JValue>().Count();
                if (len > 0)
                {
                    var row = table.NewRow();
                    int DynamicColIndex = 0;
                    foreach (JValue val in array.Children<JValue>())
                    {
                        if (val.Value == null)
                            val.Value = "";
                        if (val != null && val.Value != null && IsPrimitiveValue(val))
                        {
                            if (!table.Columns.Contains(options.ArrayValueColumnName + DynamicColIndex))
                            {
                                table.Columns.Add(options.ArrayValueColumnName + DynamicColIndex, val.Value.GetType());
                            }

                            row[options.KeyColumnName] = rowNumber;
                            row[options.ArrayValueColumnName + DynamicColIndex] = val.Value;
                        }
                        DynamicColIndex++;
                    }
                    table.Rows.Add(row);
                }
            }
        }
        #endregion

        #region Internal Methods
        internal DataSet DataSet
        {
            get
            {
                if (dataSet == null)
                {
                    dataSet = new DataSet();
                    foreach (var dataTable in this.dataTables.Values)
                    {
                        dataSet.Tables.Add(dataTable);
                    }
                    if (dataSet.Tables.Count > 0)
                    {
                        var parentTable = dataSet.Tables[options.BaseTableName];
                        if (parentTable != null)
                        {
                            var parentIndex = dataSet.Tables.IndexOf(parentTable);
                            for (int idx = 0; idx < dataSet.Tables.Count; idx++)
                            {
                                if (idx != parentIndex)
                                {
                                    var childTable = dataSet.Tables[idx];
                                    var relation = new DataRelation(string.Format("{0}.{1}", parentTable.TableName, childTable.TableName),
                                        parentTable.Columns[options.KeyColumnName],
                                        childTable.Columns[options.KeyColumnName]);
                                    dataSet.Relations.Add(relation);
                                }
                            }
                        }
                    }
                }
                return dataSet;
            }
        }
        #endregion
    }
}
