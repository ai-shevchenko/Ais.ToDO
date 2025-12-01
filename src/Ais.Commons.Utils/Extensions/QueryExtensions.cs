using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Ais.Commons.Utils.Models;

namespace Ais.Commons.Utils.Extensions;

public static class QueryExtensions
{
    extension<T>(IQueryable<T> query) where T : class
    {
        public IQueryable<T> Page(int index, int size)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            
            return query.Skip(index * size).Take(size);
        }
        
        public IQueryable<T> Sort(string sortField, SortOrder sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortField))
            {
                return query;
            }
        
            var parameter = Expression.Parameter(typeof(T), "x");
        
            var propertyIno = typeof(T).GetProperty(sortField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (propertyIno == null)
            {
                Debug.WriteLine($"Property {sortField} not found");
                return query;
            }
            
            var methodName = sortOrder == SortOrder.Ascending 
                ? nameof(Queryable.OrderBy) 
                : nameof(Queryable.OrderByDescending);

            var sortExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(T), propertyIno.PropertyType],
                query.Expression,
                Expression.Lambda(Expression.Property(parameter, propertyIno), parameter));
        
            return query.Provider
                .CreateQuery<T>(sortExpression);
        }

        public IQueryable<T> Search(string propertyName, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }
            
            search = search.Trim().ToLower();
        
            var parameter = Expression.Parameter(typeof(T), "x");
            
            var propertyIno = typeof(T).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        
            if (propertyIno == null)
            {
                Debug.WriteLine($"Property {propertyName} not found");
                return query;
            }
            
            var propertyExpression = Expression.Property(parameter, propertyIno);
            
            // x.Property != null
            var notNullExpression = Expression.NotEqual(propertyExpression, Expression.Constant(null));
            
            // x.Property.ToLower()
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            var toLowerExpression = Expression.Call(propertyExpression, toLowerMethod);
            
            // x.Property.ToLower().Contains(search)
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            var containsExpression = Expression.Call(toLowerExpression, containsMethod, Expression.Constant(search));
            
            // x.Property != null && x.Property.ToLower().Contains(search)
            var andExpression = Expression.AndAlso(notNullExpression, containsExpression);
            
            var lambda = Expression.Lambda<Func<T, bool>>(andExpression, parameter);

            return query.Where(lambda);
        }
        
        public IQueryable<T> Search(IReadOnlyList<string> properties, string search)
        {
            if (string.IsNullOrWhiteSpace(search) || properties.Count == 0)
            {
                return query;
            }
        
            search = search.Trim().ToLower();
        
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyExpressions = new List<Expression>();
        
            foreach (var propertyName in properties)
            {
                var propertyIno = typeof(T).GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        
                if (propertyIno == null)
                {
                    Debug.WriteLine($"Property {propertyName} not found");
                    continue;
                }
            
                var propertyExpression = Expression.Property(parameter, propertyIno);
            
                // x.Property != null
                var notNullExpression = Expression.NotEqual(propertyExpression, Expression.Constant(null));
            
                // x.Property.ToLower()
                var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                var toLowerExpression = Expression.Call(propertyExpression, toLowerMethod);
            
                // x.Property.ToLower().Contains(search)
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
                var containsExpression = Expression.Call(toLowerExpression, containsMethod, Expression.Constant(search));
            
                // x.Property != null && x.Property.ToLower().Contains(search)
                var andExpression = Expression.AndAlso(notNullExpression, containsExpression);
            
                propertyExpressions.Add(andExpression);
            }

            if (propertyExpressions.Count == 0)
            {
                return query;
            }
        
            var orExpression = propertyExpressions[0];
            for (var i = 1; i < propertyExpressions.Count; i++)
            {
                orExpression = Expression.OrElse(orExpression, propertyExpressions[i]);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(orExpression, parameter);

            return query.Where(lambda);
        }
    }
}