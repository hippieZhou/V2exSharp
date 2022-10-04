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
    public async Task should_not_null_when_get_nodesShow()
    {
        var node = await _apiClient.GetNodesShowAsync("swift");
        Assert.NotNull(node);
    }

    [Fact]
    public async Task should_not_null_when_get_memberShow()
    {
        var member = await _apiClient.GetMemberShowAsync("hippiezhou");
        Assert.Equal("hippiezhou", member.Username, true);
    }

    [Fact]
    public async Task should_not_empty_when_get_topics_by_name()
    {
        var response = await _apiClient.GetTopicsAsync("swift");
        Assert.True(response.Success);
    }

    [Fact]
    public async Task should_not_empty_when_get_topic_by_id()
    {
        var response = await _apiClient.GetTopicAsync(870607);
        Assert.True(response.Success);
    }
}