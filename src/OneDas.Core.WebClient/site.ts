/// <reference path="Static/ConnectionManager.ts"/>
/// <reference path="Static/ExtensionHive.ts"/>

declare var pager: any
declare var _ko: any

let _appViewModel: KnockoutObservable<AppViewModel>
let _componentLoader: KnockoutComponentTypes.Loader

// javascript
//window.onbeforeunload = function () {
//    return "Are you sure to close or reload the page? All unsaved changes will be lost."
//}

// Bootstrap
window.addEventListener("DOMContentLoaded", () =>
{
    (<any>$("body")).tooltip({ selector: '[data-toggle=tooltip]', container: "body" });
    (<any>$("body")).popover({ selector: "[data-toggle=popover]", container: "body", trigger: "focus" });

    $("body").click(() =>
    {
        $("#Project_ChannelContextMenu").hide();
    })
})

// pager workaround
ko.bindingHandlers.page = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)
    {
        var page = null;

        if (ko.utils.unwrapObservable(valueAccessor()) instanceof pager.Page)
        {
            page = ko.utils.unwrapObservable(valueAccessor());
            page.element = element;
            if (page.allBindingsAccessor == null)
            {
                page.allBindingsAccessor = allBindingsAccessor;
            }
            if (page.viewModel == null)
            {
                page.viewModel = viewModel;
            }
            if (page.bindingContext == null)
            {
                page.bindingContext = bindingContext;
            }
        } else
        {
            page = new pager.Page(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
        }

        var result = page.init();

        if (page.isStartPage() && page.parentPage.isVisible() && (!page.parentPage.route || page.parentPage.route.length === 0 || page.parentPage.route[0] === ''))
        {
            setTimeout(function ()
            {
                page.parentPage.showPage('')
            }, 0);
        }

        return result;
    }
}

// knockout
_componentLoader =
    {
        loadTemplate: (name, templateConfig, callback) =>
        {
            let extensionIdentification: ExtensionIdentificationViewModel

            if (templateConfig.ExtensionType && templateConfig.ExtensionIdentification)
            {
                extensionIdentification = templateConfig.ExtensionIdentification

                ConnectionManager.InvokeWebClientHub("GetExtensionStringResource", extensionIdentification.Id, extensionIdentification.ViewResourceName).then(extensionView =>
                {
                    let element: HTMLDivElement

                    if (templateConfig.ExtensionType === "DataGateway" || templateConfig.ExtensionType === "DataWriter")
                    {
                        element = document.createElement("div")
                        element.innerHTML = document.querySelector("#Project_ExtensionTemplate_" + templateConfig.ExtensionType).innerHTML
                        element.querySelector("#Project_ExtensionTemplate_Content").innerHTML = extensionView

                        callback([element])
                    }
                    else
                    {
                        ko.components.defaultLoader.loadTemplate(name, extensionView, callback)
                    }
                }).catch(() =>
                {
                    callback(null)
                })
            }
            else
            {
                callback(null)
            }
        }
    }

ko.selectExtensions.readValue = (element) => // for "value" binding in combination with "type=number"
{
    var anyElement: any
    var hasDomDataExpandoProperty = '__ko__hasDomDataOptionValue__';

    anyElement = element

    switch ((<any>ko.utils).tagNameLower(element))
    {
        case 'option':
            if (element[hasDomDataExpandoProperty] === true)
                return ko.utils.domData.get(element, ko.bindingHandlers.options.optionValueDomDataKey);
            return anyElement.value;
        case 'select':
            return anyElement.selectedIndex >= 0 ? ko.selectExtensions.readValue(anyElement.options[anyElement.selectedIndex]) : undefined;
        case 'input':
            if (anyElement.getAttribute("type") === "number")
            {
                return anyElement.valueAsNumber;
            }
            else
            {
                return anyElement.value;
            }
        default:
            return anyElement.value;
    }
}

ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor)
    {
        let value = valueAccessor()
        $(element).hide()
    },
    update: function (element, valueAccessor)
    {
        var value = valueAccessor()
        ko.unwrap(value) ? $(element).fadeIn(750) : $(element).fadeOut(750)
    }
}

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

ko.bindingHandlers.dynamicTooltip = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext: KnockoutBindingContext)
    {
        (<any>$(element).attr('title', ko.unwrap(valueAccessor()))).tooltip('_fixTitle')
    }
}

ko.bindingHandlers.tooltip = {
    init: function (element, valueAccessor)
    {
        var local = ko.utils.unwrapObservable(valueAccessor())
        var options = {}

        ko.utils.extend(options, ko.bindingHandlers.tooltip.options)
        ko.utils.extend(options, local);

        (<any>$(element)).tooltip(options)

        ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
        {
            (<any>$(element)).tooltip("hide")
        })
    },
    update: function (element, valueAccessor)
    {
        var local = ko.utils.unwrapObservable(valueAccessor())
        var options = {}

        ko.utils.extend(options, ko.bindingHandlers.tooltip.options)
        ko.utils.extend(options, local)
        $(element).attr('data-original-title', (<any>options).title);
        (<any>$(element)).tooltip(options)
    },
    options: {
        placement: "top",
        trigger: "hover"
    }
}

ko.bindingHandlers.numericText = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext)
    {
        var value: number = ko.utils.unwrapObservable(valueAccessor())
        var precision: number = ko.utils.unwrapObservable(allBindingsAccessor().precision)
        var formattedValue: string = value.toFixed(precision)

        if (allBindingsAccessor().unit)
        {
            ko.bindingHandlers.text.update(element, function () { return formattedValue + " " + allBindingsAccessor().unit }, allBindingsAccessor, viewModel, bindingContext)
        }
        else
        {
            ko.bindingHandlers.text.update(element, function () { return formattedValue }, allBindingsAccessor, viewModel, bindingContext)
        }
    }
}

async function Connect()
{
    let appModel: any

    while (true)
    {
        try
        {
            await ConnectionManager.WebClientHub.start()

            console.log("OneDAS: signalr connected")
            appModel = await ConnectionManager.WebClientHub.invoke("GetAppModel")

            break
        }
        catch (ex)
        {
            console.log("OneDAS: trying to reconnect ...")

            await delay(5000)
        }
    }

    if (_appViewModel())
    {
        _appViewModel().Update(appModel)
        _appViewModel().IsConnected(true)
        console.log("OneDAS: appViewModel updated")
    }
    else
    {
        _appViewModel(new AppViewModel(appModel))
        console.log("OneDAS: appViewModel created")

        this.InitializeApp()
    }
}

function InitializeApp()
{
    // pagerjs
    pager.Href5.hash = ""
    pager.useHTML5history = true
    pager.Href5.history = window.history
    pager.extendWithPage(_appViewModel())

    console.log("OneDAS: pagerjs configured")

    // history
    var pushState = window.history.pushState

    window.history.pushState = function (state, title, url)
    {
        pushState.apply(history, [state, title, url])
        dispatchEvent(new PopStateEvent('popstate', { state: state }))
    }

    window.onpopstate = function (event)
    {
        pager.goTo(window.location.pathname.substr(1))
    }

    console.log("OneDAS: HTML5 history configured")

    // Knockout
    ko.components.loaders.unshift(_componentLoader)
    ko.applyBindings(_appViewModel)

    console.log("OneDAS: Knockout configured")

    // pagerjs
    pager.goTo(window.location.pathname.substr(1))
}

ExtensionHive.Initialize()

ConnectionManager.Initialize(false)

ConnectionManager.WebClientHub.onclose(() =>
{
    console.log("OneDAS: signalr connection failed")

    if (_appViewModel())
    {
        _appViewModel().IsConnected(false)
    }

    this.Connect()
})

// start app
_appViewModel = ko.observable<AppViewModel>()

this.Connect()
