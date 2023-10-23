namespace OsuPlayer.Data.LazerModels.Interfaces;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public interface IMappingOperationOptions
{
    Func<Type, object> ServiceCtor { get; }

    /// <summary>
    /// Add context items to be accessed at map time inside an <see cref="T:AutoMapper.IValueResolver`3" /> or
    /// <see cref="T:AutoMapper.ITypeConverter`2" />
    /// </summary>
    IDictionary<string, object> Items { get; }

    /// <summary>
    /// Construct services using this callback. Use this for child/nested containers
    /// </summary>
    /// <param name="constructor"></param>
    void ConstructServicesUsing(Func<Type, object> constructor);

    /// <summary>
    /// Execute a custom function to the source and/or destination types before member mapping
    /// </summary>
    /// <param name="beforeFunction">Callback for the source/destination types</param>
    void BeforeMap(Action<object, object> beforeFunction);

    /// <summary>
    /// Execute a custom function to the source and/or destination types after member mapping
    /// </summary>
    /// <param name="afterFunction">Callback for the source/destination types</param>
    void AfterMap(Action<object, object> afterFunction);
}

public interface IMappingOperationOptions<TSource, TDestination> : IMappingOperationOptions
{
    /// <summary>
    /// Execute a custom function to the source and/or destination types before member mapping
    /// </summary>
    /// <param name="beforeFunction">Callback for the source/destination types</param>
    void BeforeMap(Action<TSource, TDestination> beforeFunction);

    /// <summary>
    /// Execute a custom function to the source and/or destination types after member mapping
    /// </summary>
    /// <param name="afterFunction">Callback for the source/destination types</param>
    void AfterMap(Action<TSource, TDestination> afterFunction);
}