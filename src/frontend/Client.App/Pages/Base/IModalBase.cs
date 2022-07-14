using MudBlazor;
using System;

namespace Client.App.Pages.Base
{
    public interface IModalBase
    {
        public bool IsProcessing { get; set; }
        public bool IsLoaded { get; set; }
    }
}
