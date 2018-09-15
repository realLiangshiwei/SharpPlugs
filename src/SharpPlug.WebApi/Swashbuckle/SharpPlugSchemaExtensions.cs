using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharpPlug.WebApi.Swashbuckle
{
    public static class SharpPlugSchemaExtensions
    {

        public static SchemaRegistrySettings Clone(this SchemaRegistrySettings settings)
        {
            var s = new SchemaRegistrySettings
            {
                DescribeAllEnumsAsStrings = settings.DescribeAllEnumsAsStrings,
                DescribeStringEnumsInCamelCase = settings.DescribeStringEnumsInCamelCase,
                IgnoreObsoleteProperties = settings.IgnoreObsoleteProperties,
                SchemaIdSelector = settings.SchemaIdSelector,
            };
            foreach (var typeMap in settings.CustomTypeMappings)
            {
                s.CustomTypeMappings.Add(typeMap.Key, typeMap.Value);
            }
            foreach (var schemaFilter in settings.SchemaFilters)
            {
                s.SchemaFilters.Add(schemaFilter);
            }
            return s;
        }

        public static SwaggerGeneratorSettings Clone(this SwaggerGeneratorSettings setting)
        {
            var s = new SwaggerGeneratorSettings
            {
                SwaggerDocs = setting.SwaggerDocs,
                DocInclusionPredicate = setting.DocInclusionPredicate,
                IgnoreObsoleteActions = setting.IgnoreObsoleteActions,
                TagSelector = setting.TagSelector,
                SortKeySelector = setting.SortKeySelector,
                DescribeAllParametersInCamelCase = setting.DescribeAllParametersInCamelCase,

            };
            foreach (var def in setting.SecurityDefinitions)
            {
                s.SecurityDefinitions.Add(def.Key, def.Value);
            }
            foreach (var operationFilter in setting.OperationFilters)
            {
                s.OperationFilters.Add(operationFilter);
            }
            foreach (var documentFilter in setting.DocumentFilters)
            {
                s.DocumentFilters.Add(documentFilter);
            }
            return s;
        }
        internal static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        internal static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        internal static PropertyInfo PropertyInfo(this JsonProperty jsonProperty)
        {
            if (jsonProperty.UnderlyingName == null) return null;

            var metadata = jsonProperty.DeclaringType.GetTypeInfo()
                .GetCustomAttributes(typeof(ModelMetadataTypeAttribute), true)
                .FirstOrDefault();

            var typeToReflect = (metadata != null)
                ? ((ModelMetadataTypeAttribute)metadata).MetadataType
                : jsonProperty.DeclaringType;

            return typeToReflect.GetProperty(jsonProperty.UnderlyingName, jsonProperty.PropertyType);
        }

        internal static Schema AssignValidationProperties(this Schema schema, JsonProperty jsonProperty)
        {
            var propInfo = jsonProperty.PropertyInfo();
            if (propInfo == null)
                return schema;

            foreach (var attribute in propInfo.GetCustomAttributes(false))
            {
                if (attribute is DefaultValueAttribute defaultValue)
                    schema.Default = defaultValue.Value;

                if (attribute is RegularExpressionAttribute regex)
                    schema.Pattern = regex.Pattern;

                if (attribute is RangeAttribute range)
                {
                    if (Int32.TryParse(range.Maximum.ToString(), out var maximum))
                        schema.Maximum = maximum;

                    if (Int32.TryParse(range.Minimum.ToString(), out var minimum))
                        schema.Minimum = minimum;
                }

                if (attribute is MinLengthAttribute minLength)
                    schema.MinLength = minLength.Length;

                if (attribute is MaxLengthAttribute maxLength)
                    schema.MaxLength = maxLength.Length;

                if (attribute is StringLengthAttribute stringLength)
                {
                    schema.MinLength = stringLength.MinimumLength;
                    schema.MaxLength = stringLength.MaximumLength;
                }
            }

            if (!jsonProperty.Writable)
                schema.ReadOnly = true;

            return schema;
        }

        internal static void PopulateFrom(this PartialSchema partialSchema, Schema schema)
        {
            if (schema == null) return;

            partialSchema.Type = schema.Type;
            partialSchema.Format = schema.Format;

            if (schema.Items != null)
            {
                // TODO: Handle jagged primitive array and error on jagged object array
                partialSchema.Items = new PartialSchema();
                partialSchema.Items.PopulateFrom(schema.Items);
            }

            partialSchema.Default = schema.Default;
            partialSchema.Maximum = schema.Maximum;
            partialSchema.ExclusiveMaximum = schema.ExclusiveMaximum;
            partialSchema.Minimum = schema.Minimum;
            partialSchema.ExclusiveMinimum = schema.ExclusiveMinimum;
            partialSchema.MaxLength = schema.MaxLength;
            partialSchema.MinLength = schema.MinLength;
            partialSchema.Pattern = schema.Pattern;
            partialSchema.MaxItems = schema.MaxItems;
            partialSchema.MinItems = schema.MinItems;
            partialSchema.UniqueItems = schema.UniqueItems;
            partialSchema.Enum = schema.Enum;
            partialSchema.MultipleOf = schema.MultipleOf;
        }
    }
}
