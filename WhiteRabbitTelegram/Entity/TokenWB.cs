namespace WhiteRabbitTelegram.Entity;

public class TokenWB
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public decimal Tokens { get; set; }
    public DateTime DateEarn { get; set; } = DateTime.UtcNow;

    protected TokenWB()
    {

    }

    public TokenWB(decimal tokens, Guid userId) : this()
    {
        Tokens = tokens;
        UserId = userId;
    }
}
