using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp9
{
    public partial class Form1 : Form
    {
        private byte[] imageBytes;
        private OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Admin\Desktop\db\testdb.accdb");
        private DataTable dataTable;
        private int currentRecordIndex = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                string query = "SELECT * FROM data";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                MessageBox.Show("تم الاتصال بنجاح");

                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsDataAlreadyExists())
                {
                    MessageBox.Show("البيانات موجودة بالفعل.");
                    return;
                }

                if (pictureBox1.Image != null)
                {
                    string query = "INSERT INTO data (the_name, [number], email, picture) VALUES (@name, @num, @email, @pic)";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text);
                        cmd.Parameters.AddWithValue("@num", textBox2.Text);
                        cmd.Parameters.AddWithValue("@email", textBox3.Text);
                        cmd.Parameters.AddWithValue("@pic", imageBytes);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("تم الحفظ بنجاح");

                    
                    LoadData();

                    ClearTextBoxes();
                    pictureBox1.Image = null;
                }
                else
                {
                    MessageBox.Show("الرجاء اختيار صورة قبل الحفظ.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء حفظ البيانات: " + ex.Message);
            }
        }

        private bool IsDataAlreadyExists()
        {
            string theName = textBox1.Text;
            string num = textBox2.Text;

            string query = "SELECT COUNT(*) FROM data WHERE the_name = @name AND [number] = @num";
            using (OleDbCommand command = new OleDbCommand(query, conn))
            {
                command.Parameters.AddWithValue("@name", theName);
                command.Parameters.AddWithValue("@num", num);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    imageBytes = ms.ToArray();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات لحذفها.");
                    return;
                }

                string query = "DELETE FROM data WHERE id = @id";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    
                    int currentRecordId = (int)dataTable.Rows[currentRecordIndex]["id"];
                    cmd.Parameters.AddWithValue("@id", currentRecordId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف السجل بنجاح");

                        
                        LoadData();

                        ClearTextBoxes();
                        pictureBox1.Image = null;
                    }
                    else
                    {
                        MessageBox.Show("لم يتم العثور على سجل يطابق البيانات المدخلة.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء حذف البيانات: " + ex.Message);
            }
        }

        private void ClearTextBoxes()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentRecordIndex < dataTable.Rows.Count - 1)
            {
                currentRecordIndex++;
                DisplayCurrentRecord();
            }
            else
            {
                MessageBox.Show("هذا هو آخر سجل.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentRecordIndex > 0)
            {
                currentRecordIndex--;
                DisplayCurrentRecord();
            }
            else
            {
                MessageBox.Show("هذا هو أول سجل.");
            }
        }

        private void DisplayCurrentRecord()
        {
            DataRow row = dataTable.Rows[currentRecordIndex];
            textBox1.Text = row["the_name"].ToString();
            textBox2.Text = row["number"].ToString();
            textBox3.Text = row["email"].ToString();

            byte[] imageBytes = (byte[])row["picture"];
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                pictureBox1.Image = Image.FromStream(ms);
            }
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM data";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                currentRecordIndex = 0;
                DisplayCurrentRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل البيانات: " + ex.Message);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            ClearTextBoxes();
            pictureBox1.Image = null;
        }
    }
}