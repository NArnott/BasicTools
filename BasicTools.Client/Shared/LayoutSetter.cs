using System;
using Microsoft.AspNetCore.Components;

namespace BasicTools.Client.Shared
{
    public class LayoutSetter : ComponentBase, IDisposable
    {
        [CascadingParameter]
        MainLayout Layout { get; set; }

        [Parameter]
        public RenderFragment RightSidebar { get; set; }

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
