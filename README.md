# 💡 Accessibility Assist

A hackathon project to supply PRs from your organization that fix similar accessibility bugs.

This doesn't already exist:https://developercommunity.visualstudio.com/t/include-pull-requests-in-queries/365706  

In fact, you can't search for work items that have linked pull requests at all: https://developercommunity.visualstudio.com/t/include-pull-requests-in-queries/365706#T-N1687999

## Mockup iterations

#### As a command line tool (POC)...
![image](https://user-images.githubusercontent.com/35906111/191382289-7fdd8953-ab03-4b1d-9148-d08eaa561549.png)

#### As a comment on the accessibility work item (iteration 1 goal)...

![image](https://user-images.githubusercontent.com/35906111/191381029-59f83f44-8b83-42aa-9e48-2e7b8179b6b5.png)

#### A comment with feedback options (stretch)...
![image](https://user-images.githubusercontent.com/35906111/191382097-a82bcf29-0ed4-40c0-8957-3f1829668c03.png)


## Other things I've learned during the hackathon
You can define your own custom PR filtering in ADO: [MSFT Docs: Define a custom pull request view](https://learn.microsoft.com/en-us/azure/devops/repos/git/view-pull-requests?view=azure-devops&tabs=browser#define-a-custom-pull-request-view)  
To get even fancier, this extension has advanced filtering (esp. by keyword): [Pull request manager hub](https://marketplace.visualstudio.com/items?itemName=caribeiro84.pull-request-manager-hub)

To convert a shared query to WIQL, just export it: https://stackoverflow.com/questions/51618736/tfs-query-editor-is-there-a-way-to-get-the-underlying-wiql-for-a-given-tfs-shar
