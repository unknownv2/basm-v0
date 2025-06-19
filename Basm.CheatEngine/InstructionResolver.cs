using System;
using System.Collections.Generic;
using System.Text;
using Basm.Architecture;
using System;
using System.Globalization;
using System.Linq;
using Basm.Architecture.X64;


namespace Basm.CheatEngine
{
    public class InstructionResolver : IExpressionResolver
    {
        private Assembler.IScriptContext _scriptContext;

        public InstructionResolver(Basm.Assembler.IScriptContext scriptContext)
        {
            _scriptContext = scriptContext;
        }

        public object Resolve(object expression)
        {
            var value = Resolve(expression as IExpression, typeof(string));
     
            return value;
        }

        public object Resolve(IExpression expression, Type type)
        {
            if (!ProcessUnaryExpression(expression, type, out object value) &&
                !ProcessBinaryExpression(expression, type, out value) &&
                !ProcessCastExpression(expression, type, out value) &&
                !ProcessPointerExpression(expression, type, out value) &&
                !ProcessRegisterExpression(expression, type, out value) &&
                !ProcessValueExpression(expression, type, out value))
  
            
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
        private bool ProcessPointerExpression(IExpression expression, Type type, out object value)
        {
            var pointerExpression = expression as IPointerExpression;
            if (pointerExpression == null || pointerExpression.Expression == null)
            {
                value = null;
                return false;
            }
            
            value = $"{pointerExpression.Type} PTR [{Resolve(pointerExpression.Expression, typeof(string))}]";

            return true;
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
                return false;
            }

            var rightOperand = ResolveDynamic(binaryExpression.Right, type);
            if (rightOperand == null)
            {
                value = null;
                return false;
            }

            switch (binaryExpression.Operator)
            {
                case BinaryOperator.Addition:
                    value = leftOperand + "+" + rightOperand;
                    return true;
                case BinaryOperator.Subtraction:
                    value = leftOperand + "-" + rightOperand;
                    return true;
                case BinaryOperator.Multiplication:
                    value = leftOperand + "*" + rightOperand;
                    return true;
                case BinaryOperator.Division:
                    value = leftOperand + "/" + rightOperand;
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
        private bool ProcessRegisterExpression(IExpression expression, Type type, out object value)
        {
            var valueNode = expression as IRegisterNode;
            if (valueNode == null)
            {
                value = null;
                return false;
            }


            value = valueNode.Register.ToString();
            return true;

        }
        private bool ProcessValueExpression(IExpression expression, Type type, out object value)
        {
            var valueNode = expression as IImmediateValueNode;
            if (valueNode == null)
            {
                value = null;
                return false;
            }
  

            // Check if there's a symbol that matches the string value.
            if (GetPossibleSymbolValue(valueNode, out value))
            {
                return true;
            }


            ValueNode node = new ValueNode((string)valueNode.Expression, true);

            value = node.GetValue(typeof(UInt64)).ToString();
            return true;
      
        }

        private bool GetPossibleSymbolValue(IImmediateValueNode node, out object value)
        {
            var script = _scriptContext.Section.Script;
            var possibleSymbolName = (string) node.Expression;

            if (script.Symbols.TryGetValue(possibleSymbolName, out long longValue) ||
                script.Assembler.Symbols.TryGetValue(possibleSymbolName, out longValue))
            {
                // Null symbol means a null value.
                // This lets the caller know that the value is unknown at this time.
                value = longValue == 0 ? null : longValue.ToString();
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
