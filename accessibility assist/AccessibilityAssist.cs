using System.Net.Http.Headers;
using System.Net.Http.Json;
using accessibility_assist;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

class AccessibilityAssist
{
    public static string personalaccesstoken = "PAT_HERE";

    static void Main(string[] args)
    {
        Runner();
    }

    static async void Runner()
    {
        var matchingWorkItemIds = await GetAccessibilityWorkItems();
        var workItemsWithPRs = new Dictionary<int, List<int>>(); // GetPRsAndWorkItems();

        foreach (int wi in matchingWorkItemIds)
        {
            Console.WriteLine(wi);
            var linkedPRs = GetWorkItemsLinkedPRs(wi).SyncResult();
            if (!linkedPRs.IsNullOrEmpty())
            {
                workItemsWithPRs.Add(wi, linkedPRs);
            }
        }
    }

    //static async Task<Dictionary<int, List<int>>> GetPRsAndWorkItems()
    //{
    //    var prsAndLinkedWorkItems = new Dictionary<int, List<int>>();

    //    try
    //    {
    //        using (HttpClient client = new HttpClient())
    //        {
    //            client.DefaultRequestHeaders.Accept.Add(
    //                new MediaTypeWithQualityHeaderValue("application/json"));

    //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
    //                Convert.ToBase64String(
    //                    System.Text.ASCIIEncoding.ASCII.GetBytes(
    //                        string.Format("{0}:{1}", "", personalaccesstoken))));

    //            using (HttpResponseMessage response = client.GetAsync(
    //                $"https://dev.azure.com/mseng/AzureDevOps/_apis/git/repositories/AzureDevOps/pullrequests?api-version=5.1&searchCriteria.includeLinks=true&searchCriteria.status=completed&$top=10").Result)
    //            {
    //                response.EnsureSuccessStatusCode();
    //                string responseBody = await response.Content.ReadAsStringAsync();
    //                JObject o = JObject.Parse(responseBody);
    //                foreach (var p in o)
    //                {
    //                    if (p.Value["value"].Value<JObject>("pullRequestId").Value <)
    //                    {
    //                        //do something
    //                    }
    //                }

    //                var prs = new List<int>();

    //                foreach (int pr in prs)
    //                {
    //                    using (HttpResponseMessage workItemsResponse = client.GetAsync(
    //                        $"https://dev.azure.com/mseng/AzureDevOps/_apis/git/repositories/AzureDevOps/pullRequests/{pr}/workitems?api-version=5.1").Result)
    //                    {
    //                        workItemsResponse.EnsureSuccessStatusCode();
    //                        string responseBody2 = await response.Content.ReadAsStringAsync();
    //                        var responseJson = JsonConvert.DeserializeObject<PRResponse>(responseBody2);
    //                        var prsWIs = new List<int>();
    //                        prsWIs = responseJson.value.Select(wi => wi.Id).Select(int.Parse).ToList();
    //                        prsAndLinkedWorkItems.Add(361393, prsWIs);
    //                        Console.WriteLine(prsAndLinkedWorkItems.FirstOrDefault().Key);
    //                        Console.WriteLine(prsAndLinkedWorkItems.FirstOrDefault().Value.FirstOrDefault());
    //                        Console.WriteLine(prsAndLinkedWorkItems.FirstOrDefault().Value.FirstOrDefault());
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.ToString());
    //    }

    //    return prsAndLinkedWorkItems;
    //}

    static async Task<List<int>> GetWorkItemsLinkedPRs(int workItemId)
    {
        Console.WriteLine("WorkItem: ", workItemId);

        var workItemsLinkedPRs = new List<int>();
        var workItemsLinkedPRst = new List<string>();

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", personalaccesstoken))));

                using (HttpResponseMessage response = client.GetAsync(
                    $"https://dev.azure.com/mseng/AzureDevOps/_apis/wit/workitems/{workItemId}?api-version=6.0&$expand=relations").Result)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<WorkItemApiResponse>(responseBody);
                    workItemsLinkedPRst = responseJson.relations.Where(relation => relation.rel == "ArtifactLink" && relation.attributes.name == "Pull Request").Select(relation => relation.url).ToList();
                    Console.WriteLine(workItemsLinkedPRst.FirstOrDefault());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return workItemsLinkedPRs;
    }

    private static async Task<List<int>> GetAccessibilityWorkItems()
    {
        var matchingWorkItemIds = new List<int>();

        try
        {
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", personalaccesstoken))));

                var myObject = new
                {
                    query = "SELECT [System.Id] FROM workitems WHERE [System.State] = 'Closed' AND [System.WorkItemType] = 'Bug' AND [System.Tags] Contains 'A11yMAS' AND [System.Tags] Contains 'High contrast' ORDER BY [System.ChangedDate] DESC"
                };

                JsonContent content = JsonContent.Create(myObject);

                using (HttpResponseMessage response = client.PostAsync(
                            "https://dev.azure.com/mseng/_apis/wit/wiql?api-version=6.0", content).Result)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<WorkItemQueryResult>(responseBody);
                    matchingWorkItemIds = responseJson.WorkItems.Select(wi => wi.Id).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return matchingWorkItemIds;
    }
}