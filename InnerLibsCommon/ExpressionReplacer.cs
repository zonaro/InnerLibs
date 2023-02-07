using System.Linq.Expressions;

namespace InnerLibs
{
    internal class ExpressionReplacer : ExpressionVisitor
    {
        #region Private Fields

        private Expression _dest;
        private Expression _source;

        #endregion Private Fields

        #region Public Constructors

        public ExpressionReplacer(Expression source, Expression dest)
        {
            _source = source;
            _dest = dest;
        }

        #endregion Public Constructors

        #region Public Methods

        public override Expression Visit(Expression node)
        {
            if (node.Equals(_source))
            {
                return _dest;
            }

            return base.Visit(node);
        }

        #endregion Public Methods
    }


}