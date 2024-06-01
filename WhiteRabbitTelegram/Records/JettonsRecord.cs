namespace WhiteRabbitTelegram.Records;

public record JettonsRecord(List<Balance> balances);

public record Balance(string balance, WalletAddress wallet_address, Jetton jetton);

public record WalletAddress(string address, bool is_scam, bool is_wallet);

public record Jetton(string address, string name, string symbol, string decimals, string image, string verification);
