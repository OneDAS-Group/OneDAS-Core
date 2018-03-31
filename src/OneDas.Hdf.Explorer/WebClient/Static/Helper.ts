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

function delay(ms: number)
{
    return new Promise(resolve => setTimeout(resolve, ms));
}

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