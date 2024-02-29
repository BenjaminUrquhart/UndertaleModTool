using System.Linq;
using System.Text;

namespace UndertaleModLib.Decompiler;

public static partial class Decompiler
{
    public class TryCatchHLStatement : BlockHLStatement
    {
        public Block tryBlock, catchBlock;

        public TryCatchHLStatement(Block tryBlock, Block catchBlock = null)
        {
            this.catchBlock = catchBlock;
            this.tryBlock = tryBlock;
        }

        public new BlockHLStatement CleanBlockStatement(DecompileContext context)
        {
            if (tryBlock.Statements != null)
                Statements = tryBlock.Statements;
            return base.CleanBlockStatement(context);
        }

        internal override AssetIDType DoTypePropagation(DecompileContext context, AssetIDType suggestedType)
        {
            return suggestedType;
        }

        public override string ToString(DecompileContext context)
        {
            StringBuilder sb = new StringBuilder();
            Statements = tryBlock.Statements;
            sb.AppendLine("try");
            sb.Append(context.Indentation);
            sb.AppendLine(ToString(context, false));

            if (catchBlock != null)
            {
                Statements = catchBlock.Statements.Skip(1).ToList();
                sb.Append(context.Indentation);
                sb.Append("catch(");
                sb.Append(catchBlock.Instructions[0].Destination.Target.Name.Content);
                sb.AppendLine(")");
                sb.Append(context.Indentation);
                sb.AppendLine(ToString(context, false));
                Statements = tryBlock.Statements;
            }

            return sb.ToString();
        }
    }
}
