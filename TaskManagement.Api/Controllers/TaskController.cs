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


        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {

            await _repository.GetAllAsync();

            return Ok();

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);

            if (task == null) return NotFound();

            var getTask = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                IsCompleted = task.IsCompleted
            };

            return Ok(getTask);
        }


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
