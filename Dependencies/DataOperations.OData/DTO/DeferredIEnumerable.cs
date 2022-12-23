using System.Text.Json.Serialization;
using DataOperations.OData;
using Newtonsoft.Json;

namespace DataOperations
{
    public class DeferredEnumerable<T> where T : IBaseDTOWithIDAndETag
    {

        [Newtonsoft.Json.JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public IOperationsDispatcher Dispatcher {protected internal get; set; }
        
        [Newtonsoft.Json.JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Deferral __deferred { get; set; }
        private List<T> _Results = new List<T>(); 
        [Newtonsoft.Json.JsonIgnore()]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<T> Results 
        {
            get
            {

                // dispatcher.Call and and deserialize a T from  the URI results, then 
                // nuke the __deferred setting so we shortcut in future
                if(__deferred?.uri == null)
                {
                    // We're already loaded, so return the result
                    return _Results;
                }
                else
                {
                    // Fetch the results from the URI and deserialize a T on a background task 
                    // but block the caller till the result is available

                    // Note that if you don't want to block the caller, you can use the
                    // awaitable GetResultsAsync() method which returns a ValueTask<T>
                    _Results = GetResultsAsync().Result;
                    return _Results;
                }
            }
            private set
            {
                _Results = value;
            }
        }
        public async Task<List<T>> GetResultsAsync()
        {
 
            if(__deferred?.uri == null)
            {
                // We're already loaded, so return the result
                return _Results;
            }
            else
            {
                // Fetch the results from the URI and deserialize a List<T>
                // Instance for the results

                //dispatcher.Call and render the URI results inline
                // then null out the __deferred setting so we shortcut in future

                Results = await Dispatcher.DispatchThroughDeferredUrlIEnumerable<T>(__deferred.uri);
                __deferred = null;

                // Return the results
                return Results;
            }
        }
    }
}
