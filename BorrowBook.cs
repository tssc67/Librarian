using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class BorrowBook : Form
    {
        public BorrowBook()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            BorrowBook.ActiveForm.Hide();
        }

        private void BorrowBook_Load(object sender, EventArgs e)
        {
            bookDetail.Rows.Clear();
            string[,] book = new String[DBCommand.countRow(), 10];
            try
            {
                book = DBCommand.searchBook("", "", "", "", "", "", "");
                for (int i = 0; i < book.GetLength(0); i++)
                {
                    bookDetail.Rows.Add();
                    for (int j = 0; j < 10; j++)
                    {
                        bookDetail.Rows[i].Cells[j].Value = book[i, j];
                    }
                }

            }
            catch (FileLoadException e2)
            {
                Console.WriteLine(e2);
                bookDetail.Rows.Add();
                for (int j = 0; j < 10; j++)
                {
                    bookDetail.Rows[0].Cells[j].Value = "N/A";
                }
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            bookDetail.Rows.Clear();
            String[,] book = new String[DBCommand.countRow(), 10];
            try
            {
                book = DBCommand.searchBook(nameField.Text, "", writerField.Text, "", "", publishedYrBox.Text, "");
                for (int i = 0; i < book.GetLength(0); i++)
                {
                    bookDetail.Rows.Add();
                    for (int j = 0; j < 10; j++)
                    {
                        bookDetail.Rows[i].Cells[j].Value = book[i, j];
                    }
                }

            }
            catch (FileLoadException e2)
            {
                Console.WriteLine(e2);
                bookDetail.Rows.Add();
                for (int j = 0; j < 10; j++)
                {
                    bookDetail.Rows[0].Cells[j].Value = "N/A";
                }
            }
        }
    }
}
