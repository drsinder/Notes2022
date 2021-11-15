using Microsoft.AspNetCore.Components;
using Notes2022.Shared;
using Syncfusion.Blazor.SplitButtons;
using System.Net.Http.Json;

namespace Notes2022.RCL.User.Comp
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
