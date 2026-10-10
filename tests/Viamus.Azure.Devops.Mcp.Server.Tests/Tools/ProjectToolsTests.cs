using System.Text.Json;
using Moq;
using Viamus.Azure.Devops.Mcp.Server.Models;
using Viamus.Azure.Devops.Mcp.Server.Services;
using Viamus.Azure.Devops.Mcp.Server.Tools;

namespace Viamus.Azure.Devops.Mcp.Server.Tests.Tools;

public class ProjectToolsTests
{
    private readonly Mock<IAzureDevOpsService> _mockService;
    private readonly ProjectTools _tools;

    public ProjectToolsTests()
    {
        _mockService = new Mock<IAzureDevOpsService>();
        _tools = new ProjectTools(_mockService.Object);
    }

    #region GetProjects Tests

    [Fact]
    public async Task GetProjects_ShouldReturnProjectList()
    {
        var projects = new List<ProjectDto>
        {
            new() { Id = "proj-1", Name = "Project1", State = "WellFormed", Visibility = "Private" },
            new() { Id = "proj-2", Name = "Project2", State = "WellFormed", Visibility = "Public" }
        };

        _mockService
            .Setup(s => s.GetProjectsAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var result = await _tools.GetProjects();

        Assert.Contains("\"count\": 2", result);
        Assert.Contains("Project1", result);
        Assert.Contains("Project2", result);
    }

    [Fact]
    public async Task GetProjects_WithStateFilterAndTop_ShouldPassArgumentsToService()
    {
        var projects = new List<ProjectDto>
        {
            new() { Id = "proj-1", Name = "Project1" }
        };

        _mockService
            .Setup(s => s.GetProjectsAsync("all", 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var result = await _tools.GetProjects("all", 10);

        Assert.Contains("\"count\": 1", result);
        _mockService.Verify(
            s => s.GetProjectsAsync("all", 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProjects_EmptyResult_ShouldReturnZeroCount()
    {
        _mockService
            .Setup(s => s.GetProjectsAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectDto>());

        var result = await _tools.GetProjects("deleted");

        Assert.Contains("\"count\": 0", result);
    }

    #endregion

    #region GetProjectDetails Tests

    [Fact]
    public async Task GetProjectDetails_ShouldReturnDetailsWithCapabilities()
    {
        var details = new ProjectDetailsDto
        {
            Id = "dbd7edc9-4195-437e-801b-4225950da9d6",
            Name = "MyProject",
            Description = "Sample project",
            State = "WellFormed",
            Visibility = "Private",
            DefaultTeam = new ProjectTeamDto
            {
                Id = "team-1",
                Name = "MyProject Team",
                Url = "https://dev.azure.com/org/_apis/teams/team-1"
            },
            Capabilities = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["versioncontrol"] = new Dictionary<string, string>
                {
                    ["sourceControlType"] = "Git",
                    ["gitEnabled"] = "True"
                },
                ["processTemplate"] = new Dictionary<string, string>
                {
                    ["templateName"] = "Agile"
                }
            }
        };

        _mockService
            .Setup(s => s.GetProjectDetailsAsync("MyProject", It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        var result = await _tools.GetProjectDetails("MyProject");

        Assert.Contains("\"name\": \"MyProject\"", result);
        Assert.Contains("capabilities", result);
        Assert.Contains("versioncontrol", result);
        Assert.Contains("processTemplate", result);
        Assert.Contains("MyProject Team", result);
    }

    [Fact]
    public async Task GetProjectDetails_WithBlankProject_ShouldReturnErrorWithoutCallingService()
    {
        var result = await _tools.GetProjectDetails(" ");

        Assert.Contains("error", result);
        Assert.Contains("required", result);
        _mockService.Verify(
            s => s.GetProjectDetailsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetProjectDetails_WhenProjectNotFound_ShouldReturnError()
    {
        _mockService
            .Setup(s => s.GetProjectDetailsAsync("MissingProject", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectDetailsDto?)null);

        var result = await _tools.GetProjectDetails("MissingProject");

        Assert.Contains("error", result);
        Assert.Contains("not found", result);
    }

    #endregion
}
