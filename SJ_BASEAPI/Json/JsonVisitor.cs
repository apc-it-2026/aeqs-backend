using Newtonsoft.Json.Linq;

namespace SJ_BASEAPI.Json
{
    internal abstract class JsonVisitor
    {
        internal JsonVisitor()
        { }

        protected virtual void VisitArray(JArray array) { }
        protected virtual void VisitArrayRow(JArray array) { }

        protected virtual void VisitObject(JObject obj) { }

        protected virtual void VisitProperty(JProperty property) { }

        internal string PrePath = string.Empty;

        internal int DynamicTableIndex = 0;

        internal bool IsArrayTable = false;

        internal void Visit(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    if (IsArrayTable)
                    {
                        VisitArrayRow((JArray)token);
                    }
                    else
                    {
                        VisitArray((JArray)token);
                    }
                    break;
                case JTokenType.Property:
                    VisitProperty((JProperty)token);
                    break;
                case JTokenType.Object:
                    VisitObject((JObject)token);
                    break;
            }
            foreach (var child in token.Children())
            {
                this.Visit(child);
            }
        }
    }
}
