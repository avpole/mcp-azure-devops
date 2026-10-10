namespace Viamus.Azure.Devops.Mcp.Server.Models;

/// <summary>
/// Data transfer object representing an Azure DevOps project in an organization or collection.
/// </summary>
public sealed record ProjectDto
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
}
