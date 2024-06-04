using Org.BouncyCastle.Asn1.Cms;

namespace WhiteRabbitTelegram.Records;

public record NftsRecord(int code, string? msg, NftsData[] data);

public record NftsData(string contract_name, string contract_address, int owns_total, int items_total, Nft[] assets);

public record Nft(string? token_address, string? contract_name, string? contract_address, string? token_id, long? block_number,
    string? minter, string? owner, long? mint_timestamp, string? mint_transaction_hash, decimal? mint_price, string? token_uri,
    string? metadata_json, string? name, string? content_type, string? content_uri, string? image_uri, string? description);



public record NftsItemsRecord(NftItem[] nft_items);

public record NftItem(string? address, long? index, Owner? owner, Collection? collection, bool? verified, Metadata? metadata, string? trust);

public record Owner(string? address, string? name, bool? is_scam, bool? is_wallet);

public record Collection(string? address, string? name, string? description);

public record Metadata(string? image, string? description, string? marketplace, string? name);
