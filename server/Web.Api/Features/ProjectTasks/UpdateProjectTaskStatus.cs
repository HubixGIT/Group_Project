﻿using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Api.Contracts.ProjectTasks.UpdateProjectTaskStatus;
using Web.Api.Database;
using Web.Api.Entities;
using Web.Api.Extensions.CurrentUserService;
using Web.Api.Shared;

namespace Web.Api.Features.ProjectTasks;

public class UpdateProjectTaskStatus
{
    public class Command : IRequest<Result>
    {
        public int ProjectTaskId { get; set; }
        public TaskStatusEnum TaskStatus { get; set; }
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectTaskId).NotEmpty();
        }
    }
    
    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IValidator<Command> _validator;
        private readonly ICurrentUserService _currentUserService;

        public Handler(ICurrentUserService currentUserService, IValidator<Command> validator, ApplicationDBContext dbContext)
        {
            _currentUserService = currentUserService;
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    return Result.Failure(new Error("UpdateProjectTaskStatus.Validation", validationResult.ToString()));
                
                var projectId = await _dbContext.ProjectTasks.Where(x => x.Id == request.ProjectTaskId)
                    .Select(x => x.ProjectId).SingleOrDefaultAsync(cancellationToken);
                
                if (!await _currentUserService.IsProjectMember(projectId, cancellationToken))
                    return Result.Failure(new Error("UpdateProjectTaskStatus.NoAccess", "Access denied"));
                
                var success = await UpdateProjectTaskStatus(request.ProjectTaskId, request.TaskStatus, cancellationToken);
                
                if(!success)
                    return Result.Failure(new Error("UpdateProjectTaskStatus.NoAccess", "Access denied"));
                
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Result.Failure(new Error("UpdateProjectTaskStatus", e.ToString()));
            }
        }
        
        public async Task<bool> UpdateProjectTaskStatus(int projectTaskId, TaskStatusEnum status, CancellationToken cancellationToken = default)
        {
            var projectTask = _dbContext.ProjectTasks
                .SingleOrDefault(x => x.Id == projectTaskId);

            if (projectTask != null)
            {
                projectTask.TaskStatus = status;
                _dbContext.ProjectTasks.Update(projectTask);
                return true;
            }

            return false;
        }
    }
}

public class UpdateProjectTaskStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/projecttasks/status/{id}", async (UpdateProjectTaskStatusRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateProjectTaskStatus.Command>();

            var result = await sender.Send(command);
            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result);
        }).RequireAuthorization().WithTags("ProjectTasks");
    }
}