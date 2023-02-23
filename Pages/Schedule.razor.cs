using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using StudioMgn;
using StudioMgn.Shared;
using Radzen;
using Radzen.Blazor;
using StudioMgn.Models;
using System.Globalization;
using StudioMgn.Services;

namespace StudioMgn.Pages
{
    public partial class Schedule
    {
        RadzenScheduler<Appointment> scheduler;
        IList<Appointment> appointments = new List<Appointment>();
        protected override async Task OnInitializedAsync()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            await service.GetLocalCollectionAsync();
            appointments = service.LocalCollection;
        }

        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
            {
                args.Attributes["style"] = "background:rgba(255, 220, 40, .2);";
            }
        }

        async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {
            console.LogInformation($"=== SlotSelect: Start={args.Start} End={args.End}");
            Appointment data = await dialogService.OpenAsync<AddAppointment>("Записаться в студию", new Dictionary<string, object> { { "Start", args.Start }, { "End", args.End } });
            console.LogInformation($"=== {data}");
            if (data != null)
            {
                await service.AddAsync(data);
                await Refresh();
            }
        }

        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
        {
            console.LogInformation($"AppointmentSelect: Appointment={args.Data.Type}");
        }

        void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Appointment> args)
        {
            // Never call StateHasChanged in AppointmentRender - would lead to infinite loop
            //if (args.Data.Type == AppointmentType.ЗАПИСЬ)
            //{
            //    args.Attributes["style"] = "background: red";
            //}
        }

        async Task Refresh()
        {
            await service.GetLocalCollectionAsync();
            await scheduler.Reload();
        }
    }
}