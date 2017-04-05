using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using ThreeLD.DB.Models;

namespace ThreeLD.DB.Repositories
{
	public class DbRepository<TEntity> : IRepository<TEntity>
		where TEntity : EntityBase, new()
	{
		private bool disposedValue = false;
		private AppDbContext context;
		private DbSet<TEntity> table;

		public DbRepository(AppDbContext context)
		{
			this.context = context;
			this.table = context.Set<TEntity>();
		}

		public void Add(TEntity entity)
		{
			this.table.Add(entity);
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			this.table.AddRange(entities);
		}
		
		public void Update(TEntity entity)
		{
			this.context.Entry(entity).State = EntityState.Modified;
		}

		public void Delete(int id)
		{
			this.context.Entry(new TEntity { Id = id }).State = EntityState.Deleted;
		}

		public void Delete(TEntity entity)
		{
			this.context.Entry(entity).State = EntityState.Deleted;
		}

		public IQueryable<TEntity> GetAll() => this.table;

		public TEntity GetById(int id) => this.table.Find(id);

		public int Save() => this.context.SaveChanges();

		public Task<int> SaveAsync() => this.context.SaveChangesAsync();

		public void Dispose()
		{
			this.Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.Save();
				}

				this.disposedValue = true;
			}
		}

	}
}
