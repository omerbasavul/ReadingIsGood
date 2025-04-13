using Newtonsoft.Json;
using ReadingIsGood.BuildingBlocks.Redis.Base;
using Redis.OM.Modeling;

namespace ReadingIsGood.Application.Models;

[Document(StorageType = StorageType.Hash, IndexName = "Customer", Prefixes = ["Customer"])]
public class CustomerModel : BaseRedis
{
    [Indexed] public Guid Id { get; set; }

    [Indexed] public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }

    [JsonIgnore]
    public IList<string> Roles { get; set; }

    [Indexed] public bool IsActive { get; set; }
    [Indexed] public bool IsDeleted { get; set; }
}