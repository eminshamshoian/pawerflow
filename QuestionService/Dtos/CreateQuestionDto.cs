using System.ComponentModel.DataAnnotations;
using QuestionService.Validators;

namespace QuestionService.Dtos;

// Concise and emmutable, aka they can not be changed
public record CreateQuestionDto(
    [Required]string Title, 
    [Required]string Content, 
    [Required][TagListValidator(1, 5)]List<string> Tags
    );