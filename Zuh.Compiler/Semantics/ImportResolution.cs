namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// encapsulates data about the resolution of an import.
    /// used by <see cref="IImportHandler"/>.
    /// </summary>
    public record ImportResolution {
        public required bool Success { get; init; }
        
        /// <summary>
        /// the unit id of the import. this can be a path or something more abstract idk.
        /// </summary>
        /// <remarks>
        /// it should be null if <see cref="Success"/> isnt true.
        /// </remarks>
        public string? Id { get; init; }
    }
}