using System.Text.RegularExpressions;

namespace accessibility_assist
{
    public class Utilities
    {
        public static string prIdRegexPattern = ".*%2[fF](?<prId>.*)";

        public static string? ParsePRIdFromArtifactUrl(string artifactLink)
        {
            Regex RE = new Regex(prIdRegexPattern);
            Match theMatch = RE.Match(artifactLink);
            theMatch.Groups.TryGetValue("prId", out Group? prId);
            return prId?.Value;
        }

        public static void PrintWorkItemsAndLinkedPRUrls(Dictionary<int, List<string>> workItemsAndLinkedPRUrls)
        {
            Console.WriteLine("\nWorkItemLink\n--> PR #1\n-->PR #2");
            foreach (int wiId in workItemsAndLinkedPRUrls.Keys)
            {
                Console.WriteLine("\n----------------------------------------------------------------");
                Console.WriteLine("https://dev.azure.com/mseng/AzureDevOps/_workitems/edit/" + wiId);
                foreach (string prId in workItemsAndLinkedPRUrls[wiId])
                {
                    Console.WriteLine("--> https://dev.azure.com/mseng/AzureDevOps/_git/AzureDevOps/pullrequest/" + prId + "?_a=files");
                }
            }
            Console.WriteLine("\n----------------------------------------------------------------");
        }
    }
}
