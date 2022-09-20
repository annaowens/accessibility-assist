using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace accessibility_assist
{

    public class PullRequestApiResponse
    {
        public int count { get; set; } = 0;
        public ResourceRef[] value = Array.Empty<ResourceRef>();
    }

    public class WorkItemApiResponse
    {
        public int id { get; set; }

        [JsonIgnore]
        public int rev { get; set; }

        [JsonIgnore]
        public JsonObject? fields { get; set; }

        public WorkItemRelationsApiResponse[] relations { get; set; } = Array.Empty<WorkItemRelationsApiResponse>();

        [JsonIgnore]
        public JsonObject? _links { get; set; }

        [JsonIgnore]
        public string? url { get; set; }


    }

    public class WorkItemRelationsApiResponse
    {
        public string rel { get; set; }

        public string url { get; set; }

        public WorkItemRelationsAttributesApiResponse attributes = new WorkItemRelationsAttributesApiResponse();
    }

    public class WorkItemRelationsAttributesApiResponse
    {

        [JsonIgnore]
        public string? authorizedDate { get; set; }

        [JsonIgnore]
        public int? id { get; set; }

        [JsonIgnore]
        public string? resourceCreatedDate { get; set; }

        [JsonIgnore]
        public string? resourceModifiedDate { get; set; }

        [JsonIgnore]
        public string? RevisedDate { get; set; }

        [JsonIgnore]
        public string? Comment { get; set; }

        public string name { get; set; }
    }
}