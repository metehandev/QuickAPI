using Microsoft.OpenApi.Models;
using MY.QuickAPI.Database.Core;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MY.QuickAPI.Core;

internal class DataSourceLoadOptionsParametersFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // 1) Loop through each path in swaggerDoc
        foreach (var (_, pathItem) in swaggerDoc.Paths)
        {
            // 2) For each path item, loop through its operations
            foreach (var (_, operation) in pathItem.Operations)
            {
                // 3) Check if this operation uses BindableDataSourceLoadOptions
                //    Typically you can look at operation.Parameters or operation.RequestBody,
                //    but in minimal APIs, it might not show up automatically.
                //    So we do a name-based check or some other heuristic:

                // For example, if your endpoint is named "GetMany" or route is "/api/items", 
                // you can check for that:
                if (!(operation.Description?.Contains(nameof(CrudOperation.GetMany)) ?? false))
                    continue;

                CreateDataSourceLoadOptionsParameters(operation);
            }
        }
    }

    private static void CreateDataSourceLoadOptionsParameters(OpenApiOperation operation)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        
        // 0) IncludeFields (string array)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "includeFields",
            In = ParameterLocation.Query,
            Required = false,
            Description =
                "Include fields for requested Data Model. Example: Navigation1",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema { Type = "string" }
            }
        });

        // 1) requireTotalCount (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "requireTotalCount",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If set to true, the total count of items is returned (DevExtreme option).",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 2) requireGroupCount (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "requireGroupCount",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, returns the group count (DevExtreme option).",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 3) isCountQuery (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "isCountQuery",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, indicates this is only a count query.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 4) isSummaryQuery (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "isSummaryQuery",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, indicates this is only a summary query.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 5) skip (int)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "skip",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Number of items to skip (paging).",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Format = "int32"
            }
        });

        // 6) take (int)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "take",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Number of items to take (paging).",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Format = "int32"
            }
        });

        // 7) sort (array of { selector: string, desc: bool })
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "sort",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Sort instructions. Example: [ { \"selector\": \"Name\", \"desc\": true } ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["selector"] = new() { Type = "string" },
                        ["desc"] = new() { Type = "boolean" }
                    }
                }
            }
        });

        // 8) group (array of { selector: string, desc: bool, groupInterval: string, isExpanded: bool })
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "group",
            In = ParameterLocation.Query,
            Required = false,
            Description =
                "Grouping instructions. Example: [ { \"selector\": \"Category\", \"desc\": false, \"groupInterval\": \"\", \"isExpanded\": true } ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["selector"] = new() { Type = "string" },
                        ["desc"] = new() { Type = "boolean" },
                        ["groupInterval"] = new() { Type = "string" },
                        ["isExpanded"] = new() { Type = "boolean" }
                    }
                }
            }
        });

        // 9) filter (array of string, though DevExtreme can be more complex)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "filter",
            In = ParameterLocation.Query,
            Required = false,
            Description =
                "Filter instructions. Typically an array in DevExtreme. Example: [ \"Field\", \"=\", \"Value\" ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema { Type = "string" }
            }
        });

        // 10) totalSummary (array of { selector: string, summaryType: string })
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "totalSummary",
            In = ParameterLocation.Query,
            Required = false,
            Description =
                "Total summary definitions. Example: [ { \"selector\": \"Price\", \"summaryType\": \"sum\" } ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["selector"] = new() { Type = "string" },
                        ["summaryType"] = new() { Type = "string" }
                    }
                }
            }
        });

        // 11) groupSummary (array of { selector: string, summaryType: string })
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "groupSummary",
            In = ParameterLocation.Query,
            Required = false,
            Description =
                "Group summary definitions. Example: [ { \"selector\": \"Price\", \"summaryType\": \"sum\" } ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["selector"] = new() { Type = "string" },
                        ["summaryType"] = new() { Type = "string" }
                    }
                }
            }
        });

        // 12) select (array of string)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "select",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Select which fields to return. Example: [ \"Id\", \"Name\" ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema { Type = "string" }
            }
        });

        // 13) preSelect (array of string)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "preSelect",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Pre-select fields. Example: [ \"Id\", \"OtherField\" ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema { Type = "string" }
            }
        });

        // 14) remoteSelect (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "remoteSelect",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, the 'select' operation is performed on the server (remote).",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 15) remoteGrouping (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "remoteGrouping",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, the 'group' operation is performed on the server (remote).",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 16) expandLinqSumType (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "expandLinqSumType",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, expands LINQ sum operations to typed expressions.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 17) primaryKey (array of string)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "primaryKey",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Defines the primary key fields for DevExtreme. Example: [ \"Id\" ].",
            Schema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema { Type = "string" }
            }
        });

        // 18) defaultSort (string)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "defaultSort",
            In = ParameterLocation.Query,
            Required = false,
            Description = "Specifies a default sorting field. Example: \"Id desc\".",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });

        // 19) stringToLower (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "stringToLower",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, transforms string comparisons to lowercase.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 20) paginateViaPrimaryKey (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "paginateViaPrimaryKey",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, uses primary key to paginate data.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 21) sortByPrimaryKey (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "sortByPrimaryKey",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, appends the primary key field to the sorting.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // 22) allowAsyncOverSync (bool)
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "allowAsyncOverSync",
            In = ParameterLocation.Query,
            Required = false,
            Description = "If true, forces asynchronous operations over synchronous ones.",
            Schema = new OpenApiSchema
            {
                Type = "boolean"
            }
        });

        // ... add filter, group, or any other DevExtreme parameters you want ...
    }
}