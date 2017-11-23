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

$(document).ready(() =>
{
    (<any>$("body")).tooltip({ selector: "[data-toggle=tooltip]", container: "body" });

    (<any>$('#start-date')).datetimepicker(
        {
            format: "DD/MM/YYYY HH:mm",
            minDate: moment.utc('2000-01-01T00:00:00.000Z'),
            maxDate: moment.utc('2030-01-01T00:00:00.000Z'),
            keepOpen: false, // not working
            ignoreReadonly: true,
            timeZone: 'utc'
        }
    );

    (<any>$('#end-date')).datetimepicker(
        {
            format: "DD/MM/YYYY HH:mm",
            minDate: moment.utc('2000-01-01T00:00:00.000Z'),
            maxDate: moment.utc('2030-01-01T00:00:00.000Z'),
            keepOpen: false, // not working
            ignoreReadonly: true,
            timeZone: 'utc'
        }
    );
})

broadcaster = new signalR.HubConnection("broadcaster");

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

        ko.bindingHandlers.dateTimePicker = {
            init: function (element, valueAccessor, allBindingsAccessor)
            {
                //initialize datepicker with some optional options
                var options = allBindingsAccessor().dateTimePickerOptions || {};
                (<any>$(element)).datetimepicker(options);

                //when a user changes the date, update the view model
                ko.utils.registerEventHandler(element, "dp.change", function (event)
                {
                    var value = valueAccessor()
                    if (ko.isObservable(value))
                    {
                        if (event.date != null && !(event.date instanceof Date))
                        {
                            value(event.date.toDate());
                        } else
                        {
                            value(event.date)
                        }
                    }
                });

                ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
                {
                    var picker = $(element).data("DateTimePicker")
                    if (picker)
                    {
                        picker.destroy();
                    }
                })
            },
            update: function (element, valueAccessor, allBindings, viewModel, bindingContext)
            {

                var picker = $(element).data("DateTimePicker")
                //when the view model is updated, update the widget
                if (picker)
                {
                    var koDate = ko.utils.unwrapObservable(valueAccessor())

                    //in case return from server datetime i am get in this form for example /Date(93989393)/ then fomat this
                    koDate = (typeof (koDate) !== 'object') ? new Date(parseFloat(koDate.replace(/[^0-9]/g, ''))) : koDate

                    picker.date(koDate)
                }
            }
        }

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