using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using MsWord = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;

namespace comzck
{
    [Guid("C0993702-DB7B-4D50-A526-3E9887305196")]
    [ComVisible(true)]
    public interface ICOMZck
    {
        void doTask1();
        void doTask2();
        void doTask3();
    }

    [Guid("9D9B759B-55E4-4FFC-B9DF-D7F2230B3439")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    internal class WordCOMZck : ICOMZck
    {
        public void doTask1()
        {
            var openFileDialog = new CommonOpenFileDialog();
            openFileDialog.IsFolderPicker = true;
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var app = new MsWord.Application();
                var doc = app.Documents.Add();
                doc.Activate();

                var abstractSec = doc.Sections.First;
                var abstractFileName = openFileDialog.FileName + "\\abstract.txt";
                abstractSec.Range.Text = File.ReadAllText(abstractFileName);
                var bodySec = doc.Sections.Add();
                var bodyFileName = openFileDialog.FileName + "\\body.txt";
                bodySec.Range.Text = File.ReadAllText(bodyFileName);
                var referenceSec = doc.Sections.Add();
                var referenceFileName = openFileDialog.FileName + "\\reference.txt";
                referenceSec.Range.Text = File.ReadAllText(referenceFileName);

                var fileName = openFileDialog.FileName + "\\paper.doc";
                doc.SaveAs(fileName);
                doc.Close();
                app.Quit();
                Marshal.ReleaseComObject(app);
                MessageBox.Show("Finished!");
            }
        }
        public void doTask2()
        {
            var openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Word Documents", "*.doc"));
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var app = new MsWord.Application();
                var fileName = openFileDialog.FileName;
                var doc = app.Documents.Open(fileName);
                doc.Activate();

                doc.Shapes.AddTextEffect(MsoPresetTextEffect.msoTextEffect1, "Some WordArt", "Comic Sans MS", 28, MsoTriState.msoTrue, MsoTriState.msoTrue, 50, 100);

                doc.Save();
                doc.Close();
                app.Quit();
                Marshal.ReleaseComObject(app);
                MessageBox.Show("Finished!");
            }
        }
        public void doTask3()
        {
            var openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Filters.Add(new CommonFileDialogFilter("Word Documents", "*.doc"));
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var app = new MsWord.Application();
                var fileName = openFileDialog.FileName;
                var doc = app.Documents.Open(fileName);
                doc.Activate();

                foreach(MsWord.Section sec in doc.Sections)
                {
                    switch(sec.Index)
                    {
                        case 1:
                            {
                                sec.Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = "Abstract";
                                break;
                            }
                        case 2:
                            {
                                sec.Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
                                sec.Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = "Body";
                                break;
                            }
                        case 3:
                            {
                                sec.Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].LinkToPrevious = false;
                                sec.Headers[MsWord.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text = "Reference";
                                break;
                            }
                    }
                }

                doc.Save();
                doc.Close();
                app.Quit();
                Marshal.ReleaseComObject(app);
                MessageBox.Show("Finished!");
            }
        }
    }

    [Guid("2FE12FD0-6AF0-4575-9EBB-D6A692FEFF9D")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    internal class ExcelCOMZck : ICOMZck
    {
        public void doTask1()
        {

        }
        public void doTask2()
        {

        }
        public void doTask3()
        {

        }
    }
}
