using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Notes2022.Client;
using Notes2022.Client.Shared;
using Notes2022.Client.Pages.User.Menus;
using Notes2022.Client.Pages.User.Panels;
using Notes2022.Shared;
using Blazored;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.SessionStorage;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.LinearGauge;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.SplitButtons;

namespace Notes2022.Client.Pages.User
{

    public partial class TrackerMover
    {
        [Parameter] public Sequencer CurrentTracker { get; set; }

        [Parameter] public List<Sequencer> Trackers { get; set; }
        [Parameter] public Tracker Tracker { get; set; }

        List<Sequencer> befores { get; set; }
        List<Sequencer> afters { get; set; }

        Sequencer before { get; set; }
        Sequencer after { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            if (CurrentTracker != null)
            {
                befores = Trackers.Where(p => p.Ordinal < CurrentTracker.Ordinal).OrderByDescending(p => p.Ordinal).ToList();
                if (befores != null && befores.Count > 0)
                    before = befores.First();

                afters = Trackers.Where(p => p.Ordinal > CurrentTracker.Ordinal).OrderBy(p => p.Ordinal).ToList();
                if (afters != null && afters.Count > 0)
                    after = afters.First();
            }
        }

        private async Task ItemSelected(MenuEventArgs args)
        {

            switch (args.Item.Text)
            {
                case "Up":
                    if (before == null)
                        return;

                    await Swap(before, CurrentTracker);

                    break;

                case "Down":
                    if (after == null)
                        return;
                    await Swap(after, CurrentTracker);

                    break;

                case "Top":
                    if (before == null)
                        return;
                    await Swap(befores[befores.Count - 1], CurrentTracker);
                    break;

                case "Bottom":
                    if (after == null)
                        return;
                    await Swap(afters[afters.Count - 1], CurrentTracker);

                    break;

                default:
                    break;
            }

            await Tracker.Shuffle();

        }


        private async Task Swap(Sequencer a, Sequencer b)
        {
            int aord = a.Ordinal;
            int bord = b.Ordinal;

            a.Ordinal = bord;
            b.Ordinal = aord;

            await Http.PutAsJsonAsync("api/sequenceredit", a);
            await Http.PutAsJsonAsync("api/sequenceredit", b);

        }
    }
}
