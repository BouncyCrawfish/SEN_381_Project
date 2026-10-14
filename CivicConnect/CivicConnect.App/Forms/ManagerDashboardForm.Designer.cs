namespace CivicConnect.App.Forms
{
    internal sealed partial class ManagerDashboardForm
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
            pnlHeader = new Panel();
            btnBack = new Button();
            lblSubtitle = new Label();
            lblTitle = new Label();
            tlpMain = new TableLayoutPanel();
            flowKpis = new FlowLayoutPanel();
            pnlKpiTotal = new Panel();
            lblKpiTotalCaption = new Label();
            lblKpiTotalValue = new Label();
            pnlKpiSubmitted = new Panel();
            lblKpiSubmittedCaption = new Label();
            lblKpiSubmittedValue = new Label();
            pnlKpiAssigned = new Panel();
            lblKpiAssignedCaption = new Label();
            lblKpiAssignedValue = new Label();
            pnlKpiInProgress = new Panel();
            lblKpiInProgressCaption = new Label();
            lblKpiInProgressValue = new Label();
            pnlKpiResolved = new Panel();
            lblKpiResolvedCaption = new Label();
            lblKpiResolvedValue = new Label();
            pnlKpiClosed = new Panel();
            lblKpiClosedCaption = new Label();
            lblKpiClosedValue = new Label();
            tlpLower = new TableLayoutPanel();
            pnlListCard = new Panel();
            dgvRequests = new DataGridView();
            flowToolbar = new FlowLayoutPanel();
            lblFilter = new Label();
            cboStatus = new ComboBox();
            txtSearch = new TextBox();
            btnApply = new Button();
            btnOpen = new Button();
            btnAudit = new Button();
            lblStatus = new Label();
            pnlCategoryCard = new Panel();
            lstCategories = new ListBox();
            lblCategoryHeading = new Label();
            pnlHeader.SuspendLayout();
            tlpMain.SuspendLayout();
            flowKpis.SuspendLayout();
            pnlKpiTotal.SuspendLayout();
            pnlKpiSubmitted.SuspendLayout();
            pnlKpiAssigned.SuspendLayout();
            pnlKpiInProgress.SuspendLayout();
            pnlKpiResolved.SuspendLayout();
            pnlKpiClosed.SuspendLayout();
            tlpLower.SuspendLayout();
            pnlListCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRequests).BeginInit();
            flowToolbar.SuspendLayout();
            pnlCategoryCard.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(btnBack);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1200, 84);
            pnlHeader.TabIndex = 0;
            pnlHeader.Tag = "header";
            // 
            // btnBack
            // 
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBack.DialogResult = DialogResult.Cancel;
            btnBack.Location = new Point(1050, 22);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(120, 40);
            btnBack.TabIndex = 2;
            btnBack.Tag = "ghost";
            btnBack.Text = "&Back";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(30, 52);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(299, 15);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Tag = "subtitle";
            lblSubtitle.Text = "Read-only view of every request, including closed ones.";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(28, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(56, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Tag = "title";
            lblTitle.Text = "Overview";
            // 
            // tlpMain
            // 
            tlpMain.ColumnCount = 1;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMain.Controls.Add(flowKpis, 0, 0);
            tlpMain.Controls.Add(tlpLower, 0, 1);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(0, 84);
            tlpMain.Name = "tlpMain";
            tlpMain.Padding = new Padding(28, 24, 28, 24);
            tlpMain.RowCount = 2;
            tlpMain.RowStyles.Add(new RowStyle());
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.Size = new Size(1200, 616);
            tlpMain.TabIndex = 1;
            // 
            // flowKpis
            // 
            flowKpis.AutoSize = true;
            flowKpis.Controls.Add(pnlKpiTotal);
            flowKpis.Controls.Add(pnlKpiSubmitted);
            flowKpis.Controls.Add(pnlKpiAssigned);
            flowKpis.Controls.Add(pnlKpiInProgress);
            flowKpis.Controls.Add(pnlKpiResolved);
            flowKpis.Controls.Add(pnlKpiClosed);
            flowKpis.Dock = DockStyle.Fill;
            flowKpis.Location = new Point(28, 24);
            flowKpis.Margin = new Padding(0, 0, 0, 16);
            flowKpis.Name = "flowKpis";
            flowKpis.Size = new Size(1144, 192);
            flowKpis.TabIndex = 0;
            // 
            // pnlKpiTotal
            // 
            pnlKpiTotal.Controls.Add(lblKpiTotalCaption);
            pnlKpiTotal.Controls.Add(lblKpiTotalValue);
            pnlKpiTotal.Location = new Point(0, 0);
            pnlKpiTotal.Margin = new Padding(0, 0, 16, 0);
            pnlKpiTotal.Name = "pnlKpiTotal";
            pnlKpiTotal.Size = new Size(190, 96);
            pnlKpiTotal.TabIndex = 0;
            pnlKpiTotal.Tag = "kpi";
            // 
            // lblKpiTotalCaption
            // 
            lblKpiTotalCaption.AutoSize = true;
            lblKpiTotalCaption.Dock = DockStyle.Top;
            lblKpiTotalCaption.Location = new Point(0, 15);
            lblKpiTotalCaption.Name = "lblKpiTotalCaption";
            lblKpiTotalCaption.Size = new Size(80, 15);
            lblKpiTotalCaption.TabIndex = 1;
            lblKpiTotalCaption.Tag = "muted";
            lblKpiTotalCaption.Text = "Total requests";
            // 
            // lblKpiTotalValue
            // 
            lblKpiTotalValue.AutoSize = true;
            lblKpiTotalValue.Dock = DockStyle.Top;
            lblKpiTotalValue.Location = new Point(0, 0);
            lblKpiTotalValue.Name = "lblKpiTotalValue";
            lblKpiTotalValue.Size = new Size(12, 15);
            lblKpiTotalValue.TabIndex = 0;
            lblKpiTotalValue.Tag = "kpi";
            lblKpiTotalValue.Text = "-";
            // 
            // pnlKpiSubmitted
            // 
            pnlKpiSubmitted.Controls.Add(lblKpiSubmittedCaption);
            pnlKpiSubmitted.Controls.Add(lblKpiSubmittedValue);
            pnlKpiSubmitted.Location = new Point(206, 0);
            pnlKpiSubmitted.Margin = new Padding(0, 0, 16, 0);
            pnlKpiSubmitted.Name = "pnlKpiSubmitted";
            pnlKpiSubmitted.Size = new Size(190, 96);
            pnlKpiSubmitted.TabIndex = 1;
            pnlKpiSubmitted.Tag = "kpi";
            // 
            // lblKpiSubmittedCaption
            // 
            lblKpiSubmittedCaption.AutoSize = true;
            lblKpiSubmittedCaption.Dock = DockStyle.Top;
            lblKpiSubmittedCaption.Location = new Point(0, 15);
            lblKpiSubmittedCaption.Name = "lblKpiSubmittedCaption";
            lblKpiSubmittedCaption.Size = new Size(62, 15);
            lblKpiSubmittedCaption.TabIndex = 1;
            lblKpiSubmittedCaption.Tag = "muted";
            lblKpiSubmittedCaption.Text = "Submitted";
            // 
            // lblKpiSubmittedValue
            // 
            lblKpiSubmittedValue.AutoSize = true;
            lblKpiSubmittedValue.Dock = DockStyle.Top;
            lblKpiSubmittedValue.Location = new Point(0, 0);
            lblKpiSubmittedValue.Name = "lblKpiSubmittedValue";
            lblKpiSubmittedValue.Size = new Size(12, 15);
            lblKpiSubmittedValue.TabIndex = 0;
            lblKpiSubmittedValue.Tag = "kpi";
            lblKpiSubmittedValue.Text = "-";
            // 
            // pnlKpiAssigned
            // 
            pnlKpiAssigned.Controls.Add(lblKpiAssignedCaption);
            pnlKpiAssigned.Controls.Add(lblKpiAssignedValue);
            pnlKpiAssigned.Location = new Point(412, 0);
            pnlKpiAssigned.Margin = new Padding(0, 0, 16, 0);
            pnlKpiAssigned.Name = "pnlKpiAssigned";
            pnlKpiAssigned.Size = new Size(190, 96);
            pnlKpiAssigned.TabIndex = 2;
            pnlKpiAssigned.Tag = "kpi";
            // 
            // lblKpiAssignedCaption
            // 
            lblKpiAssignedCaption.AutoSize = true;
            lblKpiAssignedCaption.Dock = DockStyle.Top;
            lblKpiAssignedCaption.Location = new Point(0, 15);
            lblKpiAssignedCaption.Name = "lblKpiAssignedCaption";
            lblKpiAssignedCaption.Size = new Size(55, 15);
            lblKpiAssignedCaption.TabIndex = 1;
            lblKpiAssignedCaption.Tag = "muted";
            lblKpiAssignedCaption.Text = "Assigned";
            // 
            // lblKpiAssignedValue
            // 
            lblKpiAssignedValue.AutoSize = true;
            lblKpiAssignedValue.Dock = DockStyle.Top;
            lblKpiAssignedValue.Location = new Point(0, 0);
            lblKpiAssignedValue.Name = "lblKpiAssignedValue";
            lblKpiAssignedValue.Size = new Size(12, 15);
            lblKpiAssignedValue.TabIndex = 0;
            lblKpiAssignedValue.Tag = "kpi";
            lblKpiAssignedValue.Text = "-";
            // 
            // pnlKpiInProgress
            // 
            pnlKpiInProgress.Controls.Add(lblKpiInProgressCaption);
            pnlKpiInProgress.Controls.Add(lblKpiInProgressValue);
            pnlKpiInProgress.Location = new Point(618, 0);
            pnlKpiInProgress.Margin = new Padding(0, 0, 16, 0);
            pnlKpiInProgress.Name = "pnlKpiInProgress";
            pnlKpiInProgress.Size = new Size(190, 96);
            pnlKpiInProgress.TabIndex = 3;
            pnlKpiInProgress.Tag = "kpi";
            // 
            // lblKpiInProgressCaption
            // 
            lblKpiInProgressCaption.AutoSize = true;
            lblKpiInProgressCaption.Dock = DockStyle.Top;
            lblKpiInProgressCaption.Location = new Point(0, 15);
            lblKpiInProgressCaption.Name = "lblKpiInProgressCaption";
            lblKpiInProgressCaption.Size = new Size(65, 15);
            lblKpiInProgressCaption.TabIndex = 1;
            lblKpiInProgressCaption.Tag = "muted";
            lblKpiInProgressCaption.Text = "In progress";
            // 
            // lblKpiInProgressValue
            // 
            lblKpiInProgressValue.AutoSize = true;
            lblKpiInProgressValue.Dock = DockStyle.Top;
            lblKpiInProgressValue.Location = new Point(0, 0);
            lblKpiInProgressValue.Name = "lblKpiInProgressValue";
            lblKpiInProgressValue.Size = new Size(12, 15);
            lblKpiInProgressValue.TabIndex = 0;
            lblKpiInProgressValue.Tag = "kpi";
            lblKpiInProgressValue.Text = "-";
            // 
            // pnlKpiResolved
            // 
            pnlKpiResolved.Controls.Add(lblKpiResolvedCaption);
            pnlKpiResolved.Controls.Add(lblKpiResolvedValue);
            pnlKpiResolved.Location = new Point(824, 0);
            pnlKpiResolved.Margin = new Padding(0, 0, 16, 0);
            pnlKpiResolved.Name = "pnlKpiResolved";
            pnlKpiResolved.Size = new Size(190, 96);
            pnlKpiResolved.TabIndex = 4;
            pnlKpiResolved.Tag = "kpi";
            // 
            // lblKpiResolvedCaption
            // 
            lblKpiResolvedCaption.AutoSize = true;
            lblKpiResolvedCaption.Dock = DockStyle.Top;
            lblKpiResolvedCaption.Location = new Point(0, 15);
            lblKpiResolvedCaption.Name = "lblKpiResolvedCaption";
            lblKpiResolvedCaption.Size = new Size(54, 15);
            lblKpiResolvedCaption.TabIndex = 1;
            lblKpiResolvedCaption.Tag = "muted";
            lblKpiResolvedCaption.Text = "Resolved";
            // 
            // lblKpiResolvedValue
            // 
            lblKpiResolvedValue.AutoSize = true;
            lblKpiResolvedValue.Dock = DockStyle.Top;
            lblKpiResolvedValue.Location = new Point(0, 0);
            lblKpiResolvedValue.Name = "lblKpiResolvedValue";
            lblKpiResolvedValue.Size = new Size(12, 15);
            lblKpiResolvedValue.TabIndex = 0;
            lblKpiResolvedValue.Tag = "kpi";
            lblKpiResolvedValue.Text = "-";
            // 
            // pnlKpiClosed
            // 
            pnlKpiClosed.Controls.Add(lblKpiClosedCaption);
            pnlKpiClosed.Controls.Add(lblKpiClosedValue);
            pnlKpiClosed.Location = new Point(0, 96);
            pnlKpiClosed.Margin = new Padding(0, 0, 16, 0);
            pnlKpiClosed.Name = "pnlKpiClosed";
            pnlKpiClosed.Size = new Size(190, 96);
            pnlKpiClosed.TabIndex = 5;
            pnlKpiClosed.Tag = "kpi";
            // 
            // lblKpiClosedCaption
            // 
            lblKpiClosedCaption.AutoSize = true;
            lblKpiClosedCaption.Dock = DockStyle.Top;
            lblKpiClosedCaption.Location = new Point(0, 15);
            lblKpiClosedCaption.Name = "lblKpiClosedCaption";
            lblKpiClosedCaption.Size = new Size(43, 15);
            lblKpiClosedCaption.TabIndex = 1;
            lblKpiClosedCaption.Tag = "muted";
            lblKpiClosedCaption.Text = "Closed";
            // 
            // lblKpiClosedValue
            // 
            lblKpiClosedValue.AutoSize = true;
            lblKpiClosedValue.Dock = DockStyle.Top;
            lblKpiClosedValue.Location = new Point(0, 0);
            lblKpiClosedValue.Name = "lblKpiClosedValue";
            lblKpiClosedValue.Size = new Size(12, 15);
            lblKpiClosedValue.TabIndex = 0;
            lblKpiClosedValue.Tag = "kpi";
            lblKpiClosedValue.Text = "-";
            // 
            // tlpLower
            // 
            tlpLower.ColumnCount = 2;
            tlpLower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpLower.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            tlpLower.Controls.Add(pnlListCard, 0, 0);
            tlpLower.Controls.Add(pnlCategoryCard, 1, 0);
            tlpLower.Dock = DockStyle.Fill;
            tlpLower.Location = new Point(28, 232);
            tlpLower.Margin = new Padding(0);
            tlpLower.Name = "tlpLower";
            tlpLower.RowCount = 1;
            tlpLower.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpLower.Size = new Size(1144, 360);
            tlpLower.TabIndex = 1;
            // 
            // pnlListCard
            // 
            pnlListCard.Controls.Add(dgvRequests);
            pnlListCard.Controls.Add(flowToolbar);
            pnlListCard.Dock = DockStyle.Fill;
            pnlListCard.Location = new Point(0, 0);
            pnlListCard.Margin = new Padding(0, 0, 16, 0);
            pnlListCard.Name = "pnlListCard";
            pnlListCard.Size = new Size(828, 360);
            pnlListCard.TabIndex = 0;
            pnlListCard.Tag = "card";
            // 
            // dgvRequests
            // 
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.AllowUserToDeleteRows = false;
            dgvRequests.AllowUserToResizeRows = false;
            dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRequests.Dock = DockStyle.Fill;
            dgvRequests.Location = new Point(0, 84);
            dgvRequests.MultiSelect = false;
            dgvRequests.Name = "dgvRequests";
            dgvRequests.ReadOnly = true;
            dgvRequests.RowHeadersVisible = false;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.Size = new Size(828, 276);
            dgvRequests.TabIndex = 0;
            dgvRequests.CellDoubleClick += dgvRequests_CellDoubleClick;
            // 
            // flowToolbar
            // 
            flowToolbar.AutoSize = true;
            flowToolbar.Controls.Add(lblFilter);
            flowToolbar.Controls.Add(cboStatus);
            flowToolbar.Controls.Add(txtSearch);
            flowToolbar.Controls.Add(btnApply);
            flowToolbar.Controls.Add(btnOpen);
            flowToolbar.Controls.Add(btnAudit);
            flowToolbar.Controls.Add(lblStatus);
            flowToolbar.Dock = DockStyle.Top;
            flowToolbar.Location = new Point(0, 0);
            flowToolbar.Name = "flowToolbar";
            flowToolbar.Padding = new Padding(0, 0, 0, 12);
            flowToolbar.Size = new Size(828, 84);
            flowToolbar.TabIndex = 1;
            // 
            // lblFilter
            // 
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(0, 8);
            lblFilter.Margin = new Padding(0, 8, 8, 0);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(39, 15);
            lblFilter.TabIndex = 0;
            lblFilter.Text = "Status";
            // 
            // cboStatus
            // 
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.FormattingEnabled = true;
            cboStatus.Location = new Point(47, 4);
            cboStatus.Margin = new Padding(0, 4, 16, 0);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(170, 23);
            cboStatus.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(233, 4);
            txtSearch.Margin = new Padding(0, 4, 16, 0);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search description or location";
            txtSearch.Size = new Size(280, 23);
            txtSearch.TabIndex = 2;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(529, 0);
            btnApply.Margin = new Padding(0, 0, 10, 0);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(110, 36);
            btnApply.TabIndex = 3;
            btnApply.Tag = "secondary";
            btnApply.Text = "&Apply";
            btnApply.Click += btnApply_Click;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(649, 0);
            btnOpen.Margin = new Padding(0, 0, 16, 0);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(130, 36);
            btnOpen.TabIndex = 4;
            btnOpen.Tag = "primary";
            btnOpen.Text = "&Open";
            btnOpen.Click += btnOpen_Click;
            // 
            // btnAudit
            // 
            btnAudit.Location = new Point(0, 36);
            btnAudit.Margin = new Padding(0, 0, 16, 0);
            btnAudit.Name = "btnAudit";
            btnAudit.Size = new Size(150, 36);
            btnAudit.TabIndex = 6;
            btnAudit.Tag = "secondary";
            btnAudit.Text = "Audit &trail";
            btnAudit.Click += btnAudit_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(166, 44);
            lblStatus.Margin = new Padding(0, 8, 0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 5;
            lblStatus.Tag = "muted";
            // 
            // pnlCategoryCard
            // 
            pnlCategoryCard.Controls.Add(lstCategories);
            pnlCategoryCard.Controls.Add(lblCategoryHeading);
            pnlCategoryCard.Dock = DockStyle.Fill;
            pnlCategoryCard.Location = new Point(844, 0);
            pnlCategoryCard.Margin = new Padding(0);
            pnlCategoryCard.Name = "pnlCategoryCard";
            pnlCategoryCard.Size = new Size(300, 360);
            pnlCategoryCard.TabIndex = 1;
            pnlCategoryCard.Tag = "card";
            // 
            // lstCategories
            // 
            lstCategories.Dock = DockStyle.Fill;
            lstCategories.FormattingEnabled = true;
            lstCategories.Location = new Point(0, 25);
            lstCategories.Name = "lstCategories";
            lstCategories.Size = new Size(300, 335);
            lstCategories.TabIndex = 0;
            // 
            // lblCategoryHeading
            // 
            lblCategoryHeading.AutoSize = true;
            lblCategoryHeading.Dock = DockStyle.Top;
            lblCategoryHeading.Location = new Point(0, 0);
            lblCategoryHeading.Name = "lblCategoryHeading";
            lblCategoryHeading.Padding = new Padding(0, 0, 0, 10);
            lblCategoryHeading.Size = new Size(119, 25);
            lblCategoryHeading.TabIndex = 1;
            lblCategoryHeading.Tag = "heading";
            lblCategoryHeading.Text = "Requests by category";
            // 
            // ManagerDashboardForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnBack;
            ClientSize = new Size(1200, 700);
            Controls.Add(tlpMain);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(1000, 640);
            Name = "ManagerDashboardForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "CivicConnect - Overview";
            WindowState = FormWindowState.Maximized;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tlpMain.ResumeLayout(false);
            tlpMain.PerformLayout();
            flowKpis.ResumeLayout(false);
            pnlKpiTotal.ResumeLayout(false);
            pnlKpiTotal.PerformLayout();
            pnlKpiSubmitted.ResumeLayout(false);
            pnlKpiSubmitted.PerformLayout();
            pnlKpiAssigned.ResumeLayout(false);
            pnlKpiAssigned.PerformLayout();
            pnlKpiInProgress.ResumeLayout(false);
            pnlKpiInProgress.PerformLayout();
            pnlKpiResolved.ResumeLayout(false);
            pnlKpiResolved.PerformLayout();
            pnlKpiClosed.ResumeLayout(false);
            pnlKpiClosed.PerformLayout();
            tlpLower.ResumeLayout(false);
            pnlListCard.ResumeLayout(false);
            pnlListCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRequests).EndInit();
            flowToolbar.ResumeLayout(false);
            flowToolbar.PerformLayout();
            pnlCategoryCard.ResumeLayout(false);
            pnlCategoryCard.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.FlowLayoutPanel flowKpis;
        private System.Windows.Forms.Panel pnlKpiTotal;
        private System.Windows.Forms.Label lblKpiTotalValue;
        private System.Windows.Forms.Label lblKpiTotalCaption;
        private System.Windows.Forms.Panel pnlKpiSubmitted;
        private System.Windows.Forms.Label lblKpiSubmittedValue;
        private System.Windows.Forms.Label lblKpiSubmittedCaption;
        private System.Windows.Forms.Panel pnlKpiAssigned;
        private System.Windows.Forms.Label lblKpiAssignedValue;
        private System.Windows.Forms.Label lblKpiAssignedCaption;
        private System.Windows.Forms.Panel pnlKpiInProgress;
        private System.Windows.Forms.Label lblKpiInProgressValue;
        private System.Windows.Forms.Label lblKpiInProgressCaption;
        private System.Windows.Forms.Panel pnlKpiResolved;
        private System.Windows.Forms.Label lblKpiResolvedValue;
        private System.Windows.Forms.Label lblKpiResolvedCaption;
        private System.Windows.Forms.Panel pnlKpiClosed;
        private System.Windows.Forms.Label lblKpiClosedValue;
        private System.Windows.Forms.Label lblKpiClosedCaption;
        private System.Windows.Forms.TableLayoutPanel tlpLower;
        private System.Windows.Forms.Panel pnlListCard;
        private System.Windows.Forms.DataGridView dgvRequests;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAssigned;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubmitted;
        private System.Windows.Forms.FlowLayoutPanel flowToolbar;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnAudit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlCategoryCard;
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.Label lblCategoryHeading;
    }
}
