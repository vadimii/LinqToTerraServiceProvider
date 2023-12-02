using System.Linq.Expressions;
using LinqToTerraServiceProvider.Data;

namespace LinqToTerraServiceProvider.Internal;

internal class LocationFinder(Expression exp) : ExpressionVisitor
{
    private List<string>? _locations;

    public List<string> Locations
    {
        get
        {
            if (_locations == null)
            {
                _locations = new List<string>();
                Visit(exp);
            }

            return _locations;
        }
    }

    protected override Expression VisitMethodCall(MethodCallExpression m)
    {
        if (m.Method.DeclaringType != typeof(string) || m.Method.Name != nameof(string.StartsWith))
        {
            return base.VisitMethodCall(m);
        }

        if (ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object!, typeof(Place), nameof(Place.Name)) ||
            ExpressionTreeHelpers.IsSpecificMemberExpression(m.Object!, typeof(Place), nameof(Place.State)))
        {
            _locations!.Add(ExpressionTreeHelpers.GetValueFromExpression(m.Arguments[0]));

            return m;
        }

        return base.VisitMethodCall(m);
    }

    protected override Expression VisitBinary(BinaryExpression be)
    {
        if (be.NodeType != ExpressionType.Equal)
        {
            return base.VisitBinary(be);
        }

        if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(Place), nameof(Place.Name)))
        {
            _locations!.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(Place), nameof(Place.Name)));

            return be;
        }

        if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(Place), nameof(Place.State)))
        {
            _locations!.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(Place), nameof(Place.State)));

            return be;
        }

        return base.VisitBinary(be);
    }
}
