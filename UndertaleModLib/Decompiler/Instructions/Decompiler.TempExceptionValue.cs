using static UndertaleModLib.Decompiler.Decompiler;

namespace UndertaleModLib.Decompiler;

public class TempExceptionValue : Expression
{
    private Block tryBlock, catchBlock;

    public TempExceptionValue(Block tryBlock, Block catchBlock)
    {
        this.catchBlock = catchBlock;
        this.tryBlock = tryBlock;
    }

    public override Statement CleanStatement(DecompileContext context, BlockHLStatement block)
    {
        return this;
    }

    public override string ToString(DecompileContext context)
    {
        return "// TempExceptionValue " + tryBlock + " " + catchBlock;
    }

    internal override AssetIDType DoTypePropagation(DecompileContext context, AssetIDType suggestedType)
    {
        return suggestedType;
    }
}