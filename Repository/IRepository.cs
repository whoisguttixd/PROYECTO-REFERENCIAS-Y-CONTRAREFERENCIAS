namespace PROYECTO_REFERENCIAS.Repository
{
    /// <summary>
    /// Patrón Repository: interfaz genérica que desacopla la capa de acceso a datos
    /// de EF Core. Los controladores usan IRepository<T> en lugar de DbContext directo.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
        Task<List<T>> ListAsync();
        Task<List<T>> ListAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    }
}
