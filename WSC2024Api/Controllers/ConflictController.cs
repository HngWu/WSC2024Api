using Microsoft.AspNetCore.Mvc;
using WSC2024Api.Models;

namespace WSC2024Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConflictController : Controller
    {

        BelleCroissantLyonnaisContext context = new BelleCroissantLyonnaisContext();

        [HttpGet("{id}")]
        public IActionResult GetConflicts(int id)
        {
            var promotionToCheck = context.Promotions.FirstOrDefault(x => x.PromotionId == id);

            if (promotionToCheck == null)
                return NotFound();

            var productNameList = (promotionToCheck.ApplicableProducts ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            var overlappingPromotions = context.Promotions
                .Where(x => x.PromotionId != id
                    && x.Priority == promotionToCheck.Priority
                    && x.StartDate <= promotionToCheck.EndDate
                    && x.EndDate >= promotionToCheck.StartDate
                    && !string.IsNullOrEmpty(x.ApplicableProducts)
                )
                .AsEnumerable()
                .Where(x =>
                    x.ApplicableProducts!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .Any(p => productNameList.Contains(p))
                )
                .Select(x => new WSC2024App.Dtos.PromotionDto
                {
                    PromotionId = x.PromotionId,
                    PromotionName = x.PromotionName,
                    DiscountType = x.DiscountType,
                    DiscountValue = x.DiscountValue,
                    ApplicableProducts = x.ApplicableProducts ?? string.Empty,
                    StartDate = new DateTime(x.StartDate.Year, x.StartDate.Month, x.StartDate.Day),
                    EndDate = new DateTime(x.EndDate.Year, x.EndDate.Month, x.EndDate.Day),
                    MinimumOrderValue = x.MinimumOrderValue,
                    Priority = x.Priority,
                    quantityBasedRuleDetail = x.QuantityBasedRules ?? string.Empty
                })
                .ToList();

            return Ok(overlappingPromotions);
        }
    }
}
