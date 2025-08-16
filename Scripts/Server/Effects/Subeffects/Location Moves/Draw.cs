using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DrawData : DrawXData { }

public class Draw : DrawX
{
    public Draw(DrawData data) : base(data) { }

    protected override int GetToDraw(IResolutionContext _) => 1;
}