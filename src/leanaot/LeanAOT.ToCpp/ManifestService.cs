using dnlib.DotNet;
using LeanAOT.GenerationPlan;

namespace LeanAOT.ToCpp
{
    public class ManifestService
    {
        public Manifest Manifest { get; }

        public ManifestService(Manifest manifest)
        {
            Manifest = manifest;
        }

        public bool ShouldAOT(IMethod method)
        {
            if (method is MethodDef methodDef)
            {
                return Manifest.AssemblyPlans.TryGetValue(methodDef.Module.Assembly.Name, out var assPlan) &&
                    assPlan.ContainsMethod(methodDef);
            }
            return false;
        }
    }
}
