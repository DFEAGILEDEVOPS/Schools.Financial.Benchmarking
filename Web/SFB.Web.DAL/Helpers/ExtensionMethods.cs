using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using SFB.Web.Common;

namespace SFB.Web.DAL
{
    public static class ExtensionMethods
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IQueryable<T> query)
        {
            var docQuery = query.AsDocumentQuery();
            var batches = new List<IEnumerable<T>>();

            do
            {
                var batch = await docQuery.ExecuteNextAsync<T>();

                batches.Add(batch);
            }
            while (docQuery.HasMoreResults);

            var docs = batches.SelectMany(b => b);

            return docs;
        }

        public static string GetDataGroup(this JObject json)
        {
            if (json.Properties().Any(a => a.Name == "MAT Number") || json.Properties().Any(a => a.Name == "MATNumber"))
                return json.GetProperty("URN", 0, requirements: ValidationIndicators.Number) == 0
                    ? DataGroups.MATCentral
                    : DataGroups.Academies;

            if (json.Properties().Any(a => a.Name == "TypeOfEstablishment"))
                return DataGroups.Edubase;

            if (json.Properties().Any(a => a.Name.StartsWith("Federation")))
                return DataGroups.Maintained;

            if (json.Properties().Any(a => a.Name.StartsWith("Members")))
                return DataGroups.MATOverview;

            if (json.Properties().Any(a => a.Name == "AcademyAllocation"))
                return DataGroups.MATAllocs;

            return DataGroups.Unidentified;
        }

        public static T GetProperty<T>(this JObject jo, string propName, T defaultValue = default(T),
    ValidationIndicators exclusions = ValidationIndicators.Null | ValidationIndicators.Empty,
    ValidationAction exclusionValidationAction = ValidationAction.ReturnDefault,
    ValidationIndicators requirements = ValidationIndicators.None,
    ValidationAction requirementValidationAction = ValidationAction.ReturnDefault
)
        {
            var msg = string.Empty;
            if (jo == null || string.IsNullOrEmpty(propName))
            {
                switch (exclusionValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new ArgumentNullException(nameof(jo));
                }
            }

            var rx = new Regex($"^{propName}$");
            var p = jo.Properties().FirstOrDefault(w => rx.IsMatch(w.Name))?.Name;
            if (p == null && exclusions.HasFlag(ValidationIndicators.Null))
            {
                msg = $"No properties matching {rx}";
                Console.Write(msg);
                switch (exclusionValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new Exception(msg);
                }
            }

            if (jo[p].Type == JTokenType.Null && exclusions.HasFlag(ValidationIndicators.Null))
            {
                msg = $"Property {p} does not exist on that object";
                Console.WriteLine(msg);
                switch (exclusionValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new Exception(msg);
                }
            }

            if (jo[p].Type == JTokenType.Array || jo[p].Type == JTokenType.Object)
            {
                if (typeof(JObject) == typeof(T) || typeof(JArray) == typeof(T))
                    return jo[p].Value<T>();

                return defaultValue;
            }

            var val = jo[p].Value<string>();
            var valFlags = val.GetValidationIndicators();
            if (valFlags.HasFlag(ValidationIndicators.Empty))
            {
                if (exclusions.HasFlag(ValidationIndicators.Empty))
                {
                    msg = $"Property {p} was empty but expected value";
                    Console.WriteLine(msg);
                    switch (exclusionValidationAction)
                    {
                        case ValidationAction.ReturnDefault:
                            return defaultValue;
                        case ValidationAction.Throw:
                            throw new Exception(msg);
                    }
                }
                else
                {
                    return jo[p].Value<T>();
                }
            }

            if (valFlags.HasFlag(ValidationIndicators.Null) && exclusions.HasFlag(ValidationIndicators.Null))
            {
                msg = $"Property {p} was null but expected value";
                Console.WriteLine(msg);

                switch (exclusionValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new Exception(msg);
                }
            }

            if (valFlags.HasFlag(ValidationIndicators.Zero) && exclusions.HasFlag(ValidationIndicators.Zero))
            {
                msg = "Property {p} was 0 but expected value";
                Console.WriteLine(msg);

                switch (exclusionValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new Exception(msg);
                }
            }

            if (valFlags.HasFlag(ValidationIndicators.Negative) && exclusions.HasFlag(ValidationIndicators.Negative))
            {
                msg = $"Property {p} was negative but expected positive integer";
                Console.WriteLine(msg);
                switch (exclusionValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new Exception(msg);
                }
            }

            if (!valFlags.HasFlag(ValidationIndicators.Number) && requirements.HasFlag(ValidationIndicators.Number))
            {
                msg = $"Property {p} was not a number but expected one";
                Console.WriteLine(msg);
                switch (requirementValidationAction)
                {
                    case ValidationAction.ReturnDefault:
                        return defaultValue;
                    case ValidationAction.Throw:
                        throw new Exception(msg);
                }

            }
            return jo[p].Value<T>();
        }

        public static ValidationIndicators GetValidationIndicators(this object oValue)
        {
            double dummyDouble;
            int dummyInt;
            bool dummyBool;
            ValidationIndicators result = 0;

            if (oValue == null)
                return result | ValidationIndicators.Null;

            var value = oValue.ToString();
            if (string.IsNullOrEmpty(value))
                return result | ValidationIndicators.Null;

            if (int.TryParse(value, out dummyInt))
            {
                result |= ValidationIndicators.Number;
                if (dummyInt < 0) result |= ValidationIndicators.Negative;
                if (dummyInt == 0) result |= ValidationIndicators.Zero;
                return result;
            }

            if (double.TryParse(value, out dummyDouble))
            {
                result |= ValidationIndicators.Number;
                result |= ValidationIndicators.Real;
                if (dummyDouble < 0) result |= ValidationIndicators.Negative;
                if (dummyDouble == 0) result |= ValidationIndicators.Zero;
                return result;
            }

            if (bool.TryParse(value, out dummyBool))
            {
                result |= ValidationIndicators.Bool;
                result |= dummyBool ? ValidationIndicators.True : ValidationIndicators.False;
                return result;
            }

            return result;
        }
    }
}
