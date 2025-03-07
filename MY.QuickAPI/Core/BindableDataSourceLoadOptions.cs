using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Microsoft.AspNetCore.Http;

namespace MY.QuickAPI.Core;

/// <summary>
/// Basic DataSourceLoadOptions model from DevExtreme.AspNet.Data library
/// Only overriden to Bind into Query strings for GetMany methods
/// </summary>
public class BindableDataSourceLoadOptions : DataSourceLoadOptionsBase
{
    /// <summary>
    /// Include fields for requested Data Model
    /// </summary>
    public string[]? IncludeFields { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static ValueTask<BindableDataSourceLoadOptions> BindAsync(HttpContext httpContext)
    {
        var loadOptions = new BindableDataSourceLoadOptions();
        DataSourceLoadOptionsParser.Parse(loadOptions, key => httpContext.Request.Query[key]);
        if (httpContext.Request.Query.TryGetValue("includeFields", out var includeFields))
        {
            loadOptions.IncludeFields = includeFields!;
        }
        return ValueTask.FromResult(loadOptions);
    }
}