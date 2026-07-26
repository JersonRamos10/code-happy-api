using codeHappy.Business.Dtos;
using FluentValidation;

namespace codeHappy.Business.Validators.Images;

public class UploadImageRequestValidator : AbstractValidator<UploadImageRequest>

{
    private static readonly string[] AllowedContentTypes =
        ["image/png", "image/jpeg", "image/gif", "image/webp"];
    
    public UploadImageRequestValidator()
    {
        RuleFor(i => i.ContentType)                                          
            .Must(ct => AllowedContentTypes.Contains(ct))                    
            .WithMessage($"ContentType debe ser uno de: {string.Join(", ", AllowedContentTypes)}.");
        
        RuleFor(i => i.FileLength)                                           
            .GreaterThan(0).WithMessage("El archivo no puede estar vacío.")  
            .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("El archivo no puede superar los 10MB.");
        
    }    
    
}
