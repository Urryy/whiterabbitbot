namespace WhiteRabbitTelegram.Entity;

public class NFT
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string? TokenAddress { get; set; }
    public string? ContractName { get; set; }
    public string? ContractAddress { get; set; }
    public string? TokenId { get; set; }
    public long? BlockNumber { get; set; }
    public string? Minter { get; set; }
    public string? Owner { get; set; }
    public long? MintTimestamp { get; set; }
    public string? MintTransactionHash { get; set; }
    public decimal? MintPrice { get; set; }
    public string? TokenUri { get; set; }
    public string? MetadataJson { get; set; }
    public string? Name { get; set; }
    public string? ContentType { get; set; }
    public string? ContentUri { get; set; }
    public string? ImageUri { get; set; }
    public string? Description { get; set; }

    protected NFT()
    {

    }
    public NFT(string? tokenAddress, string? contractName, string? contractAddress, string? tokenId, long? blockNumber, string? minter,
        string? owner, long? mintTimestamp, string? mintTransactionHash, decimal? mintPrice, string? tokenUri, string? metadataJson, string? name,
        string? contentType, string? contentUri, string? imageUri, string? description, Guid userId) : this()
    {
        TokenAddress = tokenAddress;
        ContractName = contractName;
        ContractAddress = contractAddress;
        TokenId = tokenId;
        BlockNumber = blockNumber;
        Minter = minter;
        Owner = owner;
        MintTimestamp = mintTimestamp;
        MintTransactionHash = mintTransactionHash;
        MintPrice = mintPrice;
        TokenUri = tokenUri;
        MetadataJson = metadataJson;
        Name = name;
        ContentType = contentType;
        ContentUri = contentUri;
        ImageUri = imageUri;
        Description = description;
        UserId = userId;
    }
}
