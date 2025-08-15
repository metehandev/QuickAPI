using Microsoft.EntityFrameworkCore;
using MY.QuickAPI.Core;
using MY.QuickAPI.Database.Data;
using MY.QuickAPI.Example.DataModels;
using MY.QuickAPI.Example.Dtos;
using MY.QuickAPI.Example.Services;
using MY.QuickAPI.Shared.Dtos;
using static Microsoft.AspNetCore.Http.Results;


namespace MY.QuickAPI.Example.Endpoints;

public class AuthenticationEndpoint : EndPointDefinitionBase, IEndpointDefinition
{
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationEndpoint(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        RequireAuthorization = false;
        CommonRole = nameof(UserRole.SuperAdmin);
    }

    public override void Define(WebApplication app)
    {
        // var authorizeData = GenerateAuthorizationOptions();
        const string name = "Authentication";
        app.MapPost($"/api/{name}", LoginAsync)
            .Produces<AuthenticationDto>()
            .WithTags(name)
            .AllowAnonymous();
        // app.MapGet($"/api/{name}", GetUsersAsync)
        //     .Produces<LoadResult>()
        //     .WithTags(name)
        //     .RequireAuthorization(authorizeData);
        // app.MapGet($"/api/{name}/{{userId:int}}", GetUserAsync)
        //     .Produces<UserReadDto>()
        //     .WithTags(name)
        //     .RequireAuthorization(authorizeData);
        // app.MapPost($"/api/{name}/register", RegisterAsync)
        //     .Produces<UserReadDto>()
        //     .WithTags(name)
        //     .RequireAuthorization();
        // app.MapDelete($"/api/{name}/{{userId:int}}", DeleteUserAsync)
        //     .Produces(200)
        //     .WithTags(name)
        //     .RequireAuthorization(authorizeData);
    }

    protected virtual async Task<IResult> LoginAsync(
        ILogger<AuthenticationEndpoint> logger,
        ITokenService tokenService,
        IConfiguration configuration,
        LoginDto userModel)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<BaseContext>();
        if (string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
        {
            logger.LogError("Username or password sent empty");
            return NotFound("Username or password can not be empty.");
        }

        var user = await context.Set<User>().FirstOrDefaultAsync(u =>
            u.Name == userModel.UserName && u.Password == userModel.Password);
        // var user = await repository.LoginAsync(userModel.UserName, userModel.Password);
        if (user == null)
        {
            logger.LogInformation("User not found {UserModelUserName}", userModel.UserName);
            return NotFound("Wrong username or password.");
        }

        var tokenData = new TokenDataDto(user.Name, user.TenantId, user.UserRole.ToString(), user.Id, user.Email);

        var generatedToken = tokenService.BuildToken(tokenData);

        return Ok(new AuthenticationDto(tokenData.UserName, generatedToken));
    }

    public override void DefineServices(IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }
}
