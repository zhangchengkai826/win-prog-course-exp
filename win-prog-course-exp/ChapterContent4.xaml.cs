using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for ChapterContent4.xaml
    /// </summary>
    public partial class ChapterContent4 : UserControl
    {
        public class Producer
        {
            public string Name;
            public int Id;
            public Thread Thread;
            public Producer(ChapterContent4 context)
            {
                Id = context.producers.Count + 1;
                Name = string.Format("生产者{0}", Id);
                StartProduce(context);
                context.producers.Add(this);
            }
            public void Produce(object obj)
            {
                var context = obj as ChapterContent4;
                while (true)
                {
                    Thread.Sleep(context.random.Next(context.ProduceSpeed / 2, context.ProduceSpeed / 2 * 3));
                    context.semaphore.WaitOne();
                    context.mutex.WaitOne();
                    context.queue.Enqueue(Id);
                    context.Log.Dispatcher.Invoke(() => { 
                        context.Log.Text += string.Format("{0}\t{1}生产了一个物品\n", DateTime.Now, Name);
                        context.Log.ScrollToEnd();
                    });
                    context.mutex.ReleaseMutex();
                }
            }
            public void StartProduce(ChapterContent4 context)
            {
                context.Log.Text += string.Format("{0}\t{1}开始生产\n", DateTime.Now, Name);
                Thread = new Thread(new ParameterizedThreadStart(Produce));
                Thread.IsBackground = true;
                Thread.Start(context);
            }
        }
        public class Consumer
        {
            public string Name;
            public int Id;
            public Thread Thread;
            public Consumer(ChapterContent4 context)
            {
                Id = context.consumers.Count + 1;
                Name = string.Format("消费者{0}", Id);
                StartConsume(context);
                context.consumers.Add(this);
            }
            public void Consume(object obj)
            {
                var context = obj as ChapterContent4;
                while (true)
                {
                    Thread.Sleep(context.random.Next(context.ConsumerSpeed / 2, context.ConsumerSpeed / 2 * 3));
                    context.mutex.WaitOne();
                    var producerId = context.queue.Dequeue();
                    context.Log.Dispatcher.Invoke(() => {
                        context.Log.Text += string.Format("{0}\t{1}消费了一个物品（来自生产者{2}）\n", DateTime.Now, Name, producerId);
                        context.Log.ScrollToEnd();
                    });
                    context.mutex.ReleaseMutex();
                    context.semaphore.Release();
                }
            }
            public void StartConsume(ChapterContent4 context)
            {
                context.Log.Text += string.Format("{0}\t{1}开始消费\n", DateTime.Now, Name);
                Thread = new Thread(new ParameterizedThreadStart(Consume));
                Thread.IsBackground = true;
                Thread.Start(context);
            }
        }
        public Random random;
        public Mutex mutex;
        public Semaphore semaphore;
        public Queue<int> queue;
        public List<Producer> producers;
        public List<Consumer> consumers;
        public const int MAX_ITEMS_IN_QUEUE = 3;
        public int ProduceSpeed = 800;
        public int ConsumerSpeed = 1200;
        public ChapterContent4()
        {
            InitializeComponent();
            mutex = new Mutex();
            semaphore = new Semaphore(MAX_ITEMS_IN_QUEUE, MAX_ITEMS_IN_QUEUE);
            queue = new Queue<int>();
            producers = new List<Producer>();
            consumers = new List<Consumer>();
            random = new Random();
        }

        private void AddProducer(object sender, RoutedEventArgs e)
        {
            new Producer(this);
        }

        private void AddConsumer(object sender, RoutedEventArgs e)
        {
            new Consumer(this);
        }

        private void PauseAll(object sender, RoutedEventArgs e)
        {
            foreach(var p in producers)
            {
                p.Thread.Suspend();
            }
            foreach (var c in consumers)
            {
                c.Thread.Suspend();
            }
        }

        private void ResumeAll(object sender, RoutedEventArgs e)
        {
            foreach (var p in producers)
            {
                p.Thread.Resume();
            }
            foreach (var c in consumers)
            {
                c.Thread.Resume();
            }
        }
    }
}
