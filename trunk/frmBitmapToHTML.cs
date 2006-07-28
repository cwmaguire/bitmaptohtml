using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;

namespace BitmapToHTML{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmBitmapToHTML : System.Windows.Forms.Form
	{
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.Button btnGetFilePath;
    private System.Windows.Forms.Label lblBitmapFile;
    private System.Windows.Forms.TextBox txtHTML;
    private System.Windows.Forms.Label lblHTML;
    private System.Windows.Forms.TextBox txtBitmapFilePath;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.Button txtCopyToClipboard;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmBitmapToHTML()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmBitmapToHTML());
		}

    /// <summary>
    ///   Get the location of the desired bitmap
    /// </summary>
    /// <param name="sender">the button</param>
    /// <param name="e">empty</param>
    private void btnGetFilePath_Click(object sender, System.EventArgs e) {
      if(openFileDialog1.ShowDialog(this) == DialogResult.OK){
        txtBitmapFilePath.Text = openFileDialog1.FileName;
      }
      
    }

    /// <summary>
    ///   Load the image pointed at by the path in txtBitmapFilePath into the picturebox
    /// </summary>
    /// <param name="sender">button</param>
    /// <param name="e">empty</param>
    private void btnLoad_Click(object sender, System.EventArgs e) {
    
      Bitmap bitmap; 
      StringBuilder strbHTML = new StringBuilder();
      CSSClassManager classMgr = new CSSClassManager();
      string strPreviousClassName = "";
      string strCurrentClassName = "";
      int iColumnCount = 1;
    
      // try to load the file using the path
      try{
        pictureBox1.Image = Image.FromFile(txtBitmapFilePath.Text);
        bitmap = new Bitmap(pictureBox1.Image);
      }catch(Exception ex){
        MessageBox.Show(this, "Ack, loading of image failed: " + ex.ToString());
        return;
      }
   
      // create the body tag
      strbHTML.Append("<body>");
      
      // create the table HTML
      strbHTML.Append("<table class=\"table\">");
      
      // for the first row, make sure that we create a table cell
      // for every pixel so that the subsequent rows with colspans
      // line up correctly
      
      /*
      // open a table row
      strbHTML.Append("  <tr>");
      
      for(int j = 0; j < bitmap.Width; j++){
       
        // notice the hard coded zero in the bitmap.GetPixel call
        // because we're doing the first row "by hand"
        strbHTML.Append("    <td class=\"" + 
                        classMgr.getClass(bitmap.GetPixel(j,0)) +
                        "\"></td>");
      }
        
      // close the table row
      strbHTML.Append("  </tr>");
      */
      
      // we start at one for the loop counter because
      // we've already handled the first row
      for(int i = 0; i < bitmap.Height; i++){
        // open a table row
        strbHTML.Append("  <tr>");
      
        for(int j = 0; j < bitmap.Width; j++){
          // get the class name for the current pixel
          strCurrentClassName = classMgr.getClass(bitmap.GetPixel(j,i));
          
          // if the current pixel is the same as the previous pixel
          // then simply update the column span count in order
          // to use a single table cell to cover multiple pixels 
          // of the same colour
          if(strCurrentClassName.Equals(strPreviousClassName)){
            iColumnCount++;
            
            //if this is the last pixel in the row then we'll need to print it now
            if(j == bitmap.Width - 1){
              strbHTML.Append("    <td class=\"" + 
                              strPreviousClassName +
                              "\" colspan=" + 
                              iColumnCount.ToString() +
                              "></td>");
            }
            
          // else if the preceding cell's column count was greater than 1
          // then we'll add a colspan attribute to the HTML
          }else if(iColumnCount > 1){
            strbHTML.Append("    <td class=\"" + 
                            strPreviousClassName +
                            "\" colspan=" + 
                            iColumnCount.ToString() +
                            "></td>");
            // reset the column count
            iColumnCount = 1;
            // reset the previous class name
            strPreviousClassName = strCurrentClassName;
            
            //if this is the last pixel in the row then we'll need to print it now
            if(j == bitmap.Width - 1){
              strbHTML.Append("    <td class=\"" + 
                              strCurrentClassName +
                              "\"></td>");
            }
          
          // else just print the preceding cell
          // (unless it's the first row, in which case we'll just leave it and print it on the next iteration)
          // (or the last row, in which case we print out the current cell)
          }else if(j != 0 && j != bitmap.Width - 1){
            strbHTML.Append("    <td class=\"" + 
                            strPreviousClassName +
                            "\"></td>");
          
            // reset the column count
            iColumnCount = 1;
            // reset the previous class name
            strPreviousClassName = strCurrentClassName;
            
          // if this is the last cell and it's not the same colour as the 
          // previous cells (handled above) then print out this pixel
          // and the previous one
          }else if(j == bitmap.Width - 1){
            strbHTML.Append("    <td class=\"" + 
                            strPreviousClassName +
                            "\"></td>" +
                            "    <td class=\"" + 
                            strCurrentClassName +
                            "\"></td>");
            
          // else this is the first pixel in the row so just set the class name and count
          }else{
            iColumnCount = 1;
            strPreviousClassName = strCurrentClassName;
          }
        }
        
        // reset the previous class name and column counter
        iColumnCount = 1;
        strPreviousClassName = "";
        
        // close the table row
        strbHTML.Append("  </tr>");
      }
      
      // close the HTML table
      strbHTML.Append("</table>");
      
      // close the body tag
      strbHTML.Append("</body>");
      
      // print out the html
      txtHTML.Text = classMgr.getStyleString() + 
                     "" +
                     strbHTML.ToString();
    }


    /// <summary>
    ///   Copy the HTML to the clipboard
    /// </summary>
    /// <param name="sender">button</param>
    /// <param name="e">empty</param>
    private void txtCopyToClipboard_Click(object sender, System.EventArgs e) {
      // copy all the txtHTML text to the clipboard,
      // true says to persist the data after the app closes
      Clipboard.SetDataObject(txtHTML.Text, true);
    }


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent(){
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.btnGetFilePath = new System.Windows.Forms.Button();
      this.txtBitmapFilePath = new System.Windows.Forms.TextBox();
      this.lblBitmapFile = new System.Windows.Forms.Label();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.txtHTML = new System.Windows.Forms.TextBox();
      this.lblHTML = new System.Windows.Forms.Label();
      this.btnLoad = new System.Windows.Forms.Button();
      this.txtCopyToClipboard = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox1.Location = new System.Drawing.Point(144, 56);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(368, 216);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // btnGetFilePath
      // 
      this.btnGetFilePath.Location = new System.Drawing.Point(336, 8);
      this.btnGetFilePath.Name = "btnGetFilePath";
      this.btnGetFilePath.Size = new System.Drawing.Size(24, 23);
      this.btnGetFilePath.TabIndex = 1;
      this.btnGetFilePath.Text = "...";
      this.btnGetFilePath.Click += new System.EventHandler(this.btnGetFilePath_Click);
      // 
      // txtBitmapFilePath
      // 
      this.txtBitmapFilePath.Location = new System.Drawing.Point(56, 8);
      this.txtBitmapFilePath.Name = "txtBitmapFilePath";
      this.txtBitmapFilePath.Size = new System.Drawing.Size(272, 20);
      this.txtBitmapFilePath.TabIndex = 2;
      this.txtBitmapFilePath.Text = "";
      // 
      // lblBitmapFile
      // 
      this.lblBitmapFile.Location = new System.Drawing.Point(8, 8);
      this.lblBitmapFile.Name = "lblBitmapFile";
      this.lblBitmapFile.Size = new System.Drawing.Size(48, 23);
      this.lblBitmapFile.TabIndex = 3;
      this.lblBitmapFile.Text = "Bitmap:";
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.Filter = "Bitmap Images|*.bmp";
      // 
      // txtHTML
      // 
      this.txtHTML.AcceptsReturn = true;
      this.txtHTML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.txtHTML.Location = new System.Drawing.Point(8, 296);
      this.txtHTML.Multiline = true;
      this.txtHTML.Name = "txtHTML";
      this.txtHTML.Size = new System.Drawing.Size(672, 344);
      this.txtHTML.TabIndex = 2;
      this.txtHTML.Text = "";
      // 
      // lblHTML
      // 
      this.lblHTML.Location = new System.Drawing.Point(8, 280);
      this.lblHTML.Name = "lblHTML";
      this.lblHTML.Size = new System.Drawing.Size(48, 16);
      this.lblHTML.TabIndex = 3;
      this.lblHTML.Text = "HTML:";
      // 
      // btnLoad
      // 
      this.btnLoad.Location = new System.Drawing.Point(368, 8);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(64, 23);
      this.btnLoad.TabIndex = 1;
      this.btnLoad.Text = "Load";
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // txtCopyToClipboard
      // 
      this.txtCopyToClipboard.Location = new System.Drawing.Point(616, 272);
      this.txtCopyToClipboard.Name = "txtCopyToClipboard";
      this.txtCopyToClipboard.Size = new System.Drawing.Size(64, 23);
      this.txtCopyToClipboard.TabIndex = 1;
      this.txtCopyToClipboard.Text = "Copy";
      this.txtCopyToClipboard.Click += new System.EventHandler(this.txtCopyToClipboard_Click);
      // 
      // frmBitmapToHTML
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(688, 646);
      this.Controls.Add(this.lblBitmapFile);
      this.Controls.Add(this.txtBitmapFilePath);
      this.Controls.Add(this.txtHTML);
      this.Controls.Add(this.btnGetFilePath);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.lblHTML);
      this.Controls.Add(this.btnLoad);
      this.Controls.Add(this.txtCopyToClipboard);
      this.Name = "frmBitmapToHTML";
      this.Text = "Bitmap to HTML";
      this.ResumeLayout(false);

    }
		#endregion

	}
}
