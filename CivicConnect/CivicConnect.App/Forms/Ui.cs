
namespace CivicConnect.App.Forms;

/// <summary>Small shared UI helpers so every form shows errors and times the same way.</summary>
internal static class Ui
{
    /// <summary>Shown instead of an exception message: exceptions can contain server names or SQL.</summary>
    public const string Unavailable = "Something went wrong. Please try again later.";

    /// <summary>Database times are UTC; people read local time.</summary>
    public static string Local(DateTime utc) =>
        DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd HH:mm");

    public static void ShowError(IWin32Window owner, string message) =>
        MessageBox.Show(owner, message, "CivicConnect", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void ShowInfo(IWin32Window owner, string message) =>
        MessageBox.Show(owner, message, "CivicConnect", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static Label FieldLabel(string text) =>
        new() { Text = text, AutoSize = true, Margin = new Padding(0, 8, 0, 2) };

    public static DataGridView CreateGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,                       // nothing typed into a grid ever reaches the database
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = SystemColors.Window
        };
        return grid;
    }

    public static DataGridViewTextBoxColumn Column(string header, string property, int weight = 100) =>
        new() { HeaderText = header, DataPropertyName = property, FillWeight = weight, SortMode = DataGridViewColumnSortMode.Automatic };

}
