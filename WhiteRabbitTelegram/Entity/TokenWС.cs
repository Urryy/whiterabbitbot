using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteRabbitTelegram.Entity;

public class TokenWС
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Wallet { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal Tokens { get; set; }
    public DateTime DateEarn { get; set; } = DateTime.Now;

    protected TokenWС()
    {

    }

    public TokenWС(decimal tokens, string wallet) : this()
    {
        Tokens = tokens;
        Wallet = wallet;
    }
}
