using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Components;

[assembly: AssemblyMetadata("ReceiveHotReloadDeltaNotification", "Microsoft.AspNetCore.Components.HotReloadHelper")]

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
        public static event Action OnCodeChanged;
        public static void DeltaApplied()
        {
            OnCodeChanged?.Invoke();
        }
    }
}
