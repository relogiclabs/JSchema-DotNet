using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Library;

internal delegate IEValue FunctionEvaluator(ScriptScope scope, List<IEValue> arguments);