using Content.Shared.CM14.Xenos.Construction;
using JetBrains.Annotations;

namespace Content.Client._CM14.Xenos.Construction;

[UsedImplicitly]
public sealed class XenoConstructionClientSystem : SharedXenoConstructionSystem
{
	public override void Initialize()
	{
		base.Initialize();
		Log.Info("[XenoWeeds] (client) XenoConstructionSystem.Initialize()");
	}
}
