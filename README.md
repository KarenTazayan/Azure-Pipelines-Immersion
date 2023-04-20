## Azure Pipelines Immersion

This guidline provide detailed steps to orgnize fully automated
Azure Pipelines for the sample solution based on Blazor Server model which works on .NET 7 and Microsoft Orleans 7.1. It mostly uses the following services: Azure Container Apps, Azure SignalR Service, Azure Key Vault, Azure Storage Account, Azure Application Insights, Azure Load Testing, Azure DevOps and many more.

### 1. Create an Azure DevOps project for the solution.

1. Open Azure DevOps portal : https://dev.azure.com/
2. Create new organization/use an existing organization. A sample name: **azure-pipelines-immersion-1**
3. Create a new public project. A sample name: **ShoppingApp**. For a public project Microsoft provides 
[unlimited restriction for parallel jobs](https://learn.microsoft.com/en-us/azure/devops/pipelines/licensing/concurrent-jobs) (by default up to 25 parallel jobs) in case you use self-hosted agents.  
4. Import existing repository: https://github.com/KarenTazayan/Azure-Pipelines-Immersion.git
5. Create YAML pipeline by using exsisting "Azure-Pipelines.yml" from imporeted sources. A sample name: **Default CI and CD** pipeline. After creation just only save it, don't run it.
   
### 2. Install Docker Desktop on your machine.

Install [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/) on operation system which you are using.  

>Attention! The next steps below (till section 3) are optional.

If you are using Windows 11 or Windows 10 it is more appropriate to use WSL 2 and install Docker Desktop on Ubuntu-20.04.

What are required for this?  
> - Microsoft Azure Subscription, [you can create a free account](https://azure.microsoft.com/en-us/free/) if you don't have any.
> - [Windows machine with virtualization technology (AMD-V / Intel VT-x)](https://learn.microsoft.com/en-us/virtualization/hyper-v-on-windows/user-guide/nested-virtualization)
>   - Windows Server 2016/Windows 10 or greater for Intel processor with VT-x
>   - Windows Server 2022/Windows 11 or greater AMD EPYC/Ryzen processor

Open terminal enable WSL 2 and install Ubuntu-20.04 with the following command. 
```
wsl --install -d Ubuntu-20.04
```
Install [Docker Engine](https://docs.docker.com/engine/install/ubuntu/) on Ubuntu.
```
$ sudo apt-get update
$ sudo apt-get install ca-certificates curl gnupg
$ sudo mkdir -m 0755 -p /etc/apt/keyrings
$ curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
$ echo \
  "deb [arch="$(dpkg --print-architecture)" signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  "$(. /etc/os-release && echo "$VERSION_CODENAME")" stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
$ sudo apt-get update
$ sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```
Start Docker service.
```
sudo service docker start
sudo service docker status
```
You can clone the solution you have imported https://dev.azure.com/azure-pipelines-immersion-1/_git/ShoppingApp into C:\ drive on your Windows machine ([Git CLI](https://git-scm.com/download/win) is required)
```
mkdir C:\Repos
cd C:\Repos
git clone https://dev.azure.com/azure-pipelines-immersion-1/_git/ShoppingApp
```
and interop with it from Ubuntu-20.04 by the following way:
```
$ cd /mnt/c/Repos/ShoppingApp/build/azure-pipelines-agents/debian-10.13/
$ sudo docker build -t azure-pipelines-agents-debian-10.13:20042023 .
```

### 3. Create a self-hosted agents pool for the Azure DevOps organization.

Build an agent docker image by using files from "build\azure-pipelines-agents" based on Debian image
```
docker build -t azure-pipelines-agents-debian-10.13:20042023 .
```
or on Ubuntu image.
```
docker build -t azure-pipelines-agents-debian-10.13:20042023 .
```
Also create Playwright image
```
docker build -t azure-pipelines-agents-playwright-1.32.0:20042023 .
```
Create [Azure DevOps personal access token (PAT token)](https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate). For the scope select: Agent Pools (read, manage), Deployment group (read, manage).  
Run Debian or Ubuntu based Azure Pipelines agent by using the following command:
```
docker run -v /var/run/docker.sock:/var/run/docker.sock \
    -e AZP_URL=https://dev.azure.com/azure-pipelines-immersion-1 \
    -e AZP_TOKEN=<PAT token> -e AZP_AGENT_NAME=01_Debian-10.13 \
    -e AZP_POOL=Default -e AZP_WORK=_work --name 01_Debian-10.13 azure-pipelines-agents-debian-10.13:20042023
```
The syntax above uses PowerShell. If you use Bash shell, just replace "`" (backtick) with "\\" (backslash).  
  
>Warning! Doing Docker within a Docker by using Docker socket has serious security implications. The code inside the container can now run as root on your Docker host. Please be very careful.

### 4. Create a service connection for the Azure DevOps project.

Use your existing Microsoft Azure Subscription, [you can create a free account](https://azure.microsoft.com/en-us/free/) if you don't have any.  
By using Azure CLI create a service principal and configure its access to Azure resources. To retrieve current subscription ID, run:  
```
az account show --query id --output tsv
```
Configure its access to Azure subscription:
```
az ad sp create-for-rbac --name Azure-Pipelines-Immersion-1 --role Owner --scopes /subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```
Cretae a new [Azure Resource Manager service connection](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/connect-to-azure?view=azure-devops#create-an-azure-resource-manager-service-connection-with-an-existing-service-principal) with an existing service principal (azure-pipelines-immersion-1), required name: **DefaultAzureServiceConnection**. Choose **Verify connection** to validate the settings you entered
Install [GitTools](https://marketplace.visualstudio.com/items?itemName=gittools.gittools) for the current Azure DevOps Organization. 
Install [Azure Load Testing](https://marketplace.visualstudio.com/items?itemName=AzloadTest.AzloadTesting) for the current Azure DevOps Organization.
Disable [shallow fetch](https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-checkout?view=azure-pipelines#shallow-fetch) for the Azure Pipeline. It's required for GitTools.

### 5. Run the Azure Pipeline.

Run the Azure Pipeline, wait till it completely deploy the solution and enjoy it.