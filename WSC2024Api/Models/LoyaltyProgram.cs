using System;
using System.Collections.Generic;

namespace WSC2024Api.Models;

public partial class LoyaltyProgram
{
    public int CustomerId { get; set; }

    public int Points { get; set; }

    public string MembershipTier { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
