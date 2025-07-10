using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSC2024Api.Dtos;
using WSC2024Api.Models;

namespace WSC2024Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyProgramsController : ControllerBase
    {
        private readonly BelleCroissantLyonnaisContext _context;

        public LoyaltyProgramsController(BelleCroissantLyonnaisContext context)
        {
            _context = context;
        }


        // Add at the end of the LoyaltyProgramsController class

        public class PointsBreakdownDto
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal OrderAmount { get; set; }
            public int PointsEarned { get; set; }
            public bool PromotionBonus { get; set; }
        }

        public class RecalculatePointsResultDto
        {
            public int CustomerId { get; set; }
            public string MembershipTier { get; set; }
            public List<PointsBreakdownDto> Orders { get; set; } = new();
            public bool AnniversaryBonus { get; set; }
            public int TotalPoints { get; set; }
        }

        // GET: api/LoyaltyPrograms/{customerId}/RecalculatePoints
        [HttpGet("{customerId}/RecalculatePoints")]
        public async Task<ActionResult<RecalculatePointsResultDto>> RecalculatePoints(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.LoyaltyProgram)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null || customer.LoyaltyProgram == null)
                return NotFound();

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            var promotions = await _context.Promotions.ToListAsync();

            int pointsPer10Euros = customer.MembershipStatus switch
            {
                "Gold" => 15,
                "Silver" => 12,
                _ => 10
            };

            var breakdown = new List<PointsBreakdownDto>();
            int totalPoints = 0;
            int promotionBonusPoints = 0;

            foreach (var order in orders)
            {
                int basePoints = (int)Math.Floor(order.TotalAmount / 10) * pointsPer10Euros;
                bool inPromotion = promotions.Any(p =>
                    order.PromotionId == p.PromotionId ||
                    (order.OrderDate.Date >= p.StartDate.ToDateTime(TimeOnly.MinValue).Date &&
                     order.OrderDate.Date <= p.EndDate.ToDateTime(TimeOnly.MaxValue).Date)
                );
                int bonus = inPromotion ? 5 : 0;
                if (inPromotion) promotionBonusPoints += 5;

                breakdown.Add(new PointsBreakdownDto
                {
                    OrderId = order.TransactionId,
                    OrderDate = order.OrderDate,
                    OrderAmount = order.TotalAmount,
                    PointsEarned = basePoints + bonus,
                    PromotionBonus = inPromotion
                });

                totalPoints += basePoints + bonus;
            }

            // Anniversary bonus
            bool anniversary = customer.JoinDate.HasValue &&
                customer.JoinDate.Value.Month == DateTime.Today.Month &&
                customer.JoinDate.Value.Day == DateTime.Today.Day;

            if (anniversary)
                totalPoints += 25;

            var result = new RecalculatePointsResultDto
            {
                CustomerId = customerId,
                MembershipTier = customer.LoyaltyProgram.MembershipTier,
                Orders = breakdown,
                AnniversaryBonus = anniversary,
                TotalPoints = totalPoints
            };

            return Ok(result);
        }

        // POST: api/LoyaltyPrograms/{customerId}/ConfirmRecalculatedPoints
        [HttpPost("{customerId}/ConfirmRecalculatedPoints")]
        public async Task<IActionResult> ConfirmRecalculatedPoints(int customerId, [FromBody] int newPoints)
        {
            var loyaltyProgram = await _context.LoyaltyPrograms.FindAsync(customerId);
            if (loyaltyProgram == null)
                return NotFound();

            loyaltyProgram.Points = newPoints;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/LoyaltyPrograms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoyaltyProgramDto>>> GetLoyaltyPrograms()
        {
            var loyaltyPrograms = await _context.LoyaltyPrograms
                .Select(lp => new LoyaltyProgramDto
                {
                    CustomerId = lp.CustomerId,
                    Points = lp.Points,
                    MembershipTier = lp.MembershipTier
                })
                .ToListAsync();

            return Ok(loyaltyPrograms);
        }

        // GET: api/LoyaltyPrograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoyaltyProgramDto>> GetLoyaltyProgram(int id)
        {
            var loyaltyProgram = await _context.LoyaltyPrograms.FindAsync(id);

            if (loyaltyProgram == null)
            {
                return NotFound();
            }

            var loyaltyProgramDto = new LoyaltyProgramDto
            {
                CustomerId = loyaltyProgram.CustomerId,
                Points = loyaltyProgram.Points,
                MembershipTier = loyaltyProgram.MembershipTier
            };


            return loyaltyProgramDto;
        }

        // PUT: api/LoyaltyPrograms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoyaltyProgram(int id, LoyaltyProgramDto loyaltyProgram)
        {
            if (id != loyaltyProgram.CustomerId)
            {
                return BadRequest();
            }

            // Replace this line in the PutLoyaltyProgram method:

            // With the following code to map the DTO to the entity and update it properly:
            var existingLoyaltyProgram = await _context.LoyaltyPrograms.FindAsync(id);
            if (existingLoyaltyProgram == null)
            {
                return NotFound();
            }

            existingLoyaltyProgram.Points = loyaltyProgram.Points;
            existingLoyaltyProgram.MembershipTier = loyaltyProgram.MembershipTier;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoyaltyProgramExists(id))
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

        // POST: api/LoyaltyPrograms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoyaltyProgram>> PostLoyaltyProgram(LoyaltyProgram loyaltyProgram)
        {
            _context.LoyaltyPrograms.Add(loyaltyProgram);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LoyaltyProgramExists(loyaltyProgram.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLoyaltyProgram", new { id = loyaltyProgram.CustomerId }, loyaltyProgram);
        }

        // DELETE: api/LoyaltyPrograms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoyaltyProgram(int id)
        {
            var loyaltyProgram = await _context.LoyaltyPrograms.FindAsync(id);
            if (loyaltyProgram == null)
            {
                return NotFound();
            }

            _context.LoyaltyPrograms.Remove(loyaltyProgram);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoyaltyProgramExists(int id)
        {
            return _context.LoyaltyPrograms.Any(e => e.CustomerId == id);
        }
    }
}
