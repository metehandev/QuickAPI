namespace MY.QuickAPI.Database.Core;

/// <summary>
/// Crud operation to generate automatically
/// </summary>
[Flags]
public enum CrudOperation
{
    /// <summary>
    /// GET method to get single element with Id
    /// </summary>
    Get = 1,
    /// <summary>
    /// GET method to get the list of the elements (with pagination support)
    /// </summary>
    GetMany = 1 << 1,
    /// <summary>
    /// POST method to add an element 
    /// </summary>
    Post = 1 << 2,
    /// <summary>
    /// PUT method to update an element
    /// </summary>
    Put = 1 << 3,
    /// <summary>
    /// DELETE method to delete an element
    /// </summary>
    Delete = 1 << 4,
    /// <summary>
    /// POST method to bulk insert multiple elements
    /// </summary>
    PostMany = 1 << 5,
    /// <summary>
    /// Generate all CrudOperations 
    /// </summary>
    All = Get | GetMany | Post | Put | Delete | PostMany
}