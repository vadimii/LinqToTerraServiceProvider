using System.Linq.Expressions;

namespace LinqToTerraServiceProvider.Internal;

internal class TerraServiceQueryProvider(TerraServiceQueryContext queryContext) : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = TypeSystem.GetElementType(expression.Type);
        try
        {
            return (IQueryable)Activator.CreateInstance(typeof(QueryableTerraServiceData<>)
                .MakeGenericType(elementType), [this, expression])!;
        }
        catch (System.Reflection.TargetInvocationException tie)
        {
            throw tie.InnerException!;
        }
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new QueryableTerraServiceData<TElement>(this, expression);
    }

    public object? Execute(Expression expression)
    {
        return queryContext.Execute(expression, false);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        var isEnumerable = typeof(TResult).Name == "IEnumerable`1";

        return (TResult)queryContext.Execute(expression, isEnumerable)!;
    }
}
