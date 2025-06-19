using System;
using System.Collections.Generic;
using System.Text;

namespace Basm.CheatEngine
{
    public enum NodeType
    {
        Identifier,
        Char,
        Byte,
        String,
        BinaryExpression,
        Integer,
        Section,
        Cast,
        Directive,
        Assignment,
        AccessModifier,
        Value,
        Label,
        InstructionBlock,
        ImmediateValue,
        UnaryExpression,
        Comment,
        Instruction,
        SectionBlock,
        StringBlock
    }
}
