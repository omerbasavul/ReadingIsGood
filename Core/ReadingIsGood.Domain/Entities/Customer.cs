using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ReadingIsGood.Domain.Entities;

[Table("Customers")]
public class Customer : IdentityUser<Guid>
{

    [StringLength(50)]
    public string Firstname { get; set; }

    [StringLength(50)]
    public string Lastname { get; set; }

    [StringLength(5)]
    public string CountryCode { get; set; }

    [StringLength(200)]
    public string Address { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}