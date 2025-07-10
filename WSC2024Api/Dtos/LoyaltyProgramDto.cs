namespace WSC2024Api.Dtos
{
    public class LoyaltyProgramDto
    {
        public int CustomerId { get; set; }

        public int Points { get; set; }

        public string MembershipTier { get; set; } = null!;
    }
}
