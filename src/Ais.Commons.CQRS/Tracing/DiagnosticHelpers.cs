using System.Diagnostics;

namespace Ais.Commons.CQRS.Tracing;

internal static class DiagnosticHelpers
{
    private static ActivitySource ActivitySource { get; } = new(DiagnosticHeaders.DefaultListenerName);
    
    public static Activity? StartActivity(string name, bool enable = true)
    {
        return !enable ? null : ActivitySource.StartActivity(name);
    }
}