namespace CivicConnect.App.Forms
{
    internal sealed partial class StaffQueueForm
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
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlCard = new System.Windows.Forms.Panel();
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
            this.chkMine = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).BeginInit();
            this.flowToolbar.SuspendLayout();
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
            this.lblSubtitle.Text = "Open work waiting for a response.";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(28, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Tag = "title";
            this.lblTitle.Text = "Staff queue";
            //
            // pnlBody
            //
            this.pnlBody.Controls.Add(this.pnlCard);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 84);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(28, 24, 28, 24);
            this.pnlBody.Size = new System.Drawing.Size(1200, 616);
            this.pnlBody.TabIndex = 1;
            //
            // pnlCard
            //
            this.pnlCard.Controls.Add(this.dgvRequests);
            this.pnlCard.Controls.Add(this.flowToolbar);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Location = new System.Drawing.Point(28, 24);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(1144, 568);
            this.pnlCard.TabIndex = 0;
            this.pnlCard.Tag = "card";
            //
            // flowToolbar
            //
            this.flowToolbar.AutoSize = true;
            this.flowToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowToolbar.Controls.Add(this.lblFilter);
            this.flowToolbar.Controls.Add(this.cboStatus);
            this.flowToolbar.Controls.Add(this.txtSearch);
            this.flowToolbar.Controls.Add(this.chkMine);
            this.flowToolbar.Controls.Add(this.btnApply);
            this.flowToolbar.Controls.Add(this.btnOpen);
            this.flowToolbar.Controls.Add(this.lblStatus);
            this.flowToolbar.Location = new System.Drawing.Point(20, 20);
            this.flowToolbar.Name = "flowToolbar";
            this.flowToolbar.Padding = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.flowToolbar.Size = new System.Drawing.Size(1104, 48);
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
            // chkMine
            //
            this.chkMine.AutoSize = true;
            this.chkMine.Margin = new System.Windows.Forms.Padding(0, 8, 16, 0);
            this.chkMine.Name = "chkMine";
            this.chkMine.TabIndex = 3;
            this.chkMine.Text = "Assigned to me";
            //
            // btnApply
            //
            this.btnApply.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(110, 36);
            this.btnApply.TabIndex = 4;
            this.btnApply.Tag = "secondary";
            this.btnApply.Text = "&Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // btnOpen
            //
            this.btnOpen.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(170, 36);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.Tag = "primary";
            this.btnOpen.Text = "&Open / respond";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.TabIndex = 6;
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
            this.dgvRequests.Location = new System.Drawing.Point(20, 68);
            this.dgvRequests.MultiSelect = false;
            this.dgvRequests.Name = "dgvRequests";
            this.dgvRequests.ReadOnly = true;
            this.dgvRequests.RowHeadersVisible = false;
            this.dgvRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequests.Size = new System.Drawing.Size(1104, 480);
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
            // StaffQueueForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "StaffQueueForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CivicConnect - Staff queue";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlBody.ResumeLayout(false);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).EndInit();
            this.flowToolbar.ResumeLayout(false);
            this.flowToolbar.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Panel pnlCard;
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
        private System.Windows.Forms.CheckBox chkMine;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label lblStatus;
    }
}
