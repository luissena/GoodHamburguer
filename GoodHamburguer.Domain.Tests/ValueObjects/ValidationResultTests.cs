using GoodHamburguer.Domain.ValueObjects;
using Xunit;

namespace GoodHamburguer.Domain.Tests.ValueObjects;

public class ValidationResultTests
{
    [Fact]
    public void Success_ShouldReturnValidResult()
    {
        var result = ValidationResult.Success();
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_WithString_ShouldReturnInvalidResult()
    {
        var errorMessage = "Test error";
        
        var result = ValidationResult.Failure(errorMessage);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(errorMessage, result.Errors.First());
    }

    [Fact]
    public void Failure_WithString_WithEmptyString_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ValidationResult.Failure(string.Empty));
    }

    [Fact]
    public void Failure_WithString_WithWhitespace_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ValidationResult.Failure("   "));
    }

    [Fact]
    public void Failure_WithString_WithNull_ShouldThrowArgumentException()
    {
        string? nullString = null;
        Assert.Throws<ArgumentException>(() => ValidationResult.Failure(nullString!));
    }

    [Fact]
    public void Failure_WithEnumerable_ShouldReturnInvalidResult()
    {
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        
        var result = ValidationResult.Failure(errors);
        
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count());
        Assert.Contains("Error 1", result.Errors);
        Assert.Contains("Error 2", result.Errors);
        Assert.Contains("Error 3", result.Errors);
    }

    [Fact]
    public void Failure_WithEnumerable_WithNull_ShouldThrowArgumentNullException()
    {
        IEnumerable<string>? nullEnumerable = null;
        Assert.Throws<ArgumentNullException>(() => ValidationResult.Failure(nullEnumerable!));
    }

    [Fact]
    public void Failure_WithEnumerable_WithEmptyList_ShouldReturnInvalidResult()
    {
        var errors = Enumerable.Empty<string>();
        
        var result = ValidationResult.Failure(errors);
        
        Assert.False(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_WithEnumerable_WithSingleError_ShouldReturnInvalidResult()
    {
        var errors = new[] { "Single error" };
        
        var result = ValidationResult.Failure(errors);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Single error", result.Errors.First());
    }
}

