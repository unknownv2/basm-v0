using System;
using System.Globalization;
using System.Linq;

namespace Basm.Assembler.Implementation
{
    internal sealed class ExpressionResolver : IExpressionResolver
    {
        private readonly BasmAssembler _assembler;
        private readonly IScriptContext _scriptContext;

        public ExpressionResolver(BasmAssembler assembler, IScriptContext scriptContext)
        {
            _assembler = assembler;
            _scriptContext = scriptContext;
        }

        public object Resolve(IExpression expression, Type type)
        {
            if (!ProcessUnaryExpression(expression, type, out object value) &&
                !ProcessBinaryExpression(expression, type, out value) &&
                !ProcessCastExpression(expression, type, out value) &&
                !ProcessValueExpression(expression, type, out value) &&
                !ProcessDirectiveExpression(expression, type, out value))
            {
                throw new Exception("Unexpected expression type.");
            }

            return value;
        }

        private dynamic ResolveDynamic(IExpression expression, Type type)
        {
            return (dynamic)Resolve(expression, type);
        }

        private bool ProcessUnaryExpression(IExpression expression, Type type, out object value)
        {
            var unaryExpression = expression as IUnaryExpression;
            if (unaryExpression == null)
            {
                value = null;
                return false;
            }

            var operand = ResolveDynamic(unaryExpression.Operand, type);
            if (operand == null)
            {
                value = null;
                return true;
            }

            switch (unaryExpression.Operator)
            {
                case UnaryOperator.Positive:
                    value = +operand;
                    return true;
                case UnaryOperator.Negative:
                    value = -operand;
                    return true;
                case UnaryOperator.BitwiseNot:
                    value = ~operand;
                    return true;
                default:
                    throw new Exception("Unknown unary operator.");
            }
        }

        private bool ProcessBinaryExpression(IExpression expression, Type type, out object value)
        {
            var binaryExpression = expression as IBinaryExpression;
            if (binaryExpression == null)
            {
                value = null;
                return false;
            }

            var leftOperand = ResolveDynamic(binaryExpression.Left, type);
            if (leftOperand == null)
            {
                value = null;
                return true;
            }

            var rightOperand = ResolveDynamic(binaryExpression.Right, type);
            if (rightOperand == null)
            {
                value = null;
                return true;
            }

            switch (binaryExpression.Operator)
            {
                case BinaryOperator.Addition:
                    value = leftOperand + rightOperand;
                    return true;
                case BinaryOperator.Subtraction:
                    value = leftOperand - rightOperand;
                    return true;
                case BinaryOperator.Multiplication:
                    value = leftOperand * rightOperand;
                    return true;
                case BinaryOperator.Division:
                    value = leftOperand / rightOperand;
                    return true;
                default:
                    throw new Exception("Unknown binary operator.");
            }
        }

        private bool ProcessCastExpression(IExpression expression, Type type, out object value)
        {
            var castExpression = expression as ICastExpression;
            if (castExpression == null)
            {
                value = null;
                return false;
            }

            value = Resolve(castExpression.Operand, type);

            if (value != null)
            {
                value = Convert.ChangeType(value, castExpression.CastType);
            }

            return true;
        }

        private bool ProcessValueExpression(IExpression expression, Type type, out object value)
        {
            var valueExpression = expression as IValueNode;
            if (valueExpression == null)
            {
                value = null;
                return false;
            }

            var typeCode = Type.GetTypeCode(type);
            var isNumeric = typeCode != TypeCode.Boolean 
                && typeCode != TypeCode.Char 
                && typeCode != TypeCode.DateTime 
                && typeCode != TypeCode.DBNull 
                && typeCode != TypeCode.Empty
                && typeCode != TypeCode.String
                && typeCode != TypeCode.Object;

            // Check if there's a symbol that matches the string value.
            if (isNumeric && GetPossibleSymbolValue(valueExpression, out value))
            {
                return true;
            }

            value = valueExpression.GetValue(type);

            return true;
        }

        private bool GetPossibleSymbolValue(IValueNode node, out object value)
        {
            var script = _scriptContext.Section.Script;
            var possibleSymbolName = (string)node.GetValue(typeof(string));

            if (script.Symbols.TryGetValue(possibleSymbolName, out long longValue) ||
                script.Assembler.Symbols.TryGetValue(possibleSymbolName, out longValue))
            {
                // Null symbol means a null value.
                // This lets the caller know that the value is unknown at this time.
                value = longValue == 0 ? null : (object)longValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        private bool ProcessDirectiveExpression(IExpression expression, Type type, out object value)
        {
            var directiveExpression = expression as IDirectiveNode;
            if (directiveExpression == null)
            {
                value = null;
                return false;
            }

            var directive = _assembler.GetDirective(directiveExpression.Name);

            var args = directiveExpression.Arguments
                .Select(expr => (IConvertible)new ConvertibleExpression(this, expr))
                .ToArray();

            var result = directive.Handle(_scriptContext, args);

            value = type == null ? null : result?.ToType(type, CultureInfo.InvariantCulture);

            return true;
        }
    }
}
