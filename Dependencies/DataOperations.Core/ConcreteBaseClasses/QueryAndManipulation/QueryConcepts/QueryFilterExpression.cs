using System.Collections.Generic;

namespace DataOperations
{
    // Expression tree class where structure is a tree of  of filter expressions that can be structured as nodes in a 
    // recursive group / expression tree 

    // If there are no groupings then there will only be one child expression in the list with a conjunction of NULL
    // If there are groupings then there will be a list of children nodes in the list with Conjunctions joining the groups together
    // There might be a sub-conjunction in the list of children nodes

    // Example:Single Expression
    // root
    //   child1 (NULL) leftoperand, operator, rightoperand
    // ----------------------------------------

    // Example:Grouping
    // root
    //   child1 (AND) 
    //     child3 leftoperand, operator, rightoperand
    //     child4 leftoperand, operator, rightoperand
    //   child2 (OR)
    //     child5 leftoperand, operator, rightoperand
    //     child6 leftoperand, operator, rightoperand
    //      child7 (AND)
    //        child8 leftoperand, operator, rightoperand
    //        child9 leftoperand, operator, rightoperand
    //        child10 leftoperand, operator, rightoperand
    // ----------------------------------------

    // Let's build the root class - it can only have child nodes 
    // the child node can consist of an actual filtering expression - ODataFilterExpression("Name", FilterOperator.Equal, "John");

    // Usage: Single Expression filter 
    // ----------------------------------------
    // new ODataFilterExpression("Name", FilterOperator.Equal, "John");
    // Needs to output $Filter=Name eq 'John'

    // Usage: Grouping filter
    // ----------------------------------------
    // new ODataFilterExpression(
    //     new ODataFilterExpression("Name", FilterOperator.Equal, "John"), 
    //        FilterConjunctionOperator.And, 
    //        new ODataFilterExpression("Name", FilterOperator.Equal, "Jean");
    // Needs to output $Filter=Name eq 'John' and Name eq 'Jean'

    // Usage: Grouping filter with sub-grouping
    // ----------------------------------------
    // new ODataFilter(
    //   new ODataFilterExpression("Name", FilterOperator.Equal, "John"), 
    //     FilterConjunctionOperator.And, 
    //     new ODataFilterExpression("Name", FilterOperator.Equal, "Jean"), 
    //       FilterConjunctionOperator.Or, 
    //       new ODataFilterExpression("Name", FilterOperator.Equal, "Bert")
    // );
    
    // Needs to output $Filter=Name eq 'John' and (Name eq 'Jean' or Name eq 'Bert')

    // Usage: Grouping filter with sub-grouping and sub-sub-grouping
    // ----------------------------------------
    // new ODataFilterExpression(
    //   new ODataFilterExpression("Name", FilterOperator.Equal, "John"),
    //     FilterConjunctionOperator.And,
    //     new ODataFilterExpression("Name", FilterOperator.Equal, "Jean"),
    //       FilterConjunctionOperator.Or,
    //       new ODataFilterExpression("Name", FilterOperator.Equal, "Bert"),
    //         FilterConjunctionOperator.And,
    //         new ODataFilterExpression("Name", FilterOperator.Equal, "Beatrice")
    // );

    // Needs to output $Filter=Name eq 'John' and (Name eq 'Jean' or Name eq 'Bert' and (Name eq 'Beatrice'))

    public class QueryFilterExpression
    {
        public string FilterOperandLeft { get; set; }
        public FilterOperator Operator { get; set; }
        public object FilterOperandRight { get; set; }
        Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>> JoinedChildren = new Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>>();
        public QueryFilterExpression(string filterOperandLeft, FilterOperator operatorValue, string filterOperandRight)
        {
            FilterOperandLeft = filterOperandLeft;
            Operator = operatorValue;
            FilterOperandRight = filterOperandRight;
        }
        public QueryFilterExpression(Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>> joinedChildren)
        {
            JoinedChildren = joinedChildren;
        }
        public string RenderOutputAsFilterClause()
        {
            return $"$filter=" + RenderOutputAsFilterClauseRecurse().TrimEnd();
        }
        public bool IsFunction(FilterOperator operatorValue)
        {
            return operatorValue == FilterOperator.contains || operatorValue == FilterOperator.startswith || operatorValue == FilterOperator.endswith || operatorValue == FilterOperator.extended;
        }
        public string RenderOutputAsFilterClauseRecurse()
        {
            // ToDo: Should this be strongly typed and generated so you can't submit an invalid field name?
            // ToDo: Should this be type aware, so for a number we drop the quotes and for a string we don't?
            var filterClause = ""; 
            if (JoinedChildren.Count == 0)
            {  
                bool quoted = true; string quote = quoted ? "'" : "";
                if(IsFunction(Operator))
                    return $"{Operator}({FilterOperandLeft},{quote}{FilterOperandRight}{quote})";
                else
                    return $"{FilterOperandLeft} {Operator} {quote}{FilterOperandRight}{quote}";
                
            }
            else
            {
                // Parse the tree and build out the filter 
                // Firstly loop through the dictionary as we should have more than one entry in the dictionary and it might have children, if so then recurse through the children
                foreach (var item in JoinedChildren)
                {
                    FilterConjunctionOperator con = item.Key;
                    List<QueryFilterExpression> children = item.Value;
                    foreach (var child in children)
                    {    
                        bool quoted = true; string quote = quoted ? "'" : "";
                        string conj = (con != FilterConjunctionOperator.root) ? con.ToString() : "";
                        filterClause += $"{child.RenderOutputAsFilterClauseRecurse()} {conj} ";
                    }
                }
            }
            return filterClause;
        }

        // Create a factory method that is simpler than 
        //  new ODataFilterExpression(
        //                 new Dictionary<FilterConjunctionOperator, List<ODataFilterExpression>>()
        //                 {
        //                     {
        //                         FilterConjunctionOperator.and,
        //                         new List<ODataFilterExpression>()
        //                         {
        //                             new ODataFilterExpression("CustomerID", FilterOperator.eq, "Customer1"),
        //                             new ODataFilterExpression("CurrencyCode", FilterOperator.eq, "GBP"),
        //                             new ODataFilterExpression("GrossAmount", FilterOperator.gt, 100)
        //                         }
        //                     },
        //                     {
        //                         FilterConjunctionOperator.or,
        //                         new List<ODataFilterExpression>()
        //                         {
        //                             new ODataFilterExpression("CustomerID", FilterOperator.eq, "Customer2"),
        //                             new ODataFilterExpression("CurrencyCode", FilterOperator.eq, "USD"),
        //                             new ODataFilterExpression(
        //                                  new Dictionary<FilterConjunctionOperator, List<ODataFilterExpression>>()
        //                                  {  
        //                                     {
        //                                         FilterConjunctionOperator.and,
        //                                         new List<ODataFilterExpression>()
        //                                         {
        //                                             new ODataFilterExpression("BillingStatus", FilterOperator.ne, "X"),
        //                                         }
        //                                     }
        //                                 }
        //                             )
        //                         }
        //                     }
        //                 }
        //             )

        // To try and abstract away the complexity of the dictionary and the list of expressions, we can create a factory method that takes a SERIES OF expressions
        // and returns a single expression. This will be the root of the tree.
   
        QueryFilterExpression BuildFilterExpression(string filterOperandLeft, FilterOperator operatorValue, string filterOperandRight)
        {
            return new QueryFilterExpression(filterOperandLeft, operatorValue, filterOperandRight);
        }
        QueryFilterExpression BuildComplexFilterExpression(Dictionary<FilterConjunctionOperator, List<QueryFilterExpression>> joinedChildren)
        {
            return new QueryFilterExpression(joinedChildren);
        }
    }
}