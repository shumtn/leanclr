using dnlib.DotNet;
using LeanAOT.Core;
using System.Text;

namespace LeanAOT.ToCpp
{
    public class MethodInvokerInfo
    {
        public bool isVirtual;
        public string name;
        public IMethod method;
    }

    public class InvokerService
    {
        private readonly MetadataService _metadataService;
        private readonly Dictionary<string, MethodInvokerInfo> _methodInvokerInfos;
        private readonly Dictionary<string, MethodInvokerInfo> _virtualMethodInvokerInfos;

        public InvokerService(MetadataService metadataService)
        {
            _metadataService = metadataService;
            _methodInvokerInfos = new Dictionary<string, MethodInvokerInfo>();
            _virtualMethodInvokerInfos = new Dictionary<string, MethodInvokerInfo>();
        }

        public List<MethodInvokerInfo> GetNotVirtualInvokers()
        {
            return _methodInvokerInfos.Values.ToList();
        }

        public List<MethodInvokerInfo> GetVirtualInvokers()
        {
            return _virtualMethodInvokerInfos.Values.ToList();
        }

        static string CreateRelaxedMethodFunctionTypeDeclaring(IMethod method)
        {
            MethodSig methodSig = method.MethodSig;
            StringBuilder sb = new StringBuilder();
            sb.Append(MethodGenerationUtil.GetResultTypeName(methodSig.RetType));
            sb.Append(" (*)(");
            var typeNameService = GlobalServices.Inst.TypeNameService;
            bool first = true;
            if (methodSig.HasThis)
            {
                sb.Append(typeNameService.GetCppTypeNameAsFieldOrArgOrLoc(MetaUtil.GetThisType(method), TypeNameRelaxLevel.AbiRelaxed));
                first = false;
            }
            sb.Append(')');
            foreach (var param in methodSig.Params)
            {
                if (first)
                    first = false;
                else
                {
                    sb.Append(", ");
                }
                sb.Append(typeNameService.GetCppTypeNameAsFieldOrArgOrLoc(param, TypeNameRelaxLevel.AbiRelaxed));
            }
            sb.Append(')');
            return sb.ToString();
        }

        private string GetInvokerName(IMethod method, bool callVir)
        {
            string hash = HashUtil.CreateMd5Hash(CreateRelaxedMethodFunctionTypeDeclaring(method));
            return $"leanclr_generated_invoke_{(callVir ? "virtual" : "")}method_{hash}";
        }

        public MethodInvokerInfo GetNotVirtualInvoker(IMethod method)
        {
            string invokerName = GetInvokerName(method, false);
            if (_methodInvokerInfos.TryGetValue(invokerName, out var info))
            {
                return info;
            }
            var newInfo = new MethodInvokerInfo
            {
                isVirtual = false,
                name = invokerName,
                method = method,
            };
            _methodInvokerInfos[invokerName] = newInfo;
            return newInfo;
        }

        public MethodInvokerInfo GetVirtualInvoker(IMethod method)
        {
            var methodDetail = _metadataService.GetMethodDetail(method);
            if (!MetaUtil.IsValueType(method.DeclaringType.ToTypeSig()) || methodDetail.IsStatic)
            {
                return GetNotVirtualInvoker(method);
            }
            string invokerName = GetInvokerName(method, true);
            if (_virtualMethodInvokerInfos.TryGetValue(invokerName, out var info))
            {
                return info;
            }
            var newInfo = new MethodInvokerInfo
            {
                isVirtual = true,
                name = invokerName,
                method = method,
            };
            _virtualMethodInvokerInfos[invokerName] = newInfo;
            return newInfo;
        }
    }
}
