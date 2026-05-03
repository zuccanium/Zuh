namespace Zuh.Compiler.Ast {
    /// <summary>
    /// this is just used in the analyzer for dictionaries that always have a key of something that has a scope.
    /// </summary>
    /// <remarks>
    /// i love interface naming conventions because i can make these things look like 2010 cat memes.
    /// 
    /// <code language="csharp">
    /// public IHasCheezburger? Cat { get; set; } = null;  
    /// </code>
    /// </remarks>
    public interface IHasScope : IZuhNode {}
}