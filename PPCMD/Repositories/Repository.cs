using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
//using static PPCMD.Repositories.IRepository;

namespace PPCMD.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context { get; set; }
        private DbSet<T> _dbSet { get; set; }

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, QueryOption<T> options)
        {
            // Start building the query from the DbSet (all rows for entity T)
            IQueryable<T> query = _dbSet;

            // Apply filter if provided (e.g. Where = c => c.IsActive)
            if (options.HasWhere)
                query = query.Where(options.Where);

            // Apply ordering if provided (e.g. OrderBy = c => c.Name)
            if (options.HasOrderBy)
                query = query.OrderBy(options.OrderBy);

            // Apply eager loading for related entities (e.g. Includes = "Orders,Address")
            foreach (string include in options.GetIncludes())
                query = query.Include(include);

            // Find the name of the primary key property (e.g. "Id", "CustomerId", "OrderId")
            var key = _context.Model
                              .FindEntityType(typeof(T))?
                              .FindPrimaryKey()?
                              .Properties
                              .FirstOrDefault();

            if (key == null) return null;

            var keyType = key.ClrType;

            if (keyType == typeof(int))
                return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, key.Name) == (int)(object)id);
            else if (keyType == typeof(string))
                return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, key.Name) == id.ToString());
            else
                throw new NotSupportedException("Only int or string PK is supported");

            // If we can’t find it, fall back to "Id"
            string keyName = key?.Name ?? "Id";

            // Finally, get the entity where the PK matches the given id
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, keyName) == id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
