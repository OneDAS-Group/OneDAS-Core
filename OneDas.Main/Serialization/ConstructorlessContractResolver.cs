using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;

namespace OneDas.Main.Serialization
{
    public class ConstructorlessContractResolver : DefaultContractResolver
    {
        public override JsonContract ResolveContract(Type type)
        {
            JsonContract contract = base.ResolveContract(type);

            contract.IsReference = false;

            return contract;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            // prepare contract using default resolver
            var objectContract = base.CreateObjectContract(objectType);

            // if type has constructor marked with JsonConstructor attribute or can't be instantiated, return default contract
            if (objectContract.OverrideCreator != null || objectContract.CreatedType.IsInterface || objectContract.CreatedType.IsAbstract)
            {
                return objectContract;
            }

            // prepare function to check that specified constructor parameter corresponds to non writable property on a type
            Func<JsonProperty, bool> isParameterForNonWritableProperty = parameter =>
            {
                var propertyForParameter = objectContract.Properties.FirstOrDefault(property => property.PropertyName == parameter.PropertyName);

                if (propertyForParameter == null)
                    return false;

                return !propertyForParameter.Writable;
            };

            // if type has parameterized constructor and any of constructor parameters corresponds to non writable property, return default contract
            // this is needed to handle special cases for types that can be initialized only via constructor, i.e. Tuple<>
            if (objectContract.OverrideCreator != null && objectContract.CreatorParameters.Any(parameter => isParameterForNonWritableProperty(parameter)))
            {
                return objectContract;
            }

            // override default creation method to create object without constructor call
            objectContract.DefaultCreatorNonPublic = false;
            objectContract.DefaultCreator = () => FormatterServices.GetSafeUninitializedObject(objectContract.CreatedType);

            return objectContract;
        }
    }
}
