namespace Otto.orders.Models
{
    public class QueueTasks
    {
        private readonly Queue<Task> _tasks;
        public QueueTasks()
        {
            _tasks = new Queue<Task>();
        }

        public void Enqueue(Task item) 
        {
            _tasks.Enqueue(item);
        }

        public Task Dequeue()
        {
            return _tasks.Dequeue();
        }

        public int Count() 
        {
            return _tasks.Count;
        }
    }
}
