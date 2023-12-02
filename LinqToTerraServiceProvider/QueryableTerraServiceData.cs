using System.Collections;
using System.Linq.Expressions;
using LinqToTerraServiceProvider.Internal;
using LinqToTerraServiceProvider.Service;

namespace LinqToTerraServiceProvider;

public class QueryableTerraServiceData<TData> : IOrderedQueryable<TData>
{
    public Type ElementType => typeof(TData);
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

    public QueryableTerraServiceData(ITerraServiceClientFactory clientFactory)
    {
        Expression = Expression.Constant(this);
        Provider = new TerraServiceQueryProvider(
            new TerraServiceQueryContext(
                new DataServiceHelper(clientFactory)));
    }

    public QueryableTerraServiceData(IQueryProvider provider, Expression expression)
    {
        Provider = provider;
        Expression = expression;
    }

    public IEnumerator<TData> GetEnumerator()
    {
        return Provider.Execute<IEnumerable<TData>>(Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
    }
}
