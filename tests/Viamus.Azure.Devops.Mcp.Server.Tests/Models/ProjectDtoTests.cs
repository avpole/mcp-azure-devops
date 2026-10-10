using System.Text.Json;
using Viamus.Azure.Devops.Mcp.Server.Models;

namespace Viamus.Azure.Devops.Mcp.Server.Tests.Models;

public class ProjectDtoTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [Fact]
    public void ProjectDto_ShouldSerializeToJson()
    {
        var dto = new ProjectDto
        {
            Id = "dbd7edc9-4195-437e-801b-4225950da9d6",
            Name = "MyProject",
            Description = "Sample project",
            State = "WellFormed",
            Visibility = "Private",
            Revision = 42,
            Abbreviation = "MyProject",
            Url = "https://dev.azure.com/org/_apis/projects/dbd7edc9-4195-437e-801b-4225950da9d6"
        };

        var json = JsonSerializer.Serialize(dto, JsonOptions);

        Assert.Contains("\"id\":\"dbd7edc9-4195-437e-801b-4225950da9d6\"", json);
        Assert.Contains("\"name\":\"MyProject\"", json);
        Assert.Contains("\"state\":\"WellFormed\"", json);
        Assert.Contains("\"visibility\":\"Private\"", json);
        Assert.Contains("\"revision\":42", json);
    }

    [Fact]
    public void ProjectDto_ShouldDeserializeFromJson()
    {
        var json = """
        {
            "id": "abc-123",
            "name": "TestProject",
            "state": "createPending",
            "visibility": "private",
            "revision": 7
        }
        """;

        var dto = JsonSerializer.Deserialize<ProjectDto>(json, JsonOptions);

        Assert.NotNull(dto);
        Assert.Equal("abc-123", dto.Id);
        Assert.Equal("TestProject", dto.Name);
        Assert.Equal("createPending", dto.State);
        Assert.Equal("private", dto.Visibility);
        Assert.Equal(7, dto.Revision);
    }

    [Fact]
    public void ProjectDto_RecordEquality_ShouldWork()
    {
        var dto1 = new ProjectDto { Id = "abc-123", Name = "project" };
        var dto2 = new ProjectDto { Id = "abc-123", Name = "project" };
        var dto3 = dto1 with { Name = "other" };

        Assert.Equal(dto1, dto2);
        Assert.NotEqual(dto1, dto3);
    }

    [Fact]
    public void ProjectDto_NullableProperties_ShouldAllowNull()
    {
        var dto = new ProjectDto
        {
            Id = null,
            Name = null,
            Description = null,
            State = null,
            Visibility = null,
            Revision = null,
            LastUpdateTime = null
        };

        Assert.Null(dto.Id);
        Assert.Null(dto.Name);
        Assert.Null(dto.Revision);
        Assert.Null(dto.LastUpdateTime);
    }
}
