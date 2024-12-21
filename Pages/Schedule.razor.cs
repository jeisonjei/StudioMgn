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
using System.Drawing;

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
            await appointmentsService.GetLocalCollectionAsync();
            appointments = appointmentsService.LocalCollection;
        }

        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
            {
                args.Attributes["style"] = "background:rgba(255, 220, 40, .2);";
            }
            if (args.View.Text=="Month" && args.Start>DateTime.Today.AddDays(14))
            {
                args.Attributes["style"] = "background:rgba(150,150,150,.5)";
            }
            if (args.View.Text=="Week" &&args.Start>DateTime.Today.AddDays(14))
            {
                args.Attributes["style"] = "background:rgba(150,150,150,.5)";
            }
            if (args.View.Text == "Day" && args.Start > DateTime.Today.AddDays(14))
            {
                args.Attributes["style"] = "background:rgba(150,150,150)";
            }
        }

        async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {
            if (args.Start > DateTime.Today.AddDays(14))
            {
                return;
            }
            console.LogInformation($"=== SlotSelect: Start={args.Start} End={args.End}");
            var currentDate=args.Start.ToLongDateString();
            var currentWeekDay = DateTimeFormatInfo.CurrentInfo.GetDayName(args.Start.DayOfWeek);
            Appointment data = await dialogService.OpenAsync<AddAppointment>($"Записаться в студию на {currentDate}", new Dictionary<string, object> { { "Start", args.Start }, { "End", args.End } });
            console.LogInformation($"=== {data}");
            if (data != null)
            {
                await appointmentsService.AddAsync(data);
                await Refresh();
                // Отправка сообщения
                string message = string.Empty;
                string name = string.Empty;
                string phone = data.Phone;
                string type=data.Type.ToString();
                string comment=data.Comment;
                if (string.IsNullOrEmpty(data.Name)||string.IsNullOrWhiteSpace(data.Name)||data.Name.Length<2)
                {
                    name = "Имя не указано";
                }
                else
                {
                    name=data.Name;
                }
                message = $"<table style=\"border-collapse:collapse;border-spacing:0\" class=\"tg\"><tbody><tr><td style=\"background-color:#ebebeb;border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">Имя</td><td style=\"border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">{name}</td></tr><tr><td style=\"background-color:#ebebeb;border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">Телефон</td><td style=\"border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;font-weight:bold;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">{phone}</td></tr><tr><td style=\"background-color:#ebebeb;border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">Событие</td><td style=\"border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">{type}</td></tr><tr><td style=\"background-color:#ebebeb;border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">Комментарий</td><td style=\"border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">{comment}</td></tr><tr><td style=\"background-color:#ebebeb;border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">Дата</td><td style=\"border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">{currentDate}</td></tr><tr><td style=\"background-color:#ebebeb;border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">Время</td><td style=\"border-color:black;border-style:solid;border-width:1px;font-family:Arial,sans-serif;font-size:14px;overflow:hidden;padding:5px5px;text-align:left;vertical-align:top;word-break:normal\">{data.Start.ToShortTimeString()}-{data.End.ToShortTimeString()}</td></tr></tbody></table><div><a href=\"http://studio-mgn.ru/schedule\">studio-mgn.ru</a></div>";
                // await emailService.SendEmailAsync("sky_jet@mail.ru","Новая запись в расписании",message);
            }
        }

        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
        {
            console.LogInformation($"AppointmentSelect: Appointment={args.Data.Type}");
            await dialogService.OpenAsync<SelectAction>("Удаление события",new Dictionary<string, object> { { "appointment",args.Data} },new DialogOptions { Width="auto"});
            await Refresh();
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
            await appointmentsService.GetLocalCollectionAsync();
            appointments = appointmentsService.LocalCollection;
            await scheduler.Reload();
        }
    }
}