using System.Drawing;
using System.IO;
using System.IO.Pipes;

namespace zckclient_winform
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();

            var lbl = new System.Windows.Forms.Label();
            lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl.Location = new System.Drawing.Point(0, 0);
            lbl.Name = "label1";
            lbl.Text = "客户端";
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.BackColor = Color.Honeydew;

            txtBox = new System.Windows.Forms.RichTextBox();
            txtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            txtBox.Location = new System.Drawing.Point(0, 0);
            txtBox.Name = "txtBox";
            txtBox.Text = "";
            txtBox.WordWrap = true;
            txtBox.BackColor = Color.Cornsilk;
            txtBox.ReadOnly = false;

            var btn = new System.Windows.Forms.Button();
            btn.Dock = System.Windows.Forms.DockStyle.Fill;
            btn.Location = new System.Drawing.Point(0, 0);
            btn.Name = "btn";
            btn.Text = "发送";
            btn.BackColor = Color.AntiqueWhite;
            btn.Click += Send;

            this.tblLayout.ColumnCount = 1;
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayout.Location = new System.Drawing.Point(0, 0);
            this.tblLayout.Name = "tableLayoutPanel1";
            this.tblLayout.RowCount = 2;
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50));
            this.tblLayout.Size = new System.Drawing.Size(800, 450);
            this.tblLayout.Controls.Add(lbl, 0, 0);
            this.tblLayout.Controls.Add(txtBox, 0, 1);
            this.tblLayout.Controls.Add(btn, 0, 2);

            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 300);
            this.Controls.Add(this.tblLayout);
            this.Name = "Form1";
            this.Text = "zck client (winform)";
            this.ResumeLayout(false);
        }

        private void Send(object sender, System.EventArgs e)
        {
            var pipe = new NamedPipeClientStream(".", "pipeZck", PipeDirection.Out);
            pipe.Connect();
            var writer = new StreamWriter(pipe);
            var text = txtBox.Text;
            if (text.Length > 0)
            {
                writer.WriteLine(txtBox.Text);
                writer.Flush();
            }
            pipe.Close();
        }

        private System.Windows.Forms.TableLayoutPanel tblLayout;
        private System.Windows.Forms.RichTextBox txtBox;

        #endregion
    }
}

