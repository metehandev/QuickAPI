using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Microsoft.AspNetCore.Http;

namespace QuickAPI.Core;

/// <summary>
/// Basic DataSourceLoadOptions model from DevExtreme.AspNet.Data library
/// Only overriden to Bind into Query strings for GetMany methods
/// </summary>
public class BindableDataSourceLoadOptions : DataSourceLoadOptionsBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static ValueTask<BindableDataSourceLoadOptions> BindAsync(HttpContext httpContext)
    {
        var loadOptions = new BindableDataSourceLoadOptions();
        DataSourceLoadOptionsParser.Parse(loadOptions, key => httpContext.Request.Query[key]);
        return ValueTask.FromResult(loadOptions);
    }
}