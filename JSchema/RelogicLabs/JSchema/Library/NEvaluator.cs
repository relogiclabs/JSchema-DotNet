using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Library;

internal delegate IEValue NEvaluator(ScopeContext scope, List<IEValue> arguments);