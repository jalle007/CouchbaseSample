using Couchbase.Linq.Filters;
using Newtonsoft.Json;
using Couchbase.Linq.Filters;

namespace CouchbaseSample.Models
{
	[DocumentTypeFilter("Person")]
	public class Person
    {
	    public Person()
	    {
		    Type = "Person";
	    }
		[JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

	    public string Type { get; set; }

		//public string Type => typeof(Person).Name.ToLower();

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(FirstName) &&
                   !string.IsNullOrEmpty(LastName) &&
                   !string.IsNullOrEmpty(Email);
        }
    }
}