namespace CivicConnect.App.Forms
{
    internal sealed partial class RequestDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblStatusBadge = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlDetailCard = new System.Windows.Forms.Panel();
            this.tlpDetail = new System.Windows.Forms.TableLayoutPanel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.lblDescriptionHeading = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblHistoryHeading = new System.Windows.Forms.Label();
            this.lstHistory = new System.Windows.Forms.ListBox();
            this.lblNotesHeading = new System.Windows.Forms.Label();
            this.lstNotes = new System.Windows.Forms.ListBox();
            this.pnlRespondCard = new System.Windows.Forms.Panel();
            this.tlpRespond = new System.Windows.Forms.TableLayoutPanel();
            this.lblRespondHeading = new System.Windows.Forms.Label();
            this.lblAssignTo = new System.Windows.Forms.Label();
            this.cboStaff = new System.Windows.Forms.ComboBox();
            this.btnAssign = new System.Windows.Forms.Button();
            this.lblMoveTo = new System.Windows.Forms.Label();
            this.cboNextStatus = new System.Windows.Forms.ComboBox();
            this.btnApplyStatus = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.btnAddNote = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.pnlDetailCard.SuspendLayout();
            this.tlpDetail.SuspendLayout();
            this.pnlRespondCard.SuspendLayout();
            this.tlpRespond.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.btnBack);
            this.pnlHeader.Controls.Add(this.lblStatusBadge);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 84);
            this.pnlHeader.TabIndex = 0;
            this.pnlHeader.Tag = "header";
            //
            // btnBack
            //
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Location = new System.Drawing.Point(1050, 22);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(120, 40);
            this.btnBack.TabIndex = 3;
            this.btnBack.Tag = "ghost";
            this.btnBack.Text = "&Back";
            //
            // lblStatusBadge
            //
            this.lblStatusBadge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusBadge.AutoSize = true;
            this.lblStatusBadge.Location = new System.Drawing.Point(900, 27);
            this.lblStatusBadge.Name = "lblStatusBadge";
            this.lblStatusBadge.Padding = new System.Windows.Forms.Padding(12, 6, 12, 6);
            this.lblStatusBadge.TabIndex = 2;
            this.lblStatusBadge.Tag = "badge";
            this.lblStatusBadge.Text = "Status";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(30, 52);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(300, 23);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Tag = "subtitle";
            this.lblSubtitle.Text = "Loading...";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(28, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Tag = "title";
            this.lblTitle.Text = "Request";
            //
            // tlpMain
            //
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 440F));
            this.tlpMain.Controls.Add(this.pnlDetailCard, 0, 0);
            this.tlpMain.Controls.Add(this.pnlRespondCard, 1, 0);
            this.tlpMain.Controls.Add(this.lblMessage, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 84);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.Padding = new System.Windows.Forms.Padding(28, 24, 28, 16);
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.Size = new System.Drawing.Size(1200, 616);
            this.tlpMain.TabIndex = 1;
            //
            // pnlDetailCard
            //
            this.pnlDetailCard.Controls.Add(this.tlpDetail);
            this.pnlDetailCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetailCard.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlDetailCard.Name = "pnlDetailCard";
            this.pnlDetailCard.TabIndex = 0;
            this.pnlDetailCard.Tag = "card";
            //
            // tlpDetail
            //
            this.tlpDetail.ColumnCount = 1;
            this.tlpDetail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDetail.Controls.Add(this.lblSummary, 0, 0);
            this.tlpDetail.Controls.Add(this.lblDescriptionHeading, 0, 1);
            this.tlpDetail.Controls.Add(this.txtDescription, 0, 2);
            this.tlpDetail.Controls.Add(this.lblHistoryHeading, 0, 3);
            this.tlpDetail.Controls.Add(this.lstHistory, 0, 4);
            this.tlpDetail.Controls.Add(this.lblNotesHeading, 0, 5);
            this.tlpDetail.Controls.Add(this.lstNotes, 0, 6);
            this.tlpDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDetail.Name = "tlpDetail";
            this.tlpDetail.RowCount = 7;
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDetail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tlpDetail.TabIndex = 0;
            //
            // lblSummary
            //
            this.lblSummary.AutoSize = true;
            this.lblSummary.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.TabIndex = 0;
            //
            // lblDescriptionHeading
            //
            this.lblDescriptionHeading.AutoSize = true;
            this.lblDescriptionHeading.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblDescriptionHeading.Name = "lblDescriptionHeading";
            this.lblDescriptionHeading.TabIndex = 1;
            this.lblDescriptionHeading.Tag = "heading";
            this.lblDescriptionHeading.Text = "Description";
            //
            // txtDescription
            //
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.TabIndex = 2;
            //
            // lblHistoryHeading
            //
            this.lblHistoryHeading.AutoSize = true;
            this.lblHistoryHeading.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblHistoryHeading.Name = "lblHistoryHeading";
            this.lblHistoryHeading.TabIndex = 3;
            this.lblHistoryHeading.Tag = "heading";
            this.lblHistoryHeading.Text = "History (cannot be edited)";
            //
            // lstHistory
            //
            this.lstHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstHistory.FormattingEnabled = true;
            this.lstHistory.HorizontalScrollbar = true;
            this.lstHistory.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.TabIndex = 4;
            //
            // lblNotesHeading
            //
            this.lblNotesHeading.AutoSize = true;
            this.lblNotesHeading.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblNotesHeading.Name = "lblNotesHeading";
            this.lblNotesHeading.TabIndex = 5;
            this.lblNotesHeading.Tag = "heading";
            this.lblNotesHeading.Text = "Staff notes";
            //
            // lstNotes
            //
            this.lstNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstNotes.FormattingEnabled = true;
            this.lstNotes.HorizontalScrollbar = true;
            this.lstNotes.Margin = new System.Windows.Forms.Padding(0);
            this.lstNotes.Name = "lstNotes";
            this.lstNotes.TabIndex = 6;
            //
            // pnlRespondCard
            //
            this.pnlRespondCard.Controls.Add(this.tlpRespond);
            this.pnlRespondCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRespondCard.Margin = new System.Windows.Forms.Padding(0);
            this.pnlRespondCard.Name = "pnlRespondCard";
            this.pnlRespondCard.TabIndex = 1;
            this.pnlRespondCard.Tag = "card";
            //
            // tlpRespond
            //
            this.tlpRespond.ColumnCount = 2;
            this.tlpRespond.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRespond.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpRespond.Controls.Add(this.lblRespondHeading, 0, 0);
            this.tlpRespond.Controls.Add(this.lblAssignTo, 0, 1);
            this.tlpRespond.Controls.Add(this.cboStaff, 0, 2);
            this.tlpRespond.Controls.Add(this.btnAssign, 1, 2);
            this.tlpRespond.Controls.Add(this.lblMoveTo, 0, 3);
            this.tlpRespond.Controls.Add(this.cboNextStatus, 0, 4);
            this.tlpRespond.Controls.Add(this.btnApplyStatus, 1, 4);
            this.tlpRespond.Controls.Add(this.lblNote, 0, 5);
            this.tlpRespond.Controls.Add(this.txtNote, 0, 6);
            this.tlpRespond.Controls.Add(this.btnAddNote, 0, 7);
            this.tlpRespond.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRespond.Name = "tlpRespond";
            this.tlpRespond.RowCount = 8;
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRespond.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRespond.TabIndex = 0;
            //
            // lblRespondHeading
            //
            this.lblRespondHeading.AutoSize = true;
            this.tlpRespond.SetColumnSpan(this.lblRespondHeading, 2);
            this.lblRespondHeading.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.lblRespondHeading.Name = "lblRespondHeading";
            this.lblRespondHeading.TabIndex = 0;
            this.lblRespondHeading.Tag = "heading";
            this.lblRespondHeading.Text = "Respond to this request";
            //
            // lblAssignTo
            //
            this.lblAssignTo.AutoSize = true;
            this.tlpRespond.SetColumnSpan(this.lblAssignTo, 2);
            this.lblAssignTo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblAssignTo.Name = "lblAssignTo";
            this.lblAssignTo.TabIndex = 1;
            this.lblAssignTo.Text = "Assign to";
            //
            // cboStaff
            //
            this.cboStaff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboStaff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStaff.FormattingEnabled = true;
            this.cboStaff.Margin = new System.Windows.Forms.Padding(0, 0, 8, 16);
            this.cboStaff.Name = "cboStaff";
            this.cboStaff.TabIndex = 2;
            //
            // btnAssign
            //
            this.btnAssign.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(120, 36);
            this.btnAssign.TabIndex = 3;
            this.btnAssign.Tag = "primary";
            this.btnAssign.Text = "&Assign";
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            //
            // lblMoveTo
            //
            this.lblMoveTo.AutoSize = true;
            this.tlpRespond.SetColumnSpan(this.lblMoveTo, 2);
            this.lblMoveTo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblMoveTo.Name = "lblMoveTo";
            this.lblMoveTo.TabIndex = 4;
            this.lblMoveTo.Text = "Move to";
            //
            // cboNextStatus
            //
            this.cboNextStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboNextStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNextStatus.FormattingEnabled = true;
            this.cboNextStatus.Margin = new System.Windows.Forms.Padding(0, 0, 8, 16);
            this.cboNextStatus.Name = "cboNextStatus";
            this.cboNextStatus.TabIndex = 5;
            //
            // btnApplyStatus
            //
            this.btnApplyStatus.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.btnApplyStatus.Name = "btnApplyStatus";
            this.btnApplyStatus.Size = new System.Drawing.Size(120, 36);
            this.btnApplyStatus.TabIndex = 6;
            this.btnApplyStatus.Tag = "primary";
            this.btnApplyStatus.Text = "Apply &status";
            this.btnApplyStatus.Click += new System.EventHandler(this.btnApplyStatus_Click);
            //
            // lblNote
            //
            this.lblNote.AutoSize = true;
            this.tlpRespond.SetColumnSpan(this.lblNote, 2);
            this.lblNote.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblNote.Name = "lblNote";
            this.lblNote.TabIndex = 7;
            this.lblNote.Text = "Note (required when resolving)";
            //
            // txtNote
            //
            this.tlpRespond.SetColumnSpan(this.txtNote, 2);
            this.txtNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNote.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNote.TabIndex = 8;
            //
            // btnAddNote
            //
            this.tlpRespond.SetColumnSpan(this.btnAddNote, 2);
            this.btnAddNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddNote.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddNote.Name = "btnAddNote";
            this.btnAddNote.Size = new System.Drawing.Size(120, 40);
            this.btnAddNote.TabIndex = 9;
            this.btnAddNote.Tag = "secondary";
            this.btnAddNote.Text = "Add &note only";
            this.btnAddNote.Click += new System.EventHandler(this.btnAddNote_Click);
            //
            // lblMessage
            //
            this.lblMessage.AutoSize = true;
            this.tlpMain.SetColumnSpan(this.lblMessage, 2);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.lblMessage.MaximumSize = new System.Drawing.Size(1100, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.TabIndex = 2;
            //
            // RequestDetailForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(1000, 640);
            this.Name = "RequestDetailForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CivicConnect - Request";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.pnlDetailCard.ResumeLayout(false);
            this.tlpDetail.ResumeLayout(false);
            this.tlpDetail.PerformLayout();
            this.pnlRespondCard.ResumeLayout(false);
            this.tlpRespond.ResumeLayout(false);
            this.tlpRespond.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblStatusBadge;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel pnlDetailCard;
        private System.Windows.Forms.TableLayoutPanel tlpDetail;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Label lblDescriptionHeading;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblHistoryHeading;
        private System.Windows.Forms.ListBox lstHistory;
        private System.Windows.Forms.Label lblNotesHeading;
        private System.Windows.Forms.ListBox lstNotes;
        private System.Windows.Forms.Panel pnlRespondCard;
        private System.Windows.Forms.TableLayoutPanel tlpRespond;
        private System.Windows.Forms.Label lblRespondHeading;
        private System.Windows.Forms.Label lblAssignTo;
        private System.Windows.Forms.ComboBox cboStaff;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.Label lblMoveTo;
        private System.Windows.Forms.ComboBox cboNextStatus;
        private System.Windows.Forms.Button btnApplyStatus;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Button btnAddNote;
        private System.Windows.Forms.Label lblMessage;
    }
}
