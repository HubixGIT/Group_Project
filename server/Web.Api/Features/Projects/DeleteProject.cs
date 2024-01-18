using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Contracts;
using Web.Api.Contracts.Projects.Delete;
using Web.Api.Database;
using Web.Api.Entities;
using Web.Api.Extensions.CurrentUserService;
using Web.Api.Shared;

namespace Web.Api.Features.Projects;

public class DeleteProject
{
    public class Command : IRequest<Result<int>>
    {
        public int ProjectId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
    internal sealed class Handler : IRequestHandler<DeleteProject.Command, Result<int>>
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IValidator<Command> _validator;
        private readonly ICurrentUserService _currentUserService;

        public Handler(ApplicationDBContext dbContext, IValidator<Command> validator, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return Result.Failure<int>(new Error("DeleteProject.Validation", validationResult.ToString()));

                if (!await CanDelete(request.ProjectId, _currentUserService.UserId, cancellationToken))
                    return Result.Failure<int>(new Error("DeleteProject.NoAccess", "Access denied"));

                await DeleteProject(request.ProjectId, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return request.ProjectId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Result.Failure<int>(new Error("DeleteProject", e.ToString()));
            }
        }

        public async Task<bool> CanDelete(int projectId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.UserProjects
                .AnyAsync(x => x.ProjectId == projectId && x.UserId == userId && x.Rank == UserProjectRankEnum.Owner,
                cancellationToken);
        }

        public async Task DeleteProject(int projectId, CancellationToken cancellationToken = default)
        {
            var project = _dbContext.Projects.FirstOrDefault(x => x.Id == projectId);
            if (project != null) 
                _dbContext.Projects.Remove(project);
        }

    }
}

public class DeleteProjectEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/projects/{id}", async (int projectId, ISender sender) =>
        {
            var command = new DeleteProject.Command() { ProjectId = projectId };

            var result = await sender.Send(command);
            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        }).RequireAuthorization();
    }
}
