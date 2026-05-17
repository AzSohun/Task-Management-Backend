using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Validators;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        private readonly ITaskRepository _repository;
        private readonly IValidator<CreateTaskDto> _validator;

        public TaskController(ITaskRepository repository, IValidator<CreateTaskDto> validator)
        {
            _repository = repository;
            _validator = validator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {

            var validationResult = await _validator.ValidateAsync(dto);
            if (validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }


            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            await _repository.AddAsync(taskItem);

            return CreatedAtAction(nameof(GetTaskById), new { id = taskItem.Id }, taskItem);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);

            return Ok(task);
        }



    }
}
