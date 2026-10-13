using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;
using Microsoft.EntityFrameworkCore;

namespace CivicConnect.Requests.Persistence;

/// <summary>
/// One short-lived DbContext per call (factory), which suits WinForms. Security notes:
///  - every value reaches PostgreSQL as a parameter (EF Core); no SQL is built from strings;
///  - search text is escaped for LIKE wildcards, so "%" and "_" cannot be used to scan everything;
///  - UPDATE only touches status and assigned_staff_id, the only request columns the
///    civicconnect_app role is granted (db/03_schema.sql); history and notes are INSERT-only.
/// </summary>
internal sealed class EfRequestRepository(IDbContextFactory<RequestsDbContext> factory) : IRequestRepository
{
    public async Task<Request?> FindByIdAsync(long id, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Requests.AsNoTracking()
            .Include(r => r.History).Include(r => r.Notes)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task AddAsync(Request request, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        db.Requests.Add(request);                 // request + its history row: one SaveChanges = one transaction
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Request request, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        // Attach: rows that already have an id stay Unchanged; new history/notes (id 0) become Added.
        db.Attach(request);
        var entry = db.Entry(request);
        entry.Property(r => r.Status).IsModified = true;
        entry.Property(r => r.AssignedStaffId).IsModified = true;

        try
        {
            await db.SaveChangesAsync(ct);        // UPDATE ... WHERE id = @id AND xmin = @xmin, plus inserts, atomically
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new RequestConcurrencyException(ex);
        }
    }

    public async Task<IReadOnlyList<RequestCategory>> ListActiveCategoriesAsync(CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Categories.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(ct);
    }

    public async Task<RequestCategory?> FindCategoryAsync(long id, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<RequestDto>> ListByRequesterAsync(long requesterId, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Requests.AsNoTracking().Where(r => r.RequesterId == requesterId)
            .OrderByDescending(r => r.CreatedAt);
        return await Project(db, query).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RequestDto>> ListQueueAsync(RequestQueueFilter filter, long? assignedStaffId, CancellationToken ct)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        var query = db.Requests.AsNoTracking().AsQueryable();

        // The queue is open work. Closed requests only show when Closed is asked for explicitly.
        query = filter.Status is { } status
            ? query.Where(r => r.Status == status)
            : query.Where(r => r.Status != RequestStatus.Closed);

        if (assignedStaffId is { } staffId)
            query = query.Where(r => r.AssignedStaffId == staffId);

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var pattern = "%" + EscapeLike(filter.SearchText!) + "%";
            query = query.Where(r => EF.Functions.ILike(r.Description, pattern, "\\")
                                  || EF.Functions.ILike(r.Location, pattern, "\\"));
        }

        // Order the entity query before projection so EF Core can translate the ordering to SQL.
        return await Project(db, query.OrderBy(r => r.CreatedAt)).ToListAsync(ct);
    }

    private static IQueryable<RequestDto> Project(RequestsDbContext db, IQueryable<Request> requests) =>
        from r in requests
        join c in db.Categories.AsNoTracking() on r.CategoryId equals c.Id
        select new RequestDto(r.Id, c.Name, r.Location, r.Description, r.Status.ToString(),
            r.RequesterId, r.AssignedStaffId, r.CreatedAt);

    internal static string EscapeLike(string input) =>
        input.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
