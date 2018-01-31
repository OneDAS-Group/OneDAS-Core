class EnumerationHelper
{
    public static Description: { [index: string]: string } = {}

    public static GetEnumLocalization = (typeName: string, value) =>
    {
        var key: string = eval(typeName + "[" + value + "]")
        return eval("EnumerationHelper.Description['" + typeName + "_" + key + "']")
    }

    public static GetEnumValues = (typeName: string) =>
    {
        let values: any[]

        values = eval("Object.keys(" + typeName + ").map(key => " + typeName + "[key])")
        return <number[]>values.filter(value => typeof (value) === "number")
    }
}