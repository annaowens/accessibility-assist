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
        var workItemsWithPRs = new Dictionary<int, List<string>>();

        foreach (int wi in matchingWorkItemIds)
        {
            var linkedPRs = GetWorkItemsLinkedPRs(wi).SyncResult();
            if (!linkedPRs.IsNullOrEmpty())
            {
                workItemsWithPRs.Add(wi, linkedPRs);
            }
        }

        Utilities.PrintWorkItemsAndLinkedPRUrls(workItemsWithPRs);
    }

    static async Task<List<string>> GetWorkItemsLinkedPRs(int workItemId)
    {
        var workItemsLinkedPRs = new List<string>();

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
                    workItemsLinkedPRs = responseJson.relations
                        .Where(relation => relation.rel == "ArtifactLink" && relation.attributes.name == "Pull Request")
                        .Select(relation => Utilities.ParsePRIdFromArtifactUrl(relation.url))
                        .Where(prId => !prId.IsNullOrEmpty())
                        .ToList();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return workItemsLinkedPRs;
    }

    private static async Task<List<int>> GetAccessibilityWorkItems(int max = 10)
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
                    matchingWorkItemIds = responseJson.WorkItems.Select(wi => wi.Id).Take(max).ToList(); ;
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