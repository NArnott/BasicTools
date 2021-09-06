using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace BasicTools.Client.Shared
{
    partial class MainLayout
    {
        readonly MudTheme DefaultTheme = new();

        protected override void OnInitialized()
        {
            DefaultTheme.Palette.Background = new MudColor("#FAFAFA");
        }

        #region Nav Drawer

        bool _navDrawerOpen = true;

        void NavDrawerToggle()
        {
            _navDrawerOpen = !_navDrawerOpen;
        }

        #endregion

        #region Page Info Drawer

        bool _pageInfoDrawerOpen = true;

        bool PageInfoDrawerOpen
        {
            get => _pageInfoContents != null && _pageInfoDrawerOpen;
            set
            {
                if (_pageInfoContents != null)
                    _pageInfoDrawerOpen = value;
            }
        }

        private RenderFragment? _pageInfoContents;

        #endregion

        public void FullExpand()
        {
            if (PageInfoDrawerOpen || _navDrawerOpen)
            {
                PageInfoDrawerOpen = false;
                _navDrawerOpen = false;
            }
            else
            {
                PageInfoDrawerOpen = true;
                _navDrawerOpen = true;
            }
        }

        public void SetRenderFragments(RenderFragment pageInfoContents)
        {
            _pageInfoContents = pageInfoContents;
            StateHasChanged();
        }

        public void ClearRenderFragments(RenderFragment pageInfoContents)
        {
            InvokeAsync(() =>
            {
                if (_pageInfoContents == pageInfoContents)
                {
                    _pageInfoContents = null;
                    StateHasChanged();
                }
            });
        }
    }
}
