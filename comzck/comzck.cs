﻿using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using MsWord = Microsoft.Office.Interop.Word;

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
                var missing = System.Reflection.Missing.Value;
                var app = new MsWord.Application();
                var doc = app.Documents.Add();
                doc.Activate();
                var fileName = openFileDialog.FileName + "\\paper.doc";
                doc.SaveAs(fileName);
                doc.Close();
            }
        }
        public void doTask2()
        {

        }
        public void doTask3()
        {

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
