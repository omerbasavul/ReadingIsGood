using System.ComponentModel.DataAnnotations.Schema;
using Redis.OM.Modeling;

namespace ReadingIsGood.BuildingBlocks.Redis.Base;

public class BaseRedis
{
    [RedisIdField] [NotMapped] public Ulid RedisId { get; set; }
}