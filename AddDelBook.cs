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
    public partial class AddDelBook : Form
    {
        public AddDelBook()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            AddDelBook.ActiveForm.Hide();
        }

        private void AddDelBook_Load(object sender, EventArgs e)
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

        private void addBtn_Click(object sender, EventArgs e)
        {
            try {
                Book b = new Book(nameField.Text, subNameField.Text, writerField.Text, subwriterField.Text, translatorField.Text,
                    Int32.Parse(editionField.Text), Int32.Parse(amountField.Text), publisherField.Text, Int32.Parse(publishedYrBox.Text), ISBNField.Text, deweyField.Text, categoryBox.Text);
                DBCommand.addBook(b);
            }
            catch (FormatException e2)
            {
                if (amountField.Text == "") MessageBox.Show("กรุณากรอกจำนวนเล่ม");
                else if(Int32.Parse(amountField.Text) <= 0) MessageBox.Show("กรุณากรอกจำนวนเล่มที่ถูกต้อง");
                else if(editionField.Text == "") MessageBox.Show("กรุณากรอกครั้งที่พิมพ์");
                else if (Int32.Parse(editionField.Text) <= 0) MessageBox.Show("กรุณากรอกครั้งที่พิมพ์ที่ถูกต้อง");
                else if (publishedYrBox.Text == "") MessageBox.Show("กรุณากรอกปีที่พิมพ์");
                else if (Int32.Parse(publishedYrBox.Text) <= 0) MessageBox.Show("กรุณากรอกปีที่พิมพ์ที่ถูกต้อง");
            }
            
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if(delISBNField.Text == "")
                {
                    MessageBox.Show("กรุณากรอกเลขประจำหนังสือ (ISBN)");
                }
                Book b = new Book(delnameField.Text, "", "", "", "", 0, Int32.Parse(delamountField.Text), "", 0, delISBNField.Text, "", "");
                DBCommand.delBook(b);
            }
            catch (FormatException e2)
            {
                if (delamountField.Text == "") MessageBox.Show("กรุณากรอกจำนวนเล่ม");
                else if (Int32.Parse(delamountField.Text) <= 0) MessageBox.Show("กรุณากรอกจำนวนเล่มที่ถูกต้อง");
            }
        }
    }
}
