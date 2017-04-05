using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThreeLD.DB.Repositories
{
	public interface IRepository<TEntity> : IDisposable
	{
		void Add(TEntity entity);
		void AddRange(IEnumerable<TEntity> entities);

		void Update(TEntity entity);

		void Delete(int id);
		void Delete(TEntity entity);

		IQueryable<TEntity> GetAll();
		TEntity GetById(int id);

		int Save();
		Task<int> SaveAsync();
	}
}
