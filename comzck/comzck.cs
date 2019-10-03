using System;
using System.Runtime.InteropServices;

namespace comzck
{
    [Guid("388d279f-591c-416d-bfa7-24c61c4e041a")]
    [ComVisible(true)]
    public interface ICOMZck
    {
        void doTask1();
        void doTask2();
        void doTask3();
    }

    [Guid("54e45d06-8372-4998-adac-703bcc97bab8")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class WordCOMZck : ICOMZck
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

    [Guid("2be67539-154d-47c8-b040-49833d1535f1")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ExcelCOMZck : ICOMZck
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
