using System.Linq.Expressions;

namespace LinqToTerraServiceProvider.Internal;

internal static class ExpressionTreeHelpers
{
    public static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName)
    {
        if (exp.NodeType != ExpressionType.Equal)
        {
            return false;
        }

        var binaryExpression = (BinaryExpression)exp;

        if (IsSpecificMemberExpression(binaryExpression.Left, declaringType, memberName) &&
            IsSpecificMemberExpression(binaryExpression.Right, declaringType, memberName))
        {
            throw new InvalidQueryException("Cannot have 'member' == 'member' in an expression!");
        }

        return IsSpecificMemberExpression(binaryExpression.Left, declaringType, memberName) ||
               IsSpecificMemberExpression(binaryExpression.Right, declaringType, memberName);
    }

    public static string GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType, string memberName)
    {
        if (be.NodeType != ExpressionType.Equal)
        {
            throw new InvalidQueryException("There is a bug in this program.");
        }

        if (be.Left.NodeType == ExpressionType.MemberAccess)
        {
            var me = (MemberExpression)be.Left;

            if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
            {
                return GetValueFromExpression(be.Right);
            }
        }
        else if (be.Right.NodeType == ExpressionType.MemberAccess)
        {
            var me = (MemberExpression)be.Right;

            if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
            {
                return GetValueFromExpression(be.Left);
            }
        }

        throw new InvalidQueryException("There is a bug in this program.");
    }

    public static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
    {
        return exp is MemberExpression memberExpression &&
               memberExpression.Member.DeclaringType == declaringType &&
               memberExpression.Member.Name == memberName;
    }

    public static string GetValueFromExpression(Expression expression)
    {
        if (expression.NodeType == ExpressionType.Constant)
        {
            return ((ConstantExpression)expression).Value as string ?? string.Empty;
        }

        throw new InvalidQueryException($"The expression type {expression.NodeType} is not supported to obtain a value.");
    }
}
