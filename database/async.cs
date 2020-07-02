using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace CloudBase {
	partial class Query {
		public Task<DbQueryResponse> GetAsync()
		{
			var dest = this.Get();
			return Task.FromResult(dest);
		}

		public Task<DbCountResponse> CountAsync() {
			var dest= this.Count();
			return Task.FromResult(dest);
		}

		public Task<DbUpdateResponse> UpdateAsync(object data) {
			var dest= this.Update(data);
			return Task.FromResult(dest);
		}

		public Task<DbRemoveResponse> RemoveAsync() {
			var dest= this.Remove();
			return Task.FromResult(dest);
		}
	};

	partial class Collection {
		public Task<DbCreateResponse> AddAsync(object data) {
			var dest = this.Add(data);
			return Task.FromResult(dest);
		}
	}

	partial class Document {
		public Task<DbQueryResponse> GetAsync() {
			var dest = this.Get();
			return Task.FromResult(dest);
		}

		public Task<DbCreateResponse> CreateAsync(object data) {
			var dest = this.Create(data);
			return Task.FromResult(dest);
		}

		public Task<DbUpdateResponse> SetAsync(object data) {
			var dest = this.Set(data);
			return Task.FromResult(dest);
		}

		public Task<DbUpdateResponse> UpdateAsync(object data) {
			var dest = this.Update(data);
			return Task.FromResult(dest);
		}

		public Task<DbRemoveResponse> RemoveAsync() {
			var dest = this.Remove();
			return Task.FromResult(dest);
		}
	}
}