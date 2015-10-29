using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections;
using System.IO;
namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }

    class Book
    {
        public String bookName { get; set; }
        public String bookSubname { get; set; }
        public String writer { get; set; }
        public String subwriter { get; set; }
        public String translator { get; set; }
        public String publisher { get; set; }
        public String ISBN { get; set; }
        public String deweyNumber { get; set; }
        public String category { get; set; }
        public int edition { get; set; }
        public int total { get; set; }
        public int publishedYr { get; set; }
        public int left { get; set; }
        public bool canBorrow;

        public Book(String bookName, String bookSubname, String writer, String subwriter, String translator, int edition, int total,
            String publisher, int publishedYr, String ISBN, String deweyNumber, String category)
        {
            this.bookName = bookName;
            this.bookSubname = bookSubname;
            this.writer = writer;
            this.subwriter = subwriter;
            this.translator = translator;
            this.edition = edition;
            this.total = this.left = total;
            this.publisher = publisher;
            this.publishedYr = publishedYr;
            this.ISBN = ISBN;
            this.deweyNumber = deweyNumber;
            this.category = category;
            this.canBorrow = true;
        }
    }

    class DBCommand
    {
        
        public static string[,] searchBook(String bookName, String bookSubname, String writer, String translator, String publisher, String publishedYr, String category)
        {
            SQLiteConnection conn = new SQLiteConnection("data source=libDB.sqlite;");
            SQLiteCommand cmd;
            string sql;
            try
            {
                conn.Open();
                int row = DBCommand.countRow();
                string[,] data = new String[row, 10];
                int i = 0;
                //SELECT
                sql = String.Format("SELECT * FROM bookData WHERE bookName LIKE \"%{0}%\" AND bookSubname LIKE \"%{1}%\" AND writer LIKE \"%{2}%\" AND translator LIKE \"%{3}%\" AND publisher LIKE \"%{4}%\" AND publishedYr LIKE \"%{5}%\" AND category LIKE \"%{6}%\";"
                                    , bookName, bookSubname, writer, translator, publisher, publishedYr, category);
                cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data[i, 0] += reader["bookName"];
                    data[i, 1] += reader["left"].ToString() + "/" + reader["total"];
                    data[i, 2] += reader["category"];
                    data[i, 3] += reader["writer"] + ", " + reader["subwriter"];
                    data[i, 4] += reader["translator"];
                    data[i, 5] += reader["publisher"];
                    data[i, 6] += reader["publishedYr"];
                    data[i, 7] += reader["edition"];
                    data[i, 8] += reader["ISBN"];
                    data[i, 9] += reader["deweyNumber"];
                    i++;
                }
                return data;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return new string[1, 10];
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
        }

        public static void addBook(Book b)
        {
            SQLiteConnection conn = new SQLiteConnection("data source=libDB.sqlite;");
            SQLiteCommand cmd;
            try
            {
                conn.Open();
                string sql = String.Format("SELECT * FROM bookData WHERE bookName = \"{0}\" AND ISBN LIKE \"%{1}%\";", b.bookName, b.ISBN);
                cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        try
                        {
                            int total = Int32.Parse(reader["total"].ToString());
                            int left = Int32.Parse(reader["left"].ToString());
                            string sql2 = String.Format("UPDATE bookData SET total = \"{0}\",left = \"{1}\" WHERE bookName = \"{2}\" AND ISBN LIKE \"%{3}%\";", total + b.total, left + b.total, b.bookName, b.ISBN);
                            cmd = new SQLiteCommand(sql2, conn);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("เพิ่มหนังสือเรียบร้อยแล้ว");
                        }
                        catch (SQLiteException e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
                else
                {
                    try
                    {
                        string sql3 = $"INSERT INTO bookData VALUES (\"{b.bookName}\", \"{b.bookSubname}\", \"{b.writer}\", \"{b.subwriter}\", \"{b.translator}\", \"{b.edition}\", " +
                            $"\"{b.total}\", \"{b.publisher}\", \"{b.publishedYr}\", \"{b.ISBN}\", \"{b.deweyNumber}\", \"{b.category}\", \"{b.total}\", \"true\")";
                        cmd = new SQLiteCommand(sql3, conn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("เพิ่มหนังสือเรียบร้อยแล้ว");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
        }

        public static void delBook(Book b)
        {
            SQLiteConnection conn = new SQLiteConnection("data source=libDB.sqlite;");
            SQLiteCommand cmd;
            string sql;
            try
            {
                conn.Open();
                sql = $"SELECT * FROM bookData WHERE bookName = \"{b.bookName}\" AND ISBN = \"{b.ISBN}\";";
                cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    sql = $"UPDATE bookData SET total = \"{Convert.ToInt32(reader["total"]) - Convert.ToInt32(b.total)}\", left = \"{Convert.ToInt32(reader["left"]) - Convert.ToInt32(b.total)}\" WHERE bookName = \"{b.bookName}\" AND ISBN = \"{b.ISBN}\";";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("ลบหนังสือจำนวน " + b.total + " เล่ม\nเรียบร้อยแล้ว");
                    if(Convert.ToInt32(reader["total"]) <= 0)
                    {
                        sql = $"DELETE FROM bookData WHERE bookName = \"{b.bookName}\" AND ISBN = \"{b.ISBN}\";";
                        cmd = new SQLiteCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
        }

        public static int countRow()
        {
            string sql = "SELECT COUNT(*) FROM bookData";
            int count = 0;

            using (SQLiteConnection conn = new SQLiteConnection("data source=libDB.sqlite;"))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    conn.Open();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }
    }
}
