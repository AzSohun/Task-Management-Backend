using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
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

        // Create A Task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {

            var validatorResult = await _validator.ValidateAsync(dto);

            if (!validatorResult.IsValid)
            {
                return BadRequest(validatorResult.Errors);
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


        // Get All Tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {

            var tasks = await _repository.GetAllAsync();

            var taskDtos = tasks.Select(task => new TaskDto 
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                IsCompleted = task.IsCompleted
            });

            return Ok(taskDtos);

        }


        // Get A Task
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);

            if (task == null) return NotFound();

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                IsCompleted = task.IsCompleted
            };

            return Ok(taskDto);
        }


        // Update A Task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
        {

            var exsitingTask = await _repository.GetByIdAsync(id);

            if (exsitingTask == null) return NotFound();


            exsitingTask.Title = dto.Title;
            exsitingTask.Description = dto.Description;
            exsitingTask.IsCompleted = dto.IsCompleted;
            exsitingTask.UpdatedAt = DateTime.UtcNow;
            

            await _repository.UpdateAsync(exsitingTask);

            return NoContent();

        }


        // Delete A Task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {

            var existingTask = await _repository.GetByIdAsync(id);

            if (existingTask == null) return NotFound();

            await _repository.DeleteAsync(id);

            return NoContent();

        }

    }
}
