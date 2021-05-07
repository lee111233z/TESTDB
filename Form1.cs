using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySqlDB
{
    public partial class Form1 : Form
    {
        //DB 서버 연결 
        static string conString = "Server = localhost; Database = studentdb; Uid = root; Pwd = 1234;";
        MySqlConnection con = new MySqlConnection(conString);
        MySqlCommand cmd;
        MySqlDataAdapter adapter;
        DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();

            //데이터그리드뷰 속성
            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "Name";
            dataGridView1.Columns[2].Name = "Position";
            dataGridView1.Columns[3].Name = "Team";

            //각 열의 데이터에 맞게 자동으로 사이즈를 조절
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //선택한 셀만 선택
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        // CREATE FROM DB
        private void inserte(string name, string pos, string team)
        {
            //SQL 명령어
            string sql = "INSERT INTO student(Name, Position, Team ) VALUES(@PNAME , @POSITION, @TEAM)";
            cmd = new MySqlCommand(sql, con);

            //파라미터 추가   프로시저를 호출할때 결과값 
            cmd.Parameters.AddWithValue("@PNAME", name);
            cmd.Parameters.AddWithValue("@POSITION", pos);
            cmd.Parameters.AddWithValue("@TEAM", team);

            //서버 연결 열기, CREATE ,ExecutNonQuery << 데이터를 삭제하고 삽입하는데 사용.
            try
            {
                con.Open();

                if (cmd.ExecuteNonQuery() > 0)
                {
                    clearTxts();
                    MessageBox.Show("Successfully Created");
                }

                con.Close();
                read();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void populate(string id , string name, string pos, string team)
        {
            dataGridView1.Rows.Add(id, name, pos, team);
        }

        //READ FROM DB
        private void read()
        {
            dataGridView1.Rows.Clear();

            //SQL 명령어
            string sql = "SELECT * FROM student";
            cmd = new MySqlCommand(sql, con);

            //서버 연결, Gridview에 DB 불러오기  , Dataadapter 는 DataSet에 저장하는 방식이므로 여러 table 가능
            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(dt);
                
                //Loop THRU DT
                foreach(DataRow row in dt.Rows)
                {
                    populate(row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString());
                }

                con.Close();

                dt.Rows.Clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }

        }

        private void update(int id, string name, string pos, string team)
        {
            string sql = "UPDATE student SET Name ='" + name + "',Position='" + pos + "',Team = '" + team + "' WHERE ID=" + id + "";
            cmd = new MySqlCommand(sql, con);

            try
            {
                con.Open();
                adapter = new MySqlDataAdapter(cmd);

                adapter.UpdateCommand = con.CreateCommand();
                adapter.UpdateCommand.CommandText = sql;

                if (adapter.UpdateCommand.ExecuteNonQuery() > 0)
                {
                    clearTxts();
                    MessageBox.Show("Successfully updated");
                }

                con.Close();
                read();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void delete (int id)
        {
            //SQL 명령어

            string sql = "DELETE FROM student WHERE ID = " + id + "";
            cmd = new MySqlCommand(sql,con);

            try
            {
                con.Open();

                adapter = new MySqlDataAdapter(cmd);
                adapter.DeleteCommand = con.CreateCommand();
                adapter.DeleteCommand.CommandText = sql;

                if(MessageBox.Show("맞으신가요??","삭제",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning)==DialogResult.OK)
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        clearTxts();
                        MessageBox.Show("Successfully Deleted");
                    }
                }

                con.Close();

                read();
           

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                con.Close();
            }
        }

        private void clearTxts()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            read();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            inserte(textBox1.Text, textBox2.Text, textBox3.Text);
        }



        private void button3_Click(object sender, EventArgs e)
        {
            string selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);

            update(id, textBox1.Text, textBox2.Text, textBox3.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            int id = Convert.ToInt32(selected);

            delete(id);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
    }
}
