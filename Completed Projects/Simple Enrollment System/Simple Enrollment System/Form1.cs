using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SimpleEnrollmentSystem
{
    public partial class Form1 : Form
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;
        private bool updateReady;
        private bool insertReady;

        public Form1()
        {
            InitializeComponent();

            connection = new SqlConnection();
            command = connection.CreateCommand();
            // Modify Source name based on server instance name, by defualt this is what it is on sqlexpress
            //connection.ConnectionString = @"Data Source=.\SQLEXPRESS;" + 
            //    "Initial Catalog=University; Integrated Security=SSPI";

            // Defualt connection string on full SQL server
            connection.ConnectionString = @"Data Source=(local);" +
                "Initial Catalog=University; Integrated Security=SSPI";
            

            updateReady = false;
            insertReady = false;
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            command.CommandText = "SELECT * FROM Students WHERE StudentID=@StudentID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@StudentID", textBoxStudentID.Text);

            try
            {
                connection.Open();
                reader = command.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    textBoxFirstName.Text = reader["FirstName"].ToString();
                    textBoxLastName.Text = reader["LastName"].ToString();
                    textBoxGender.Text = reader["Gender"].ToString();
                    textBoxAge.Text = reader["Age"].ToString();
                    textBoxAddress.Text = reader["Address"].ToString();
                }
                else
                    MessageBox.Show("StudentID does not exist.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (!updateReady)
            {
                buttonEdit.Text = "Update";
                textBoxStudentID.Enabled = false;
                textBoxFirstName.Enabled = true;
                textBoxLastName.Enabled = true;
                textBoxGender.Enabled = true;
                textBoxAge.Enabled = true;
                textBoxAddress.Enabled = true;
                buttonShow.Enabled = false;
                buttonAddNew.Enabled = false;
                buttonDelete.Enabled = false;
                updateReady = true;
            }
            else
            {
                buttonEdit.Text = "Edit";

                command.CommandText = "UPDATE Students SET FirstName=@FirstName, " +
                    "LastName=@LastName, Gender=@Gender, Age=@Age, Address=@Address " +
                    "WHERE StudentID=@StudentID";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                command.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                command.Parameters.AddWithValue("@Gender", textBoxGender.Text);
                command.Parameters.AddWithValue("@Age", textBoxAge.Text);
                command.Parameters.AddWithValue("@Address", textBoxAddress.Text);
                command.Parameters.AddWithValue("@StudentID", textBoxStudentID.Text);

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                        MessageBox.Show("Student details successfully updated.");
                    else
                        MessageBox.Show("Update failed.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                textBoxStudentID.Enabled = true;
                textBoxFirstName.Enabled = false;
                textBoxLastName.Enabled = false;
                textBoxGender.Enabled = false;
                textBoxAge.Enabled = false;
                textBoxAddress.Enabled = false;
                buttonShow.Enabled = true;
                buttonAddNew.Enabled = true;
                buttonDelete.Enabled = true;
                updateReady = false;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            command.CommandText = "DELETE FROM Students WHERE StudentID=@StudentID";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@StudentID", textBoxStudentID.Text);

            try
            {
                connection.Open();

                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Student successfully deleted.");
                    ClearFields();
                }
                else
                    MessageBox.Show("Failed to delete student.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void ClearFields()
        {
            textBoxFirstName.Text = String.Empty;
            textBoxLastName.Text = String.Empty;
            textBoxGender.Text = String.Empty;
            textBoxAge.Text = String.Empty;
            textBoxAddress.Text = String.Empty;
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            if (!insertReady)
            {
                buttonAddNew.Text = "Enroll";
                ClearFields();
                textBoxStudentID.Text = GetNextStudentID().ToString();

                textBoxStudentID.Enabled = false;
                textBoxFirstName.Enabled = true;
                textBoxLastName.Enabled = true;
                textBoxGender.Enabled = true;
                textBoxAge.Enabled = true;
                textBoxAddress.Enabled = true;
                buttonShow.Enabled = false;
                buttonEdit.Enabled = false;
                buttonDelete.Enabled = false;
                insertReady = true;
            }
            else
            {
                buttonAddNew.Text = "Add New";

                command.CommandText = "INSERT INTO Students " + 
                    "(FirstName, LastName, Gender, Age, Address) VALUES " +
                    "(@FirstName, @LastName, @Gender, @Age, @Address)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                command.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                command.Parameters.AddWithValue("@Gender", textBoxGender.Text);
                command.Parameters.AddWithValue("@Age", textBoxAge.Text);
                command.Parameters.AddWithValue("@Address", textBoxAddress.Text);

                try
                {
                    connection.Open();

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                        MessageBox.Show("Student successfully enrolled.");
                    else
                        MessageBox.Show("Failed to enroll student.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                textBoxStudentID.Enabled = true;
                textBoxFirstName.Enabled = false;
                textBoxLastName.Enabled = false;
                textBoxGender.Enabled = false;
                textBoxAge.Enabled = false;
                textBoxAddress.Enabled = false;
                buttonShow.Enabled = true;
                buttonEdit.Enabled = true;
                buttonDelete.Enabled = true;
                insertReady = false;
            }
        }

        private int GetNextStudentID()
        {        
            int nextID;
            command.CommandText = "SELECT IDENT_CURRENT('Students')";
            try
            {
                connection.Open();
                nextID = Convert.ToInt32(command.ExecuteScalar());

                if (nextID == 1)
                {
                    // catches a bug in the system where ID thinks it is 2 on a new system.
                    return nextID;
                }
                else
                {
                    command.CommandText = "SELECT IDENT_CURRENT('Students') + IDENT_INCR('Students')";

                    try
                    {
                        nextID = Convert.ToInt32(command.ExecuteScalar());
                        return nextID;
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            
            return 0;
        }

        private void textBoxStudentID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
