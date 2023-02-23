using Microsoft.EntityFrameworkCore;
using StudioMgn.Data;

namespace StudioMgn.Services
{
    public class Service<T, Tself> where T : class
    {
        public ILogger<Tself> Logger { get; set; }
        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; set; }

        public Service(ILogger<Tself> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            this.Logger = logger;
            this.DbContextFactory = dbContextFactory;
        }

        public List<T> LocalCollection { get; set; } = new();
        public List<T> MyLocalCollection { get; set; } = new();
        public List<T> RemoteCollection { get; set; } = new();

        public virtual async Task GetLocalCollectionAsync()
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                var list = ctx.Set<T>().AsNoTracking();
                LocalCollection = list.ToList();
            }
            Logger.LogInformation($"=== Коллекция {typeof(T)} получена, всего {LocalCollection.Count} {typeof(T)}");

        }
        public virtual async Task AddAsync(T item)
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                ctx.Set<T>().Add(item);
                await ctx.SaveChangesAsync();
                Logger.LogInformation($"=== Объект {nameof(item)} добавлен в базу данных");
            }
        }
        public virtual async Task UpdateAsync(T item)
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                ctx.Attach(item).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
                Logger.LogInformation($"=== Объект {nameof(item)} обновлён в базе данных");
            }
        }
        public virtual async Task RemoveAsync(T item)
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                ctx.Attach(item).State = EntityState.Deleted;
                await ctx.SaveChangesAsync();
                Logger.LogInformation($"=== Объект {nameof(item)} удалён из базы данных");
            }
        }
        public virtual async Task AddRange(IEnumerable<T> collection)
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                foreach (var item in collection)
                {
                    ctx.Set<T>().Add(item);
                }
                await ctx.SaveChangesAsync();
                Logger.LogInformation($"=== В базу данных добавлена коллекция из {collection.Count()} {typeof(T)}");
            }
        }
        public virtual async Task RemoveRange(IEnumerable<T> collection)
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                foreach (var item in collection)
                {
                    ctx.Attach(item).State = EntityState.Deleted;
                }
                await ctx.SaveChangesAsync();
                Logger.LogInformation($"=== Из базы данных удалена коллекция из {collection.Count()} {typeof(T)}");
            }
        }
        public virtual async Task UpdateRange(IEnumerable<T> collection)
        {
            using (var ctx = await DbContextFactory.CreateDbContextAsync())
            {
                ctx.UpdateRange(collection);
                await ctx.SaveChangesAsync();
                Logger.LogInformation($"=== Коллекция {typeof(T).ToString()} обновлена в базе данных");
            }
        }

    }
}
