using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.Helpers;
using Microsoft.AspNetCore.Http;

namespace QuickAPI.Core;

public class BindableDataSourceLoadOptions : DataSourceLoadOptionsBase
{
    public static ValueTask<BindableDataSourceLoadOptions> BindAsync(HttpContext httpContext)
    {
        var loadOptions = new BindableDataSourceLoadOptions();
        DataSourceLoadOptionsParser.Parse(loadOptions, key => httpContext.Request.Query[key]);
        return ValueTask.FromResult(loadOptions);
    }
}