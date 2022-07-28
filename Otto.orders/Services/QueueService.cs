using Otto.orders.Models;

namespace Otto.orders.Services
{
    public class QueueService : BackgroundService
    {
        //Queue<Task> tasks 
        PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        private readonly QueueTasks _queueTasks;

        public QueueService(QueueTasks queueTasks)
        {
            _queueTasks = queueTasks;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested ) 
            {
                // TODO y que la task no este vacia
                if(_queueTasks.Count() > 0)
                    DoWorkAsync(_queueTasks.Dequeue());
            }          
        }

        private async void DoWorkAsync(Task task) 
        {
            try
            {
                //TODO try catch, si hubo error en la task de guardar en la tabla de ordenes pendientes volver a encolar

                task.Start();
                Console.WriteLine(task.IsCompletedSuccessfully);

                await task;

                Console.WriteLine(task.IsCompletedSuccessfully);

                Console.WriteLine("Do workkkk");

            }
            catch (Exception ex)
            {                

                Console.WriteLine($"Error aca: {ex}");

                throw;
            }

        }
    }
}
