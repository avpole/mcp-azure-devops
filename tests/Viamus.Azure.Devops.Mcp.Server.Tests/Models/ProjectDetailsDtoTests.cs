using System.Text.Json;
using Viamus.Azure.Devops.Mcp.Server.Models;

namespace Viamus.Azure.Devops.Mcp.Server.Tests.Models;

public class ProjectDetailsDtoTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void ProjectDetailsDto_ShouldSerializeCapabilitiesAndDefaultTeam()
    {
        var dto = new ProjectDetailsDto
        {
            Id = "dbd7edc9-4195-437e-801b-4225950da9d6",
            Name = "LoadTesting",
            State = "WellFormed",
            Visibility = "Private",
            DefaultTeam = new ProjectTeamDto
            {
                Id = "team-1",
                Name = "LoadTesting Team",
                Url = "https://tfs/_apis/projects/dbd7edc9/teams/team-1"
            },
            Capabilities = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["versioncontrol"] = new Dictionary<string, string>
                {
                    ["sourceControlType"] = "Git",
                    ["gitEnabled"] = "True",
                    ["tfvcEnabled"] = "False"
                },
                ["processTemplate"] = new Dictionary<string, string>
                {
                    ["templateName"] = "Agile"
                }
            }
        };

        var json = JsonSerializer.Serialize(dto, JsonOptions);

        Assert.Contains("\"defaultTeam\"", json);
        Assert.Contains("\"name\":\"LoadTesting Team\"", json);
        Assert.Contains("\"capabilities\":{\"versioncontrol\":{", json);
        Assert.Contains("\"sourceControlType\":\"Git\"", json);
        Assert.Contains("\"processTemplate\":{\"templateName\":\"Agile\"}", json);
    }

    [Fact]
    public void ProjectDetailsDto_ShouldDeserializeFromJson()
    {
        var json = """
        {
            "id": "abc-123",
            "name": "Project1",
            "defaultTeam": { "id": "t1", "name": "Project1 Team" },
            "capabilities": { "versioncontrol": { "sourceControlType": "Tfvc" } }
        }
        """;

        var dto = JsonSerializer.Deserialize<ProjectDetailsDto>(json, JsonOptions);

        Assert.NotNull(dto);
        Assert.Equal("abc-123", dto.Id);
        Assert.Equal("Project1 Team", dto.DefaultTeam?.Name);
        Assert.NotNull(dto.Capabilities);
        Assert.Equal("Tfvc", dto.Capabilities["versioncontrol"]["sourceControlType"]);
    }

    [Fact]
    public void ProjectDetailsDto_WithoutCapabilities_ShouldSerializeNulls()
    {
        var dto = new ProjectDetailsDto { Id = "abc-123", Name = "Project1" };

        var json = JsonSerializer.Serialize(dto, JsonOptions);

        Assert.Contains("\"defaultTeam\":null", json);
        Assert.Contains("\"capabilities\":null", json);
    }

    [Fact]
    public void ProjectDetailsDto_RecordEquality_ShouldWork()
    {
        var capabilities = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            ["versioncontrol"] = new Dictionary<string, string> { ["sourceControlType"] = "Git" }
        };

        var dto1 = new ProjectDetailsDto { Id = "abc", Capabilities = capabilities };
        var dto2 = new ProjectDetailsDto { Id = "abc", Capabilities = capabilities };
        var dto3 = dto1 with { Name = "other" };

        Assert.Equal(dto1, dto2);
        Assert.NotEqual(dto1, dto3);
    }
}
