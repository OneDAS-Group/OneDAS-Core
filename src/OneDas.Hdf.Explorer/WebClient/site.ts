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

async function Connect()
{
    try
    {
        await broadcaster.start()

        try
        {
            let appModel: any
            let connection: any

            appModel = await broadcaster.invoke("GetAppModel")
            appViewModel(new AppViewModel(appModel))

            ko.bindingHandlers.toggleArrow = {
                init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext: KnockoutBindingContext) {
                    $(element).on("click", function () {
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
    }
    catch (e)
    {
        console.log("OneDAS: signalr connection failed")
    }
}

async function Reconnect()
{
    console.log("OneDAS: signalr connection failed")

    if (appViewModel())
    {
        appViewModel().IsConnected(false);
    }

    while (!appViewModel().IsConnected())
    {
        try
        {
            let state: HdfExplorerStateEnum;

            await broadcaster.start()

            state = await broadcaster.invoke("GetHdfExplorerState")
            console.log("OneDAS: signalr reconnected")

            appViewModel().HdfExplorerState(state);
            appViewModel().IsConnected(true);
        }
        catch (e)
        {
            console.log("OneDAS: trying to reconnect ...")
        }

        await delay(5000)
    }
}

broadcaster = new signalR.HubConnection("/broadcaster");
broadcaster.onclose(() => this.Reconnect())

this.Connect()