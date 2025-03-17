using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Draw : DrawX
{
    protected override int GetToDraw(IResolutionContext _) => 1;
}