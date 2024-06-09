namespace WhiteRabbitTelegram.Entity;

public class ClannItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ClannId { get; set; }
    protected ClannItem()
    {

    }

    public ClannItem(Guid userId, Guid clannId) : this()    
    {
        UserId = userId;
        ClannId = clannId;
    }
}
