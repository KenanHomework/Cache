using Common.Enums;

namespace WolfCache.ConnectionsModels;

public sealed class Response
{

    public ResultStatus Result { get; set; }

    public string? ResponseData { get; set; }

    public string? Message { get; set; }

}
