namespace EventContracts
{
    public interface ValueEntered
    {
        string Value { get; }
    }
}

namespace ConsoleEventListener
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EventContracts;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("event-listener", e =>
                {
                    e.Consumer<EventConsumer>();
                    e.Consumer<EventConsumer2>();
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        class EventConsumer :
            IConsumer<ValueEntered>
        {
            public async Task Consume(ConsumeContext<ValueEntered> context)
            {
                Console.WriteLine("Value: {0} consumer 2 1", context.Message.Value);
            }
        }
        class EventConsumer2 :
                IConsumer<ValueEntered>
        {
            public async Task Consume(ConsumeContext<ValueEntered> context)
            {
                Console.WriteLine("Value: {0} consumer 2 2", context.Message.Value);
            }
        }
    }
}