using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SFA.DAS.EmployerAccounts.TestCommon
{
    public class DbSetStub<T> : DbSet<T>, IDbAsyncEnumerable<T>, IQueryable<T> where T : class
    {
        public Expression Expression => _data.Expression;
        public Type ElementType => _data.ElementType;
        public override IEntityType EntityType { get; }
        public IQueryProvider Provider => new DbAsyncQueryProviderStub<T>(_data.Provider);

        private readonly IQueryable<T> _data;
        
        public DbSetStub(params T[] data) : this(data.AsEnumerable()) { }

        public DbSetStub(IEnumerable<T> data)
        {
            _data = data.AsQueryable();
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new DbAsyncEnumeratorStub<T>(_data.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}