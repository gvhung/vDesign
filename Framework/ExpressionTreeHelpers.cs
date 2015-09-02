using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Framework
{
    /// <summary>
    /// Этот класс содержит методы, используемые для определения сведений и извлечения данных из определенных типов деревьев выражений.
    /// </summary>
    public class ExpressionTreeHelpers
    {
        public static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName)
        {
            if (exp.NodeType != ExpressionType.Equal)
                return false;

            BinaryExpression be = (BinaryExpression)exp;

            // Assert.
            if (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) &&
                ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName))
                throw new Exception("Cannot have 'member' == 'member' in an expression!");

            return (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) ||
                ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName));
        }

        public static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
        {
            return ((exp is MemberExpression) &&
                (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                (((MemberExpression)exp).Member.Name == memberName));
        }

        public static object GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType, string memberName)
        {
            if (be.NodeType != ExpressionType.Equal)
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Left;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                {
                    return GetValueFromExpression(be.Right);
                }
            }
            else if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Right;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                {
                    return GetValueFromExpression(be.Left);
                }
            }

            // We should have returned by now.
            throw new Exception("There is a bug in this program.");
        }

        public static object GetValueFromExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)expression).Value;
            else
                throw new InvalidQueryException(
                    String.Format("The expression type {0} is not supported to obtain a value.", expression.NodeType));
        }
    }

    public class InvalidQueryException : Exception
    {
        private string message;

        public InvalidQueryException(string message)
        {
            this.message = message + " ";
        }

        public override string Message
        {
            get
            {
                return "The client query is invalid: " + message;
            }
        }
    }

    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        class SubtreeEvaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        class Nominator : ExpressionVisitor
        {
            Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression> candidates;
            bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }
    }

}
