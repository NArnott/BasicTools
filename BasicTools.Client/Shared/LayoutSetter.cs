using System;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Client.Shared
{
    public sealed class LayoutSetter : ComponentBase, IDisposable
    {
        [CascadingParameter]
        MainLayout Layout { get; set; } = default!;

        [Parameter]
        public RenderFragment RightSidebar { get; set; } = default!;

        protected override void OnInitialized()
        {
            Layout.SetRenderFragments(RightSidebar);
        }

        public void Dispose()
        {
            Layout.ClearRenderFragments(RightSidebar);
        }
    }
}
