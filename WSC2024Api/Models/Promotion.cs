using System;
using System.Collections.Generic;

namespace WSC2024Api.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string PromotionName { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public string? ApplicableProducts { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal? MinimumOrderValue { get; set; }

    public int Priority { get; set; }

    public string? QuantityBasedRules { get; set; }

    public virtual ICollection<QuantityBasedRuleDetail> QuantityBasedRuleDetails { get; set; } = new List<QuantityBasedRuleDetail>();
}
