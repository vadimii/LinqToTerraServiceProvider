using System.Linq.Expressions;

namespace LinqToTerraServiceProvider.Internal;

internal class TerraServiceQueryContext(DataServiceHelper dataServiceHelper)
{
    public object? Execute(Expression expression, bool isEnumerable)
    {
        if (!IsQueryOverDataSource(expression))
        {
            throw new InvalidProgramException("No query over the data source was specified");
        }

        var whereFinder = new InnermostWhereFinder();
        var whereExpression = whereFinder.GetInnermostWhere(expression);
        var lambdaExpression = (LambdaExpression)((UnaryExpression)whereExpression!.Arguments[1]).Operand;

        lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

        var lf = new LocationFinder(lambdaExpression.Body);
        var locations = lf.Locations;

        if (locations.Count == 0)
        {
            throw new InvalidQueryException("You must specify at least one place name in your query.");
        }

        var places = dataServiceHelper.GetPlacesFromTerraService(locations);
        var queryablePlaces = places.AsQueryable();
        var treeCopier = new ExpressionTreeModifier(queryablePlaces);
        var newExpressionTree = treeCopier.Visit(expression);

        return isEnumerable
            ? queryablePlaces.Provider.CreateQuery(newExpressionTree)
            : queryablePlaces.Provider.Execute(newExpressionTree);
    }

    private static bool IsQueryOverDataSource(Expression expression)
    {
        return expression is MethodCallExpression;
    }
}
