namespace WhiteRabbitTelegram.Command;

public class UserCommands
{
    //Basic commands
    public static string StartCommand = "/start";
    public static string BackCommand = "back";
    public static string BackIntoMainMenu = "BackIntoMainMenu";
    public static string BackIntoPersonalAccountCommand = "BackIntoPersonalAccount";
    //First Sign
    public static string ConnectWalletCommand = "wallet";
    public static string CheckNFTCollectionCommand = "nft_collection";
    public static string CheckWhiteCoinsCommand = "white_coins";
    public static string MainMenuCommand = "main_menu";
    //MainMenu commands
    public static string EarnWBCoinsCommand = "EarnWBCoinsCommand";
    public static string EarnWBCoinsByNotificationCommand = "EarnWBCoinsByNotificationCommand";
    public static string TopUsersCommand = "TopUsersCommand";
    public static string TopUsersForAllTheTime = "TopUsersForAllTheTime";
    public static string PersonalAccountCommand = "PersonalAccountCommand";
    public static string ReferralLinkCommand = "ReferralLinkCommand";
    public static string ChangeWalletAddressCommand = "ChangeWalletAddressCommand";
    public static string ConnectNewWalletAddressCommand = "ConnectNewWalletAddressCommand";
    public static string CreateClannCommand = "CreateClannCommand";
    public static string ConfirmCreateClannCommand = "ConfirmCreateClannCommand";
    public static string InformationAboutClannCommand = "InformationAboutClannCommand";
    public static string AllUsersCommand(int page) => $"AllUsersCommand_{page}";
    public static string CheckSubscribeCommand = "CheckSubscribeCommand";
}

public class BotCommands
{
    //Basic commands
    public static string BackCommand = "Назад";
    public static string StartCommand = "/start";
    //First Sign
    public static string ConnectWalletCommand = "Пожалуйста, введите ваш TON space address 💳\n\nЭто делается для проверки ваших NFT и начисление бонусных токенов.";
    public static string CheckWhiteCoinsCommand = "Проверяем количество ваших WhiteCoin's🫰🏻";
    public static string CheckNFTCollectionCommand = "Проверяем вашу коллекцию NFT для начисления бонусных токенов.💫";
    public static string AgainCheckNftCollectionCommand = "Проверям количество ваших NFT💫";
    public static string MainMenuCommand = "Переводим вас на главное меню.";
    public static string CardMainMenuCommand = "Добывай монеты до релиза игры 🐇\r\nКупи NFT и добывай в 3x быстрее 🚀\r\n\r\nВы в главном меню:";
    //MainMenu commands


}
