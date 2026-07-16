using codeHappy.Data.Models;
namespace codeHappy.Business.Dtos.Blocks;

public record UpdateBlockRequest (
    string? Title,
    string? Content,
    string? Language
);