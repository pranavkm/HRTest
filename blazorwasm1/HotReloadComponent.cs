#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Components
{
    // For a real implementation, we'd bake this functionality into the renderer, not into a component base class
    public class HotReloadComponentBase : ComponentBase, IDisposable
    {
        public HotReloadComponentBase()
        {
            HotReloadHelper.OnCodeChanged += HandleCodeChange;
        }

        public void Dispose()
        {
            HotReloadHelper.OnCodeChanged -= HandleCodeChange;
        }

        private void HandleCodeChange()
        {
            _ = InvokeAsync(StateHasChanged);
        }
    }

    public static class HotReloadHelper
    {
        public static event Action? OnCodeChanged;

        [JSInvokable]
        public static void ApplyHotReloadDelta(IEnumerable<UpdateDelta> update)
        {
            Console.WriteLine("Applying delta");
            foreach (var item in update)
            {
                Console.WriteLine(Convert.ToBase64String(item.MetadataDelta));
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.Modules.FirstOrDefault() is Module m && m.ModuleVersionId == item.ModuleId);
                if (assembly is not null)
                {
                    Console.WriteLine($"Applying update to {assembly}.");
                    System.Reflection.Metadata.AssemblyExtensions.ApplyUpdate(assembly, item.MetadataDelta, item.ILDelta, ReadOnlySpan<byte>.Empty);
                }
            }

            OnCodeChanged?.Invoke();
        }

        public readonly struct UpdateDelta
        {
            public Guid ModuleId { get; init; }
            public byte[] MetadataDelta { get; init; }
            public byte[] ILDelta { get; init; }
        }
    }
}
