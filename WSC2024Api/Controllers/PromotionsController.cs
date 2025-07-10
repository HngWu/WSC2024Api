using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSC2024Api.Models;
using WSC2024App.Dtos;

namespace WSC2024Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        BelleCroissantLyonnaisContext _context = new BelleCroissantLyonnaisContext();


        // GET: api/Promotions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotions()
        {
            return await _context.Promotions.ToListAsync();
        }

        // GET: api/Promotions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Promotion>> GetPromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);

            if (promotion == null)
            {
                return NotFound();
            }

            return promotion;
        }

        // PUT: api/Promotions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotion(int id, PromotionDto promotionDto)
        {
            var promotionToUpdate = _context.Promotions.FirstOrDefault(x => x.PromotionId == id);

            if (id != promotionToUpdate.PromotionId)
            {
                return BadRequest();
            }

            // Replace the problematic block in PutPromotion with property assignments
            promotionToUpdate.PromotionId = promotionDto.PromotionId;
            promotionToUpdate.PromotionName = promotionDto.PromotionName;
            promotionToUpdate.DiscountType = promotionDto.DiscountType;
            promotionToUpdate.DiscountValue = promotionDto.DiscountValue;
            promotionToUpdate.ApplicableProducts = promotionDto.ApplicableProducts;
            promotionToUpdate.StartDate = DateOnly.FromDateTime(promotionDto.StartDate);
            promotionToUpdate.EndDate = DateOnly.FromDateTime(promotionDto.EndDate);
            promotionToUpdate.MinimumOrderValue = promotionDto.MinimumOrderValue;
            promotionToUpdate.Priority = promotionDto.Priority;
            promotionToUpdate.QuantityBasedRules = promotionDto.quantityBasedRuleDetail;

            _context.Update(promotionToUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PromotionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Promotions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Promotion>> PostPromotion(PromotionDto promotionDto)
        {
            var promotion = new Promotion
            {
                PromotionId = promotionDto.PromotionId,
                PromotionName = promotionDto.PromotionName,
                DiscountType = promotionDto.DiscountType,
                DiscountValue = promotionDto.DiscountValue,
                ApplicableProducts = promotionDto.ApplicableProducts,
                StartDate = DateOnly.FromDateTime(promotionDto.StartDate),
                EndDate = DateOnly.FromDateTime(promotionDto.EndDate),
                MinimumOrderValue = promotionDto.MinimumOrderValue,
                Priority = promotionDto.Priority,
                QuantityBasedRules = promotionDto.quantityBasedRuleDetail
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            // Return the PromotionId in the response
            return CreatedAtAction("GetPromotion", new { id = promotion.PromotionId }, new { promotion.PromotionId });
        }

        // DELETE: api/Promotions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.PromotionId == id);
        }
    }
}
