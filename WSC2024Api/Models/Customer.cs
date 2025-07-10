using System;
using System.Collections.Generic;

namespace WSC2024Api.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public string? PostalCode { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string MembershipStatus { get; set; } = null!;

    public DateOnly? JoinDate { get; set; }

    public DateOnly? LastPurchaseDate { get; set; }

    public decimal TotalSpending { get; set; }

    public decimal? AverageOrderValue { get; set; }

    public string? Frequency { get; set; }

    public string? PreferredCategory { get; set; }

    public bool? Churned { get; set; }

    public virtual LoyaltyProgram? LoyaltyProgram { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
