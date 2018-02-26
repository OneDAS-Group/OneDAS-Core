declare var signalR: any

let appViewModel: KnockoutObservable<AppViewModel> = ko.observable<AppViewModel>()
let broadcaster: any

window.addEventListener("DOMContentLoaded", () =>
{
    (<any>$("body")).tooltip({ selector: "[data-toggle=tooltip]", container: "body" });

    $(function ()
    {
        (<any>$('[data-toggle="tooltip"]')).tooltip()
    })

    $(function ()
    {
        let startDate: Date

        startDate = new Date()
        startDate.setHours(0, 0, 0, 0);
        startDate = addDays(startDate, -1);

        let endDate: Date

        endDate = new Date()
        endDate.setHours(0, 0, 0, 0);

        (<any>$("#start-date")).datetimepicker(
            {
                format: "DD/MM/YYYY HH:mm",
                minDate: new Date("2000-01-01T00:00:00.000Z"),
                maxDate: new Date("2030-01-01T00:00:00.000Z"),
                defaultDate: startDate,
                ignoreReadonly: true,
                //calendarWeeks: true // not working
            }
        );

        (<any>$("#end-date")).datetimepicker(
            {
                format: "DD/MM/YYYY HH:mm",
                minDate: new Date("2000-01-01T00:00:00.000Z"),
                maxDate: new Date("2030-01-01T00:00:00.000Z"),
                defaultDate: endDate,
                ignoreReadonly: true,
                //calendarWeeks: true // not working
            }
        );

        (<any>$("#start-date")).on("change.datetimepicker", function (e)
        {
            (<any>$('#end-date')).datetimepicker('minDate', e.date);
        });

        (<any>$("#end-date")).on("change.datetimepicker", function (e)
        {
            (<any>$('#start-date')).datetimepicker('maxDate', e.date);
        });
    });
})

broadcaster = new signalR.HubConnection("/broadcaster");

broadcaster.onclose(async error =>
{
    console.log("OneDAS: signalr connection failed")

    if (appViewModel())
    {
        appViewModel().IsConnected(false);
    }

    while (!appViewModel().IsConnected())
    {
        broadcaster.start().then(async () =>
        {
            console.log("OneDAS: signalr reconnected")  
            appViewModel().IsConnected(true);
        }).catch(() =>
        {
            console.log("OneDAS: trying to reconnect ...")
        })

        await delay(5000)
    }
})

broadcaster.start().then(async () =>
{
    let appModel: any
    let connection: any

    try
    {
        appModel = await broadcaster.invoke("GetAppModel")
        appViewModel(new AppViewModel(appModel))

        ko.bindingHandlers.toggleArrow = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext: KnockoutBindingContext)
            {
                $(element).on("click", function ()
                {
                    $(".fa", this)
                        .toggleClass("fa-caret-right")
                        .toggleClass("fa-caret-down")
                })
            }
        }

        ko.applyBindings(appViewModel)
    }
    catch (e)
    {
        alert(e.message)
    }
}).catch(() =>
{
    console.log("OneDAS: signalr connection failed")
})