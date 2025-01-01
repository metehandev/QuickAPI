namespace QuickAPI.Database.Core;

[Flags]
public enum CrudOperation
{
    Get = 1,
    GetMany = 1 << 1,
    Post = 1 << 2,
    Put = 1 << 3,
    Delete = 1 << 4,
    PostMany = 1 << 5,
    All = Get | GetMany | Post | Put | Delete | PostMany
}