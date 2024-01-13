using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web.Api.Contracts;
using Web.Api.Database;
using Web.Api.Entities;
using Web.Api.Shared;

namespace Web.Api.Features.Users;

public class LoginUser
{
    public class Command : IRequest<Result<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Email).EmailAddress();
            RuleFor(c => c.Password).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<string>>

    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IValidator<Command> _validator;
        private readonly JwtOptions _options;
        public Handler(ApplicationDBContext dbContext, IValidator<Command> validator, IOptions<JwtOptions> options)
        {
            _dbContext = dbContext;
            _validator = validator;
            _options = options.Value;
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Failure<string>(new Error("LoginUser.Validation", validationResult.ToString()));

            var hashedPassword = QuickHash(request.Password);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email && x.Password == hashedPassword,
                    cancellationToken: cancellationToken);
            if(user is null)
                return Result.Failure<string>(new Error("LoginUser.UserNotFound", "User has not be found"));

            return GenerateJWT(user);
        }
        string QuickHash(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }

        string GenerateJWT(User user)
        {
            var claims = new Claim[]
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email),
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                _options.Issueer,
                _options.Audience,
                claims,
                null,
                DateTime.UtcNow.AddHours(24),
                signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return tokenValue;
        }
    }
}

public class JwtOptions
{
    public string Issueer { get; set; } = "tedt";
    public string Audience { get; set; } = "test";
    public string SecretKey { get; set; } = "aHJMb2xoRHd6LUltbmZXQy1RQmFSYWNxR1ROZ2lDOV9fVG9NUUxHMW9Ma00tZ0J2NklSbHdkZDZTMUFxRHQtWGtnU2R5MlhzUjZwbE5IWTB2ZVg5bHc";
}

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;
    private const string SectionName = "Jwt";

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);   
    }
}

public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options;

    public JwtBearerOptionsSetup(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _options.Issueer,
            ValidAudience = _options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey))
        };
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}

public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/login", async (LoginUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginUser.Command>();
            
            var result = await sender.Send(command);
            if (result.IsFailure)
                return Results.BadRequest(result.Error);
                
            return Results.Ok(result.Value);
        });
    }
}