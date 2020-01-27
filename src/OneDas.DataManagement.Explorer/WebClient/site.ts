declare var signalR: any

let _appViewModel: KnockoutObservable<AppViewModel> = ko.observable<AppViewModel>()
let _broadcaster: any

window.addEventListener("DOMContentLoaded", () =>
{
    (<any>$("body")).tooltip({ selector: "[data-toggle=tooltip]", container: "body" })

    $(function ()
    {
        (<any>$('[data-toggle="tooltip"]')).tooltip()
    })
})

ko.bindingHandlers.callFunction = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext: KnockoutBindingContext)
    {
        let functionSet = ko.unwrap(valueAccessor())

        if (Array.isArray(functionSet))
        {
            functionSet.forEach(x =>
            {
                x(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)
            })
        }
        else
        {
            functionSet(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)
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

async function Connect()
{
    let appModel: any
    let connection: any

    try
    {
        await _broadcaster.start()

        console.log("HDF Explorer: signalr connected")

        try
        {
            appModel = await _broadcaster.invoke("GetAppModel")
            _appViewModel(new AppViewModel(appModel))

            console.log("HDF Explorer: app model received")

            ko.applyBindings(_appViewModel)
        }
        catch (e)
        {
            alert(e.message)
        }
    }
    catch (e)
    {
        console.log("HDF Explorer: signalr connection failed")
    }
}

async function Reconnect()
{
    let state: HdfExplorerStateEnum

    console.log("HDF Explorer: signalr connection failed")

    if (_appViewModel())
    {
        _appViewModel().IsConnected(false)
    }

    while (!_appViewModel() || !_appViewModel().IsConnected())
    {
        try
        {
            await _broadcaster.start()

            console.log("HDF Explorer: signalr reconnected")

            state = await _broadcaster.invoke("GetHdfExplorerState")

            _appViewModel().HdfExplorerState(state)
            _appViewModel().IsConnected(true)
        }
        catch
        {
            console.log("HDF Explorer: trying to reconnect ...")
        }

        await delay(5000)
    }
}

_broadcaster = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl('/broadcaster')
    .build();
_broadcaster.onclose(() => this.Reconnect())

this.Connect()