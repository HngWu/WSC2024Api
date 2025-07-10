using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSC2024App.Dtos
{
    public class PromotionDto
    {
        public int PromotionId { get; set; }

        public string PromotionName { get; set; }

        public string DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public string ApplicableProducts { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? MinimumOrderValue { get; set; }

        public int Priority { get; set; }

        public string quantityBasedRuleDetail { get; set; }
    }
}
