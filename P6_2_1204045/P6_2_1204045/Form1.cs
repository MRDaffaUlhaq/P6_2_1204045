using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace P6_2_1204045
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private DataSet dsProdi;
        public DataSet CreateProdiDataSet()
        {
            DataSet myDataSet = new DataSet();
            try
            {
                //connection string digunakan untuk koneksi ke basisdata PRG2_XXXXXXX. 
                //perhatikan data source berisi "." menunjukkan komputer localhost,
                //pada kenyataannya Anda akan menggantinya dengan alamat IP komputer server basisdata.
                //string myConnectionString = @"Data Source=LAPTOP-2TQJ2POM\P6_1204045;Initial Catalog=P6_1204045;Integrated Security=True";

                //membuat object dengan nama myConnection, inisialisasi dengan connection string
                SqlConnection myConnection = new SqlConnection(@"Data Source=LAPTOP-2TQJ2POM\P6_1204045;Initial Catalog=P6_1204045;Integrated Security=True");

                //membuat object dengan nama my Command, inisialisasi dari class SqlCommand
                SqlCommand myCommand = new SqlCommand();

                //menetapkan koneksi basisdata yang digunakan, yaitu object myConnection
                myCommand.Connection = myConnection;

                //mengatur perintah SQL yang digunakan untuk object Command
                myCommand.CommandText = "SELECT * FROM msprodi";
                myCommand.CommandType = CommandType.Text;

                //buatlah data adapter dan tentukan object command 
                //tambahkan table mapping untuk prodi 
                SqlDataAdapter myDataAdapter = new SqlDataAdapter();
                myDataAdapter.SelectCommand = myCommand;
                myDataAdapter.TableMappings.Add("Table", "Prodi");

                //gunakan method Fill dari data adapter untuk mengisi dataset
                myDataAdapter.Fill(myDataSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return myDataSet;
        }

        private void RefreshDataset() 
        {
            dsProdi = CreateProdiDataSet();

            dgProdi.DataSource = dsProdi.Tables["Prodi"];
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataset();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection(@"Data Source=LAPTOP-2TQJ2POM\P6_1204045;Initial Catalog=P6_1204045;Integrated Security=True");

            myConnection.Open();

            SqlDataAdapter myAdapter = new SqlDataAdapter("SELECT * FROM msprodi", myConnection);
            SqlCommandBuilder myCmdBuilder = new SqlCommandBuilder(myAdapter);

            myAdapter.InsertCommand = myCmdBuilder.GetInsertCommand();
            myAdapter.UpdateCommand = myCmdBuilder.GetUpdateCommand();
            myAdapter.DeleteCommand = myCmdBuilder.GetDeleteCommand();

            SqlTransaction myTransaction;
            myTransaction = myConnection.BeginTransaction();
            myAdapter.DeleteCommand.Transaction = myTransaction;
            myAdapter.UpdateCommand.Transaction = myTransaction;
            myAdapter.InsertCommand.Transaction = myTransaction;

            try
            {
                int rowsUpdated = myAdapter.Update(dsProdi, "Prodi");

                myTransaction.Commit();

                MessageBox.Show(rowsUpdated.ToString() + " baris diperbarui", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshDataset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update: " + ex.Message);
                myTransaction.Rollback();
            }

        }
    }
}
