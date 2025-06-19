using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Basm.Assembler.Implementation
{
    internal sealed class Section : ISection
    {
        private readonly ISectionNode _root;
        private readonly IMemoryWriter _memoryWriter;
        private readonly IExpressionResolver _expressionResolver;
        private readonly ScriptContext _scriptContext;
        private readonly List<Label> _labels;

        private Label _currentLabel;

        public Section(BasmAssembler assembler, IScript script, ISectionNode section, IMemoryWriter memoryWriter)
        {
            _root = section;
            _scriptContext = new ScriptContext(this);
            _memoryWriter = memoryWriter;
            _expressionResolver = new ExpressionResolver(assembler, _scriptContext);
            _labels = new List<Label>();

            Script = script;
            Name = section.Name;
        }

        public IScript Script { get; }
        public string Name { get; }
        private ulong _currentAddress;

        public void Execute()
        {
            if (!_root.Children.All(IsValidTopLevelNode))
            {
                throw new Exception("Invalid node type in section.");
            }

            // Process top-level directives.
            foreach (var node in _root.Children.OfType<IDirectiveNode>())
            {
                ProcessDirective(node);
            }

            // Pre-process labels to allocate symbols.
            foreach (var node in _root.Children.OfType<ILabelNode>())
            {
                PreprocessLabel(node);
            }

            // Process all labels.
            foreach (var label in _labels)
            {
                ProcessLabel(label);
            }

            // Last pass, copy instructions to destination.
            foreach (var label in _labels)
            {
                FinalizeAndCopyInstructions(label);
            }
        }

        private bool IsValidTopLevelNode(INode node)
        {
            return (node is IDirectiveNode) || (node is ILabelNode) || (node is ICommentNode);
        }

        private void PreprocessLabel(ILabelNode node)
        {
            var valueExpression = node.Expression as IValueNode;
            if (valueExpression == null)
            {
                BeginNewLabel(node);
                return;
            }

            var probableSymbolName = (string)valueExpression.GetValue(typeof(string));

            var script = _scriptContext.Section.Script;

            // We assume that IValueNodes are symbol names. They can be numbers, 
            // but when has anybody created a label at an absolute address?
            if (probableSymbolName.StartsWith("0")
                || (_expressionResolver.Resolve(node.Expression, typeof(long)) != null))
                //|| script.Symbols.ContainsKey(probableSymbolName)
                //|| script.Assembler.Symbols.ContainsKey(probableSymbolName))
            {
                
                BeginNewLabel(node);
            }
            else
            {
                if (_currentLabel == null)
                {
                    throw new Exception($"Unknown symbol for label ${probableSymbolName}.");
                }

                script.Symbols[probableSymbolName] = 0; // TODO: Allocate near last label with expression.
                _currentLabel.Children.Add(node);
            }
        }

        private void BeginNewLabel(ILabelNode node)
        {
            _currentLabel = new Label(node);
            _labels.Add(_currentLabel);
        }

        private void ProcessLabel(Label label)
        {
            label.Address = ResolveLabelAddress(label);
            _currentAddress = (ulong) (label.Address);

            _scriptContext.InstructionStream = label.InstructionStream;
            _scriptContext.UnresolvedImmediates = label.UnresolvedImmediates;

            foreach (var childNode in label.Node.Children)
            {
                if (!ProcessDirective(childNode) &&
                    !ProcessInstruction(childNode) &&
                    !ProcessComment(childNode))
                {
                    throw new Exception("Invalid node type in label.");
                }
            }

            foreach (var childLabel in label.Children)
            {
                var symbolName = (string)((IValueNode)childLabel.Expression).GetValue(typeof(string));

                // Record the address of this label.
                _scriptContext.Section.Script.Symbols[symbolName] 
                    = label.Address + label.InstructionStream.Position;

                _currentAddress = (ulong) (label.Address + label.InstructionStream.Position);
                
                foreach (var childNode in childLabel.Children)
                {
                    if (!ProcessDirective(childNode) &&
                        !ProcessInstruction(childNode) &&
                        !ProcessComment(childNode))
                    {
                        throw new Exception("Invalid node type in label.");
                    }
                }
            }
        }

        private long ResolveLabelAddress(Label label)
        {
            return (long)_expressionResolver.Resolve(label.Node.Expression, typeof(long));
        }

        private bool ProcessDirective(INode node)
        {
            var directiveNode = node as IDirectiveNode;
            if (directiveNode == null)
            {
                return false;
            }

            _expressionResolver.Resolve(directiveNode, null);

            return true;
        }
        private bool ProcessInstruction(INode node)
        {
            var instructionNode = node as IInstructionNode;
            if (instructionNode == null)
            {
                return false;
            }
            if (_scriptContext.InstructionStream == null)
            {
                throw new Exception("Instructions must appear within a label.");
            }

            instructionNode.SetResolver(_scriptContext);
            instructionNode.SetAddress(_currentAddress + (ulong) _scriptContext.InstructionStream.Position);
            var immediates = instructionNode.Assemble(_scriptContext.InstructionStream, _expressionResolver);

            _scriptContext.UnresolvedImmediates.AddRange(immediates);

            return true;
        }

        private bool ProcessComment(INode node)
        {
            return node is ICommentNode;
        }

        private void FinalizeAndCopyInstructions(Label label)
        {
            using (label.InstructionStream)
            {
                var size = (int)label.InstructionStream.Position;

                // Resolve expressions that couldn't be resolved earlier.
                foreach (var unresolvedImmediate in label.UnresolvedImmediates)
                {
                    var value = _expressionResolver.Resolve(unresolvedImmediate.Expression, typeof(decimal));
                    var buffer = CreateImmediateValueBytes(value, unresolvedImmediate.Size);

                    label.InstructionStream.Position = unresolvedImmediate.Offset;
                    label.InstructionStream.Write(buffer, 0, (int)unresolvedImmediate.Size);
                }

                // Copy instructions to destination address.
                label.InstructionStream.Position = 0;
                _memoryWriter.WriteMemory(label.Address, label.InstructionStream, size);
            }

            label.InstructionStream = null;
            label.UnresolvedImmediates = null;
        }

        private byte[] CreateImmediateValueBytes(object value, ImmediateValueSize size)
        {
            var decimalValue = Convert.ToDecimal(value);

            switch (size)
            {
                case ImmediateValueSize.B1:
                    return new byte[] { (byte)decimalValue }; // Should we support signed bytes?
                case ImmediateValueSize.B2:
                    return decimalValue < 0
                        ? BitConverter.GetBytes((short)decimalValue)
                        : BitConverter.GetBytes((ushort)decimalValue);
                case ImmediateValueSize.B4:
                    if (IsFloatingPointType(value))
                    {
                        return BitConverter.GetBytes((float)decimalValue);
                    }
                    else
                    {
                        return decimalValue < 0
                            ? BitConverter.GetBytes((int)decimalValue)
                            : BitConverter.GetBytes((uint)decimalValue);
                    }
                case ImmediateValueSize.B8:
                    if (IsFloatingPointType(value))
                    {
                        return BitConverter.GetBytes((double)decimalValue);
                    }
                    else
                    {
                        return decimalValue < 0
                            ? BitConverter.GetBytes((long)decimalValue)
                            : BitConverter.GetBytes((ulong)decimalValue);
                    }
                default:
                    throw new Exception("Invalid immediate value size.");
            }
        }

        private bool IsFloatingPointType(object value)
        {
            var type = value.GetType();
            return type == typeof(Single)
                || type == typeof(Double)
                || type == typeof(Decimal);
        }
    }

    internal sealed class ScriptContext : IScriptContext
    {
        public ISection Section { get; }
        public Stream InstructionStream { get; set; }
        public List<IUnresolvedImmediateValue> UnresolvedImmediates { get; set; }

        public ScriptContext(ISection section)
        {
            Section = section;
        }
    }

    internal sealed class Label
    {
        public long Address;
        public ILabelNode Node;
        public List<ILabelNode> Children;
        public Stream InstructionStream;
        public List<IUnresolvedImmediateValue> UnresolvedImmediates;

        public Label(ILabelNode node, int initialChildCapacity = 16)
        {
            Node = node;
            Children = new List<ILabelNode>(8);
            InstructionStream = new MemoryStream(Environment.SystemPageSize);
            UnresolvedImmediates = new List<IUnresolvedImmediateValue>(16);
        }
    }
}
