namespace Viamus.Azure.Devops.Mcp.Server.Models;

/// <summary>
/// Data transfer object with full details of an Azure DevOps project, including capabilities and the default team.
/// </summary>
public sealed record ProjectDetailsDto
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? State { get; init; }
    public string? Visibility { get; init; }
    public long? Revision { get; init; }
    public string? Abbreviation { get; init; }
    public string? Url { get; init; }
    public DateTime? LastUpdateTime { get; init; }
    public ProjectTeamDto? DefaultTeam { get; init; }
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? Capabilities { get; init; }
}

/// <summary>
/// Data transfer object representing the default team of an Azure DevOps project.
/// </summary>
public sealed record ProjectTeamDto
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Url { get; init; }
}
