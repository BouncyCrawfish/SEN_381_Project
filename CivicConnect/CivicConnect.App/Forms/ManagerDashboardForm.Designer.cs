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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.flowKpis = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlKpiTotal = new System.Windows.Forms.Panel();
            this.lblKpiTotalValue = new System.Windows.Forms.Label();
            this.lblKpiTotalCaption = new System.Windows.Forms.Label();
            this.pnlKpiSubmitted = new System.Windows.Forms.Panel();
            this.lblKpiSubmittedValue = new System.Windows.Forms.Label();
            this.lblKpiSubmittedCaption = new System.Windows.Forms.Label();
            this.pnlKpiAssigned = new System.Windows.Forms.Panel();
            this.lblKpiAssignedValue = new System.Windows.Forms.Label();
            this.lblKpiAssignedCaption = new System.Windows.Forms.Label();
            this.pnlKpiInProgress = new System.Windows.Forms.Panel();
            this.lblKpiInProgressValue = new System.Windows.Forms.Label();
            this.lblKpiInProgressCaption = new System.Windows.Forms.Label();
            this.pnlKpiResolved = new System.Windows.Forms.Panel();
            this.lblKpiResolvedValue = new System.Windows.Forms.Label();
            this.lblKpiResolvedCaption = new System.Windows.Forms.Label();
            this.pnlKpiClosed = new System.Windows.Forms.Panel();
            this.lblKpiClosedValue = new System.Windows.Forms.Label();
            this.lblKpiClosedCaption = new System.Windows.Forms.Label();
            this.tlpLower = new System.Windows.Forms.TableLayoutPanel();
            this.pnlListCard = new System.Windows.Forms.Panel();
            this.dgvRequests = new System.Windows.Forms.DataGridView();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAssigned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubmitted = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFilter = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlCategoryCard = new System.Windows.Forms.Panel();
            this.lstCategories = new System.Windows.Forms.ListBox();
            this.lblCategoryHeading = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.flowKpis.SuspendLayout();
            this.pnlKpiTotal.SuspendLayout();
            this.pnlKpiSubmitted.SuspendLayout();
            this.pnlKpiAssigned.SuspendLayout();
            this.pnlKpiInProgress.SuspendLayout();
            this.pnlKpiResolved.SuspendLayout();
            this.pnlKpiClosed.SuspendLayout();
            this.tlpLower.SuspendLayout();
            this.pnlListCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).BeginInit();
            this.flowToolbar.SuspendLayout();
            this.pnlCategoryCard.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.btnBack);
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
            this.btnBack.TabIndex = 2;
            this.btnBack.Tag = "ghost";
            this.btnBack.Text = "&Back";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(30, 52);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(300, 23);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Tag = "subtitle";
            this.lblSubtitle.Text = "Read-only view of every request, including closed ones.";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(28, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Tag = "title";
            this.lblTitle.Text = "Overview";
            //
            // tlpMain
            //
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.flowKpis, 0, 0);
            this.tlpMain.Controls.Add(this.tlpLower, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 84);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.Padding = new System.Windows.Forms.Padding(28, 24, 28, 24);
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1200, 616);
            this.tlpMain.TabIndex = 1;
            //
            // flowKpis
            //
            this.flowKpis.AutoSize = true;
            this.flowKpis.Controls.Add(this.pnlKpiTotal);
            this.flowKpis.Controls.Add(this.pnlKpiSubmitted);
            this.flowKpis.Controls.Add(this.pnlKpiAssigned);
            this.flowKpis.Controls.Add(this.pnlKpiInProgress);
            this.flowKpis.Controls.Add(this.pnlKpiResolved);
            this.flowKpis.Controls.Add(this.pnlKpiClosed);
            this.flowKpis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowKpis.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.flowKpis.Name = "flowKpis";
            this.flowKpis.TabIndex = 0;
            //
            // pnlKpiTotal
            //
            this.pnlKpiTotal.Controls.Add(this.lblKpiTotalCaption);
            this.pnlKpiTotal.Controls.Add(this.lblKpiTotalValue);
            this.pnlKpiTotal.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlKpiTotal.Name = "pnlKpiTotal";
            this.pnlKpiTotal.Size = new System.Drawing.Size(190, 96);
            this.pnlKpiTotal.TabIndex = 0;
            this.pnlKpiTotal.Tag = "kpi";
            //
            // lblKpiTotalValue
            //
            this.lblKpiTotalValue.AutoSize = true;
            this.lblKpiTotalValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiTotalValue.Name = "lblKpiTotalValue";
            this.lblKpiTotalValue.TabIndex = 0;
            this.lblKpiTotalValue.Tag = "kpi";
            this.lblKpiTotalValue.Text = "-";
            //
            // lblKpiTotalCaption
            //
            this.lblKpiTotalCaption.AutoSize = true;
            this.lblKpiTotalCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiTotalCaption.Name = "lblKpiTotalCaption";
            this.lblKpiTotalCaption.TabIndex = 1;
            this.lblKpiTotalCaption.Tag = "muted";
            this.lblKpiTotalCaption.Text = "Total requests";
            //
            // pnlKpiSubmitted
            //
            this.pnlKpiSubmitted.Controls.Add(this.lblKpiSubmittedCaption);
            this.pnlKpiSubmitted.Controls.Add(this.lblKpiSubmittedValue);
            this.pnlKpiSubmitted.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlKpiSubmitted.Name = "pnlKpiSubmitted";
            this.pnlKpiSubmitted.Size = new System.Drawing.Size(190, 96);
            this.pnlKpiSubmitted.TabIndex = 1;
            this.pnlKpiSubmitted.Tag = "kpi";
            //
            // lblKpiSubmittedValue
            //
            this.lblKpiSubmittedValue.AutoSize = true;
            this.lblKpiSubmittedValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiSubmittedValue.Name = "lblKpiSubmittedValue";
            this.lblKpiSubmittedValue.TabIndex = 0;
            this.lblKpiSubmittedValue.Tag = "kpi";
            this.lblKpiSubmittedValue.Text = "-";
            //
            // lblKpiSubmittedCaption
            //
            this.lblKpiSubmittedCaption.AutoSize = true;
            this.lblKpiSubmittedCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiSubmittedCaption.Name = "lblKpiSubmittedCaption";
            this.lblKpiSubmittedCaption.TabIndex = 1;
            this.lblKpiSubmittedCaption.Tag = "muted";
            this.lblKpiSubmittedCaption.Text = "Submitted";
            //
            // pnlKpiAssigned
            //
            this.pnlKpiAssigned.Controls.Add(this.lblKpiAssignedCaption);
            this.pnlKpiAssigned.Controls.Add(this.lblKpiAssignedValue);
            this.pnlKpiAssigned.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlKpiAssigned.Name = "pnlKpiAssigned";
            this.pnlKpiAssigned.Size = new System.Drawing.Size(190, 96);
            this.pnlKpiAssigned.TabIndex = 2;
            this.pnlKpiAssigned.Tag = "kpi";
            //
            // lblKpiAssignedValue
            //
            this.lblKpiAssignedValue.AutoSize = true;
            this.lblKpiAssignedValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiAssignedValue.Name = "lblKpiAssignedValue";
            this.lblKpiAssignedValue.TabIndex = 0;
            this.lblKpiAssignedValue.Tag = "kpi";
            this.lblKpiAssignedValue.Text = "-";
            //
            // lblKpiAssignedCaption
            //
            this.lblKpiAssignedCaption.AutoSize = true;
            this.lblKpiAssignedCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiAssignedCaption.Name = "lblKpiAssignedCaption";
            this.lblKpiAssignedCaption.TabIndex = 1;
            this.lblKpiAssignedCaption.Tag = "muted";
            this.lblKpiAssignedCaption.Text = "Assigned";
            //
            // pnlKpiInProgress
            //
            this.pnlKpiInProgress.Controls.Add(this.lblKpiInProgressCaption);
            this.pnlKpiInProgress.Controls.Add(this.lblKpiInProgressValue);
            this.pnlKpiInProgress.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlKpiInProgress.Name = "pnlKpiInProgress";
            this.pnlKpiInProgress.Size = new System.Drawing.Size(190, 96);
            this.pnlKpiInProgress.TabIndex = 3;
            this.pnlKpiInProgress.Tag = "kpi";
            //
            // lblKpiInProgressValue
            //
            this.lblKpiInProgressValue.AutoSize = true;
            this.lblKpiInProgressValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiInProgressValue.Name = "lblKpiInProgressValue";
            this.lblKpiInProgressValue.TabIndex = 0;
            this.lblKpiInProgressValue.Tag = "kpi";
            this.lblKpiInProgressValue.Text = "-";
            //
            // lblKpiInProgressCaption
            //
            this.lblKpiInProgressCaption.AutoSize = true;
            this.lblKpiInProgressCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiInProgressCaption.Name = "lblKpiInProgressCaption";
            this.lblKpiInProgressCaption.TabIndex = 1;
            this.lblKpiInProgressCaption.Tag = "muted";
            this.lblKpiInProgressCaption.Text = "In progress";
            //
            // pnlKpiResolved
            //
            this.pnlKpiResolved.Controls.Add(this.lblKpiResolvedCaption);
            this.pnlKpiResolved.Controls.Add(this.lblKpiResolvedValue);
            this.pnlKpiResolved.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlKpiResolved.Name = "pnlKpiResolved";
            this.pnlKpiResolved.Size = new System.Drawing.Size(190, 96);
            this.pnlKpiResolved.TabIndex = 4;
            this.pnlKpiResolved.Tag = "kpi";
            //
            // lblKpiResolvedValue
            //
            this.lblKpiResolvedValue.AutoSize = true;
            this.lblKpiResolvedValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiResolvedValue.Name = "lblKpiResolvedValue";
            this.lblKpiResolvedValue.TabIndex = 0;
            this.lblKpiResolvedValue.Tag = "kpi";
            this.lblKpiResolvedValue.Text = "-";
            //
            // lblKpiResolvedCaption
            //
            this.lblKpiResolvedCaption.AutoSize = true;
            this.lblKpiResolvedCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiResolvedCaption.Name = "lblKpiResolvedCaption";
            this.lblKpiResolvedCaption.TabIndex = 1;
            this.lblKpiResolvedCaption.Tag = "muted";
            this.lblKpiResolvedCaption.Text = "Resolved";
            //
            // pnlKpiClosed
            //
            this.pnlKpiClosed.Controls.Add(this.lblKpiClosedCaption);
            this.pnlKpiClosed.Controls.Add(this.lblKpiClosedValue);
            this.pnlKpiClosed.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlKpiClosed.Name = "pnlKpiClosed";
            this.pnlKpiClosed.Size = new System.Drawing.Size(190, 96);
            this.pnlKpiClosed.TabIndex = 5;
            this.pnlKpiClosed.Tag = "kpi";
            //
            // lblKpiClosedValue
            //
            this.lblKpiClosedValue.AutoSize = true;
            this.lblKpiClosedValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiClosedValue.Name = "lblKpiClosedValue";
            this.lblKpiClosedValue.TabIndex = 0;
            this.lblKpiClosedValue.Tag = "kpi";
            this.lblKpiClosedValue.Text = "-";
            //
            // lblKpiClosedCaption
            //
            this.lblKpiClosedCaption.AutoSize = true;
            this.lblKpiClosedCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKpiClosedCaption.Name = "lblKpiClosedCaption";
            this.lblKpiClosedCaption.TabIndex = 1;
            this.lblKpiClosedCaption.Tag = "muted";
            this.lblKpiClosedCaption.Text = "Closed";
            //
            // tlpLower
            //
            this.tlpLower.ColumnCount = 2;
            this.tlpLower.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLower.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tlpLower.Controls.Add(this.pnlListCard, 0, 0);
            this.tlpLower.Controls.Add(this.pnlCategoryCard, 1, 0);
            this.tlpLower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLower.Margin = new System.Windows.Forms.Padding(0);
            this.tlpLower.Name = "tlpLower";
            this.tlpLower.RowCount = 1;
            this.tlpLower.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLower.TabIndex = 1;
            //
            // pnlListCard
            //
            this.pnlListCard.Controls.Add(this.dgvRequests);
            this.pnlListCard.Controls.Add(this.flowToolbar);
            this.pnlListCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlListCard.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlListCard.Name = "pnlListCard";
            this.pnlListCard.TabIndex = 0;
            this.pnlListCard.Tag = "card";
            //
            // flowToolbar
            //
            this.flowToolbar.AutoSize = true;
            this.flowToolbar.Controls.Add(this.lblFilter);
            this.flowToolbar.Controls.Add(this.cboStatus);
            this.flowToolbar.Controls.Add(this.txtSearch);
            this.flowToolbar.Controls.Add(this.btnApply);
            this.flowToolbar.Controls.Add(this.btnOpen);
            this.flowToolbar.Controls.Add(this.lblStatus);
            this.flowToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowToolbar.Name = "flowToolbar";
            this.flowToolbar.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.flowToolbar.TabIndex = 1;
            //
            // lblFilter
            //
            this.lblFilter.AutoSize = true;
            this.lblFilter.Margin = new System.Windows.Forms.Padding(0, 8, 8, 0);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.TabIndex = 0;
            this.lblFilter.Text = "Status";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Margin = new System.Windows.Forms.Padding(0, 4, 16, 0);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(170, 31);
            this.cboStatus.TabIndex = 1;
            //
            // txtSearch
            //
            this.txtSearch.Margin = new System.Windows.Forms.Padding(0, 4, 16, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Search description or location";
            this.txtSearch.Size = new System.Drawing.Size(280, 30);
            this.txtSearch.TabIndex = 2;
            //
            // btnApply
            //
            this.btnApply.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(110, 36);
            this.btnApply.TabIndex = 3;
            this.btnApply.Tag = "secondary";
            this.btnApply.Text = "&Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // btnOpen
            //
            this.btnOpen.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(130, 36);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Tag = "primary";
            this.btnOpen.Text = "&Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Tag = "muted";
            //
            // dgvRequests
            //
            this.dgvRequests.AllowUserToAddRows = false;
            this.dgvRequests.AllowUserToDeleteRows = false;
            this.dgvRequests.AllowUserToResizeRows = false;
            this.dgvRequests.AutoGenerateColumns = false;
            this.dgvRequests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRequests.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colId,
                this.colCategory,
                this.colLocation,
                this.colStatus,
                this.colAssigned,
                this.colSubmitted});
            this.dgvRequests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRequests.MultiSelect = false;
            this.dgvRequests.Name = "dgvRequests";
            this.dgvRequests.ReadOnly = true;
            this.dgvRequests.RowHeadersVisible = false;
            this.dgvRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequests.TabIndex = 0;
            this.dgvRequests.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRequests_CellDoubleClick);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 35F;
            this.colId.HeaderText = "Ref";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colCategory
            //
            this.colCategory.DataPropertyName = "Category";
            this.colCategory.FillWeight = 80F;
            this.colCategory.HeaderText = "Category";
            this.colCategory.Name = "colCategory";
            this.colCategory.ReadOnly = true;
            //
            // colLocation
            //
            this.colLocation.DataPropertyName = "Location";
            this.colLocation.FillWeight = 120F;
            this.colLocation.HeaderText = "Location";
            this.colLocation.Name = "colLocation";
            this.colLocation.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 65F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colAssigned
            //
            this.colAssigned.DataPropertyName = "Assigned";
            this.colAssigned.FillWeight = 65F;
            this.colAssigned.HeaderText = "Assigned to";
            this.colAssigned.Name = "colAssigned";
            this.colAssigned.ReadOnly = true;
            //
            // colSubmitted
            //
            this.colSubmitted.DataPropertyName = "Submitted";
            this.colSubmitted.FillWeight = 85F;
            this.colSubmitted.HeaderText = "Submitted";
            this.colSubmitted.Name = "colSubmitted";
            this.colSubmitted.ReadOnly = true;
            //
            // pnlCategoryCard
            //
            this.pnlCategoryCard.Controls.Add(this.lstCategories);
            this.pnlCategoryCard.Controls.Add(this.lblCategoryHeading);
            this.pnlCategoryCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCategoryCard.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCategoryCard.Name = "pnlCategoryCard";
            this.pnlCategoryCard.TabIndex = 1;
            this.pnlCategoryCard.Tag = "card";
            //
            // lblCategoryHeading
            //
            this.lblCategoryHeading.AutoSize = true;
            this.lblCategoryHeading.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCategoryHeading.Name = "lblCategoryHeading";
            this.lblCategoryHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblCategoryHeading.TabIndex = 1;
            this.lblCategoryHeading.Tag = "heading";
            this.lblCategoryHeading.Text = "Requests by category";
            //
            // lstCategories
            //
            this.lstCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCategories.FormattingEnabled = true;
            this.lstCategories.Name = "lstCategories";
            this.lstCategories.TabIndex = 0;
            //
            // ManagerDashboardForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tlpMain);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(1000, 640);
            this.Name = "ManagerDashboardForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CivicConnect - Overview";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.flowKpis.ResumeLayout(false);
            this.pnlKpiTotal.ResumeLayout(false);
            this.pnlKpiTotal.PerformLayout();
            this.pnlKpiSubmitted.ResumeLayout(false);
            this.pnlKpiSubmitted.PerformLayout();
            this.pnlKpiAssigned.ResumeLayout(false);
            this.pnlKpiAssigned.PerformLayout();
            this.pnlKpiInProgress.ResumeLayout(false);
            this.pnlKpiInProgress.PerformLayout();
            this.pnlKpiResolved.ResumeLayout(false);
            this.pnlKpiResolved.PerformLayout();
            this.pnlKpiClosed.ResumeLayout(false);
            this.pnlKpiClosed.PerformLayout();
            this.tlpLower.ResumeLayout(false);
            this.pnlListCard.ResumeLayout(false);
            this.pnlListCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).EndInit();
            this.flowToolbar.ResumeLayout(false);
            this.flowToolbar.PerformLayout();
            this.pnlCategoryCard.ResumeLayout(false);
            this.pnlCategoryCard.PerformLayout();
            this.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlCategoryCard;
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.Label lblCategoryHeading;
    }
}
