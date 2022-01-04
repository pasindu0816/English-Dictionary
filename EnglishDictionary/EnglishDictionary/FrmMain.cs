using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EnglishDictionary
{
    public partial class FrmMain : Form
    {
        public class DictItem
        {
            public string Word { get; set; }
            public string Definition { get; set; }
        }

        public FrmMain()
        {
            InitializeComponent();
            // Load the existing data from the file

            CreateDictionary();
            // Create the datafile.dat
        }

        public void CreateDictionary()
        {
            //string directory = Path.GetDirectoryName(Environment.);
            string directory = Application.StartupPath.Replace("bin\\Debug\\netcoreapp3.1", "Data");

            //string filePath = Path.Combine(Application.StartupPath.Replace("bin\\Debug", "Data"), "datafile.dat");
            string filePath = Path.Combine(directory, "datafile.dat");

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
        }

        public string GetDictionaryPath()
        {

            return Path.Combine(Application.StartupPath.Replace("bin\\Debug\\netcoreapp3.1", "Data"), "datafile.dat");
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            DictItem NewDictItem = new DictItem();
            NewDictItem.Word = TxtWord.Text;
            NewDictItem.Definition = TxtMeaning.Text;

            var dictionary = JsonConvert.DeserializeObject<List<DictItem>>(File.ReadAllText(GetDictionaryPath()));
            if (dictionary == null)
            {
                dictionary = new List<DictItem>();
            }

            // Check if the word already exists
            var existingWord = dictionary?.Where(i => i.Word == NewDictItem.Word).FirstOrDefault();

            // If it exists, check if the meaning already exists
            if (existingWord != null)
            {
                if (existingWord != null && !String.IsNullOrEmpty(existingWord.Definition))
                {
                    // If the meaning already exists, replace it after confirmation
                    DialogResult dialogResult = MessageBox.Show($"Do you want to update the exisiting Meaning of this word?", "Confirmation", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        existingWord.Definition = NewDictItem.Definition;
                    }
                }
                else
                {
                    // If the meaning doesn't exist, add it
                    //DialogResult dialogResult = MessageBox.Show($"Definition of this word doesn't exist!");
                    existingWord.Definition = NewDictItem.Definition;

                }
            }
            else
            {
                // If the word doesn't exist, add it
                //DialogResult dialogResult = MessageBox.Show("Please add definition to continue!");
                dictionary.Add(NewDictItem);
            }

            File.WriteAllText(GetDictionaryPath(), JsonConvert.SerializeObject(dictionary));

        }  

        private void BtnFind_Click(object sender, EventArgs e)
        {
            // Search for the matching words
            var dictionary = JsonConvert.DeserializeObject<List<DictItem>>(File.ReadAllText(GetDictionaryPath()));

            var searchedWord = dictionary?.Where(i => i.Word == i.Word).FirstOrDefault();

            var matchingResult = dictionary?.Where(i => i.Word.Contains(textBox1.Text)).Select(i => i.Definition).ToArray();

            // If it exists, check if the meaning already exists
            // If there are results display them in the list box
            if (searchedWord != null && matchingResult.Length > 0)
            {
                if(matchingResult.Length > 0)
                { 
                    LbDictionaryData.Items.AddRange(matchingResult);

                }

                if (matchingResult.Length == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("The definition of this word doesn't exist in dictionary!. Add the definition by using above section.", "NOTICE");
                }
            }

            else
            {
                DialogResult dialogResult = MessageBox.Show("The word doesn't exist in dictionary!. Add this word and definition by using above section.", "NOTICE");
            }          

        }   

        private void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            var selected = LbDictionaryData.SelectedItem;
            var dictionary = JsonConvert.DeserializeObject<List<DictItem>>(File.ReadAllText(GetDictionaryPath()));
            // Ask for confirmation; Warn for unrecoverability
            DialogResult dialogResult = MessageBox.Show("This action cannot be undone!. Are you sure to delete this word? ", "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // If confirmed delete the word
                dictionary = dictionary?.Where(i => i.Word != selected.ToString()).ToList();
                File.WriteAllText(GetDictionaryPath(), JsonConvert.SerializeObject(dictionary));

            }
        }

        private void BtnListAll_Click(object sender, EventArgs e)
        {
            // Iteratively populate the list box in the ascending order of words
            var dictionary = JsonConvert.DeserializeObject<List<DictItem>>(File.ReadAllText(GetDictionaryPath()))?.OrderBy(i => i.Word)?.Select(i => i.Word)?.ToArray();
            LbDictionaryData.Items.AddRange(dictionary);

        }

        private void BtnDeleteAll_Click(object sender, EventArgs e)
        {
            // Ask for confirmation TWICE; Warn for unrecoverability
            DialogResult dialogResult = MessageBox.Show("This action cannot be undone!. Are you sure to delete all the words permenently?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)

            {
                DialogResult dialogResult2 = MessageBox.Show("Are you sure to delete all words?", "Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult2 == DialogResult.Yes)
                {
                    // If confirmed delete the word
                    File.WriteAllText(GetDictionaryPath(), JsonConvert.SerializeObject(new List<DictItem>()));
                }
            }
        }

        // Implement a timed task to automatically check if there are unsaved changes and save them if there are any
        
        

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Start();
            MessageBox.Show("Item(s) saved!");
        }
    }
}
