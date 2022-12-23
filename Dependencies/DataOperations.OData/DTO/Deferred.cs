using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using DataOperations.OData;
using Newtonsoft.Json;

namespace DataOperations
{
    public class Deferred<T> where T : IBaseDTOWithIDAndETag
    {
        [Newtonsoft.Json.JsonIgnore()]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public IOperationsDispatcher Dispatcher {protected internal get; set; }

        [Newtonsoft.Json.JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Deferral __deferred { get; set; }
        
        private T _Result = default(T); 
        [Newtonsoft.Json.JsonIgnore()]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public T Result { 
            get
            { 
                // dispatcher.Call and and deserialize a T from  the URI results, then 
                // nuke the __deferred setting so we shortcut in future
                if(__deferred?.uri == null)
                {
                    // We're already loaded, so return the result
                    return _Result;
                }
                else
                {
                    // Fetch the results from the URI and deserialize a T on a background task 
                    // but block the caller till the result is available

                    // Note that if you don't want to block the caller, you can use the
                    // awaitable GetAsync() method which returns a ValueTask<T>
                    _Result = GetAsync().Result;
                    return _Result;
                }
            }
            set
            {
                _Result = value;
            }
        } 
        public async ValueTask<T> GetAsync()
        {
            if(__deferred?.uri == null)
            {
                // We're already loaded, so return the result
                return _Result;
            }
            else
            {
                Result = await Dispatcher.DispatchThroughDeferredURL<T>(__deferred.uri);
                __deferred = null;
                return _Result;
            }
        }
    }    
}


