namespace CivicConnect.App.Forms
{
    internal sealed partial class SubmitRequestForm
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
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tlpOuter = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.tlpCard = new System.Windows.Forms.TableLayoutPanel();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblCounter = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.tlpOuter.SuspendLayout();
            this.pnlCard.SuspendLayout();
            this.tlpCard.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 84);
            this.pnlHeader.TabIndex = 0;
            this.pnlHeader.Tag = "header";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Location = new System.Drawing.Point(30, 52);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(300, 23);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Tag = "subtitle";
            this.lblSubtitle.Text = "Tell us what needs attention and where.";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(28, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Tag = "title";
            this.lblTitle.Text = "Submit a request";
            //
            // tlpOuter
            //
            this.tlpOuter.ColumnCount = 3;
            this.tlpOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 720F));
            this.tlpOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOuter.Controls.Add(this.pnlCard, 1, 0);
            this.tlpOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOuter.Location = new System.Drawing.Point(0, 84);
            this.tlpOuter.Name = "tlpOuter";
            this.tlpOuter.Padding = new System.Windows.Forms.Padding(0, 28, 0, 28);
            this.tlpOuter.RowCount = 1;
            this.tlpOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOuter.Size = new System.Drawing.Size(1200, 616);
            this.tlpOuter.TabIndex = 1;
            //
            // pnlCard
            //
            this.pnlCard.Controls.Add(this.tlpCard);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Location = new System.Drawing.Point(243, 31);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(714, 554);
            this.pnlCard.TabIndex = 0;
            this.pnlCard.Tag = "card";
            //
            // tlpCard
            //
            this.tlpCard.ColumnCount = 1;
            this.tlpCard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCard.Controls.Add(this.lblCategory, 0, 0);
            this.tlpCard.Controls.Add(this.cboCategory, 0, 1);
            this.tlpCard.Controls.Add(this.lblLocation, 0, 2);
            this.tlpCard.Controls.Add(this.txtLocation, 0, 3);
            this.tlpCard.Controls.Add(this.lblDescription, 0, 4);
            this.tlpCard.Controls.Add(this.txtDescription, 0, 5);
            this.tlpCard.Controls.Add(this.lblCounter, 0, 6);
            this.tlpCard.Controls.Add(this.lblMessage, 0, 7);
            this.tlpCard.Controls.Add(this.flowButtons, 0, 8);
            this.tlpCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCard.Location = new System.Drawing.Point(0, 0);
            this.tlpCard.Name = "tlpCard";
            this.tlpCard.RowCount = 9;
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCard.Size = new System.Drawing.Size(714, 554);
            this.tlpCard.TabIndex = 0;
            //
            // lblCategory
            //
            this.lblCategory.AutoSize = true;
            this.lblCategory.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.TabIndex = 0;
            this.lblCategory.Text = "&Category";
            //
            // cboCategory
            //
            this.cboCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategory.Enabled = false;
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.TabIndex = 1;
            //
            // lblLocation
            //
            this.lblLocation.AutoSize = true;
            this.lblLocation.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "&Location (where is the problem?)";
            //
            // txtLocation
            //
            this.txtLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLocation.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.TabIndex = 3;
            //
            // lblDescription
            //
            this.lblDescription.AutoSize = true;
            this.lblDescription.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "&Description";
            //
            // txtDescription
            //
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.TabIndex = 5;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            //
            // lblCounter
            //
            this.lblCounter.AutoSize = true;
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.TabIndex = 6;
            this.lblCounter.Tag = "muted";
            this.lblCounter.Text = "0 characters";
            //
            // lblMessage
            //
            this.lblMessage.AutoSize = true;
            this.lblMessage.Margin = new System.Windows.Forms.Padding(0, 10, 0, 6);
            this.lblMessage.MaximumSize = new System.Drawing.Size(660, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.TabIndex = 7;
            //
            // flowButtons
            //
            this.flowButtons.AutoSize = true;
            this.flowButtons.Controls.Add(this.btnSubmit);
            this.flowButtons.Controls.Add(this.btnCancel);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowButtons.Name = "flowButtons";
            this.flowButtons.TabIndex = 8;
            //
            // btnSubmit
            //
            this.btnSubmit.Enabled = false;
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(180, 42);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Tag = "primary";
            this.btnSubmit.Text = "&Submit request";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 42);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Tag = "secondary";
            this.btnCancel.Text = "Cancel";
            //
            // SubmitRequestForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tlpOuter);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "SubmitRequestForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CivicConnect - Submit a request";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tlpOuter.ResumeLayout(false);
            this.pnlCard.ResumeLayout(false);
            this.tlpCard.ResumeLayout(false);
            this.tlpCard.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tlpOuter;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.TableLayoutPanel tlpCard;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
    }
}
