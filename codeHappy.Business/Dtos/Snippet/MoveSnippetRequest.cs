using System.Text.Json.Serialization;

namespace codeHappy.Business.Dtos.Snippet;

public sealed class MoveSnippetRequest
{
    private Guid? _spaceId;
    private Guid? _groupId;

    public Guid? SpaceId
    {
        get => _spaceId;
        init
        {
            _spaceId = value;
            HasSpaceId = true;
        }
    }

    public Guid? GroupId
    {
        get => _groupId;
        init
        {
            _groupId = value;
            HasGroupId = true;
        }
    }

    [JsonIgnore]
    public bool HasSpaceId { get; private set; }

    [JsonIgnore]
    public bool HasGroupId { get; private set; }

    [JsonIgnore]
    public bool HasLocationField => HasSpaceId || HasGroupId;
}
