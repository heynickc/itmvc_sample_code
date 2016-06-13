using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;

namespace ContosoUniversity.Tests {
    public class FakeSchoolContext : ISchoolContext {

        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<Person> People { get; set; }

        public FakeSchoolContext() {
            Courses = new TestDbSet<Course>();
            Departments = new TestDbSet<Department>();
            Enrollments = new TestEnrollmentDbSet();
            Instructors = new TestDbSet<Instructor>();
            Students = new TestDbSet<Student>();
            OfficeAssignments = new TestDbSet<OfficeAssignment>();
            People = new TestDbSet<Person>();
        }

        public int SaveChangesCount { get; private set; }
        public int SaveChanges() {
            this.SaveChangesCount++;
            return 1;
        }

        public Task<int> SaveChangesAsync() {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() {

        }
    }

    public class TestEnrollmentDbSet : TestDbSet<Enrollment> {
        public override Enrollment Find(params object[] keyValues) {
            var id = (int)keyValues.Single();
            return this.SingleOrDefault(b => b.EnrollmentID == id);
        }
    }

    public class TestDbSet<TEntity> : DbSet<TEntity>, IQueryable, IEnumerable<TEntity>, IDbAsyncEnumerable<TEntity>
            where TEntity : class {
        ObservableCollection<TEntity> _data;
        IQueryable _query;

        public TestDbSet() {
            _data = new ObservableCollection<TEntity>();
            _query = _data.AsQueryable();
        }

        public override TEntity Add(TEntity item) {
            _data.Add(item);
            return item;
        }

        public override TEntity Remove(TEntity item) {
            _data.Remove(item);
            return item;
        }

        public override TEntity Attach(TEntity item) {
            _data.Add(item);
            return item;
        }

        public override TEntity Create() {
            return Activator.CreateInstance<TEntity>();
        }

        public override TDerivedEntity Create<TDerivedEntity>() {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local {
            get { return _data; }
        }

        Type IQueryable.ElementType {
            get { return _query.ElementType; }
        }

        Expression IQueryable.Expression {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider {
            get { return new TestDbAsyncQueryProvider<TEntity>(_query.Provider); }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _data.GetEnumerator();
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() {
            return _data.GetEnumerator();
        }

        IDbAsyncEnumerator<TEntity> IDbAsyncEnumerable<TEntity>.GetAsyncEnumerator() {
            return new TestDbAsyncEnumerator<TEntity>(_data.GetEnumerator());
        }
    }

    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider {
        private readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner) {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression) {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression) {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression) {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T> {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression) { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator() {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider {
            get { return new TestDbAsyncQueryProvider<T>(this); }
        }
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T> {
        private readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner) {
            _inner = inner;
        }

        public void Dispose() {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken) {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current {
            get { return _inner.Current; }
        }

        object IDbAsyncEnumerator.Current {
            get { return Current; }
        }
    }
}
