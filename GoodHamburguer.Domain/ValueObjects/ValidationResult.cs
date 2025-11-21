namespace GoodHamburguer.Domain.ValueObjects;

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public IEnumerable<string> Errors { get; private set; }

    private ValidationResult(bool isValid, IEnumerable<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Success()
    {
        return new ValidationResult(true, Enumerable.Empty<string>());
    }

    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        if (errors == null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return new ValidationResult(false, errors.ToList());
    }

    public static ValidationResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("The error cannot be empty or null.", nameof(error));
        }

        return new ValidationResult(false, new[] { error });
    }
}

