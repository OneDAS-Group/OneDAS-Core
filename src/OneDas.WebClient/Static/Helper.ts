class ObservableGroup<T>
{
    Key: string;
    Members: KnockoutObservableArray<T>

    constructor(key: string, members: T[] = new Array<T>())
    {
        this.Key = key
        this.Members = ko.observableArray(members)
    }
}

function ObservableGroupBy<T>(list: T[], nameGetter: (x: T) => string, groupNameGetter: (x: T) => string, filter: string): ObservableGroup<T>[]
{
    let result: ObservableGroup<T>[]
    let regExp: RegExp

    result = []
    regExp = new RegExp(filter, "i")

    list.forEach(element =>
    {
        if (regExp.test(nameGetter(element)))
        {
            groupNameGetter(element).split("\n").forEach(groupName =>
            {
                AddToGroupedArray(element, groupName, result)
            })
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

//function AddToGroupedObservableArray<T>(item: T, groupName: string, observableGroupSet: KnockoutObservableArray<ObservableGroup<T>>)
//{
//    let group: ObservableGroup<T>

//    group = observableGroupSet().find(y => y.Key === groupName)

//    if (!group)
//    {
//        group = new ObservableGroup<T>(groupName)
//        observableGroupSet.push(group)
//    }

//    group.Members.push(item)
//}

//function RemoveFromGroupedObservableArray<T>(item: T, observableGroupSet: KnockoutObservableArray<ObservableGroup<T>>)
//{
//    var group: ObservableGroup<T>

//    observableGroupSet().some(x =>
//    {
//        if (x.Members().indexOf(item) > -1)
//        {
//            group = x

//            return true
//        }

//        return false
//    })

//    if (group)
//    {
//        group.Members.remove(item)

//        if (group.Members().length === 0)
//        {
//            observableGroupSet.remove(group)
//        }

//        return true
//    }

//    return false
//}

//function UpdateGroupedObservableArray<T>(item: T, groupName: string, observableGroupSet: KnockoutObservableArray<ObservableGroup<T>>)
//{
//    RemoveFromGroupedObservableArray(item, observableGroupSet)
//    AddToGroupedObservableArray(item, groupName, observableGroupSet)
//}

function MapMany<TArrayElement, TSelect>(array: TArrayElement[], mapFunc: (item: TArrayElement) => TSelect[]): TSelect[]
{
    return array.reduce((previous, current, i) =>
    {
        return previous.concat(mapFunc(current));
    }, <TSelect[]>[]);
}

class Guid
{
    static NewGuid()
    {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c)
        {
            var r = Math.random() * 16 | 0
            var v = c === 'x' ? r : (r & 0x3 | 0x8)

            return v.toString(16)
        })
    }
}

let CheckNamingConvention = (value: string) =>
{
    var regExp: any

    if (value.length === 0)
    {
        return { HasError: true, ErrorDescription: ErrorMessage["Project_NameEmpty"] }
    }

    regExp = new RegExp("[^A-Za-z0-9_]")

    if (regExp.test(value))
    {
        return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidCharacters"] }
    }

    regExp = new RegExp("^[0-9_]")

    if (regExp.test(value))
    {
        return { HasError: true, ErrorDescription: ErrorMessage["Project_InvalidLeadingCharacter"] }
    }

    return {
        HasError: false,
        ErrorDescription: ""
    }
}