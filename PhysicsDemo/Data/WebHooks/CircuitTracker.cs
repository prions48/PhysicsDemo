namespace PhysicsDemo.Data.WebHooks;
using Microsoft.AspNetCore.Components.Server.Circuits;
public class CircuitTracker : CircuitHandler
{
    private static int _count;
    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken ct)
    {
        Interlocked.Increment(ref _count);
        Console.WriteLine($"Circuit opened. Total: {_count}");
        return Task.CompletedTask;
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken ct)
    {
        Interlocked.Decrement(ref _count);
        Console.WriteLine($"Circuit closed. Total: {_count}");
        return Task.CompletedTask;
    }
}
