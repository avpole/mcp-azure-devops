using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Viamus.Azure.Devops.Mcp.Server.Services;

namespace Viamus.Azure.Devops.Mcp.Server.Tools;

/// <summary>
/// MCP tools for Azure DevOps project (organization/collection) operations.
/// </summary>
[McpServerToolType]
public sealed class ProjectTools
{
    private readonly IAzureDevOpsService _azureDevOpsService;
    private readonly IAzureDevOpsOrganizationContextAccessor _organizationContextAccessor;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ProjectTools(IAzureDevOpsService azureDevOpsService)
        : this(azureDevOpsService, new AzureDevOpsOrganizationContextAccessor())
    {
    }

    public ProjectTools(
        IAzureDevOpsService azureDevOpsService,
        IAzureDevOpsOrganizationContextAccessor organizationContextAccessor)
    {
        _azureDevOpsService = azureDevOpsService;
        _organizationContextAccessor = organizationContextAccessor;
    }

    [McpServerTool(Name = "get_projects")]
    [Description("Gets all projects in an Azure DevOps organization or collection. Returns project details including name, ID, description, state, and visibility. By default returns well-formed projects; use stateFilter 'all' to also include projects pending creation or deletion.")]
    public async Task<string> GetProjects(
        [Description("Project state filter: 'wellFormed' (default), 'all', 'new', 'createPending', or 'deleted'")] string? stateFilter = null,
        [Description("Maximum number of projects to return (optional; all matching projects are returned when omitted)")] int? top = null,
        [Description("The Azure DevOps organization alias or URL (optional if default organization is configured)")] string? organization = null,
        CancellationToken cancellationToken = default)
    {
        using var organizationScope = _organizationContextAccessor.Use(organization);
        var projects = await _azureDevOpsService.GetProjectsAsync(stateFilter, top, cancellationToken);
        return JsonSerializer.Serialize(new { count = projects.Count, projects }, JsonOptions);
    }

    [McpServerTool(Name = "get_project_details")]
    [Description("Gets details of a specific Azure DevOps project by name or ID, including capabilities (source control type, process template) and the default team.")]
    public async Task<string> GetProjectDetails(
        [Description("The project name or ID")] string project,
        [Description("The Azure DevOps organization alias or URL (optional if default organization is configured)")] string? organization = null,
        CancellationToken cancellationToken = default)
    {
        using var organizationScope = _organizationContextAccessor.Use(organization);
        if (string.IsNullOrWhiteSpace(project))
        {
            return JsonSerializer.Serialize(new { error = "Project name or ID is required" }, JsonOptions);
        }

        var details = await _azureDevOpsService.GetProjectDetailsAsync(project, cancellationToken);

        if (details is null)
        {
            return JsonSerializer.Serialize(new { error = $"Project '{project}' not found" }, JsonOptions);
        }

        return JsonSerializer.Serialize(details, JsonOptions);
    }
}
