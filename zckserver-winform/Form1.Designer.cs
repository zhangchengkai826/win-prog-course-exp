using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace zckserver_winform
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
        /// Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();

            var lbl = new System.Windows.Forms.Label();
            lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl.Location = new System.Drawing.Point(0, 0);
            lbl.Name = "label1";
            lbl.Text = "服务器端";
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lbl.BackColor = Color.Beige;

            var txtBox = new System.Windows.Forms.RichTextBox();
            txtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            txtBox.Location = new System.Drawing.Point(0, 0);
            txtBox.Name = "txtBox";
            txtBox.Text = "";
            txtBox.WordWrap = false;
            txtBox.BackColor = Color.AntiqueWhite;
            txtBox.ReadOnly = true;

            this.tblLayout.ColumnCount = 1;
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayout.Location = new System.Drawing.Point(0, 0);
            this.tblLayout.Name = "tableLayoutPanel1";
            this.tblLayout.RowCount = 2;
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.tblLayout.Size = new System.Drawing.Size(800, 450);
            this.tblLayout.Controls.Add(lbl, 0, 0);
            this.tblLayout.Controls.Add(txtBox, 0, 1);

            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tblLayout);
            this.Name = "Form1";
            this.Text = "zck server (winform)";
            this.ResumeLayout(false);

            Task.Factory.StartNew(() =>
            {
                var pipe = new NamedPipeServerStream("pipeZck", PipeDirection.In);
                while (true)
                {
                    pipe.WaitForConnection();
                    var reader = new StreamReader(pipe);
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        if (line.Length > 0)
                        {
                            var data = "收到来自客户端的数据： " + line + "\n";
                        }
                    }
                    pipe.Disconnect();
                }
            });
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayout;
    }
}

