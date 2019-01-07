using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAMActivityExtractApp
{
    public partial class BAMActivityExtractor : Form
    {
        string ActivityName = "";
        string ActivityDefnXML = "";
        string ViewName = "";
        string ViewDefnXML = "";
        string path = "";
        public BAMActivityExtractor()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            int counter = 0;
            try
            {
                if (txtConnString.Text != null)
                {
                    connection.ConnectionString = txtConnString.Text;// @"Data Source=[SQLSERVERNAME]\[INSTANCENAME];Initial Catalog=BAMPrimaryImport;Integrated Security=SSPI;Persist Security Info=False;Password=[PASSWORD];User ID=[USERNAME];";
                }
                
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = "select A.ActivityName,A.DefinitionXml,V.ViewName,V.DefinitionXml from bam_Metadata_Activities A inner join bam_Metadata_ActivityViews AV on AV.ActivityName = A.ActivityName INNER JOIN bam_Metadata_Views V ON AV.ViewName = V.ViewName ";

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ActivityName = reader.GetString(0);
                        ActivityDefnXML = reader.GetString(1);
                        ViewName = reader.GetString(2);
                        ViewDefnXML = reader.GetString(3);

                        path = txtPath.Text + "\\bam_" + ActivityName + ".xml";  
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            sw.WriteLine("<BAMDefinition xmlns = 'http://schemas.microsoft.com/BizTalkServer/2004/10/BAM' > ");
                            sw.WriteLine(ActivityDefnXML);
                            sw.WriteLine(ViewDefnXML);
                            sw.WriteLine("</BAMDefinition>");
                        }
                        counter++;
                    }
                    
                    MessageBox.Show(counter.ToString() + " Activity Definitions extracted!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("SQL", ex.InnerException.ToString());
            }
            finally
            {
                connection.Close();
            }

        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
