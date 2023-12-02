using System.Linq.Expressions;
using LinqToTerraServiceProvider.Data;

namespace LinqToTerraServiceProvider.Internal;

internal class ExpressionTreeModifier(IQueryable<Place> queryablePlaces) : ExpressionVisitor
{
    protected override Expression VisitConstant(ConstantExpression constantExpression)
    {
        return constantExpression.Type == typeof(QueryableTerraServiceData<Place>)
            ? Expression.Constant(queryablePlaces)
            : constantExpression;
    }
}
