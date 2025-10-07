namespace WebApi.Infrastructure.Mediator;

/// <summary>
/// Represents a void type for requests that don't return a value
/// </summary>
public struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// Default value for Unit
    /// </summary>
    public static readonly Unit Value = new();

    public bool Equals(Unit other) => true;

    public override bool Equals(object? obj) => obj is Unit;

    public override int GetHashCode() => 0;

    public static bool operator ==(Unit first, Unit second) => true;

    public static bool operator !=(Unit first, Unit second) => false;

    public override string ToString() => "()";
}

