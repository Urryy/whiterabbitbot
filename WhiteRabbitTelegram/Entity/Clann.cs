namespace WhiteRabbitTelegram.Entity;

public class Clann
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public string RefLink { get; set; }
    protected Clann()
    {

    }

    public Clann(Guid ownerId, string refLink) : this()
    {
        OwnerId = ownerId;
        RefLink = refLink;
    }
}
