namespace CivicConnect.Common;

/// <summary>
/// Shared kernel: the three roles allowed by the users_role_valid CHECK constraint.
/// Enum names MUST match the database strings exactly (stored with HasConversion&lt;string&gt;()).
/// </summary>
public enum UserRole
{
    Requester,
    Staff,
    Manager
}
