namespace CivicConnect.App.Forms;

/// One place for the look of the application, so every screen is consistent and a colour change is one edit.
/// The Designer only sets structure (layout, text, docking) and a <c>Tag</c> on controls that need a style;
/// <see cref="Apply"/> reads those tags once the form has been built:
///   Panel  : "header" (navy bar), "card" (white panel with a thin border), "kpi" (summary tile)
///   Label  : "title", "subtitle", "heading", "muted", "kpi"
///   Button : "primary" (blue), "secondary" (outlined), "danger", "ghost" (on the header), "tile" (dashboard)

internal static class Theme
{
    public static readonly Color Navy = ColorTranslator.FromHtml("#1F3A5F");
    public static readonly Color NavyLight = ColorTranslator.FromHtml("#C7D4E8");
    public static readonly Color Accent = ColorTranslator.FromHtml("#2563EB");
    public static readonly Color AccentDark = ColorTranslator.FromHtml("#1D4ED8");
    public static readonly Color Page = ColorTranslator.FromHtml("#F3F5F9");
    public static readonly Color Card = Color.White;
    public static readonly Color Border = ColorTranslator.FromHtml("#DDE3EC");
    public static readonly Color Text = ColorTranslator.FromHtml("#1F2937");
    public static readonly Color Muted = ColorTranslator.FromHtml("#6B7280");
    public static readonly Color Danger = ColorTranslator.FromHtml("#B91C1C");
    public static readonly Color Success = ColorTranslator.FromHtml("#15803D");
    public static readonly Color RowAlt = ColorTranslator.FromHtml("#F8FAFC");
    public static readonly Color Selection = ColorTranslator.FromHtml("#DBEAFE");

    private const string FontName = "Segoe UI";
    public static Font Base => new(FontName, 10F);
    public static Font Bold => new(FontName, 10F, FontStyle.Bold);

    //Colour used for a status in grids and badges.
    public static Color StatusColor(string status) => status switch
    {
        "Submitted" => ColorTranslator.FromHtml("#B45309"),   // amber: waiting for staff
        "Assigned" => ColorTranslator.FromHtml("#1D4ED8"),    // blue
        "InProgress" => ColorTranslator.FromHtml("#6D28D9"),  // purple
        "Resolved" => ColorTranslator.FromHtml("#15803D"),    // green
        "Closed" => Muted,
        _ => Text
    };

    //Styles a form and every control inside it. Call once, right after InitializeComponent().
    public static void Apply(Form form)
    {
        form.Font = Base;
        form.BackColor = Page;
        form.ForeColor = Text;
        form.WindowState = FormWindowState.Maximized;   // every screen opens maximized
        Walk(form.Controls);
    }

    private static void Walk(Control.ControlCollection controls)
    {
        foreach (Control c in controls)
        {
            Style(c);
            if (c.HasChildren) Walk(c.Controls);
        }
    }

    private static void Style(Control c)
    {
        var tag = c.Tag as string;
        switch (c)
        {
            case Button b: StyleButton(b, tag); break;
            case DataGridView g: StyleGrid(g); break;
            case Label l: StyleLabel(l, tag); break;
            case TextBox t: t.BorderStyle = BorderStyle.FixedSingle; t.BackColor = Color.White; break;
            case ListBox lb: lb.BorderStyle = BorderStyle.FixedSingle; lb.IntegralHeight = false; break;
            case ComboBox cb: cb.FlatStyle = FlatStyle.Flat; cb.BackColor = Color.White; break;
            case Panel p: StylePanel(p, tag); break;
        }
    }

    private static void StylePanel(Panel p, string? tag)
    {
        switch (tag)
        {
            case "header":
                p.BackColor = Navy;
                p.Padding = new Padding(28, 0, 28, 0);
                break;
            case "card":
                p.BackColor = Card;
                p.Padding = new Padding(20);
                p.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, Border, ButtonBorderStyle.Solid);
                break;
            case "kpi":
                p.BackColor = Card;
                p.Padding = new Padding(14, 10, 14, 10);
                p.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, Border, ButtonBorderStyle.Solid);
                break;
        }
    }

    private static void StyleLabel(Label l, string? tag)
    {
        switch (tag)
        {
            case "title":
                l.Font = new Font(FontName, 18F, FontStyle.Bold); l.ForeColor = Color.White; l.BackColor = Color.Transparent; break;
            case "subtitle":
                l.Font = new Font(FontName, 10F); l.ForeColor = NavyLight; l.BackColor = Color.Transparent; break;
            case "heading":
                l.Font = new Font(FontName, 12F, FontStyle.Bold); l.ForeColor = Navy; break;
            case "muted":
                l.ForeColor = Muted; break;
            case "kpi":
                l.Font = new Font(FontName, 24F, FontStyle.Bold); l.ForeColor = Navy; break;
            default:
                l.ForeColor = Text; break;
        }
    }

    private static void StyleButton(Button b, string? tag)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.Cursor = Cursors.Hand;
        b.UseVisualStyleBackColor = false;
        b.FlatAppearance.BorderSize = 1;
        ApplyButtonColors(b, tag);

        // A disabled flat button must look disabled, whatever its normal colours are. Registered once.
        b.EnabledChanged += (_, _) => ApplyButtonColors(b, tag);
    }

    private static void ApplyButtonColors(Button b, string? tag)
    {
        if (!b.Enabled)
        {
            b.BackColor = ColorTranslator.FromHtml("#E5E7EB");
            b.ForeColor = Muted;
            b.FlatAppearance.BorderColor = Border;
            return;
        }

        switch (tag)
        {
            case "primary":
                b.BackColor = Accent; b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = Accent; b.FlatAppearance.MouseOverBackColor = AccentDark;
                b.Font = Bold; break;
            case "danger":
                b.BackColor = Danger; b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = Danger; b.Font = Bold; break;
            case "ghost":
                b.BackColor = Color.Transparent; b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = NavyLight;
                b.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#2C4C78"); break;
            case "tile":
                b.BackColor = Card; b.ForeColor = Navy; b.TextAlign = ContentAlignment.MiddleLeft;
                b.Padding = new Padding(18, 0, 12, 0);
                b.FlatAppearance.BorderColor = Border; b.FlatAppearance.MouseOverBackColor = Selection;
                b.Font = new Font(FontName, 12F, FontStyle.Bold); break;
            default: // "secondary" and anything untagged
                b.BackColor = Card; b.ForeColor = Navy;
                b.FlatAppearance.BorderColor = Border; b.FlatAppearance.MouseOverBackColor = Selection; break;
        }
    }

    private static void StyleGrid(DataGridView g)
    {
        g.BackgroundColor = Card;
        g.BorderStyle = BorderStyle.None;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.GridColor = Border;
        g.EnableHeadersVisualStyles = false;
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersHeight = 38;
        g.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#EEF2F7");
        g.ColumnHeadersDefaultCellStyle.ForeColor = Navy;
        g.ColumnHeadersDefaultCellStyle.Font = Bold;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EEF2F7");
        g.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        g.DefaultCellStyle.BackColor = Card;
        g.DefaultCellStyle.ForeColor = Text;
        g.DefaultCellStyle.SelectionBackColor = Selection;
        g.DefaultCellStyle.SelectionForeColor = Text;
        g.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        g.AlternatingRowsDefaultCellStyle.BackColor = RowAlt;
        g.RowTemplate.Height = 34;
        g.RowHeadersVisible = false;

        // Status text coloured per status (the column is the one whose DataPropertyName is "Status").
        g.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value is not string text) return;
            if (g.Columns[e.ColumnIndex].DataPropertyName != "Status") return;
            e.CellStyle.ForeColor = StatusColor(text);
            e.CellStyle.Font = Bold;
        };
    }

    //Shows a success or error line under a form.
    public static void ShowMessage(Label label, string? text, bool isError)
    {
        label.ForeColor = isError ? Danger : Success;
        label.Text = text ?? string.Empty;
    }
}
