namespace Interop

///<summary>
/// Entry point for safe interop with F# code.
/// Copied (mostly)verbatim from: http://stackoverflow.com/a/5463951/1400198
/// Author: Daniel (StackOverflow user)
///</summary>
[<AutoOpen>]
module InteropFiler = 
    open System
    
    let inline isNull value = Object.ReferenceEquals(value, null)
    let inline nil<'T> = Unchecked.defaultof<'T>
    let inline safeUnbox value = if isNull value then nil else unbox value
    let (|Null|_|) value = if isNull value then Some() else None