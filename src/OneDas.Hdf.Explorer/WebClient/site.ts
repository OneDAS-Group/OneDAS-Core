declare var signalR: any

let appViewModel: KnockoutObservable<AppViewModel> = ko.observable<AppViewModel>()
let broadcaster: any

interface Map<K, V>
{
    toJSON(): any;
}

Map.prototype.toJSON = function ()
{
    var obj = {}

    for (let [key, value] of this)
        obj[key] = value

    return obj
}

class ObservableGroup<T>
{
    Key: string;
    Members: KnockoutObservableArray<T>

    constructor(key: string)
    {
        this.Key = key
        this.Members = ko.observableArray([])
    }
}

function ObservableGroupBy<T>(list: T[], nameGetter: (x: T) => string, groupNameGetter: (x: T) => string, filter: string): ObservableGroup<T>[]
{
    let result: ObservableGroup<T>[]

    result = []

    list.forEach(x =>
    {
        if (nameGetter(x).indexOf(filter) > -1)
        {
            AddToGroupedArray(x, groupNameGetter(x), result)
        }
    })

    return result
}

function AddToGroupedArray<T>(item: T, groupName: string, observableGroupSet: ObservableGroup<T>[])
{
    let group: ObservableGroup<T>

    group = observableGroupSet.find(y => y.Key === groupName)

    if (!group)
    {
        group = new ObservableGroup<T>(groupName)
        observableGroupSet.push(group)
    }

    group.Members.push(item)
}

function MapMany<TArrayElement, TSelect>(array: TArrayElement[], mapFunc: (item: TArrayElement) => TSelect[]): TSelect[]
{
    return array.reduce((previous, current, i) =>
    {
        return previous.concat(mapFunc(current));
    }, <TSelect[]>[]);
}

function addDays(date, days)
{
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

$(document).ready(() =>
{
    (<any>$("body")).tooltip({ selector: "[data-toggle=tooltip]", container: "body" });

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

// improve: how to handle this?

//let options:signalR.HttpConnection

//new signalR.HubConnection(new signalR.HttpConnection(urlOrConnection, options).).on("disconnect", () =>
//{
//    debugger

//    //$.connection.hub.start();

//    //if ($.connection.hub.state == 0)
//    //{
//    //    console.log("OneDAS: signalr reconnected")
//    //}
//    //else
//    //{
//    //    console.log("OneDAS: signalr disconnected")
//    //}
//})

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