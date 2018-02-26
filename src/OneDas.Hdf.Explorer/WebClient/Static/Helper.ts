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