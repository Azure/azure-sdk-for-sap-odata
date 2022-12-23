using System;
using System.Collections.Generic;
using System.Linq;

namespace DataOperations
{
    public class QueryFilter
    {
        // Represents the odata filter construct which is list of expressions, that can contain other nested expressions.
        public QueryFilterExpression Filter {get;set;}
        private Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>> _expressions = new Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>>();
        public bool IsValid() {
            return Filter != null || _expressions.Any();
        }
        
        // ctor 
        public QueryFilter(QueryFilterExpression Filter)
        {
            this.Filter = Filter;
            this._expressions.Add(FilterConjunctionOperator.root, new List<QueryFilterExpression>() { Filter });
        }

        // Factory method for the odata filter construct
        public static QueryFilter FilterFactory(QueryFilterExpression Filter)
        {
            return new QueryFilter(Filter);
        }
        // Factory if our initial filter contains nesting
        public static QueryFilter FilterFactory(Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>> joinedChildren)
        {
            return new QueryFilter(new QueryFilterExpression(joinedChildren));
        }
        public void AddBlockToFilter(FilterConjunctionOperator fcq, IEnumerable<QueryFilterExpression> Filters)
        {
            if (this._expressions.Count == 0) this._expressions.Add(fcq, new List<QueryFilterExpression>(){Filter});
            else if (fcq != FilterConjunctionOperator.root)
            {
                this._expressions.Add(fcq, Filters.ToList());
            }
            else
            {
                throw new Exception("Cannot add a conjunction block to the root of the filter use the conjunction operator 'root'");
            }
        }
        
        public void AddSingleToFilter(FilterConjunctionOperator fcq, QueryFilterExpression Filter) 
        {
            this.AddBlockToFilter(fcq, new List<QueryFilterExpression>(){Filter});
        }
        public void AddFromDictionary(FilterConjunctionOperator fcq, Dictionary<string, object> KeyValuePairs)
        {
            
            var qfelist = new List<QueryFilterExpression>();

            foreach(KeyValuePair<string, object> KeyValuePair in KeyValuePairs)
            {
                qfelist.Add(new QueryFilterExpression(KeyValuePair.Key, FilterOperator.eq, KeyValuePair.Value.ToString()));
            }
            this._expressions.Add(fcq, qfelist);
        }
    }
}