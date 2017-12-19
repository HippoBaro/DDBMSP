namespace DDBMSP.Entities.Query
{
    public class QueryDefinition
    {
        public string Name { get; set; }
        public string SelectorLambda { get; set; }
        public string AggregationLambda { get; set; }
        public string ReturnTypeName { get; set; }
    }
}