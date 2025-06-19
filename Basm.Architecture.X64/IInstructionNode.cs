using System;
using System.Collections.Generic;

namespace Basm.Architecture.X64
{
    /// <summary>
    /// Base interface of all x86-64 nodes.
    /// </summary>
    public interface INode
    {

    }

    /// <summary>
    /// A single x86-64 instruction.
    /// </summary>
    public interface IInstructionNode : INode
    {
        /// <summary>
        /// The type of instruction.
        /// </summary>
        Instruction Instruction { get; }

        /// <summary>
        /// The instruction's operands.
        /// </summary>
        IEnumerable<IExpression> Operands { get; }
    }

    /// <summary>
    /// A resolvable node.
    /// </summary>
    public interface IExpression : INode
    {

    }

    public interface IBinaryExpression : IExpression
    {
        /// <summary>
        /// The operation to apply to the left and right expressions.
        /// </summary>
        BinaryOperator Operator { get; }

        /// <summary>
        /// The expression on the left side of the operator.
        /// </summary>
        IExpression Left { get; }

        /// <summary>
        /// The expression on the right side of the operator.
        /// </summary>
        IExpression Right { get; }
    }

    /// <summary>
    /// An operation that can be applied to two expresions.
    /// </summary>
    public enum BinaryOperator
    {
        Multiplication,
        Division,
        Addition,
        Subtraction,
    }
    /// <summary>
    /// An expression that applies an operation to one operand.
    /// </summary>
    public interface IUnaryExpression : IExpression
    {
        /// <summary>
        /// The operation to apply to the operand.
        /// </summary>
        UnaryOperator Operator { get; }

        /// <summary>
        /// The expression the operator will manipulate.
        /// </summary>
        IExpression Operand { get; }
    }

    /// <summary>
    /// An operation that can applied to a single operand.
    /// </summary>
    public enum UnaryOperator
    {
        Positive,
        Negative,
        BitwiseNot,
    }
    /// <summary>
    /// An expression that resolves to a pointer.
    /// </summary>
    public interface IPointerExpression : IExpression
    {
        /// <summary>
        /// The pointer size.
        /// </summary>
        PointerType Type { get; }

        /// <summary>
        /// An expression that resolves to
        /// the type of pointer.
        /// </summary>
        IExpression Expression { get; }
    }

    public enum PointerType
    {
        BYTE,
        WORD,
        DWORD,
        QWORD,
        XMMWORD,
        YMMWORD,
        ZMMWORD
    }

    /// <summary>
    /// An immediate value.
    /// </summary>
    public interface IImmediateValueNode : IExpression
    {
        /// <summary>
        /// The expression of the immediate value.
        /// </summary>
        object Expression { get; }
    }

    public interface ICastExpression : IExpression
    {
        /// <summary>
        /// The type to cast the operand to.
        /// </summary>
        Type CastType { get; }

        /// <summary>
        /// The expression to cast.
        /// </summary>
        IExpression Operand { get; }
    }

    /// <summary>
    /// A CPU register.
    /// </summary>
    public interface IRegisterNode : IExpression
    {
        Register Register { get; }
    }

    public enum Register
    {
        ST,
        RAX, EAX, AX, AL, MM0, XMM0, ST0, CS, CR0, DR0,
        RBX, EBX, BX, BL, MM1, XMM1, ST1, DS, CR1, DR1,
        RCX, ECX, CX, CL, MM2, XMM2, ST2, ES, CR2, DR2,
        RDX, EDX, DX, DL, MM3, XMM3, ST3, FS, CR3, DR3,
        RSP, ESP, SP, AH, MM4, XMM4, ST4, GS, CR4, DR4, SPL,
        RBP, EBP, BP, BH, MM5, XMM5, ST5, HS, CR5, DR5, BPL,
        RSI, ESI, SI, CH, MM6, XMM6, ST6, IS, CR6, DR6, SIL,
        RDI, EDI, DI, DH, MM7, XMM7, ST7, SS, CR7, DR7, DIL,
        R8, R8D, R8W, R8L, MM8, XMM8, ST8, JS, CR8, DR8,
        R9, R9D, R9W, R9L, MM9, XMM9, ST9, KS, CR9, DR9,
        R10, R10D, R10W, R10L, MM10, XMM10, ST10, CR10, DR10,
        R11, R11D, R11W, R11L, MM11, XMM11, ST11, LS, CR11, DR11,
        R12, R12D, R12W, R12L, MM12, XMM12, ST12, MS, CR12, DR12,
        R13, R13D, R13W, R13L, MM13, XMM13, ST13, NS, CR13, DR13,
        R14, R14D, R14W, R14L, MM14, XMM14, ST14, OS, CR14, DR14,
        R15, R15D, R15W, R15L, MM15, XMM15, ST15, PS, CR15, DR15,
    }

    public enum Instruction
    {
        AAD,
        AAM,
        AAS,
        ADC,
        ADD,
        ADDPD,
        ADDPS,
        ADDSD,
        ADDSS,
        ADDSUBPD,
        ADDSUBPS,
        ADX,
        ALTER,
        AMX,
        AND,
        ANDNPD,
        ANDNPS,
        ANDPD,
        ANDPS,
        ARPL,
        BLENDPS,
        BLENDVPD,
        BLENDVPS,
        BOUND,
        BSF,
        BSR,
        BSWAP,
        BT,
        BTC,
        BTR,
        BTS,
        CALL,
        CALLF,
        CBW,
        CDQ,
        CLC,
        CLD,
        CLFLUSH,
        CLI,
        CLTS,
        CMC,
        CMOVNO,
        CMOVNS,
        CMOVO,
        CMOVS,
        CMP,
        CMPPD,
        CMPPS,
        CMPSD,
        CMPSS,
        CMPXCHG,
        CMPXCHG8B,
        COMISD,
        COMISS,
        CPUID,
        CRC32,
        CS,
        CVTDQ2PD,
        CVTDQ2PS,
        CVTPD2DQ,
        CVTPD2PI,
        CVTPD2PS,
        CVTPI2PD,
        CVTPI2PS,
        CVTPS2DQ,
        CVTPS2PD,
        CVTPS2PI,
        CVTSD2SI,
        CVTSD2SS,
        CVTSI2SD,
        CVTSI2SS,
        CVTSS2SD,
        CVTSS2SI,
        CVTTPD2DQ,
        CVTTPD2PI,
        CVTTPS2DQ,
        CVTTPS2PI,
        CVTTSD2SI,
        CVTTSS2SI,
        CWD,
        CWDE,
        DAS,
        DEC,
        DIV,
        DIVPD,
        DIVPS,
        DIVSD,
        DIVSS,
        DPPD,
        DPPS,
        DS,
        ENTER,
        ES,
        EXTRACTPS,
        FABS,
        FADD,
        FADDP,
        FBLD,
        FBSTP,
        FCHS,
        FCLEX,
        FCMOVB,
        FCMOVBE,
        FCMOVE,
        FCMOVNB,
        FCMOVNBE,
        FCMOVNE,
        FCMOVNU,
        FCMOVU,
        FCOM,
        FCOMI,
        FCOMIP,
        FCOMP,
        FCOMPP,
        FCOS,
        FDECSTP,
        FDISI,
        FDIV,
        FDIVP,
        FDIVR,
        FDIVRP,
        FENI,
        FFREE,
        FFREEP,
        FIADD,
        FICOM,
        FICOMP,
        FIDIV,
        FIDIVR,
        FILD,
        FIMUL,
        FINCSTP,
        FINIT,
        FIST,
        FISTP,
        FISTTP,
        FISUB,
        FISUBR,
        FLD,
        FLD1,
        FLDCW,
        FLDENV,
        FLDL2E,
        FLDL2T,
        FLDLG2,
        FLDLN2,
        FLDPI,
        FLDZ,
        FMUL,
        FMULP,
        FNCLEX,
        FNDISI,
        FNENI,
        FNINIT,
        FNOP,
        FNSAVE,
        FNSETPM,
        FNSTCW,
        FNSTENV,
        FNSTSW,
        FPATAN,
        FPREM,
        FPREM1,
        FPTAN,
        FRNDINT,
        FRSTOR,
        FS,
        FSAVE,
        FSCALE,
        FSETPM,
        FSIN,
        FSINCOS,
        FSQRT,
        FST,
        FSTCW,
        FSTENV,
        FSTP,
        FSTSW,
        FSUB,
        FSUBP,
        FSUBR,
        FSUBRP,
        FTST,
        FUCOM,
        FUCOMI,
        FUCOMIP,
        FUCOMP,
        FUCOMPP,
        FXAM,
        FXCH,
        FXRSTOR,
        FXSAVE,
        FXTRACT,
        FYL2X,
        FYL2XP1,
        GS,
        HADDPS,
        HINT_NOP,
        HLT,
        HSUBPD,
        HSUBPS,
        IDIV,
        IMUL,
        IN,
        INC,
        INT,
        INTO,
        INVD,
        INVEPT,
        INVLPG,
        INVVPID,
        IRET,
        IRETD,
        JMP,
        JMPE,
        JMPF,
        JNO,
        JNS,
        JO,
        JS,
        LDDQU,
        LDMXCSR,
        LDS,
        LEA,
        LEAVE,
        LES,
        LFENCE,
        LFS,
        LGDT,
        LGS,
        LIDT,
        LLDT,
        LMSW,
        LOADALL,
        LOCK,
        LOOP,
        LSS,
        LTR,
        MASKMOVQ,
        MAXPD,
        MAXPS,
        MAXSD,
        MAXSS,
        MFENCE,
        MINPD,
        MINPS,
        MINSD,
        MINSS,
        MONITOR,
        MOV,
        MOVAPD,
        MOVAPS,
        MOVBE,
        MOVD,
        MOVDDUP,
        MOVDQ2Q,
        MOVDQA,
        MOVDQU,
        MOVHLPS,
        MOVHPD,
        MOVHPS,
        MOVLHPS,
        MOVLPD,
        MOVLPS,
        MOVMSKPD,
        MOVMSKPS,
        MOVNTDQ,
        MOVNTDQA,
        MOVNTI,
        MOVNTPD,
        MOVNTPS,
        MOVNTQ,
        MOVQ,
        MOVQ2DQ,
        MOVSD,
        MOVSHDUP,
        MOVSLDUP,
        MOVSS,
        MOVSX,
        MOVSXD,
        MOVUPD,
        MOVUPS,
        MOVZX,
        MPSADBW,
        MUL,
        MULPD,
        MULPS,
        MULSD,
        MULSS,
        MWAIT,
        NEG,
        NOP,
        NOT,
        NTAKEN,
        OR,
        ORPD,
        ORPS,
        OUT,
        PABSB,
        PABSD,
        PABSW,
        PACKSSDW,
        PACKSSWB,
        PACKUSDW,
        PACKUSWB,
        PADDB,
        PADDD,
        PADDQ,
        PADDSB,
        PADDSW,
        PADDUSB,
        PADDUSW,
        PADDW,
        PALIGNR,
        PAND,
        PANDN,
        PAUSE,
        PAVGB,
        PAVGW,
        PBLENDVB,
        PBLENDW,
        PCMPEQB,
        PCMPEQD,
        PCMPEQQ,
        PCMPEQW,
        PCMPESTRI,
        PCMPESTRM,
        PCMPGTB,
        PCMPGTD,
        PCMPGTQ,
        PCMPGTW,
        PCMPISTRI,
        PCMPISTRM,
        PEXTRW,
        PHADDD,
        PHADDSW,
        PHADDW,
        PHMINPOSUW,
        PHSUBD,
        PHSUBSW,
        PHSUBW,
        PMADDUBSW,
        PMADDWD,
        PMAXSB,
        PMAXSD,
        PMAXSW,
        PMAXUB,
        PMAXUD,
        PMAXUW,
        PMINSB,
        PMINSD,
        PMINSW,
        PMINUB,
        PMINUD,
        PMINUW,
        PMOVMSKB,
        PMULDQ,
        PMULHRSW,
        PMULHUW,
        PMULHW,
        PMULLD,
        PMULLW,
        PMULUDQ,
        POP,
        POPA,
        POPAD,
        POPCNT,
        POPF,
        POPFD,
        POR,
        PREFETCHNTA,
        PREFETCHT0,
        PREFETCHT1,
        PREFETCHT2,
        PSADBW,
        PSHUFB,
        PSHUFD,
        PSHUFHW,
        PSHUFLW,
        PSHUFW,
        PSIGNB,
        PSIGND,
        PSIGNW,
        PSLLD,
        PSLLDQ,
        PSLLQ,
        PSLLW,
        PSRAD,
        PSRAW,
        PSRLD,
        PSRLDQ,
        PSRLQ,
        PSRLW,
        PSUBB,
        PSUBD,
        PSUBQ,
        PSUBSB,
        PSUBSW,
        PSUBUSB,
        PSUBUSW,
        PSUBW,
        PTEST,
        PUNPCKHBW,
        PUNPCKHDQ,
        PUNPCKHQDQ,
        PUNPCKHWD,
        PUNPCKLBW,
        PUNPCKLDQ,
        PUNPCKLQDQ,
        PUNPCKLWD,
        PUSH,
        PUSHA,
        PUSHAD,
        PUSHF,
        PUSHFD,
        PXOR,
        RCL,
        RCPPS,
        RCPSS,
        RCR,
        RDMSR,
        RDPMC,
        RDTSC,
        RDTSCP,
        REP,
        RETF,
        RETN,
        REX,
        REX_B,
        REX_R,
        REX_RB,
        REX_RX,
        REX_RXB,
        REX_W,
        REX_WB,
        REX_WR,
        REX_WRB,
        REX_WRX,
        REX_WRXB,
        REX_WX,
        REX_WXB,
        REX_X,
        REX_XB,
        ROL,
        ROR,
        ROUNDPD,
        ROUNDPS,
        ROUNDSD,
        ROUNDSS,
        RSM,
        RSQRTPS,
        RSQRTSS,
        SAR,
        SBB,
        SETNO,
        SETNS,
        SETO,
        SETS,
        SFENCE,
        SGDT,
        SHLD,
        SHR,
        SHRD,
        SHUFPD,
        SHUFPS,
        SIDT,
        SQRTPD,
        SQRTPS,
        SQRTSD,
        SQRTSS,
        SS,
        STC,
        STD,
        STI,
        STMXCSR,
        SUB,
        SUBPD,
        SUBPS,
        SUBSD,
        SUBSS,
        SWAPGS,
        SYSCALL,
        SYSENTER,
        SYSEXIT,
        SYSRET,
        TEST,
        UCOMISS,
        UD,
        UD2,
        UNPCKHPD,
        UNPCKHPS,
        UNPCKLPD,
        UNPCKLPS,
        VERW,
        VMCALL,
        VMCLEAR,
        VMLAUNCH,
        VMPTRLD,
        VMPTRST,
        VMREAD,
        VMRESUME,
        VMWRITE,
        VMXOFF,
        VMXON,
        WRMSR,
        XADD,
        XCHG,
        XGET,
        XOR,
        XORPD,
        XORPS,
        XRSTOR,
        XSAVE,
        XSETBV,
    }
}
