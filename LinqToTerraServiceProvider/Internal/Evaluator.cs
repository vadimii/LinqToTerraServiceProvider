using System.Linq.Expressions;

namespace LinqToTerraServiceProvider.Internal;

internal static class Evaluator
{
    public static Expression PartialEval(Expression expression)
    {
        return PartialEval(expression, CanBeEvaluatedLocally);
    }

    private static bool CanBeEvaluatedLocally(Expression expression)
    {
        return expression.NodeType != ExpressionType.Parameter;
    }

    private static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
    {
        return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
    }

    private class SubtreeEvaluator(IReadOnlySet<Expression> candidates) : ExpressionVisitor
    {
        public Expression Eval(Expression exp)
        {
            return Visit(exp)!;
        }

        public override Expression? Visit(Expression? exp)
        {
            if (exp != null && candidates.Contains(exp))
            {
                return Evaluate(exp);
            }

            return base.Visit(exp);
        }

        private static Expression Evaluate(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant)
            {
                return e;
            }

            var lambda = Expression.Lambda(e);
            var fn = lambda.Compile();

            return Expression.Constant(fn.DynamicInvoke(null), e.Type);
        }
    }

    private class Nominator(Func<Expression, bool> canBeEvaluated) : ExpressionVisitor
    {
        private HashSet<Expression>? _candidates;
        private bool _cannotBeEvaluated;

        public HashSet<Expression> Nominate(Expression expression)
        {
            _candidates = [];
            Visit(expression);
            return _candidates;
        }

        public override Expression? Visit(Expression? expression)
        {
            if (expression == null)
            {
                return expression;
            }

            var saveCannotBeEvaluated = _cannotBeEvaluated;
            _cannotBeEvaluated = false;
            base.Visit(expression);
            if (!_cannotBeEvaluated)
            {
                if (canBeEvaluated(expression))
                {
                    _candidates!.Add(expression);
                }
                else
                {
                    _cannotBeEvaluated = true;
                }
            }

            _cannotBeEvaluated |= saveCannotBeEvaluated;

            return expression;
        }
    }
}
