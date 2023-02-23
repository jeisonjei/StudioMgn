using Microsoft.EntityFrameworkCore;
using StudioMgn.Data;

namespace StudioMgn.Services
{
    public interface IService<T, TSelf> where TSelf : IService<T, TSelf>
    {
        public Task GetLocalCollectionAsync();
        public Task GetRemoteCollectionAsync();

        public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; set; }
        public ILogger<TSelf> Logger { get; }
        public List<T> LocalCollection { get; set; }
        public List<T> RemoteCollection { get; set; }

        public Task AddAsync(T item);

        public Task UpdateAsync(T item);

        public Task DeleteAsync(T item);


    }
}
