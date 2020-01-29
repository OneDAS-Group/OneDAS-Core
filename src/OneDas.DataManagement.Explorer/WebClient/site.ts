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

    try
    {
        await _broadcaster.start()

        console.log("OneDAS Explorer: signalr connected")

        try
        {
            console.log("OneDAS Explorer: before app model")
            appModel = await _broadcaster.invoke("GetAppModel")
            _appViewModel(new AppViewModel(appModel))

            console.log("OneDAS Explorer: app model received")

            ko.applyBindings(_appViewModel)
        }
        catch (e)
        {
            alert(e.message)
        }
    }
    catch (e)
    {
        console.log("OneDAS Explorer: signalr connection failed")
    }
}

async function Reconnect()
{
    let state: OneDasExplorerStateEnum

    console.log("OneDAS Explorer: signalr connection failed")

    if (_appViewModel())
    {
        _appViewModel().IsConnected(false)
    }

    while (!_appViewModel() || !_appViewModel().IsConnected())
    {
        try
        {
            await _broadcaster.start()

            console.log("OneDAS Explorer: signalr reconnected")

            state = await _broadcaster.invoke("GetState")

            _appViewModel().ExplorerState(state)
            _appViewModel().IsConnected(true)
        }
        catch
        {
            console.log("OneDAS Explorer: trying to reconnect ...")
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