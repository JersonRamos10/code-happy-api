namespace codeHappy.Business.Dtos.Blocks;

public record CreateAnnotationRequest(
        int LineNumber,
        string Text
    );