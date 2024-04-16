using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Library;

internal delegate IEValue MethodEvaluator(IEValue self, List<IEValue> arguments, ScriptScope scope);