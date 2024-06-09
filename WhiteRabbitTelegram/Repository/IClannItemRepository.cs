namespace WhiteRabbitTelegram.Repository;

public interface IClannItemRepository
{
    Task<IQueryable<Entity.ClannItem>> GetAllClannItems();
    Task AddRangeClannItem(List<Entity.ClannItem> ClannItems);
    Task AddClannItem(Entity.ClannItem ClannItem);
    Task RemoveRangeClannItems(List<Entity.ClannItem> ClannItems);
    Task RemoveClannItem(Entity.ClannItem ClannItem);
    Task UpdateClannItem(Entity.ClannItem ClannItem);
    Task<Entity.ClannItem> GetClannItemByUserId(Guid id);
    Task<Entity.ClannItem> GetClannItemByClannId(Guid id);
}
