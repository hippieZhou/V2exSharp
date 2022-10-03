using V2exSharp.Client;

namespace V2exSharp.ApiTest;

public class V2exAPIFacts : BaseApiTestFixture
{
    private readonly IV2exApiClient _apiClient;

    public V2exAPIFacts()
    {
        _apiClient = GetService<IV2exApiClient>();
    }

    [Fact]
    public async Task should_not_empty_when_get_nodesList()
    {
        var items = await _apiClient.GetNodeListAsync();
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task should_not_empty_when_get_latestTopics()
    {
        var items = await _apiClient.GetLatestTopicsAsync();
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task should_not_empty_when_get_hotTopics()
    {
        var items = await _apiClient.GetHotTopicsAsync();
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task should_not_empty_when_getnodesShow()
    {
        var node = await _apiClient.GetNodesShowAsync("swift");
        Assert.NotNull(node);
    }
}