using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteRabbitTelegram.Entity;

public class ClannItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ClannId { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal EarnedCoins { get; set; }
    protected ClannItem()
    {

    }

    public ClannItem(Guid userId, Guid clannId, decimal coins) : this()    
    {
        UserId = userId;
        ClannId = clannId;
        EarnedCoins = coins;
    }
}
