using CivicConnect.Common;

namespace CivicConnect.Auth.Tests;

internal sealed class TestClock : TimeProvider
{
    private DateTimeOffset _now = new(2026, 10, 8, 9, 0, 0, TimeSpan.Zero);
    public override DateTimeOffset GetUtcNow() => _now;
    public void Advance(TimeSpan by) => _now += by;
}

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private long _nextId = 1;

    public IReadOnlyList<User> All => _users;

    public User Add(User user) { user.Id = _nextId++; _users.Add(user); return user; }

    public Task<User?> FindByEmailAsync(string e, CancellationToken ct) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Email == e));
    public Task<User?> FindByIdAsync(long id, CancellationToken ct) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    public Task<bool> EmailExistsAsync(string e, CancellationToken ct) =>
        Task.FromResult(_users.Any(u => u.Email == e));
    public Task<IReadOnlyList<User>> ListByRoleAsync(UserRole role, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.Role == role).ToList());
    public Task<bool> TryAddAsync(User user, CancellationToken ct)
    {
        if (_users.Any(u => u.Email == user.Email)) return Task.FromResult(false);
        Add(user);
        return Task.FromResult(true);
    }
    public Task UpdatePasswordHashAsync(long id, string newHash, CancellationToken ct)
    {
        _users.Single(u => u.Id == id).ReplacePasswordHash(newHash);
        return Task.CompletedTask;
    }
}

/// <summary>Wires the real services to a fake repository. Work factor 4 keeps tests fast.</summary>
internal sealed class AuthHarness
{
    public FakeUserRepository Repo { get; } = new();
    public BCryptPasswordHasher Hasher { get; } = new(4);
    public TestClock Clock { get; } = new();
    public SessionContext Session { get; } = new();
    public AccessControl Access { get; }
    public AuthService Auth { get; }
    public UserService Users { get; }

    public AuthHarness()
    {
        Access = new AccessControl(Session);
        Auth = new AuthService(Repo, Hasher, new InMemoryLoginAttemptTracker(Clock), Session, Session,
            new LoginDtoValidator(), new ChangePasswordDtoValidator());
        Users = new UserService(Repo, Hasher, Session, Access, new RegisterRequesterDtoValidator());
    }

    public User Seed(string email, string password, UserRole role = UserRole.Requester, string name = "Test User") =>
        Repo.Add(User.Create(name, EmailNormalizer.Normalize(email), Hasher.Hash(password), role));

    public async Task SignInAs(string email, string password, UserRole role)
    {
        Seed(email, password, role);
        var r = await Auth.LoginAsync(new LoginDto(email, password));
        Assert.True(r.IsSuccess);
    }
}
